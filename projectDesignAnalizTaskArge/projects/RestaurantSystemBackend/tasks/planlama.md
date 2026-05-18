# RestaurantSystemBackend — Planlama

Tartışmalar, kesinleşmemiş görevler, fikirler.

---

### Tartışma: Multi-tenancy Yaklaşımı

**Soru:** Birden fazla restoranı tek sistemde nasıl yönetiriz?

**Seçenek A: Shared DB + tenant_id filter**
- Tüm restoranlar aynı veritabanını paylaşır
- Her tabloda `RestaurantId` (tenant_id) kolonu bulunur
- EF Core Global Query Filter ile otomatik filtreleme
- Artıları: Kolay deployment, düşük altyapı maliyeti, bakım kolaylığı
- Eksileri: Veri izolasyonu zayıf, büyük veri setlerinde performans sorunları, bir restoranın yanlış sorgusu diğerlerini etkileyebilir

**Seçenek B: Ayrı DB per tenant**
- Her restoranın kendi veritabanı
- Artıları: Tam veri izolasyonu, bağımsız ölçeklendirme, güvenlik
- Eksileri: Yüksek altyapı maliyeti, migration yönetimi karmaşık, deployment zorlukları

**Karar: Seçenek A — EF Core Global Query Filter ile**
- `RestaurantId` her entity'de zorunlu
- `ApplicationDbContext`'te `HasQueryFilter(e => e.RestaurantId == _currentRestaurantId)` uygulanacak
- `ICurrentRestaurantService` ile aktif restoran context'ten çekilecek
- İleride Seçenek B'ye geçiş için servis katmanı soyutlaması korunacak

---

### Tartışma: Sipariş Bildirimleri

**Soru:** Panel'de yeni sipariş geldiğinde nasıl bildirim yapılacak?

**SignalR:**
- Artıları: Gerçek zamanlı, anında güncelleme, ses bildirimi entegrasyonu kolay
- Eksileri: Sunucu tarafında bağlantı yönetimi, load balancer durumunda sticky session veya Redis backplane gerekir

**Polling (HTTP):**
- Artıları: Basit implementasyon, sunucu bağlantısı yok
- Eksileri: Gecikme (interval kadar), gereksiz HTTP trafiği, sunucu yükü

**Karar:**
- **Panel (Angular Admin):** SignalR — `OrderHub` üzerinden sipariş güncelleme olayları yayınlanacak, ses bildirimi entegre edilecek
- **QR App (Müşteri):** Polling (5 saniye interval) veya SignalR — mobil pil tasarrufu için polling tercih edilebilir, kesin karar QR planlama.md'de ele alınacak

---

### Fikir: Mutfak Ekranı (Kitchen Display Screen)

**Açıklama:** Mutfakta tablet üzerinde çalışacak ayrı bir "Kitchen Display" ekranı. Siparişleri `Accepted` → `Preparing` → `Ready` olarak güncelleyebilir.

**Durum:** Henüz kesinleşmedi

**Değerlendirme:**
- Backend endpoint'i zaten hazır olacak (`OrderStatus` geçiş servisi)
- Yeni bir Angular projesi olarak eklenebilir (RestaurantSystemKitchen)
- Kimlik doğrulama: `Kitchen` rolü ile sınırlı erişim
- Sadece kendi restoranının siparişlerini görebilir
- SignalR ile Panel'den bağımsız real-time güncelleme alır

**Aksiyon:** Temel geliştirme tamamlandıktan sonra değerlendirilecek

---

### Fikir: Masa QR Yenileme

**Soru:** Masa QR kodu değiştirildiğinde eski QR'lar geçersiz olmalı mı?

**Senaryo:** Müşteri masadan ayrıldıktan sonra QR kodunu fotoğrafladıysa ve daha sonra tekrar sipariş vermeye çalışırsa ne olacak?

**Token Rotasyon Stratejisi Seçenekleri:**
1. **UUID token:** QR yenilenince yeni UUID üretilir, eski token geçersiz olur — basit ama müşteri sayfası anında hata verir
2. **Süreli token (JWT):** Token'a TTL (örn. 24 saat) eklenir — esneklik sağlar ama masanın kapandığını anlayamaz
3. **Oturum bazlı:** Masa her "açıldığında" yeni oturum ID'si üretilir, QR bu ID'yi içerir — granüler kontrol

**Durum:** Token rotasyon stratejisi tartışılacak, `Table` entity tasarlanırken kesinleştirilecek
