# RestaurantSystemPanel — Görev Havuzu

Kesinleşmiş ama henüz alınmamış görevler. İş alınınca `yapilacaklar.md`'ye taşınır.

---

## Altyapı

- [ ] **Angular scaffolding (Metronic template entegrasyonu)** — Yeni Angular projesi oluşturma, Metronic theme kurulumu, gereksiz demo sayfaların temizlenmesi. _(Tahmini: 3 saat)_
- [ ] **Core/SharedAdmin/Restaurant klasör yapısı kurulumu** — `core/` (services, models, guards, interceptors), `shared-admin/` (common components), `restaurant/` (feature modules) klasör hiyerarşisi. _(Tahmini: 1 saat | Bağımlılık: Angular scaffolding)_
- [ ] **HTTP Interceptor (Auth + Spinner + Error)** — `AuthInterceptor` (Bearer token ekleme), `SpinnerInterceptor` (yükleme göstergesi), `ErrorInterceptor` (hata yönetimi + toast). _(Tahmini: 3 saat | Bağımlılık: Klasör yapısı)_
- [ ] **AuthGuard + RoleGuard** — Route koruması, token geçerlilik kontrolü, rol bazlı erişim kısıtlaması. _(Tahmini: 2 saat | Bağımlılık: HTTP Interceptor)_
- [ ] **Auth modülü (login sayfası, token yönetimi)** — Login formu, `AuthService` (login/logout/refresh), token localStorage yönetimi, auto-login. _(Tahmini: 4 saat | Bağımlılık: AuthGuard)_
- [ ] **Layout modülü (sidebar nav, header, breadcrumb)** — Metronic layout entegrasyonu, dinamik sidebar menüsü, header kullanıcı bilgisi, breadcrumb servisi. _(Tahmini: 4 saat | Bağımlılık: Auth modülü)_
- [ ] **404/403 error sayfaları** — Özelleştirilmiş hata sayfaları, yönlendirme kuralları. _(Tahmini: 1 saat | Bağımlılık: Layout modülü)_
- [ ] **ngx-spinner global entegrasyonu** — Spinner component global kurulumu, interceptor ile otomatik göster/gizle. _(Tahmini: 1 saat | Bağımlılık: HTTP Interceptor)_
- [ ] **Angular Material + ApexCharts kurulumu** — Material modülleri import, ApexCharts wrapper component, theme uyumu. _(Tahmini: 2 saat | Bağımlılık: Angular scaffolding)_

---

## Sayfalar

- [ ] **Dashboard** — Özet istatistikler (bugünkü sipariş sayısı, ciro, aktif masa sayısı), son siparişler listesi, ApexCharts ile günlük satış grafiği. _(Tahmini: 5 saat | Bağımlılık: Layout modülü, Backend API)_
- [ ] **Menü Kategorileri (liste + ekleme/düzenleme)** — Kategori listesi, sıralama (drag-and-drop), ekleme/düzenleme modal, silme onayı. _(Tahmini: 4 saat | Bağımlılık: Layout modülü)_
- [ ] **Menü Ürünleri (liste + ekleme/düzenleme + görsel yükleme)** — Ürün listesi (kategori filtreli), ekleme/düzenleme formu, görsel yükleme (drag-and-drop + preview), aktif/pasif toggle. _(Tahmini: 6 saat | Bağımlılık: Menü Kategorileri)_
- [ ] **Siparişler - Aktif (real-time, SignalR)** — Kanban görünümü (Pending/Accepted/Preparing/Ready sütunları), SignalR ile canlı güncelleme, ses bildirimi, sipariş detay modal, durum güncelleme butonları. _(Tahmini: 8 saat | Bağımlılık: Layout modülü, SignalR backend)_
- [ ] **Siparişler - Geçmiş (sayfalı liste)** — Tarih filtreli sipariş geçmişi, sayfalama (server-side), sipariş detay görünümü, CSV export. _(Tahmini: 4 saat | Bağımlılık: Layout modülü)_
- [ ] **Masa Yönetimi (liste + QR görüntüleme/yenileme)** — Kart grid görünümü, masa ekleme/düzenleme, QR kod görüntüleme modal (qrcode.js), QR yenileme, yazdırma. _(Tahmini: 5 saat | Bağımlılık: Layout modülü)_
- [ ] **Kullanıcı Yönetimi** — Kullanıcı listesi, davet/ekleme formu, rol atama, aktif/pasif toggle, şifre sıfırlama. _(Tahmini: 4 saat | Bağımlılık: Layout modülü)_
- [ ] **Raporlar (günlük/aylık satış)** — Tarih aralığı seçici, günlük/aylık satış tablosu, ApexCharts grafikler, PDF/Excel export. _(Tahmini: 6 saat | Bağımlılık: Dashboard)_
- [ ] **Restoran Ayarları** — Restoran bilgileri (ad, adres, logo), çalışma saatleri, bildirim tercihleri. _(Tahmini: 3 saat | Bağımlılık: Layout modülü)_
