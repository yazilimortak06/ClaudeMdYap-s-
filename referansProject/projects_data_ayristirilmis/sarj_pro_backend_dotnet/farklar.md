# sarj_pro_backend_dotnet — sarj_backend_dotnet'ten Farklar

Bu dosya, `sarj_pro_backend_dotnet` (SarjAllPro) ile kaynak proje `sarj_backend_dotnet` (RotaWattBackEnd) arasındaki tüm bilinen farklılıkları belgeler.

---

## 1. Ödeme Sağlayıcısı

| Özellik | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Mevcut sağlayıcılar | IYZICO, PARAM, MOKA | IYZICO, PARAM, MOKA |
| **Aktif sağlayıcı** | IYZICO veya PARAM | **MOKA** |

```json
// sarj_backend_dotnet appsettings
"PaymentIntegrations": [
  { "Name": "IYZICO", "Selected": true },
  { "Name": "PARAM",  "Selected": false },
  { "Name": "MOKA",   "Selected": false }
]

// sarj_pro_backend_dotnet appsettings
"PaymentIntegrations": [
  { "Name": "IYZICO", "Selected": false },
  { "Name": "PARAM",  "Selected": false },
  { "Name": "MOKA",   "Selected": true }
]
```

---

## 2. Docker Image İsimleri

| Servis | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| notification | `rotawatt/notification.api` | `sarjpro/notification.api` |
| gateway | `rotawatt/gateway.api` | `sarjpro/gateway.api` |
| bank | `rotawatt/bank.api` | `sarjpro/bank.api` |
| station | `rotawatt/station.api` | `sarjpro/station.api` |
| ... | `rotawatt/[servis]` | `sarjpro/[servis]` |

---

## 3. Connection String Key Adı

| | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Key adı | `MainConnectionString` | Farklı key adı (tespit edilmeli) |

appsettings'teki `ConnectionStrings` bölümündeki key adı değişmiş olabilir. Docker-compose environment değişkeni de buna uygun güncellenmelidir.

```yaml
# sarj_backend_dotnet
- ConnectionStrings__MainConnectionString=[CONNECTION_STRING]

# sarj_pro_backend_dotnet (muhtemel)
- ConnectionStrings__SarjProConnectionString=[CONNECTION_STRING]
# veya aynı key adı ile farklı değer
```

---

## 4. RabbitMQ Credentials

| | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Username | rotawatt_user | sarjpro_user (farklı) |
| Password | [şifre_1] | [şifre_2] (farklı) |
| VirtualHost | /rotawatt veya / | /sarjpro veya / |

İki projenin aynı RabbitMQ sunucusunu paylaşması durumunda VirtualHost ayrımı şarttır. Aksi takdirde kuyruk isimleri çakışır.

---

## 5. Servis Sayısı

| | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Toplam API | 20+ | 13+ |

**Çıkarılan veya eksik servisler:**
- `GoogleService.Api` — coğrafi servisler azaltılmış veya birleştirilmiş
- `Vm.Api` — büyük ihtimalle `sarj_vm_backend_dotnet`'e taşınmış
- `FirmIntegration` alt sistemi — 8 proje azaltılmış

---

## 6. Proje Adı ve Namespace

| | sarj_backend_dotnet | sarj_pro_backend_dotnet |
|---|---|---|
| Root namespace | `RotaWatt.[ServiceName]` | `SarjAllPro.[ServiceName]` veya `SarjPro.[ServiceName]` |
| Solution adı | RotaWattBackEnd.sln | SarjAllPro.sln |

---

## 7. docker-compose Servis Adları

docker-compose'daki servis isimleri prefix'le ayrışır:

```yaml
# sarj_backend_dotnet
services:
  notification.api:
  bank.api:

# sarj_pro_backend_dotnet
services:
  sarjpro.notification.api:
  sarjpro.bank.api:
  # veya
  notification.api:  (aynı kalabilir, image tag farklı)
```

---

## 8. Ortak Kalan Özellikler

Aşağıdakiler **değişmemiştir** — iki proje tamamen aynıdır:

- FrameworkCore yapısı ve içeriği
- BaseStartup pattern
- Controller pattern (InnerRequestAttribute, Result<T>)
- Persistence pattern (Repository + UoW)
- MassTransit + RabbitMQ entegrasyon yapısı
- appsettings key yapısı (ConnectionStrings, RabbitMqSettings, StartupConfigs)
- Dockerfile yapısı (multi-stage build)
- EF Core + NetTopologySuite kullanımı
- FluentValidation, AutoMapper, Autofac kullanımı

---

## 9. Deployment Checklist

`sarj_pro_backend_dotnet` deploy edilmeden önce kontrol edilmesi gereken noktalar:

- [ ] Tüm image prefix'lerinin `sarjpro/` olduğu doğrulandı
- [ ] MOKA ödeme sağlayıcısının aktif olduğu doğrulandı
- [ ] Connection string key adı doğrulandı
- [ ] RabbitMQ credentials güncellendi
- [ ] Docker-compose servis adları çakışma yok
- [ ] Port numaraları `sarj_backend_dotnet` ile çakışmıyor
- [ ] Ortam değişkenleri production değerleri ile dolduruldu
