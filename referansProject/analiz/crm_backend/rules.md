# crm_backend — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
.NET 5 CRM / SaaS backend geliştirirken referans olarak kullanılabilir.

---

## 1. Proje Yapısı — 5 Katmanlı Mimari

**Kural:** Her ana servis için 5 proje oluştur (Infrastructure'ı dahil et):
- `[ServiceName].API` — Presentation (controller, middleware)
- `[ServiceName].Application` — Business logic, service interface implementasyonları
- `[ServiceName].Domain` — Entity, interface, DTO (dış bağımlılık yok)
- `[ServiceName].Persistence` — EF Core context, repository, migration
- `[ServiceName].Infrastructure` — Dış servis entegrasyonları (mail, SMS, 3. parti API)

**Kural:** Infrastructure katmanı, Application katmanındaki interface'leri implement eder. Application, Infrastructure'a bağımlı olamaz — yalnızca interface'lere.

**Kural:** Domain katmanı saf kalacak — herhangi bir framework veya altyapı kütüphanesine bağımlılık içermemeli.

---

## 2. Log Servisi — Event-Driven + MongoDB

**Kural:** Request/response loglama ana servisi bloklamamalı. Ayrı bir log microservisi oluştur:
1. Ana servis log event'ini RabbitMQ'ya publish eder (fire-and-forget)
2. LogService consumer olarak RabbitMQ'yu dinler
3. LogService log dokümanını MongoDB'ye yazar

**Kural:** Log servisi için MongoDB kullan. SQL Server'da log tablosu oluşturma. Gerekçeler:
- Şema esnekliği (her request farklı payload olabilir)
- Yüksek yazım hızı
- Log verisi nadiren transaction gerektirmez

**Kural:** Log dokümanı şeması:
```json
{
  "ServiceName": "crm.api",
  "RequestId": "guid",
  "HttpMethod": "POST",
  "Path": "/v1/Customer/Create",
  "RequestBody": "...",
  "ResponseBody": "...",
  "StatusCode": 200,
  "Duration": 125,
  "UserId": "...",
  "Timestamp": "2024-01-01T00:00:00Z"
}
```

**Kural:** Exception logları da aynı servise gönderilsin. `ExceptionType`, `StackTrace`, `Message` alanları eklenmeli.

---

## 3. Token Servisi — Merkezi JWT Yönetimi

**Kural:** JWT üretimini ayrı bir `TokenService.API` mikroservisinde topla. Diğer servisler token doğrulama yapabilir ama üretim TokenService'e bırakılsın.

**Kural:** Token servisi şu endpoint'leri expose etsin:
- `POST /v1/Token/Generate` — kullanıcı credentials ile token üret
- `POST /v1/Token/Refresh` — refresh token ile yeni access token al
- `POST /v1/Token/Validate` — token doğrulama (internal use)

**Kural:** JWT secret key yalnızca TokenService.API'nin appsettings / environment variable'ında bulunsun. Diğer servislerde sadece public key ile doğrulama yapılsın (asimetrik şifreleme tercih edilir).

**Kural:** Token servisi `InnerRequestAttribute` ile koruma altına alınsın — sadece tanımlı iç servisler çağırabilsin.

---

## 4. Notification Servisi Pattern

**Kural:** Bildirim gönderimi ana servisten tamamen ayır. Notification microservisi şu sorumlulukları üstlensin:
- E-posta gönderimi
- SMS gönderimi
- Push notification (mobile)

**Kural:** Ana servis, notification event'ini publish ederken şu bilgileri göndersin:
```json
{
  "NotificationType": "EMAIL | SMS | PUSH",
  "RecipientId": "",
  "TemplateName": "WelcomeEmail",
  "TemplateData": { "FirstName": "...", "ActionUrl": "..." }
}
```

**Kural:** Notification servisi template bazlı çalışsın. Template metinleri veritabanında tutulsun, kod içinde hardcode edilmemeli.

---

## 5. Worker Service Pattern

**Kural:** Zamanlı ve uzun süren görevler için ayrı WorkerService projesi oluştur. İki tür Worker Service:
- `WorkerService.API` — HTTP endpoint'e sahip, manuel tetiklenebilir job'lar
- `WorkerService` (hosted service) — zamanlayıcıyla çalışan, HTTP olmayan arka plan servisi

**Kural:** Worker job'ları RabbitMQ consumer üzerinden tetiklenebilmeli. Hem zamanla hem event ile tetikleme desteklenmeli.

**Kural:** Her job idempotent olsun — aynı job iki kez çalışırsa sistem tutarlı kalmalı.

---

## 6. File API Pattern

**Kural:** Dosya yönetimini ayrı `FileAPI` microservisine al. Diğer servisler dosyaları doğrudan kaydetmemeli.

**Kural:** FileAPI şu endpoint'leri içersin:
- `POST /v1/File/Upload` — dosya yükle, dosya ID döndür
- `GET /v1/File/Get/{id}` — dosyayı getir / stream et
- `DELETE /v1/File/Delete/{id}` — dosyayı sil

**Kural:** Diğer servisler dosya referansını (ID veya URL) kendi veritabanında saklasın; dosya içeriğini kendi DB'sine kaydetmesin.

**Kural:** Yüklenen dosyalar disk veya object storage (S3-compatible) üzerinde saklanmalı. Volume mount ile Docker container dışına kalıcı depolama yapılsın.

---

## 7. Docker Compose Yapısı

**Kural:** docker-compose servis adları düz ve tutarlı olsun:
- `crm.api`
- `crmlogservice.api`
- `crmnotificationservice.api`
- `crmFile.api`
- `rabbitmq`
- `logdb` (MongoDB)

**Kural:** MongoDB için network izolasyonu — sadece log servisi `logdb`'ye erişebilmeli. Diğer servisler MongoDB'ye doğrudan bağlanmamalı.

**Kural:** RabbitMQ management plugin aktif olsun (rabbitmq:3-management-alpine). Development'ta port 15672 expose edilsin.

---

## 8. Infrastructure Katmanı Kullanımı

**Kural:** Dış servis entegrasyonları (mail, SMS, ödeme, CRM entegrasyonu) Infrastructure katmanında implement edilsin:

```
Infrastructure/
├── Email/
│   ├── IEmailService.cs          (Application'da tanımlanmış interface)
│   └── SmtpEmailService.cs       (Infrastructure implementasyonu)
├── Sms/
│   ├── ISmsService.cs
│   └── NetgsmSmsService.cs
└── ThirdParty/
    └── [IntegrationName]Client.cs
```

**Kural:** Application servislerinde sadece `IEmailService`, `ISmsService` gibi interface'ler kullanılsın. Concrete sınıflar Autofac ile Infrastructure katmanından inject edilsin.

**Kural:** Infrastructure bağımlılıkları (SMTP client, SMS API key) appsettings ile yapılandırılsın. Interface değişmeden farklı provider'a geçilebilir olsun.

---

## 9. Domain Katmanı

**Kural:** Domain katmanı CRM entity'lerini içersin:
- `Customer` / `Lead` / `Contact` — temel CRM nesneleri
- `Activity` — görüşme, toplantı, görev
- `Pipeline` / `Stage` — satış süreci
- `Note` / `Attachment` — ek bilgiler

**Kural:** Entity'ler `BaseEntity`'den türesin. Audit fields (CreatedAt, UpdatedAt, CreatedBy) base class'ta olsun.

**Kural:** Domain event mekanizması varsa entity içinde domain event list tut, Application servisinde publish et. (sarj_backend'de yoktu, CRM'de eklenebilir.)

---

## 10. Persistence Katmanı

**Kural:** `sarj_backend_dotnet` ile aynı Repository + UnitOfWork pattern'i uygula. Detaylar için bkz: `sarj_backend_dotnet/rules.md` madde 7-9.

**Kural:** CRM'e özgü sorgular için Generic Repository yeterli olmayabilir. Specification pattern veya özel repository metodları ekle:
```csharp
Task<IList<Customer>> GetLeadsWithActivitiesAsync(LeadFilterDto filter);
Task<int> CountByPipelineStageAsync(int stageId);
```

---

## 11. Genel Servis Kalitesi

**Kural:** Her microservis için health check endpoint'i ekle: `GET /health`

**Kural:** Her serviste Swagger açık olsun (development ortamında). Servisler arası API sözleşmesi Swagger üzerinden belgelenmeli.

**Kural:** Servisler arası HTTP iletişimde Refit kullan. Her servisin API client interface'i Infrastructure katmanında tutulsun.
