# SarjAllPro vs VmBackend — Farklar ve Analiz

## Kaynak Projeler
- **VmBackend**: `E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop`  
- **SarjAllPro**: `E:\Projeler\Backend\SarjAllPro`

---

## 1. MİMARİ FARK — OCPP Bağlantı Modeli

### VmBackend (rotawatt)
- **VM (Virtual Middleware) mimarisi**: Cihaz ↔ VM ↔ CPO ve Cihaz ↔ VM ↔ Server (üçgen)
- Her cihaz için 3 ayrı WebSocket tutulur: Device, CPO, Server
- `VmConnectionService` 17 partial dosyadan oluşur (çok büyük, karmaşık)
- `ConcurrentDictionary<string, VmConnectionSessionDto>` — thread-safe, kilit yok
- SSL bypass fallback mekanizması var
- Heartbeat monitoring, orphan transaction reconciliation, meter values drain
- Retry exponential backoff, connection health monitor

### SarjAllPro (pixdinn)
- **Doğrudan OCPP sunucusu**: Cihaz ↔ SarjAllPro sunucusu (ikili)
- Tek WebSocket (device bağlantısı), CPO/Server bağlantısı YOK
- `Ocpp16ConnectionService` tek dosya (~700 satır)
- `Dictionary<long, DeviceSessionStatusDto>` — lock() ile yönetilen (thread-safe DEĞİL, basit)
- Reconnect, SSL bypass, heartbeat monitor YOK (basit implementasyon)
- `OcppDeviceTypeEnum.CIRCONTROL` — sabit cihaz tipi (VM'de dinamik)

---

## 2. SERVİS SAYISI FARKI

| Özellik | VmBackend | SarjAllPro |
|---------|-----------|------------|
| Docker servisleri | 5 | 16 |
| OCPP endpoint | `/Mocpp/{id}` | `/OCPP16/Connection/{id}` |
| Image prefix | rotawatt/ | pixdinn/ |
| .NET versiyonu | .NET 6+ | .NET 5.0 (FirmIntegration .NET 6) |
| API Gateway | Ocelot | Ocelot |

### SarjAllPro'ya Özgü Servisler (VmBackend'de YOK):
- `bank.api` — Ödeme servisi (Moka/banka entegrasyonu altyapısı)
- `integration.api` — Harici servis entegrasyonu
- `notification.api` — SignalR gerçek zamanlı bildirimler
- `mailsms.api` — Mail/SMS servisi
- `token.api` — JWT token yönetimi (ayrı microservis)
- `file.api` — Dosya yükleme
- `googleservice.api` — Google Maps/Geocoding
- `workerservice` + `workerservice.api` — Background worker'lar
- `station.api` — İstasyon yönetimi
- `mobil.api` / `SarjAllMobil.Api` — Mobil uygulama API

---

## 3. ÖDEME (BANK/MOKA) ENTEGRASYONu

### SarjAllPro Bank.Application / Bank.Persistence:
- **Altyapı kurulmuş ama implementasyon boş**
- `PaymentDbContext` tanımlanmış, Entity Fluent ve Repositories boş
- `Bank.Persistence` projesi **Refit stubs** içeriyor → HttpClient tabanlı ödeme API'ye bağlanmak için
- `Integration.Persistence` de Refit stubs içeriyor → harici servis entegrasyonu planlandı
- Muhtemelen **Moka Ödeme** veya benzeri ödeme gateway'i için HTTP client interface'leri
  yazılacak (kaynak kodda henüz tanımlanmamış)
- `ChargeController.GetPayment()` endpoint mevcut → `IChargeService.GetPayment()` çağırıyor

### VmBackend (rotawatt):
- Ödeme entegrasyonu YOK — sadece OCPP VM işlevi var

---

## 4. OCPP İMPLEMENTASYON FARKLARI

### VmBackend:
- OCPP mesajları hem CPO'ya hem Server'a iletilir (bridging)
- StartTransaction → CPO/Server'dan TransactionId beklenir (STARTTX_WAIT)
- Authorize → iki kaynaktan yanıt analizi (AUTOCHARGE FIX)
- MeterValues → reconnect sırasında biriken değerler drain edilir
- TriggerMessage rate limiting (10s / 120s BootNotification)
- Connection state machine: TRY_ESTAB ↔ CONNECTED ↔ DISCONNECTED

### SarjAllPro:
- OCPP mesajları doğrudan işlenir (bridge yok)
- StartTransaction → doğrudan DB kayıt + yanıt
- Heartbeat, BootNotification, StatusNotification, MeterValues ayrı service sınıflarında
  (Ocpp16BootNotification/, Ocpp16StartTransaction/ vb. klasörler)
- TriggerMessage yönetimi ayrı `OcppTriggerMessageManagmentController`
- RemoteStartTransaction/RemoteStopTransaction ayrı `RemoteTransactionController`

---

## 5. SHARED DOMAIN FARKLARI

### VmBackend Entities:
- `VmCpo`, `VmStation`, `VmDevice`, `VmConnector` — OCPP cihaz hiyerarşisi
- `VmStartTransaction`, `VmMeterValue`, `VmMeterSampledValue` — transaction verisi
- `VmCommandMessage` — OCPP mesaj logu
- `VmPanelAdmin`, `VmPanelRootAdmin` — panel kullanıcıları
- Tümü `[Table("...", Schema = "RotaWatt")]`

### SarjAllPro Entities:
- `PanelAdmin`, `PanelRootAdmin`, `PanelAdminType`, `AdminUser`, `AdminUserType` — daha detaylı yetkilendirme
- `Policy` — yetkilendirme politikaları
- `Parameter`, `ParameterGroup`, `ParameterValue` — dinamik konfigürasyon
- `Country`, `City`, `Town` — bölge verileri
- `GeneralTask` — worker service görevleri
- `ContentLanguage` — çoklu dil desteği
- OcppEntities ayrı assembly'de (`Ocpp.Persistence` projesinde)

---

## 6. LOG MİMARİSİ FARKI

### VmBackend:
- `VmLog.Api` + `VmLog.Application` + `VmLog.Persistence` — ayrı log microservisi
- `RequestResponseLogInfoConsumer` — RabbitMQ consumer (hem reqres hem exception)
- Log DB'ye direkt write (VM kendi log DB'sine yazar)

### SarjAllPro:
- `Log.Api` + `Log.Application` + `Log.Persistence` — benzer mimari
- Ayrıca `MobilException` entity'si var (mobil uygulama hataları için)
- `WorkerServiceProcessInfo` — worker service izleme logu

---

## 7. DİĞER NOTLAR

- SarjAllPro **FirmIntegration** alt projesi (.NET 6) — tamamen iskelet, Class1.cs boş
- Her iki proje de `FrameworkCore` ortak kütüphanesini kullanıyor
- SarjAllPro'da `TcDogrulama` Connected Service (kimlik doğrulama web servisi entegrasyonu)
- VmBackend'de `VmIntegrationConfig` entity'si var → CPO bazlı entegrasyon konfigürasyonu
