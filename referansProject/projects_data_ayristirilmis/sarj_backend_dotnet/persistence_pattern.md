# sarj_backend_dotnet — Persistence Pattern

## Genel Mimari

Her microservice'in kendi `Persistence` projesi vardır. Bu proje şu klasörleri içerir:

```
[ServiceName].Persistence/
├── DbContext/
│   └── [ServiceName]DbContext.cs
├── EntityFluent/
│   └── [EntityName]Configuration.cs   (her entity için ayrı dosya)
├── Migrations/
│   └── (EF Core migration dosyaları)
└── Repositories/
    └── [EntityName]Repository.cs
```

---

## Repository Pattern

### Base Repository (FrameworkCore'dan kalıtım)

```csharp
// FrameworkCore/Bases/BaseRepository içinde tanımlanmış
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IList<TEntity>> GetAllAsync();
    Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task BulkInsertAsync(IList<TEntity> entities);        // EFCore.BulkExtensions
    Task BulkUpdateAsync(IList<TEntity> entities);        // EFCore.BulkExtensions
}

// Concrete Repository
public class [EntityName]Repository : BaseRepository<[EntityName]>, I[EntityName]Repository
{
    public [EntityName]Repository([ServiceName]DbContext context) : base(context) { }

    // Servis'e özgü sorgular buraya eklenir
    public async Task<[EntityName]> GetWithDetailsAsync(int id)
    {
        return await Context.Set<[EntityName]>()
            .Include(x => x.RelatedEntity)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
```

---

## Unit of Work Pattern

```csharp
// FrameworkCore/Bases/BaseUnitOfWork içinde tanımlanmış
public interface IUnitOfWork : IDisposable
{
    I[EntityName]Repository [EntityName]Repository { get; }
    // ... diğer repository'ler
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class [ServiceName]UnitOfWork : BaseUnitOfWork, I[ServiceName]UnitOfWork
{
    public I[EntityName]Repository [EntityName]Repository { get; private set; }

    public [ServiceName]UnitOfWork([ServiceName]DbContext context,
        I[EntityName]Repository entityRepository) : base(context)
    {
        [EntityName]Repository = entityRepository;
    }
}
```

---

## EntityFluent (Fluent API ile EF Core Konfigürasyonu)

EF Core entity konfigürasyonları ayrı `IEntityTypeConfiguration<T>` sınıflarında tutulur. `OnModelCreating` kalabalıklaşmaz.

```csharp
// EntityFluent/[EntityName]Configuration.cs
public class [EntityName]Configuration : IEntityTypeConfiguration<[EntityName]>
{
    public void Configure(EntityTypeBuilder<[EntityName]> builder)
    {
        builder.ToTable("[TableName]");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Spatial veri (koordinat) — NetTopologySuite
        builder.Property(x => x.Location)
            .HasColumnType("geography");

        // İlişkiler
        builder.HasMany(x => x.Children)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(x => x.ExternalCode).IsUnique();
    }
}

// DbContext içinde konfigürasyonları uygulama
public class [ServiceName]DbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tek satırla tüm konfigürasyonları uygula
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
```

---

## Autofac ile Repository/UoW Kayıtları

```csharp
// Startup.cs → ConfigureContainer
builder.ConfigureRepositories();

// Extension metodu (Persistence katmanında tanımlı)
public static class RepositoryRegistration
{
    public static void ConfigureRepositories(this ContainerBuilder builder)
    {
        builder.RegisterType<[EntityName]Repository>()
               .As<I[EntityName]Repository>()
               .InstancePerLifetimeScope();

        builder.RegisterType<[ServiceName]UnitOfWork>()
               .As<I[ServiceName]UnitOfWork>()
               .InstancePerLifetimeScope();
    }
}
```

---

## Data Seeding

```csharp
// FrameworkCore/Bases/BaseDataSeeding
// Uygulama başlangıcında temel verileri ekler
public class [ServiceName]DataSeeding : BaseDataSeeding
{
    public override async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<[ServiceName]DbContext>();

        if (!await context.[Entities].AnyAsync())
        {
            await context.[Entities].AddRangeAsync(GetSeedData());
            await context.SaveChangesAsync();
        }
    }
}
```

---

## Özet

| Bileşen | Sorumluluk |
|---------|-----------|
| `BaseRepository<T>` | CRUD + Bulk operasyonlar |
| `[Entity]Repository` | Servise özgü sorgular |
| `BaseUnitOfWork` | Transaction yönetimi, SaveChanges |
| `[Service]UnitOfWork` | Repository'leri tek noktada toplar |
| `EntityFluent/` | EF Core tablo/kolon/ilişki konfigürasyonu |
| `BaseDataSeeding` | Başlangıç verisi yükleme |
