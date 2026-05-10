# RestaurantSystemQr — Gelişen Mimari

> Proje ilerledikçe alınan kararlar doğrultusunda güncellenir. Her önemli mimari karar buraya eklenir.

---

## Alınan Kararlar

### Temel Teknoloji Seçimleri

| Karar | Değer | Gerekçe |
|---|---|---|
| Angular versiyonu | **14+** | LTS, signal API kullanılabilir |
| Component mimarisi | **Standalone Components** | NgModule yok; hafif bundle, temiz import |
| Routing stratejisi | **HashLocationStrategy** | Panel ile tutarlı; sunucu config gerekmez |
| CSS sistemi | **SCSS + CSS custom properties** | Mobile-first değişkenler için |
| TypeScript | **strict: true** | Null safety, `any` yok |
| State yönetimi | **Service + computed signal** | Sepet gibi basit state için RxJS BehaviorSubject + Angular signal |
| Sepet kalıcılığı | **localStorage** | Sayfa yenileme ve sekme kapanmasına karşı dayanıklı |
| HTTP client | **Angular HttpClient + provideHttpClient()** | Standalone için yeni API |
| Görüntü optimizasyon | **loading="lazy" + onerror fallback** | Hızlı ilk yükleme |
| PWA | **İleride değerlendirilecek** | Manifest ve service worker altyapısı hazır tutulacak |
| Real-time (sipariş takip) | **HTTP Polling (10s)** | Sipariş durumu sayfası için; SignalR QR uygulamasına şu an eklenmedi |

### Sayfa Yapısı ve Routing Kararları

```
/qr/:tableId             → QrLandingComponent      (entry point — masa doğrulama)
/menu                    → MenuHomeComponent        (kategori listesi)
/menu/category/:id       → CategoryDetailComponent  (ürün listesi)
/menu/item/:id           → ItemDetailComponent      (ürün detay + sepete ekle)
/cart                    → CartComponent            (sepet)
/order/confirm           → OrderConfirmComponent    (sipariş özeti + gönder)
/order/status/:orderCode → OrderStatusComponent     (sipariş takip)
```

**Routing kararları:**
- `replaceUrl: true` ile QR landing → menü yönlendirmesi yapılır (geri tuşu landing'e dönmez).
- `TableGuard` → masa bilgisi localStorage'da yoksa `/qr` yönlendirir (henüz implement edilmedi, sprint 2'de eklenecek).
- Tüm route'lar `loadComponent()` ile lazy yüklenir.

### State Yönetimi Kararı

```
CartService (providedIn: 'root')
  ├── _items: WritableSignal<CartItem[]>   ← internal, private
  ├── items: Signal<CartItem[]>            ← readonly, component'ler okur
  ├── itemCount: computed Signal           ← sepet ikonu sayısı
  ├── totalAmount: computed Signal         ← toplam tutar
  └── isEmpty: computed Signal             ← sepet boş mu?

localStorage senkronizasyonu:
  - Her değişiklikte saveToStorage() çağrılır
  - Uygulama başında loadFromStorage() çağrılır
```

Angular signal API tercih edildi çünkü:
- `ChangeDetectionStrategy.OnPush` ile kutudan çıkar çalışır
- `computed()` ile türetilmiş değerler otomatik güncellenir
- RxJS pipe zinciri yazmak gerekmez

### Mobil UX Kararları

- **Alt navigasyon çubuğu (`BottomCartBarComponent`):** Her sayfada sabit, sepet özeti gösterir.
- **Kategori sayfası grid:** 2 sütunlu kart grid (320px'de 1 sütun).
- **Ürün detayı:** Tam ekran görsel + kaydırmalı açıklama + sticky sepete ekle butonu.
- **Sipariş tamamlandı:** Konfeti animasyonu (CSS keyframe) + sipariş takip butonuna yönlendir.

---

## Mimari Genel Görünüm

```
┌───────────────────────────────────────────────────────┐
│               RestaurantSystemQr                      │
│           (Angular 14+ Standalone)                    │
└───────────────────────────────────────────────────────┘
                        │
       ┌────────────────┼────────────────┐
       │                │                │
 ┌─────▼──────┐  ┌──────▼──────┐  ┌─────▼──────┐
 │   core/    │  │   shared/   │  │   pages/   │
 │------------|  │-------------|  │------------|
 │models      │  │components   │  │qr-landing  │
 │services    │  │(skeleton,   │  │menu-home   │
 │(table,     │  │ qty-select, │  │category    │
 │ menu,      │  │ cart-bar,   │  │item-detail │
 │ cart,      │  │ price-disp) │  │cart        │
 │ order)     │  │pipes        │  │order-conf. │
 │guards      │  └─────────────┘  │order-stat. │
 └────────────┘                   └────────────┘
                        │
          ┌─────────────┴─────────────┐
          │                           │
   ┌──────▼──────┐             ┌──────▼──────┐
   │  REST API   │             │ localStorage │
   │ /qr/...     │             │  (sepet)     │
   │ (public)    │             └─────────────┘
   └─────────────┘

Kullanıcı Akışı:
  QR Okut → /qr/:tableId → Masa doğrulama → localStorage'a kaydet
          → /menu (kategori listesi) → kategori seç
          → /menu/category/:id (ürün listesi) → ürün seç
          → /menu/item/:id (ürün detayı) → sepete ekle
          → /cart (sepet) → siparişi onayla
          → /order/confirm → sipariş API'ye gönderilir
          → /order/status/:orderCode → durum takibi (10s polling)
```

---

## Notlar / Tartışmalar

### Çözüme Bağlanan Tartışmalar

**NgModule vs Standalone**
- Karar: QR uygulaması tamamen standalone olacak.
- Gerekçe: Küçük uygulama, tek bir feature, NgModule gereksiz karmaşıklık ekler.
- Panel NgModule kullandığı için iki uygulama arasında mimari farklılık kasıtlıdır.

**SignalR vs Polling (sipariş takibi)**
- Karar: Sipariş takip sayfasında şimdilik **10 saniyede bir HTTP polling** yapılır.
- Gerekçe: QR uygulaması sayfa görüntüleme süresi kısa; SignalR bağlantı yönetimi ek maliyet.
- İlerleyen sprintte: Yoğun kullanım varsa SignalR eklenir.

**Ödeme entegrasyonu**
- Karar: QR uygulaması ödeme almaz; ödeme kasada yapılır.
- Gelecek değerlendirme: Online ödeme gerekirse ayrı bir sprint olarak planlanır.

### Açık Konular / Tartışılacaklar

- [ ] **PWA:** Service worker eklenecek mi? Offline menü görüntüleme için faydalı.
- [ ] **TableGuard:** Sprint 2'de implement edilecek. Masa seçmeden menüye erişim engellenmeli.
- [ ] **Dil desteği:** QR menüde İngilizce dil seçeneği (backend'den `nameEn` geliyor, UI henüz yok).
- [ ] **Sipariş geçmişi:** Müşteri aynı masada birden fazla sipariş verebilir; listeleme sayfası eklenebilir.
- [ ] **Analytics:** Google Analytics veya Mixpanel ile ürün görüntüleme takibi.
- [ ] **Erişilebilirlik (a11y):** `aria-label`, `role` attribute'ları eklenmeli.
