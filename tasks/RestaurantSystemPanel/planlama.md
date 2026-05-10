# RestaurantSystemPanel — Planlama

Tartışmalar, kesinleşmemiş görevler, fikirler.

---

### Tartışma: Sipariş Sayfası Real-time Güncellemeleri

**Soru:** Panel'deki aktif siparişler sayfası anlık olarak nasıl güncellenecek?

**SignalR:**
- Artıları: Anlık güncelleme, sunucu push, kullanıcı aksiyonu gerektirmez, ses bildirimi tetiklenebilir
- Eksileri: Bağlantı yönetimi, reconnect mantığı yazılmalı

**Angular Polling:**
- Artıları: Basit, bağlantı sorunu yok
- Eksileri: Gecikme (interval kadar), gereksiz HTTP isteği, kullanıcı deneyimi zayıf

**Karar: SignalR**
- `OrderHub`'a bağlanılacak, restoran bazlı group'a join olunacak
- Yeni sipariş geldiğinde `newOrder` eventi dinlenecek
- Sipariş durumu değiştiğinde `orderStatusChanged` eventi dinlenecek
- Ses bildirimi: yeni sipariş geldiğinde tarayıcı Audio API ile ses çalınacak
- Bağlantı kopması durumunda otomatik reconnect (exponential backoff)

---

### Tartışma: Masa Görünümü

**Soru:** Masa yönetimi sayfasında masalar nasıl gösterilecek?

**Grid (Kart) Görünümü:**
- Masa numarası, kapasite, durum (boş/dolu/rezerve) kartlar halinde
- Artıları: Hızlı geliştirme, responsive, mobil uyumlu
- Eksileri: Fiziksel salon düzenini yansıtmıyor

**İnteraktif Salon Planı:**
- Drag-and-drop ile masa konumlandırma, gerçek salon haritası
- Artıları: Görsel olarak zengin, gerçekçi
- Eksileri: Geliştirme süresi uzun, mobilde karmaşık

**Karar: Önce Grid**
- İlk versiyonda kart tabanlı grid görünümü (masa numarası, kapasite, durum badge)
- Filtreler: boş/dolu/tümü
- Her kart üzerinde: QR görüntüle, QR yenile, düzenle, sil aksiyonları
- Salon planı ileride eklenebilir (ayrı sekme veya mod olarak)

---

### Fikir: Dark Mode

**Açıklama:** Metronic template'in hazır dark theme desteği var.

**Durum:** Henüz kesinleşmedi

**Değerlendirme:**
- Metronic'te tema değiştirme CSS variable tabanlı, Angular'a entegrasyon kolay
- Kullanıcı tercihi localStorage'da saklanabilir
- Gece vardiyası çalışanları için faydalı
- İlk versiyonda düşük öncelikli, temel sayfalar tamamlandıktan sonra eklenebilir

---

### Fikir: Dil Desteği (i18n)

**Açıklama:** Panel'de çok dil desteği.

**Durum:** Tasarım aşamasında

**Değerlendirme:**
- `ngx-translate` kurulumu hazır, `TranslateModule` global import edilecek
- İlk hedef: TR (Türkçe) + EN (İngilizce)
- Çeviri dosyaları: `assets/i18n/tr.json`, `assets/i18n/en.json`
- Dil seçimi header'da dropdown ile değiştirilebilir
- `TranslatePipe` tüm template'lerde kullanılacak, hardcoded string yasak
- Backend hata mesajları için ayrıca çeviri mapping gerekebilir
