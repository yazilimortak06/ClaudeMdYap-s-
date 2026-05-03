# sarj_backend_dotnet — Analiz

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
