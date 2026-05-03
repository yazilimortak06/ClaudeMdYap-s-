# perde_iot_backend — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | .NET 8 |
| Uygulama Tipi | ASP.NET MVC (web arayüzlü) |
| DI Container | Autofac 6.3.0 |
| Mapping | AutoMapper 11.0.1 |
| Container | Docker (Linux base image) |
| Base | BaseStartup kalıtımı |
| DI Yükleme | Dinamik DLL yükleme (`*.*` pattern) |

## Genel Bakış

`perde_iot_backend` (PixdinnPerdeci), IoT perde kontrol sistemi backend'idir. .NET 8 MVC mimarisi kullanılmıştır. Web arayüzü içerir (Views klasörü) ve aynı zamanda servis interface pattern'i ile iş mantığı soyutlanmıştır.

## Mimari Pattern

**MVC Pattern + Service/ServiceInterface Layer + BaseStartup Kalıtımı**

```
Controllers/     ← HTTP isteği alır, servisi çağırır, view döner
    ↓
Services/        ← İş mantığı implementasyonu
    ↑
ServiceInterfaces/  ← Service abstract tanımları (interface'ler)
    ↕
Models/          ← ViewModel ve data modelleri
Views/           ← Razor view'lar (.cshtml)
Bases/           ← Base controller ve servis sınıfları
```

## Proje Klasör Yapısı

```
PixdinnPerdeci/
├── Bases/                   — BaseController, BaseService, BaseStartup
├── Controllers/             — MVC controller'lar
│   ├── HomeController.cs
│   ├── DeviceController.cs
│   ├── CurtainController.cs
│   └── ...
├── Models/                  — ViewModel'lar, DTO'lar
├── Services/                — Servis implementasyonları
│   ├── DeviceService.cs
│   ├── CurtainService.cs
│   └── ...
├── ServiceInterfaces/       — Servis interface'leri
│   ├── IDeviceService.cs
│   ├── ICurtainService.cs
│   └── ...
├── Views/                   — Razor view'lar
│   ├── Home/
│   ├── Device/
│   └── Shared/
├── wwwroot/                 — Static dosyalar (CSS, JS, images)
├── Bases/
├── Program.cs
├── Startup.cs
└── Dockerfile
```

## Program.cs — Dinamik DLL Yükleme

```csharp
// Autofac ServiceProviderFactory kullanımı
Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureWebHostDefaults(webBuilder => {
        webBuilder.UseStartup<Startup>();
    });
```

Dinamik DLL yükleme pattern'i: `*.*` glob ile dll'ler runtime'da yüklenir. Bu yaklaşım plugin-based veya modüler mimaride kullanılır; startup'ta tüm assembly'ler taranarak Autofac'a kayıt yapılır.

## Startup.cs — BaseStartup Kalıtımı

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration) : base(configuration) { }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddAutoMapper(typeof(Startup));
        services.AddControllersWithViews();
    }

    public override void ConfigureContainer(ContainerBuilder builder)
    {
        base.ConfigureContainer(builder);
        // Servis kayıtları — dinamik DLL tarama ile
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
```

## IoT Perde Kontrol Sistemi

Bu uygulamanın IoT katmanı muhtemelen şunları içerir:
- Perde cihazlarıyla MQTT veya HTTP protokolü üzerinden iletişim
- Cihaz durumu (açık/kapalı/yüzde) izleme
- Zamanlayıcı/otomasyonla kontrol
- Web arayüzü üzerinden manuel kontrol

## .NET 8 Avantajı

`sarj_backend_dotnet` ve `crm_backend` .NET 5 kullanırken bu proje .NET 8 ile geliştirilmiştir. Bu proje, daha yeni bir geliştirme sürecinde oluşturulmuş veya upgrade edilmiştir. .NET 8 LTS sürümüdür; 2026'ya kadar destek garantisi vardır.

## Docker — Linux Base Image

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ...
```

Linux base image, hem Docker Desktop hem de Linux server'da çalışır. Windows container gerektirmez.

## Autofac 6.3.0

.NET 8 ile uyumlu güncel Autofac versiyonu. `AutofacServiceProviderFactory` kullanımı .NET 5'teki pattern ile aynıdır; sadece versiyon güncellenmiştir.

## Projeler Arası Karşılaştırma

| Özellik | perde_iot_backend | sarj_backend_dotnet |
|---|---|---|
| .NET Versiyon | .NET 8 | .NET 5 |
| Uygulama Tipi | MVC (web UI var) | API only |
| DI Container | Autofac 6.3.0 | Autofac |
| BaseStartup | Var | Var |
| Microservice | Değil (monolith MVC) | Evet (20+) |
| Docker | Var | Var |

## Dikkat Çeken Noktalar

### Olumlu
- .NET 8 LTS — güncel ve desteklenen versiyon
- BaseStartup pattern `sarj_backend_dotnet` ile tutarlı — aynı ekip
- Service/ServiceInterface ayrımı test edilebilirlik sağlar
- Docker ile konteynerize

### İyileştirme Alanları
- Dinamik DLL yükleme (`*.*` pattern) güvenlik riski taşır. Sadece belirli namespace'den DLL'ler yüklenmeli
- MVC + IoT kombinasyonu: IoT cihaz iletişimi için WebSocket veya MQTT ayrı bir katmana çıkarılabilir
- Views ve API endpoint'ler aynı projede — frontend ayrılabilir (Angular/React)

## Sonuç

Bu proje, IoT perde kontrol sistemi için .NET 8 MVC backend'dir. `sarj_backend_dotnet` ile aynı ekipten geldiği anlaşılıyor (BaseStartup, Autofac, AutoMapper pattern'leri aynı). .NET 8 kullanılması bu projenin daha güncel bir geliştirme sürecinde olduğunu gösteriyor. Küçük-orta ölçekli IoT kontrol sistemi için yeterli, ölçeklenmesi gerekirse API katmanı ayrıştırılabilir.
