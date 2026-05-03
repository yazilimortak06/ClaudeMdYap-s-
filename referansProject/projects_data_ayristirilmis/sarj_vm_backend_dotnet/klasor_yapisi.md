# sarj_vm_backend_dotnet — Klasör Yapısı

## Genel Bakış
.NET tabanlı VM/şarj cihazı odaklı microservice backend. 4 uygulama servisi içerir: Vm.Api, VmPanel.Api, VmLog.Api ve GateWay.Api (Ocelot). 4 ortam için docker-compose yapısı vardır.

---

## FrameworkCore

`sarj_backend_dotnet` ile aynı veya türevi framework:

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
        │   ├── WrapperCore/
        │   ├── FilterAttributeCore/
        │   ├── ProblemDetailCore/
        │   ├── Repository/
        │   └── UnitOfWorkCore/
        └── Utils/
```

---

## src/

```
src/
│
├── Core/
│   ├── Applications/
│   │   ├── Vm.Application/
│   │   │   ├── Services/           — Bağlantı, transaction, OCPP işlemleri
│   │   │   ├── Filters/
│   │   │   ├── Extentions/
│   │   │   ├── Interceptors/
│   │   │   └── RabbitMq/
│   │   │       └── Consumers/
│   │   ├── VmPanel.Application/
│   │   │   ├── Services/           — Auth, CPO yönetimi, istasyon konfigürasyonu
│   │   │   ├── Filters/
│   │   │   └── Extentions/
│   │   └── VmLog.Application/
│   │       ├── Consumers/          — RabbitMQ consumer'lar (log alıcı)
│   │       └── Services/
│   │
│   └── Persistences/
│       ├── Vm.Persistence/
│       │   ├── DbContext/
│       │   ├── EntityFluent/
│       │   ├── Migrations/
│       │   └── Repositories/
│       └── VmLog.Persistence/
│           ├── DbContext/
│           ├── Migrations/
│           └── Repositories/
│
├── Presentation/
│   ├── Vm.Api/
│   │   ├── Controllers/
│   │   │   ├── ConnectionController.cs     — Cihaz bağlantı yönetimi
│   │   │   ├── TransactionController.cs    — Şarj transaction'ları
│   │   │   └── OcppController.cs           — OCPP mesaj işleme
│   │   ├── Program.cs
│   │   ├── Startup.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   │
│   ├── VmPanel.Api/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── AdminController.cs
│   │   │   ├── CpoController.cs            — CPO yönetimi
│   │   │   └── StationController.cs        — İstasyon konfigürasyonu
│   │   ├── Program.cs
│   │   ├── Startup.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   │
│   ├── VmLog.Api/
│   │   ├── Program.cs
│   │   ├── Startup.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   │   (NOT: HTTP endpoint minimal — öncelikli olarak RabbitMQ consumer)
│   │
│   └── GateWay.Api/
│       ├── ocelot.json                     — Base Ocelot konfigürasyonu
│       ├── ocelot.Development.json
│       ├── ocelot.Local.json
│       ├── ocelot.Test.json
│       ├── ocelot.Production.json
│       ├── Program.cs
│       ├── Startup.cs
│       ├── appsettings.json
│       └── Dockerfile
│
└── Shared/
    └── Shared.Domain/
        ├── DTOs/
        │   ├── Vm/
        │   ├── VmPanel/
        │   └── Common/
        └── Interfaces/
```

---

## Docker-Compose Yapısı (4 Ortam)

```
docker-compose.dev.yml
docker-compose.dev.override.yml

docker-compose.local.yml
docker-compose.local.override.yml

docker-compose.test.yml
docker-compose.test.override.yml

docker-compose.prod.yml
docker-compose.prod.override.yml
```

Her ortam için:
- Base dosya (`docker-compose.[env].yml`): Servis tanımları, image adları
- Override dosya (`docker-compose.[env].override.yml`): Port'lar, ortama özgü env variables, volume mount'lar

---

## Genel Notlar

| Servis | Port Grubu | DB | RabbitMQ |
|---|---|---|---|
| gateway.api | Dışa açık | - | - |
| vm.api | Internal | SQL Server | Hem publish hem consume |
| vmpanel.api | Internal | SQL Server | Opsiyonel |
| vmlog.api | Internal | SQL Server | Sadece consume |
