# sarj_pro_panel_angular — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | Angular 13.1.2 |
| UI Template | Metronic v7.1.7 (Keenthemes) |
| CSS | SCSS |
| UI Kütüphanesi | Angular Material ^11.0.0 |
| Auth | @auth0/angular-jwt ^5.0.2 |
| Realtime | @microsoft/signalr ^6.0.1 |
| i18n | @ngx-translate/core ^13.0.0 |
| Grafik | ApexCharts + Chart.js |
| Tarih | Moment.js ^2.29.1 |
| HTML Editör | ngx-editor |
| Dosya Yükleme | ngx-dropzone |
| Spinner | ngx-spinner |

Paket bağımlılıkları sarj_ev_panel_angular ile **özdeş**.

## Mimari Pattern

3-katmanlı yapı:
1. `core/` — sarj_ev ile özdeş teknik altyapı
2. `sarjAllPro/` — şarj istasyon yönetim altyapısına ait feature
3. `shared_admin/` — sarj_ev ile özdeş layout katmanı

`_fake/` klasörü bu projede yok — muhtemelen fake data kullanılmıyor veya doğrudan API ile çalışıyor.

## Dikkat Çeken Kararlar

### Zengin Model Katmanı (13 Model Grubu)
sarjAllPro'nun dikkat çekici özelliği, nispeten az feature kategorisine (8) karşın çok kapsamlı bir model katmanına sahip olması:

```
models/
├── apiexception/
├── authentication/
├── authority/
├── contentLanguage/
├── countryCityAndTown/   ← coğrafi lokasyon
├── mediaFile/
├── panelAdmin/
├── panelAdminType/
├── parameter/
├── parameterGroup/
├── parameterValue/
├── policy/
└── requestResponse/
```

Bu durum, projenin "infrastructure/platform" odaklı olduğunu gösteriyor — feature sayısı az ama veri modelleri ayrıntılı.

### Enum Katmanı (5 Dosya)
```
enums/
├── authentication/
│   └── panel-admin-user-type.enum.ts
└── mediaFile/
    ├── file-extention.enum.ts
    ├── file-media-type.enum.ts
    ├── file-type.enum.ts
    └── picture-type.enum.ts
```
Dosya yönetimi için ayrıntılı enum yapısı, medya yönetiminin bu projede önemli olduğunu gösteriyor.

### sarj_ev ile Neredeyse Aynı Template
`core/`, `shared_admin/`, `package.json`, environment stratejisi (4 dosya) — hepsi sarj_ev ile özdeş. Bu, sarj_pro'nun sarj_ev'in bir fork'u veya "Pro" versiyonu olduğunu düşündürüyor.

Fark, feature scope'unda: sarj_pro daha az sayfa/component içeriyor ancak daha sağlam bir model/enum altyapısına sahip. Bu, sarj_pro'nun backend-heavy, az UI'lı bir yönetim paneli olduğuna işaret ediyor.

### HashLocationStrategy
sarj_ev ile aynı — `useHash: true`.

### 4 Environment (local + test dahil)
sarj_ev ile aynı ortam stratejisi. Aktif geliştirme dönemine girmiş, multi-env ihtiyacı var.

## Feature Modülü Organizasyonu

8 kategori, minimal UI scope:

| Grup | Kategoriler |
|---|---|
| Kimlik | auth, authority-management |
| Yönetim | panelAdmin |
| Yapılandırma | paramaters, system-parameter, language |
| İçerik | policy |
| İzleme | log |

Bu kategoriler ağırlıklı olarak platform altyapısını yönetiyor: kullanıcılar, parametreler, politikalar, dil. Kullanıcıya yönelik özellikler (şarj cihazı yönetimi, ödeme) burada yok — bunlar sarj_ev'de.

**sarj_pro, sarj_ev'in "altyapı yönetim paneli" görevi görüyor olabilir.**

## Core / Shared / Feature Ayrımı

`core/` ve `shared_admin/` sarj_ev ile birebir aynı — zaten bu 3 projenin ortak paydası.

`sarjAllPro/` içinde dikkat çeken nokta: feature içinde `pipes/` klasörü var. Feature-specific pipe'lar feature modülünde tutulmuş, core/pipes'a taşınmamış. Bu ayrımın bilinçli olduğu düşünülebilir.

## Interceptor / Guard Yaklaşımı

sarj_ev ile aynı pattern. `shared_admin/utils/interseptors/http-event-interseptor.ts` (aynı typo dahil).

## Metronic Template Kullanımı

sarj_ev ile özdeş Metronic v7.1.7 kullanımı. Layout sistemi birebir aynı.

## Projeler Arası Benzerlikler

| Bileşen | sarj_ev | sarj_pro | yonetim |
|---|---|---|---|
| core/ | A | A | A |
| shared_admin/ | A | A | A |
| package.json | A | A | ~A |
| Interceptor | A | A | A |
| Guard | A | A | A |
| Environment | 4 | 4 | 2 |
| Feature kategorisi | 28 | 8 | 6 |
| Model grubu | Orta | 13 (en fazla) | Az |
| Enum | Yok | 5 | Yok |

A = Özdeş, ~A = Neredeyse aynı

**Çıkarım:** Bu üç proje aynı Angular Admin Starter template'inden türetilmiş. Altyapı paylaşılmış, sadece feature katmanı domain'e göre değişmiş. sarj_ev ve sarj_pro ikisi de şarj alanında, muhtemelen farklı müşteri/ürün için ayrı paneller.

## Sonuç

sarj_pro_panel_angular, sarj_ev ile aynı altyapıya sahip ama farklı rol üstleniyor: platform/altyapı yönetimi. Az sayfa ama kapsamlı model katmanı, bunu doğruluyor. Enum yapısının detaylı olması, medya dosyası yönetiminin bu platformda kritik olduğunu gösteriyor. sarj_ev ve sarj_pro birlikte tüm şarj platformunu yönetiyor olabilir: biri operasyonel (sarj_ev), diğeri altyapısal (sarj_pro).
