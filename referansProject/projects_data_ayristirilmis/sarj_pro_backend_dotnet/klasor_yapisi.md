# sarj_pro_backend_dotnet — Klasör Yapısı

## Genel Bakış
`sarj_backend_dotnet` (RotaWattBackEnd) projesinin fork'u. Aynı mimari ve framework yapısı, farklı servis kapsamı (13+ API) ve farklı konfigürasyonlar.

**Fark referansı için bkz:** `sarj_pro_backend_dotnet/farklar.md`

---

## Framework/Core/FrameworkCore/

`sarj_backend_dotnet` ile birebir aynı yapı:

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

## src/ — Presentation (API Katmanı)

`sarj_backend_dotnet`'ten çıkarılan veya azaltılan servisler var. Mevcut API'ler:

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
    ├── Station.Api/
    ├── Ocpp.Api/
    ├── Tocken.Api/
    ├── Integration.Api/          (opsiyonel)
    └── WorkerService/            (hosted service)
```

**Çıkarılan servisler** (sarj_backend_dotnet'e kıyasla):
- `GoogleService.Api` — çıkarılmış veya birleştirilmiş
- `Vm.Api` — ayrı proje olarak taşınmış (`sarj_vm_backend_dotnet`)
- `FirmIntegration.Api` — çıkarılmış veya basitleştirilmiş

---

## src/ — Core/Applications

```
src/
└── Core/
    └── Applications/
        ├── Bank.Application/
        ├── Web.Application/
        ├── Mobil.Application/
        ├── Log.Application/
        ├── Notification.Application/
        ├── WorkerService.Application/
        ├── File.Application/
        ├── MailSms.Application/
        ├── Station.Application/
        ├── Ocpp.Application/
        ├── Tocken.Application/
        └── Integration.Application/
```

---

## src/ — Core/Persistences

```
src/
└── Core/
    └── Persistences/
        ├── Bank.Persistence/
        ├── Web.Persistence/
        ├── Mobil.Persistence/
        ├── Log.Persistence/
        ├── Notification.Persistence/
        ├── WorkerService.Persistence/
        ├── File.Persistence/
        ├── MailSms.Persistence/
        ├── Station.Persistence/
        ├── Ocpp.Persistence/
        ├── Tocken.Persistence/
        └── Integration.Persistence/
```

---

## src/ — Shared

```
src/
└── Shared/
    └── Shared.Domain/
```

---

## Her API Projesi İç Yapısı

`sarj_backend_dotnet` ile özdeş:

```
[ServiceName].Api/
├── Controllers/
├── Program.cs
├── Startup.cs
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile
```

---

## Genel Proje Sayısı

| Katman | Proje Sayısı |
|--------|-------------|
| Presentation (API) | ~13 |
| Applications | ~12 |
| Persistences | ~12 |
| Framework/FrameworkCore | 1 (çok katmanlı) |
| Shared | 1 |
| **Toplam** | **~39** |

---

## sarj_backend_dotnet vs sarj_pro_backend_dotnet Yapı Farkı

| Özellik | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Toplam API | 20+ | 13+ |
| FirmIntegration | 8 proje alt sistemi | Yok / Basitleştirilmiş |
| GoogleService | Var | Yok |
| Vm.Api | Var (büyük) | Ayrı projeye taşınmış |
| Image prefix | `rotawatt/` | `sarjpro/` |
