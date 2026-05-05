# sarj_ev_panel_angular — Klasör Yapısı

Angular 13, EV şarj yönetim admin paneli, Metronic template tabanlı.

## Genel Yapı

```
sarj_ev_panel_angular/
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
│   │   ├── evtech/                   (feature modülü — 28 component kategorisi)
│   │   │   ├── components/
│   │   │   │   ├── announcement/
│   │   │   │   ├── auth/
│   │   │   │   ├── auth-management/
│   │   │   │   ├── authority-management/
│   │   │   │   ├── campaign/
│   │   │   │   ├── chargeDevice/
│   │   │   │   ├── chargeDeviceReservation/
│   │   │   │   ├── chargeManagment/
│   │   │   │   ├── company/
│   │   │   │   ├── home/
│   │   │   │   │   └── dashboardStats/
│   │   │   │   ├── language/
│   │   │   │   ├── live-monitoring/
│   │   │   │   ├── log/
│   │   │   │   ├── messagesender/
│   │   │   │   ├── panelAdmin/
│   │   │   │   ├── paramaters/
│   │   │   │   ├── payments/
│   │   │   │   ├── policy/
│   │   │   │   ├── reporting/
│   │   │   │   ├── stations/
│   │   │   │   ├── support/
│   │   │   │   ├── system-parameter/
│   │   │   │   ├── technical-support/
│   │   │   │   ├── test/
│   │   │   │   ├── users/
│   │   │   │   ├── version-management/
│   │   │   │   ├── wallet/
│   │   │   │   └── webSite-contact/
│   │   │   ├── enums/
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
│       ├── environment.prod.ts
│       ├── environment.local.ts
│       └── environment.test.ts
│
├── angular.json
├── package.json
└── tsconfig.json
```

## Katman Açıklamaları

| Katman | Açıklama |
|---|---|
| `_fake/` | Geliştirme ortamı için mock data ve servisler |
| `core/` | Uygulama genelinde kullanılan teknik altyapı (directive, pipe, adapter, base class) |
| `evtech/` | Domain'e ait tüm feature componentleri, modelleri ve servisleri |
| `shared_admin/` | Metronic layout sistemi, auth, guard ve interceptor |
| `environments/` | 4 ortam: production, development, local, test |

## Feature Modülü (evtech) Kategorileri

- **Kimlik / Yetkilendirme:** auth, auth-management, authority-management
- **Şarj Cihazları:** chargeDevice, chargeDeviceReservation, chargeManagment
- **İstasyonlar:** stations, live-monitoring
- **Kullanıcı:** users, wallet, payments
- **İçerik / Duyuru:** announcement, campaign, policy, messagesender, webSite-contact
- **Yönetim:** panelAdmin, company, support, technical-support
- **Yapılandırma:** paramaters, system-parameter, language, version-management
- **Analitik:** home/dashboardStats, reporting, log
- **Diğer:** test
