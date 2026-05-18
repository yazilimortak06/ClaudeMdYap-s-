# RestaurantSystemQr — Görev Havuzu

Kesinleşmiş ama henüz alınmamış görevler. İş alınınca `yapilacaklar.md`'ye taşınır.

---

## Altyapı

- [ ] **Angular scaffolding (standalone components, minimal)** — Standalone component mimarisi, minimal bundle hedefi, routing kurulumu, environment konfigürasyonu. _(Tahmini: 2 saat)_
- [ ] **QrLanding sayfası (masa token doğrulama)** — URL'den token parse etme, backend'e token doğrulama isteği, geçersiz/süresi dolmuş QR için hata ekranı, başarılı doğrulamada menüye yönlendirme. _(Tahmini: 3 saat | Bağımlılık: Angular scaffolding)_
- [ ] **MenuHome sayfası (kategoriler grid)** — Aktif kategoriler grid görünümü, restoran adı/logosu header, kategori kartı (resim + ad + ürün sayısı). _(Tahmini: 3 saat | Bağımlılık: QrLanding)_
- [ ] **CategoryDetail sayfası (ürünler + filtreleme)** — Kategoriye ait ürünler listesi, arama filtresi, ürün kartı (resim, ad, fiyat, sepete ekle butonu). _(Tahmini: 4 saat | Bağımlılık: MenuHome)_
- [ ] **ItemDetail sayfası (ürün detayı, seçenekler, sepete ekle)** — Ürün büyük resim, açıklama, seçenek grupları (radio/checkbox), adet seçici, "Sepete Ekle" butonu. _(Tahmini: 4 saat | Bağımlılık: CategoryDetail)_
- [ ] **Cart (sepet) — FloatingButton + Cart sayfası** — Sabit floating sepet butonu (adet badge), Cart sayfası (ürün listesi, adet güncelleme, kaldır, toplam tutar). _(Tahmini: 4 saat | Bağımlılık: ItemDetail, CartService)_
- [ ] **CartService (localStorage ile state yönetimi)** — `CartService` singleton, sepet state'i `localStorage`'da sakla, `BehaviorSubject` ile reactive state. _(Tahmini: 3 saat | Bağımlılık: Angular scaffolding)_
- [ ] **OrderConfirm sayfası (sepet özeti + not + sipariş ver)** — Sepet özeti görünümü, masa notu text alanı, "Siparişi Gönder" butonu, backend'e sipariş POST isteği. _(Tahmini: 3 saat | Bağımlılık: Cart, CartService)_
- [ ] **OrderStatus sayfası (sipariş durumu polling)** — Sipariş ID ile durum polling (5sn), durum zaman çizelgesi (Pending→Paid), tamamlanma animasyonu. _(Tahmini: 4 saat | Bağımlılık: OrderConfirm)_

---

## UX / Performans

- [ ] **Skeleton screen tüm sayfalarda** — MenuHome, CategoryDetail, ItemDetail, OrderStatus sayfaları için içerik yüklenmeden önce gösterilen skeleton placeholder component'leri. _(Tahmini: 3 saat | Bağımlılık: İlgili sayfalar)_
- [ ] **Lazy image loading (IntersectionObserver)** — Ürün görselleri için `IntersectionObserver` tabanlı lazy loading directive, blur-up placeholder. _(Tahmini: 2 saat | Bağımlılık: CategoryDetail)_
- [ ] **Mobile-first CSS (bottom nav, 44px touch target)** — Bottom navigation bar (Menü / Sepet / Siparişim), tüm tıklanabilir elemanlar min 44px, safe-area-inset desteği (iPhone notch). _(Tahmini: 3 saat | Bağımlılık: Angular scaffolding)_
