# crm_backend — Persistence Pattern

## Genel Mimari

`sarj_backend_dotnet` ile aynı Repository + UnitOfWork yapısı. Ek olarak MongoDB log repository mevcuttur.

Her CRM servisinin kendi `Persistence` projesi vardır:

```
PixdinnCrm.Persistence/
├── DbContext/
│   └── CrmDbContext.cs
├── EntityFluent/
│   ├── CustomerConfiguration.cs
│   ├── LeadConfiguration.cs
│   └── ActivityConfiguration.cs
├── Migrations/
│   └── (EF Core migration dosyaları)
└── Repositories/
    ├── CustomerRepository.cs
    ├── LeadRepository.cs
    └── ActivityRepository.cs
```

---

## SQL Server Repository Pattern

### Interface (Domain katmanında)

```csharp
// PixdinnCrm.Domain/Interfaces/Repositories/ICustomerRepository.cs
public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer> GetWithActivitiesAsync(int id);
    Task<IList<Customer>> GetByPipelineStageAsync(int stageId);
    Task<int> CountActiveCustomersAsync();
}
```

### Implementasyon (Persistence katmanında)

```csharp
// PixdinnCrm.Persistence/Repositories/CustomerRepository.cs
public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(CrmDbContext context) : base(context) { }

    public async Task<Customer> GetWithActivitiesAsync(int id)
    {
        return await Context.Customers
            .Include(c => c.Activities)
            .Include(c => c.PipelineStage)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<IList<Customer>> GetByPipelineStageAsync(int stageId)
    {
        return await Context.Customers
            .Where(c => c.PipelineStageId == stageId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountActiveCustomersAsync()
    {
        return await Context.Customers.CountAsync(c => c.IsActive && !c.IsDeleted);
    }
}
```

---

## Unit of Work Pattern

```csharp
// Interface
public interface ICrmUnitOfWork : IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    ILeadRepository LeadRepository { get; }
    IActivityRepository ActivityRepository { get; }
    IPipelineRepository PipelineRepository { get; }
}

// Implementasyon
public class CrmUnitOfWork : BaseUnitOfWork, ICrmUnitOfWork
{
    public ICustomerRepository CustomerRepository { get; private set; }
    public ILeadRepository LeadRepository { get; private set; }
    public IActivityRepository ActivityRepository { get; private set; }
    public IPipelineRepository PipelineRepository { get; private set; }

    public CrmUnitOfWork(
        CrmDbContext context,
        ICustomerRepository customerRepository,
        ILeadRepository leadRepository,
        IActivityRepository activityRepository,
        IPipelineRepository pipelineRepository) : base(context)
    {
        CustomerRepository = customerRepository;
        LeadRepository = leadRepository;
        ActivityRepository = activityRepository;
        PipelineRepository = pipelineRepository;
    }
}
```

---

## EntityFluent — Soft Delete Konfigürasyonu

CRM entity'leri fiziksel olarak silinmez; `IsDeleted` bayrağı kullanılır.

```csharp
// EntityFluent/CustomerConfiguration.cs
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(500);

        // Global query filter — soft delete
        // IsDeleted = true olan kayıtlar otomatik filtreden geçer
        builder.HasQueryFilter(x => !x.IsDeleted);

        // İlişkiler
        builder.HasMany(x => x.Activities)
               .WithOne(x => x.Customer)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index'ler
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.CreatedAt);
    }
}
```

---

## MongoDB Log Repository

```csharp
// Log servisi için ayrı MongoDB repository

public interface ILogRepository
{
    Task InsertRequestResponseLogAsync(RequestResponseLogDocument document);
    Task InsertExceptionLogAsync(ExceptionLogDocument document);
    Task<IList<RequestResponseLogDocument>> GetLogsAsync(LogFilterDto filter);
}

public class MongoLogRepository : ILogRepository
{
    private readonly IMongoCollection<RequestResponseLogDocument> _requestLogs;
    private readonly IMongoCollection<ExceptionLogDocument> _exceptionLogs;

    public MongoLogRepository(IMongoDatabase database)
    {
        _requestLogs = database.GetCollection<RequestResponseLogDocument>("request_logs");
        _exceptionLogs = database.GetCollection<ExceptionLogDocument>("exception_logs");
    }

    public async Task InsertRequestResponseLogAsync(RequestResponseLogDocument document)
    {
        document.Timestamp = DateTime.UtcNow;
        await _requestLogs.InsertOneAsync(document);
    }

    public async Task<IList<RequestResponseLogDocument>> GetLogsAsync(LogFilterDto filter)
    {
        var filterBuilder = Builders<RequestResponseLogDocument>.Filter;
        var dateFilter = filterBuilder.Gte(x => x.Timestamp, filter.StartDate)
                       & filterBuilder.Lte(x => x.Timestamp, filter.EndDate);

        return await _requestLogs.Find(dateFilter)
                                 .SortByDescending(x => x.Timestamp)
                                 .Skip(filter.Skip)
                                 .Limit(filter.Take)
                                 .ToListAsync();
    }
}
```

---

## Log Document Şeması

```csharp
[BsonCollection("request_logs")]
public class RequestResponseLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string ServiceName { get; set; }
    public string RequestId { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public int? UserId { get; set; }
    public DateTime Timestamp { get; set; }
}
```

---

## Autofac Kayıt Extension'ları

```csharp
// Persistence katmanında
public static class RepositoryRegistration
{
    public static void ConfigureRepositories(this ContainerBuilder builder)
    {
        // SQL Server repository'ler
        builder.RegisterType<CustomerRepository>().As<ICustomerRepository>().InstancePerLifetimeScope();
        builder.RegisterType<LeadRepository>().As<ILeadRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ActivityRepository>().As<IActivityRepository>().InstancePerLifetimeScope();
        builder.RegisterType<PipelineRepository>().As<IPipelineRepository>().InstancePerLifetimeScope();

        builder.RegisterType<CrmUnitOfWork>().As<ICrmUnitOfWork>().InstancePerLifetimeScope();
    }
}

// Log servisi için
public static class LogRepositoryRegistration
{
    public static void ConfigureLogRepositories(this ContainerBuilder builder)
    {
        // MongoDB repository singleton — bağlantı her request'te yeniden kurulmasın
        builder.RegisterType<MongoLogRepository>().As<ILogRepository>().SingleInstance();
    }
}
```

---

## Özet

| Bileşen | Sorumluluk |
|---------|-----------|
| `BaseRepository<T>` | CRUD + Bulk operasyonlar (FrameworkCore'dan) |
| `CustomerRepository` | CRM'e özgü müşteri sorguları |
| `ICrmUnitOfWork` | Tüm CRM repository'lerini tek noktada toplar |
| `EntityFluent/` | EF Core soft delete query filter dahil konfigürasyonlar |
| `MongoLogRepository` | Log dokümanlarını MongoDB'ye yazar |
| `HasQueryFilter(IsDeleted)` | Soft delete otomatik filtresi |
