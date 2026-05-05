# perde_iot_backend — Service/ServiceInterface Pattern

## Genel Açıklama

`perde_iot_backend`, Service/ServiceInterface ayrımını uygular:
- `ServiceInterfaces/` klasörü: Soyut tanımlar (interface'ler)
- `Services/` klasörü: Concrete implementasyonlar
- Autofac, interface'i gördüğünde otomatik olarak doğru implementasyonu inject eder

Bu ayrım sayesinde:
- Controller'lar concrete sınıftan haberdar olmaz (loose coupling)
- Unit test'lerde mock servis inject edilebilir
- IoT protokol değişirse sadece Service değişir, Controller etkilenmez

---

## Interface Tanımları (ServiceInterfaces/)

### ICurtainService

```csharp
// ServiceInterfaces/ICurtainService.cs
public interface ICurtainService
{
    /// <summary>
    /// Perdeyi belirtilen yüzdeye aç
    /// </summary>
    Task<ServiceResult> OpenToPositionAsync(string deviceId, string curtainId, int positionPercent);

    /// <summary>
    /// Perdeyi tamamen kapat
    /// </summary>
    Task<ServiceResult> CloseAsync(string deviceId, string curtainId);

    /// <summary>
    /// Perdeyi tamamen aç
    /// </summary>
    Task<ServiceResult> OpenAsync(string deviceId, string curtainId);

    /// <summary>
    /// Perde hareketini durdur
    /// </summary>
    Task<ServiceResult> StopAsync(string deviceId, string curtainId);

    /// <summary>
    /// Mevcut perde durumunu getir
    /// </summary>
    Task<CurtainStatus> GetStatusAsync(string deviceId, string curtainId);

    /// <summary>
    /// Tüm perdeler için durum listesi
    /// </summary>
    Task<IList<CurtainStatus>> GetAllStatusAsync();
}
```

### IDeviceService

```csharp
// ServiceInterfaces/IDeviceService.cs
public interface IDeviceService
{
    Task<IList<DeviceViewModel>> GetAllDevicesAsync();
    Task<DeviceViewModel> GetDeviceByIdAsync(string deviceId);
    Task<ServiceResult> AddDeviceAsync(AddDeviceDto device);
    Task<ServiceResult> UpdateDeviceAsync(UpdateDeviceDto device);
    Task<ServiceResult> DeleteDeviceAsync(string deviceId);
    Task<bool> IsDeviceOnlineAsync(string deviceId);
}
```

### IIoTConnectionService

```csharp
// ServiceInterfaces/IIoTConnectionService.cs
public interface IIoTConnectionService
{
    /// <summary>
    /// IoT cihazına komut gönder
    /// </summary>
    Task<bool> SendCommandAsync(string deviceId, IoTCommand command);

    /// <summary>
    /// Cihaz durumunu sorgula
    /// </summary>
    Task<DeviceState> QueryStateAsync(string deviceId);

    /// <summary>
    /// Cihaz bağlantı durumu
    /// </summary>
    bool IsConnected(string deviceId);

    /// <summary>
    /// Bağlantı olaylarına abone ol
    /// </summary>
    event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;
}
```

### IScenarioService

```csharp
// ServiceInterfaces/IScenarioService.cs
public interface IScenarioService
{
    Task<IList<ScenarioViewModel>> GetScenariosAsync();
    Task<ServiceResult> CreateScenarioAsync(CreateScenarioDto scenario);
    Task<ServiceResult> ExecuteScenarioAsync(int scenarioId);
    Task<ServiceResult> ScheduleScenarioAsync(int scenarioId, DateTime executeAt);
    Task<ServiceResult> CancelScheduledScenarioAsync(int scheduleId);
}
```

---

## Implementasyonlar (Services/)

### CurtainService

```csharp
// Services/CurtainService.cs
public class CurtainService : ICurtainService
{
    private readonly IIoTConnectionService _iotService;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<CurtainService> _logger;

    public CurtainService(
        IIoTConnectionService iotService,
        IDeviceRepository deviceRepository,
        ILogger<CurtainService> logger)
    {
        _iotService = iotService;
        _deviceRepository = deviceRepository;
        _logger = logger;
    }

    public async Task<ServiceResult> OpenToPositionAsync(string deviceId, string curtainId, int positionPercent)
    {
        try
        {
            if (positionPercent < 0 || positionPercent > 100)
                return ServiceResult.Failure("Konum 0-100 arasında olmalıdır.");

            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            if (device == null)
                return ServiceResult.Failure("Cihaz bulunamadı.");

            if (!_iotService.IsConnected(deviceId))
                return ServiceResult.Failure("Cihaz çevrimdışı.");

            var command = new IoTCommand
            {
                CommandType = "SET_POSITION",
                CurtainId = curtainId,
                Parameters = new Dictionary<string, object> { { "position", positionPercent } }
            };

            var success = await _iotService.SendCommandAsync(deviceId, command);
            if (!success)
                return ServiceResult.Failure("Komut gönderilemedi.");

            _logger.LogInformation("Perde {CurtainId} → %{Position}", curtainId, positionPercent);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Perde komut hatası: {DeviceId}/{CurtainId}", deviceId, curtainId);
            return ServiceResult.Failure("Beklenmeyen bir hata oluştu.");
        }
    }

    // ... diğer metodlar
}
```

---

## Autofac Kayıt Stratejisi

### Yöntem 1: Explicit Kayıt

```csharp
// Startup.cs ConfigureContainer
public override void ConfigureContainer(ContainerBuilder builder)
{
    base.ConfigureContainer(builder);

    builder.RegisterType<CurtainService>().As<ICurtainService>().InstancePerLifetimeScope();
    builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
    builder.RegisterType<ScenarioService>().As<IScenarioService>().InstancePerLifetimeScope();

    // IoT bağlantı servisi SINGLETON — bağlantı korunmalı
    builder.RegisterType<MqttIoTConnectionService>().As<IIoTConnectionService>().SingleInstance();
}
```

### Yöntem 2: Dinamik Assembly Tarama (Projede kullanılan)

```csharp
public override void ConfigureContainer(ContainerBuilder builder)
{
    base.ConfigureContainer(builder);

    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName.StartsWith("PixdinnPerdeci"))
        .ToArray();

    // "*Service" ile biten tüm sınıfları interface'lerine kaydet
    builder.RegisterAssemblyTypes(assemblies)
           .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
           .AsImplementedInterfaces()
           .InstancePerLifetimeScope();

    // IoT bağlantı servisi Singleton olarak override
    builder.RegisterType<MqttIoTConnectionService>()
           .As<IIoTConnectionService>()
           .SingleInstance();
}
```

---

## Controller'da Kullanım

```csharp
public class CurtainController : Controller
{
    private readonly ICurtainService _curtainService;   // Concrete sınıf değil, interface
    private readonly IDeviceService _deviceService;

    // Autofac constructor injection
    public CurtainController(
        ICurtainService curtainService,
        IDeviceService deviceService)
    {
        _curtainService = curtainService;
        _deviceService = deviceService;
    }

    [HttpPost]
    public async Task<IActionResult> SetPosition(string deviceId, string curtainId, int position)
    {
        var result = await _curtainService.OpenToPositionAsync(deviceId, curtainId, position);
        if (result.Success)
            return Ok(new { message = "Komut gönderildi." });
        return BadRequest(new { error = result.ErrorMessage });
    }
}
```

---

## ServiceResult Model

```csharp
// Tüm servis metodlarının dönüş tipi
public class ServiceResult
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    public string ErrorCode { get; private set; }

    public static ServiceResult Success() => new() { Success = true };
    public static ServiceResult Failure(string message, string code = null)
        => new() { Success = false, ErrorMessage = message, ErrorCode = code };
}

public class ServiceResult<T> : ServiceResult
{
    public T Data { get; private set; }

    public static ServiceResult<T> Success(T data) => new() { Success = true, Data = data };
    public new static ServiceResult<T> Failure(string message)
        => new() { Success = false, ErrorMessage = message };
}
```
