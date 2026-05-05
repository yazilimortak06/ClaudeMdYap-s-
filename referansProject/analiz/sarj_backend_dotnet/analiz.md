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
