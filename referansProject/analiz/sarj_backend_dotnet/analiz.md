# sarj_backend_dotnet — Detayli Mimari Analiz
Orijinal: `E:\Projeler\Backend\RotaWattBackEnd`

---

## 1. Platform & Tech Stack

| Katman | Teknoloji |
|--------|-----------|
| Framework | ASP.NET Core (C#) |
| ORM | Entity Framework Core (EFCore) |
| DB | MSSQL Server (SQL Server + NetTopologySuite) |
| DI Container | Autofac (ConfigureContainer pattern) |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| API Versioning | URL segment versioning (`/v{version}/controller/action`) |
| Message Broker | MassTransit + RabbitMQ |
| HTTP Client | Typed HttpClient + Polly (3 retry, 600ms araliklarla) |
| Bulk Operations | EFCore.BulkExtensions |
| API Gateway | Ocelot (GateWay.Api) |
| Realtime | SignalR (UseSignalRBuilder) |
| Error Handling | Hellang.Middleware.ProblemDetails |
| Swagger | Swashbuckle + JWT destegi |
| Container | Docker / docker-compose |

---

## 2. Mimari Aciklama

### Katmanlar (Clean Architecture benzeri yapı)

```
RotaWattBackEnd/
├── Framework/
│   └── Core/
│       └── FrameworkCore/              <- Ozel framework katmani
│           ├── Bases/
│           │   ├── BaseRepository/     <- Generik repository base
│           │   ├── BaseServices/       <- AutoMapper inject base
│           │   ├── BaseUnitOfWork/     <- DbContext + repo factory
│           │   ├── BaseEntities/       <- IEntity, BaseEntity
│           │   └── StartupBase/        <- Startup, DI extensions
│           └── FrameworkCore/
│               └── WrapperCore/        <- Result<T> wrapper sistemi
└── src/
    ├── Presentation/                   <- API katmanlari
    │   ├── Bank.Api/                   <- Odeme API
    │   ├── GateWay.Api/                <- Ocelot API Gateway
    │   ├── Mobil.Api/                  <- Mobil kullanici API
    │   ├── Notification.Api/           <- SignalR bildirimleri
    │   ├── Integration.Api/            <- Odeme entegrasyonlari (Moka vb)
    │   ├── Tocken.Api/                 <- JWT token servisi
    │   ├── Station.Api/                <- Sarj istasyonu API
    │   ├── Ocpp.Api/                   <- OCPP protokol
    │   ├── Vm.Api/                     <- Sanal makine proxy
    │   └── WorkerService/              <- Background worker
    ├── Core/
    │   ├── Applications/               <- Business logic (service) katmani
    │   │   └── Bank.Application/
    │   └── Persistences/               <- Repository + DbContext katmani
    │       └── Bank.Persistence/
    └── Shared/
        └── Shared.Domain/              <- Ortak entity, DTO, interface, enum
```

### Temel Prensip
- Her API kendi `Startup.cs`, `DbContext`, `Repository` ve `Application Service` katmanina sahip
- Tüm API'lar `Framework/Core/FrameworkCore` katmanindaki baz class'lari kullanir
- `Shared.Domain` entity, DTO, service interface, repository interface, enum, hata enumlarini barindiriyor
- DI kayitlari Autofac `ConfigureContainer` ile yapiliyor, assembly scan ile otomatik

---

## 3. Gercek Kod Bloklari

### BaseRepository.cs

```csharp
// Framework/Core/FrameworkCore/Bases/BaseRepository/BaseRepository.cs

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly UnitOfWork _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(IUnitOfWork dbContext)
    {
        _dbContext = dbContext == null ? throw new ArgumentNullException(nameof(dbContext)) : dbContext as UnitOfWork;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public virtual TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll)
        => _dbSet.Add(entity).Entity;

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var res = await _dbSet.AddAsync(entity, cancellationToken);
        return res.Entity;
    }

    public virtual void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll)
        => _dbSet.Update(entity);

    // Soyut - ConnectedRepository implement eder
    public abstract void Delete(object id);
    public abstract void DeleteWithState(object id);
    public abstract void DeleteWithStateRange(IEnumerable<TEntity> entities);
    public abstract void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties);
    public abstract void UpdateWithPropertiesForProperty(TEntity entity, Expression<Func<TEntity, object>>[] properties);

    // IQueryable query builder
    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;
        query = SetTracking(query, tracking);
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);
        return orderBy != null ? orderBy(query) : query;
    }

    // Sayfalama
    public async Task<object[]> GetPagedAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Expression<Func<TEntity, object>>[] include = null,
        Expression<Func<TEntity, bool>> predicate = null,
        int? page = 0, int? pageSize = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (predicate != null) query = query.Where(predicate);
        if (include != null && include.Any())
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
        if (orderBy != null) query = orderBy(query);
        else throw new ArgumentNullException("The order by is necessary in Pagining");

        var dataCount = query.Count();
        if (page != null && page > 0)
            query = query.Skip(((int)page - 1) * (int)pageSize);
        if (pageSize != null) query = query.Take((int)pageSize);

        return new object[] { await query.ToListAsync(), dataCount };
    }

    // Aggregation: Count, Sum, Max, Min, Average
    public int Count(Expression<Func<TEntity, bool>> predicate = null)
        => predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
    public decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        => predicate == null ? _dbSet.Sum(selector) : _dbSet.Where(predicate).Sum(selector);

    // Abstract - tracking davranisi alt sinif belirler
    protected abstract IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking);
    public abstract IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities);

    // IQueryable impl - SetTracking uygulayarak
    public Type ElementType => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).ElementType;
    public Expression Expression => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Expression;
    public IQueryProvider Provider => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Provider;
}
```

### ConnectedRepository.cs

```csharp
// Framework/Core/FrameworkCore/Bases/BaseRepository/ConnectedRepository.cs

public class ConnectedRepository<TEntity> : BaseRepository<TEntity> where TEntity : class, IEntity
{
    public ConnectedRepository(IUnitOfWork dbContext) : base(dbContext) { }

    // Soft delete - Deleted flag = true, fiziksel silme yok
    public override void DeleteWithState(object id)
    {
        var entity = _dbSet.Find(id);
        entity.Deleted = true;      // DIKKAT: null check'ten once!
        if (entity != null) Update(entity);
    }

    public override void DeleteWithStateRange(IEnumerable<TEntity> entities)
    {
        entities.ToList().ForEach(item => { item.Deleted = true; });
        Update(entities);
    }

    // Partial update - sadece belirtilen property'ler guncellenir
    public override void UpdateWithProperties(TEntity entity, Expression<Func<TEntity, object>>[] properties)
    {
        _dbContext.Entry(entity).State = EntityState.Unchanged;
        foreach (var property in properties)
        {
            try { _dbContext.Entry(entity).Property(property).IsModified = true; }
            catch { _dbContext.Entry(entity).Reference(property).IsModified = true; }
        }
    }

    // UpdateWithPropertiesForProperty: once tüm property'leri false, sonra belirtilenleri true
    public override void UpdateWithPropertiesForProperty(TEntity entity, Expression<Func<TEntity, object>>[] properties)
    {
        _dbContext.Entry(entity).Properties.ToList().ForEach(item => { item.IsModified = false; });
        foreach (var property in properties)
            _dbContext.Entry(entity).Property(property).IsModified = true;
    }

    // EFCore.BulkExtensions ile toplu islemler
    public async Task AddBulkAsync(TEntity[] entities) => await _dbContext.BulkInsertAsync(entities);
    public async Task UpdateBulkAsync(TEntity[] entities) => await _dbContext.BulkUpdateAsync(entities);
    public async Task DeleteBulkAsync(TEntity[] entities) => await _dbContext.BulkDeleteAsync(entities);

    public override IQueryable<TEntity> GetAllNotDeleted()
        => _dbSet.Where(t => t.Deleted == false);

    protected override IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking)
    {
        if (tracking == TrackingBehaviour.AsNoTracking) query = query.AsNoTracking();
        return query;
    }

    // Expression tree ile dinamik siralama - nested property destegi (e.g. "User.Name")
    public override IQueryable<TEntity> ApplySorting(string property, string directive, IQueryable<TEntity> entities)
    {
        if (!string.IsNullOrEmpty(directive) && !string.IsNullOrEmpty(property))
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var body = property.Split('.').Aggregate((Expression)parameter, Expression.Property);
            if (body.Type.IsValueType) body = Expression.Convert(body, typeof(object));
            var selector = Expression.Lambda<Func<TEntity, object>>(body, parameter);
            return directive == "asc" ? entities.OrderBy(selector) : entities.OrderByDescending(selector);
        }
        return entities.OrderBy(i => i.Id);
    }
}
```

### BaseService.cs

```csharp
// Framework/Core/FrameworkCore/Bases/BaseServices/BaseService.cs
public class BaseService
{
    protected IMapper _mapper;
    public BaseService(IMapper mapper) { _mapper = mapper; }
}
```

### UnitOfWork.cs

```csharp
// Framework/Core/FrameworkCore/Bases/BaseUnitOfWork/UnitOfWork.cs

public abstract class UnitOfWork : DbContext, IUnitOfWork
{
    private Dictionary<Type, object> repositories;

    public UnitOfWork([NotNull] DbContextOptions dbContextOptions) : base(dbContextOptions) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);  // SQL loglamasi - production'da ILogger kullanilmali
        base.OnConfiguring(optionsBuilder);
    }

    // Generic repository factory - singleton-like caching
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
    {
        if (repositories == null) repositories = new Dictionary<Type, object>();
        var type = typeof(TEntity);
        if (!repositories.ContainsKey(type))
            repositories[type] = new ConnectedRepository<TEntity>(this);
        return (IRepository<TEntity>)repositories[type];
    }

    // Activator ile ozel repository olusturma
    public TRepo GetOwnRepository<TRepo, TEntity>()
        => (TRepo)Activator.CreateInstance(typeof(TRepo), new object[] { this });

    public override int SaveChanges()
    {
        BeforeSave(DateTime.Now);
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        BeforeSave(DateTime.Now);
        return await base.SaveChangesAsync();
    }

    private void BeforeSave(DateTime timestamp)
    {
        ChangeTracker.DetectChanges();  // Degisiklikleri tespit et
    }
}
```

### WrapperCore — Result<T> sistemi

```csharp
// Result.cs — base
public class Result<T>
{
    public virtual ResultType ResultType { get; set; } = ResultType.Error;
    public virtual int ErrorCode { get; set; }
    public virtual string ErrorMessage { get; set; }
    public virtual T Data { get; set; }
}

// ResultType.cs
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultType { Ok, Error, ValidationError, Unauthorized, Exception, VersionInvalid }

// SuccessResult.cs
public class SuccessResult<T> : Result<T>
{
    private readonly T _data;
    public SuccessResult(T data) : base() { _data = data; ErrorCode = 0; ErrorMessage = ""; }
    public override ResultType ResultType => ResultType.Ok;
    public override T Data => _data;
}

// ErrorResult.cs — Enum hatasi veya string mesaj ile
public class ErrorResult<T> : Result<T>
{
    private readonly T? _data;
    public ErrorResult(T? data, Enum error) : base()
    {
        _data = data;
        ErrorCode = (int)((object)error);
        ErrorMessage = error.ToDescriptionString();  // Enum Description attribute okunuyor
    }
    public ErrorResult(T? data, string error) : base() { _data = data; ErrorMessage = error; }
    public override ResultType ResultType => ResultType.Error;
}

// ResultExtensions.cs — Controller extension metotlari
public static class ResultExtensions
{
    // Data dogrudan döner (sarmalama yok)
    public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
        => result.ResultType == ResultType.Ok ? controller.Ok(result.Data) : controller.BadRequest(result);

    // Tüm Result sarmalaniyla döner (HttpClient proxy pattern)
    public static ActionResult FromHttpClientResult<T>(this ControllerBase controller, Result<T> result)
        => controller.Ok(result);

    // Mobil: her zaman Result sarmalaniyla, hata durumunda BadRequest
    public static ActionResult FromMobilResult<T>(this ControllerBase controller, Result<T> result)
        => result.ResultType == ResultType.Ok ? controller.Ok(result) : controller.BadRequest(result);
}
```

### BaseStartup.cs + ApiServiceBaseCollectionExtensions.cs (secme kisimlar)

```csharp
// BaseStartup.cs
public class BaseStartup
{
    public ApiOptions ApiOptions { get; set; }
    public string ProjectPrefix;

    public BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
    {
        ApiOptions = new ApiOptions()
        {
            ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
            // Assembly scan: ProjectPrefix ("Bank", "Vm" vb.) veya "Shared" ile baslayan DLL'ler
            RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
               .Where(filePath => Path.GetFileName(filePath).StartsWith(ProjectPrefix) || Path.GetFileName(filePath).StartsWith("Shared"))
               .Select(Assembly.LoadFrom)
        };
    }

    public Action<DbContextOptionsBuilder> GetDbContextOption(UsingDbType dbType, string connection, string migrationassembly)
    {
        if (dbType == UsingDbType.MSSQL)
            return dbBuilder => dbBuilder.UseSqlServer(connection,
                b => b.MigrationsAssembly(migrationassembly).UseNetTopologySuite());
        return null;
    }

    public void ConfigureBuilderInit(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseCors("_myAllowSpecificOrigins");
        app.UseRouting();
    }
}

// ApiServiceBaseCollectionExtensions.cs — temel kayitlar
public static IServiceCollection AddRotaWattApiService(this IServiceCollection services, ...)
{
    // URL versioning: /v1/Controller/Action
    services.AddApiVersioning(options => {
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.Conventions.Add(new VersionByNamespaceConvention());
    });
    // Polly retry: 3 deneme, 600ms aralikla
    services.AddHttpClient("MyGenericClient", config => config.Timeout = new TimeSpan(0, 0, 30))
        .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, n => TimeSpan.FromMilliseconds(600)));
    // ProblemDetails: iş kuralı ve validasyon hataları
    services.AddProblemDetails(x => {
        x.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
        x.Map<ValidationException>(ex => new ValidationExceptionProblemDetails(ex));
    });
    // CORS: tum origin, method, header izinli
    services.AddCors(options => options.AddPolicy("_myAllowSpecificOrigins",
        builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build()));
    // DateTime custom serializer
    services.AddControllers().AddJsonOptions(opt =>
        opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter()));
    return services;
}
```

### Bank.Api/Startup.cs (maskeli credential)

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
        base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");  // "Bank"
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. MSSQL DbContext kaydi
        var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
        dbcontextOptions.Add(GetDbContextOption(UsingDbType.MSSQL,
            GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),  // [MASKED]
            GetAppSettingValue("StartupConfigs:MigrationAssembly")));         // "Bank.Persistence"
        services.AddRotaWattDbService<PaymentDbContext>(dbcontextOptions);

        // 2. API altyapisi (versioning, cors, swagger, polly, problem details, fluent validation)
        services.AddRotaWattApiService(Configuration, WebHostEnvironment, ...);

        // 3. AutoMapper — assembly scan ile profil kaydi
        services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);

        // 4. Action filter DI kaydi
        services.AddFilters();

        // 5. Framework utility servisleri (IUtilService, IGenericHttpClientService)
        services.AddFrameworkServices();

        // 6. RabbitMQ/MassTransit
        services.RegisterMasstransit();
    }

    // Autofac DI container kayitlari (assembly scan)
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureRepositories(ApiOptions);  // IXxxRepository -> XxxRepository
        builder.ConfigureServices(ApiOptions);       // IXxxService -> XxxService
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);              // CORS, Routing
        app.UseSwaggerBuilder(provider, ApiOptions); // Swagger UI
        app.UseSignalRBuilder(provider, ApiOptions); // SignalR hub
        app.UseErrorBuilder(provider, ApiOptions);   // Global error handling
    }
}
```

### PaymentController.cs

```csharp
[ApiController]
[Produces("application/json")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    // Cuzdandan odeme
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]  // Request/response log
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]       // Sadece MOBIL API cagirir
    public async Task<IActionResult> PaymentWallet(PaymentWalletRequestDto paymentWalletRequest)
    {
        var result = await _paymentService.PaymentWallet(paymentWalletRequest);
        return this.FromHttpClientResult(result);  // Result<T> sarmalaniyla Ok()
    }

    // Direkt kredi karti odeme
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
    public async Task<IActionResult> PaymentDirectDebitCard(PaymentDirectDebitCardRequestDto request)
    {
        var result = await _paymentService.PaymentDirectDebitCard(request);
        return this.FromHttpClientResult(result);
    }

    // 3D odeme tamamlama - Integration API tarafindan cagirilir
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.INTEGRATION })]
    public async Task<IActionResult> PaymentCompleteDebitCard3D(PaymentCompleteDebitCard3DRequestDto request)
    {
        var result = await _paymentService.PaymentCompleteDebitCard3D(request);
        return this.FromHttpClientResult(result);
    }
}
```

### PaymentService.cs (kisa ozet — tam dosya 946 satir)

```csharp
public class PaymentService : BaseService, IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPaymentIntegrationClientService _paymentIntegrationClientService;
    private readonly ICreateArchiveAndInvoiceRequestProducer _createArchiveRequestProducer;
    // + IPaymentCallbackDataRepository, IWalletSpendMoneyRepository,
    //   IPaymentNotificationClientService, IPaymentInfoClientService, IUtilService...

    public async Task<Result<PaymentWalletResponseDto>> PaymentWallet(PaymentWalletRequestDto request)
    {
        var response = new PaymentWalletResponseDto();

        // 1. Duplicate odeme kontrolu
        if (request.PaymentReason == PaymentReasonEnum.CHARGE)
        {
            var existing = await _paymentRepository.GetPaymentAsNoTracking(
                new PaymentFilterDto { ChargeGuiId = request.ChargeGuiId, PaymentStatus = PaymentStatusEnum.SUCCESSFUL })
                .FirstOrDefaultAsync();
            if (existing != null) return new ErrorResult<...>(response, PaymentErrorEnum.PAYMENT_WAS_COMPLETED);
        }

        // 2. Cüzdan bul ve bakiye kontrol et
        var wallet = await _walletRepository.GetWallet(...).FirstOrDefaultAsync();
        if (wallet == null) return new ErrorResult<...>(response, PaymentErrorEnum.WALLET_NOT_FOUND);
        if (walletAmount < amount) return new ErrorResult<...>(response, PaymentErrorEnum.WALLET_AMOUNT_INSUFFICIENT);

        // 3. Partial update ile cüzdan guncelle
        _walletRepository.UpdateWithProperties(wallet, new Expression<Func<Wallet, object>>[] {
            s => s.ProcessKey, s => s.WalletAmount, s => s.AmountTockenGuiId,
        });
        wallet.WalletAmount = wallet.WalletAmount - amount;

        // 4. Payment kaydet ve SaveChanges
        await _paymentRepository.InsertAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        // 5. RabbitMQ ile fatura komutu gonder
        _createArchiveRequestProducer.archiveAndInvoiceCreateRequestCommand(new CreateArchiveAndInvoiceRequestCommand { ... });

        return new SuccessResult<PaymentWalletResponseDto>(response);
    }
}
```

### PaymentRepository.cs

```csharp
public class PaymentRepository : ConnectedRepository<Payment>, IPaymentRepository
{
    private PaymentDbContext _appDbContext { get => _dbContext as PaymentDbContext; }

    public PaymentRepository(IUnitOfWork dbContext) : base(dbContext) { }

    // IQueryable doner - query chain disarida tamamlanir
    public IQueryable<Payment> GetPayment(PaymentFilterDto paymentFilter)
    {
        return _appDbContext.Payment
            .Include(p => p.PaymentCallbackData)
            .Include(p => p.WalletSpendMoney)
            .Where(GetPaymentPredicate(paymentFilter))
            .AsSplitQuery();  // N+1'i onler
    }

    public IQueryable<Payment> GetPaymentAsNoTracking(PaymentFilterDto paymentFilter)
        => GetPayment(paymentFilter).AsNoTracking();

    // Dinamik predicate builder - null olmayan filterler ekleniyor
    private Expression<Func<Payment, bool>> GetPaymentPredicate(PaymentFilterDto filter)
    {
        Expression<Func<Payment, bool>> predicate = p => !p.Deleted;
        if (filter.Id != null)           predicate = predicate.And(p => p.Id == filter.Id);
        if (filter.PaymentGuiId != null) predicate = predicate.And(p => p.GuiId == filter.PaymentGuiId);
        if (filter.ChargeGuiId != null)  predicate = predicate.And(p => p.ChargeGuiId == filter.ChargeGuiId);
        if (filter.SecurityKey != null)  predicate = predicate.And(p => p.SecurityKey == filter.SecurityKey);
        if (filter.PaymentMethod != null) predicate = predicate.And(p => p.PaymentMethod == filter.PaymentMethod);
        if (filter.PaymentStatus != null) predicate = predicate.And(p => p.PaymentStatus == filter.PaymentStatus);
        if (filter.PaymentStatusList?.Count > 0)
            predicate = predicate.And(p => filter.PaymentStatusList.Contains(p.PaymentStatus));
        return predicate;
    }
}
```

### Payment.cs Entity

```csharp
[Table("Payment", Schema = "RotaWatt")]
public class Payment : BaseEntity  // Id, Deleted base'den geliyor
{
    public DateTime CreatedDate { get; set; }
    public string GuiId { get; set; }          // Dis sistemler icin unique ID (GUID string)
    public string FirmGuiId { get; set; }
    public decimal Price { get; set; }
    public decimal PaidPrice { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public PaymentReasonEnum PaymentReason { get; set; }
    public string SecurityKey { get; set; }

    public DateTime? CompletedDate { get; set; }
    public string? ChargeGuiId { get; set; }         // Sarj islemi referansi
    public string? WalletProcessGuiId { get; set; }  // Cüzdan islem referansi
    public double? Kdv { get; set; }
    public string? PaymentChargeInfoJsonBase64 { get; set; }  // Base64 encoded JSON
    public string? UserAdressJsonBase64 { get; set; }

    // Navigation properties - lazy loading kapali, explicit include gerekli
    public PaymentCallbackData? PaymentCallbackData { get; set; }
    public WalletSpendMoney? WalletSpendMoney { get; set; }
    public WalletPushMoney? WalletPushMoney { get; set; }
}
```

---

## 4. Mimari Kararlar

| Karar | Aciklama |
|-------|----------|
| `BaseRepository<TEntity>` abstract | Her entity icin ayni CRUD/query metotlari; soft-delete ve tracking davranisi alt sinif tarafindan implement edilir |
| `ConnectedRepository` concrete | Tek bir concrete implementasyon; `DeleteWithState` soft delete (Deleted=true) yapar |
| `IQueryable<T>` dönüsü | Repository IQueryable döner, servis katmani `.FirstOrDefaultAsync()` ile bitirir. Sorgu zinciri servis katmaninda sekilleniyor |
| Predicate builder pattern | `predicate.And(...)` ile dinamik filtre zincirleme. Expression<Func<T,bool>> birlestirme |
| `AsSplitQuery()` | Include ile N+1 problemini azaltmak icin split query kullaniliyor |
| `UpdateWithProperties` | EF state'ini Unchanged yapip sadece belirtilen kolonlari IsModified=true yapiyor. Partial update |
| `Result<T>` wrapper | Tüm service metotlari Result<T> döner. Controller FromHttpClientResult ile sarmalayip Ok() döner |
| Autofac ConfigureContainer | Assembly scan ile repo ve service kayitlari otomatik |
| Assembly scan DI | ProjectPrefix ve Shared ile baslayan DLL'ler otomatik yuklenir |
| `DateTimeConverter` | JSON serializasyonda tarih `yyyy-MM-ddTHH:mm:ss` formatina donuyor (RotaWattBackEnd'de ekstra, SarjAllPro'da yok) |
| MassTransit + RabbitMQ | Fatura ve arsiv talepleri async olarak command ile gonderiliyor |
| `InnerRequestAttribute` | API'lar arasi iletisimde hangi API'nin cagirabilecegini kontrol eden attribute |
| SignalR | 3D odeme sonucu kullaniciya gercek zamanli bildirim (paymentNotification) |

---

## 5. Dikkat Edilecekler ve Potansiyel Sorunlar

1. **DateTime timezone**: `DateTimeConverter` `yyyy-MM-ddTHH:mm:ss` formatinda yazar, UTC offset yok. Sunucu saati UTC olmazsa sorun cikar.

2. **SQL Console log**: `UnitOfWork.OnConfiguring` her SQL'i `Console.WriteLine` ile yaziyor. Production'da ILogger kullanilmali.

3. **DeleteWithState null check sirasi**: `entity.Deleted = true` null check'ten once yapiliyor. Entity null ise NullReferenceException.

4. **UpdateWithProperties exception yutma**: `try/catch` icerisinde hata loglanmiyor.

5. **appsettings.json'da gercek credential**: Orijinal `appsettings.json` icerisinde gercek DB password, RabbitMQ password, Parasut API key, Logo entegrasyon credential'lari duruyor. Secrets vault'a tasinmali.

6. **Cüzdan amount token format**: Cüzdan miktari Base64 string icinde `/` separator ile saklaniyor (`GuiId/Amount/TockenGuiId`). Bu format fragile; JSON ya da ayri kolon tercih edilmeli.

7. **Repository.SaveChanges**: Servis katmani hem `InsertAsync` hem de hemen ardindan `SaveChangesAsync` cagiriyor. Bir islem birden fazla aggregate'i kapsiyorsa transaction yonetimi eksik.

8. **FirmIntegration ayri path**: `src/FirmIntegration/` altinda ana `src/Core` yapisinin disinda, ayri Dockerfile ve namespace.

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | .NET 5 |
| ORM | Entity Framework Core 5 |
| Veritabanı | SQL Server + NetTopologySuite (spatial) |
| DI Container | Autofac |
| Mesajlaşma | MassTransit + RabbitMQ |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| API Docs | Swagger / Swashbuckle |
| Realtime | SignalR |
| HTTP Client | Refit |
| Error Handling | ProblemDetails |
| Versioning | Microsoft.AspNetCore.Mvc.Versioning |
| Container | Docker / docker-compose (19 servis) |

## Mimari Pattern

**Clean Architecture + Microservices + Event-Driven Architecture**

Sistem 20+ bağımsız API'ye ayrılmış microservice mimarisine sahiptir. Her microservice kendi Application + Persistence katmanına sahip olup ortak bir `FrameworkCore` kütüphanesi paylaşır.

### Katman Hiyerarşisi (her servis için)

```
[ServiceName].Api           ← Presentation (HTTP endpoint'leri)
    ↓
[ServiceName].Application   ← Business logic, service implementasyonları
    ↓
[ServiceName].Persistence   ← DB context, repository, migration
    ↓
Shared.Domain               ← Ortak DTO, interface, model
    ↓
FrameworkCore               ← Base sınıflar, utility, wrapper
```

## API Servisleri (20+)

| Servis | Sorumluluk |
|---|---|
| Bank.Api | Ödeme ve banka işlemleri |
| Web.Api | Web panel backend |
| Mobil.Api | Mobil uygulama backend |
| Log.Api | İstek/yanıt loglama |
| Notification.Api | Push bildirim yönetimi |
| WorkerService.Api | Background job API |
| WorkerService | Arka planda çalışan hosted service |
| File.Api | Dosya yükleme / indirme |
| MailSms.Api | E-posta ve SMS gönderimi |
| GateWay.Api | API gateway, routing, auth |
| GoogleService.Api | Google Maps, geocoding entegrasyonu |
| Integration.Api | 3. taraf entegrasyonlar |
| Station.Api | Şarj istasyonu yönetimi |
| Ocpp.Api | OCPP protokol implementasyonu |
| Vm.Api | Sanal makine / cihaz yönetimi |
| Tocken.Api | JWT token üretimi ve doğrulama |
| FirmIntegration.Api | Firma entegrasyon alt sistemi |
| FirmIntegration.WorkerService | Firma entegrasyon worker |

## FrameworkCore — Özel Framework Katmanı

Kendi geliştirilen, tüm servislerde paylaşılan altyapı kütüphanesi:

```
Framework/Core/FrameworkCore/
├── Bases/
│   ├── BaseAttribute/          — Özel attribute base sınıfları
│   ├── BaseDataSeeding/        — Seed data mekanizması
│   ├── BaseDtos/               — Temel DTO sınıfları (IdRequestDto, vb.)
│   ├── BaseEntities/           — Entity base sınıfları (audit fields)
│   ├── BaseRepository/         — Generic repository interface ve implementasyon
│   ├── BaseServices/           — Service base class'ları
│   ├── BaseToken/              — Token yönetimi base
│   ├── BaseUnitOfWork/         — UoW pattern base
│   └── StartupBase/            — Startup.cs base (BaseStartup)
├── FrameworkCore/
│   ├── Api/                    — API yardımcı sınıflar
│   ├── DataProperties/         — Veri özellikleri
│   ├── Enums/                  — Framework enum'ları (ApiName, vb.)
│   ├── Extentions/             — Extension metodlar
│   ├── FilterAttributeCore/    — Action filter'lar (RequestResponseFilter)
│   ├── ProblemDetailCore/      — RFC 7807 hata formatı
│   ├── Repository/             — Repository concrete implementasyon
│   ├── UnitOfWorkCore/         — UoW concrete implementasyon
│   └── WrapperCore/            — Result<T> wrapper, FromHttpClientResult
└── Utils/
    ├── EntityUtils/            — Entity yardımcı sınıflar
    ├── Interface/              — Utility interface'leri
    ├── Models/                 — Utility model sınıfları
    └── Services/               — Utility servis implementasyonları
```

## Kritik Tasarım Kararları

### BaseStartup Kalıtımı
Her microservice `BaseStartup`'tan türeyen `Startup.cs` içerir. Bu pattern:
- CORS, auth, routing gibi tekrar eden konfigürasyonları tek noktada toplar
- Her servis sadece kendi özelleştirmesini override eder
- Tüm servislerde tutarlı middleware pipeline sağlar

### InnerRequestAttribute — Servisler Arası Yetkilendirme
```csharp
[InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
```
Bu attribute, controller action'larının yalnızca belirtilen iç servislerden çağrılabileceğini enforce eder. Doğrudan dış erişimi engeller.

### Result<T> Wrapper
Tüm API yanıtları `Result<T>` tipinde döner. `WrapperCore`'dan gelen `FromHttpClientResult()` extension metodu, bu nesneyi uygun HTTP status code'a çevirir.

### FirmIntegration Alt Sistemi
Diğer microservislerden izole, kendi 4 katmanlı yapısına sahip bağımsız alt sistem. API ve WorkerService olmak üzere 2 presentation projesi içerir.

## Event-Driven Mimari (MassTransit + RabbitMQ)

- Her servisin kendi `RabbitMq/Consumers/` klasörü var
- Consumer'lar servis katmanında tanımlanır, Startup'ta `RegisterMasstransit()` ile kaydedilir
- Log servisi tamamen event-driven: tüm loglar RabbitMQ üzerinden alınır

## Spatial Veri (NetTopologySuite)

EF Core 5 ile entegre coğrafi veri desteği. Şarj istasyonlarının koordinat bilgisi `geography` tipinde SQL Server'da saklanır. Yakın istasyon sorgulama ve mesafe hesaplama için kullanılır.

## Docker Yapısı

`docker-compose.yml` ile tüm 19 servis tek komutla ayağa kalkar. Her API servisi için ayrı `Dockerfile` mevcuttur. RabbitMQ management UI dahil.

## Dikkat Çeken Noktalar

### Olumlu
- FrameworkCore ile DRY prensibi güçlü biçimde uygulanmış
- InnerRequestAttribute servisleri dış erişimden koruyor
- NetTopologySuite entegrasyonu ile spatial özellikler production-ready
- 4 ortam desteği (docker-compose override mekanizması)

### İyileştirme Alanları
- .NET 5, LTS bitmişti — .NET 8'e upgrade öncelikli
- Domain katmanı ayrı proje değil, Shared.Domain olarak ortak tutulmuş (her servis için ayrı domain katmanı tartışmalı)
- API versioning var ama v2 endpoint'i bulunmuyor (hazırlık amacıyla eklemiş olabilir)

## FirmIntegration Alt Sistemi Yapısı

```
src/FirmIntegration/
├── Presentation/
│   ├── FirmIntegration.Api/
│   └── FirmIntegration.WorkerService/
├── Applications/
│   ├── FirmIntegration.Application/
│   └── FirmIntegrationWorkerService.Application/
├── Domains/
│   ├── FirmIntegration.Domain/
│   └── FirmIntegrationWorkerService.Domain/
└── Persistences/
    ├── FirmIntegration.Persistence/
    └── FirmIntegrationWorkerService.Persistence/
```

## Sonuç

Bu proje, .NET 5 ile geliştirilmiş kapsamlı bir EV şarj yönetim microservice backend'idir. 20+ bağımsız API, ortak bir `FrameworkCore` ile birleştirilmiş, event-driven mimari ile ölçeklenebilir yapı oluşturulmuştur. Aynı framework, `sarj_pro_backend_dotnet` ve `sarj_vm_backend_dotnet` projelerinde de kullanılmaktadır.

---

## Mobil.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Mobil.Api/
├── Controllers/
│   ├── CampaignController.cs
│   ├── ChargeDeviceController.cs
│   ├── ChargeProcessController.cs
│   ├── CountryCityAndTownController.cs
│   ├── DebitCardController.cs
│   ├── FirmController.cs
│   ├── HomeController.cs
│   ├── LoginController.cs
│   ├── OpportunityController.cs
│   ├── PaymentInfoArchiveInvoiceController.cs
│   ├── PaymentInfoController.cs
│   ├── RegisterController.cs
│   ├── RouteProcessController.cs
│   ├── SearchController.cs
│   ├── SplashScreenController.cs
│   ├── StationController.cs
│   ├── StationRatingProcessController.cs
│   ├── SupportProcessController.cs
│   ├── UserAddressController.cs
│   ├── UserCarProcessController.cs
│   ├── UserForgetPasswordController.cs
│   ├── UserProfileController.cs
│   └── WalletInfoController.cs
├── Program.cs
└── Startup.cs
```

### Startup.cs

```csharp
namespace Mobil.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }
     
        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<RotaWattDbContext>(dbcontextOptions);
            services.AddRotaWattApiService(Configuration, WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFrameworkServices();
            services.AddFluentValidators();
            services.AddFilters();
            services.RegisterMasstransit();
            services.AddMobilJwtTocken(Configuration);
            services.AddContextProvider();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### Controller: LoginController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class LoginController : ControllerBase
{
    private readonly IMobilLoginService _mobilLoginService;

    public LoginController(IMobilLoginService mobilLoginService)
    {
        _mobilLoginService = mobilLoginService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilLoginFormResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "LoginForm")]
    [ValidateFilter]
    public async Task<IActionResult> LoginForm(MobilLoginFormRequestDto mobilLoginFormRequest)
    {
        var result = await _mobilLoginService.LoginForm(mobilLoginFormRequest);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilLoginResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Login")]
    [ValidateFilter]
    public async Task<IActionResult> Login(MobilLoginRequestDto mobilLoginRequest)
    {
        var result = await _mobilLoginService.LoginCheck(mobilLoginRequest);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilLogOutResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "LogOut")]
    [ValidateFilter]
    public async Task<IActionResult> LogOut()
    {
        var result = await _mobilLoginService.LogOut();
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilGuestLoginResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "GuestLogin")]
    [ValidateFilter]
    public async Task<IActionResult> GuestLogin(MobilGuestLoginRequestDto mobilGuestLoginRequest)
    {
        var result = await _mobilLoginService.GuestLogin(mobilGuestLoginRequest);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilCheckAuthorizeResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "CheckAuthorize")]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
    public IActionResult CheckAuthorize()
    {
        return this.FromMobilResult(new SuccessResult<MobilCheckAuthorizeResponseDto>(new MobilCheckAuthorizeResponseDto { IsAuthorized = true }));
    }
}
```

### Controller: RegisterController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;
    public RegisterController(IRegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<RegisterValidatePersonalInfoResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "RegisterValidatePersonalInfo")]
    [ValidateFilter]
    public async Task<IActionResult> RegisterValidatePersonalInfo(RegisterValidatePersonalInfoRequestDto userRegister)
    {
        var result = await _registerService.RegisterValidatePersonalInfo(userRegister);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<MobilRegisterSmsVerifyResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "VerifyRegisterSms")]
    [ValidateFilter]
    public async Task<IActionResult> VerifyRegisterSms(MobilRegisterSmsVerifyRequestDto mobilRegisterSmsVerifyRequest)
    {
        var result = await _registerService.VerifyRegisterSms(mobilRegisterSmsVerifyRequest);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<UserRegisterCompleteResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "RegisterComplete")]
    [ValidateFilter]
    public async Task<IActionResult> RegisterComplete(UserRegisterCompleteRequestDto userRegister)
    {
        var result = await _registerService.RegisterComplete(userRegister);
        return this.FromMobilResult(result);
    }
}
```

### Controller: ChargeProcessController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class ChargeProcessController : ControllerBase
{
    private readonly IChargeProcessService _chargeProcessService;

    public ChargeProcessController(IChargeProcessService chargeProcessService)
    {
        _chargeProcessService = chargeProcessService;
    }

    // CheckChargeProcess, SelectDevice, GetSocketListForCharge, SelectSocket
    // StartCharge, StopCharge, GetCharges, GetChargeDetail
    // GetChargeCompleteSummary, ChargePaymentResultDetail
    // Tüm action'lar:
    //   [HttpPost] [Authorize] [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    //   [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    //   return this.FromMobilResult(result);

    [HttpPost]
    [ProducesResponseType(typeof(Result<StartChargeResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "StartCharge")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    public async Task<IActionResult> StartCharge(StartChargeRequestDto startChargeRequest)
    {
        var result = await _chargeProcessService.StartCharge(startChargeRequest);
        return this.FromMobilResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<StopChargeResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "StopCharge")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    public async Task<IActionResult> StopCharge(StopChargeRequestDto stopChargeRequest)
    {
        var result = await _chargeProcessService.StopCharge(stopChargeRequest);
        return this.FromMobilResult(result);
    }
}
```

### Controller: StationController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class StationController : ControllerBase
{
    private readonly IStationProcessService _stationProcessService;
    private readonly IUserContextProvider _userContextProvider;

    // Endpoints:
    // PrepareStationFilter, GetStationList, GetStationFilterCount, GetStationMapList
    // GetStationListDetail, GetStationDetail, GetStationPicture
    // GetFavoriteStation, AddFavoriteStation, RemoveFavoriteStation, GetStationPrices
    // Guest-accessible olanlar: MobilApiAuthenticationWithGuestFilterAttribute
    // Sadece auth gerektiren: MobilApiAuthenticationFilterAttribute
}
```

### Controller: WalletInfoController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class WalletInfoController : ControllerBase
{
    private readonly IWalletInfoService _walletInfoService;

    // GetWalletForm, CreateWalletInfo, GetWalletAmount
    // Tümü [HttpPost], return this.FromMobilResult(result)
}
```

### Controller: SupportProcessController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
[ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
public class SupportProcessController : ControllerBase
{
    private readonly ISupportProcessService _supportProcessService;

    // GetSupportList, GetSupportDetail, GetSupportTitleTypeList
    // CreateSupport, AnswerSupport, RatingSupport, CloseSupport
    // SetMessageSupportSeen, SetMessageSupportRead
    // GetUnSeenMessageCount, GetSupportMessage
    // SetAllMessagesSupportSeen, SetSupportAllMessagesRead
}
```

### Diger Controller'lar (Özet)

| Controller | Endpoint'ler | Auth |
|---|---|---|
| DebitCardController | GetDebitCardList, GetDebitCardDetail, AddDebitCard, SetDefaultDebitCard, RemoveDebitCard, GetBankInfoFromMobil | JWT + MobilApiAuthentication |
| PaymentInfoController | GetPaymentInfoStatus, PreparePaymentForm, GetPaymentCharge, GetPaymentAddBalanceWallet, CompletePaymentInfoDebitCard3D | JWT + MobilApiAuthentication |
| PaymentInfoArchiveInvoiceController | GetPaymentInfoArchiveInvoice, GetDocumentPaymentInfoArchiveInvoice (GET) | Log filter only |
| UserProfileController | GetUserGeneralInfo, RemoveUserReceiveSms, RemoveUser, GetUserProcessList | JWT + MobilApiAuthentication |
| UserCarProcessController | AddUserCar, UpdateUserCar, PrepareUserCarUpdateFrom, GetCarBrands, GetCarModels, GetCarTypes | JWT + MobilApiAuthentication |
| UserAddressController | GetUserAddressList, UpdateUserAddress, UserAddressPrepareInsertForm, UserAddressPrepareUpdateForm, AddUserAddress, RemoveUserAddress | JWT + MobilApiAuthentication |
| UserForgetPasswordController | UserForgetPasswordSendSms, UserForgetPasswordSmsVerify, UserForgetPasswordComplete | Version filter only |
| DebitCardController | GetDebitCardList, AddDebitCard, RemoveDebitCard, SetDefaultDebitCard, GetBankInfoFromMobil | JWT + MobilApiAuthentication |
| CampaignController | GetCampaignList, GetMobilCampaignDetail | JWT + MobilApiAuthenticationWithGuest |
| HomeController | GetCampaignHome, GetUnreadedAnnouncementCount, GetUserProcessMobilHome, GetHomeMapInfo | Karışık (bazı guest) |
| FirmController | GetFirmList, GetFirmPriceList | JWT + MobilApiAuthenticationWithGuest |
| OpportunityController | GetOpportunityList | JWT + MobilApiAuthenticationWithGuest |
| RouteProcessController | CreateRouteProcess, GetRouteStation, GetRouteDetail | JWT + MobilApiAuthentication |
| SearchController | GetSearchResult | Version filter only |
| SplashScreenController | GetSplashScreen | Log filter only |
| StationRatingProcessController | PreparingStationRatingForm, MakeStationRating, GetStationRatingForStationDetail, GetStationRatingPointsForStationDetail | JWT + MobilApiAuthenticationWithGuest |
| ChargeDeviceController | GetDeviceListForStationDetail | JWT + MobilApiAuthenticationWithGuest |
| CountryCityAndTownController | GetCountryList, GetCityList, GetTownList | Log filter only |

### Mimari Notlar

- Mobil.Api, mobil uygulama kullanıcılarına yönelik tüm endpoint'leri barındırır
- `FromMobilResult` kullanır (Web.Api'deki `FromResult`'tan farklı): her zaman Result<T> sarmalı döner
- İki auth seviyesi var: `MobilApiAuthenticationFilterAttribute` (tam auth) ve `MobilApiAuthenticationWithGuestFilterAttribute` (misafir+üye)
- `MobilApiVersionFilterAttribute` ile uygulama sürümü kontrolü yapılır
- `MobilApiRequestResponseLogFilterAttribute` ile tüm istek/yanıt çiftleri loglanır
- JWT token kayıtları `services.AddMobilJwtTocken(Configuration)` ile yapılır (Web.Api'den farklı token)
- 3D ödeme tamamlama endpoint'i `[InnerRequestAttribute(ApiName.BANK)]` ile sadece Bank.Api'den çağrılabilir

---

## Web.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Web.Api/
├── Controllers/
│   ├── AnnouncementManagmentController.cs
│   ├── AuthenticationController.cs
│   ├── AuthorizeManagmentController.cs
│   ├── CampaignManagmentController.cs
│   ├── CarManagmentController.cs
│   ├── ChargeManagmentController.cs
│   ├── CompanyManagmentController.cs
│   ├── ContentLanguageManagmentController.cs
│   ├── CountryCityAndTownController.cs
│   ├── DashboardController.cs
│   ├── DeviceConnectorManagmentController.cs
│   ├── DeviceManagmentController.cs
│   ├── DeviceRemoteManagmentController.cs
│   ├── FirmManagmentController.cs
│   ├── GibManagmentController.cs
│   ├── LogManagmentController.cs
│   ├── PanelAdminController.cs
│   ├── PanelAdminTypeController.cs
│   ├── ParameterController.cs
│   ├── ParameterGroupController.cs
│   ├── ParameterValueController.cs
│   ├── PaymentInfoArchiveAndInvoiceManagmentController.cs
│   ├── PaymentInfoManagmentController.cs
│   ├── PolicyManagmentController.cs
│   ├── ReportingController.cs
│   ├── StationManagmentController.cs
│   ├── SupportManagmentController.cs
│   ├── UserAddressManagmentController.cs
│   ├── UserManagmentController.cs
│   └── WalletInfoManagmentController.cs
├── Program.cs
└── Startup.cs
```

### Startup.cs

```csharp
namespace Web.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<RotaWattDbContext>(dbcontextOptions);
            services.AddRotaWattApiService(Configuration, WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFrameworkServices();
            services.AddFluentValidators();
            services.AddFilters();
            services.RegisterMasstransit();
            services.AddWebJwtTocken(Configuration);   // Web JWT (Mobil'den farklı)
            services.AddContextProvider();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### Controller: AuthenticationController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<LoginFormResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "LoginForm")]
    public async Task<IActionResult> LoginForm(LoginFormRequestDto loginFormRequest)
    {
        var result = await _authenticationService.LoginForm(loginFormRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "Login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequest)
    {
        var result = await _authenticationService.Login(loginRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<LogoutResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "LogOut")]
    public async Task<IActionResult> LogOut()
    {
        var result = await _authenticationService.LogOut();
        return this.FromResult(result);
    }
}
```

### Controller: StationManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class StationManagmentController : ControllerBase
{
    private readonly IStationManagmentService _stationManagmentService;

    // GetStationsForSelectList, GetStationDataTablePanel (DataTable), GetStationList
    // GetStationForUpdate, AddStation, ChangeStationState, RemoveStation, UpdateStation
    // AddStationPicture (IFormFile), StationPrepareInsertForm
    // Hepsi [HttpPost], return this.FromResult(result)
}
```

### Controller: DashboardController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    // GetStations, GetMonthlyCharge, GetSumOfProcess
}
```

### Controller: ChargeManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class ChargeManagmentController : ControllerBase
{
    private readonly IChargeManagmentService _chargeManagmentService;

    // GetChargeDatatable, GetChargeDetail, ChangeChargeState
    // ExportExcelChargeProcess, ExportPdfChargeProcess
}
```

### Controller: FirmManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[Authorize]
public class FirmManagmentController : ControllerBase
{
    private readonly IFirmManagmentService _firmManagmentService;

    // AddFirm, UpdateFirm, RemoveFirm - [WebApiMainAdminAuthenticationFilterAttribute]
    // GetFirmDataTablePanel, GetFirmPriceDatatablePanel - [WebApiMainAdminAuthenticationFilterAttribute]
    // GetFirmForSelectList - [WebApiFirmAdminAuthenticationFilterAttribute]
    // GetFirmForUpdate, FirmSettingPrepareForm, ChangeActiveStateFirm - [WebApiMainAdminAuthenticationFilterAttribute]
    // AddOrUpdateFirmPrice, RemoveFirmPrice, AddFirmLogo - [WebApiMainAdminAuthenticationFilterAttribute]
    // AddOrUpdateFirmSettingFromPanel
}
```

### Controller: DeviceRemoteManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class DeviceRemoteManagmentController : ControllerBase
{
    private readonly IChargeDeviceRemoteManagmentService _chargeDeviceRemoteManagmentService;

    // GetDeviceForRemoteManagmentDataTablePanel
    // GetOcppCommandMessageDataTablePanel
    // DeviceRemoteStartTransactionFromPanel
    // DeviceRemoteStopTransactionFromPanel
    // ChangeDeviceAvailabilityFromPanel
    // DeviceRemoteResetFromPanel
    // PrepareChangeConfigurationDeviceForm
    // DeviceRemoteChangeConfigurationFromPanel
    // Panel'den cihaza uzaktan komut gönderme flow'u
}
```

### Controller: ReportingController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class ReportingController : ControllerBase
{
    private readonly IStationReportingService _stationReportingService;
    private readonly IPaymentReportingService _paymentReportingService;
    private readonly IDeviceReportingService _deviceReportingService;

    // GetSumOfStationProcessReporting, GetStationProcessReporting
    // GetSumOfStationProcessByPowerTypeReporting
    // GetStationProcessReportingDataTablePanel
    // GetDeviceProcessReportingDataTablePanel
    // GetUserPaymentReportingDataTablePanel
    // PrepareMonthlyDeviceProcessByPowerTypeReporting
    // GetMonthlyDeviceProcessByPowerTypeReporting
    // 3 servis inject ediyor: Station + Payment + Device raporlama
}
```

### Controller: PanelAdminController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(WebApiRootAdminAuthenticationFilterAttribute))]  // En yüksek yetki seviyesi
public class PanelAdminController : ControllerBase
{
    // List (DataTable), GetPanelAdminById (GET), ChangeActiveState (PUT)
    // Add, RemovePanelAdmin, UpdatePanelAdmin (PUT)
    // HTTP metodları karışık: POST, GET, PUT
}
```

### Diger Controller'lar (Özet)

| Controller | Ana Endpoint'ler | Yetki Filtresi |
|---|---|---|
| DeviceManagmentController | AddChargeDevice, UpdateChargeDevice, ChargeDevicePrepareInsertForm, GetChargeDeviceForUpdate, GetChargeDeviceListPanel, GetChargeDeviceDataTablePanel, GetDevicePowerTypes | WebApiAuthenticationFilter |
| WalletInfoManagmentController | GetWalletInfoDataTablePanel, AddBalanceToWallet, RemoveBalanceToWallet, GetWalletProcessDataTablePanel, GetWalletPullMoneyDataTablePanel | WebApiMainAdminAuthentication |
| GibManagmentController | GetEsuGibChargeDeviceDataTablePanel, GetInstantGibReportDataTablePanel, AddEsuGibTaxPayer, AddEsuGibChargeDevice | WebApiMainAdminAuthentication |
| SupportManagmentController | GetSupportDataTablePanel, GetSupportList, GetSupportListForNotification, GetSupportDetail, AnswerSupport, CloseSupport, GetAwaitAnswerSupportsCount | WebApiAuthentication |
| CampaignManagmentController | AddCampaign, UpdateCampaign, GetCampaignForUpdate, GetCampaignDataTablePanel, RemoveCampaign, ChangeCampaignState, AddCampaignPicture | WebApiAuthentication |
| PaymentInfoManagmentController | GetPanelPaymentInfoDashboard, GetPaymentInfoDetailPanel, GetPaymentInfoDataTablePanel, AutomaticPayment, ExportExcelPaymentInfo, ExportPdfPaymentInfo | WebApiAuthentication + InnerRequest(WORKER_SERVICE) |
| UserManagmentController | GetUserDataTablePanel, GetUser, GetUsersForSelectList, ChangeUserState, RemoveUser | WebApiAuthentication |

### Mimari Notlar

- Web.Api, admin paneli backend'idir; tüm yönetim işlemleri buradan yapılır
- `FromResult` kullanır: `Ok(result.Data)` (Mobil'deki `FromMobilResult`'tan farklı)
- Üç seviye admin filtresi: `WebApiAuthenticationFilterAttribute` < `WebApiFirmAdminAuthenticationFilterAttribute` < `WebApiMainAdminAuthenticationFilterAttribute` < `WebApiRootAdminAuthenticationFilterAttribute`
- `DataTableFilterModel<T>` pattern ile server-side DataTable entegrasyonu var
- Excel/PDF export endpoint'leri var (ChargeManagment, PaymentInfo)
- File upload endpoint'leri `IFormFile` ile alınır (Station picture, Firm logo, Campaign picture)
- `services.AddWebJwtTocken(Configuration)` ile Web-özgü JWT kayıtlanır
- PanelAdmin controller'da `[HttpGet]` ve `[HttpPut]` de var (diğer controller'lardan farklı)

---

## Station.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Station.Api/
├── Controllers/
│   ├── ChargeDeviceConnectorOcppManagment.cs
│   ├── ChargeDeviceOcppManagementController.cs
│   ├── ChargeOcppManagmentController.cs
│   └── OcppFirmManagmentController.cs
├── Program.cs
└── Startup.cs
```

### Startup.cs

```csharp
namespace Station.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<RotaWattDbContext>(dbcontextOptions);
            services.AddRotaWattApiService(Configuration, WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFilters();
            services.RegisterMasstransit();
            services.AddFrameworkServices();
            services.RegisterSingletonService();   // Singleton servisler (OCPP state gibi)
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            // UseAuthentication/Authorization YOK — sadece InnerRequestAttribute ile güvenlik
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### Controller: ChargeOcppManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
public class ChargeOcppManagmentController : ControllerBase
{
    private readonly IChargeOcppManagmentService _chargeOcppManagmentService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<UpdateChargeOcppResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "UpdateCharge")]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> UpdateCharge(UpdateChargeOcppRequestDto updateChargeOcppRequest)
    {
        var result = await _chargeOcppManagmentService.UpdateCharge(updateChargeOcppRequest);
        return this.FromHttpClientResult(result);
    }
}
```

### Controller: ChargeDeviceOcppManagementController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
public class ChargeDeviceOcppManagementController : ControllerBase
{
    private readonly IChargeDeviceOcppManagmentService _chargeDeviceOcppManagmentService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> GetChargeDeviceForOcpp(GetChargeDeviceForOcppRequestDto request)
    {
        var result = await _chargeDeviceOcppManagmentService.GetChargeDeviceForOcpp(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> UpdateChargeDeviceForOcpp(UpdateChargeDeviceForOcppRequestDto request)
    {
        var result = await _chargeDeviceOcppManagmentService.UpdateChargeDeviceForOcpp(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Controller: ChargeDeviceConnectorOcppManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
public class ChargeDeviceConnectorOcppManagmentController : ControllerBase
{
    private readonly IChargeDeviceConnectorOcppManagmentService _chargeDeviceConnectorOcppManagmentService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> GetChargeDeviceConnectorForOcpp(GetChargeDeviceConnectorForOcppRequestDto request)
    {
        var result = await _chargeDeviceConnectorOcppManagmentService.GetChargeDeviceConnectorForOcpp(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> UpdateChargeDeviceConnectorForOcpp(UpdateChargeDeviceConnectorForOcppRequestDto request)
    {
        var result = await _chargeDeviceConnectorOcppManagmentService.UpdateChargeDeviceConnectorForOcpp(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Controller: OcppFirmManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
public class OcppFirmManagmentController : ControllerBase
{
    private readonly IOcppFirmManagmentService _ocppFirmManagmentService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.VM })]   // Sanal Makine API'den çağrılır
    public async Task<IActionResult> GetFirmForOcpp(GetFirmForOcppRequestDto request)
    {
        var result = await _ocppFirmManagmentService.GetFirmForOcpp(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Mimari Notlar

- Station.Api, OCPP protokolü ile şarj cihazları arasında köprü görevi görür
- Sadece `StationApiRequestInfoFilterAttribute` filtresi var; JWT auth yok
- Tüm endpoint'ler `[InnerRequestAttribute]` ile korunur: sadece OCPP.Api veya VM.Api çağırabilir
- `services.RegisterSingletonService()` ile OCPP state management için singleton servisler kaydedilir
- Her controller `FromHttpClientResult` kullanır (proxy pattern)
- Cihaz connector durumu, cihaz durumu ve şarj işlemi güncelleme endpoint'leri var

---

## Ocpp.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Ocpp.Api/
├── Controllers/
│   ├── OCPP16Controller.cs
│   ├── Ocpp16RemoteController.cs
│   ├── OcppCommandMessageController.cs
│   ├── OcppTriggerMessageManagmentController.cs
│   ├── RemoteTransactionController.cs
│   └── SocketMovementController.cs
├── Program.cs
└── Startup.cs
```

### Startup.cs

```csharp
namespace Ocpp.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<OcppDbContext>(dbcontextOptions);   // Ocpp'a özgü DbContext
            services.AddRotaWattApiService(Configuration, WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFilters();
            services.RegisterMasstransit();
            services.AddFrameworkServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            var webSocketOptions = new WebSocketOptions()
            {
                ReceiveBufferSize = 8 * 1024,
                KeepAliveInterval = TimeSpan.FromMinutes(10)
            };
            app.UseWebSockets(webSocketOptions);   // WebSocket desteği — OCPP için kritik
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### Controller: OCPP16Controller (WebSocket)

```csharp
public class OCPP16Controller : Controller
{
    private readonly IOcpp16ConnectionService _ocpp16ConnectionService;

    [Route("[controller]/[action]/{Identifier}")]
    public async Task Connection(string Identifier)
    {
        await _ocpp16ConnectionService.Connection(Identifier);
        // WebSocket upgrade burada yapılır; HTTP context üzerinden cihaz tanımlayıcısı alınır
    }
}
```

### Controller: Ocpp16RemoteController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class Ocpp16RemoteController : ControllerBase
{
    private readonly IOcpp16ChangeAvailabilityService _ocpp16ChangeAvailabilityService;
    private readonly IOcpp16ChangeConfigurationService _ocpp16ChangeConfigurationService;
    private readonly IOcpp16RemoteTransactionService _ocpp16RemoteTransactionService;
    private readonly IOcpp16RemoteResetService _ocpp16RemoteResetService;
    private readonly IOcppCommandMessageService _ocppCommandMessageService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
    public async Task<IActionResult> ChangeAvailability(Ocpp16ChangeAvailabilityRequestDto request)
    {
        var result = await _ocpp16ChangeAvailabilityService.ChangeAvailability(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
    public async Task<IActionResult> ChangeConfiguration(Ocpp16ChangeConfigurationRequestDto request)
    {
        var result = await _ocpp16ChangeConfigurationService.ChangeConfiguration(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> RemoteStartTransaction(Ocpp16StationRemoteStartTransactionRequestDto request)
    {
        var result = await _ocpp16RemoteTransactionService.RemoteStartTransactionAsync(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> RemoteStopTransaction(Ocpp16StationRemoteStopTransactionRequestDto request)
    {
        var result = await _ocpp16RemoteTransactionService.RemoteStopTransactionAsync(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
    public async Task<IActionResult> RemoteReset(Ocpp16ResetRequestDto request)
    {
        var result = await _ocpp16RemoteResetService.RemoteReset(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCommand(ProcessOcppCommandRequestDto request)
    {
        var result = await _ocppCommandMessageService.ProcessCommand(request);
        return this.FromResult(result);
    }
}
```

### Controller: OcppCommandMessageController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class OcppCommandMessageController : ControllerBase
{
    private readonly IOcppCommandMessageService _ocppCommandMessageService;

    [HttpPost]
    public async Task<IActionResult> GetOcppCommandMessage(DataTableFilterModel<OcppCommandMessageFilterDto> dataTableFilterModel)
    {
        var result = await _ocppCommandMessageService.GetOcppCommandMessage(dataTableFilterModel);
        return this.FromHttpClientResult(result);
    }
}
```

### Controller: OcppTriggerMessageManagmentController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class OcppTriggerMessageManagmentController : ControllerBase
{
    private readonly IOcppTriggerMessageManagmentService _ocppTriggerMessageManagmentService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE })]
    public async Task<IActionResult> GetDevicesToTrigger(GetDevicesToTriggerRequestDto request)
    {
        var result = await _ocppTriggerMessageManagmentService.GetDevicesToTrigger(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Controller: SocketMovementController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class SocketMovementController : ControllerBase
{
    private readonly ISocketMovementService _socketMovementService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> GetLastSocketMovement(GetLastSocketMovementRequestDto request)
    {
        var result = await _socketMovementService.GetLastSocketMovement(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Mimari Notlar

- Ocpp.Api, OCPP 1.6 protokolünü implement eden kritik servistir
- `app.UseWebSockets(webSocketOptions)` ile WebSocket desteği aktif edilir (8KB buffer, 10dk keepalive)
- `OCPP16Controller`, WebSocket bağlantısı için `/{Identifier}` route kullanır; normal API versioning yoktur
- Kendi `OcppDbContext`'i var (diğer API'lardan farklı schema)
- RemoteStartTransaction ve RemoteStopTransaction hem Mobil hem Web'den çağrılabilir
- ChangeAvailability, ChangeConfiguration, RemoteReset sadece Web panelden
- GetDevicesToTrigger sadece WorkerService'ten
- 5 farklı servis inject ediyor: ChangeAvailability, ChangeConfiguration, RemoteTransaction, RemoteReset, CommandMessage

---

## Notification.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Notification.Api/
├── Controllers/
│   ├── AutomaticPaymentNotificationController.cs
│   ├── ChargeNotificationController.cs
│   ├── ChargePointConnectionStateNotification.cs
│   ├── ConnectorConnectionController.cs
│   ├── ConnectorStateNotificationController.cs
│   ├── DeviceSocketNotificationController.cs
│   ├── DeviceStateNotificationController.cs
│   ├── MobilConnectionController.cs
│   ├── PanelAdminConnectionController.cs
│   ├── PaymentNotificationController.cs
│   └── SupportNotificationController.cs
├── Program.cs
└── Startup.cs
```

### Startup.cs

```csharp
namespace Notification.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ... DbContext, API servisi, AutoMapper kayıtları ...
            services.AddFrameworkServices();
            services.AddFilters();
            services.RegisterRabbitMq();   // MassTransit değil, doğrudan RabbitMQ consumer
            services.AddCors(options =>
            {
                options.AddPolicy(name: GetAppSettingValue("StartupConfigs:Policy"),
                    builder =>
                    {
                        builder.WithOrigins(GetAppSettingValue("SignalR:AllowLocalUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                        builder.WithOrigins(GetAppSettingValue("SignalR:AllowApiUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                        builder.WithOrigins(GetAppSettingValue("SignalR:AllowDomainUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    });
            });
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials());
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // SignalR Hub'ları
                endpoints.MapHub<ChargePointConnectionStateHub>("/deviceConnectionStateNotification");
                endpoints.MapHub<ConnectorStateHub>("/connectorStateNotification");
                endpoints.MapHub<SocketHub>("/deviceSocketNotification");
                endpoints.MapHub<ChargeHub>("/chargeNotification");
                endpoints.MapHub<DeviceStateHub>("/deviceStateNotification");
                endpoints.MapHub<PaymentHub>("/paymentNotification");
                endpoints.MapHub<PaymentHub>("/automaticPaymentNotification");
                endpoints.MapHub<SupportHub>("/supportNotification");
                endpoints.MapHub<SupportNotificationHub>("/supportForPanelNotification");
            });
        }
    }
}
```

### Controller: ChargeNotificationController

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
[ApiController]
[ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
public class ChargeNotificationController : ControllerBase
{
    private readonly IHubContext<ChargeHub> _hub;

    // SendOnlyUser: ConnectionId üzerinden tek kullanıcıya şarj durumu gönderir (mobil için)
    // SendChargeProcessInformationToPanel: ChargeGuiId üzerinden panel'e gönderir

    [HttpPost]
    public IActionResult SendOnlyUser(ChargeNotificationDto chargeNotification)
    {
        // Percent > 100 ise 100'e sabitlenir
        // StartDateLong/EndDateLong nullable DateTime parse edilir
        _hub.Clients.All.SendAsync(chargeNotification.ConnectionId, new ChargeNotificationDto { ... });
        return Ok("success");
    }

    [HttpPost]
    public IActionResult SendChargeProcessInformationToPanel(ChargeNotificationDto chargeNotification)
    {
        _hub.Clients.All.SendAsync(chargeNotification.ChargeGuiId, new ChargeNotificationDto { ... });
        return Ok("success");
    }
}
```

### Controller: SupportNotificationController

```csharp
[Route("v{version:apiVersion}/[controller]/[action]")]
[ApiController]
[ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
public class SupportNotificationController : ControllerBase
{
    private readonly IMobilConnectionRepository _mobilConnectionRepository;
    private readonly IPanelAdminConnectionRepository _panelAdminConnectionRepository;
    private readonly IConnectionMessagesRepository _connectionMessagesRepository;
    private readonly IHubContext<SupportHub> _hub;
    private readonly IHubContext<SupportNotificationHub> _supportNotificationHub;

    // SendOnlyUser: MobilUserGuiId -> ConnectionId çekip sadece o kullanıcıya gönderir
    // Send: AdminGuiId varsa admin connection'ına, yoksa broadcast eder
    //       Ayrıca _supportNotificationHub üzerinden panele bildirim gönderir
    //       ConnectionMessage tablosuna mesaj kaydeder (kalıcı log)
}
```

### Controller: ConnectorConnectionController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
public class ConnectorConnectionController : ControllerBase
{
    private readonly IConnectorConnectionService _connectorConnectionService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
    public async Task<IActionResult> AddMultipleConnectorConnection(AddMultipleConnectorConnectionRequestDto request)
    {
        var result = await _connectorConnectionService.AddMultipleConnectorConnection(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.STATION, ApiName.MOBIL })]
    public async Task<IActionResult> GetConnectorConnection(GetConnectorConnectionRequestDto request)
    {
        var result = await _connectorConnectionService.GetConnectorConnection(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Diger Controller'lar (Özet)

| Controller | Hub | Metod | Açıklama |
|---|---|---|---|
| ChargePointConnectionStateNotificationController | ChargePointConnectionStateHub | SendAll | Cihaz bağlantı durumu broadcast |
| ConnectorStateNotificationController | ConnectorStateHub | SendAll | Konnektör durum broadcast |
| DeviceStateNotificationController | DeviceStateHub | SendAll | Cihaz durum broadcast |
| DeviceSocketNotificationController | SocketHub | Send | ConnectorConnection lookup yaparak hedefli gönderim |
| PaymentNotificationController | PaymentHub | SendOnlyUser | ConnectionId üzerinden tek kullanıcıya ödeme sonucu |
| AutomaticPaymentNotificationController | PaymentHub | SendOnlyUser | Otomatik ödeme sonucu bildirim |
| MobilConnectionController | — | InsertOrUpdateMobilConnection, GetMobilConnection | SignalR ConnectionId takip servisi |
| PanelAdminConnectionController | — | InsertOrUpdatePanelAdminConnection, GetPanelAdminConnection | Admin connection takip servisi |

### Mimari Notlar

- Notification.Api, tüm gerçek zamanlı bildirimlerin merkezi
- 9 farklı SignalR Hub endpoint'i var
- CORS konfigürasyonu özel: `AddSignalR` için `AllowCredentials()` gerekiyor, ayrıca `SetIsOriginAllowed((host) => true)` ile tüm origin'lere izin verilmiş
- `RegisterRabbitMq()` ile MassTransit yerine doğrudan RabbitMQ consumer kullanılıyor
- Controller'lar genellikle `IHubContext<THub>` inject ediyor ve `Clients.All.SendAsync(connectionId, data)` ile hedefli mesaj gönderiyor
- `MobilConnectionRepository` ve `PanelAdminConnectionRepository` ile ConnectionId → kullanıcı eşleşmesi yapılıyor
- SupportNotificationController repository'yi doğrudan inject ediyor (servis katmanı bypass'lanıyor)
- Bildirim DTO'ları `Notification.Api`'e özel namespace altında: `Shared.Domain.Dto.NotificationDto.*`

---

## WorkerService + WorkerService.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/WorkerService/
├── Workers/
│   ├── AddOrUpdateArchiveAndInvoiceTokenWorker.cs
│   ├── AddOrUpdateEpdkTokenWorker.cs
│   ├── ArchiveAndInvoiceCreateRequestWorker.cs
│   ├── ArchiveAndInvoiceGetStatusWorker.cs
│   ├── ArchiveAndInvoiceSetDocumentDataWorker.cs
│   ├── ArchiveSetDocumentCanceledDataWorker.cs
│   ├── AutomaticPaymentWorker.cs
│   ├── EpdkCheckChargeWorker.cs
│   ├── EpdkPriceInfoWorker.cs
│   ├── RefundDebitCardVerificationWorker.cs
│   ├── SendMailArchiveAndInvoiceWorker.cs
│   └── TaskControlWorker.cs
└── Program.cs

src/Presentation/WorkerService.Api/
├── Startup.cs
└── Program.cs
```

### WorkerService Program.cs

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();  // WorkerService.Api Startup kullanılıyor
        })
        .ConfigureServices((hostContext, services) =>
        {
            IConfiguration configuration = hostContext.Configuration;
            var refitSetting = new RefitSettings { CollectionFormat = CollectionFormat.Csv };
            services.AddHttpClient();
            services.AddSingleton<IUtilService, UtilService>();
            services.AddSingleton<IWorkerServiceUtilService, WorkerServiceUtilService>();
            services.AddSingleton<IGenericHttpClientService, GenericHttpClientService>();
            // Hosted Services (arka planda çalışan worker'lar)
            services.AddHostedService<TaskControlWorker>();
            services.AddHostedService<AutomaticPaymentWorker>();
            services.AddHostedService<EpdkCheckChargeWorker>();
            services.AddHostedService<EpdkPriceInfoWorker>();
            services.AddHostedService<RefundDebitCardVerificationWorker>();
            services.AddHostedService<ArchiveAndInvoiceCreateRequestWorker>();
            services.AddHostedService<ArchiveAndInvoiceGetStatusWorker>();
            services.AddHostedService<ArchiveAndInvoiceSetDocumentDataWorker>();
            services.AddHostedService<ArchiveSetDocumentCanceledDataWorker>();
            services.AddSingleton<IWorkerServiceExceptionProducer, WorkerServiceExceptionProducer>();
        });
}
```

### WorkerService.Api Startup.cs

```csharp
namespace WorkerService.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<WorkerServiceDbContext>(DepencyInjectionType.TRANSIENT, dbcontextOptions);
            // DIKKAT: TRANSIENT DbContext — her worker döngüsünde yeni context
            services.AddRotaWattApiService(Configuration, WebHostEnvironment, ...);
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFrameworkServices();
            services.AddFilters();
            services.RegisterMasstransit();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // FARK: ConfigureRepositories kullanılmıyor
            builder.RegisterGeneric(typeof(ConnectedRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
            builder.RegisterAssemblyTypes(ApiOptions.RegistrationAssemblies.ToArray())
                .Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### TaskControlWorker.cs

```csharp
public class TaskControlWorker : BackgroundService
{
    public static List<GeneralTaskDto> _generaTasks;   // Static — tüm worker'lar erişir
    private readonly IGeneralTaskService _generalTaskService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                GetGeneralTaskRequestDto getGeneralTaskRequest = new GetGeneralTaskRequestDto();
                var taskProcess = await _generalTaskService.GetGeneralTask(getGeneralTaskRequest).ConfigureAwait(true);
                _generaTasks = taskProcess.Data.GeneralTasks;  // DB'den task konfigürasyonu yükleniyor
                await Task.Delay(3000);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Sistem Hatası: " + exception.Message);
                await Task.Delay(3000, stoppingToken);
                await StopAsync(stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_lifetime.ApplicationStopping.IsCancellationRequested)
        {
            await Task.Delay(600000);   // Hata durumunda 10 dakika bekle
            await ExecuteAsync(cancellationToken);  // Tekrar başlat
        }
    }
}
```

### AutomaticPaymentWorker.cs (Örnek Worker)

```csharp
public class AutomaticPaymentWorker : BackgroundService
{
    private int? delaySecond;  // DB'den alınan gecikme süresi

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (TaskControlWorker._generaTasks != null &&
                TaskControlWorker._generaTasks.Where(x => x.Id == (int)GeneralTaskIdEnum.AUTOMATIC_PAYMENT && x.Active) != null)
            {
                if (delaySecond == null)
                {
                    delaySecond = TaskControlWorker._generaTasks
                        .Where(x => x.Id == (int)GeneralTaskIdEnum.AUTOMATIC_PAYMENT && x.Active)
                        .FirstOrDefault().TaskScheduleSecond;
                }

                using (var scope = _services.CreateScope())
                {
                    var automaticPaymentProcessRepository = scope.ServiceProvider.GetRequiredService<IAutomaticPaymentProcessRepository>();
                    var paymentInfoManagmentClientService = scope.ServiceProvider.GetRequiredService<IPaymentInfoManagmentClientService>();

                    DateTime requestDate = DateTime.Now;
                    automaticPaymentResponse = await paymentInfoManagmentClientService.AutomaticPayment(automaticPaymentRequest);

                    AutomaticPaymentProcess automaticPaymentProcess = _mapper.Map<AutomaticPaymentProcess>(automaticPaymentResponse.Data, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            var destData = dest as AutomaticPaymentProcess;
                            destData.RequestDate = requestDate;
                        });
                    });
                    await automaticPaymentProcessRepository.InsertAsync(automaticPaymentProcess);
                    await automaticPaymentProcessRepository.SaveChangesAsync();
                }
                await Task.Delay(delaySecond.GetValueOrDefault() * 1000, stoppingToken);
            }
            else
            {
                await Task.Delay(5000, stoppingToken);  // Task bulunamazsa 5 sn bekle
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_lifetime.ApplicationStopping.IsCancellationRequested)
        {
            await Task.Delay(5000);
            await ExecuteAsync(cancellationToken);  // Hata durumunda kendini yeniden başlatır
        }
    }
}
```

### Worker Listesi ve Görevleri

| Worker | Görev |
|---|---|
| TaskControlWorker | DB'den task konfigürasyonlarını 3 sn'de bir yükler; diğer worker'lar bu static listeye bakar |
| AutomaticPaymentWorker | Ödenmemiş şarj işlemleri için otomatik ödeme tetikler |
| EpdkCheckChargeWorker | EPDK'ya şarj işlemlerini raporlar |
| EpdkPriceInfoWorker | EPDK'ya fiyat bilgisi gönderir |
| RefundDebitCardVerificationWorker | İade durumunu kontrol eder |
| ArchiveAndInvoiceCreateRequestWorker | Arşiv/fatura oluşturma talebi açar |
| ArchiveAndInvoiceGetStatusWorker | Arşiv/fatura durumunu sorgular |
| ArchiveAndInvoiceSetDocumentDataWorker | Belge verisini günceller |
| ArchiveSetDocumentCanceledDataWorker | İptal edilen belgeleri işler |
| AddOrUpdateArchiveAndInvoiceTokenWorker | Arşiv/fatura token'ını yeniler |
| AddOrUpdateEpdkTokenWorker | EPDK token'ını yeniler |
| SendMailArchiveAndInvoiceWorker | Fatura e-postası gönderir |

### Mimari Notlar

- WorkerService, ayrı bir host process ama WorkerService.Api Startup'ını da kullanır
- Her worker `BackgroundService` abstract'tan türer
- `IServiceProvider` + `CreateScope()` pattern: her döngüde scoped servis çözümlenir (DbContext leak önlenir)
- `TaskControlWorker._generaTasks` static field — task aktif/pasif ve gecikme süreleri DB'den gelir
- `WorkerServiceDbContext` TRANSIENT olarak kayıtlıdır (her scope için yeni instance)
- Hata durumunda: exception → RabbitMQ producer ile log API'ye → 5 sn bekle → StopAsync → tekrar başlat
- Utility servisler Singleton kaydedilir: `IWorkerServiceExceptionProducer`, `IWorkerServiceUtilService`

---

## GateWay.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/GateWay.Api/
├── Startup.cs
├── Program.cs
├── ocelot.json
├── ocelot.Local.json
├── ocelot.Prod.json
└── ocelot.Test.json
```

### Startup.cs

```csharp
namespace GateWay.Api
{
    public class Startup
    {
        // BaseStartup'tan türemiyor — Ocelot'a özgü minimal startup
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddCacheManager(
                settings => settings.WithDictionaryHandle());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
            app.UseWebSockets();
            app.UseOcelot().Wait();   // Ocelot middleware — tüm routing burada
        }
    }
}
```

### ocelot.json (Temel Yapılandırma)

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/v1/{url}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "mobil.api",
          "Port": "80"
        }
      ],
      "UpstreamPathTemplate": "/mobil/{url}",
      "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
      "LoadBalancerOptions": {
        "Type": "LeastConnection"
      }
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:8000"
  }
}
```

### Mimari Notlar

- GateWay.Api, `BaseStartup`'tan türemez; minimal Ocelot startup kullanır
- Ocelot API Gateway: upstream /mobil/* → downstream mobil.api:80/v1/*
- `LeastConnection` load balancer ile downstream servis seçimi
- Ortam başına ayrı ocelot config: `ocelot.Local.json`, `ocelot.Prod.json`, `ocelot.Test.json`
- `AddCacheManager` ile DictionaryHandle cache eklendi (response caching)
- `app.UseWebSockets()` ile WebSocket proxy desteği
- `app.UseOcelot().Wait()` — async olmayan çağrı (eski pattern)

---

## Integration.Api — Tam Kod Analizi

### Klasör Yapısı

```
src/Presentation/Integration.Api/
├── Controllers/
│   ├── ArchiveAndInvoiceIntegrationController.cs
│   ├── EDMIntegrationController.cs
│   ├── EpdkIntegrationController.cs
│   ├── GibIntegrationController.cs
│   ├── PaymentIntegrationController.cs
│   ├── PaymentMokaIntegrationController.cs
│   └── SmsIntegrationController.cs
├── Startup.cs
└── Program.cs
```

### Startup.cs

```csharp
namespace Integration.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRotaWattDbService<IntegrationDbContext>(dbcontextOptions);
            services.AddRotaWattApiService(Configuration, WebHostEnvironment, ...);
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFilters();
            services.AddFrameworkServices();
            services.RegisterMasstransit();
            services.AddContextProvider();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
```

### Controller: ArchiveAndInvoiceIntegrationController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
public class ArchiveAndInvoiceIntegrationController : ControllerBase
{
    private readonly IArchiveAndInvoiceIntegrationService _archiveAndInvoiceIntegrationService;
    private readonly IArchiveAndInvoiceEDMIntegrationService _archiveEDMIntegrationService;

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.TOKEN })]
    public async Task<IActionResult> ArchiveAndInvoiceIntegrationLogin(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
    public async Task<IActionResult> CreateArchiveAndInvoice(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
    public async Task<IActionResult> CreateRequestArchiveAndInvoice(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
    public async Task<IActionResult> GetStatusArchiveAndInvoice(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.BANK })]
    public async Task<IActionResult> GetStatusArchiveAndInvoiceList(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
    public async Task<IActionResult> SetDocumentDataArchiveAndInvoice(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> GetDocumentArchiveAndInvoice(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> ArchiveEDMLogin(...) { ... }

    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> ArchiveEDMSendInvoice(...) { ... }
}
```

### Controller: EpdkIntegrationController

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
public class EpdkIntegrationController : ControllerBase
{
    private readonly IEpdkIntegrationService _epdkIntegrationService;

    // EpdkLoginIntegration - [InnerRequest: TOKEN]
    // StartInUseSocket - [InnerRequest: OCPP]
    // CheckChargeProcess - [InnerRequest: WORKER_SERVICE]
    // EndInUseSocket - [InnerRequest: OCPP, WORKER_SERVICE]
    // AddMultiplePriceInfo - [InnerRequest: WEB]
    // AddPriceInfo - [InnerRequest: STATION, MOBIL, WEB]
}
```

### Controller: PaymentIntegrationController

```csharp
[ApiController]
[Produces("application/json")]
[ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
public class PaymentIntegrationController : ControllerBase
{
    private readonly IPaymentIntegrationService _paymentIntegrationService;

    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
    public async Task<IActionResult> PaymentDirectIntegration(...) { ... }

    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
    public async Task<IActionResult> PaymentStart3DIntegration(...) { ... }

    [Route("v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    [InnerRequestAttribute(new ApiName[] { ApiName.BANK })]
    public async Task<IActionResult> PaymentRefundIntegration(...) { ... }
}
```

### Controller: PaymentMokaIntegrationController

```csharp
[ApiController]
[Produces("application/json")]
public class PaymentMokaIntegrationController : ControllerBase
{
    // 3D ödeme callback endpoint'i — Moka ödeme sisteminden POST alır
    // Route parametreli: paymentCallbackDataId, paymentDataId, PaymentIntegrationProcessGuiId
    // [FromForm] ile hashValue, resultCode, resultMessage, trxCode, OtherTrxCode alır

    [Route("v{version:apiVersion}/[controller]/[action]/{paymentCallbackDataId}/{paymentDataId}/{PaymentIntegrationProcessGuiId}")]
    [HttpPost]
    public async Task<IActionResult> PaymentCompleteFromMoka(
        long paymentCallbackDataId, long paymentDataId, string paymentIntegrationProcessGuiId,
        [FromForm] string hashValue, [FromForm] string resultCode, [FromForm] string resultMessage,
        [FromForm] string trxCode, [FromForm] string OtherTrxCode)
    {
        // InnerRequestAttribute YOK — dış sistem (Moka) doğrudan çağırır
        var result = await _paymentIntegrationService.PaymentComplete3DIntegration(request);
        return this.FromHttpClientResult(result);
    }
}
```

### Diger Controller'lar

| Controller | Endpoint'ler | Çağıran |
|---|---|---|
| GibIntegrationController | AddEsuGibIntegrationTaxPayer, AddEsuGibIntegrationChargeDevice | WEB |
| SmsIntegrationController | SendSmsIntegration | MAIL_SMS |
| EDMIntegrationController | ArchiveAndInvoiceEDMLogin | TOKEN |

### Mimari Notlar

- Integration.Api, tüm 3. taraf entegrasyonların tek noktasıdır
- EPDK (Enerji Piyasası Düzenleme Kurumu), GİB (Gelir İdaresi), Moka (ödeme) entegrasyonları burada
- Her controller action `[InnerRequestAttribute]` ile hangi API'nin çağırabileceğini belirtir
- `PaymentMokaIntegrationController`'da `[InnerRequestAttribute]` yok — Moka'nın callback'i dışarıdan geliyor
- `IntegrationApiRequestResponseLogFilterAttribute` ile tüm istekler loglanır
- Kendi `IntegrationDbContext`'i var
- Route yapısı bazı controller'larda class seviyesinde, bazılarında method seviyesinde (PaymentIntegrationController)
