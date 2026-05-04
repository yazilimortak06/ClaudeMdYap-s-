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
