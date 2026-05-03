# sarj_pro_backend_dotnet — Çıkarılan Kurallar

Bu dosya, `sarj_pro_backend_dotnet` (SarjAllPro) projesine özgü veya `sarj_backend_dotnet`'ten farklılaşan kuralları içerir.

**Temel kural seti için bkz:** `sarj_backend_dotnet/rules.md` — tüm kurallar bu proje için de geçerlidir.

---

## sarj_backend_dotnet Kurallarına Ekler ve İstisnalar

### 1. Fork Projelerde Konfigürasyon Yönetimi

**Kural:** Bir projeyi fork ederken değiştirilmesi gereken tüm konfigürasyon noktalarını belgelenmiş bir liste halinde tut:

| Konfigürasyon | Değer |
|---|---|
| Docker image prefix | `sarjpro/` |
| Connection string key | Proje özelinde belirle |
| RabbitMQ credentials | Farklı kullanıcı/şifre |
| Aktif ödeme sağlayıcısı | MOKA |
| docker-compose servis adları | sarjpro prefix'li |

**Kural:** Fork proje deployment öncesinde tüm bu değerlerin doğrulandığı bir checklist oluştur. Kaynak proje değerleriyle karışma riskini minimize et.

---

### 2. Ödeme Sağlayıcısı Seçimi

**Kural:** Aktif ödeme sağlayıcısı appsettings'te `Selected: true` ile işaretlensin. Kod değişikliği olmadan provider değiştirilebilsin:

```json
"PaymentIntegrations": [
  { "Name": "IYZICO", "Selected": false },
  { "Name": "PARAM",  "Selected": false },
  { "Name": "MOKA",   "Selected": true  }
]
```

**Kural:** Payment service factory, `Selected: true` olan provider'ı otomatik olarak seçsin. Birden fazla `Selected: true` varsa ilki alınsın veya exception fırlatılsın.

**Kural:** Her ödeme sağlayıcısı `IPaymentService` interface'ini implement etsin. Provider değişikliği sıfır kod değişikliği gerektirsin.

---

### 3. Servis Sayısı Azaltma Kararları

**Kural:** Bir servisi fork'ta çıkarırken şunları yap:
1. Servisi docker-compose'dan kaldır
2. İlgili Application ve Persistence projelerini solution'dan kaldır (veya disabled tut)
3. Diğer servislerin bu servise olan Refit bağımlılıklarını kaldır veya null-guard ekle
4. `ApiName` enum'ından çıkarılacak servisi sil (InnerRequestAttribute etkilenebilir)

**Kural:** Bir servis çıkarılırken o servise ait RabbitMQ consumer/publisher kodunu da temizle. Ölü consumer'lar bağlantı kaynakları tüketir.

---

### 4. RabbitMQ Ortam İzolasyonu

**Kural:** Farklı projeler (sarj_backend vs sarj_pro_backend) aynı RabbitMQ sunucusunu paylaşıyorsa mutlaka ayrı vhost kullan:
```
vhost: /rotawatt   → sarj_backend_dotnet
vhost: /sarjpro    → sarj_pro_backend_dotnet
```

**Kural:** vhost ayırımı yapılamazsa farklı RabbitMQ sunucusu kullan. Kuyruk isimleri çakışırsa cross-project mesaj karışması yaşanır.

**Kural:** docker-compose'daki RabbitMQ credentials ortam değişkenleri üzerinden sağlan. Her ortam (dev, test, prod) farklı credentials kullanmalı.

---

### 5. Docker Image Yönetimi

**Kural:** Fork projelerin docker image'ları farklı bir registry namespace altında tutulsun:
```
sarj_backend:     registry.domain.com/rotawatt/notification.api:TAG
sarj_pro_backend: registry.domain.com/sarjpro/notification.api:TAG
```

**Kural:** Aynı servis adı (notification.api) iki farklı projede olabilir. Image namespace'i proje adıyla ayrıştır. Docker Hub'da karışma yaşanmasın.

---

### 6. FrameworkCore Versiyonlama

**Kural:** FrameworkCore bir NuGet paketi olarak yayımlanıyorsa semantic versioning kullan. Her iki proje (sarj_backend + sarj_pro_backend) bağımlı oldukları versiyonu sabitlemeli.

**Kural:** FrameworkCore'da breaking change yapılacaksa:
1. Major version artır
2. Her iki projeyi ayrı ayrı test et
3. Projeleri sırayla yükselt, aynı anda değil

**Kural:** FrameworkCore kaynak kodunun fork'u olması durumunda bir `FRAMEWORK_VERSION.md` dosyası oluştur ve kaynak ile son sync tarihini belirt.
