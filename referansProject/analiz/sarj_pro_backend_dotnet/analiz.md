# sarj_pro_backend_dotnet — Detayli Mimari Analiz
Orijinal: `E:\Projeler\Backend\SarjAllPro`

> RotaWattBackEnd'in fork'u — gercek farklar asagida dokumante edildi.

## 1. Platform & Tech Stack

| Katman | Teknoloji |
|--------|-----------|
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
| Container | Docker / docker-compose (13+ servis) |

## Kaynak Proje

`sarj_pro_backend_dotnet` (SarjAllPro), `sarj_backend_dotnet` (RotaWattBackEnd) projesinin fork'udur. Aynı `FrameworkCore` altyapısını, aynı mimariyi ve aynı pattern'leri paylaşır.

**Temel fark:** Farklı iş alanı / müşteri için özelleştirilmiş, bazı servisler çıkarılmış, bazı konfigürasyonlar değiştirilmiştir.

## Mimari Pattern

`sarj_backend_dotnet` ile birebir aynıdır. Detaylar için bkz: `sarj_backend_dotnet/analiz.md`.

### Katman Hiyerarşisi (her servis için)

```
[ServiceName].Api           ← Presentation
[ServiceName].Application   ← Business Logic
[ServiceName].Persistence   ← Data Access
Shared.Domain               ← Ortak DTO / Interface
FrameworkCore               ← Base sınıflar / Utility
```

## API Servisleri (13+)

`sarj_backend_dotnet`'teki 20+ servisten bazıları çıkarılmıştır. Aktif servisler:

| Servis | Durum |
|---|---|
| Bank.Api | Aktif |
| Web.Api | Aktif |
| Mobil.Api | Aktif |
| Log.Api | Aktif |
| Notification.Api | Aktif |
| WorkerService.Api | Aktif |
| File.Api | Aktif |
| MailSms.Api | Aktif |
| GateWay.Api | Aktif |
| Station.Api | Aktif |
| Ocpp.Api | Aktif |
| Tocken.Api | Aktif |
| Integration.Api | Muhtemelen aktif |
| GoogleService.Api | Kısmen / opsiyonel |
| FirmIntegration | Çıkarılmış veya basitleştirilmiş |

## sarj_backend_dotnet'ten Temel Farklar

### 1. Ödeme Entegrasyonu
| Özellik | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Sağlayıcılar | IYZICO + PARAM + MOKA | IYZICO + PARAM + MOKA |
| Aktif sağlayıcı | IYZICO veya PARAM | **MOKA** (aktif) |
| appsettings config | PaymentIntegrations array | PaymentIntegrations array, MOKA selected:true |

### 2. Docker Image İsimleri
`sarj_backend_dotnet`'teki image isimleri `rotawatt/` prefix'i kullanırken, `sarj_pro_backend_dotnet`'te `sarjpro/` veya `sarjallpro/` prefix'i kullanılmaktadır.

### 3. Connection String Key
`sarj_backend_dotnet`: `MainConnectionString`
`sarj_pro_backend_dotnet`: Farklı bir key adı (örn: `SarjProConnectionString` veya aynı kalıp fakat farklı değer)

### 4. RabbitMQ Credentials
İki proje farklı RabbitMQ kullanıcı adı ve şifresi kullanır. Aynı RabbitMQ sunucusunda çalışmaz — farklı ortamlar veya farklı vhost'lar kullanılır.

### 5. Servis Sayısı
`sarj_backend_dotnet`: 20+ API (FirmIntegration dahil)
`sarj_pro_backend_dotnet`: 13+ API (bazı servisler çıkarılmış)

## FrameworkCore Paylaşımı

Her iki proje de aynı `FrameworkCore` yapısını kullanır. Bu durum iki seçenekten birini gösterir:
1. FrameworkCore ayrı bir NuGet paketi olarak yayımlanmış ve her iki projeye de eklenmiş
2. FrameworkCore kaynak kodu kopyalanmış ve iki projede de fork olarak yaşıyor

Kaynak kod kopyası olması durumunda, FrameworkCore'da yapılan bir iyileştirmenin iki projeye de uygulanması gerekir — bu teknik borç oluşturur.

## Docker Yapısı

13+ servis, tek `docker-compose.yml` ile yönetilir. `sarj_backend_dotnet` ile benzer yapıda; servis isimleri ve image adları farklıdır.

## Sonuç

Bu proje, `sarj_backend_dotnet`'in daha küçük kapsamlı ve farklı ödeme entegrasyonuyla özelleştirilmiş versiyonudur. Mimari ve pattern açısından birebir aynı. Geliştirme ve bakım maliyetini düşürmek için iki proje ortak FrameworkCore üzerinden birleştirilebilir, ancak farklı müşteri/ortam gereksinimleri bunu zorlaştırabilir.

---

## 2. Gercek Kod Farklari (diff bazli)

### Bank.Api/Startup.cs — Temel Fark

```csharp
// sarj_backend_dotnet (RotaWattBackEnd)
services.AddRotaWattDbService<PaymentDbContext>(dbcontextOptions);
services.AddRotaWattApiService(...);
services.AddRotaWattAutoMapperService(...);
// ConnectionStrings:RotaWattConnectionString

// sarj_pro_backend_dotnet (SarjAllPro)
services.AddPixdinnDbService<PaymentDbContext>(dbcontextOptions);   // <- Pixdinn prefix
services.AddPixdinnApiService(...);                                  // <- Pixdinn prefix
services.AddPixdinnAutoMapperService(...);                           // <- Pixdinn prefix
// ConnectionStrings:PixdinnConnectionString                         // <- Farkli key
```

### ApiServiceBaseCollectionExtensions.cs — Method Isimleri

```csharp
// sarj_backend_dotnet
public static IServiceCollection AddRotaWattDbService<TContext>(...)
public static IServiceCollection AddRotaWattApiService(...)
public static IServiceCollection AddRotaWattAutoMapperService(...)

// sarj_pro_backend_dotnet
public static IServiceCollection AddPixdinnDbService<TContext>(...)
public static IServiceCollection AddPixdinnApiService(...)
public static IServiceCollection AddPixdinnAutoMapperService(...)
```

### ApiServiceBaseCollectionExtensions.cs — DateTimeConverter Farki

```csharp
// sarj_backend_dotnet (EKSTRA): DateTimeConverter sinifi var
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
    }
}
// Ve AddControllers'a ekleniyor:
opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());

// sarj_pro_backend_dotnet: DateTimeConverter YOK
// Sadece JsonStringEnumConverter kullaniyor:
opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
```

### docker-compose.yml — Image Prefix Farki

```yaml
# sarj_backend_dotnet
rabbitmq:
  environment:
    - "RABBITMQ_DEFAULT_PASS=[MASKED]"
    - "RABBITMQ_DEFAULT_USER=rotawatt"    # rotawatt
  mobil.api:
    image: rotawatt/mobilapi:1             # rotawatt/ prefix

# sarj_pro_backend_dotnet
rabbitmq:
  environment:
    - "RABBITMQ_DEFAULT_PASS=[MASKED]"
    - "RABBITMQ_DEFAULT_USER=evtech"       # evtech (farkli kullanici)
  mobil.api:
    image: pixdinn/mobilapi:1              # pixdinn/ prefix
```

### appsettings.json — Credential Yapisi Farki

```json
// sarj_backend_dotnet — Sadece MOKA aktif, Iyzico/Param yok
{
  "ConnectionStrings": {
    "RotaWattConnectionString": "[MASKED]"  // key: RotaWattConnectionString
  },
  "RabbitMqSettings": {
    "UserName": "[MASKED]",
    "Password": "[MASKED]"
  }
}

// sarj_pro_backend_dotnet — Iyzico, Param, Moka hepsinin konfig var
{
  "ConnectionStrings": {
    "PixdinnConnectionString": "[MASKED]"   // key: PixdinnConnectionString
  },
  "Iyzico": {
    "ApiKey": "[MASKED]",
    "SecretKey": "[MASKED]",
    "BaseUrl": "https://sandbox-api.iyzipay.com",
    "CallBackUrl": "[MASKED]"
  },
  "Param": {
    "CLIENT_CODE": "[MASKED]",
    "CLIENT_USERNAME": "[MASKED]",
    "CLIENT_PASSWORD": "[MASKED]",
    "GUID": "[MASKED]",
    "CallBackUrl": "[MASKED]"
  },
  "Moka": {
    "DealerCode": "[MASKED]",
    "Username": "[MASKED]",
    "Password": "[MASKED]",
    "CheckKey": "[MASKED]",
    "BaseUrl": "https://service.refmoka.com",
    "CallBackUrl": "[MASKED]"
  },
  "RabbitMqSettings": {
    "UserName": "[MASKED]",
    "Password": "[MASKED]"
  }
}
```

### Eksik Servisler (sarj_pro'da yok)

| Servis | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|--------|---------------------|-------------------------|
| FirmIntegration.Api | VAR | YOK |
| Vm.Api | VAR | YOK |
| Station.Api | VAR | VAR |
| WorkerService | VAR | VAR |
| ManagerRootAdmin.Api | VAR | VAR |
| SarjAllMobil.Api | VAR | VAR |

---

## 3. FrameworkCore Fark Ozeti (diff)

Okunarak dogrulanan farklar:

| Konum | RotaWattBackEnd | SarjAllPro |
|-------|----------------|------------|
| Method prefix | `AddRotaWatt*` | `AddPixdinn*` |
| Connection key | `RotaWattConnectionString` | `PixdinnConnectionString` |
| DateTime custom converter | VAR (production icin) | YOK |
| Extra imports | System.Diagnostics, System.Text.Json | Daha az import |
| BaseRepository.cs | Birebir ayni | Birebir ayni |
| ConnectedRepository.cs | Birebir ayni | Birebir ayni |
| UnitOfWork.cs | Birebir ayni | Birebir ayni |
| Result<T> sistemi | Birebir ayni | Birebir ayni |
| Docker image prefix | `rotawatt/` | `pixdinn/` |
| RabbitMQ user | `rotawatt` | `evtech` |
