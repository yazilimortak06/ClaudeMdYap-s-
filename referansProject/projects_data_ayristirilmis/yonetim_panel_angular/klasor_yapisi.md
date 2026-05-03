# yonetim_panel_angular — Klasör Yapısı

Angular 13, restoran yönetim admin paneli, Metronic template tabanlı.

## Genel Yapı

```
yonetim_panel_angular/
├── src/
│   ├── app/
│   │   ├── _fake/
│   │   │   ├── fake-data.ts
│   │   │   └── fake-service.ts
│   │   │
│   │   ├── core/
│   │   │   ├── adapters/
│   │   │   ├── bases/
│   │   │   │   ├── base-datatable/
│   │   │   │   └── base-tree-table/
│   │   │   ├── configs/
│   │   │   ├── date-core/
│   │   │   ├── directives/           (7 directive)
│   │   │   ├── external-components/
│   │   │   │   ├── pixdinn-dropzone/
│   │   │   │   └── pixdinn-html-editor/
│   │   │   ├── pipes/                (6 pipe)
│   │   │   ├── services/
│   │   │   ├── wrapper-core/
│   │   │   └── core.module.ts
│   │   │
│   │   ├── pixdinnRestaurantSystem/  (feature modülü — 6 component kategorisi)
│   │   │   ├── components/
│   │   │   │   ├── auth/
│   │   │   │   ├── authority-management/
│   │   │   │   ├── business-management/
│   │   │   │   ├── category-management/
│   │   │   │   ├── panel-admin-management/
│   │   │   │   └── home/
│   │   │   ├── models/
│   │   │   ├── services/
│   │   │   └── pages-routing.module.ts
│   │   │
│   │   ├── shared_admin/
│   │   │   ├── auth/
│   │   │   │   ├── auth.guard.ts
│   │   │   │   └── authentication-service.ts
│   │   │   ├── partials/
│   │   │   │   ├── aside/
│   │   │   │   ├── dialogs/
│   │   │   │   ├── footer/
│   │   │   │   ├── header/
│   │   │   │   ├── layout/
│   │   │   │   ├── subheader/
│   │   │   │   └── topbar/
│   │   │   ├── template/
│   │   │   │   ├── error-pages/
│   │   │   │   └── splash-screen/
│   │   │   ├── utils/
│   │   │   │   └── interseptors/
│   │   │   │       └── http-event-interseptor.ts
│   │   │   ├── general-material.module.ts
│   │   │   ├── layout.module.ts
│   │   │   └── shared.module.ts
│   │   │
│   │   ├── app-routing.module.ts
│   │   └── app.module.ts
│   │
│   └── environments/
│       ├── environment.ts
│       └── environment.prod.ts
│
├── angular.json
├── package.json
└── tsconfig.json
```

## Katman Açıklamaları

| Katman | Açıklama |
|---|---|
| `_fake/` | Geliştirme ortamı için mock data ve servisler |
| `core/` | Uygulama genelinde kullanılan teknik altyapı (sarj_ev_panel_angular ile aynı yapı) |
| `pixdinnRestaurantSystem/` | Domain'e ait tüm feature componentleri, modelleri ve servisleri |
| `shared_admin/` | Metronic layout sistemi, auth, guard ve interceptor |
| `environments/` | 2 ortam: production, development |

## sarj_ev_panel_angular ile Farklar

| Özellik | sarj_ev_panel_angular | yonetim_panel_angular |
|---|---|---|
| Feature modülü | evtech/ (28 kategori) | pixdinnRestaurantSystem/ (6 kategori) |
| Environment sayısı | 4 (prod, dev, local, test) | 2 (prod, dev) |
| Domain | EV şarj yönetimi | Restoran yönetimi |
| Core yapısı | Özdeş | Özdeş |
| shared_admin yapısı | Özdeş | Özdeş |
| Metronic version | v7.1.7 | v7.1.7 |
| HTML editör | ngx-editor | ngx-editor (Summernote tabanlı) |
| CSS framework | — | Bootstrap 4.6.1 |

## Feature Modülü Kategorileri

- **Kimlik / Yetkilendirme:** auth, authority-management
- **İşletme:** business-management
- **Kategori:** category-management
- **Yönetim:** panel-admin-management
- **Analitik:** home
