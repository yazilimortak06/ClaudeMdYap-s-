# sarj_vm_backend_dotnet — Cikarilan Kurallar
Orijinal: `E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop`

Bu dosya gercek kod okunarak cikarilan somut kurallari icerir.
Ocelot gateway'li ve WebSocket/OCPP tabanli .NET microservice backend gelistirirken referans.

**Temel kural seti icin bkz:** `sarj_backend_dotnet/rules.md`

---

## 1. Ocelot API Gateway Yapısı

**Kural:** API Gateway için Ocelot kullan. `GateWay.Api` projesi sadece `ocelot.json` ve `Startup.cs` içersin — business logic içermemeli.

**Kural:** Ocelot konfigürasyonu için ortam bazlı JSON dosyaları oluştur:
```
GateWay.Api/
├── ocelot.json              (base)
├── ocelot.Development.json
├── ocelot.Staging.json
└── ocelot.Production.json
```

**Kural:** `Program.cs`'de environment'a göre doğru dosya yüklensin:
```csharp
.ConfigureAppConfiguration((hostingContext, config) => {
    config
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                     optional: true, reloadOnChange: true);
})
```

**Kural:** Ocelot route şablonu tutarlı namespace kullanmalı:
```json
"UpstreamPathTemplate": "/vm/{everything}",      // Vm.Api için
"UpstreamPathTemplate": "/vmpanel/{everything}",  // VmPanel.Api için
"UpstreamPathTemplate": "/log/{everything}"       // VmLog.Api için
```

---

## 2. Ocelot Authentication Entegrasyonu

**Kural:** Authentication Gateway'de tek noktada yapılsın. Downstream servisler token doğrulama yapmayabilir — ama yapmaları daha güvenlidir (defense in depth).

**Kural:** Ocelot JWT konfigürasyonu:
```json
"AuthenticationOptions": {
  "AuthenticationProviderKey": "Bearer",
  "AllowedScopes": []
}
```

```csharp
// Startup.cs — Gateway
services.AddAuthentication()
    .AddJwtBearer("Bearer", options => {
        options.Authority = Configuration["TokenService:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false
        };
    });
```

**Kural:** Public endpoint'ler (login, token alma) Ocelot'ta `AuthenticationOptions` içermemeli.

---

## 3. Ocelot Rate Limiting

**Kural:** Rate limiting Gateway'de Ocelot üzerinden yapılsın. Her route için ayrı kota tanımlanabilir:

```json
"RateLimitOptions": {
  "ClientWhitelist": [],
  "EnableRateLimiting": true,
  "Period": "1s",
  "PeriodTimespan": 1,
  "Limit": 10
}
```

**Kural:** Rate limit ayarları `ocelot.json`'da tutulsun, ortama göre override edilebilsin.

---

## 4. 4 Ortam Docker-Compose Yapısı

**Kural:** Ortam sayısı 3+'sa docker-compose override stratejisi kullan:
```
docker-compose.yml             (base — servis tanımları)
docker-compose.override.yml   (local dev — port expose, volume mount)
docker-compose.test.yml       (test ortamı overrides)
docker-compose.prod.yml       (production overrides — resource limits, replica)
```

**Kural:** Base `docker-compose.yml` image adlarını ve servis isimlerini içersin. Override dosyaları port, environment variable ve volume gibi ortama özgü değerleri tanımlasın.

**Kural:** Production override dosyası şunları içersin:
```yaml
services:
  vm.api:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 256M
      restart_policy:
        condition: on-failure
        max_attempts: 3
```

**Kural:** Ortam seçimi:
```bash
# Local
docker-compose -f docker-compose.yml -f docker-compose.override.yml up

# Test
docker-compose -f docker-compose.yml -f docker-compose.test.yml up

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up
```

---

## 5. OCPP Protokol Implementasyonu

**Kural:** OCPP mesajları `Vm.Application` servisinde işlensin. Her OCPP mesaj tipi için ayrı handler sınıfı oluştur:

```
Vm.Application/
└── Ocpp/
    ├── Handlers/
    │   ├── BootNotificationHandler.cs
    │   ├── HeartbeatHandler.cs
    │   ├── StartTransactionHandler.cs
    │   ├── StopTransactionHandler.cs
    │   └── StatusNotificationHandler.cs
    └── Interfaces/
        └── IOcppMessageHandler.cs
```

**Kural:** OCPP handler'lar bir `IOcppMessageHandler` interface'i implement etsin. Factory pattern ile mesaj tipine göre doğru handler seçilsin.

**Kural:** OCPP WebSocket bağlantısı SignalR hub veya raw WebSocket olabilir. Bağlantı state'i (hangi cihaz bağlı, son heartbeat zamanı) in-memory cache veya Redis'te tutulsun.

---

## 6. Küçük Scope Microservice Yönetimi

**Kural:** 4-5 servisli bir sistemde aşırı karmaşıklıktan kaçın. Her servis için:
- Ayrı repository gerektirir mi? Belki VmPanel ve Vm aynı DB'yi paylaşabilir.
- Ayrı Persistence projesi gerekli mi? Belki shared persistence kullanılabilir.

**Kural:** Servis sayısı az olsa bile her servis kendi build ve deploy pipeline'ına sahip olsun. Tek monolith'e dönüşümü önler.

**Kural:** Tüm servisler bir `docker-compose.yml`'de yönetilsin. Kubernetes'e geçiş için `docker-compose.prod.yml` referans alınabilir.

---

## 7. Log Servisi Minimal Tasarım

**Kural:** `VmLog.Application` RabbitMQ consumer olarak şu iki event tipini dinlesin:
- `RequestResponseLogEvent` — her HTTP isteği/yanıtı
- `ExceptionLogEvent` — yakalanmış exception'lar

**Kural:** Log servisi senkron HTTP endpoint sunmayabilir. Tamamen async consumer olarak çalışabilir. Docker'da port expose gerekmeyebilir.

**Kural:** Log servisi için health check, RabbitMQ bağlantısını kontrol etsin — HTTP health değil, consumer health.

---

## 8. Gateway Servisi Özel Durumlar

**Kural:** Ocelot Gateway'den geçmeyen endpoint'ler olmamalı. Vm.Api ve VmPanel.Api dış dünyaya port expose etmemeli — sadece Gateway'in Docker network'üne açık olmalı.

**Kural:** Docker Compose'da internal network tanimla:
```yaml
networks:
  backend:
    driver: bridge

services:
  vm.api:
    networks:
      - backend
    # ports: expose edilmemeli

  gateway.api:
    networks:
      - backend
    ports:
      - "5000:80"   # Sadece gateway disariya acik
```

---

## 9. Gercek Koddan Cikarilan Ek Kurallar

### WebSocket + OCPP proxy pattern

```csharp
// Controller WebSocket'i kabul etmez, servise devreder
[Route("[controller]/{Identifier}")]
public async Task ConnectionDevice(string Identifier)
{
    await _vmConnectionService.ConnectionDevice(Identifier);
    // WebSocket kapaninana kadar bu metot bloklar
    // HTTP response gönderilmez — bağlantı WebSocket'e upgrade edilir
}

// Servis icerisinde WebSocket kabul
if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.IsWebSocketRequest)
{
    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync(subProtocol))
    {
        // Ana döngü — WebSocket acik oldukca calisir
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
            if (result.MessageType != WebSocketMessageType.Close)
            {
                // Mesaj isle
            }
        }
    }
}
```

### ConcurrentDictionary session yönetimi

```csharp
// Tüm aktif cihaz baglantilari thread-safe dictionary'de
public static ConcurrentDictionary<string, VmConnectionSessionDto> _vmSessionStatusDict
    = new ConcurrentDictionary<string, VmConnectionSessionDto>();

// Yeni baglanti ekle (var olan baglanti kapatilir)
bool statusSuccess = await UpdateVmDictionaryForDevice(vmConnectionSession, identifier);
// identifier = cihaz kimlik numarasi (OCPP ChargeBox ID)
```

### IServiceProvider ile scoped servis

```csharp
// WebSocket uzun sure acik - DI scope'u kendi yönetmelisin
// Scoped servisler using (CreateScope()) ile alinir

VmParameter vmParameter = null;
using (var scope = _services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IVmParameterRepository>();
    vmParameter = await repo.GetVmParameter(...).FirstOrDefaultAsync();
}
// scope dispose oldugunda repository ve DbContext kapanir

// YANLIS: Singleton veya uzun ömürlü serviste scoped servis direkt inject etme
// DOGRU: IServiceProvider inject et, kullanim aninda scope ac
```

### Atomic first connection pattern

```csharp
int firstConnection = 1; // 1=true, 0=false

while (webSocket.State == WebSocketState.Open)
{
    // ... mesaj isle ...

    // Sadece ilk mesajda DB update yap - thread-safe
    if (Interlocked.CompareExchange(ref firstConnection, 0, 1) == 1)
    {
        await UpdateDeviceConnectionDb(vmConnectionSession, true, ChargeDeviceInstantStateEnum.AVAILABLE);
    }
    // Sonraki iterasyonlarda CompareExchange 0 dönüyor, update atlanıyor
}
```

### OCPP mesaj dispatch pattern

```csharp
// Action string'e göre dispatch — switch yerine if/else zinciri
private async Task ProcessDeviceMessages(VmConnectionSessionDto session,
    Uri uriServer, Uri uriCpo, string messageType, string messageId, JToken payload, string action)
{
    // action = "MeterValues", "Heartbeat", "Authorize" vb.
    if (action == OcppFromChargePointActionTypeEnum.METER_VALUES.ToDescriptionString())
    {
        await SendMeterValuesResponseToDevice(session, messageId, payload);
        await SendMeterValuesToCpoAndServer(session, uriServer, uriCpo, ...);
    }
    else if (action == OcppFromChargePointActionTypeEnum.AUTHORIZE.ToDescriptionString())
    {
        // Response bekle, hemen cevap verme
        await SendAuthorizeToCpoAndServer(session, uriServer, uriCpo, ...);
        // CPO/Server'dan gelen yanit sonra islenecek
    }
    else if (action == OcppFromChargePointActionTypeEnum.START_TRANSACTION.ToDescriptionString())
    {
        // TransactionTarget'a göre farkli davranis
        if (session.TransactionTarget == TransactionTargetEnum.VM_ONLY)
            await SendStartTransactionResponseToDevice(session, messageId, payload);
        else
            await HandleStartTransactionWithExternalWait(session, ...);
    }
}
```

### Heartbeat izleme pattern

```csharp
// Her gelen mesaj = cihaz hayatta
vmConnectionSession.LastHeartbeatTime = DateTime.UtcNow;
vmConnectionSession.MissedHeartbeatCount = 0;

// Ayri monitor gorevi arka planda heartbeat kontrol eder
await EnsureHeartbeatMonitorStarted(vmConnectionSession);
// Monitor: beklenen interval'da heartbeat gelmediyse MissedHeartbeatCount arttirir
// Belirli esik asildiysa baglanti kopuk kabul edilir
```

### Ocelot WebSocket route

```json
// WebSocket downstream = "ws" scheme ile isaretlenir
{
  "DownstreamScheme": "ws",           // WebSocket proxy
  "UpstreamPathTemplate": "/vm/{url}" // OCPP cihazlar buradan baglanir
}

// HTTP downstream normal "http"
{
  "DownstreamScheme": "http",
  "UpstreamPathTemplate": "/web/{url}"  // VmPanel REST API
}
```

**Kural**: Ocelot'ta WebSocket ve HTTP route'lari farkli scheme ile ayirt edilmeli. `"DownstreamScheme": "ws"` olmadan WebSocket upgrade calismaz.
