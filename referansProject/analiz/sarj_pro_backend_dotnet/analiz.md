# sarj_pro_backend_dotnet — Analiz

## Platform ve Tech Stack

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
