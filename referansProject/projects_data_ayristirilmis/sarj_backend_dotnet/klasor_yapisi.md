# sarj_backend_dotnet — Klasör Yapısı

## Genel Bakış
.NET 5 tabanlı EV şarj yönetim microservice backend. 20+ bağımsız API projesi içerir. Ortak bir `FrameworkCore` katmanı tüm servisler tarafından paylaşılır.

---

## Framework/Core/FrameworkCore/

```
Framework/
└── Core/
    └── FrameworkCore/
        ├── Bases/
        │   ├── BaseAttribute/
        │   ├── BaseDataSeeding/
        │   ├── BaseDtos/
        │   ├── BaseEntities/
        │   ├── BaseRepository/
        │   ├── BaseServices/
        │   ├── BaseToken/
        │   ├── BaseUnitOfWork/
        │   └── StartupBase/
        ├── FrameworkCore/
        │   ├── Api/
        │   ├── DataProperties/
        │   ├── Enums/
        │   ├── Extentions/
        │   ├── FilterAttributeCore/
        │   ├── ProblemDetailCore/
        │   ├── Repository/
        │   ├── UnitOfWorkCore/
        │   └── WrapperCore/
        └── Utils/
            ├── EntityUtils/
            ├── Interface/
            ├── Models/
            └── Services/
```

---

## src/

### Presentation (API Katmanı)

Her API projesi aynı yapıya sahiptir: `Controllers/`, `Program.cs`, `Startup.cs`, `appsettings.json`, `Dockerfile`

```
src/
└── Presentation/
    ├── Bank.Api/
    ├── Web.Api/
    ├── Mobil.Api/
    ├── Log.Api/
    ├── Notification.Api/
    ├── WorkerService.Api/
    ├── File.Api/
    ├── MailSms.Api/
    ├── GateWay.Api/
    ├── GoogleService.Api/
    ├── Integration.Api/
    ├── Station.Api/
    ├── Ocpp.Api/
    ├── Vm.Api/
    ├── Tocken.Api/
    └── WorkerService/
```

### Core/Applications

Her Application projesi şu yapıya sahiptir: `Services/`, `Filters/`, `Extentions/`, `Interceptors/`, `RabbitMq/Consumers/`

```
src/
└── Core/
    └── Applications/
        ├── Bank.Application/
        ├── Web.Application/
        ├── Api.Application/
        ├── File.Application/
        ├── Log.Application/
        ├── MailSms.Application/
        ├── Notification.Application/
        ├── Tocken.Application/
        ├── WorkerService.Application/
        ├── GoogleService.Application/
        ├── Integration.Application/
        ├── Vm.Application/
        ├── Ocpp.Application/
        ├── Station.Application/
        ├── ManagerRootAdmin.Application/
        ├── FirmIntegration.Application/
        └── FirmIntegrationWorkerService.Application/
```

### Core/Persistences

Her Persistence projesi şu yapıya sahiptir: `DbContext/`, `EntityFluent/`, `Migrations/`, `Repositories/`

```
src/
└── Core/
    └── Persistences/
        ├── Bank.Persistence/
        ├── Web.Persistence/
        ├── Api.Persistence/
        ├── File.Persistence/
        ├── Log.Persistence/
        ├── MailSms.Persistence/
        ├── Notification.Persistence/
        ├── Tocken.Persistence/
        ├── WorkerService.Persistence/
        ├── GoogleService.Persistence/
        ├── Integration.Persistence/
        ├── Vm.Persistence/
        ├── Ocpp.Persistence/
        ├── Station.Persistence/
        └── ManagerRootAdmin.Persistence/
```

### Shared

```
src/
└── Shared/
    └── Shared.Domain/
```

### FirmIntegration (Ayrı Alt Sistem)

```
src/
└── FirmIntegration/
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

---

## Genel Proje Sayısı

| Katman | Proje Sayısı |
|--------|-------------|
| Presentation (API) | 16 |
| Applications | 16 |
| Persistences | 15 |
| FirmIntegration alt sistemi | 8 |
| Framework/FrameworkCore | 1 (çok katmanlı) |
| Shared | 1 |
| **Toplam** | **~57** |
