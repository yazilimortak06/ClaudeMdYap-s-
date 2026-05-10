# RestaurantSystemPanel — Gelişen Mimari

> Proje ilerledikçe alınan kararlar doğrultusunda güncellenir. Her önemli mimari karar buraya eklenir.
> Bu dosya "canlı doküman"dır — karar değişirse eski karar üzerine yorum satırıyla not düşülür.

---

## Alınan Kararlar

### Temel Teknoloji Seçimleri

| Karar | Değer | Gerekçe |
|---|---|---|
| Angular versiyonu | **14+** (strict mode) | LTS, standalone component desteği var, takım deneyimi |
| UI şablonu | **Metronic 8 (Angular)** | Hızlı dashboard geliştirme, hazır bileşenler |
| Routing stratejisi | **HashLocationStrategy** | Sunucu tarafı URL yönlendirme gerekmez; nginx config kolaylığı |
| CSS sistemi | **SCSS** | Değişken, mixin, nesting desteği; Metronic ile uyumlu |
| TypeScript | **strict: true** | `any` yok, null safety, derleme zamanı hata yakalama |
| HTTP client | **Angular HttpClient** | Built-in, interceptor desteği tam |
| Real-time | **SignalR (`@microsoft/signalr`)** | Backend .NET Core ile tam uyumlu, WebSocket + fallback |
| Spinner | **ngx-spinner** | Kolay entegrasyon, interceptor ile global kullanım |
| Dosya yükleme | **ngx-dropzone** | Sürükle-bırak, önizleme, çoklu dosya desteği |
| Grafikler | **ng-apexcharts** | ApexCharts wrapper, rapor sayfaları için |
| State yönetimi | **Service + BehaviorSubject** | NgRx eklememek için; karmaşıklık yeterince düşük |

### Modul Stratejisi

- Her feature kendi **lazy-loaded NgModule**'üne sahiptir.
- `CoreModule` singleton guard ile (`@Optional() @SkipSelf()`) korunur — iki kez import edilemez.
- `SharedAdminModule` `exports` dizisinde sık kullanılan Angular Material modülleri bulunur.
- Feature modüller `SharedAdminModule`'ü import eder; doğrudan Material modülünü import etmez.

```
Lazy loading kararı:
  - auth        → /auth     (ayrı bundle, token gerektirmez)
  - dashboard   → /dashboard
  - menu        → /menu
  - orders      → /orders
  - tables      → /tables
  - qr          → /qr
  - users       → /users    (Admin only)
  - reports     → /reports  (Admin + Manager)
  - settings    → /settings
```

### Klasör Yapısı Kararı

```
src/app/
  core/              → Singleton servisler, interceptor, guard, model (uygulama geneli)
  shared_admin/      → Layout bileşenleri, paylaşılan UI, pipe, directive
  restaurant/        → Feature modülleri (her biri kendi klasöründe)
```

**Neden `shared_admin`?** QR uygulaması ile ayrı repo/proje olduğu için `shared` adı çakışmaya neden olabilir. `_admin` suffix, bu dosyaların yalnızca panel uygulamasına ait olduğunu netleştirir.

### HTTP Interceptor Sıralaması

`app.module.ts` içinde interceptor sırası önemlidir:

```
1. SpinnerInterceptor   → her isteğin başında spinner açar
2. AuthInterceptor      → token ekler, 401/403 yönetir
3. ErrorInterceptor     → 500+ hatalarını yakalar ve toast gösterir
```

### SignalR — Real-time Kararları

| Karar | Değer |
|---|---|
| Hub endpoint | `/hubs/order` |
| Kimlik doğrulama | Query param: `access_token` (SignalR WebSocket'te header desteklemez) |
| Yeniden bağlanma | `withAutomaticReconnect([0, 2000, 5000, 10000])` |
| Bağlantı başlatma noktası | `MainLayoutComponent.ngOnInit()` |
| Bağlantı kapatma noktası | `AuthService.logout()` |

**Dinlenen event'ler:**

```
OrderCreated          → DashboardHomeComponent, OrderActiveComponent, KitchenDisplayComponent, TopbarComponent
OrderStatusChanged    → DashboardHomeComponent, OrderActiveComponent, KitchenDisplayComponent
OrderItemAdded        → OrderActiveComponent
TableStatusChanged    → TableListComponent
```

---

## Mimari Genel Görünüm

```
┌─────────────────────────────────────────────────────────┐
│                 RestaurantSystemPanel                   │
│                   (Angular 14+)                         │
└─────────────────────────────────────────────────────────┘
                          │
          ┌───────────────┼───────────────┐
          │               │               │
    ┌─────▼─────┐  ┌──────▼──────┐  ┌────▼──────────┐
    │   core/   │  │shared_admin/│  │  restaurant/  │
    │-----------│  │-------------|  │ (feature lazy)│
    │interceptor│  │layout/      │  │---------------|
    │guards     │  │components/  │  │auth/          │
    │models     │  │pipes/       │  │dashboard/     │
    │services   │  │directives/  │  │menu-mgmt/     │
    │(singleton)│  │             │  │order-mgmt/    │
    └───────────┘  └─────────────┘  │table-mgmt/    │
                                    │qr-mgmt/       │
                                    │user-mgmt/     │
                                    │reports/       │
                                    │settings/      │
                                    └───────────────┘
                                          │
                    ┌─────────────────────┼─────────────────────┐
                    │                     │                     │
             ┌──────▼──────┐     ┌────────▼───────┐   ┌────────▼──────┐
             │  REST API   │     │   File Server  │   │   SignalR Hub │
             │ /api/v1/... │     │(resim yükleme) │   │  /hubs/order  │
             └─────────────┘     └────────────────┘   └───────────────┘

Akış:
  Kullanıcı → HashRouter → AuthGuard → RoleGuard → MainLayout
           → Interceptor (Spinner + Auth + Error) → HttpClient
           → REST API (CRUD işlemleri)
           → SignalR Hub (gerçek zamanlı bildirimler)
```

---

## Notlar / Tartışmalar

### Çözüme Bağlanan Tartışmalar

**NgRx vs Service+BehaviorSubject**
- Karar: Service+BehaviorSubject yeterli. Sipariş akışı SignalR üzerinden geldiği için global store karmaşıklık ekliyor.
- Eğer ilerleyen dönemde offline destek veya karmaşık state gerekirse NgRx düşünülür.

**Angular Material vs Metronic bileşenleri**
- Karar: Metronic layout ve kart bileşenleri kullanılır, form alanları için Angular Material kullanılır.
- Sebep: Metronic'in form bileşenleri Angular reactive forms ile bazen çakışıyor.

**Standalone Components vs NgModule**
- Karar: Panel'de **NgModule** devam eder (Metronic şablonu NgModule tabanlı geldiği için geçiş maliyeti yüksek).
- QR uygulaması ise standalone kullanır.

### Açık Konular / Tartışılacaklar

- [ ] Refresh token akışı: token dolunca otomatik yenileme mi yapılacak, yoksa direkt login mi?
- [ ] İngilizce dil desteği: `ngx-translate` entegrasyonu ne zaman eklenecek?
- [ ] Rapor sayfaları için PDF/Excel export: `jsPDF` veya `xlsx` kütüphanesi seçilecek.
- [ ] E2E test: Cypress mi, Playwright mi? (Önce unit test kapsamı sağlanacak.)
