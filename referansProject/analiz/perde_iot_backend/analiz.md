# perde_iot_backend — Tam Analiz (Yeniden Yazildi)

## Platform ve Tech Stack

| Bilesen | Versiyon / Detay |
|---|---|
| Framework | .NET 8 |
| Uygulama Tipi | ASP.NET MVC (web arayuzlu) |
| DI Container | Autofac 6.3.0 |
| Mapping | AutoMapper 11.0.1 |
| Container | Docker (Linux base image) |
| Base | BaseStartup kalitimi |
| DI Yukleme | Dinamik DLL yukleme (Autofac RegisterAssemblyTypes) |
| Proje Adi | PixdinnPerdeci |

---

## Genel Bakis

`perde_iot_backend` (PixdinnPerdeci), IoT perde kontrol sistemi backend'idir. .NET 8 MVC mimarisi kullanilmistir. Web arayuzu icerir (Views klasoru) ve ayni zamanda service/serviceinterface pattern'i ile is mantigi soyutlanmistir.

Bu proje `sarj_vm_backend_dotnet` ile ayni ekipten geldigini gosteren ortak pattern'ler icerir: BaseStartup kalitimi, Autofac, AutoMapper, Dinamik DLL yukleme.

---

## Mimari Pattern

```
Controllers/     <- HTTP istegi alir, servisi cagirir, view doner
    |
Services/        <- Is mantigi implementasyonu
    ^
ServiceInterfaces/  <- Service abstract tanimlari (interface'ler)
    |
Models/          <- ViewModel ve data modelleri
Views/           <- Razor view'lar (.cshtml)
Bases/           <- Base controller ve servis siniflari
```

---

## Proje Klasor Yapisi (Gercek)

```
PixdinnPerdeci/
├── Bases/
│   ├── Models/
│   │   └── ApiOptions.cs
│   └── StartupBase/
│       ├── BaseStartup.cs
│       └── ApiConfigureBaseContainerExtensions.cs
├── Controllers/
│   └── HomeController.cs          (tek controller, iskelet proje)
├── Models/
│   └── ErrorViewModel.cs
├── Services/
│   └── Class.cs                   (bos, eklenecek)
├── ServiceInterfaces/
│   └── Class.cs                   (bos, eklenecek)
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── Error.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── Startup.cs
└── Dockerfile
```

**Onemli not**: Bu proje henuz iskelet asamasindadir. Controllers, Services, ServiceInterfaces klasorleri bos class dosyalari icermektedir. Gercek IoT perde mantigi henuz eklenmemistir veya baska bir dala tasimistir.

---

## Program.cs (tam kod)

```csharp
using Autofac.Extensions.DependencyInjection;
using Autofac;
using PixdinnPerdeci;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder.Extensions;
using PixdinnPerdeci.Core.Models;
using PixdinnPerdeci.Bases.StartupBase;

var builder = WebApplication.CreateBuilder(args);

#region autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.ConfigureServices(new ApiOptions()
    {
        ApiClientList = null,
        ApiName = "PixdinnPerdeci",
        RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory,
                        "*.dll", SearchOption.TopDirectoryOnly)
               .Where(filePath => Path.GetFileName(filePath).StartsWith("PixdinnPerdeci")
                               || Path.GetFileName(filePath).StartsWith("Shared"))
               .Select(Assembly.LoadFrom)
    });
});
#endregion

#region startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
builder.Services.AddControllersWithViews();
var app = builder.Build();
startup.Configure(app, builder.Environment);
#endregion
```

**Not**: Dinamik DLL yukleme pattern'i: `PixdinnPerdeci*` ve `Shared*` ile baslayan DLL'ler runtime'da yuklenir. `sarj_backend_dotnet`'te ayni pattern kullanilir.

---

## Startup.cs (tam kod)

```csharp
using Autofac;
using PixdinnPerdeci.Bases.Models;
using PixdinnPerdeci.Bases.StartupBase;

namespace PixdinnPerdeci
{
    public class Startup : BaseStartup
    {
        public ApiOptions ApiOptions { get; set; }

        public Startup(IConfiguration configuration)
                : base(configuration)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddRazorPages();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            ConfigureBuilderInit(app, env);
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
```

---

## BaseStartup.cs (tam kod)

```csharp
using PixdinnPerdeci.Bases.Models;
using System.Reflection;

namespace PixdinnPerdeci.Bases.StartupBase
{
    public class BaseStartup
    {
        public IConfiguration Configuration { get; }
        public ApiOptions ApiOptions { get; set; }
        public string ProjectPrefix;

        public BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApiOptions = new ApiOptions()
            {
                ApiClientList = null,
                ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
                RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory,
                        "*.dll", SearchOption.TopDirectoryOnly)
               .Where(filePath => Path.GetFileName(filePath).StartsWith(ProjectPrefix)
                               || Path.GetFileName(filePath).StartsWith("Shared"))
               .Select(Assembly.LoadFrom)
            };
        }

        public string GetAppSettingValue(string name)
        {
            if (Configuration.GetSection(name) == null) return "";
            return Configuration.GetSection(name).Value;
        }

        public void ConfigureBuilderInit(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("_myAllowSpecificOrigins");
            app.UseRouting();
        }
    }
}
```

---

## ApiConfigureBaseContainerExtensions.cs (tam kod)

```csharp
using Autofac;
using PixdinnPerdeci.Bases.Models;

namespace PixdinnPerdeci.Bases.StartupBase
{
    public static class ApiConfigureBaseContainerExtensions
    {
        public static void ConfigureServices(this ContainerBuilder builder, ApiOptions options)
        {
            // Convention-based kayit: Name "Service" ile biten siniflar, implement ettigi interface'ler ile kayit olur
            builder.RegisterAssemblyTypes(options.RegistrationAssemblies.ToArray())
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();
        }
    }
}
```

---

## ApiOptions.cs (tam kod)

```csharp
using System.Reflection;

namespace PixdinnPerdeci.Bases.Models
{
    public class ApiOptions
    {
        public IEnumerable<Assembly> RegistrationAssemblies { get; set; }
        public IEnumerable<Type> HubListSignalR { get; set; }
        public IEnumerable<Type> ApiClientList { get; set; }
        public string ApiName { get; set; }
    }
}
```

---

## HomeController.cs (tam kod)

```csharp
using Microsoft.AspNetCore.Mvc;
using PixdinnPerdeci.Models;
using System.Diagnostics;

namespace PixdinnPerdeci.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
```

---

## ErrorViewModel.cs (tam kod)

```csharp
namespace PixdinnPerdeci.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
```

---

## Services/Class.cs ve ServiceInterfaces/Class.cs (mevcut — bos)

```csharp
// Services/Class.cs — henuz implementasyon eklenmemis
namespace PixdinnPerdeci.Services
{
    public class Class { }
}

// ServiceInterfaces/Class.cs — henuz interface eklenmemis
namespace PixdinnPerdeci.ServiceInterfaces
{
    public class Class { }
}
```

---

## appsettings.json (tam, maskelenmis)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Not**: `StartupConfigs:ProjectPrefix` ve `StartupConfigs:ApiName` appsettings'te tanimlanmamis. Bu degerlerin `appsettings.Development.json` veya environment variable'lardan gelmesi bekleniyor.

---

## View'lar

### Views/Home/Index.cshtml

```html
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>
```

### Views/Shared/_Layout.cshtml

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - PixdinnPerdeci</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/PixdinnPerdeci.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">PixdinnPerdeci</a>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>
    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2024 - PixdinnPerdeci - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

## Proje Durumu Degerlendirmesi

Bu proje henuz **iskelet (scaffold)** asamasindadir. Temel altyapi kurulmuş, ancak asil IoT is mantigi eklenmemistir.

### Mevcut Olan
- Program.cs ve Startup.cs (calisan)
- BaseStartup kalitimi (Autofac, AutoMapper)
- HomeController (index/privacy/error)
- _Layout.cshtml ve view'lar
- Dockerfile
- CORS yapisi (`_myAllowSpecificOrigins` policy)

### Eksik Olan (henuz eklenmemis)
- IoT perde cihazi iletisim servisi (MQTT, HTTP veya WebSocket)
- Cihaz durumu izleme servisleri
- Zamanlayici / otomasyon servisleri
- Kullanici auth servisi
- Device model/entity siniflari
- Gercek controller'lar (DeviceController, CurtainController vb.)

---

## .NET 8 vs Diger Projeler

| Proje | .NET Versiyonu | Not |
|---|---|---|
| sarj_backend_dotnet | .NET 5 | Eski, LTS disi |
| crm_backend | .NET 5 | Eski, LTS disi |
| sarj_vm_backend_dotnet | .NET 5/6 | Karma |
| perde_iot_backend | .NET 8 | En guncel, LTS 2026 |

---

## Autofac Convention-Based Kayit

```
RegisterAssemblyTypes("PixdinnPerdeci*.dll", "Shared*.dll")
    .Where(t => t.Name.EndsWith("Service"))
    .AsImplementedInterfaces()
```

Ornekler:
- `DeviceService : IDeviceService` -> otomatik kayit
- `CurtainService : ICurtainService` -> otomatik kayit
- Yeni service eklemek icin sadece `*Service` adini vermek yeterli

---

## Notlar

- `appsettings.json` cok sade — uretim konfigurasyonu environment variable'lardan gelmeli
- CORS policy adi `_myAllowSpecificOrigins` — `BaseStartup.ConfigureBuilderInit`'te `UseCors` cagirilmis ama `AddCors` servisi `Startup.ConfigureServices`'ta tanimlanmamis, bu bir bug olabilir
- `app.MapRazorPages()` ile `app.MapControllerRoute()` her ikisi de kayitli — hem Razor Pages hem MVC endpoint'leri aktif
- `app.Run()` `Configure()` metodunun sonunda cagriliyor — .NET 8 minimal API yapisinda bu standart
