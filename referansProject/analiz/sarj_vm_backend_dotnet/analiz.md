# sarj_vm_backend_dotnet — Detayli Mimari Analiz
Orijinal: `E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop`

## 1. Platform & Tech Stack

| Katman | Teknoloji |
|--------|-----------|
| Framework | .NET (5 veya 6) |
| ORM | Entity Framework Core |
| Veritabanı | SQL Server |
| DI Container | Autofac |
| Mesajlaşma | MassTransit + RabbitMQ |
| API Gateway | Ocelot |
| Realtime | SignalR (OCPP için) |
| Container | Docker / docker-compose (4 environment × 2 dosya) |

## Genel Bakış

`sarj_vm_backend_dotnet` (rotawattvmbackend-develop), `sarj_backend_dotnet`'e kıyasla daha küçük, VM (Virtual Machine / cihaz yönetimi) ve şarj odaklı bir microservice backend'dir. 4 uygulama servisi içerir ve her biri Ocelot API Gateway arkasında çalışır.

## Servisler

| Servis | Sorumluluk |
|---|---|
| `Vm.Api` | VM/cihaz bağlantı yönetimi, OCPP protokol işlemleri, transaction yönetimi |
| `VmPanel.Api` | Auth, admin yönetimi, CPO (Charge Point Operator) yönetimi, istasyon konfigürasyonu |
| `VmLog.Api` | Request/response ve exception loglama (RabbitMQ consumer) |
| `GateWay.Api` | Ocelot tabanlı API gateway — routing, kimlik doğrulama, rate limiting |

## Mimari Pattern

**Microservices + API Gateway (Ocelot) + Event-Driven (Log)**

```
İstemci
    ↓
GateWay.Api (Ocelot)
    ├── /vm/*        → Vm.Api
    ├── /vmpanel/*   → VmPanel.Api
    └── /log/*       → VmLog.Api (direkt veya event-driven)
```

### Her Servisin Katman Yapısı

```
[ServiceName].Api              ← Presentation
    ↓
[ServiceName].Application      ← Business Logic
    ↓
[ServiceName].Persistence      ← Data Access
    ↓
Shared.Domain                  ← Ortak DTO / Interface
    ↓
FrameworkCore                  ← Base sınıflar (sarj_backend ile aynı)
```

## Klasör Yapısı

```
src/
├── Core/
│   ├── Applications/
│   │   ├── Vm.Application/
│   │   ├── VmPanel.Application/
│   │   └── VmLog.Application/
│   └── Persistences/
│       ├── Vm.Persistence/
│       └── VmLog.Persistence/
├── Presentation/
│   ├── Vm.Api/
│   ├── VmPanel.Api/
│   ├── VmLog.Api/
│   └── GateWay.Api/
└── Shared/
    └── Shared.Domain/

Framework/
└── Core/
    └── FrameworkCore/
```

## Vm.Application — OCPP ve Cihaz Yönetimi

VM (Virtual Machine / Cihaz) servisi şu sorumlulukları taşır:
- Fiziksel şarj cihazlarıyla OCPP (Open Charge Point Protocol) iletişimi
- Şarj oturumu başlatma / durdurma işlemleri
- Bağlantı durumu izleme (WebSocket / SignalR)
- Transaction (şarj işlemi) kayıt ve yönetimi

**Kritik:** OCPP protokol mesajları (BootNotification, Heartbeat, StartTransaction, StopTransaction, StatusNotification) bu servis tarafından işlenir.

## VmPanel.Application — Yönetici Paneli

- Authentication / Authorization
- Admin kullanıcı yönetimi
- CPO (Charge Point Operator) konfigürasyonu
- Şarj istasyonu ekleme / güncelleme / silme
- Cihaz konfigürasyonu

## VmLog.Application — Log Servisi

`sarj_backend_dotnet`'teki Log.Api ile benzer pattern:
- RabbitMQ consumer olarak çalışır
- Diğer servisler log event'lerini publish eder
- VmLog consumer'ı olayı alıp kaydeder
- Request/response pair + exception loglama

## GateWay.Api — Ocelot Konfigürasyonu

Ocelot, JSON tabanlı route konfigürasyonu ile çalışır:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "vm.api", "Port": 80 }],
      "UpstreamPathTemplate": "/vm/{everything}",
      "UpstreamHttpMethod": ["POST", "GET"],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer"
      }
    }
  ]
}
```

Gateway'in sorumlulukları:
- **Routing:** İstekleri doğru downstream servise yönlendir
- **Authentication:** JWT token doğrulama (token içeriğini kontrol et)
- **Rate Limiting:** İstemci başına istek kotası
- **Load Balancing:** Opsiyonel, birden fazla instance varsa

## Docker Ortam Yapısı

4 farklı ortam için 4 çift docker-compose dosyası:

```
docker-compose.dev.yml + docker-compose.dev.override.yml
docker-compose.local.yml + docker-compose.local.override.yml
docker-compose.test.yml + docker-compose.test.override.yml
docker-compose.prod.yml + docker-compose.prod.override.yml
```

Her ortamda servis konfigürasyonları farklıdır (log seviyesi, replica sayısı, resource limiti vb.)

## sarj_backend_dotnet ile Farklar

| Özellik | sarj_backend_dotnet | sarj_vm_backend_dotnet |
|---|---|---|
| Servis sayısı | 20+ | 4 |
| API Gateway | GateWay.Api (özel) | GateWay.Api (Ocelot) |
| Ortam sayısı | 1 docker-compose | 4 ortam × 2 dosya |
| Log DB | SQL Server | SQL Server (Ocelot log ayrı olabilir) |
| Odak | Tam EV şarj platformu | VM/cihaz + şarj protokolü |

## Dikkat Çeken Noktalar

### Olumlu
- Ocelot gateway ile merkezi authentication ve routing
- 4 ortam yönetimi sistematik (dev/local/test/prod)
- Küçük scope — 4 servis, bakımı kolay
- OCPP protokolü entegre

### İyileştirme Alanları
- VmPanel.Persistence eksik (listelenmemiş) — VmPanel muhtemelen ayrı DB kullanmıyor veya Vm.Persistence paylaşıyor
- Ocelot konfigürasyon dosyaları environment'a göre dışarıdan mount edilmeli

## Sonuç

Bu proje, daha büyük `sarj_backend_dotnet` platformunun cihaz yönetimi alt sistemini bağımsız bir backend olarak implemente ediyor. Ocelot API Gateway kullanımı, 4 servisli mimaride routing ve auth yönetimini merkezi hale getiriyor. 4 ortam desteği ile deployment pipeline'i olgunlasmis durumda.

---

## 2. Gercek Kod Bloklari

### GateWay.Api — ocelot.json (gercek dosya)

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/{url}",
      "DownstreamScheme": "ws",              // WebSocket - OCPP cihaz baglantisi
      "DownstreamHostAndPorts": [
        { "Host": "vm.api", "Port": 80 }
      ],
      "UpstreamPathTemplate": "/vm/{url}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ]
    },
    {
      "DownstreamPathTemplate": "/v1/{url}",
      "DownstreamScheme": "http",            // HTTP - VmPanel REST API
      "DownstreamHostAndPorts": [
        { "Host": "vmpanel.api", "Port": "80" }
      ],
      "UpstreamPathTemplate": "/web/{url}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ],
      "FileCacheOptions": { "TtlSeconds": 0 },
      "LoadBalancerOptions": { "Type": "LeastConnection" },
      "DangerousAcceptAnyServerCertificateValidator": true
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "gatewayapi:5080",
    "DangerousAcceptAnyServerCertificateValidator": true
  }
}
```

**Onemli nokta**: `/vm/{url}` upstream WebSocket (ws) olarak vm.api'ye gidiyor — OCPP cihaz baglantisi WebSocket protokolü kullaniliyor.

### Vm.Api — MocppController.cs

```csharp
// src/Presentation/Vm.Api/Controllers/MocppController.cs
// "Mocpp" = Mock OCPP (Virtual Machine OCPP proxy)

[ServiceFilter(typeof(VmRequestResponseLogFilterAttribute))]  // Request/response log filter
public class MocppController : Controller  // Not ControllerBase - WebSocket icin Controller
{
    private readonly IVmConnectionService _vmConnectionService;

    public MocppController(IVmConnectionService vmConnectionService)
    {
        _vmConnectionService = vmConnectionService;
    }

    // WebSocket endpointi — HTTP degil, WS upgrade
    // Route: /Mocpp/{Identifier}  — gateway /vm/{url} -> vm.api /{url}
    [Route("[controller]/{Identifier}")]
    public async Task ConnectionDevice(string Identifier)
    {
        await _vmConnectionService.ConnectionDevice(Identifier);
        // Metot void gibi görünüyor ama WebSocket kabul edince bloklar
        // WebSocket kapaninana kadar burada bekler
    }
}
```

**Mimari not**: Controller burada sadece delegasyon yapiyor. Tüm is mantigi `VmConnectionService`'in icinde. WebSocket `AcceptWebSocketAsync` ile kabul ediliyor ve servis boyunca mesaj döngüsü devam ediyor.

### VmConnectionService.cs — Temel yapi

```csharp
// src/Core/Applications/Vm.Application/Services/ConnectionManagement/VmConnectionService.cs
// partial class — 18 parcaya bölünmüs

public partial class VmConnectionService : BaseService, IVmConnectionService
{
    ICustomHttpUtilService _customHttpUtilService;
    ILogger<VmConnectionService> _logger;
    private readonly IServiceProvider _services;  // Scoped servisler için IServiceProvider
    private readonly IConfiguration _configuration;

    // Thread-safe global session dictionary — tüm aktif cihaz baglantilari
    public static ConcurrentDictionary<string, VmConnectionSessionDto> _vmSessionStatusDict
        = new ConcurrentDictionary<string, VmConnectionSessionDto>();

    // Rate limiting — ayni mesaj tipinin kısa sürede tekrar gönderilmesini önler
    private static ConcurrentDictionary<string, DateTime> _triggerMessageRateLimits
        = new ConcurrentDictionary<string, DateTime>();

    private const int TriggerMessageRateLimitSeconds = 10;
    private const int TriggerMessageBootNotificationRateLimitSeconds = 120; // 2 dakika

    // Thread-safe Random (Guid seed ile)
    private static readonly ThreadLocal<Random> _randomGenerator
        = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

    // Non-blocking reconnect tracking
    private readonly ConcurrentDictionary<string, bool> _serverReconnectInProgress = new();
    private readonly ConcurrentDictionary<string, bool> _cpoReconnectInProgress = new();

    private const int MaxRetryAttempts = 5;
    private const int InitialRetryDelayMs = 1000;   // 1 saniye
    private const int MaxRetryDelayMs = 30000;      // 30 saniye
    private const int ConnectionMonitorIntervalMs = 30000; // 30 sn'de bir kontrol
    private const string Protocol_OCPP16 = "ocpp1.6";
    private const string Protocol_OCPP16J = "ocpp1.6j";

    public async Task ConnectionDevice(string identifier)
    {
        VmConnectionSessionDto vmConnectionSession = null;
        try
        {
            if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Scoped servis: VmParameter DB'den cekilir
                VmParameter vmParameter = null;
                using (var scope = _services.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IVmParameterRepository>();
                    vmParameter = await repo.GetVmParameter(new VmParameterFilterDto() { }, null).FirstOrDefaultAsync();
                }

                if (vmParameter != null && vmParameter.IsVmActive)
                {
                    // Cihaz baglantisi kaydi al veya olustur
                    GetOrAddVmDeviceConnectionResponseDto vmDeviceConnection = null;
                    using (var scope = _services.CreateScope())
                    {
                        var svc = scope.ServiceProvider.GetRequiredService<IVmDeviceConnectionManagementService>();
                        vmDeviceConnection = await svc.GetOrAddVmDeviceConnection(new GetOrAddVmDeviceConnectionRequestDto
                        {
                            Identifier = identifier,
                            Date = DateTime.UtcNow,   // OCPP UTC zorunlu
                            ProtocolList = _customHttpUtilService.GetHttpContext().HttpContext.WebSockets.WebSocketRequestedProtocols.ToList()
                        });
                    }

                    if (vmDeviceConnection != null)
                    {
                        await SaveVmCommandMessage(vmDeviceConnection.Id, identifier);
                        string subProtocol = GetSocketSubProtocol();  // "ocpp1.6" veya "ocpp1.6j"
                        vmConnectionSession = InitVmConnectionSession(identifier, vmDeviceConnection, subProtocol);

                        bool statusSuccess = await UpdateVmDictionaryForDevice(vmConnectionSession, identifier);
                        if (statusSuccess)
                        {
                            _logger.LogInformation("Device connected: {Identifier}", identifier);

                            // Startup reconciliation: orphan transaction taramasi
                            _ = Task.Run(async () => { try { await RunStartupReconciliationAsync(); } catch { } });

                            // WebSocket kabul edildi
                            using (WebSocket webSocket = await _customHttpUtilService.GetHttpContext()
                                .HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol))
                            {
                                vmConnectionSession.DeviceWebSocket = webSocket;
                                byte[] buffer = new byte[1024 * 4];
                                using (MemoryStream memStream = new MemoryStream(buffer.Length))
                                {
                                    // Ana döngü: cihazdan mesaj geldiginde isle
                                    await MainProcess(vmConnectionSession,
                                        vmDeviceConnection.VmDevice.VmStation.VmCpo.OcppUrlAddress,
                                        vmParameter.ServerUrl, memStream, buffer);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                await DeviceCannotStartConnect(identifier);
            }
        }
        catch (Exception ex)
        {
            await DeviceCannotStartConnectWithException(identifier, ex);
        }
    }

    // Ana mesaj döngüsü
    private async Task MainProcess(VmConnectionSessionDto vmConnectionSession,
        string ocppUrlAddress, string serverUrlAddress,
        MemoryStream memoryStreamDevice, byte[] buffer)
    {
        try
        {
            // CPO ve server baglantilari kur
            var (uriServer, hasValidServerUrl) = await SetupServerConnection(vmConnectionSession, serverUrlAddress);
            var (uriCpo, hasValidCpoUrl) = await SetupCpoConnection(vmConnectionSession, ocppUrlAddress);

            int firstConnection = 1; // Atomic flag

            while (vmConnectionSession.DeviceWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult resultDevice;
                try
                {
                    resultDevice = await vmConnectionSession.DeviceWebSocket.ReceiveAsync(
                        buffer, vmConnectionSession.DeviceCancellationTokenSource.Token);
                }
                catch (WebSocketException wsEx) { break; }
                catch (OperationCanceledException)  { break; }

                if (resultDevice != null && resultDevice.MessageType != WebSocketMessageType.Close)
                {
                    memoryStreamDevice.Write(buffer, 0, resultDevice.Count);

                    // Reconnect kontrol ve listener'lari baslat
                    CheckAndReconnectServer(vmConnectionSession, uriServer, hasValidServerUrl);
                    if (hasValidServerUrl) EnsureServerListenTaskStarted(vmConnectionSession);
                    CheckAndReconnectCpo(vmConnectionSession, uriCpo, hasValidCpoUrl);
                    if (hasValidCpoUrl && uriCpo != null) EnsureCpoListenTaskStarted(vmConnectionSession, uriCpo);

                    // Monitor'lari baslat (heartbeat, connection health)
                    await EnsureHeartbeatMonitorStarted(vmConnectionSession);
                    await EnsureConnectionHealthMonitorStarted(vmConnectionSession, serverUrlAddress, ocppUrlAddress);

                    if (resultDevice.EndOfMessage)
                    {
                        try { await ProcessIncomingDeviceMessage(vmConnectionSession, buffer, memoryStreamDevice, uriServer, uriCpo); }
                        catch (Exception msgEx) { /* MESSAGE_GUARD: hata yut, baglanti koru */ }
                    }

                    // Ilk mesaj geldiginde DB guncelle (tek seferlik, atomic)
                    if (Interlocked.CompareExchange(ref firstConnection, 0, 1) == 1)
                        await UpdateDeviceConnectionDb(vmConnectionSession, true, ChargeDeviceInstantStateEnum.AVAILABLE);
                }
                else
                {
                    await DeviceDisconnected(vmConnectionSession);
                    break;
                }
            }

            await DeviceDisconnected(vmConnectionSession);
        }
        catch (Exception ex)
        {
            await DeviceDisconnectedWithException(vmConnectionSession, ex);
        }
    }

    // OCPP mesaj yönlendirme — action string'e göre dispatch
    private async Task ProcessDeviceMessages(VmConnectionSessionDto session,
        Uri uriServer, Uri uriCpo, string messageType, string messageId, JToken payloadToken, string action)
    {
        if (action == OcppFromChargePointActionTypeEnum.METER_VALUES.ToDescriptionString())
        {
            await SendMeterValuesResponseToDevice(session, messageId, payloadToken);
            await SendMeterValuesToCpoAndServer(session, uriServer, uriCpo, messageType, messageId, payloadToken, action);
        }
        else if (action == OcppFromChargePointActionTypeEnum.HEARTBEAT.ToDescriptionString())
        {
            await SendHeartbeatResponseToDevice(session, messageId, payloadToken);
            await SendHeartbeatToCpoAndServer(session, uriServer, uriCpo, messageType, messageId, payloadToken, action);
        }
        else if (action == OcppFromChargePointActionTypeEnum.AUTHORIZE.ToDescriptionString())
        {
            // AUTOCHARGE FIX: Cihaza hemen yanit gönderilmiyor
            // CPO/Server yaniti gelince nihai yanit gönderilir
            await SendAuthorizeToCpoAndServer(session, uriServer, uriCpo, messageType, messageId, payloadToken, action);
        }
        else if (action == OcppFromChargePointActionTypeEnum.START_TRANSACTION.ToDescriptionString())
        {
            if (session.TransactionTarget == TransactionTargetEnum.VM_ONLY)
                await SendStartTransactionResponseToDevice(session, messageId, payloadToken);
            else
                await HandleStartTransactionWithExternalWait(session, uriServer, uriCpo, messageType, messageId, payloadToken, action);
        }
        else if (action == OcppFromChargePointActionTypeEnum.STOP_TRANSACTION.ToDescriptionString())
        {
            await SendStopTransactionResponseToDevice(session, messageId, payloadToken);
            await SendStopTransactionToCpoAndServer(session, uriServer, uriCpo, messageType, messageId, payloadToken, action);
        }
        // + BOOT_NOTIFICATION, STATUS_NOTIFICATION, DATA_TRANSFER, DIAGNOSTIC, FIRMWARE_STATUS
    }

    // Mesaj geldiginde heartbeat zamani guncelle (missedHeartbeat'i sifirla)
    private async Task ProcessIncomingDeviceMessage(...)
    {
        // ...parse...
        vmConnectionSession.LastHeartbeatTime = DateTime.UtcNow;
        vmConnectionSession.MissedHeartbeatCount = 0;  // Her mesaj = alive
        // ...dispatch...
    }
}
```

### docker-compose.yml (VM backend)

```yaml
services:
  gateway.api:
    image: rotawatt/gatewayapi:1
    build:
      context: .
      dockerfile: src/Presentation/GateWay.Api/Dockerfile

  rabbitmq:
    image: rabbitmq:3-management-alpine
    environment:
      - "RABBITMQ_DEFAULT_PASS=[MASKED]"
      - "RABBITMQ_DEFAULT_USER=[MASKED]"

  vm.api:
    image: rotawatt/vm:1
    build:
      context: .
      dockerfile: src/Presentation/Vm.Api/Dockerfile

  vmpanel.api:
    image: rotawatt/vmpanel:1
    build:
      context: .
      dockerfile: src/Presentation/VmPanel.Api/Dockerfile

  vmlog.api:
    image: rotawatt/vmlog:1
    build:
      context: .
      dockerfile: src/Presentation/VmLog.Api/Dockerfile
```

---

## 3. Mimari Kararlar (VM Spesifik)

| Karar | Aciklama |
|-------|----------|
| `partial class VmConnectionService` | 18 partial dosya. `ConnectionDevice`, `MainProcess`, `Listeners`, `Reconnect`, `Persistence`, `WebSocketSend`, `Reconciliation` gibi ayri sorumluluklar |
| `ConcurrentDictionary<string, VmConnectionSessionDto>` | Thread-safe global session registry. Her Identifier icin aktif WebSocket session ve durumu |
| `IServiceProvider _services` | WebSocket uzun süre acik kalir. Scoped servisler `CreateScope()` ile alinir, kapsam bitince dispose olur |
| Atomic `firstConnection` | `Interlocked.CompareExchange` ile DB update sadece bir kez yapilir, race condition önlenir |
| Heartbeat monitor | Her gelen mesaj `LastHeartbeatTime` ve `MissedHeartbeatCount` gunceller. Ayri monitor gorevi heartbeat bekleniyor mu diye bakar |
| Message guard try/catch | Ana mesaj isleme try/catch ile sariyor — hata olsa bile WebSocket baglantisi kesilmiyor |
| Rate limiting | `TriggerMessage` icin ayni cihazdaayni mesaj tipi icin 10 sn minimum ara zorlu |
| Per-device semaphore | Global semaphore yerine per-device semaphore (SessionDto icinde). Bir cihaz diger cihazlarin reconnect'ini bloklamiyor |
| OCPP UTC zorunlu | `DateTime.UtcNow` - OCPP standardı UTC gerektirir |

---

## 4. sarj_backend_dotnet ile Kiyaslama

| Ozellik | sarj_backend_dotnet | sarj_vm_backend_dotnet |
|---------|---------------------|------------------------|
| Servis sayisi | 20+ | 4 (Vm, VmPanel, VmLog, GateWay) |
| Ana protokol | REST/HTTP | WebSocket (OCPP 1.6) |
| Gateway tipi | Ozel GateWay | Ocelot tabanlı |
| Oturum yonetimi | HTTP session/JWT | ConcurrentDictionary WebSocket session |
| Connection scope | Request scoped | Long-lived WebSocket connection |
| Partial class | Hayir | Evet (18 dosya) |
| Ortam sayisi | 1 docker-compose | 4 ortam (dev/local/test/prod) |
| Odak | Odeme, Sarj, Cuzdan | OCPP cihaz proxy, VM yonetimi |

---

## 5. Potansiyel Sorunlar

1. **Global static dictionary**: `_vmSessionStatusDict` ve `_triggerMessageRateLimits` static. Uygulama yeniden basladiginda kaybolur; cihazlar yeniden baglanmak zorunda kalir.

2. **Message guard hata yutma**: Mesaj isleme hatasi loglanip devam ediliyor ama hata turu ve frekansi izlenmiyor. Silent failure riski.

3. **`DangerousAcceptAnyServerCertificateValidator: true`**: Ocelot konfig ve WebSocket baglantida TLS sertifika dogrulamasi devre disi. Production'da risk.

4. **Scoped service resolve**: Her mesajda `CreateScope()` yapiliyor. Yüksek mesaj hacminde overhead olusabilir; pooling dusunulmeli.

5. **MemoryStream buffer reuse**: `MemoryStream` `buffer.Length` ile baslatiyor ama Position sifirlanmiyor mu? Fragmented mesajlarda sorun olabilir.

6. **Heartbeat zaman damgasi**: `LastHeartbeatTime = DateTime.UtcNow` her gelen mesajda guncelleniyor. Bu iyi bir pratik ama heartbeat'i OCPP'de tanimlanmis belirli bir interval beklemeyi zorluyor; cihazlar heartbeat gondermeden diger mesajlarla bunu gecebilir.

---

## 6. TUM CONTROLLER KODLARI (VmPanel.Api)

### AuthenticationController.cs

```csharp
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.VmPanelDto.AuthenticationDtos;
using Shared.Domain.ServiceInterfaces.VmPanelServiceInterfaces.AuthenticationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using VmPanel.Application.Filters;

namespace VmPanel.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LoginForm")]
        public async Task<IActionResult> LoginForm(LoginFormRequestDto loginFormRequest)
        {
            var result = await _authenticationService.LoginForm(loginFormRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Login")]
        [ValidateFilter]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var result = await _authenticationService.Login(loginRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LogoutResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var result = await _authenticationService.LogOut();
            return this.FromResult(result);
        }
    }
}
```

### CpoManagementController.cs

```csharp
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
[Authorize]
[ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
public class CpoManagementController : ControllerBase
{
    private readonly ICpoManagementService _cpoManagementService;
    private readonly IConfiguration _configuration;

    public CpoManagementController(IConfiguration configuration, ICpoManagementService cpoManagementService)
    {
        _configuration = configuration;
        _cpoManagementService = cpoManagementService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<AddCpoResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "AddCpo")]
    public async Task<IActionResult> AddCpo(AddCpoRequestDto addCpoRequest)
    {
        var result = await _cpoManagementService.AddCpo(addCpoRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<UpdateCpoResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "UpdateCpo")]
    public async Task<IActionResult> UpdateCpo(UpdateCpoRequestDto updateCpoRequest)
    {
        var result = await _cpoManagementService.UpdateCpo(updateCpoRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CpoDataTableItemDto>>), statusCode: 200)]
    [SwaggerOperation(OperationId = "GetCpoDataTablePanel")]
    public async Task<IActionResult> GetCpoDataTablePanel(DataTableFilterModel<GetCpoDataTableRequestDto> dataTableFilterModel)
    {
        var result = await _cpoManagementService.GetCpoDataTablePanel(dataTableFilterModel);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetCpoForUpdateResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "GetCpoForUpdate")]
    public async Task<IActionResult> GetCpoForUpdate(GetCpoForUpdateRequestDto getCpoForUpdateRequest)
    {
        var result = await _cpoManagementService.GetCpoForUpdate(getCpoForUpdateRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<GetCpoForSelectListResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "GetCpoForSelectList")]
    public async Task<IActionResult> GetCpoForSelectList(GetCpoForSelectListRequestDto getCpoForSelectListRequest)
    {
        var result = await _cpoManagementService.GetCpoForSelectList(getCpoForSelectListRequest);
        return this.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<RemoveCpoResponseDto>), statusCode: 200)]
    [SwaggerOperation(OperationId = "RemoveCpo")]
    public async Task<IActionResult> RemoveCpo(RemoveCpoRequestDto removeCpoRequest)
    {
        var result = await _cpoManagementService.RemoveCpo(removeCpoRequest);
        return this.FromResult(result);
    }
}
```

### VmDeviceManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmDeviceManagementController : ControllerBase
{
    private readonly IVmDeviceManagementService _vmDeviceManagementService;
    private readonly IConfiguration _configuration;

    public VmDeviceManagementController(IConfiguration configuration, IVmDeviceManagementService vmDeviceManagementService)
    {
        _configuration = configuration;
        _vmDeviceManagementService = vmDeviceManagementService;
    }

    [HttpPost][SwaggerOperation(OperationId = "AddVmDevice")]
    public async Task<IActionResult> AddVmDevice(AddVmDeviceRequestDto addVmDeviceRequest)
    {
        var result = await _vmDeviceManagementService.AddVmDevice(addVmDeviceRequest);
        return this.FromResult(result);
    }

    [HttpPost][SwaggerOperation(OperationId = "UpdateVmDevice")]
    public async Task<IActionResult> UpdateVmDevice(UpdateVmDeviceRequestDto updateVmDeviceRequest)
    {
        var result = await _vmDeviceManagementService.UpdateVmDevice(updateVmDeviceRequest);
        return this.FromResult(result);
    }

    [HttpPost][SwaggerOperation(OperationId = "GetVmDeviceDataTablePanel")]
    public async Task<IActionResult> GetVmDeviceDataTablePanel(DataTableFilterModel<GetVmDeviceDataTableRequestDto> dataTableFilterModel)
    {
        var result = await _vmDeviceManagementService.GetVmDeviceDataTablePanel(dataTableFilterModel);
        return this.FromResult(result);
    }

    [HttpPost][SwaggerOperation(OperationId = "GetVmDeviceForUpdate")]
    public async Task<IActionResult> GetVmDeviceForUpdate(GetVmDeviceForUpdateRequestDto getVmDeviceForUpdateRequest)
    {
        var result = await _vmDeviceManagementService.GetVmDeviceForUpdate(getVmDeviceForUpdateRequest);
        return this.FromResult(result);
    }
}
```

### VmStationManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmStationManagementController : ControllerBase
{
    private readonly IVmStationManagementService _vmStationManagementService;
    private readonly IConfiguration _configuration;

    public VmStationManagementController(IConfiguration configuration, IVmStationManagementService vmStationManagementService)
    {
        _configuration = configuration;
        _vmStationManagementService = vmStationManagementService;
    }

    [HttpPost] public async Task<IActionResult> AddVmStation(AddVmStationRequestDto r) => this.FromResult(await _vmStationManagementService.AddVmStation(r));
    [HttpPost] public async Task<IActionResult> UpdateVmStation(UpdateVmStationRequestDto r) => this.FromResult(await _vmStationManagementService.UpdateVmStation(r));
    [HttpPost] public async Task<IActionResult> GetVmStationDataTablePanel(DataTableFilterModel<GetVmStationDataTableRequestDto> r) => this.FromResult(await _vmStationManagementService.GetVmStationDataTablePanel(r));
    [HttpPost] public async Task<IActionResult> GetVmStationForUpdate(GetVmStationForUpdateRequestDto r) => this.FromResult(await _vmStationManagementService.GetVmStationForUpdate(r));
    [HttpPost] public async Task<IActionResult> RemoveVmStation(RemoveVmStationRequestDto r) => this.FromResult(await _vmStationManagementService.RemoveVmStation(r));
    [HttpPost] public async Task<IActionResult> GetVmStationsForSelectList(GetVmStationsForSelectListRequestDto r) => this.FromResult(await _vmStationManagementService.GetVmStationsForSelectList(r));
}
```

### VmPanelAdminManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmPanelAdminManagementController : ControllerBase
{
    private readonly IVmPanelAdminManagementService _vmPanelAdminManagementService;
    private readonly IConfiguration _configuration;

    public VmPanelAdminManagementController(IConfiguration configuration, IVmPanelAdminManagementService vmPanelAdminManagementService)
    {
        _configuration = configuration;
        _vmPanelAdminManagementService = vmPanelAdminManagementService;
    }

    [HttpPost] public async Task<IActionResult> AddVmPanelAdmin(AddVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.AddVmPanelAdmin(r));
    [HttpPost] public async Task<IActionResult> UpdateVmPanelAdmin(UpdateVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.UpdateVmPanelAdmin(r));
    [HttpPost] public async Task<IActionResult> GetVmPanelAdminForUpdate(GetVmPanelAdminForUpdateRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.GetVmPanelAdminForUpdate(r));
    [HttpPost] public async Task<IActionResult> GetVmPanelAdminsDatatablePanel(DataTableFilterModel<GetVmPanelAdminDataTableRequestDto> r) => this.FromResult(await _vmPanelAdminManagementService.GetVmPanelAdminsDatatablePanel(r));
    [HttpPost] public async Task<IActionResult> ChangeVmPanelAdminActiveState(ChangeVmPanelAdminActiveStateRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.ChangeVmPanelAdminActiveState(r));
    [HttpPost] public async Task<IActionResult> RemoveVmPanelAdmin(RemoveVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.RemoveVmPanelAdmin(r));
}
```

### VmPanelHomeManagementController.cs (VmPanelAuthenticationFilterAttribute — daha az kisitli)

```csharp
[ApiController][Authorize]
[ServiceFilter(typeof(VmPanelAuthenticationFilterAttribute))]  // <-- MainAdmin degil, Authentication
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmPanelHomeManagementController : ControllerBase
{
    private readonly IVmPanelHomeMangementService _vmPanelHomeMangementService;
    private readonly IConfiguration _configuration;

    public VmPanelHomeManagementController(IConfiguration configuration, IVmPanelHomeMangementService vmPanelHomeMangementService)
    {
        _configuration = configuration;
        _vmPanelHomeMangementService = vmPanelHomeMangementService;
    }

    [HttpPost]
    public async Task<IActionResult> PrepareVmPanelHome(PrepareVmPanelHomeRequestDto prepareVmPanelHomeRequest)
    {
        var result = await _vmPanelHomeMangementService.PrepareVmPanelHome(prepareVmPanelHomeRequest);
        return this.FromResult(result);
    }
}
```

### VmTransactionManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmTransactionManagementController : ControllerBase
{
    private readonly IVmTransactionManagementService _vmTransactionManagementService;
    private readonly IConfiguration _configuration;

    public VmTransactionManagementController(IConfiguration configuration, IVmTransactionManagementService vmTransactionManagementService)
    {
        _configuration = configuration;
        _vmTransactionManagementService = vmTransactionManagementService;
    }

    [HttpPost]
    public async Task<IActionResult> GetVmTransactionDataTablePanel(DataTableFilterModel<GetVmTransactionDataTableRequestDto> dataTableFilterModel)
    {
        var result = await _vmTransactionManagementService.GetVmTransactionDataTablePanel(dataTableFilterModel);
        return this.FromResult(result);
    }
}
```

### VmConnectorManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmConnectorManagementController : ControllerBase
{
    private readonly IVmConnectorManagementService _vmConnectorManagementService;
    private readonly IConfiguration _configuration;

    public VmConnectorManagementController(IConfiguration configuration, IVmConnectorManagementService vmConnectorManagementService)
    {
        _configuration = configuration;
        _vmConnectorManagementService = vmConnectorManagementService;
    }

    [HttpPost]
    public async Task<IActionResult> GetVmPanelConnectorPowerType(GetVmPanelConnectorPowerTypeRequestDto r)
    {
        var result = await _vmConnectorManagementService.GetVmPanelConnectorPowerType(r);
        return this.FromResult(result);
    }
}
```

### VmDeviceConnectionManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmDeviceConnectionManagementController : ControllerBase
{
    private readonly IVmDeviceConnectionManagementService _vmDeviceConnectionManagementService;
    private readonly IConfiguration _configuration;

    [HttpPost]
    public async Task<IActionResult> GetVmDeviceConnectionDataTablePanel(DataTableFilterModel<GetVmDeviceConnectionDataTableRequestDto> r)
    {
        var result = await _vmDeviceConnectionManagementService.GetVmDeviceConnectionDataTablePanel(r);
        return this.FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> GetVmCommandMessageDataTablePanel(DataTableFilterModel<GetVmCommandMessageDataTableRequestDto> r)
    {
        var result = await _vmDeviceConnectionManagementService.GetVmCommandMessageDataTablePanel(r);
        return this.FromResult(result);
    }
}
```

### VmConnectorConnectionManagementController.cs

```csharp
[ApiController][Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
[Route("v{version:apiVersion}/[controller]/[action]")]
[ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
public class VmConnectorConnectionManagementController : ControllerBase
{
    private readonly IVmConnectorConnectionManagementService _vmConnectorConnectionManagementService;
    private readonly IConfiguration _configuration;

    [HttpPost]
    public async Task<IActionResult> GetVmConnectorConnectionDataTablePanel(DataTableFilterModel<GetVmConnectorConnectionDataTableRequestDto> r)
    {
        var result = await _vmConnectorConnectionManagementService.GetVmConnectorConnectionDataTablePanel(r);
        return this.FromResult(result);
    }
}
```

### Vm.Api — MocppController.cs (tam kod)

```csharp
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.ConnectionManagementsServiceInterfaces;
using System.Threading.Tasks;
using Vm.Application.Filters;

namespace Vm.Api.Controllers
{
    [ServiceFilter(typeof(VmRequestResponseLogFilterAttribute))]
    public class MocppController : Controller
    {
        private readonly IVmConnectionService _vmConnectionService;

        public MocppController(IVmConnectionService vmConnectionService)
        {
            _vmConnectionService = vmConnectionService;
        }

        [Route("[controller]/{Identifier}")]
        public async Task ConnectionDevice(string Identifier)
        {
            await _vmConnectionService.ConnectionDevice(Identifier);
        }
    }
}
```

---

## 7. TUM ENTITY KODLARI

### VmCpo.cs

```csharp
[Table("VmCpo", Schema = "RotaWatt")]
public class VmCpo : BaseEntity
{
    public string GuiId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public string OcppUrlAddress { get; set; }
    public string CpoSubProtocol { get; set; }
    public VmIntegrationConfig VmIntegrationConfig { get; set; }
    public virtual ICollection<VmStation> VmStation { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public virtual ICollection<VmPanelAdmin> VmPanelAdmin { get; set; }
}
```

### VmStation.cs

```csharp
[Table("VmStation", Schema = "RotaWatt")]
public class VmStation : BaseEntity
{
    public string GuiId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    [ForeignKey("VmStation_VmCpo")]
    public long VmCpoId { get; set; }
    public VmCpo VmCpo { get; set; }
    public virtual ICollection<VmDevice> VmDevice { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
```

### VmDevice.cs

```csharp
[Table("VmDevice", Schema = "RotaWatt")]
public class VmDevice : BaseEntity
{
    public string GuiId { get; set; }
    public string DeviceServerGuiId { get; set; }
    public string Identifier { get; set; }
    [ForeignKey("VmDevice_VmStation")]
    public long VmStationId { get; set; }
    public VmStation VmStation { get; set; }
    public VmDeviceConnection? VmDeviceConnection { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public ChargeDeviceMarkEnum? ChargeDeviceMarkId { get; set; }
    public virtual ICollection<VmConnector> VmConnector { get; set; }
}
```

### VmConnector.cs

```csharp
[Table("VmConnector", Schema = "RotaWatt")]
public class VmConnector : BaseEntity
{
    [MaxLength(36)] public string GuiId { get; set; }
    public string Identifier { get; set; }
    public int ConnectorNo { get; set; }
    [ForeignKey("VmConnector_VmDevice")]
    public long VmDeviceId { get; set; }
    public VmDevice VmDevice { get; set; }
    [ForeignKey("VmConnector_VmConnectorPowerType")]
    public long VmConnectorPowerTypeId { get; set; }
    public VmConnectorPowerType VmConnectorPowerType { get; set; }
    public VmConnectorConnection? VmConnectorConnection { get; set; }
    public virtual ICollection<VmStartTransaction> VmStartTransaction { get; set; }
}
```

### VmDeviceConnection.cs

```csharp
[Table("VmDeviceConnection", Schema = "RotaWatt")]
public class VmDeviceConnection : BaseEntity
{
    public string GuiId { get; set; }
    public bool ConnectionState { get; set; }
    [ForeignKey("VmDeviceConnection_VmDevice")]
    public long VmDeviceId { get; set; }
    public VmDevice VmDevice { get; set; }
    public ChargeDeviceInstantStateEnum? InstantState { get; set; }
    public VmCpoConnection? VmCpoConnection { get; set; }
    public VmServerConnection? VmServerConnection { get; set; }
    public string? ConnectedIpAddress { get; set; }
    public DateTime? ConnectionDate { get; set; }
    public DateTime? DisconnectionDate { get; set; }
    public string? DisconnectionMessage { get; set; }
    public DateTime? HearthBeatDate { get; set; }
    public DateTime? LastInstantStateUpdatedDate { get; set; }
    public DateTime? LastMessageReceiveDate { get; set; }
}
```

### VmCpoConnection.cs

```csharp
[Table("VmCpoConnection", Schema = "RotaWatt")]
public class VmCpoConnection : BaseEntity
{
    public string GuiId { get; set; }
    public bool ConnectionState { get; set; }
    public DateTime ConnectionDate { get; set; }
    public DateTime DisconnectionDate { get; set; }
    public string DisconnectionMessage { get; set; }
    public string ConnectedCpoOcppUrlAddress { get; set; }
    [ForeignKey("VmCpoConnection_VmDeviceConnection")]
    public long VmDeviceConnectionId { get; set; }
    public VmDeviceConnection VmDeviceConnection { get; set; }
}
```

### VmServerConnection.cs

```csharp
[Table("VmServerConnection", Schema = "RotaWatt")]
public class VmServerConnection : BaseEntity
{
    public string GuiId { get; set; }
    public bool ConnectionState { get; set; }
    public DateTime ConnectionDate { get; set; }
    public DateTime DisconnectionDate { get; set; }
    public string DisconnectionMessage { get; set; }
    public string ConnectedServerUrlAddress { get; set; }
    [ForeignKey("VmServerConnection_VmDeviceConnection")]
    public long VmDeviceConnectionId { get; set; }
    public VmDeviceConnection VmDeviceConnection { get; set; }
}
```

### VmConnectorConnection.cs

```csharp
[Table("VmConnectorConnection", Schema = "RotaWatt")]
public class VmConnectorConnection : BaseEntity
{
    public string GuiId { get; set; }
    [ForeignKey("VmConnectorConnection_VmConnector")]
    public long VmConnectorId { get; set; }
    public VmConnector VmConnector { get; set; }
    public ChargeDeviceConnectorInstantStateEnum? InstantState { get; set; }
    public DateTime? LastInstantStateUpdatedDate { get; set; }
}
```

### VmStartTransaction.cs

```csharp
[Table("VmStartTransaction", Schema = "RotaWatt")]
public class VmStartTransaction : BaseEntity
{
    public string GuiId { get; set; }
    public string MessageUniqueId { get; set; }
    public double MeterStart { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public TransactionStateEnum State { get; set; }
    [MaxLength(64)] public string TransactionIdTag { get; set; }
    [ForeignKey("VmStartTransaction_VmConnector")]
    public long VmConnectorId { get; set; }
    public VmConnector VmConnector { get; set; }
    public long? VmIdTagInfoId { get; set; }
    public VmIdTagInfo? VmIdTagInfo { get; set; }
    public long? CpoTransactionId { get; set; }
    public long? ServerTransactionId { get; set; }
    public string? CpoIdTag { get; set; }
    public string? ServerIdTag { get; set; }
    public double? LoadedKw { get; set; }
    public virtual ICollection<VmMeterValue> VmMeterValue { get; set; }
}
```

### VmMeterValue.cs

```csharp
[Table("VmMeterValue", Schema = "RotaWatt")]
public class VmMeterValue : BaseEntity
{
    public string GuiId { get; set; }
    public string MessageUniqueId { get; set; }
    public double ChargeAmountOfEnergy { get; set; }
    public double TotalChargePower { get; set; }
    public DateTime Timestamp { get; set; }
    public int ConnectorNo { get; set; }
    public long TransactionId { get; set; }
    public long? VmStartTransactionId { get; set; }
    public VmStartTransaction? VmStartTransaction { get; set; }
    public double? StateOfCharge { get; set; }
    public SampledValueContextEnum? SampledValueContext { get; set; }
    public virtual ICollection<VmMeterSampledValue> VmMeterSampledValue { get; set; }
}
```

### VmCommandMessage.cs

```csharp
[Table("VmCommandMessage", Schema = "RotaWatt")]
public class VmCommandMessage : BaseEntity
{
    public string GuiId { get; set; }
    public string Data { get; set; }
    public string Identifier { get; set; }
    public string MessageUniqueId { get; set; }
    public string SenderIpAddress { get; set; }
    public VmCommandMessageTypeEnum SenderType { get; set; }
    public VmCommandMessageTypeEnum ReceivedType { get; set; }
    public DateTime CreatedDate { get; set; }
    public long? VmDeviceConnectionId { get; set; }
    public VmDeviceConnection? VmDeviceConnection { get; set; }
    public string? MessageDescription { get; set; }
    public VmCommandMessageTopicEnum? Topic { get; set; }
}
```

### VmPanelAdmin.cs

```csharp
[Table("VmPanelAdmin", Schema = "RotaWatt")]
public class VmPanelAdmin : BaseEntity
{
    [StringLength(36)] public string AdminUserGuid { get; set; }
    [StringLength(24)] public string UserName { get; set; }
    [StringLength(36)] public string Name { get; set; }
    [StringLength(36)] public string Surname { get; set; }
    [StringLength(36)] public string Mail { get; set; }
    public bool IsActive { get; set; }
    [StringLength(14)] public string Phone { get; set; }
    public AdminManagmentTypeEnum AdminManagmentType { get; set; }
    public string Md5Password { get; set; }
    public long? VmCpoId { get; set; }
    public VmCpo? VmCpo { get; set; }
}
```

---

## 8. VmDbContext (tam kod)

```csharp
public class VmDbContext : UnitOfWork
{
    public VmDbContext(DbContextOptions<VmDbContext> dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new VmCommandMessageFluent());
        modelBuilder.ApplyConfiguration(new VmCpoConnectionFluent());
        modelBuilder.ApplyConfiguration(new VmDeviceConnectionFluent());
        modelBuilder.ApplyConfiguration(new VmServerConnectionFluent());
        modelBuilder.ApplyConfiguration(new VmConnectorFluent());
        modelBuilder.ApplyConfiguration(new VmEvccIdFluent());
        modelBuilder.ApplyConfiguration(new VmConnectorPowerTypeSeed());
    }

    public DbSet<VmCpoConnection> VmCpoConnection { get; set; }
    public DbSet<VmDeviceConnection> VmDeviceConnection { get; set; }
    public DbSet<VmServerConnection> VmServerConnection { get; set; }
    public DbSet<VmConnector> VmConnector { get; set; }
    public DbSet<VmCpo> VmCpo { get; set; }
    public DbSet<VmDevice> VmDevice { get; set; }
    public DbSet<VmStation> VmStation { get; set; }
    public DbSet<VmIntegrationConfig> VmIntegrationConfig { get; set; }
    public DbSet<VmParameter> VmParameter { get; set; }
    public DbSet<VmCommandMessage> VmCommandMessage { get; set; }
    public DbSet<VmIdTagInfo> VmIdTagInfo { get; set; }
    public DbSet<VmMeterSampledValue> VmMeterSampledValue { get; set; }
    public DbSet<VmMeterValue> VmMeterValue { get; set; }
    public DbSet<VmStartTransaction> VmStartTransaction { get; set; }
    public DbSet<VmConnectorConnection> VmConnectorConnection { get; set; }
    public DbSet<VmPanelRootAdmin> VmPanelRootAdmin { get; set; }
    public DbSet<VmPanelAdmin> VmPanelAdmin { get; set; }
    public DbSet<PanelLoginSession> PanelLoginSession { get; set; }
    public DbSet<PanelLoginAttention> PanelLoginAttention { get; set; }
    public DbSet<PanelDevice> PanelDevice { get; set; }
    public DbSet<VmEvccId> VmEvccId { get; set; }
    public DbSet<VmConnectorPowerType> VmConnectorPowerType { get; set; }
}
```

---

## 9. VmPanel.Api Startup (tam kod)

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
        base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        base.RegistirationPrefixList = base.Configuration
            .GetSection("StartupConfigs:RegistirationPrefixList").Get<string[]>();
        ConfigureApiOptions();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
        dbcontextOptions.Add(GetDbContextOption(
            FrameworkCore.Enums.UsingDbType.POSTGRESQL,
            GetAppSettingValue("ConnectionStrings:RotaWattPostgreConnectionString"),
            GetAppSettingValue("StartupConfigs:MigrationAssembly")));
        services.AddRotaWattDbService<VmDbContext>(dbcontextOptions);
        services.AddRotaWattApiService(Configuration, WebHostEnvironment,
            GetAppSettingValue("StartupConfigs:Policy"),
            GetAppSettingValue("StartupConfigs:ApiUrl"));
        services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
        services.AddFilters();
        services.AddFrameworkServices();
        services.AddWebJwtTocken(Configuration);
        services.AddAuthorization();
        services.AddContextProvider();
        services.RegisterMasstransit();
        services.AddCors(options => {
            options.AddPolicy("AllowSpa", builder =>
                builder.WithOrigins(GetAppSettingValue("StartupConfigs:SpaOrigin"))
                    .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        });
        services.AddDistributedMemoryCache();
        services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = false;
            options.Cookie.IsEssential = true;
        });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureRepositories(ApiOptions);
        builder.ConfigureServices(ApiOptions);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowSpa");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        app.UseSignalRBuilder(provider, ApiOptions);
    }
}
```

## 10. Vm.Api Startup (tam kod)

```csharp
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
        base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
        dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.POSTGRESQL,
            GetAppSettingValue("ConnectionStrings:RotaWattPostgreConnectionString"),
            GetAppSettingValue("StartupConfigs:MigrationAssembly")));
        services.AddRotaWattDbService<VmDbContext>(dbcontextOptions);
        services.AddRotaWattApiService(Configuration, WebHostEnvironment,
            GetAppSettingValue("StartupConfigs:Policy"),
            GetAppSettingValue("StartupConfigs:ApiUrl"));
        services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
        services.AddFilters();
        services.RegisterMasstransit();
        services.AddFrameworkServices();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureRepositories(ApiOptions);
        builder.ConfigureServices(ApiOptions);
        builder.RegisterHandlers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        ConfigureBuilderInit(app, env);
        var webSocketOptions = new WebSocketOptions()
        {
            ReceiveBufferSize = 32 * 1024,
            KeepAliveInterval = TimeSpan.FromSeconds(30)
        };
        app.UseWebSockets(webSocketOptions);
        app.UseSwaggerBuilder(provider, ApiOptions);
        app.UseSignalRBuilder(provider, ApiOptions);
        app.UseErrorBuilder(provider, ApiOptions);
    }
}
```
