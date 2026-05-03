# crm_backend — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | .NET 5 |
| ORM | Entity Framework Core 5 |
| Veritabanı | SQL Server (ana veri) + MongoDB (log) |
| DI Container | Autofac |
| Mesajlaşma | MassTransit + RabbitMQ |
| Mapping | AutoMapper |
| Auth | JWT Bearer Token |
| API Docs | Swagger / Swashbuckle |
| Container | Docker / docker-compose |

## Mimari Pattern

**Layered Architecture + Microservices + Event-Driven Architecture**

Ana CRM servisi klasik katmanlı mimari (Domain → Application → Persistence → API) kullanırken; Log, Notification, File ve Worker servisleri ayrı bağımsız microservisler olarak konuşlandırılmaktadır. Token yönetimi ayrı bir servis olarak çıkarılmıştır.

### Microservice Envanteri

| Servis | Sorumluluk | DB |
|---|---|---|
| PixdinnCrm.API | Ana CRM API (müşteri, lead, aktivite, raporlama) | SQL Server |
| FileAPI | Dosya yükleme, saklama, erişim | SQL Server / Disk |
| PixdinnCrmLogService | RabbitMQ tüketicisi, istek-yanıt logları | MongoDB |
| PixdinnCrmNotificationService | Push / email / SMS bildirim | SQL Server |
| PixdinnCrmWorkerServiceApi | Zamanlı görevler, background job | SQL Server |
| TokenService.API | JWT üretimi ve doğrulama | SQL Server |
| Framework | Ortak altyapı kütüphanesi | — |

## Katman Yapısı (Ana CRM Servisi)

```
PixdinnCrm.API/              ← HTTP endpoint'leri, controller'lar
    ↓
PixdinnCrm.Application/      ← Business logic, servis implementasyonları
    ↓
PixdinnCrm.Persistence/      ← EF Core DbContext, repository, migration
    ↓
PixdinnCrm.Domain/           ← Entity, interface, DTO tanımları
    ↓
PixdinnCrm.Infrastructure/   ← Dış servis entegrasyonları (mail, sms, 3. parti)
    ↓
Framework/                   ← Ortak base sınıflar, utility
```

## Framework Katmanı

`sarj_backend_dotnet` ile aynı veya türevi FrameworkCore kütüphanesi. BaseRepository, BaseUnitOfWork, BaseServices, BaseToken, WrapperCore (Result<T>), FilterAttributeCore, ProblemDetailCore içerir.

## Log Servisi — MongoDB Kullanımı

`PixdinnCrmLogService`, tüm API request/response çiftlerini ve exception'ları RabbitMQ üzerinden alıp MongoDB'ye yazar. Bu tasarım:
- Ana CRM servisini log I/O'sundan izole eder
- Yüksek hacimli log yazımı için MongoDB'nin document yapısı uygundur
- Log dokümanları şema değişikliğine gerek duymadan genişleyebilir

## Docker Servisleri

```
docker-compose.yml içindeki servisler:
├── crm.api              (PixdinnCrm.API)
├── crmlogservice.api    (PixdinnCrmLogService)
├── crmnotificationservice.api (PixdinnCrmNotificationService)
├── crmFile.api          (FileAPI)
├── rabbitmq             (RabbitMQ + Management UI)
└── logdb                (MongoDB — sadece log servisi için)
```

TokenService ve WorkerService muhtemelen aynı docker-compose'da yer alır.

## Event-Driven Akışlar

### Request/Response Loglama
```
[CRM API] → (log event publish) → [RabbitMQ] → [LogService Consumer] → [MongoDB]
```

### Bildirim Gönderimi
```
[CRM API] → (notification event publish) → [RabbitMQ] → [NotificationService Consumer] → [Push/Mail/SMS]
```

### Background Job Tetikleme
```
[CRM API] → (job event publish) → [RabbitMQ] → [WorkerService Consumer] → [Scheduled Task]
```

## Kritik Tasarım Kararları

### Ayrı Token Servisi
JWT üretimi `TokenService.API`'de merkezi olarak yönetilir. Diğer servisler token doğrulama yapabilir ama üretim sadece TokenService üzerinden. Bu:
- Token yapısı değişince tek noktada güncelleme
- Secret key tek serviste saklanır
- Token revocation gibi gelişmiş özellikler için merkezi nokta sağlar

### Infrastructure Katmanı
`sarj_backend_dotnet`'te olmayan ama `crm_backend`'de yer alan katman. Dış servis entegrasyonlarını (e-posta provider, SMS gateway, CRM entegrasyonları) Application katmanından ayırır. Dependency Inversion Principle daha güçlü uygulanmış.

### MongoDB Sadece Log İçin
Hibrit veritabanı stratejisi: transactional data SQL Server, log data MongoDB. Bu pragmatik bir karar — log için ayrı şema migrasyonu gerektirmez, hızlı yazım performansı.

## Dikkat Çeken Noktalar

### Olumlu
- Infrastructure katmanı ile dış servis izolasyonu temiz
- MongoDB log tercihi ölçeklenebilir ve pratik
- Merkezi Token servisi güvenlik yönetimini kolaylaştırır
- Docker compose ile tüm bağımlılıklar tek komutla hazır

### İyileştirme Alanları
- .NET 5, destek süresi dolmuş — .NET 8'e upgrade gerekli
- TokenService ile diğer servisler arasındaki iletişim protokolü (Refit mi? HTTP mi?) belgelenmeli
- MongoDB ve SQL Server credential yönetimi ortam değişkenleri üzerinden yapılmalı

## Projeler Arası Benzerlikler

### crm_backend vs sarj_backend_dotnet
- Aynı `FrameworkCore` veya türevi kullanıyor
- Aynı BaseStartup + Autofac + MassTransit + RabbitMQ stack
- crm_backend'de Infrastructure katmanı ek olarak var
- crm_backend daha az API sayısı (tek ana API vs 20+)
- crm_backend MongoDB ekliyor (log için)

## Sonuç

CRM backend, `sarj_backend_dotnet`'e göre daha az sayıda ama daha belirgin katmanlı (Infrastructure eklendi) bir microservice mimarisi kullanıyor. MongoDB + SQL Server hibrit veri stratejisi, log verisi için iyi bir seçim. Framework katmanı `sarj_backend_dotnet` ile paylaşılıyor, bu iki proje aynı geliştirici / ekipten çıktığını gösteriyor.
