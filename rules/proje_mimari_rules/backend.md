# Backend Mimari Kuralları

> **DEĞİŞMEZ** — Bu dosyadaki kuralları değiştirmek için ayrı ve özel bir sohbet gereklidir.
> Normal geliştirme akışında bu dosya güncellenmez.

---

## 1. Genel

### 1.1 Clean Architecture Zorunludur

Tüm backend kodları aşağıdaki katman yapısına uymak zorundadır:

```
RestaurantSystem.Domain      → Entity, Enum, Domain Exception, Domain Event
RestaurantSystem.Application → Service Interface, DTO, Validator, AutoMapper Profile
RestaurantSystem.Persistence → DbContext, Repository impl., Migration, Entity Configuration
RestaurantSystem.API         → Controller, Middleware, Filter, Hub, DI kaydı
FrameworkCore                → BaseEntity, BaseRepository, UnitOfWork, InnerResult, JWT, Interceptor
```

### 1.2 Katman Bağımlılık Yönü

- `Domain` hiçbir üst katmanı referans almaz.
- `Application` yalnızca `Domain` ve `FrameworkCore`'u referans alır; `Persistence`'ı bilmez.
- `Persistence` yalnızca `Application` interface'lerini implement eder.
- `API` katmanı `Application`'ı çağırır; `Persistence`'ı doğrudan kullanmaz.

**İhlal edilemez kural:** Controller içinde `DbContext` veya `Repository` doğrudan enjekte edilemez.
Her zaman `IXxxService` interface'i üzerinden erişilir.

### 1.3 Tüm Response'lar InnerResult ile Sarmalanır

FrameworkCore'daki `InnerResult<T>` sarmalayıcısı kullanılır. Uygulama hataları exception
yerine `InnerResult.Fail(message)` ile döndürülür; beklenmedik hatalar global middleware'de yakalanır.

```csharp
// FrameworkCore/Result/InnerResult.cs
public class InnerResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static InnerResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static InnerResult<T> Fail(string message) => new() { IsSuccess = false, Message = message };
}
```

---

## 2. Persistence Katmanı

### 2.1 BaseRepository

Tüm repository'ler `FrameworkCore`'daki `BaseRepository<TEntity, TId>` sınıfından kalıtım alır.
Temel CRUD operasyonları base class'ta tanımlıdır; entity'ye özgü sorgular alt sınıfta eklenir.

```csharp
// FrameworkCore/Persistence/BaseRepository.cs
public abstract class BaseRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id));

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task AddAsync(TEntity entity)
        => await _dbSet.AddAsync(entity);

    public virtual void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;  // güncelleme zamanını otomatik set et
        _dbSet.Update(entity);
    }

    public virtual void SoftDelete(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }
}

// Restoran domain'i için gerçek kullanım örneği:
// Persistence/Repositories/MenuItemRepository.cs
public class MenuItemRepository : BaseRepository<MenuItem, Guid>
{
    public MenuItemRepository(RestaurantDbContext context) : base(context) { }

    // Kategoriye göre aktif ve stokta olan ürünleri getirir
    public async Task<IEnumerable<MenuItem>> GetAvailableItemsByCategoryAsync(Guid categoryId)
        => await _dbSet
            .Include(i => i.Options.Where(o => o.IsActive))
            .Where(i => i.MenuCategoryId == categoryId && i.IsActive && i.IsAvailable)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();

    // Aktif siparişlerde kullanılan ürün silinebilir mi kontrolü
    public async Task<bool> IsUsedInActiveOrderAsync(Guid menuItemId)
        => await _context.Set<OrderItem>()
            .AnyAsync(oi => oi.MenuItemId == menuItemId
                         && oi.Order.Status != OrderStatus.Paid
                         && oi.Order.Status != OrderStatus.Cancelled);
}
```

### 2.2 UnitOfWork

Tüm veritabanı yazma operasyonları `IUnitOfWork` üzerinden commit edilir.
Repository içinde `SaveChanges` çağrılmaz; bu sorumluluk `UnitOfWork`'e aittir.

```csharp
// FrameworkCore/Persistence/IUnitOfWork.cs
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
}

// Kullanım – Application/Services/OrderService.cs
public async Task<OrderDetailResponse> CreateOrderAsync(CreateOrderRequest request)
{
    // ... validasyon ve entity oluşturma ...
    await _orderRepository.AddAsync(order);
    await _unitOfWork.CommitAsync();          // tek commit noktası
    return _mapper.Map<OrderDetailResponse>(order);
}
```

### 2.3 Entity Fluent Configuration

Her entity için ayrı bir `IEntityTypeConfiguration<T>` sınıfı yazılır.
`OnModelCreating` içinde inline konfigürasyon yapılmaz.

```csharp
// Persistence/Configurations/OrderConfiguration.cs
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
            .HasConversion<string>()    // enum veritabanında string olarak saklanır
            .HasMaxLength(20);

        builder.HasOne(o => o.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict); // masa silinse bile sipariş korunur

        builder.HasOne(o => o.Restaurant)
            .WithMany()
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete global query filter
        builder.HasQueryFilter(o => !o.IsDeleted);

        // Multi-tenant global query filter (tenant context servis üzerinden inject edilir)
        // Bkz. TenantQueryFilter middleware belgesi
    }
}

// Persistence/Configurations/MenuItemConfiguration.cs
public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(m => m.MenuCategory)
            .WithMany(c => c.MenuItems)
            .HasForeignKey(m => m.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // kategori silinse de ürün korunur

        builder.HasMany(m => m.Options)
            .WithOne(o => o.MenuItem)
            .HasForeignKey(o => o.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade); // ürün silinince seçenekleri de silinir

        builder.HasQueryFilter(m => !m.IsDeleted);

        // SortOrder için index (hızlı sıralı sorgular)
        builder.HasIndex(m => new { m.MenuCategoryId, m.SortOrder });
    }
}
```

### 2.4 Migration Kuralları

- Migration'lar `Persistence` projesinden `dotnet ef` CLI ile üretilir.
- Migration adı değişikliği açıkça belirtir: `Add{Özellik}`, `Remove{Alan}`, `Rename{Tablo}`.
- Üretim migration'ları geri alınamaz veri kaybına yol açabilecek operasyonlar içeriyorsa
  migration'a açıklayıcı yorum eklenir.
- Seed data `IEntityTypeConfiguration.Configure` içinde `HasData` ile tanımlanır.

```bash
# Yeni migration ekleme
dotnet ef migrations add AddMenuItemPreparationTime \
  --project RestaurantSystem.Persistence \
  --startup-project RestaurantSystem.API

# Migration uygulama
dotnet ef database update \
  --project RestaurantSystem.Persistence \
  --startup-project RestaurantSystem.API
```

---

## 3. Controller Katmanı

### 3.1 Route Convention

- Tüm route'lar `/v1/` prefix'i ile başlar.
- Kaynak adları çoğul ve kebab-case: `/v1/menu-items`, `/v1/order-items`.
- Controller base class'ı `ApiControllerBase` (FrameworkCore'dan), bu class `ControllerBase`'den kalıtım alır.

```csharp
// API/Controllers/OrderController.cs
[ApiController]
[Route("v1/orders")]
[Authorize]  // varsayılan olarak tüm action'lar auth gerektirir
public class OrderController : ApiControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [ServiceFilter(typeof(TenantValidationFilter))]  // tenant context doğrulaması
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query)
    {
        var result = await _orderService.GetOrdersAsync(query);
        return FromInnerResult(result);  // ApiControllerBase helper metodu
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(orderId);
        return FromInnerResult(result);
    }

    [HttpPost]
    [AllowAnonymous]   // QR müşterisi JWT olmadan sipariş verir
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(request);
        return FromInnerResult(result, statusCode: 201);
    }

    [HttpPatch("{orderId:guid}/status")]
    [ServiceFilter(typeof(TenantValidationFilter))]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid orderId,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, request);
        return FromInnerResult(result);
    }
}
```

### 3.2 ServiceFilter Kullanımı

Cross-cutting concern'ler (tenant doğrulama, yetki kontrol vb.) `ServiceFilter` attribute ile
uygulanır. Action içinde tekrarlayan kontrol kodu yazılmaz.

```csharp
// API/Filters/TenantValidationFilter.cs
public class TenantValidationFilter : IAsyncActionFilter
{
    private readonly ICurrentUserService _currentUserService;

    public TenantValidationFilter(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // JWT'den alınan RestaurantId ile tenant context set edilir
        if (!_currentUserService.IsSuperAdmin && _currentUserService.RestaurantId == Guid.Empty)
        {
            context.Result = new UnauthorizedObjectResult("Tenant bilgisi bulunamadı.");
            return;
        }
        await next();
    }
}
```

### 3.3 InnerRequest Pattern

`InnerRequestAttribute` action parametrelerine uygulanarak HTTP context'ten user bilgisi
otomatik inject edilir. Controller action'ı `ICurrentUserService` enjekte etmek zorunda kalmaz.

```csharp
// FrameworkCore/Filters/InnerRequestAttribute.cs
// Bu attribute, action parametresine işaretli sınıfa JWT claim'lerini doldurur

// Kullanım:
public class CreateMenuItemRequest
{
    [JsonIgnore]              // body'den gelmez, filter tarafından set edilir
    public Guid RestaurantId { get; set; }
    [JsonIgnore]
    public Guid RequestingUserId { get; set; }

    // Gerçek body alanları:
    public Guid MenuCategoryId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Controller:
[HttpPost]
public async Task<IActionResult> CreateMenuItem(
    [InnerRequest][FromBody] CreateMenuItemRequest request)
    // InnerRequest filter, request.RestaurantId ve request.RequestingUserId'yi JWT'den doldurur
{
    var result = await _menuService.CreateItemAsync(request);
    return FromInnerResult(result, statusCode: 201);
}
```

### 3.4 FromHttpClientResult Pattern

Başka bir iç servise (FileAPI, TokenService vb.) HTTP çağrısı yapıldığında
`HttpClientWrapper` üzerinden `InnerResult<T>` ile sarmalı yanıt alınır.

```csharp
// API/Controllers/TableController.cs – QR görsel üretimi için FileAPI çağrısı
[HttpPost("{tableId:guid}/qr-code")]
public async Task<IActionResult> GenerateQrCode(Guid tableId)
{
    // 1. QR token oluştur ve veritabanına kaydet
    var qrResult = await _tableService.GenerateQrCodeAsync(tableId);
    if (!qrResult.IsSuccess) return FromInnerResult(qrResult);

    // 2. FileAPI'ya QR görsel üretim isteği at
    var fileResult = await _fileApiClient.GenerateQrImageAsync(qrResult.Data!.Token);
    if (!fileResult.IsSuccess) return FromInnerResult(fileResult);

    // 3. QR kaydını görsel URL ile güncelle
    var updateResult = await _tableService.UpdateQrCodeImageUrlAsync(
        tableId, fileResult.Data!.Url);

    return FromInnerResult(updateResult);
}
```

---

## 4. Service Katmanı

### 4.1 Autofac Interceptor ile Transaction ve Logging

Servis metotları `[Transactional]` ve `[Loggable]` attribute'leri ile işaretlenir.
Autofac interceptor'ları bu attribute'leri tanıyarak AOP yaklaşımıyla transaction yönetimi
ve loglama gerçekleştirir. Service implementasyonunda elle try-catch veya transaction kodu yazılmaz.

```csharp
// Application/Services/OrderService.cs
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint; // MassTransit

    public OrderService(
        IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [Transactional]   // Autofac interceptor – commit/rollback otomatik yönetilir
    [Loggable]        // Autofac interceptor – metot giriş/çıkış ve hata loglanır
    public async Task<InnerResult<OrderDetailResponse>> CreateOrderAsync(CreateOrderRequest request)
    {
        // QR token doğrulama ve masa resolve
        var qrCode = await _qrCodeRepository.GetActiveByTokenAsync(request.QrToken);
        if (qrCode == null)
            return InnerResult<OrderDetailResponse>.Fail("Geçersiz veya süresi dolmuş QR kodu.");

        // Sipariş oluştur
        var order = new Order
        {
            Id = Guid.NewGuid(),
            RestaurantId = qrCode.RestaurantId,
            TableId = qrCode.TableId,
            OrderCode = GenerateOrderCode(),
            Status = OrderStatus.Pending,
            CustomerNote = request.CustomerNote
        };

        // Kalemler ve snapshot'lar
        decimal total = 0;
        foreach (var itemReq in request.Items)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(itemReq.MenuItemId);
            if (menuItem == null || !menuItem.IsAvailable)
                return InnerResult<OrderDetailResponse>.Fail($"Ürün mevcut değil: {itemReq.MenuItemId}");

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                MenuItemId = menuItem.Id,
                MenuItemName = menuItem.Name,       // snapshot
                UnitPrice = menuItem.Price,          // snapshot
                Quantity = itemReq.Quantity,
                Note = itemReq.Note
            };
            // ... seçenekler eklenir, total hesaplanır ...
            total += orderItem.TotalPrice;
        }

        order.TotalAmount = total;
        await _orderRepository.AddAsync(order);
        await _unitOfWork.CommitAsync();

        // Event yayımla (RabbitMQ → Panel SignalR bildirimi)
        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            RestaurantId = order.RestaurantId,
            TableId = order.TableId,
            OrderCode = order.OrderCode
        });

        return InnerResult<OrderDetailResponse>.Success(
            _mapper.Map<OrderDetailResponse>(order));
    }

    [Transactional]
    [Loggable]
    public async Task<InnerResult<OrderResponse>> UpdateOrderStatusAsync(
        Guid orderId, UpdateOrderStatusRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return InnerResult<OrderResponse>.Fail("Sipariş bulunamadı.");

        // Domain kuralı: geçersiz durum geçişi kontrolü
        if (!order.CanTransitionTo(request.NewStatus))
            return InnerResult<OrderResponse>.Fail(
                $"{order.Status} → {request.NewStatus} geçişi geçersiz.");

        order.Status = request.NewStatus;
        // Zaman damgalarını set et
        switch (request.NewStatus)
        {
            case OrderStatus.Accepted:  order.AcceptedAt  = DateTime.UtcNow; break;
            case OrderStatus.Ready:     order.PreparedAt  = DateTime.UtcNow; break;
            case OrderStatus.Delivered: order.DeliveredAt = DateTime.UtcNow; break;
            case OrderStatus.Paid:      order.PaidAt      = DateTime.UtcNow; break;
            case OrderStatus.Cancelled:
                order.CancelReason = request.CancelReason;
                break;
        }

        _orderRepository.Update(order);
        await _unitOfWork.CommitAsync();

        await _publishEndpoint.Publish(new OrderStatusChangedEvent
        {
            OrderId = order.Id,
            RestaurantId = order.RestaurantId,
            NewStatus = order.Status
        });

        return InnerResult<OrderResponse>.Success(_mapper.Map<OrderResponse>(order));
    }

    private static string GenerateOrderCode()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}
```

### 4.2 AutoMapper Profilleri

Her domain için ayrı bir `Profile` sınıfı tanımlanır. Controller veya service içinde
elle mapping (`new XxxResponse { ... }`) yapılmaz.

```csharp
// Application/Mappings/OrderMappingProfile.cs
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Entity → Response
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.TableName,
                opt => opt.MapFrom(src => src.Table.Name))
            .ForMember(dest => dest.StatusDisplay,
                opt => opt.MapFrom(src => src.Status.ToDisplayString())); // extension metot

        CreateMap<Order, OrderDetailResponse>()
            .IncludeBase<Order, OrderResponse>();

        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => src.OrderItemOptions));

        CreateMap<OrderItemOption, OrderItemOptionResponse>();

        // Request → Entity (sadece oluşturma için; güncelleme service içinde yapılır)
        CreateMap<CreateOrderItemRequest, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemName, opt => opt.Ignore()) // service'de set edilir
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore());   // service'de set edilir
    }
}

// Application/Mappings/MenuMappingProfile.cs
public class MenuMappingProfile : Profile
{
    public MenuMappingProfile()
    {
        CreateMap<MenuItem, MenuItemResponse>();
        CreateMap<MenuItem, MenuItemDetailResponse>()
            .IncludeBase<MenuItem, MenuItemResponse>();
        CreateMap<MenuItemOption, MenuItemOptionResponse>();
        CreateMap<MenuCategory, MenuCategoryResponse>();
        CreateMap<MenuCategory, MenuCategoryDetailResponse>()
            .IncludeBase<MenuCategory, MenuCategoryResponse>();
        CreateMap<CreateMenuItemRequest, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}
```

---

## 5. Auth

### 5.1 JWT Bearer

JWT doğrulaması `Program.cs`'de yapılandırılır. Token içindeki claim'ler:

```csharp
// FrameworkCore/Auth/TokenClaims.cs
public static class TokenClaims
{
    public const string UserId       = "userId";
    public const string RestaurantId = "restaurantId";
    public const string Email        = ClaimTypes.Email;
    public const string Roles        = ClaimTypes.Role;
}

// API/Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
        // SignalR için query string'den token okuma
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });
```

### 5.2 ICurrentUserService

Controller ve service'lerde kullanıcı bilgisine `ICurrentUserService` üzerinden erişilir.
`HttpContext` doğrudan inject edilmez.

```csharp
// Application/Interfaces/ICurrentUserService.cs
public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid RestaurantId { get; }
    string Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
}

// API/Services/CurrentUserService.cs
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        Guid.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(TokenClaims.UserId) ?? Guid.Empty.ToString());

    public Guid RestaurantId =>
        Guid.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(TokenClaims.RestaurantId) ?? Guid.Empty.ToString());

    public bool IsSuperAdmin =>
        _httpContextAccessor.HttpContext!.User
            .IsInRole("SuperAdmin");
}
```

### 5.3 InnerRequestAttribute

Bu attribute, action parametresindeki `RestaurantId` ve `RequestingUserId` alanlarını
JWT'den otomatik doldurur; action'a `[FromBody]` ile birlikte uygulanır.

```csharp
// Kullanım örneği – API/Controllers/MenuController.cs
[HttpPost]
[ServiceFilter(typeof(TenantValidationFilter))]
public async Task<IActionResult> CreateMenuItem(
    [InnerRequest][FromBody] CreateMenuItemRequest request)
    // request.RestaurantId → JWT'den otomatik set edilir
    // request.RequestingUserId → JWT'den otomatik set edilir
{
    var result = await _menuService.CreateItemAsync(request);
    return FromInnerResult(result, statusCode: 201);
}
```

---

## 6. Event-Driven Mimari (MassTransit + RabbitMQ)

### 6.1 Event Tanımlama

Events `Application` katmanında tanımlanır; hem publisher hem consumer bu katmanı referans alır.

```csharp
// Application/Events/OrderCreatedEvent.cs
public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid RestaurantId { get; init; }
    public Guid TableId { get; init; }
    public string OrderCode { get; init; } = default!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

// Application/Events/OrderStatusChangedEvent.cs
public record OrderStatusChangedEvent
{
    public Guid OrderId { get; init; }
    public Guid RestaurantId { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
}

// Application/Events/OrderPaidEvent.cs
public record OrderPaidEvent
{
    public Guid OrderId { get; init; }
    public Guid RestaurantId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime PaidAt { get; init; } = DateTime.UtcNow;
}
```

### 6.2 MassTransit Konfigürasyonu

```csharp
// API/Program.cs
builder.Services.AddMassTransit(x =>
{
    // Consumer'ları tara ve kaydet
    x.AddConsumers(typeof(OrderNotificationConsumer).Assembly);

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ConfigureEndpoints(ctx);

        // Retry policy: 3 deneme, 5'er saniye aralıkla
        cfg.UseMessageRetry(r => r.Intervals(
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(5)));
    });
});
```

### 6.3 Consumer Örneği

```csharp
// API/Consumers/OrderNotificationConsumer.cs
// Yeni sipariş geldiğinde Panel'e SignalR bildirimi gönderir
public class OrderNotificationConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly INotificationService _notificationService;

    public OrderNotificationConsumer(
        IHubContext<OrderHub> hubContext,
        INotificationService notificationService)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var evt = context.Message;

        // Panel grubuna SignalR bildirimi gönder
        await _hubContext.Clients
            .Group($"restaurant-{evt.RestaurantId}")
            .SendAsync("OrderCreated", new
            {
                evt.OrderId,
                evt.OrderCode,
                evt.TableId,
                evt.CreatedAt
            });

        // Notification tablosuna kalıcı kayıt ekle
        await _notificationService.CreateAsync(new CreateNotificationRequest
        {
            RestaurantId = evt.RestaurantId,
            Type = NotificationType.NewOrder,
            Title = "Yeni Sipariş",
            Message = $"{evt.OrderCode} kodlu yeni sipariş geldi.",
            Payload = System.Text.Json.JsonSerializer.Serialize(new { evt.OrderId })
        });
    }
}
```

---

## 7. Docker

### 7.1 Her Servis Kendi Dockerfile'ına Sahiptir

```dockerfile
# RestaurantSystem.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve bağımlılıkları yükle
COPY ["RestaurantSystem.API/RestaurantSystem.API.csproj",           "RestaurantSystem.API/"]
COPY ["RestaurantSystem.Application/RestaurantSystem.Application.csproj", "RestaurantSystem.Application/"]
COPY ["RestaurantSystem.Domain/RestaurantSystem.Domain.csproj",     "RestaurantSystem.Domain/"]
COPY ["RestaurantSystem.Persistence/RestaurantSystem.Persistence.csproj", "RestaurantSystem.Persistence/"]
RUN dotnet restore "RestaurantSystem.API/RestaurantSystem.API.csproj"

# Kaynak kodları kopyala ve build et
COPY . .
WORKDIR "/src/RestaurantSystem.API"
RUN dotnet build "RestaurantSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestaurantSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestaurantSystem.API.dll"]
```

### 7.2 docker-compose.yml

```yaml
# docker-compose.yml – tüm servisler tek compose dosyasında yönetilir
version: '3.8'

services:
  restaurant-api:
    build:
      context: .
      dockerfile: RestaurantSystem.API/Dockerfile
    container_name: restaurant-api
    ports:
      - "5100:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Default=Server=sqlserver;Database=RestaurantSystemDb;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True
      - Jwt__SecretKey=${JWT_SECRET}
      - Jwt__Issuer=RestaurantSystem
      - Jwt__Audience=RestaurantSystemClients
      - RabbitMQ__Host=rabbitmq
      - RabbitMQ__Username=guest
      - RabbitMQ__Password=guest
    depends_on:
      - sqlserver
      - rabbitmq
    networks:
      - restaurant-net

  file-api:
    build:
      context: .
      dockerfile: FileAPI/Dockerfile
    container_name: file-api
    ports:
      - "5200:80"
    volumes:
      - file-storage:/app/uploads   # dosya depolama volume
    networks:
      - restaurant-net

  notification-service:
    build:
      context: .
      dockerfile: NotificationService/Dockerfile
    container_name: notification-service
    ports:
      - "5300:80"
    environment:
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - rabbitmq
    networks:
      - restaurant-net

  worker-service:
    build:
      context: .
      dockerfile: WorkerService/Dockerfile
    container_name: worker-service
    environment:
      - RabbitMQ__Host=rabbitmq
      - ConnectionStrings__Default=Server=sqlserver;Database=RestaurantSystemDb;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True
    depends_on:
      - sqlserver
      - rabbitmq
    networks:
      - restaurant-net

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${SQL_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - restaurant-net

  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    ports:
      - "5672:5672"    # AMQP portu
      - "15672:15672"  # yönetim paneli
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest
    networks:
      - restaurant-net

volumes:
  sqlserver-data:
  file-storage:

networks:
  restaurant-net:
    driver: bridge
```

### 7.3 Environment Değişkenleri

Hassas bilgiler (şifre, JWT secret) `.env` dosyasında tutulur, source control'e eklenmez.

```bash
# .env.example – kayıt altına alınan örnek; gerçek değerler .env dosyasında
SQL_PASSWORD=YourStrong@Passw0rd
JWT_SECRET=your-256-bit-secret-key-minimum-32-characters
```
