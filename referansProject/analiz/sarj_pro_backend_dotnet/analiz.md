# sarj_pro_backend_dotnet Analiz

Kaynak: `E:\Projeler\Backend\SarjAllPro\`
Platform: .NET 5 (net5.0) — FirmIntegration modulleri net6.0
Solution: `SarjAllPro.sln`

---

## Genel Bakis

SarjAllPro, EV (Elektrikli Arac) sarj istasyonu yonetim platformudur. Microservice mimarisi uzerine kurulmus olup her biri bagimsiz calisan 20+ proje icerir. Tum servisler ortak `FrameworkCore` kutuphanesini ve `Shared.Domain` projesini kullanir.

**Temel ozellikler:**
- OCPP 1.6 protokolu uzerinden WebSocket baglantisi ile fiziksel sarj cihazlarina erisim
- Ocelot API Gateway ile trafik yonlendirme
- RabbitMQ ile asenkron log ve exception yonetimi
- JWT tabanli cok katmanli kimlik dogrulama (Panel Admin, Root Admin, Mobil Kullanici)
- AutoMapper + FluentValidation + Autofac DI kombinasyonu
- EF Core Code First + UnitOfWork + Repository pattern

---

## API Listesi

| No | Proje Adi | Namespace | Rol | DbContext |
|----|-----------|-----------|-----|-----------|
| 1 | Web.Api | Web.Api | Panel Web API (Login, Admin, Config) | SarjAllProDbContext |
| 2 | Tocken.Api | Tocken.Api | JWT Token ve Session Yonetimi | TockenDbContext |
| 3 | Station.Api | Station.Api | Istasyon ve Cihaz Yonetimi | StationDbContext (EvTechDbContext) |
| 4 | Ocpp.Api | Ocpp.Api | OCPP 1.6 WebSocket Server | OcppDbContext |
| 5 | SarjAllMobil.Api | SarjAllMobil.Api | Mobil Uygulama API | - |
| 6 | Mobil.Api | Mobil.Api | Ek Mobil API | - |
| 7 | Bank.Api | Bank.Api | Odeme ve Banka Entegrasyonu | PaymentDbContext |
| 8 | File.Api | File.Api | Dosya Yukleme/Indirme | FileDbContext |
| 9 | Log.Api | Log.Api | Request/Response ve Exception Loglari | LogDbContext |
| 10 | MailSms.Api | MailSms.Api | E-posta ve SMS Servisi | MailSmsDbContext |
| 11 | Notification.Api | Notification.Api | Push Bildirim Servisi | NotificationDbContext |
| 12 | GoogleService.Api | GoogleService.Api | Google API Entegrasyonu | GoogleServiceDbContext |
| 13 | Integration.Api | Integration.Api | Dis Sistem Entegrasyonlari | IntegrationDbContext |
| 14 | WebSite.Api | WebSite.Api | Web Site Backend | - |
| 15 | ManagerRootAdmin.Api | ManagerRootAdmin.Api | Root Admin Yonetimi | - |
| 16 | GateWay.Api | GateWay.Api | **Ocelot API Gateway** | - |
| 17 | WorkerService | WorkerService | Background Worker (HostedService) | WorkerServiceDbContext |
| 18 | WorkerService.Api | WorkerService.Api | Worker Service HTTP API | - |
| 19 | FirmIntegration.Api | FirmIntegration.Api | Firma Entegrasyon API (net6.0) | FirmIntegration DbContext |
| 20 | FirmIntegration.WorkerService | FirmIntegration.WorkerService | Firma Entegrasyon Worker (net6.0) | - |

---

## Proje Yapisi

```
SarjAllPro/
+-- SarjAllPro.sln
+-- Framework/
|   +-- Core/
|       +-- FrameworkCore/          <- Ortak base class'lar, WrapperCore, Repository, UoW
|           +-- Bases/
|           |   +-- BaseEntities/BaseEntity.cs
|           |   +-- BaseRepository/BaseRepository.cs
|           |   +-- BaseServices/BaseService.cs
|           |   +-- BaseUnitOfWork/UnitOfWork.cs
|           |   +-- StartupBase/BaseStartup.cs
|           +-- FrameworkCore/
|               +-- WrapperCore/Models/  <- Result<T>, SuccessResult, ErrorResult
|               +-- Api/ApiOptions.cs
+-- src/
    +-- Shared/
    |   +-- Shared.Domain/          <- Tum interface, DTO, Entity, Enum tanimlari
    |       +-- ContextProviders/   <- IAdminContextProvider, ITockenContextProvider
    |       +-- Dto/                <- ApiDto, TockenDto, OcppDto, SarjAllMobilDto
    |       +-- Entities/           <- ApiEntities, TockenEntities, ...
    |       +-- Enums/              <- ApiEnums, TockenEnums, OcppEnums, Auth
    |       +-- Errors/             <- Hata enum'lari (WebPanel, Tocken, MobilApi)
    |       +-- GeneralAttribute/   <- InnerRequestAttribute
    |       +-- GeneralEnums/       <- ApiName enum
    |       +-- HttpClients/        <- PanelUserClientService, FileClientService
    |       +-- RabbitMq/           <- LogProducer, RabbitmqConstants, Commands
    |       +-- RepositoryIntefaces/
    |       +-- ServiceIntefaces/
    +-- Core/
    |   +-- Applications/           <- Is mantigi servisleri
    |   |   +-- Api.Application/    <- AuthenticationService, PolicyManagmentService
    |   |   +-- Tocken.Application/ <- PanelUserLoginService, JwtTokenService
    |   |   +-- Ocpp.Application/   <- Ocpp16ConnectionService
    |   |   +-- ...
    |   +-- Persistences/           <- EF Core DbContext ve Repository implementasyonlari
    |       +-- Api.Persistence/    <- SarjAllProDbContext, Fluent, DataSeed
    |       +-- Ocpp.Persistence/   <- OcppDbContext
    |       +-- Tocken.Persistence/ <- TockenDbContext
    |       +-- ...
    +-- Presentation/               <- API projeleri (Controller + Startup + Program)
    |   +-- Web.Api/
    |   +-- Tocken.Api/
    |   +-- Ocpp.Api/
    |   +-- Station.Api/
    |   +-- GateWay.Api/
    |   +-- WorkerService/
    |   +-- ...
    +-- FirmIntegration/            <- Ayri firma entegrasyon modulu (net6.0)
        +-- Applications/
        +-- Domains/
        +-- Persistences/
        +-- Presentation/
```

---

## Program.cs ve Startup.cs (Her API Icin)

### Ortak Pattern (Tum API'ler)

Tum API'ler Autofac DI container kullanir. `Program.cs` ayni template'i paylasar:

```csharp
// Program.cs -- Tum standard API'lerde ayni yapi
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

### Tocken.Api Startup.cs

JWT token ve oturum yonetiminin merkezi. RabbitMQ producer kayitli.

```csharp
// src/Presentation/Tocken.Api/Startup.cs
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
        : base(configuration, env)
    {
        base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // DB: TockenDbContext -- token ve session tablolari
        var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
        dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                            GetAppSettingValue("ConnectionStrings:PixdinnConnectionString"),
                            GetAppSettingValue("StartupConfigs:MigrationAssembly")));
        services.AddPixdinnDbService<TockenDbContext>(dbcontextOptions);

        services.AddPixdinnApiService(Configuration, WebHostEnvironment,
                                GetAppSettingValue("StartupConfigs:Policy"),
                                GetAppSettingValue("StartupConfigs:ApiUrl"));
        services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);
        services.AddFilters();
        services.AddFrameworkServices();
        services.AddContextProvider();
        services.RegisterRabbitmq();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureRepositories(ApiOptions);
        builder.ConfigureServices(ApiOptions);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
    }
}
```

### Web.Api Startup.cs

Panel frontend API'si. JWT middleware + authentication aktif.

```csharp
// src/Presentation/Web.Api/Startup.cs
public class Startup : BaseStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // DB: SarjAllProDbContext -- Parameter, Policy, PanelAdmin, Region tablolari
        services.AddPixdinnDbService<SarjAllProDbContext>(dbcontextOptions);
        services.AddPixdinnApiService(...);
        services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);
        services.AddFrameworkServices();
        services.AddFluentValidators();
        services.AddFilters();
        services.RegisterMasstransit();
        services.AddWebJwtTocken(Configuration); // <- JWT middleware
        services.AddContextProvider();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        app.UseAuthentication();   // <- Authentication middleware
        app.UseAuthorization();
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
    }
}
```

### Station.Api Startup.cs

WebSocket destekli. Singleton servisler kayitli.

```csharp
// src/Presentation/Station.Api/Startup.cs
public class Startup : BaseStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // DB: EvTechDbContext (StationDbContext) -- sarj istasyonu verileri
        services.AddPixdinnDbService<EvTechDbContext>(dbcontextOptions);
        services.AddPixdinnApiService(...);
        services.AddFilters();
        services.RegisterMasstransit();
        services.AddFrameworkServices();
        services.RegisterSingletonService(); // <- static dictionary icin singleton
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        var webSocketOptions = new WebSocketOptions() { ReceiveBufferSize = 8 * 1024 };
        app.UseWebSockets(webSocketOptions);  // <- WebSocket destegi
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
    }
}
```

### Ocpp.Api Startup.cs

OCPP 1.6 WebSocket sunucusu. KeepAlive 10 dakika.

```csharp
// src/Presentation/Ocpp.Api/Startup.cs
public class Startup : BaseStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // DB: OcppDbContext -- OCPP mesajlari, baglanti durumlari
        services.AddPixdinnDbService<OcppDbContext>(dbcontextOptions);
        services.AddPixdinnApiService(...);
        services.AddFilters();
        services.RegisterMasstransit();
        services.AddFrameworkServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        var webSocketOptions = new WebSocketOptions()
        {
            ReceiveBufferSize = 8 * 1024,
            KeepAliveInterval = TimeSpan.FromMinutes(10) // <- OCPP icin 10 dk keepalive
        };
        app.UseWebSockets(webSocketOptions);
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
    }
}
```

### GateWay.Api Startup.cs

Ocelot tabanli API Gateway. `BaseStartup` kullanmaz.

```csharp
// src/Presentation/GateWay.Api/Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOcelot().AddCacheManager(
            settings => settings.WithDictionaryHandle());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        app.UseWebSockets();
        app.UseOcelot().Wait();  // <- Ocelot middleware
    }
}
```

Ocelot konfigurasyonu `ocelot.{Environment}.json` dosyasindan okunur:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/v1/{url}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "mobil.api", "Port": "80" }],
      "UpstreamPathTemplate": "/mobil/{url}",
      "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
      "LoadBalancerOptions": { "Type": "LeastConnection" }
    }
  ],
  "GlobalConfiguration": { "BaseUrl": "http://localhost:8000" }
}
```

### WorkerService Program.cs

`IHostedService` tabanli arka plan islemi.

```csharp
// src/Presentation/WorkerService/Program.cs
public class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<IUtilService, UtilService>();
                services.AddSingleton<IGenericHttpClientService, GenericHttpClientService>();
                services.AddHostedService<TaskControlWorker>(); // <- HostedService
            });
}
```

---

## Controller'lar (Tam Kod)

### OCPP16Controller (Ocpp.Api)

WebSocket baglantisini kabul edip `Ocpp16ConnectionService`'e devreder.

```csharp
// src/Presentation/Ocpp.Api/Controllers/OCPP16Controller.cs
public class OCPP16Controller : Controller
{
    private readonly IOcpp16ConnectionService _ocpp16ConnectionService;

    public OCPP16Controller(IOcpp16ConnectionService ocpp16ConnectionService)
    {
        _ocpp16ConnectionService = ocpp16ConnectionService;
    }

    [Route("[controller]/[action]/{Identifier}")]
    public async Task Connection(string Identifier)
    {
        await _ocpp16ConnectionService.Connection(Identifier, OcppDeviceTypeEnum.CIRCONTROL);
    }
}
```

Not: `[ApiController]` yok, donus tipi `Task` (IActionResult degil) -- WebSocket endpoint'i.

### RemoteTransactionController (Ocpp.Api)

Sarj cihazina remote start/stop komutu gonderir.

```csharp
// src/Presentation/Ocpp.Api/Controllers/RemoteTransactionController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class RemoteTransactionController : ControllerBase
{
    private readonly IOcpp16RemoteTransactionService _ocpp16RemoteTransactionService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<Ocpp16StationRemoteStartTransactionResponseDto>), 200)]
    [SwaggerOperation(OperationId = "RemoteStartTransaction")]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> RemoteStartTransaction(
        Ocpp16StationRemoteStartTransactionRequestDto request)
    {
        var result = await _ocpp16RemoteTransactionService.RemoteStartTransactionAsync(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<Ocpp16StationRemoteStopTransactionResponseDto>), 200)]
    [SwaggerOperation(OperationId = "RemoteStopTransaction")]
    [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
    public async Task<IActionResult> RemoteStopTransaction(
        Ocpp16StationRemoteStopTransactionRequestDto request)
    {
        var result = await _ocpp16RemoteTransactionService.RemoteStopTransactionAsync(request);
        return this.FromHttpClientResult(result);
    }
}
```

### AuthenticationController (Web.Api)

Panel login akisi. Form hazirlama, Login, Logout.

```csharp
// src/Presentation/Web.Api/Controllers/AuthenticationController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<LoginFormResponseDto>), 200)]
    [SwaggerOperation(OperationId = "LoginForm")]
    public async Task<IActionResult> LoginForm(LoginFormRequestDto loginFormRequest)
    {
        var result = await _authenticationService.LoginForm(loginFormRequest);
        return this.FromResult(result);  // <- FromResult: basari=200 OK data, hata=400
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), 200)]
    [SwaggerOperation(OperationId = "Login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequest)
    {
        var result = await _authenticationService.Login(loginRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<LogoutResponseDto>), 200)]
    [SwaggerOperation(OperationId = "LogOut")]
    public async Task<IActionResult> LogOut()
    {
        var result = await _authenticationService.LogOut();
        return this.FromResult(result);
    }
}
```

### AuthController (Tocken.Api)

Tocken API'nin Auth endpoint'i. `InnerRequestAttribute` ile sadece WEB API cagrisi kabul eder.

```csharp
// src/Presentation/Tocken.Api/Controllers/AuthController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(TokenRequestResponseInfoFilterAttribute))]
public class AuthController : ControllerBase
{
    private readonly IAuthGroupService _authGroupService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<List<AuthGroupDto>>), 200)]
    [SwaggerOperation(OperationId = "TockenGetAuthGroupWithAuthList")]
    [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]  // <- Sadece WEB erisebilir
    public async Task<IActionResult> TockenGetAuthGroupWithAuthList()
    {
        var result = await _authGroupService.TockenGetAuthGroupWithAuthList();
        return this.FromHttpClientResult(result); // <- her zaman 200
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<List<AuthGroupDto>>), 200)]
    [SwaggerOperation(OperationId = "TockenGetAuthList")]
    public async Task<IActionResult> TockenGetAuthList()
    {
        var result = await _authGroupService.TockenGetAuthGroupWithAuthList();
        return this.FromResult(result);
    }
}
```

### ChargeController (SarjAllMobil.Api)

Mobil sarj islemleri. `[Authorize]` ve filter ile cift korumal.

```csharp
// src/Presentation/SarjAllMobil.Api/Controllers/ChargeController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[Authorize]
[ServiceFilter(typeof(SarjAllMobilApiAuthenticationFilterAttribute))]
[ServiceFilter(typeof(SarjAllMobilApiRequesResponsetLogFilterAttribute))]
public class ChargeController : ControllerBase
{
    private readonly IChargeService _chargeService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<PrepareChargeProcessResponseDto>), 200)]
    [SwaggerOperation(OperationId = "PrepareChargeProcess")]
    public async Task<IActionResult> PrepareChargeProcess(PrepareChargeProcessRequestDto request)
    {
        var result = await _chargeService.PrepareChargeProcess(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetSocketListResponseDto>), 200)]
    [SwaggerOperation(OperationId = "GetSocketListForChargeProcess")]
    public async Task<IActionResult> GetSocketListForChargeProcess(GetSocketListRequestDto request)
    {
        var result = await _chargeService.GetSocketListForChargeProcess(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<SelectSocketForPrepareChargeProcessResponseDto>), 200)]
    [SwaggerOperation(OperationId = "SelectSocketForPrepareChargeProcess")]
    public async Task<IActionResult> SelectSocketForPrepareChargeProcess(
        SelectSocketForPrepareChargeProcessRequestDto request)
    {
        var result = await _chargeService.SelectSocketForPrepareChargeProcess(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<StartChargeResponseDto>), 200)]
    [SwaggerOperation(OperationId = "StartCharge")]
    public async Task<IActionResult> StartCharge(StartChargeRequestDto request)
    {
        var result = await _chargeService.StartCharge(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetChargeStatusResponseDto>), 200)]
    [SwaggerOperation(OperationId = "GetChargeStatus")]
    public async Task<IActionResult> GetChargeStatus(GetChargeStatusRequestDto request)
    {
        var result = await _chargeService.GetChargeStatus(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<StopChargeResponseDto>), 200)]
    [SwaggerOperation(OperationId = "StopCharge")]
    public async Task<IActionResult> StopCharge(StopChargeRequestDto request)
    {
        var result = await _chargeService.StopCharge(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetPaymentResponseDto>), 200)]
    [SwaggerOperation(OperationId = "GetPayment")]
    public async Task<IActionResult> GetPayment(GetPaymentRequestDto request)
    {
        var result = await _chargeService.GetPayment(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetChargeProcessSarjAllResponseDto>), 200)]
    [SwaggerOperation(OperationId = "GetChargeProcess")]
    public async Task<IActionResult> GetChargeProcess(GetChargeProcessSarjAllRequestDto request)
    {
        var result = await _chargeService.GetChargeProcess(request);
        return this.FromHttpClientResult(result);
    }
}
```

### ChargeDeviceOcppManagementController (Station.Api)

Istasyon cihaz bilgilerini OCPP API'ye saglar. Sadece OCPP API erisir.

```csharp
// src/Presentation/Station.Api/Controllers/ChargeDeviceOcppManagementController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
public class ChargeDeviceOcppManagementController : ControllerBase
{
    private readonly IChargeDeviceOcppManagmentService _chargeDeviceOcppManagmentService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetChargeDeviceForOcppResponseDto>), 200)]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })] // <- Sadece OCPP erisir
    public async Task<IActionResult> GetChargeDeviceForOcpp(
        GetChargeDeviceForOcppRequestDto request)
    {
        var result = await _chargeDeviceOcppManagmentService.GetChargeDeviceForOcpp(request);
        return this.FromHttpClientResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<UpdateChargeDeviceForOcppResponseDto>), 200)]
    [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
    public async Task<IActionResult> UpdateChargeDeviceForOcpp(
        UpdateChargeDeviceForOcppRequestDto request)
    {
        var result = await _chargeDeviceOcppManagmentService.UpdateChargeDeviceForOcpp(request);
        return this.FromHttpClientResult(result);
    }
}
```

### OcppCommandMessageController (Ocpp.Api)

OCPP mesaj loglarini sorgular.

```csharp
// src/Presentation/Ocpp.Api/Controllers/OcppCommandMessageController.cs
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
public class OcppCommandMessageController : ControllerBase
{
    private readonly IOcppCommandMessageService _ocppCommandMessageService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<DataTableResponseWrapper<OcppCommandMessageDto>>), 200)]
    [SwaggerOperation(OperationId = "GetOcppCommandMessage")]
    public async Task<IActionResult> GetOcppCommandMessage(
        DataTableFilterModel<OcppCommandMessageFilterDto> dataTableFilterModel)
    {
        var result = await _ocppCommandMessageService.GetOcppCommandMessage(dataTableFilterModel);
        return this.FromResult(result);
    }
}
```

---

## Application Katmani -- Onemli Servisler

### AuthenticationService (Api.Application)

Web panel login akisinin tamami. TockenApi'ye HTTP client ile cagri atar.

```csharp
// src/Core/Applications/Api.Application/Services/PanelServices/Authentication/AuthenticationService.cs
public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly IPanelAdminRepository _panelAdminRepository;
    private readonly IPanelRootAdminRepository _panelRootAdminRepository;
    private readonly IPanelUserClientService _panelUserClientService; // <- HTTP client
    private readonly IAdminContextProvider _adminContextProvider;    // <- JWT claim'leri

    public AuthenticationService(IMapper mapper, IConfiguration configuration,
        IUtilService utilService, IPanelAdminRepository panelAdminRepository,
        IPanelRootAdminRepository panelRootAdminRepository,
        ICustomHttpUtilService customHttpUtilService,
        IAdminContextProvider adminContextProvider,
        IPanelUserClientService panelUserClientService) : base(mapper)
    {
        _panelAdminRepository = panelAdminRepository;
        _panelRootAdminRepository = panelRootAdminRepository;
        _panelUserClientService = panelUserClientService;
        _adminContextProvider = adminContextProvider;
    }

    public async Task<Result<LoginFormResponseDto>> LoginForm(LoginFormRequestDto request)
    {
        // 1. TockenApi'ye form hazirla istegi at
        var prepareRequest = new PanelLoginFormPrepareRequestDto
        {
            Identifier = _customHttpUtilService.GetHttpContext().UserAgent,
            IpAddress = _customHttpUtilService.GetHttpContext().IpAddress
        };
        var prepareResponse = await _panelUserClientService.PanelLoginFormPrepare(prepareRequest);

        // 2. Basariliysa form key don
        if (prepareResponse.ResultType == ResultType.Ok)
            return new SuccessResult<LoginFormResponseDto>(
                new LoginFormResponseDto { LoginFormKey = prepareResponse.Data.LoginFormKey });
        else
            return new ErrorResult<LoginFormResponseDto>(new LoginFormResponseDto(),
                PanelLoginErrorEnum.PANEL_LOGIN_FORM_CAN_NOT_CREATED);
    }

    public async Task<Result<LoginResponseDto>> Login(LoginRequestDto request)
    {
        PanelAdmin adminUser = null;
        PanelRootAdmin rootAdminUser = null;

        // 1. Admin user veritabanindan cek
        var adminFilter = _mapper.Map<PanelAdminFilterDto>(request);
        adminUser = await _panelAdminRepository.GetPanelAdmins(adminFilter).FirstOrDefaultAsync();

        if (adminUser == null)
        {
            var rootFilter = _mapper.Map<PanelRootAdminFilterDto>(request);
            rootAdminUser = await _panelRootAdminRepository.GetPanelRootAdmin(rootFilter)
                                                           .FirstOrDefaultAsync();
            if (rootAdminUser == null)
                return new ErrorResult<LoginResponseDto>(new LoginResponseDto(),
                    PanelLoginErrorEnum.USER_CAN_NOT_FOUND);
        }

        // 2. TockenApi'ye login check istegi at (MD5 sifre ile)
        var loginCheckRequest = new PanelLoginCheckRequestDto
        {
            LoginFormKey = request.LoginFormKey,
            Md5Password = _utilService.decryptMd5(request.Password),
            AdminId = adminUser?.Id ?? rootAdminUser.Id,
            // ... diger alanlar
        };
        var loginCheckResponse = await _panelUserClientService.PanelLoginCheck(loginCheckRequest);

        if (loginCheckResponse.ResultType == ResultType.Ok)
        {
            var loginResponse = new LoginResponseDto();
            loginResponse.AccessToken = loginCheckResponse.Data.AccessToken;
            return new SuccessResult<LoginResponseDto>(loginResponse);
        }

        // Hata koduna gore farkli ErrorResult don
        if (loginCheckResponse.ErrorCode == (int)PanelTockenErrorEnum.ADMIN_USER_CAN_NOT_FOUND)
            return new ErrorResult<LoginResponseDto>(new LoginResponseDto(), PanelLoginErrorEnum.USER_CAN_NOT_FOUND);
        else if (loginCheckResponse.ErrorCode == (int)PanelTockenErrorEnum.LOGIN_TIME_ERROR)
            return new ErrorResult<LoginResponseDto>(new LoginResponseDto(), PanelLoginErrorEnum.PANEL_LOGIN_TIME_ERROR);
        else
            return new ErrorResult<LoginResponseDto>(new LoginResponseDto(), PanelLoginErrorEnum.USER_CAN_NOT_FOUND);
    }

    public async Task<Result<LogoutResponseDto>> LogOut()
    {
        // Context provider'dan mevcut kullanici bilgilerini al
        var logOutRequest = new PanelLogOutRequestDto
        {
            PanelAdminUserType = _adminContextProvider.PanelAdminUserType,
            AdminId = _adminContextProvider.TockenAdminId,
            LoginSessionId = _adminContextProvider.LoginSessionId,
            LoginSessionJwtKey = _adminContextProvider.LoginSessionJwtKey,
            LoginSessionKey = _adminContextProvider.LoginSessionKey,
            DeviceGuiId = _adminContextProvider.DeviceGuiId,
            DeviceIdentifier = _adminContextProvider.DeviceIdentifier,
            RememberKey = _adminContextProvider.RememberKey
        };
        var response = await _panelUserClientService.PanelLogOut(logOutRequest);
        if (response.ResultType == ResultType.Ok)
            return new SuccessResult<LogoutResponseDto>(new LogoutResponseDto { State = response.Data.State });
        else
            return new ErrorResult<LogoutResponseDto>(new LogoutResponseDto(), PanelLogOutErrorEnum.LOG_OUT_FAILED);
    }
}
```

### PanelUserLoginService (Tocken.Application)

Session yonetiminin gercek implementasyonu. Cok boyutlu login flow.

```csharp
// src/Core/Applications/Tocken.Application/Services/PanelUserLoginService.cs
public class PanelUserLoginService : BaseService, IPanelUserLoginService
{
    private readonly IPanelDeviceRepository _panelDeviceRepository;
    private readonly IPanelLoginFormRepository _panelLoginFormRepository;
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IRootAdminUserRepository _rootAdminUserRepository;
    private readonly IPanelLoginSessionRepository _panelLoginSessionRepository;
    private readonly IPanelTockenHelperService _panelTockenHelperService;
    private readonly IUtilService _utilService;

    // Form hazirlama: device kaydi + login form key uretimi
    public async Task<Result<PanelLoginFormPrepareResponseDto>> PanelLoginFormPrepare(
        PanelLoginFormPrepareRequestDto request)
    {
        var panelDeviceFilter = _mapper.Map<PanelDeviceFilterDto>(request);
        var panelDevice = await _panelDeviceRepository
            .GetPanelDeviceAsNoTracking(panelDeviceFilter).FirstOrDefaultAsync();

        if (panelDevice != null)
        {
            var loginForm = await _panelLoginFormRepository
                .GetPanelLoginForm(new PanelLoginFormFilterDto { PanelDeviceId = panelDevice.Id })
                .FirstOrDefaultAsync();

            if (loginForm == null)
            {
                loginForm = new PanelLoginForm
                {
                    FailedAttentionCount = 0,
                    LoginFormKey = _utilService.GetRandomString(128),
                    PanelDeviceId = panelDevice.Id,
                    CreatedDate = DateTime.Now
                };
                await _panelLoginFormRepository.InsertAsync(loginForm);
                await _panelLoginFormRepository.SaveChangesAsync();
            }
            return new SuccessResult<PanelLoginFormPrepareResponseDto>(
                new PanelLoginFormPrepareResponseDto { LoginFormKey = loginForm.LoginFormKey });
        }
        else
        {
            // Yeni device + form olustur
            panelDevice = _mapper.Map<PanelDevice>(request, opt =>
            {
                opt.AfterMap((src, dest) => (dest as PanelDevice).Guid = Guid.NewGuid() + "");
            });
            var newLoginForm = new PanelLoginForm
            {
                FailedAttentionCount = 0,
                LoginFormKey = _utilService.GetRandomString(128),
                PanelDevice = panelDevice,
                CreatedDate = DateTime.Now
            };
            await _panelLoginFormRepository.InsertAsync(newLoginForm);
            await _panelLoginFormRepository.SaveChangesAsync();
            return new SuccessResult<PanelLoginFormPrepareResponseDto>(
                new PanelLoginFormPrepareResponseDto { LoginFormKey = newLoginForm.LoginFormKey });
        }
    }

    // Login check: zaman kontrolu (min 3 sn) + session olusturma + JWT token uretimi
    public async Task<Result<PanelLoginCheckResponseDto>> PanelLoginCheck(
        PanelLoginCheckRequestDto request)
    {
        var loginForm = await _panelLoginFormRepository
            .GetPanelLoginForm(new PanelLoginFormFilterDto { LoginFormKey = request.LoginFormKey })
            .FirstOrDefaultAsync();

        if (loginForm == null)
            return new ErrorResult<PanelLoginCheckResponseDto>(
                new PanelLoginCheckResponseDto { AccessToken = new AccessTokenDto() },
                PanelTockenErrorEnum.LOGIN_FORM_CAN_NOT_FOUND);

        // Form olusturulmasindan 3 saniye gecmeli
        var seconds = _utilService.dateCompareSeconds(
            loginForm.UpdatedDate ?? loginForm.CreatedDate.GetValueOrDefault(), DateTime.Now);

        if (seconds < 3)
            return new ErrorResult<PanelLoginCheckResponseDto>(
                new PanelLoginCheckResponseDto { AccessToken = new AccessTokenDto() },
                PanelTockenErrorEnum.LOGIN_TIME_ERROR);

        // Admin veya root admin dogrulama
        AdminUser adminUser = null;
        RootAdminUser rootAdminUser = null;

        if (request.PanelAdminUserType == PanelAdminUserType.ADMIN_USER)
        {
            var adminFilter = _mapper.Map<AdminUserFilterDto>(request);
            adminUser = await _adminUserRepository.GetAdminUser(adminFilter).FirstOrDefaultAsync();
            if (adminUser == null)
            {
                // Basarisiz giris logla
                _panelLoginFormRepository.UpdateWithProperties(loginForm,
                    new Expression<Func<PanelLoginForm, object>>[]
                    { s => s.FailedAttentionCount, s => s.FailedAttentionDate });
                loginForm.FailedAttentionCount += 1;
                loginForm.FailedAttentionDate = DateTime.Now;
                await _panelLoginFormRepository.SaveChangesAsync();
                return new ErrorResult<PanelLoginCheckResponseDto>(
                    new PanelLoginCheckResponseDto { AccessToken = new AccessTokenDto() },
                    PanelTockenErrorEnum.ADMIN_USER_CAN_NOT_FOUND);
            }
        }
        else if (request.PanelAdminUserType == PanelAdminUserType.ROOT_ADMIN_USER)
        {
            var rootFilter = _mapper.Map<RootAdminUserFilterDto>(request);
            rootAdminUser = await _rootAdminUserRepository.GetRootAdminUser(rootFilter).FirstOrDefaultAsync();
            // Benzer hata yonetimi...
        }

        // 3 adet random key uret (jwt, session, remember)
        var keys = _utilService.getRandomStringMulti(128, 3);
        var loginSessionJwtKey = keys[0];
        var loginSessionKey = keys[1];
        var rememberKey = keys[2];

        // Session olustur veya guncelle
        var panelLoginSession = await _panelLoginSessionRepository
            .GetPanelLoginSession(panelLoginSessionFilter).FirstOrDefaultAsync();

        if (panelLoginSession != null)
        {
            _panelLoginSessionRepository.UpdateWithProperties(panelLoginSession,
                new Expression<Func<PanelLoginSession, object>>[]
                { s => s.LastVerificationDate, s => s.SessionJwtKey, s => s.SessionKey, s => s.RememberKey });
            panelLoginSession.LastVerificationDate = DateTime.Now;
            panelLoginSession.SessionJwtKey = loginSessionJwtKey;
            panelLoginSession.SessionKey = loginSessionKey;
            panelLoginSession.RememberKey = rememberKey;
        }
        else
        {
            panelLoginSession = new PanelLoginSession
            {
                CreatedDate = DateTime.Now,
                SessionJwtKey = loginSessionJwtKey,
                SessionKey = loginSessionKey,
                RememberKey = rememberKey,
                AdminUserId = adminUser?.Id,
                RootAdminUserId = rootAdminUser?.Id
            };
            await _panelLoginSessionRepository.InsertAsync(panelLoginSession);
        }

        await _panelLoginFormRepository.SaveChangesAsync();

        // JWT claim doldur ve token uret
        var claim = new WebClaimDto
        {
            DeviceGuiId = loginForm.PanelDevice.Guid,
            LoginSessionJwtKey = loginSessionJwtKey,
            LoginSessionKey = loginSessionKey,
            RememberKey = rememberKey,
            LoginSessionId = panelLoginSession.Id,
            UserName = request.UserName,
            AdminId = request.AdminId,
            PanelAdminUserType = request.PanelAdminUserType
        };
        var accessToken = CreateAccessToken(claim);
        return new SuccessResult<PanelLoginCheckResponseDto>(
            new PanelLoginCheckResponseDto { AccessToken = accessToken.Data });
    }

    // Logout: session key'lerini yenile (token gecersiz kil)
    public async Task<Result<PanelLogOutResponseDto>> PanelLogOut(PanelLogOutRequestDto request)
    {
        var loginSession = await _panelLoginSessionRepository
            .GetPanelLoginSession(panelLoginSessionFilter).FirstOrDefaultAsync();

        if (loginSession != null)
        {
            var newKeys = _utilService.getRandomStringMulti(128, 3);
            _panelLoginSessionRepository.UpdateWithProperties(loginSession,
                new Expression<Func<PanelLoginSession, object>>[]
                { s => s.RememberKey, s => s.SessionJwtKey, s => s.SessionKey });
            loginSession.RememberKey = newKeys[2];
            loginSession.SessionKey = newKeys[1];
            loginSession.SessionJwtKey = newKeys[0];
            await _panelLoginSessionRepository.SaveChangesAsync();
        }
        return new SuccessResult<PanelLogOutResponseDto>(
            new PanelLogOutResponseDto { State = loginSession != null });
    }

    public Result<AccessTokenDto> CreateAccessToken(WebClaimDto claim)
    {
        var accessToken = _panelTockenHelperService.CreateToken(claim);
        return new SuccessResult<AccessTokenDto>(accessToken);
    }
}
```

### Ocpp16ConnectionService (Ocpp.Application)

OCPP 1.6 WebSocket sunucusunun kalbi. Statik dictionary ile aktif baglantilar yonetilir.

```csharp
// src/Core/Applications/Ocpp.Application/Services/Ocpp16/Ocpp16Connection/Ocpp16ConnectionService.cs
public class Ocpp16ConnectionService : BaseService, IOcpp16ConnectionService
{
    // Tum bagli cihazlarin aktif WebSocket oturumları static dictionary'de tutulur
    public static Dictionary<long, DeviceSessionStatusDto> _deviceSessionStatusDict
        = new Dictionary<long, DeviceSessionStatusDto>();

    private const string Protocol_OCPP16 = "ocpp1.6";
    private const string Protocol_OCPP16J = "ocpp1.6j";
    private static string MessageRegExp =
        "^\\[\\s*(\\d)\\s*,\\s*\"([^\"]*)\"\\s*,(?:\\s*\"(\\w*)\"\\s*,)?\\s*(.*)\\s*\\]$";

    // Istek kuyrugu: cihaza gonderilip cevap bekleyen mesajlar
    private Dictionary<string, Ocpp16MessageDto> _requestQueue
        = new Dictionary<string, Ocpp16MessageDto>();

    private readonly IServiceProvider _services; // <- Scoped servis cozumu icin

    public async Task Connection(string identifier, OcppDeviceTypeEnum occpDeviceType)
    {
        if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.IsWebSocketRequest)
        {
            // 1. Komut mesaji kaydet
            await SaveCommandMessage(identifier, occpDeviceType, OcppMessageTypeEnum.RECEIVED, date);

            // 2. Device connection getir/olustur
            deviceConnection = await GetDeviceconnection(identifier, deviceConnection, date);

            if (deviceConnection != null)
            {
                // 3. SubProtocol belirle (ocpp1.6 veya ocpp1.6j)
                string subProtocol = GetSocketSubProtocol();

                // 4. Session status olustur
                deviceSessionStatus = SetDeviceSessionStatus(occpDeviceType, deviceConnection, subProtocol);

                // 5. Static dictionary'yi guncelle, eski baglantiyi kapat
                bool statusSuccess = await UpdateChargePointDictionary(deviceSessionStatus, deviceConnection);

                if (statusSuccess)
                {
                    using (WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol))
                    {
                        deviceSessionStatus.WebSocket = webSocket;
                        await Receive(deviceSessionStatus, occpDeviceType); // <- Mesaj dongusu
                    }
                }
            }
        }
    }

    private async Task Receive(DeviceSessionStatusDto deviceSessionStatus, OcppDeviceTypeEnum occpDeviceType)
    {
        byte[] buffer = new byte[1024 * 4];
        MemoryStream memStream = new MemoryStream(buffer.Length);
        var firstConnection = true;

        while (deviceSessionStatus.WebSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await deviceSessionStatus.WebSocket
                .ReceiveAsync(buffer, CancellationToken.None);

            if (result != null && result.MessageType != WebSocketMessageType.Close)
            {
                memStream.Write(buffer, 0, result.Count);
                if (result.EndOfMessage)
                {
                    byte[] bMessage = processMemoryStream(buffer, ref memStream);
                    string ocppMessage;
                    List<object> data;
                    getDataFromMessage(bMessage, out ocppMessage, out data);

                    if (data.Count >= 4)
                    {
                        string messageTypeId = data[0] + ""; // "2"=Request, "3"=Response, "4"=Error
                        string uniqueId = data[1] + "";
                        string action = data[2] + "";
                        string jsonPayload = data[3] + "";
                        var msgIn = new Ocpp16MessageDto(messageTypeId, uniqueId, action, jsonPayload);

                        await ProcessOcppCommand(deviceSessionStatus, occpDeviceType,
                            datetimeNow, firstConnection, ocppMessage, uniqueId, msgIn, datetimeNow);

                        if (msgIn.MessageType == "2") // Cihazdan sunucuya istek
                        {
                            Ocpp16MessageDto msgOut;
                            using (var scope = _services.CreateScope())
                            {
                                var processService = scope.ServiceProvider
                                    .GetRequiredService<IOcpp16ProcessRequestAndResponseService>();
                                msgOut = await processService.ProcessRequest(msgIn, deviceSessionStatus, datetimeNow);
                            }
                            await SendOcppMessage(msgOut, deviceSessionStatus, null, datetimeNow);
                        }
                        else if (msgIn.MessageType == "3" || msgIn.MessageType == "4") // Cevap
                        {
                            if (_requestQueue.ContainsKey(msgIn.UniqueId))
                            {
                                ProcessAnswer(msgIn, _requestQueue[msgIn.UniqueId]);
                                _requestQueue.Remove(msgIn.UniqueId);
                            }
                        }
                    }
                }
            }
            else
            {
                await ClosingSocketConnection(deviceSessionStatus);
                await UpdateDeviceConnection(deviceSessionStatus, null, datetimeNow, datetimeNow, null, null, false, true);
            }
            firstConnection = false;
        }
    }

    // Sunucudan cihaza komut gonder
    public async Task SendOcppMessage(Ocpp16MessageDto msg, DeviceSessionStatusDto session,
        OcppRequestTypeEnum? ocppRequestType, DateTime date)
    {
        string ocppTextMessage;
        if (string.IsNullOrEmpty(msg.ErrorCode))
        {
            if (msg.MessageType == "2") // Request
            {
                msg.TaskCompletionSource = new TaskCompletionSource<string>();
                _requestQueue.Add(msg.UniqueId, msg); // <- Cevap icin kuyruğa al
                ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]",
                    msg.MessageType, msg.UniqueId, msg.Action, msg.JsonPayload);
            }
            else // Response
            {
                ocppTextMessage = string.Format("[{0},\"{1}\",{2}]",
                    msg.MessageType, msg.UniqueId, msg.JsonPayload);
            }
        }
        else
        {
            ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]",
                msg.MessageType, msg.UniqueId, msg.ErrorCode, msg.ErrorDescription, "{}");
        }

        byte[] binaryMessage = UTF8Encoding.UTF8.GetBytes(ocppTextMessage);
        if (session.WebSocket.State == WebSocketState.Open)
            await session.WebSocket.SendAsync(
                new ArraySegment<byte>(binaryMessage), WebSocketMessageType.Text, true,
                CancellationToken.None);
    }

    // Cihaza cevap geldiginde hangi komuta ait oldugunu bul
    public void ProcessAnswer(Ocpp16MessageDto msgIn, Ocpp16MessageDto msgOut)
    {
        using (var scope = _services.CreateScope())
        {
            var handleService = scope.ServiceProvider.GetRequiredService<IOcpp16HandleService>();
            switch (msgOut.Action)
            {
                case "Reset":               handleService.HandleReset(msgIn, msgOut); break;
                case "UnlockConnector":     handleService.HandleUnlockConnector(msgIn, msgOut); break;
                case "RemoteStartTransaction": handleService.HandleRemoteStartTransaction(msgIn, msgOut); break;
                default: break;
            }
        }
    }

    private DeviceSessionStatusDto SetDeviceSessionStatus(OcppDeviceTypeEnum occpDeviceType,
        DeviceConnectionDto deviceConnection, string subProtocol)
    {
        var deviceSessionStatus = new DeviceSessionStatusDto(deviceConnection);
        deviceSessionStatus.DeviceType = occpDeviceType;
        deviceSessionStatus.Protocol = subProtocol;
        return deviceSessionStatus;
    }

    private async Task<bool> UpdateChargePointDictionary(DeviceSessionStatusDto deviceSessionStatus,
        DeviceConnectionDto deviceConnection)
    {
        bool statusSuccess = false;
        DeviceSessionStatusDto dummydeviceSessionStatus = null;
        try
        {
            lock (_deviceSessionStatusDict)
            {
                if (_deviceSessionStatusDict.ContainsKey(deviceConnection.Id))
                    _deviceSessionStatusDict.Remove(deviceConnection.Id, out dummydeviceSessionStatus);
                _deviceSessionStatusDict.Add(deviceConnection.Id, deviceSessionStatus);
                statusSuccess = true;
            }
            await CloseOldConnection(dummydeviceSessionStatus);
        }
        catch (Exception exp)
        {
            _logger.LogError("error: " + exp.Message);
        }
        return statusSuccess;
    }
}
```

---

## Domain -- Entity'ler

### BaseEntity (FrameworkCore)

```csharp
// Framework/Core/FrameworkCore/Bases/BaseEntities/BaseEntity.cs
public class BaseEntity : IEntity
{
    public long Id { get; set; }
    public bool Deleted { get; set; } = false;  // Soft delete destegi
}
```

### AdminUser (TockenEntities)

```csharp
// src/Shared/Shared.Domain/Entities/TockenEntities/PanelAdminModule/AdminUser.cs
[Table("AdminUser", Schema = "EvTech")]
public class AdminUser : BaseEntity
{
    public string GuiId { get; set; }
    public string Md5Password { get; set; }
}
```

### GeneralUser (TockenEntities)

Mobil uygulama kullanicisi.

```csharp
// src/Shared/Shared.Domain/Entities/TockenEntities/GeneralUserModule/GeneralUser.cs
[Table("GeneralUser", Schema = "EvTech")]
public class GeneralUser : BaseEntity
{
    public string UserSignatureKey { get; set; }   // dogrulama imzasi
    public DateTime CreatedDate { get; set; }
    public string Md5Password { get; set; }
    public string Phone { get; set; }
    public string PhoneCountryCode { get; set; }
    public string UserInfoHash { get; set; }
}
```

### Auth (TockenEntities)

Yetki tanimlari. AuthGroup ile iliskili.

```csharp
// src/Shared/Shared.Domain/Entities/TockenEntities/AuthModule/Auth.cs
[Table("Auth", Schema = "EvTech")]
public class Auth : BaseEntity
{
    [ForeignKey("AuthGroup")]
    public long? AuthGroupId { get; set; }
    public AuthGroup? AuthGroup { get; set; }
    public string Description { get; set; }
    public bool DefaultValue { get; set; }
    public string AuthSecurityKey { get; set; }
}
```

### Tum Entity Listesi

**ApiEntities (SarjAllProDbContext):**
- `PanelAdminType`, `PanelAdmin`, `PanelRootAdmin` -- yonetici kullanicilari
- `Parameter`, `ParameterGroup`, `ParameterValue` -- sistem parametreleri
- `Policy` -- kullanim politikalari
- `Country`, `City`, `Town` -- cografi bolge (seed data ile dolu)
- `ContentLanguage` -- coklu dil destegi

**TockenEntities (TockenDbContext):**
- `AdminUser`, `RootAdminUser` -- panel kullanicilari
- `AdminUserType`, `AdminUserTypeAuth` -- kullanici tipi ve yetki eslesmesi
- `Auth`, `AuthGroup` -- yetki gruplari
- `GeneralUser` -- mobil kullanicilari
- `PanelDevice`, `PanelLoginForm`, `PanelLoginSession` -- session yonetimi

**OcppEntities (OcppDbContext):**
- `DeviceConnection`, `ConnectorConnection` -- cihaz baglanti durumlari
- `TempDeviceConnection`, `TempConnectorConnection` -- gecici baglantilar
- `MeterValue`, `MeterValueEvent`, `SampledValue` -- elektrik olcum verileri
- `StatusNotificationEvent` -- cihaz durum bildirimleri
- `StartTransaction`, `StopTransaction`, `IdTagInfo` -- sarj islem kayitlari
- `OcppRequest`, `OcppCommandMessage` -- OCPP mesaj loglari
- `HeartBeat`, `SocketMovements` -- baglanti saglik verileri

**LogEntities:**
- `ApiException`, `MobilException` -- hata kayitlari
- `RequestResponse` -- API cagri loglari
- `EntityProcesses`, `WorkerServiceProcessInfo`

---

## Infrastructure -- DbContext ve Repository'ler

### SarjAllProDbContext (Ana veritabani)

```csharp
// src/Core/Persistences/Api.Persistence/DbContext/SarjAllProDbContext.cs
public class SarjAllProDbContext : UnitOfWork
{
    public SarjAllProDbContext(DbContextOptions<SarjAllProDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API konfigurasyonlari
        modelBuilder.ApplyConfiguration(new ParameterGroupFluent());
        modelBuilder.ApplyConfiguration(new ParameterFluent());
        modelBuilder.ApplyConfiguration(new ParameterValueFluent());
        modelBuilder.ApplyConfiguration(new CountryFluent());
        modelBuilder.ApplyConfiguration(new CityFluent());
        modelBuilder.ApplyConfiguration(new TownFluent());
        modelBuilder.ApplyConfiguration(new PanelRootAdminFluent());
        modelBuilder.ApplyConfiguration(new PanelAdminFluent());
        modelBuilder.ApplyConfiguration(new PolicyFluent());

        // Data Seeding
        modelBuilder.ApplyConfiguration(new ParameterSeed());
        modelBuilder.ApplyConfiguration(new ParameterGroupsSeed());
        modelBuilder.ApplyConfiguration(new CountrySeed());
        modelBuilder.ApplyConfiguration(new CitySeed());
        modelBuilder.ApplyConfiguration(new TownSeed());
    }

    public DbSet<Parameter> Parameter { get; set; }
    public DbSet<ParameterValue> ParameterValue { get; set; }
    public DbSet<ParameterGroup> ParameterGroup { get; set; }
    public DbSet<ContentLanguage> ContentLanguage { get; set; }
    public DbSet<PanelAdmin> PanelAdmin { get; set; }
    public DbSet<PanelAdminType> PanelAdminType { get; set; }
    public DbSet<PanelRootAdmin> PanelRootAdmin { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<Town> Town { get; set; }
    public DbSet<Policy> Policy { get; set; }
}
```

### OcppDbContext (OCPP veritabani)

```csharp
// src/Core/Persistences/Ocpp.Persistence/DbContext/OcppDbContext.cs
public class OcppDbContext : UnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DeviceConnectionFluent());
        modelBuilder.ApplyConfiguration(new ConnectorConnectionFluent());
    }

    public DbSet<DeviceConnection> DeviceConnection { get; set; }
    public DbSet<ConnectorConnection> ConnectorConnection { get; set; }
    public DbSet<MeterValue> MeterValue { get; set; }
    public DbSet<MeterValueEvent> MeterValueEvent { get; set; }
    public DbSet<SampledValue> SampledValue { get; set; }
    public DbSet<StatusNotificationEvent> StatusNotificationEvent { get; set; }
    public DbSet<IdTagInfo> IdTagInfo { get; set; }
    public DbSet<StartTransaction> StartTransaction { get; set; }
    public DbSet<StopTransaction> StopTransaction { get; set; }
    public DbSet<OcppRequest> OcppRequest { get; set; }
    public DbSet<OcppCommandMessage> OcppCommandMessage { get; set; }
    public DbSet<HeartBeat> HeartBeat { get; set; }
    public DbSet<SocketMovements> SocketMovements { get; set; }
    public DbSet<TempDeviceConnection> TempDeviceConnection { get; set; }
    public DbSet<TempConnectorConnection> TempConnectorConnection { get; set; }
}
```

### TockenDbContext (Token veritabani)

```csharp
// src/Core/Persistences/Tocken.Persistence/DbContext/TockenDbContext.cs
public class TockenDbContext : UnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AdminUserTypeAuthFluent());
        modelBuilder.ApplyConfiguration(new AuthGroupFluent());
        modelBuilder.ApplyConfiguration(new AuthFluent());
        modelBuilder.ApplyConfiguration(new PanelDeviceFluent());
        modelBuilder.ApplyConfiguration(new PanelLoginFormFluent());
        modelBuilder.ApplyConfiguration(new PanelLoginSessionFluent());
        modelBuilder.ApplyConfiguration(new GeneralUserFluent());

        // Seed: Auth gruplari ve yetkiler baslangicta dolu
        modelBuilder.ApplyConfiguration(new AuthGroupSeed());
        modelBuilder.ApplyConfiguration(new AuthSeed());
    }

    public DbSet<Auth> Auth { get; set; }
    public DbSet<AuthGroup> AuthGroup { get; set; }
    public DbSet<AdminUserType> AdminUserType { get; set; }
    public DbSet<AdminUserTypeAuth> AdminUserTypeAuth { get; set; }
    public DbSet<GeneralUser> GeneralUser { get; set; }
    public DbSet<RootAdminUser> RootAdminUser { get; set; }
    public DbSet<AdminUser> AdminUser { get; set; }
    public DbSet<PanelDevice> PanelDevice { get; set; }
    public DbSet<PanelLoginForm> PanelLoginForm { get; set; }
    public DbSet<PanelLoginSession> PanelLoginSession { get; set; }
}
```

### BaseRepository (Generic Repository)

Tum repository'lerin turettigi abstract class. Tam CRUD + sayfalama + aggregate.

```csharp
// Framework/Core/FrameworkCore/Bases/BaseRepository/BaseRepository.cs
public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly UnitOfWork _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(IUnitOfWork dbContext)
    {
        _dbContext = dbContext as UnitOfWork;
        _dbSet = _dbContext.Set<TEntity>();
    }

    // CRUD
    public virtual TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll)
        => _dbSet.Add(entity).Entity;

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => (await _dbSet.AddAsync(entity, cancellationToken)).Entity;

    public virtual void Update(TEntity entity, ...) => _dbSet.Update(entity);

    public abstract void Delete(object id);

    // Secici guncelleme: sadece belirtilen property'ler UPDATE edilir
    public abstract void UpdateWithProperties(TEntity entity,
        Expression<Func<TEntity, object>>[] properties);

    // Query
    public IQueryable<TEntity> GetAll() => _dbSet;

    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;
        query = SetTracking(query, tracking);
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);
        return orderBy != null ? orderBy(query) : query;
    }

    // Sayfalama: [data, dataCount] dondurmek icin
    public async Task<object[]> GetPagedAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Expression<Func<TEntity, object>>[] include = null,
        Expression<Func<TEntity, bool>> predicate = null,
        int? page = 0, int? pageSize = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (predicate != null) query = query.Where(predicate);
        if (include != null)
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
        query = orderBy(query);
        var dataCount = query.Count();
        if (page != null && page > 0)
            query = query.Skip(((int)page - 1) * (int)pageSize);
        if (pageSize != null)
            query = query.Take((int)pageSize);
        var data = await query.ToListAsync();
        return new object[] { data, dataCount };
    }

    // Aggregate operasyonlar
    public int Count(Expression<Func<TEntity, bool>> predicate = null)
        => predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
    public bool Exists(Expression<Func<TEntity, bool>> selector = null)
        => selector == null ? _dbSet.Any() : _dbSet.Any(selector);
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null)
        => selector == null ? await _dbSet.AnyAsync() : await _dbSet.AnyAsync(selector);

    // UoW
    public int SaveChanges() => _dbContext.SaveChanges();
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    protected abstract IQueryable<TEntity> SetTracking(
        IQueryable<TEntity> query, TrackingBehaviour tracking);
}
```

---

## Ozel Pattern'ler

### WrapperCore -- Result<T> Pattern

Tum servisler ayni Result wrapper'i kullanir. Controller katmani iki extension method ile response uretir.

```csharp
// Framework/Core/FrameworkCore/FrameworkCore/WrapperCore/Models/Result.cs
public class Result<T>
{
    public virtual ResultType ResultType { get; set; }
    public virtual int ErrorCode { get; set; }
    public virtual string ErrorMessage { get; set; }
    public virtual T Data { get; set; }
}

// SuccessResult: ErrorCode=0, ResultType=Ok
public class SuccessResult<T> : Result<T>
{
    private readonly T _data;
    public SuccessResult(T data) : base()
    {
        _data = data;
        ErrorCode = 0;
        ErrorMessage = "";
    }
    public override ResultType ResultType => ResultType.Ok;
    public override T Data => _data;
}

// ErrorResult: enum'dan ErrorCode ve Description alir
public class ErrorResult<T> : Result<T>
{
    private readonly T? _data;
    public ErrorResult(T? data, Enum error) : base()
    {
        _data = data;
        ErrorCode = (int)((object)error);
        ErrorMessage = error.ToDescriptionString(); // <- Description attribute'dan okunur
    }
    public override ResultType ResultType => ResultType.Error;
    public override T Data => _data;
}
```

```csharp
// Framework/Core/FrameworkCore/FrameworkCore/WrapperCore/ResultExtensions.cs
public static class ResultExtensions
{
    // Ic servis cagrisi: basari=200 sadece Data, hata=400 tum Result
    public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.ResultType == ResultType.Ok)
            return controller.Ok(result.Data);
        else
            return controller.BadRequest(result);
    }

    // HTTP client uzerinden gelen cevap: her zaman 200, Result wrapper don
    public static ActionResult FromHttpClientResult<T>(this ControllerBase controller, Result<T> result)
        => controller.Ok(result);

    // Sadece data dondur
    public static ActionResult FromHttpClientDataResult<T>(this ControllerBase controller, T result)
        => controller.Ok(result);
}
```

**Kullanim ornuntuleri:**
- `FromResult` -- icsek servis cagrilari (Web.Api kendi kontrolcusu)
- `FromHttpClientResult` -- baska API'ye HTTP client ile cagri yapilmis (OCPP, Station arasi)

---

### RabbitMQ -- LogProducer Pattern

Her API'den log ve exception mesajlari RabbitMQ'ya gonderilir. Singleton `LogProducer` baglantiyi kurar.

```csharp
// src/Shared/Shared.Domain/RabbitMq/Producers/LogProducer.cs
public class LogProducer : ILogProducer
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _conn;
    private readonly IModel _channel;

    public LogProducer(ILogger<LogProducer> logger, IConfiguration configuration)
    {
        _factory = new ConnectionFactory()
        {
            HostName = configuration.GetSection("RabbitMqSettings:HostName").Value,
            Port = int.Parse(configuration.GetSection("RabbitMqSettings:PortNumber").Value),
            RequestedHeartbeat = TimeSpan.FromSeconds(60),
            UserName = configuration.GetSection("RabbitMqSettings:UserName").Value,
            Password = configuration.GetSection("RabbitMqSettings:Password").Value
        };

        _conn = _factory.CreateConnection();
        _channel = _conn.CreateModel();

        // Direct exchange olustur
        _channel.ExchangeDeclare(RabbitmqConstants.evtechDirectExchangeName, ExchangeType.Direct);

        // Request/Response log kuyrugu
        _channel.QueueDeclare(queue: RabbitmqConstants.generalReqResLogQueueName,
            durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(
            queue: RabbitmqConstants.generalReqResLogQueueName,
            routingKey: RabbitmqConstants.generalReqResLogQueueKey,
            exchange: RabbitmqConstants.evtechDirectExchangeName);

        // Exception log kuyrugu
        _channel.QueueDeclare(queue: RabbitmqConstants.generalApiExceptionQueueName,
            durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(
            queue: RabbitmqConstants.generalApiExceptionQueueName,
            routingKey: RabbitmqConstants.generalApiExceptionQueueKey,
            exchange: RabbitmqConstants.evtechDirectExchangeName);
    }

    public void sendSaveRequestResponseCommand(SaveRequestResponseLog command)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(command);
        var message = System.Text.Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(
            exchange: RabbitmqConstants.evtechDirectExchangeName,
            routingKey: RabbitmqConstants.generalReqResLogQueueKey,
            basicProperties: null,
            body: message);
    }

    public void sendSaveApiExceptionCommand(SaveApiException command)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(command);
        var message = System.Text.Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(
            exchange: RabbitmqConstants.evtechDirectExchangeName,
            routingKey: RabbitmqConstants.generalApiExceptionQueueKey,
            basicProperties: null, body: message);
    }
}
```

```csharp
// src/Shared/Shared.Domain/RabbitMq/RabbitmqConstants.cs
public class RabbitmqConstants
{
    public const string evtechDirectExchangeName     = "evtech-direct-exchange";
    public const string generalReqResLogQueueName    = "general-req-res-log-queue";
    public const string generalReqResLogQueueKey     = "general-req-res-log-queue-key";
    public const string generalApiExceptionQueueName = "general-api-exception-queue-name";
    public const string generalApiExceptionQueueKey  = "general-api-exception-queue-key";
}
```

---

### HTTP Client -- PanelUserClientService Pattern

Ic servisler arasi iletisim `IGenericHttpClientService` ile gerceklestirilir.

```csharp
// src/Shared/Shared.Domain/HttpClients/HttpClientServices/TockenApiServices/PanelUserClientService.cs
public class PanelUserClientService : IPanelUserClientService
{
    private readonly IGenericHttpClientService _genericHttpClientService;
    private readonly string _baseUrl;

    public PanelUserClientService(IGenericHttpClientService genericHttpClientService,
        IConfiguration configuration)
    {
        _genericHttpClientService = genericHttpClientService;
        _baseUrl = configuration.GetSection("TockenApi:Url").Value; // <- appsettings'ten URL
    }

    public async Task<Result<AddPanelAdminResponseDto>> AddPanelAdmin(AddPanelAdminRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<AddPanelAdminResponseDto>>(
            request, _baseUrl, "PanelUser/AddPanelAdmin", null);
    }

    public async Task<Result<PanelLoginCheckResponseDto>> PanelLoginCheck(PanelLoginCheckRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<PanelLoginCheckResponseDto>>(
            request, _baseUrl, "PanelUser/PanelLoginCheck", null);
    }

    public async Task<Result<PanelLoginSessionCheckResponseDto>> PanelLoginSessionCheck(
        PanelLoginSessionCheckRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<PanelLoginSessionCheckResponseDto>>(
            request, _baseUrl, "PanelUser/PanelLoginSessionCheck", null);
    }

    public async Task<Result<PanelLoginFormPrepareResponseDto>> PanelLoginFormPrepare(
        PanelLoginFormPrepareRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<PanelLoginFormPrepareResponseDto>>(
            request, _baseUrl, "PanelUser/PanelLoginFormPrepare", null);
    }

    public async Task<Result<PanelLogOutResponseDto>> PanelLogOut(PanelLogOutRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<PanelLogOutResponseDto>>(
            request, _baseUrl, "PanelUser/PanelLogOut", null);
    }

    public async Task<Result<UpdatePanelAdminResponseDto>> UpdatePanelAdmin(UpdatePanelAdminRequestDto request)
    {
        return await _genericHttpClientService.PostJson<Result<UpdatePanelAdminResponseDto>>(
            request, _baseUrl, "PanelUser/UpdatePanelAdmin", null);
    }
}
```

---

### InnerRequestAttribute -- API Guvenlik Katmani

Belirli endpoint'lere sadece yetkili API'lerin erisebilmesi icin. Ic servis aramalari icin soft guvenlik katmani.

```csharp
// src/Shared/Shared.Domain/GeneralAttribute/InnerRequestAttribute.cs
public class InnerRequestAttribute : ActionFilterAttribute
{
    public ApiName[] ApiNames { get; set; }

    public InnerRequestAttribute(params ApiName[] apiNames)
    {
        this.ApiNames = apiNames;
    }
}

// src/Shared/Shared.Domain/GeneralEnums/ApiName.cs
public enum ApiName
{
    BANK = 1, FILE = 2, GOOGLE_SERVICE = 3, LOG = 4,
    MAIL_SMS = 5, MOBIL = 6, NOTIFICATION = 7,
    STATION = 8, TOCKEN = 9, WEB = 10,
    WORKER_SERVICE = 11, INTEGRATION = 12
}
```

**Ornek kullanim:**

```csharp
// Station.Api -- sadece OCPP API cagrisi kabul eder
[InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
public async Task<IActionResult> GetChargeDeviceForOcpp(...)

// Tocken.Api -- sadece WEB paneli cagrisi kabul eder
[InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
public async Task<IActionResult> TockenGetAuthGroupWithAuthList()

// Ocpp.Api -- hem MOBIL hem WEB cagrisi kabul eder
[InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
public async Task<IActionResult> RemoteStartTransaction(...)
```

---

### WebApiRequestInfoFilterAttribute -- Log + Exception Filter

Her request/response'u RabbitMQ'ya gonderir.

```csharp
// src/Core/Applications/Api.Application/Filters/WebApi/WebApiRequestInfoFilterAttribute.cs
public class WebApiRequestInfoFilterAttribute : IAsyncActionFilter, IAsyncExceptionFilter
{
    private readonly ILogProducer _logProducer;
    private String actionName = "";
    private String requestBody = "";
    private DateTime requestDate = DateTime.Now;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        this.requestDate = DateTime.Now;
        this.actionName = context.RouteData.Values["action"] as string;

        foreach (KeyValuePair<string, object> entry in context.ActionArguments)
            this.requestBody += Newtonsoft.Json.JsonConvert.SerializeObject(entry.Value);

        var resultContext = await next();

        try
        {
            var jsonData = (ObjectResult)resultContext.Result;
            _logProducer.sendSaveRequestResponseCommand(new SaveRequestResponseLog
            {
                IPAddress = resultContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                ActionName = actionName,
                RequestBody = requestBody,
                ServiceName = "Web.API",
                RequestDate = requestDate,
                ResponseDate = DateTime.Now,
                ResponseBody = JsonConvert.SerializeObject(jsonData.Value),
                ResponseStatusCode = jsonData.StatusCode.ToString()
            });
        }
        catch (Exception ex) { _logger.LogError(ex.Message); }
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        try
        {
            _logProducer.sendSaveApiExceptionCommand(new SaveApiException
            {
                Message = context.Exception.GetBaseException().Message,
                ExceptionType = context.Exception.GetBaseException().GetType().ToString(),
                Stack = context.Exception.StackTrace,
                ExceptionDate = DateTime.Now,
                ActionName = actionName,
                ServiceName = "Web.API"
            });
        }
        catch (Exception e) { _logger.LogError(e.Message); }
    }
}
```

---

### WebApiAuthenticationFilterAttribute -- Session Dogrulama Filter

JWT claim'leri okur, TockenApi'ye session kontrolu yaptirir.

```csharp
// src/Core/Applications/Api.Application/Filters/WebApi/WebApiAuthenticationFilterAttribute.cs
public class WebApiAuthenticationFilterAttribute : ActionFilterAttribute
{
    private readonly IAdminContextProvider _adminContextProvider;
    private readonly IPanelUserClientService _panelUserClientService;

    public async override Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var adminCheck = false;
        var rootAdminCheck = false;

        // 1. Veritabaninda kullanici var mi?
        if (_adminContextProvider.PanelAdminUserType == PanelAdminUserType.ADMIN_USER)
        {
            adminCheck = await _panelAdminRepository.AnyAsync(
                x => !x.Deleted && x.AdminUserGuid == _adminContextProvider.AdminGuiId && x.IsActive);
        }
        else if (_adminContextProvider.PanelAdminUserType == PanelAdminUserType.ROOT_ADMIN_USER)
        {
            rootAdminCheck = await _panelRootAdminRepository.AnyAsync(
                x => !x.Deleted && x.RootAdminUserGuid == _adminContextProvider.AdminGuiId);
        }

        if (!adminCheck && !rootAdminCheck)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                new AuthorizeErrorResult(PanelLoginErrorEnum.PANEL_LOGIN_INVALID.ToDescriptionString())));
            return;
        }

        // 2. TockenApi'ye session check yaptirir
        var sessionCheckRequest = new PanelLoginSessionCheckRequestDto
        {
            PanelAdminUserType = _adminContextProvider.PanelAdminUserType,
            GuiId = _adminContextProvider.AdminGuiId,
            AdminId = _adminContextProvider.TockenAdminId,
            LoginSessionJwtKey = _adminContextProvider.LoginSessionJwtKey,
            DeviceGuiId = _adminContextProvider.DeviceGuiId,
            LoginSessionKey = _adminContextProvider.LoginSessionKey,
            LoginSessionId = _adminContextProvider.LoginSessionId,
            RememberKey = _adminContextProvider.RememberKey,
            DeviceIdentifier = _adminContextProvider.DeviceIdentifier
        };
        var sessionCheckResponse = await _panelUserClientService.PanelLoginSessionCheck(sessionCheckRequest);

        if (sessionCheckResponse.ResultType == ResultType.Ok)
            await next();
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                new AuthorizeErrorResult(PanelLoginErrorEnum.PANEL_LOGIN_INVALID.ToDescriptionString())));
        }
    }
}
```

---

### BaseStartup -- Ortak Startup Altyapisi

Tum API Startup class'larinin turettigi temel class.

```csharp
// Framework/Core/FrameworkCore/Bases/StartupBase/BaseStartup.cs
public class BaseStartup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment WebHostEnvironment { get; }
    public ApiOptions ApiOptions { get; set; }
    public string ProjectPrefix;

    public BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        WebHostEnvironment = env;
        ApiOptions = new ApiOptions()
        {
            ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
            // Sadece proje prefix'i ile baslayan ve Shared ile baslayan DLL'ler yuklenir
            RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory,
                    "*.dll", SearchOption.TopDirectoryOnly)
           .Where(filePath => Path.GetFileName(filePath).StartsWith(ProjectPrefix)
                           || Path.GetFileName(filePath).StartsWith("Shared"))
           .Select(Assembly.LoadFrom)
        };
    }

    // DB baglanti secenegi olusturur (MSSQL destegi)
    public Action<DbContextOptionsBuilder> GetDbContextOption(UsingDbType dbType,
        string connection, string migrationassembly)
    {
        if (dbType == UsingDbType.MSSQL)
            return dbBuilder => dbBuilder.UseSqlServer(connection,
                b => b.MigrationsAssembly(migrationassembly));
        return null;
    }

    public string GetAppSettingValue(string name)
        => Configuration.GetSection(name)?.Value ?? "";

    public void ConfigureBuilderInit(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseCors("_myAllowSpecificOrigins");
        app.UseRouting();
    }
}
```

---

## appsettings.json Yapisi

### Tocken.Api appsettings.json

```json
{
  "ConnectionStrings": {
    "PixdinnConnectionString": "Server=213.159.5.123;Database=EvTechTockenApi;User Id=ev_tech;Password=ev_tech.!299;..."
  },
  "StartupConfigs": {
    "ProjectPrefix": "Tocken",
    "Policy": "_myAllowSpecificOrigins",
    "ApiUrl": "https://localhost:14493",
    "MigrationAssembly": "Tocken.Persistence",
    "AllowAnonymous": false,
    "ApiName": "Tocken"
  },
  "WebToken": {
    "Issuer": "https://localhost:44320/",
    "Audience": "http://localhost:4200/",
    "SecurityKey": "c7243fs3-qa4f3-3253-9036-15a43bbd9e51l",
    "AccessTokenExpiration": 11,
    "RefreshTokenExpiration": 12
  },
  "MobilToken": {
    "SecurityKey": "c7243fs3-qa4f3-3253-9036-15a43bbd9e51r",
    "AccessTokenExpiration": 11
  },
  "SarjAllMobilToken": {
    "SecurityKey": "c7243fs3-qa4f3-3253-9036-15a43bbd9e51a",
    "AccessTokenExpiration": 11
  },
  "RabbitMqSettings": {
    "HostName": "localhost",
    "PortNumber": 5672,
    "UserName": "evtech",
    "Password": "nizip299"
  },
  "HashKeys": {
    "RegisterAuthHash": "9zzDDhQB1xNumVukcl4qwZi87sWA8sib+"
  }
}
```

### Station.Api appsettings.json

Diger API'lerin URL'lerini tanimlar (servisler arasi iletisim icin):

```json
{
  "ConnectionStrings": {
    "PixdinnConnectionString": "Server=213.159.5.123;Database=EvTechApi;..."
  },
  "RabbitMqSettings": { "HostName": "localhost", "PortNumber": 5672, "UserName": "evtech" },
  "NotificationApi": { "Url": "https://localhost:44378/v1/" },
  "WebApi": { "Url": "https://localhost:44312/v1/" },
  "BankApi": { "Url": "https://localhost:44375/v1/" },
  "MobilApi": { "Url": "https://localhost:44368/v1/" },
  "IntegrationApi": { "Url": "https://localhost:44322/v1/" },
  "StartupConfigs": {
    "ProjectPrefix": "Api",
    "Policy": "_myAllowSpecificOrigins",
    "MigrationAssembly": "Api.Persistence",
    "ApiName": "Api"
  }
}
```

---

## Mimari Ozet

```
[Angular Panel]          [Mobil Uygulama]
      |                        |
      v                        v
 [GateWay.Api]          [GateWay.Api]
 Ocelot Gateway         Ocelot Gateway
      |                        |
      +------------------------+
      |
      +---> [Web.Api]         JWT + RabbitMQ Log
      |         |
      |         +---> [Tocken.Api]   Session + JWT uretimi
      |
      +---> [Station.Api]    Sarj Istasyonu CRUD + WebSocket
      |
      +---> [Ocpp.Api]       OCPP 1.6 WebSocket Server
      |         |
      |         +---> [Fiziksel Sarj Cihazlari] (WebSocket)
      |
      +---> [SarjAllMobil.Api]  Mobil Sarj Islemleri
      |
      +---> [Bank.Api]       Odeme Islemleri
      |
      +---> [Log.Api]        Request/Response Loglari
                ^
                | (RabbitMQ Consumer)

[RabbitMQ] <--- LogProducer (her API'den) ---> [Log.Api Consumer]
           exchange: evtech-direct-exchange
           queue-1: general-req-res-log-queue
           queue-2: general-api-exception-queue
```
