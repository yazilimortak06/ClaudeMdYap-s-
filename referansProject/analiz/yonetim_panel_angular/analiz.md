# yonetim_panel_angular — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | Angular 13.1.2 |
| UI Template | Metronic v7.1.7 (Keenthemes) |
| CSS | SCSS + Bootstrap 4.6.1 (explicit bağımlılık) |
| UI Kütüphanesi | Angular Material ^11.0.0 |
| Auth | @auth0/angular-jwt ^5.0.2 |
| Realtime | @microsoft/signalr ^6.0.1 |
| i18n | @ngx-translate/core ^13.0.0 |
| Grafik | ApexCharts + Chart.js |
| Tarih | Moment.js ^2.29.1 |
| HTML Editör | ngx-editor (Summernote tabanlı) |
| Dosya Yükleme | ngx-dropzone |
| Spinner | ngx-spinner |

## Mimari Pattern

Aynı 3-katmanlı yapı:
1. `core/` — teknik altyapı (sarj_ev ile özdeş)
2. `pixdinnRestaurantSystem/` — restoran domain'ine ait feature
3. `shared_admin/` — layout ve cross-cutting concerns (sarj_ev ile özdeş)

Bu proje üç Angular admin panel içinde en küçük scope'u olan: 6 feature kategorisi.

## Dikkat Çeken Kararlar

### Bootstrap Explicit Bağımlılığı
`package.json`'da `"bootstrap": "4.6.1"` açıkça bulunuyor. Diğer projelerde Bootstrap Metronic içinde gelirken burada explicit ekleme tercih edilmiş. Bu, daha az Metronic'e bağımlılık veya özelleştirilmiş Bootstrap kullanımına işaret edebilir.

### En Az Environment Dosyası (2)
Sadece `environment.ts` ve `environment.prod.ts`. Bu, projenin local/test ortamı ayrımına ihtiyaç duymadığına veya daha erken bir aşamada dondurulduğuna işaret eder.

### Küçük Feature Scope (6 Kategori)
Restoran yönetim paneli için 6 kategori yeterli görünüyor:
- auth, authority-management, business-management, category-management, panel-admin-management, home

Bu scope, bir restoran zinciri için temel CRUD yönetim paneli düzeyinde.

### SignalR Dahil Ama Küçük Panel
`@microsoft/signalr` bağımlılık olarak var ama 6 kategoride realtime feature görünmüyor. Muhtemelen altyapısal olarak şablondan gelmiş, aktif kullanılmıyor.

### HashLocationStrategy
sarj_ev ile aynı — `useHash: true`.

## Feature Modülü Organizasyonu

6 kategori, gruplar:

| Grup | Kategoriler |
|---|---|
| Kimlik | auth, authority-management |
| İçerik | business-management, category-management |
| Yönetim | panel-admin-management |
| Analitik | home |

Bu minimalist yapı, MVP veya v1 panel için tipik başlangıç noktası.

## Core / Shared / Feature Ayrımı

`core/` ve `shared_admin/` tamamen sarj_ev_panel_angular ile özdeş. Bu iki modülün yeniden kullanımı, aynı geliştirici altyapısını farklı projelere taşıma stratejisini gösteriyor — ortak bir "Angular admin starter" var, feature katmanı değişiyor.

## Interceptor / Guard Yaklaşımı

sarj_ev_panel_angular ile birebir aynı:
- `shared_admin/utils/interseptors/http-event-interseptor.ts`
- `shared_admin/auth/auth.guard.ts`

Aynı pattern, sadece feature modülü ismi farklı (import path'de `pixdinnRestaurantSystem`).

## Metronic Template Kullanımı

sarj_ev ile aynı Metronic v7.1.7. Layout sistemi birebir. Ek olarak Bootstrap 4.6.1 explicit dahil edilmiş — Metronic'in kullandığı Bootstrap versiyonunu kilitleme tercihinden olabilir.

## Projeler Arası Benzerlikler

Bu proje, sarj_ev_panel_angular ile altyapı açısından %90 aynı:

| Bileşen | sarj_ev | yonetim |
|---|---|---|
| core/ | Özdeş | Özdeş |
| shared_admin/ | Özdeş | Özdeş |
| package.json | Neredeyse aynı | + Bootstrap 4.6.1 |
| Interceptor | Aynı | Aynı |
| Guard | Aynı | Aynı |
| Routing pattern | Aynı | Aynı |
| Feature modülü | evtech/ (28) | pixdinnRestaurantSystem/ (6) |
| Environment | 4 | 2 |

## Sonuç

Bu proje, sarj_ev_panel_angular'ın daha küçük scope'lu bir fork'u veya aynı Angular admin starter şablonundan türetilmiş bir panel. Restoran yönetimi için temel işlevleri karşılıyor. Altyapı olgun, feature scope küçük — bu bir MVP veya internal tool paneli. Bootstrap bağımlılığının explicit eklenmesi ve 2 environment yapısı daha pragmatik bir yaklaşımı yansıtıyor.
