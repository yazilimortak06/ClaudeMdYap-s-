# crm_backend — Klasör Yapısı

## Genel Bakış
.NET 5 tabanlı CRM microservice backend. Ana CRM API'ye ek olarak Log, Notification, File ve Worker servisleri ayrı microservis olarak konuşlandırılmıştır. MongoDB log depolama ve SQL Server ana veri için hibrit yaklaşım kullanılmaktadır.

---

## Ana CRM Servisi — 5 Katmanlı Yapı

```
PixdinnCrm.API/
├── Controllers/
│   ├── CustomerController.cs
│   ├── LeadController.cs
│   ├── ActivityController.cs
│   ├── PipelineController.cs
│   └── ReportController.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile

PixdinnCrm.Application/
├── Services/
│   ├── CustomerService.cs
│   ├── LeadService.cs
│   └── ActivityService.cs
├── Filters/
├── Extentions/
│   ├── RepositoryRegistration.cs
│   └── ServiceRegistration.cs
├── Interceptors/
└── RabbitMq/
    └── Consumers/
        ├── NotificationResultConsumer.cs
        └── WorkerJobResultConsumer.cs

PixdinnCrm.Domain/
├── Entities/
│   ├── Customer.cs
│   ├── Lead.cs
│   ├── Activity.cs
│   └── Pipeline.cs
├── Interfaces/
│   ├── Repositories/
│   └── Services/
└── DTOs/
    ├── Customer/
    ├── Lead/
    └── Activity/

PixdinnCrm.Persistence/
├── DbContext/
│   └── CrmDbContext.cs
├── EntityFluent/
│   ├── CustomerConfiguration.cs
│   └── LeadConfiguration.cs
├── Migrations/
└── Repositories/
    ├── CustomerRepository.cs
    └── LeadRepository.cs

PixdinnCrm.Infrastructure/
├── Email/
│   ├── IEmailService.cs
│   └── SmtpEmailService.cs
├── Sms/
│   ├── ISmsService.cs
│   └── NetgsmSmsService.cs
└── ThirdParty/
    └── ExternalCrmClient.cs
```

---

## Microservis Projeleri

```
FileAPI/
├── Controllers/
│   └── FileController.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
└── Dockerfile

PixdinnCrmLogService/
├── Consumers/
│   ├── RequestResponseLogConsumer.cs
│   └── ExceptionLogConsumer.cs
├── Models/
│   └── LogDocument.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
└── Dockerfile

PixdinnCrmNotificationService/
├── Consumers/
│   └── NotificationEventConsumer.cs
├── Services/
│   ├── EmailNotificationService.cs
│   └── PushNotificationService.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
└── Dockerfile

PixdinnCrmWorkerServiceApi/
├── Controllers/
│   └── JobController.cs
├── Jobs/
│   ├── DailyReportJob.cs
│   └── ReminderJob.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
└── Dockerfile

TokenService.API/
├── Controllers/
│   └── TokenController.cs
├── Services/
│   └── JwtTokenService.cs
├── Program.cs
├── Startup.cs
├── appsettings.json
└── Dockerfile
```

---

## Framework / Ortak Kütüphane

```
Framework/
└── Core/
    └── FrameworkCore/
        ├── Bases/
        │   ├── BaseRepository/
        │   ├── BaseUnitOfWork/
        │   ├── BaseServices/
        │   ├── BaseToken/
        │   ├── BaseDtos/
        │   ├── BaseEntities/
        │   └── StartupBase/
        ├── FrameworkCore/
        │   ├── WrapperCore/        (Result<T>)
        │   ├── FilterAttributeCore/
        │   ├── ProblemDetailCore/
        │   ├── Repository/
        │   └── UnitOfWorkCore/
        └── Utils/
```

---

## Proje Sayısı Özeti

| Katman | Proje |
|--------|-------|
| Ana CRM API | 1 |
| Application | 1 |
| Domain | 1 |
| Persistence | 1 |
| Infrastructure | 1 |
| FileAPI | 1 |
| LogService | 1 |
| NotificationService | 1 |
| WorkerServiceAPI | 1 |
| TokenService | 1 |
| Framework | 1 (çok katmanlı) |
| **Toplam** | **~11** |
