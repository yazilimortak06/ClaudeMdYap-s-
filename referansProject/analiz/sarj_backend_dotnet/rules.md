# sarj_backend_dotnet — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
.NET 5 microservice backend geliştirirken referans olarak kullanılabilir.

---

## 1. Proje ve Çözüm Yapısı

**Kural:** Her microservice için 3 ayrı proje oluştur:
- `[ServiceName].Api` — Presentation
- `[ServiceName].Application` — Business Logic
- `[ServiceName].Persistence` — Data Access

**Kural:** Tüm microservisler tek bir Solution (.sln) dosyasında yönetilsin. Bu, derleme ve bağımlılık yönetimini kolaylaştırır.

**Kural:** Ortak altyapıyı `FrameworkCore` veya benzeri paylaşımlı bir kütüphane projesinde topla. Servisler bu projeye NuGet veya project reference üzerinden bağlansın.

**Kural:** İzole alt sistemler (örn: FirmIntegration) kendi 4 katmanlı yapısıyla `src/` altında ayrı bir klasörde tutulsun.

---

## 2. BaseStartup Pattern

**Kural:** Her microservice `BaseStartup`'tan türeyen bir `Startup.cs` sınıfı içersin.

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration) : base(configuration) { }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddProjectDbService(Configuration);
        services.AddProjectApiService(Configuration);
        services.AddProjectAutoMapperService();
        services.AddFilters();
        services.AddFrameworkServices(Configuration);
        services.RegisterMasstransit(Configuration);
    }

    public override void ConfigureContainer(ContainerBuilder builder)
    {
        base.ConfigureContainer(builder);
        builder.ConfigureRepositories();
        builder.ConfigureServices();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);
        app.ConfigureBuilderInit(env);
        app.UseSwaggerBuilder(Configuration);
        app.UseSignalRBuilder();
        app.UseErrorBuilder(env);
    }
}
```

**Kural:** `Program.cs`'de her zaman `UseServiceProviderFactory(new AutofacServiceProviderFactory())` kullan.

**Kural:** Servise özgü middleware'ler (SignalR hub, özel route, vb.) override edilen `Configure` metoduna eklensin. Base'de tüm servisler için ortak middleware'ler çalışır.

---

## 3. Controller Pattern

**Kural:** Her controller action'ı şu attribute setini içersin:
1. `[Route("v{version:apiVersion}/[controller]/[action]")]` — versioned routing
2. `[HttpPost]` — tüm action'lar POST (body ile veri iletimi)
3. `[ProducesResponseType(typeof(Result<T>), 200)]` — tip güvenli response
4. `[ServiceFilter(typeof(RequestResponseFilterAttribute))]` — otomatik loglama
5. `[InnerRequestAttribute(new ApiName[] { ... })]` — servisler arası yetkilendirme

**Kural:** Controller action'ları şu form üzerinden yanıt dönsün:
```csharp
return this.FromHttpClientResult(result);
```
`FromHttpClientResult` extension metodu `Result<T>` nesnesini HTTP status code'a çevirir.

**Kural:** Controller'lar `ControllerBase`'den türesin, `Controller`'dan değil. View ihtiyacı yok.

**Kural:** HTTP metodu olarak her zaman POST kullan. Hem liste hem de tekil sorgular POST ile yapılır; query string yerine request body kullanılır.

---

## 4. InnerRequestAttribute — Servisler Arası Güvenlik

**Kural:** Bir endpoint sadece belirli iç servislerden çağrılabilecekse `InnerRequestAttribute` kullan:
```csharp
[InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]          // Sadece Mobil API
[InnerRequestAttribute(new ApiName[] { ApiName.WEB, ApiName.MOBIL })]  // Web ve Mobil
```

**Kural:** `ApiName` enum'u `FrameworkCore/Enums/` altında tanımlanmış tüm servisleri listelesin:
`BANK`, `WEB`, `MOBIL`, `LOG`, `NOTIFICATION`, `FILE`, `MAILSMS`, `GATEWAY`, `GOOGLESERVICE`, `INTEGRATION`, `STATION`, `OCPP`, `VM`, `TOCKEN`

**Kural:** Dışarıdan gelen isteklere açık public endpoint'ler `InnerRequestAttribute` içermemeli. Sadece servisler arası (internal) endpoint'lere uygula.

---

## 5. Result<T> Wrapper

**Kural:** Tüm service metodları `Task<Result<T>>` dönsün. `Result<T>` sınıfı şu alanları içersin:
- `bool IsSuccess`
- `T Data`
- `string ErrorMessage`
- `int ErrorCode`
- `List<string> Errors` (FluentValidation hataları için)

**Kural:** Controller katmanı, `Result<T>` içeriğini yorumlamak yerine `FromHttpClientResult()` extension metoduna devretsin.

**Kural:** Başarılı işlem: `Result<T>.Success(data)`, başarısız işlem: `Result<T>.Failure(errorCode, message)` factory metodları ile oluşturulsun.

---

## 6. Autofac DI Kayıtları

**Kural:** Repository ve Service kayıtları `ConfigureRepositories()` ve `ConfigureServices()` isimli iki extension metoda ayrılsın.

**Kural:** `ConfigureRepositories()` Persistence katmanında, `ConfigureServices()` Application katmanında tanımlanmış statik extension metot olsun.

**Kural:** Tüm kayıtlar `InstancePerLifetimeScope()` ile yapılsın. Singleton ve transient'e ihtiyaç duyan özel durumlar belgelenmeli.

**Kural:** Interceptor kullanımı (logging, transaction) `ConfigureServices()` içinde Autofac'ın `EnableInterfaceInterceptors()` ile kurulsun.

---

## 7. MassTransit + RabbitMQ Entegrasyonu

**Kural:** Her servis kendi consumer'larını `Application/RabbitMq/Consumers/` altında tanımlasın.

**Kural:** Consumer kayıtları `Startup.cs`'deki `services.RegisterMasstransit(Configuration)` extension metodunda yapılsın.

**Kural:** appsettings.json'daki RabbitMQ ayarları şu key yapısını kullansın:
```json
"RabbitMqSettings": {
  "HostName": "",
  "PortNumber": "",
  "UserName": "",
  "Password": ""
}
```

**Kural:** Log servisi tamamen event-driven olsun. Diğer servisler log event'lerini RabbitMQ'ya yayınlar, Log.Api consumer olarak dinler ve kaydeder.

---

## 8. EF Core + NetTopologySuite

**Kural:** Coğrafi veri (konum, koordinat) içeren entity'lerde `NetTopologySuite.Geometries.Point` tipini kullan. SQL Server'da `geography` tipine map et.

**Kural:** `OnModelCreating` içinde konfigürasyonları tek tek yazmak yerine `ApplyConfigurationsFromAssembly` kullan. Her entity için ayrı `IEntityTypeConfiguration<T>` sınıfı tanımla.

**Kural:** Migration'lar her servisin kendi `Persistence` projesinde tutulsun. Servisler arası migration paylaşımı yapılmasın.

**Kural:** Bulk insert/update operasyonları için `EFCore.BulkExtensions` kullan. `AddRangeAsync` yerine `BulkInsertAsync` büyük veri setlerinde performansı artırır.

---

## 9. Unit of Work Pattern

**Kural:** Her servis için `I[ServiceName]UnitOfWork` interface'i ve implementasyonu oluştur.
```csharp
public interface IStationUnitOfWork : IUnitOfWork
{
    IStationRepository StationRepository { get; }
    IConnectorRepository ConnectorRepository { get; }
}
```

**Kural:** Service katmanı doğrudan repository'lere bağımlı olmasın; UnitOfWork üzerinden erişsin.

**Kural:** Transaction gerektiren işlemlerde `BeginTransactionAsync()` / `CommitTransactionAsync()` / `RollbackTransactionAsync()` döngüsünü kullan.

---

## 10. API Versioning

**Kural:** Tüm controller route'ları `v{version:apiVersion}` içersin:
```
/v1/Station/GetList
/v2/Station/GetList  (yeni sürüm)
```

**Kural:** Default API version 1.0 olsun. `ApiVersioningOptions.AssumeDefaultVersionWhenUnspecified = true` ayarlanmış olsun.

**Kural:** Yeni versiyonlar geriye dönük uyumluluğu bozmadan eklenmeli. Eski endpoint'ler kaldırılmamalı, deprecated olarak işaretlenmeli.

---

## 11. Swagger / API Dokümantasyonu

**Kural:** Her servis kendi Swagger endpoint'ini `UseSwaggerBuilder()` extension metoduyla expose etsin.

**Kural:** Swagger UI sadece development ve test ortamlarında aktif olsun. Production'da kapalı tutulsun.

**Kural:** API versioning ile Swagger entegrasyonu yapılsın. Her versiyon için ayrı Swagger endpoint tanımlanmalı.

---

## 12. Error Handling (ProblemDetails)

**Kural:** Global hata yakalama için RFC 7807 standardındaki `ProblemDetails` formatını kullan. `UseErrorBuilder(env)` extension metodu middleware pipeline'a eklenmeli.

**Kural:** Development ortamında stack trace dahil edilsin, production'da sadece genel hata mesajı gösterilsin.

**Kural:** FluentValidation hata yanıtları da `ProblemDetails` formatında dönsün. `extensions.errors` alanına validation hataları yerleştirilmeli.

---

## 13. Docker Yapısı

**Kural:** Her microservice için ayrı `Dockerfile` oluştur. Multi-stage build kullan:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
# ...
```

**Kural:** `docker-compose.yml`'de tüm servislerin `restart: unless-stopped` ile çalışmasını sağla.

**Kural:** RabbitMQ'ya bağımlı servisler `depends_on: - rabbitmq` içersin.

**Kural:** Connection string ve şifreler docker-compose'da ortam değişkeni olarak verilsin, image içine gömülmesin.

---

## 14. appsettings Yapısı

**Kural:** appsettings.json key yapısı şu standartlara uysun:

```json
{
  "ConnectionStrings": { "MainConnectionString": "" },
  "StartupConfigs": {
    "ProjectPrefix": "",
    "ApiUrl": "",
    "ApiName": "",
    "AllowAnonymous": false
  },
  "RabbitMqSettings": { "HostName": "", "UserName": "", "Password": "" },
  "Wallet": { "TockenKey": "" }
}
```

**Kural:** Connection string key adı olarak `MainConnectionString` standardını tüm servislerde koru.

**Kural:** Secret değerleri (DB password, token key, API key) ortam değişkenleri veya Secret Manager üzerinden sağlan. appsettings.json'a yazılmasın.
