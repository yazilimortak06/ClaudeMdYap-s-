# sarj_vm_backend_dotnet — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
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

Bu proje, daha büyük `sarj_backend_dotnet` platformunun cihaz yönetimi alt sistemini bağımsız bir backend olarak implemente ediyor. Ocelot API Gateway kullanımı, 4 servisli mimaride routing ve auth yönetimini merkezi hale getiriyor. 4 ortam desteği ile deployment pipeline'ı olgunlaşmış durumda.
