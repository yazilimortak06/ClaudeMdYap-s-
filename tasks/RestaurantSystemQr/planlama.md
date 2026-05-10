# RestaurantSystemQr — Planlama

Tartışmalar, kesinleşmemiş görevler, fikirler.

---

### Tartışma: Sipariş Verme Özelliği

**Soru:** QR uygulama sadece menü görüntüleme mi yapacak, yoksa müşteri sipariş de verebilecek mi?

**Sadece Menü Görüntüleme:**
- Artıları: Basit, hızlı geliştirme, garson siparişi alır
- Eksileri: Dijital sipariş avantajı yok, garson iş yükü azalmaz

**Menü Görüntüleme + Sipariş:**
- Artıları: Tam self-servis, garson yükü azalır, hata riski düşer, sipariş geçmişi dijital
- Eksileri: Daha fazla geliştirme süresi, ödeme entegrasyonu ileride gerekebilir

**Karar: Sipariş de verilebilir**
- Sepet (cart) özelliği: ürün ekleme/çıkarma, adet güncelleme
- Sipariş onayı sayfası: sepet özeti + masa notu alanı + "Siparişi Gönder" butonu
- Sipariş gönderildikten sonra `OrderStatus` takip sayfası açılır
- Ödeme entegrasyonu ilk versiyonda yok (garson kasada alacak)

---

### Tartışma: Gerçek Zamanlı Sipariş Durumu

**Soru:** Müşteri siparişini verdikten sonra durumu nasıl takip edecek?

**Polling (HTTP):**
- 5 saniyede bir `GET /orders/{id}/status` isteği
- Artıları: Basit, bağlantı yönetimi yok, mobil pil dostu
- Eksileri: Hafif gecikme, fazladan HTTP trafiği

**SignalR:**
- Artıları: Anlık güncelleme
- Eksileri: Mobil tarayıcıda arka plana geçince bağlantı kopabilir, pil tüketimi daha fazla

**Karar: Polling (5 saniye)**
- `OrderStatusService` ile interval tabanlı polling
- Sayfa görünür durumdayken (Page Visibility API) polling aktif, arka planda duraklatılır
- Sipariş `Delivered` veya `Paid` durumuna gelince polling durur
- Durum değişimlerinde animasyon ile kullanıcıya bildirim

---

### Tartışma: PWA (Progressive Web App)

**Soru:** Offline kullanım veya Service Worker eklenecek mi?

**PWA Artıları:**
- Ana ekrana ekle (Add to Home Screen)
- Offline menü görüntüleme (cache)
- Push notification desteği

**PWA Eksileri:**
- Service worker geliştirme ve test karmaşıklığı
- Cache invalidation sorunları (menü güncellenince eski cache)

**Karar: İlk versiyonda PWA yok**
- `@angular/pwa` paketi eklenmeyecek
- İleride eklenebilir; mimari bunu engellemiyor
- Offline menü için alternatif: agresif HTTP cache header'ları

---

### Tartışma: Dil Desteği

**Soru:** QR uygulama çok dil destekleyecek mi?

**Karar:**
- İlk versiyonda sadece Türkçe
- Mimari i18n'e uygun yazılacak (hardcoded string yok, enum label'lar merkezi)
- `ngx-translate` ileride eklenebilir
- Restoran kendi menü içeriğini zaten kendi dilinde girecek (backend'den gelecek)
- UI metinleri için çeviri dosyası yapısı hazır bırakılacak
