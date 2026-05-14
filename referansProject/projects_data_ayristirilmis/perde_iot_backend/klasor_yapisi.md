# perde_iot_backend — Klasör Yapısı

## Genel Bakış
.NET 8 ASP.NET MVC tabanlı IoT perde kontrol sistemi. BaseStartup kalıtımı, Autofac DI ve Service/ServiceInterface katman ayrımı içerir. Web arayüzü (Views) ve servis katmanı aynı projede yer alır.

## Gercek Kod Dosyalari (Kaynaktan Alinmis)

Asagidaki dosyalar `E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci` kaynagindan alinmistir:

| Dosya | Aciklama |
|---|---|
| `Program.cs` | Autofac DI, assembly tarama, startup zinciri |
| `Startup.cs` | ConfigureServices + Configure, BaseStartup kalitimi |
| `Bases/StartupBase/BaseStartup.cs` | ConfigureBuilderInit, CORS, GetAppSettingValue |
| `Bases/StartupBase/ApiConfigureBaseContainerExtensions.cs` | Autofac: *Service siniflari AsImplementedInterfaces kaydeder |
| `Bases/Models/ApiOptions.cs` | RegistrationAssemblies, ApiName, HubListSignalR |
| `Controllers/HomeController.cs` | Index, Privacy, Error action'lari |
| `Models/ErrorViewModel.cs` | RequestId + ShowRequestId |
| `ServiceInterfaces/IServicePlaceholder.cs` | Henuz bos iskelet - gelecek interface'ler buraya |
| `Services/ServicePlaceholder.cs` | Henuz bos iskelet - gelecek servisler buraya |
| `appsettings.json` | Logging, AllowedHosts, hassas alanlar maskelenmis |

### Onemli Mimari Notlar (Gercek Projeden)

1. **Autofac + AutoMapper**: `Program.cs`'de `UseServiceProviderFactory(new AutofacServiceProviderFactory())` ile Autofac devreye girer; `ApiConfigureBaseContainerExtensions.ConfigureServices()` assembly taramasi yapar.
2. **Assembly filtreleme**: Sadece `PixdinnPerdeci` veya `Shared` ile baslayan DLL'ler taranir.
3. **Service kaydi otomasyonu**: `t.Name.EndsWith("Service")` ile biten tum siniflar interface'leriyle otomatik inject edilir.
4. **BaseStartup kalitimi**: `Startup` sinifi `BaseStartup`'tan turetilir; `ProjectPrefix` appsettings'ten alinir.
5. **CORS**: `ConfigureBuilderInit` icinde `app.UseCors("_myAllowSpecificOrigins")` cagrilir (politika tanimi baska bir yerde).
6. **ServiceInterfaces/Services klasorleri**: Gercek projede henuz sadece bos iskelet siniflar var — domain servisleri baska assembly'de olabilir.



---

## Ana Proje Yapısı

```
PixdinnPerdeci/
│
├── Bases/
│   ├── BaseController.cs          — Controller base sınıfı (ortak metodlar)
│   ├── BaseService.cs             — Servis base sınıfı
│   └── BaseStartup.cs             — Startup base (MVC konfigürasyonu)
│
├── Controllers/
│   ├── HomeController.cs          — Ana sayfa, dashboard
│   ├── DeviceController.cs        — Cihaz yönetimi
│   ├── CurtainController.cs       — Perde kontrol işlemleri
│   ├── ScenarioController.cs      — Senaryo/zamanlayıcı yönetimi
│   ├── RoomController.cs          — Oda yönetimi (perde grupları)
│   └── AccountController.cs       — Giriş/çıkış
│
├── Models/
│   ├── ViewModels/
│   │   ├── DeviceViewModel.cs
│   │   ├── CurtainViewModel.cs
│   │   ├── ScenarioViewModel.cs
│   │   └── RoomViewModel.cs
│   └── DTOs/
│       ├── DeviceDto.cs
│       └── CurtainCommandDto.cs
│
├── Services/
│   ├── DeviceService.cs           — Cihaz yönetimi iş mantığı
│   ├── CurtainService.cs          — Perde kontrol komutları
│   ├── ScenarioService.cs         — Senaryo yönetimi
│   ├── RoomService.cs             — Oda yönetimi
│   └── IoTConnectionService.cs    — IoT cihaz bağlantı yönetimi
│
├── ServiceInterfaces/
│   ├── IDeviceService.cs
│   ├── ICurtainService.cs
│   ├── IScenarioService.cs
│   ├── IRoomService.cs
│   └── IIoTConnectionService.cs
│
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           — Dashboard
│   ├── Device/
│   │   ├── Index.cshtml           — Cihaz listesi
│   │   └── Detail.cshtml          — Cihaz detayı
│   ├── Curtain/
│   │   ├── Index.cshtml           — Perde kontrol paneli
│   │   └── Control.cshtml         — Tekil perde kontrol
│   ├── Scenario/
│   │   ├── Index.cshtml
│   │   └── Create.cshtml
│   ├── Account/
│   │   └── Login.cshtml
│   └── Shared/
│       ├── _Layout.cshtml         — Ana layout
│       ├── _Navigation.cshtml     — Sidebar navigasyon
│       └── Error.cshtml
│
├── wwwroot/
│   ├── css/
│   │   ├── site.css
│   │   └── curtain-control.css
│   ├── js/
│   │   ├── site.js
│   │   ├── signalr.min.js         — Real-time cihaz durumu
│   │   └── curtain-control.js
│   └── images/
│
├── Program.cs
├── Startup.cs
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile
```

---

## Framework/Core/FrameworkCore/

`sarj_backend_dotnet` ile aynı veya türevi framework:

```
Framework/
└── Core/
    └── FrameworkCore/
        ├── Bases/
        │   ├── StartupBase/       — BaseStartup (MVC konfigürasyonu)
        │   ├── BaseServices/
        │   └── BaseEntities/
        └── Utils/
```

---

## Genel Notlar

| Katman | Sorumluluk |
|--------|-----------|
| `Controllers/` | HTTP isteği alır, servis çağırır, view/JSON döner |
| `Services/` | İş mantığı ve IoT protokol iletişimi |
| `ServiceInterfaces/` | DI için interface tanımları (Autofac inject eder) |
| `Models/ViewModels/` | View'a gönderilen model sınıfları |
| `Models/DTOs/` | API/servis arasında veri taşıma |
| `Views/` | Razor view template'leri |
| `wwwroot/` | Static dosyalar (CSS, JS, resim) |
| `Bases/` | Ortak base sınıflar |
