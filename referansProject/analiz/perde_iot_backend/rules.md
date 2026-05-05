# perde_iot_backend — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
.NET 8 MVC IoT backend geliştirirken referans olarak kullanılabilir.

---

## 1. .NET 8 MVC Proje Yapısı

**Kural:** IoT kontrol uygulamaları için .NET 8 MVC tercih et. Hem web arayüzü hem de API sunar:
```
ProjectName/
├── Bases/               — Base controller, service sınıfları
├── Controllers/         — HTTP controller'lar
├── Models/              — ViewModel, DTO
├── Services/            — İş mantığı implementasyonu
├── ServiceInterfaces/   — Servis interface'leri
├── Views/               — Razor template'leri
├── wwwroot/             — Static assets
├── Program.cs
├── Startup.cs
└── Dockerfile
```

**Kural:** Service/ServiceInterface ayrımı yap. Controller doğrudan concrete servis çağırmasın:
```csharp
// Controller
public class CurtainController : Controller
{
    private readonly ICurtainService _curtainService;

    public CurtainController(ICurtainService curtainService)
    {
        _curtainService = curtainService;
    }
}
```

---

## 2. BaseStartup Pattern (.NET 8)

**Kural:** .NET 8'de `Startup.cs` + `BaseStartup` kalıtımı:

```csharp
// Startup.cs
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration) : base(configuration) { }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddAutoMapper(typeof(Startup));
        services.AddControllersWithViews();
        // IoT servis bağlantıları
        services.AddSingleton<IIoTConnectionService, IotMqttService>();
    }

    public override void ConfigureContainer(ContainerBuilder builder)
    {
        base.ConfigureContainer(builder);
        // Servis/repository kayıtları
        builder.RegisterType<CurtainService>()
               .As<ICurtainService>()
               .InstancePerLifetimeScope();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
```

**Kural:** `Program.cs`'de Autofac factory'yi kaydet:
```csharp
var builder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureWebHostDefaults(webBuilder => {
        webBuilder.UseStartup<Startup>();
    });
```

---

## 3. Autofac 6.x ile .NET 8

**Kural:** Autofac 6.3.0+ kullan. Eski `ConfigureContainer(IApplicationBuilder)` override yerine yeni API:

```csharp
// Autofac 6.x ile doğru kullanım
public void ConfigureContainer(ContainerBuilder builder)
{
    // IServiceCollection değil, doğrudan ContainerBuilder
    builder.RegisterType<DeviceService>()
           .As<IDeviceService>()
           .InstancePerLifetimeScope();
}
```

**Kural:** Autofac module'leri ile kayıtları grupla:
```csharp
public class ServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CurtainService>().As<ICurtainService>().InstancePerLifetimeScope();
        builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
    }
}

// Startup'ta
builder.RegisterModule<ServiceModule>();
```

---

## 4. Dinamik DLL Yükleme — Güvenli Kullanım

**Kural:** Dinamik DLL yüklemede `*.*` pattern yerine spesifik namespace filtresi kullan:
```csharp
// RISKLI — tüm DLL'leri yükler
var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*")
    .Where(f => f.EndsWith(".dll"))
    .Select(Assembly.LoadFrom);

// GÜVENLİ — sadece kendi namespace'inden
var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "PixdinnPerdeci*.dll")
    .Select(Assembly.LoadFrom);
```

**Kural:** Dinamik yüklenen assembly'leri Autofac'a kayıt ederken sadece belirli interface'leri implement eden tipleri al:
```csharp
builder.RegisterAssemblyTypes(assemblies.ToArray())
       .Where(t => t.Name.EndsWith("Service"))
       .AsImplementedInterfaces()
       .InstancePerLifetimeScope();
```

---

## 5. IoT Cihaz İletişimi Pattern

**Kural:** IoT cihaz bağlantısını ayrı bir servis katmanında izole et. Controller IoT protokolünü bilmemeli:

```csharp
public interface ICurtainDeviceService
{
    Task<bool> OpenCurtainAsync(string deviceId, int percentage);
    Task<bool> CloseCurtainAsync(string deviceId);
    Task<CurtainStatus> GetStatusAsync(string deviceId);
}

// MQTT implementasyonu
public class MqttCurtainDeviceService : ICurtainDeviceService
{
    // MQTT bağlantısı ve mesaj gönderimi
}

// HTTP implementasyonu
public class HttpCurtainDeviceService : ICurtainDeviceService
{
    // HTTP API çağrısı
}
```

**Kural:** IoT bağlantı servisi `AddSingleton<>` ile kaydet — bağlantı state'i uygulama boyunca yaşamalı. `InstancePerLifetimeScope` ile kayıt yapma; her request'te yeni bağlantı açılır.

**Kural:** Cihaz durumunu gerçek zamanlı göstermek için SignalR veya Server-Sent Events (SSE) kullan. Polling yerine push mekanizması tercih et.

---

## 6. AutoMapper 11.x Konfigürasyonu

**Kural:** AutoMapper 11'de static API kaldırıldı. Constructor injection ile kullan:
```csharp
// Startup'ta
services.AddAutoMapper(typeof(Startup));

// Profile tanımı
public class CurtainMappingProfile : Profile
{
    public CurtainMappingProfile()
    {
        CreateMap<CurtainEntity, CurtainViewModel>();
        CreateMap<CreateCurtainDto, CurtainEntity>();
    }
}

// Servis içinde
public class CurtainService : ICurtainService
{
    private readonly IMapper _mapper;

    public CurtainService(IMapper mapper)
    {
        _mapper = mapper;
    }
}
```

---

## 7. MVC View Pattern

**Kural:** Razor view'lar `Views/[ControllerName]/[ActionName].cshtml` konvansiyonunu takip etsin.

**Kural:** Ortak layout `Views/Shared/_Layout.cshtml`'de tanımlanmalı.

**Kural:** Cihaz durumu gerçek zamanlı gösterilecekse view'da JavaScript SignalR client kullan:
```html
<!-- _Layout.cshtml -->
<script src="~/js/signalr/dist/browser/signalr.js"></script>
<script>
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/deviceHub")
    .build();

connection.on("DeviceStatusUpdated", (deviceId, status) => {
    document.getElementById(`device-${deviceId}`).innerText = status;
});

connection.start();
</script>
```

---

## 8. Docker .NET 8

**Kural:** .NET 8 için Dockerfile:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PixdinnPerdeci/PixdinnPerdeci.csproj", "PixdinnPerdeci/"]
RUN dotnet restore "PixdinnPerdeci/PixdinnPerdeci.csproj"
COPY . .
WORKDIR "/src/PixdinnPerdeci"
RUN dotnet build "PixdinnPerdeci.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PixdinnPerdeci.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixdinnPerdeci.dll"]
```

**Kural:** Static dosyalar (`wwwroot/`) Docker image'a dahil edilsin. `COPY . .` komutu bunu sağlar.
