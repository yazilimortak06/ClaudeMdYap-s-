# sarj_ev_panel_angular — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | Angular 13.1.2 |
| UI Template | Metronic v7.1.7 (Keenthemes) |
| CSS | SCSS, Bootstrap (Metronic içinde) |
| UI Kütüphanesi | Angular Material ^11.0.0 |
| Auth | @auth0/angular-jwt ^5.0.2 |
| Realtime | @microsoft/signalr ^6.0.1 |
| i18n | @ngx-translate/core ^13.0.0 |
| Grafik | ApexCharts + Chart.js (ikisi birden) |
| Tarih | Moment.js ^2.29.1 |
| HTML Editör | ngx-editor ^7.3.2 |
| Dosya Yükleme | ngx-dropzone ^2.3.0 |
| Spinner | ngx-spinner ^13.0.0 |

## Mimari Pattern

**Monorepo değil, tek Angular projesi.** Uygulama içi katmanlama 3 seviyede:

1. `core/` — teknik altyapı katmanı
2. `evtech/` — feature/domain katmanı
3. `shared_admin/` — layout ve çapraz kesim kaygıları (cross-cutting concerns)

Bu ayrım standart Angular önerilerini takip eder. `shared_admin` ismi şablona (Metronic'in klasör önerisi) bağlı kalmıştır.

## Dikkat Çeken Kararlar

### HashLocationStrategy
```typescript
RouterModule.forRoot(routes, { useHash: true })
```
Hash-based routing kullanılmış. Bu genellikle SPA'yı static file server veya CDN üzerine deploy etmek için ya da backend routing konfigürasyonundan kaçınmak için tercih edilir. Admin paneller için makul bir seçim.

### JWT localStorage
Token localStorage'da saklanıyor. Bu güvenlik açısından tartışmalı (XSS riski) ancak admin panellerde yaygın pratik. HttpOnly cookie tercih edilebilirdi.

### Multi-environment (4 ortam)
`environment.ts`, `environment.prod.ts`, `environment.local.ts`, `environment.test.ts` — 4 farklı ortam. Hem local geliştirme hem de test sunucusu için ayrı konfigürasyon tutuluyor. Bu iyi bir pratik.

### Lazy Loading — Tüm Feature Modülleri
Auth, Layout ve Error sayfaları hepsi lazy load ediliyor:
```typescript
loadChildren: () => import('./evtech/auth/auth.module')...
```
İlk yükleme süresini minimize eder, admin panel büyüklüğü göz önüne alındığında kritik.

### Dual Chart Library
Hem `apexcharts`/`ng-apexcharts` hem de `chart.js`/`ng2-charts` kullanılıyor. İki farklı chart kütüphanesi olması muhtemelen farklı zamanlarda eklenen componentlerden kaynaklanıyor. Bundle size artırır, tek kütüphaneye geçiş tercih edilir.

### @microsoft/signalr
Live monitoring componenti için SignalR WebSocket kullanılıyor. Real-time cihaz durumu takibi için.

## Feature Modülü Organizasyonu

28 component kategorisi — en büyük panel. Kategoriler şu prensiple ayrılmış:

- **İşlevsel gruplar:** şarj cihazı, istasyon, rezervasyon, ödeme kendi klasörlerinde
- **Cross-cutting:** auth, authority, panelAdmin genel yönetim için ayrı
- **Support:** log, reporting, technical-support izleme için ayrı
- **Config:** paramaters, system-parameter, language, version-management konfigürasyon için ayrı

Bu granülarite büyük projelerde iyi çalışır, her kategori kendi modülüne lazy load edilebilir.

## Core / Shared / Feature Ayrımı

### core/
- `adapters/` — data dönüşüm katmanı (API response → component model)
- `bases/base-datatable/`, `bases/base-tree-table/` — tablo componentleri için abstract base class'lar
- `directives/` — 7 directive (form validation, input masking, vb.)
- `pipes/` — 6 pipe (format dönüşümleri)
- `external-components/` — özelleştirilmiş üçüncü parti wrapper'lar (dropzone, html editor)
- `date-core/` — tarih işlemleri için özel servis/util
- `configs/` — global konfigürasyon nesneleri
- `wrapper-core/` — dış servis wrapper'ları

Core modülü `AppModule`'e import edilir, uygulama boyunca tekil instance sağlar.

### shared_admin/
- Layout componentleri (Metronic'in aside, header, footer, topbar yapısı)
- Auth guard ve authentication service
- HTTP interceptor
- Material modülü — Angular Material import consolidation

### evtech/ (feature)
- Tüm business logic
- Domain'e ait model, service ve component'lar
- Kendi içinde alt routing modülü

## Interceptor / Guard Yaklaşımı

### HTTP Interceptor (http-event-interseptor.ts)
- `shared_admin/utils/interseptors/` altında (typo: "interseptor")
- `AppModule.providers`'a HTTP_INTERCEPTORS token ile sağlanır
- Her request'e Bearer token ekler
- 401'de localStorage temizleme + redirect

### AuthGuard
- `shared_admin/auth/` altında
- `CanActivate` implement eder
- localStorage token varlığı kontrolü
- Token yoksa `/auth/login`'e yönlendirir, returnUrl query param ile

## Metronic Template Kullanımı

Metronic v7.1.7, Keenthemes'in ticari Angular admin template'i. Bu proje Metronic'i bazı şekillerde kullanıyor:

1. **Layout sistemi** — aside, header, footer, subheader, topbar componentleri Metronic'ten alınmış, özelleştirilmiş
2. **SCSS değişkenleri** — renk şeması, tipografi Metronic tema sistemi üzerinden
3. **shared_admin/** klasörü — Metronic'in önerdiği klasör yapısına yakın
4. **Splash screen** — Metronic'in yükleme ekranı komponenti

Metronic, lisanslı bir template olduğundan doğrudan npm paketi olarak gelmez; kaynak koduna entegre edilir.

## Projeler Arası Benzerlikler

### sarj_ev_panel_angular vs sarj_pro_panel_angular
Bu iki proje neredeyse aynı altyapıya sahip:
- Özdeş `core/` yapısı
- Özdeş `shared_admin/` yapısı
- Özdeş `package.json` bağımlılıkları
- 4 environment dosyası (aynı ortam stratejisi)
- Sadece feature modülü (evtech vs sarjAllPro) ve scope farklı

Bu durum muhtemelen sarj_pro'nun sarj_ev'den fork edildiğine ya da aynı template'ten başlatıldığına işaret ediyor.

### sarj_ev_panel_angular vs yonetim_panel_angular
- Core ve shared_admin yapısı yine özdeş
- yonetim_panel daha az feature kategorisine sahip (6 vs 28)
- yonetim_panel sadece 2 environment dosyasına sahip
- yonetim_panel Bootstrap'i explicit bağımlılık olarak ekliyor

## Sonuç

Bu proje, tek developer veya küçük ekibin Metronic template üzerine hızla geliştirdiği, kapsamlı bir EV şarj yönetim paneli. Mimari kararlar pragmatik: lazy loading doğru kullanılmış, environment yönetimi iyi, ancak çift chart kütüphanesi ve localStorage JWT gibi iyileştirilebilecek noktalar var. Feature modülünün 28 kategoriye büyümesi, projenin zamanla scope'unu genişlettiğini gösteriyor.
