# sarj_pro_backend_dotnet -- Cikarilan Kurallar

Kaynak: `E:\Projeler\Backend\SarjAllPro\`
Gercek kod okunarak dogrulandi.

Bu dosya SarjAllPro projesinden cikarilan mimari ve kodlama kurallarini icerir.
Her kural kod kaniti ile birlikte verilmistir.

---

## KURAL 1: Her servis kendi DbContext'ine sahiptir

Her API projesi farkli bir DbContext kullanir. Veritabanlari birbirinden tamamen bagimsizdirlar.

```csharp
// Web.Api -- SarjAllProDbContext (ana panel veritabani)
services.AddPixdinnDbService<SarjAllProDbContext>(dbcontextOptions);

// Tocken.Api -- TockenDbContext (session ve JWT veritabani)
services.AddPixdinnDbService<TockenDbContext>(dbcontextOptions);

// Ocpp.Api -- OcppDbContext (OCPP mesaj veritabani)
services.AddPixdinnDbService<OcppDbContext>(dbcontextOptions);

// Bank.Api -- PaymentDbContext (odeme veritabani)
services.AddPixdinnDbService<PaymentDbContext>(dbcontextOptions);
```

**Kural:** Bir API projesinin baska bir API'nin DbContext'ine dogrudan erisimi yoktur. Servisler arasi veri transferi HTTP client veya RabbitMQ uzerinden yapilir.

---

## KURAL 2: Tum DbContext'ler UnitOfWork'ten turetilir

Kendi SaveChanges yonetimi icin UoW pattern zorunludur.

```csharp
// SarjAllProDbContext -- UnitOfWork base class
public class SarjAllProDbContext : UnitOfWork
{
    public SarjAllProDbContext(DbContextOptions<SarjAllProDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ParameterGroupFluent());
        // ...
    }

    public DbSet<Parameter> Parameter { get; set; }
    // ...
}

// UnitOfWork abstract base (FrameworkCore)
// SaveChanges + SaveChangesAsync repository seviyesinde kullanilir
public int SaveChanges() => _dbContext.SaveChanges();
public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
```

---

## KURAL 3: Her entity BaseEntity'den turetilir ve Deleted=false ile baslangic durumu gelir

Soft delete zorunludur. `Deleted` alanini fiziksel silme yerine kullan.

```csharp
// Framework/Core/FrameworkCore/Bases/BaseEntities/BaseEntity.cs
public class BaseEntity : IEntity
{
    public long Id { get; set; }
    public bool Deleted { get; set; } = false;  // Varsayilan: silinmemis
}

// Kullanim: her entity [Table] ve Schema = "EvTech" ile isaretlenir
[Table("AdminUser", Schema = "EvTech")]
public class AdminUser : BaseEntity
{
    public string GuiId { get; set; }
    public string Md5Password { get; set; }
}

// Sorgularda Deleted kontrolu zorunlu
adminCheck = await _panelAdminRepository.AnyAsync(
    x => !x.Deleted && x.AdminUserGuid == _adminContextProvider.AdminGuiId && x.IsActive);
```

---

## KURAL 4: Result<T> pattern -- tum servis metodlari Result<T> dondurur

Hic bir servis metodu null veya exception ile sonuclanmaz. Her zaman SuccessResult veya ErrorResult doner.

```csharp
// Basarili sonuc
return new SuccessResult<LoginFormResponseDto>(
    new LoginFormResponseDto { LoginFormKey = prepareResponse.Data.LoginFormKey });

// Hatali sonuc -- enum'dan ErrorCode ve Description alir
return new ErrorResult<LoginResponseDto>(new LoginResponseDto(),
    PanelLoginErrorEnum.USER_CAN_NOT_FOUND);

// Controller katmaninda iki farkli extension method:

// FromResult: basari=200 sadece Data don, hata=400 tum Result
return this.FromResult(result);

// FromHttpClientResult: her zaman 200 OK, Result wrapper don (ic API cagrilerinde)
return this.FromHttpClientResult(result);
```

**Kural:** `ErrorResult` olusturulurken mutlaka DTO nesnesi de verilmeli (null gecilemez),
`SuccessResult` icin ise data her zaman dolu olmali.

---

## KURAL 5: Controller'lar is mantigi icermez

Her controller sadece servisi cagir ve sonucu wrapper'dan don. Hic bir is mantigi controller'da olmamali.

```csharp
// DOGRU -- Controller sadece service cagirip wrap ediyor
[HttpPost]
public async Task<IActionResult> Login(LoginRequestDto loginRequest)
{
    var result = await _authenticationService.Login(loginRequest);
    return this.FromResult(result);
}

// YANLIS -- Controller'da if/else is mantigi
[HttpPost]
public async Task<IActionResult> Login(LoginRequestDto loginRequest)
{
    if (loginRequest.UserName == null) return BadRequest("...");  // YANLIS
    var user = await _repo.GetUser(loginRequest.UserName);        // YANLIS
    // ...
}
```

---

## KURAL 6: Autofac ile DI -- her API'de ConfigureContainer metodu zorunlu

Autofac Container builder `ConfigureRepositories` ve `ConfigureServices` extension metodlari ile doldurulur.

```csharp
// Her Startup'ta bu iki metod zorunlu
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.ConfigureRepositories(ApiOptions);  // Repository'leri kaydet
    builder.ConfigureServices(ApiOptions);      // Servisleri kaydet
}

// Program.cs'de AutofacServiceProviderFactory kullanimi zorunlu
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

---

## KURAL 7: AutoMapper -- AfterMap ile post-mapping islemi

Haritalama sirasinda hesaplanmasi gereken alanlar `AfterMap` icinde yapilir.

```csharp
// AfterMap ile ek alanlar ayarlaniyor
panelDevice = _mapper.Map<PanelDevice>(request, opt =>
{
    opt.AfterMap((src, dest) =>
    {
        var destData = dest as PanelDevice;
        destData.Guid = Guid.NewGuid() + "";  // Otomatik Guid olusturma
    });
});

// Diger ornek -- loginResponse'a PanelAdminUserType ekleniyor
loginResponse = _mapper.Map<LoginResponseDto>(rootAdminUser, opt =>
{
    opt.AfterMap((src, dest) =>
    {
        var destData = dest as LoginResponseDto;
        destData.PanelAdminUserType = PanelAdminUserType.ROOT_ADMIN_USER;
    });
});
```

---

## KURAL 8: InnerRequestAttribute -- servisler arasi erisim kontrolu

Belirli endpoint'lere sadece belirli API'lerin erisebilmesi icin `[InnerRequestAttribute]` kullan.

```csharp
// Sadece OCPP API'den cagrimi kabul et
[InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
public async Task<IActionResult> GetChargeDeviceForOcpp(...)

// Sadece WEB API'den cagrimi kabul et
[InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
public async Task<IActionResult> TockenGetAuthGroupWithAuthList()

// Hem MOBIL hem WEB API'den cagrimi kabul et
[InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
public async Task<IActionResult> RemoteStartTransaction(...)
```

**ApiName enum -- tum API'ler tanimli:**

```csharp
public enum ApiName
{
    BANK = 1, FILE = 2, GOOGLE_SERVICE = 3, LOG = 4,
    MAIL_SMS = 5, MOBIL = 6, NOTIFICATION = 7,
    STATION = 8, TOCKEN = 9, WEB = 10,
    WORKER_SERVICE = 11, INTEGRATION = 12
}
```

---

## KURAL 9: ServiceFilter ile filter baglama -- constructor injection destekli

Filter'lar `[ServiceFilter(typeof(...))]` ile baglnir, bu sayede DI desteklenir.

```csharp
// ServiceFilter: DI container'dan filter olusturulur
[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
public class AuthenticationController : ControllerBase { ... }

// Mobil API -- cift filter
[ServiceFilter(typeof(SarjAllMobilApiAuthenticationFilterAttribute))]
[ServiceFilter(typeof(SarjAllMobilApiRequesResponsetLogFilterAttribute))]
public class ChargeController : ControllerBase { ... }
```

**YANLIS kullanim:**
```csharp
// [TypeFilter] veya dogrudan attribute instantiation kullanma
[TypeFilter(typeof(WebApiRequestInfoFilterAttribute))]  // Kacin
```

---

## KURAL 10: Log filter -- her API'nin kendi log filter'i vardir

Her API projesi icin ozel log filter yazilir, tum request/response loglari RabbitMQ'ya gonderilir.

```csharp
// WebApiRequestInfoFilterAttribute -- ayni zamanda hem log hem exception filter
public class WebApiRequestInfoFilterAttribute : IAsyncActionFilter, IAsyncExceptionFilter
{
    private readonly ILogProducer _logProducer;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requestDate = DateTime.Now;
        // request body topla...
        var resultContext = await next();
        // response bilgileri al, RabbitMQ'ya gonder
        _logProducer.sendSaveRequestResponseCommand(new SaveRequestResponseLog { ... });
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        _logProducer.sendSaveApiExceptionCommand(new SaveApiException { ... });
    }
}
```

---

## KURAL 11: RabbitMQ -- LogProducer singleton'dir

`LogProducer` uygulama basladiginda bir kez baglanti kurar ve singleton olarak yasatilir.

```csharp
// LogProducer constructor'da baglanti olusturulur
public LogProducer(ILogger<LogProducer> logger, IConfiguration configuration)
{
    _factory = new ConnectionFactory() { HostName = "...", Port = 5672, ... };
    _conn = _factory.CreateConnection();
    _channel = _conn.CreateModel();
    _channel.ExchangeDeclare(RabbitmqConstants.evtechDirectExchangeName, ExchangeType.Direct);
    _channel.QueueDeclare(queue: RabbitmqConstants.generalReqResLogQueueName, ...);
    _channel.QueueBind(...);
    _channel.QueueDeclare(queue: RabbitmqConstants.generalApiExceptionQueueName, ...);
    _channel.QueueBind(...);
}

// Yayinlama -- her request'te cagrilir (singleton'dan)
public void sendSaveRequestResponseCommand(SaveRequestResponseLog command)
{
    var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
    _channel.BasicPublish(
        exchange: RabbitmqConstants.evtechDirectExchangeName,
        routingKey: RabbitmqConstants.generalReqResLogQueueKey,
        basicProperties: null, body: message);
}
```

---

## KURAL 12: HTTP Client -- URL appsettings'ten okunur, sabit kodlama yasaktir

Her HTTP client servisi base URL'yi appsettings.json'dan okur.

```csharp
// PanelUserClientService -- appsettings'ten URL
public PanelUserClientService(IGenericHttpClientService genericHttpClientService,
    IConfiguration configuration)
{
    _baseUrl = configuration.GetSection("TockenApi:Url").Value;
    // NOT: "TockenApi:Url" appsettings.json'da tanimlidir
}

// YANLIS -- sabit URL
public PanelUserClientService(...)
{
    _baseUrl = "https://localhost:14493/v1/";  // YANLIS - hic bir zaman sabit URL kullanma
}
```

**appsettings.json:**
```json
{
  "TockenApi": {
    "Url": "https://localhost:14493/v1/",
    "BaseUrl": "https://localhost:14493"
  }
}
```

---

## KURAL 13: OCPP WebSocket -- statik dictionary ile baglanti yonetimi

Aktif OCPP baglantilari static Dictionary'de tutulur. lock() ile thread-safe erisim saglanir.

```csharp
// Tum bagli cihazlarin WebSocket oturumlari static tutulur
public static Dictionary<long, DeviceSessionStatusDto> _deviceSessionStatusDict
    = new Dictionary<long, DeviceSessionStatusDto>();

// Yeni baglanti geldiginde -- eski baglantiyi kapat, yenisini ekle
lock (_deviceSessionStatusDict)
{
    if (_deviceSessionStatusDict.ContainsKey(deviceConnection.Id))
        _deviceSessionStatusDict.Remove(deviceConnection.Id, out dummydeviceSessionStatus);
    _deviceSessionStatusDict.Add(deviceConnection.Id, deviceSessionStatus);
}

// Trigger message -- id ile cihazo bul ve mesaj gonder
lock (_deviceSessionStatusDict)
{
    if (_deviceSessionStatusDict.ContainsKey(deviceConnectionId))
        deviceSessionStatus = _deviceSessionStatusDict[deviceConnectionId];
}
```

---

## KURAL 14: OCPP mesaj kuyrugu -- requestQueue ile cevap eslestirmesi

Sunucudan cihaza gonderilen komutlar bir dictionary'de bekletilir, cevap geldiginde uniqueId ile eslestir.

```csharp
// Istek kuyruğu -- per-connection
private Dictionary<string, Ocpp16MessageDto> _requestQueue
    = new Dictionary<string, Ocpp16MessageDto>();

// Komut gonderirken kuyruğa ekle
msg.TaskCompletionSource = new TaskCompletionSource<string>();
_requestQueue.Add(msg.UniqueId, msg);
ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]",
    msg.MessageType, msg.UniqueId, msg.Action, msg.JsonPayload);

// Cevap geldiginde -- uniqueId ile eslestir ve kuyruktan cikar
if (msgIn.MessageType == "3" || msgIn.MessageType == "4")
{
    if (_requestQueue.ContainsKey(msgIn.UniqueId))
    {
        ProcessAnswer(msgIn, _requestQueue[msgIn.UniqueId]);
        _requestQueue.Remove(msgIn.UniqueId);
    }
}

// Hangi komut oldugunu belirle -- action ile switch
switch (msgOut.Action)
{
    case "Reset": handleService.HandleReset(msgIn, msgOut); break;
    case "UnlockConnector": handleService.HandleUnlockConnector(msgIn, msgOut); break;
    case "RemoteStartTransaction": handleService.HandleRemoteStartTransaction(msgIn, msgOut); break;
}
```

---

## KURAL 15: Scoped servis cozumu -- IServiceProvider.CreateScope() kullan

Singleton veya uzun yasayan servislerde scoped servislere erisim icin `CreateScope()` kullan.

```csharp
// Ocpp16ConnectionService -- IServiceProvider injection ile scoped cozum
private readonly IServiceProvider _services;

// Singleton context'ten scoped servis cagirirken scope olustur
private async Task<DeviceConnectionDto> GetDeviceconnection(string identifier, ...)
{
    using (var scope = _services.CreateScope())
    {
        var deviceConnectionService = scope.ServiceProvider
            .GetRequiredService<IDeviceConnectionService>();
        var response = await deviceConnectionService.GetOrAddDeviceConnection(
            new GetOrAddDeviceConnectionRequestDto() { Identifier = identifier });
        // ...
    }
    return deviceConnection;
}

// Her kullanim kendi using bloğunda olmali
using (var scope = _services.CreateScope())
{
    var processService = scope.ServiceProvider
        .GetRequiredService<IOcpp16ProcessRequestAndResponseService>();
    msgOut = await processService.ProcessRequest(msgIn, deviceSessionStatus, datetimeNow);
}
```

---

## KURAL 16: UpdateWithProperties -- secici property guncelleme

Entity guncellemeye sadece degisen property'ler dahil edilir. Tum entity update etme yasaktir.

```csharp
// DOGRU -- Sadece degisen alanlar guncelleniyor
_policyRepository.UpdateWithProperties(policy,
    new Expression<Func<Policy, object>>[]
    {
        s => s.PolicyKey,
        s => s.PolicyContent
    });
policy.PolicyContent = request.PolicyContent;
policy.PolicyKey = _utilService.GetRandomString(64);

// Login form update -- sadece belirli alanlar
_panelLoginFormRepository.UpdateWithProperties(panelLoginForm,
    new Expression<Func<PanelLoginForm, object>>[]
    {
        s => s.UpdatedDate,
        s => s.LoginFormKey,
        s => s.SuccessAttentionDate,
        s => s.IsLogined,
    });
panelLoginForm.UpdatedDate = datetimeNow;
panelLoginForm.IsLogined = true;

// YANLIS -- tum entity guncelleniyor
_dbContext.Update(policy);  // Tum alanlari gunceller, performans sorunu
```

---

## KURAL 17: Login session guvenlik modeli -- anahtarlari surekli yenile

Logout veya session guncelleme sirasinda JWT, session ve remember key'leri yenile.

```csharp
// PanelUserLoginService -- logout'ta key'leri degistir (token gecersiz kil)
var newKeys = _utilService.getRandomStringMulti(128, 3);
var loginSessionJwtKey = newKeys[0];
var loginSessionKey = newKeys[1];
var rememberKey = newKeys[2];

_panelLoginSessionRepository.UpdateWithProperties(loginSession,
    new Expression<Func<PanelLoginSession, object>>[]
    {
        s => s.RememberKey,
        s => s.SessionJwtKey,
        s => s.SessionKey
    });
loginSession.RememberKey = rememberKey;
loginSession.SessionKey = loginSessionKey;
loginSession.SessionJwtKey = loginSessionJwtKey;
await _panelLoginSessionRepository.SaveChangesAsync();
// Eski JWT artik gecersiz -- yeni key ile dogrulama olmaz
```

---

## KURAL 18: WebSocket baglantisi -- subprotocol zorunlu

OCPP baglantisinda `ocpp1.6` veya `ocpp1.6j` subprotocol beklenir. Baska protokolle baglanti reddedilir.

```csharp
private const string Protocol_OCPP16 = "ocpp1.6";
private const string Protocol_OCPP16J = "ocpp1.6j";

private string GetSocketSubProtocol()
{
    string subProtocol = null;
    if (!_customHttpUtilService.GetHttpContext().HttpContext.Response.HasStarted)
    {
        var protocols = _customHttpUtilService.GetHttpContext().HttpContext
            .WebSockets.WebSocketRequestedProtocols;

        if (protocols.Contains(Protocol_OCPP16))
            subProtocol = Protocol_OCPP16;
        else if (protocols.Contains(Protocol_OCPP16J))
            subProtocol = Protocol_OCPP16J;
    }
    return subProtocol;
}

// Kabul etme -- subprotocol ile
using (WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol))
```

---

## KURAL 19: Startup'ta WebSocket secenekleri -- OCPP vs Station farkli ayar

OCPP sunucusu daha uzun keepalive ister.

```csharp
// Station.Api -- sadece buffer boyutu
var webSocketOptions = new WebSocketOptions()
{
    ReceiveBufferSize = 8 * 1024  // 8 KB
};
app.UseWebSockets(webSocketOptions);

// Ocpp.Api -- keepalive da gerekli (OCPP protokolü geregi)
var webSocketOptions = new WebSocketOptions()
{
    ReceiveBufferSize = 8 * 1024,
    KeepAliveInterval = TimeSpan.FromMinutes(10)  // 10 dakika
};
app.UseWebSockets(webSocketOptions);
```

---

## KURAL 20: Ocelot Gateway -- ortam bazinda farkli konfigurasyonlar

Gateway konfigurasyonu ortama gore ayri JSON dosyasindan yuklenmelidir.

```csharp
// GateWay.Api/Program.cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            // Ortama gore ocelot.{Environment}.json yukle
            config.AddJsonFile(
                $"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                optional: true, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

```json
// ocelot.Local.json -- gelistirme ortami
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

---

## KURAL 21: Hata kodlari enum'dan gelir -- sabit integer yasaktir

Hata kodlari Enum tanimli olmali ve `ErrorResult<T>` ile sarmalanmali.

```csharp
// Hata enum ornegi
public enum PanelLoginErrorEnum
{
    [Description("Panel login form olusturulamadi")]
    PANEL_LOGIN_FORM_CAN_NOT_CREATED = 1001,

    [Description("Kullanici bulunamadi")]
    USER_CAN_NOT_FOUND = 1002,

    [Description("Panel oturum gecersiz")]
    PANEL_LOGIN_INVALID = 1003
}

// Kullanim -- enum deger otomatik olarak int'e donusur
return new ErrorResult<LoginResponseDto>(new LoginResponseDto(),
    PanelLoginErrorEnum.USER_CAN_NOT_FOUND);
// ErrorCode = 1002, ErrorMessage = "Kullanici bulunamadi"

// YANLIS -- sabit integer veya sabit string
return new ErrorResult<LoginResponseDto>(new LoginResponseDto(), 1002);  // YANLIS
```

---

## KURAL 22: API versiyonlama -- tum route'lar v{version:apiVersion} ile baslar

Tum endpoint'ler API version prefix'i tasir. Bu convention'dan sapilmaz.

```csharp
// Tum standard controller'lar
[Route("v{version:apiVersion}/[controller]/[action]")]
public class AuthenticationController : ControllerBase { ... }

// Swagger operation ID zorunlu
[SwaggerOperation(OperationId = "Login")]
public async Task<IActionResult> Login(LoginRequestDto loginRequest) { ... }

// Istisna: OCPP WebSocket controller -- versiyonsuz route
[Route("[controller]/[action]/{Identifier}")]  // <- OCPP'de versiyon yok
public class OCPP16Controller : Controller { ... }
```

---

## KURAL 23: ProducesResponseType -- her action metodu response tipi belirtir

Swagger dokumaninin dogru olusturulmasi icin zorunludur.

```csharp
[HttpPost]
[ProducesResponseType(typeof(Result<LoginResponseDto>), statusCode: 200)]
[SwaggerOperation(OperationId = "Login")]
public async Task<IActionResult> Login(LoginRequestDto loginRequest)
{
    var result = await _authenticationService.Login(loginRequest);
    return this.FromResult(result);
}

// Wrapper tipler icin
[ProducesResponseType(typeof(Result<DataTableResponseWrapper<OcppCommandMessageDto>>), 200)]
```

---

## KURAL 24: BaseStartup -- ProjectPrefix ile assembly filtreleme

Her API sadece kendi assembly'lerini ve Shared assembly'lerini yukler.

```csharp
// BaseStartup.cs
ApiOptions = new ApiOptions()
{
    ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
    // Sadece bu proje ve Shared DLL'ler yuklenir
    RegistrationAssemblies = Directory
        .EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
        .Where(filePath =>
            Path.GetFileName(filePath).StartsWith(ProjectPrefix)  // ornek: "Tocken"
         || Path.GetFileName(filePath).StartsWith("Shared"))
        .Select(Assembly.LoadFrom)
};

// appsettings'te prefix tanimlanir
// "StartupConfigs": { "ProjectPrefix": "Tocken" }
```

**Ornek:** Tocken.Api icin `ProjectPrefix = "Tocken"` olunca sadece
`Tocken.Application.dll`, `Tocken.Persistence.dll`, `Shared.Domain.dll` yuklenir.

---

## KURAL 25: Upsert pattern -- var ise guncelle, yoksa ekle

Veri ekleme ve guncelleme islemleri genellikle upsert seklinde yazilir.

```csharp
// PolicyManagmentService -- upsert ornegi
var policy = await _policyRepository.GetPolicy(policyFilter).FirstOrDefaultAsync();

if (policy != null)
{
    // Var -- sadece belirli alanları guncelle
    _policyRepository.UpdateWithProperties(policy,
        new Expression<Func<Policy, object>>[] { s => s.PolicyKey, s => s.PolicyContent });
    policy.PolicyContent = request.PolicyContent;
    policy.PolicyKey = _utilService.GetRandomString(64);
}
else
{
    // Yok -- yeni kayit olustur
    policy = _mapper.Map<Policy>(request, opt =>
    {
        opt.AfterMap((src, dest) =>
        {
            (dest as Policy).PolicyKey = _utilService.GetRandomString(64);
        });
    });
    await _policyRepository.InsertAsync(policy);
}

await _policyRepository.SaveChangesAsync();
return new SuccessResult<UpdatePolicyResponseDto>(new UpdatePolicyResponseDto());
```

---

## Ozet Tablo

| Kural No | Konu | Temel Pattern |
|----------|------|---------------|
| 1 | DbContext izolasyonu | Her servis kendi DB'sine sahip |
| 2 | UnitOfWork | Tum DbContext UoW'den turetilir |
| 3 | Soft delete | BaseEntity.Deleted = false varsayilan |
| 4 | Result<T> | SuccessResult / ErrorResult her zaman |
| 5 | Controller temizligi | Controller'da is mantigi yok |
| 6 | Autofac | ConfigureContainer zorunlu |
| 7 | AutoMapper | AfterMap ile post-mapping |
| 8 | InnerRequestAttribute | Servisler arasi erisim kontrolu |
| 9 | ServiceFilter | DI destekli filter baglama |
| 10 | Log filter | Her API'de request/response log |
| 11 | RabbitMQ singleton | LogProducer singleton yasam suresi |
| 12 | HTTP Client URL | appsettings'ten oku, sabit yazma |
| 13 | OCPP static dict | lock() ile thread-safe baglanti dict |
| 14 | OCPP requestQueue | uniqueId ile cevap eslestirme |
| 15 | Scoped servis | CreateScope() singleton icinde |
| 16 | UpdateWithProperties | Secici property guncelleme |
| 17 | Session guvenlik | Logout'ta key'leri yenile |
| 18 | WebSocket protocol | ocpp1.6 subprotocol zorunlu |
| 19 | WebSocket ayarlar | OCPP 10dk keepalive, Station 8KB buffer |
| 20 | Ocelot ortam | ocelot.{Environment}.json |
| 21 | Hata kodlari | Enum tabanli, sabit integer yasak |
| 22 | API versiyonlama | v{version:apiVersion} zorunlu |
| 23 | ProducesResponseType | Her action icin tanimlama zorunlu |
| 24 | Assembly filtreleme | ProjectPrefix ile sadece ilgili DLL |
| 25 | Upsert pattern | Kontrol et, var ise guncelle, yok ise ekle |
