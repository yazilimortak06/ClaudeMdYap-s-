# sarj_pro_panel_angular — Klasör Yapısı

Angular 13, şarj istasyon yönetim admin paneli, Metronic v7.1.7 template tabanlı.

## Genel Yapı

```
sarj_pro_panel_angular/
├── src/
│   ├── app/
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
│   │   ├── sarjAllPro/               (feature modülü — 8 component kategorisi)
│   │   │   ├── components/
│   │   │   │   ├── auth/
│   │   │   │   ├── authority-management/
│   │   │   │   ├── language/
│   │   │   │   ├── log/
│   │   │   │   ├── panelAdmin/
│   │   │   │   ├── paramaters/
│   │   │   │   ├── policy/
│   │   │   │   └── system-parameter/
│   │   │   ├── enums/
│   │   │   │   ├── authentication/
│   │   │   │   │   └── panel-admin-user-type.enum.ts
│   │   │   │   └── mediaFile/
│   │   │   │       ├── file-extention.enum.ts
│   │   │   │       ├── file-media-type.enum.ts
│   │   │   │       ├── file-type.enum.ts
│   │   │   │       └── picture-type.enum.ts
│   │   │   ├── models/
│   │   │   │   ├── apiexception/
│   │   │   │   ├── authentication/
│   │   │   │   ├── authority/
│   │   │   │   ├── contentLanguage/
│   │   │   │   ├── countryCityAndTown/
│   │   │   │   ├── mediaFile/
│   │   │   │   ├── panelAdmin/
│   │   │   │   ├── panelAdminType/
│   │   │   │   ├── parameter/
│   │   │   │   ├── parameterGroup/
│   │   │   │   ├── parameterValue/
│   │   │   │   ├── policy/
│   │   │   │   └── requestResponse/
│   │   │   ├── pipes/
│   │   │   ├── services/
│   │   │   │   ├── adminUserTypeAuth/
│   │   │   │   ├── authority/
│   │   │   │   ├── contentLanguage/
│   │   │   │   ├── countryCityAndTown/
│   │   │   │   ├── panelAdmin/
│   │   │   │   └── ...
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
| `core/` | Teknik altyapı — sarj_ev_panel_angular ile neredeyse özdeş |
| `sarjAllPro/` | Domain-specific feature modülü; daha az component kategorisi, daha detaylı model/enum yapısı |
| `shared_admin/` | Metronic layout sistemi — sarj_ev_panel_angular ile özdeş |
| `environments/` | 4 ortam: production, development, local, test |

## Feature Modülü (sarjAllPro) Kategorileri

- **Kimlik / Yetkilendirme:** auth, authority-management
- **Yönetim:** panelAdmin, system-parameter
- **Yapılandırma:** paramaters, language
- **İçerik:** policy
- **İzleme:** log

## Model Katmanı Detayı

sarjAllPro daha kapsamlı bir model katmanına sahiptir:

| Model Grubu | Açıklama |
|---|---|
| `apiexception/` | API hata modelleri |
| `authentication/` | Kimlik doğrulama modelleri |
| `authority/` | Yetki modelleri |
| `contentLanguage/` | Dil/içerik modelleri |
| `countryCityAndTown/` | Coğrafi lokasyon modelleri |
| `mediaFile/` | Dosya/medya modelleri |
| `panelAdmin/` | Panel yönetici modelleri |
| `panelAdminType/` | Yönetici tipi modelleri |
| `parameter/` | Parametre modelleri |
| `parameterGroup/` | Parametre grubu modelleri |
| `parameterValue/` | Parametre değer modelleri |
| `policy/` | Politika modelleri |
| `requestResponse/` | API istek/cevap sarmalayıcıları |

## Enum Katmanı Detayı

| Enum Grubu | Değerler |
|---|---|
| `authentication/panel-admin-user-type` | Panel yönetici kullanıcı tipleri |
| `mediaFile/file-extention` | Dosya uzantıları |
| `mediaFile/file-media-type` | Medya tipleri |
| `mediaFile/file-type` | Dosya tipleri |
| `mediaFile/picture-type` | Resim tipleri |

## sarj_ev_panel_angular ile Farklar

| Özellik | sarj_ev_panel_angular | sarj_pro_panel_angular |
|---|---|---|
| Feature modülü | evtech/ (28 kategori) | sarjAllPro/ (8 kategori) |
| Model granülaritesi | Daha az klasör | 13 model klasörü, ayrıntılı |
| Enum grubu | — | 5 enum dosyası, 2 kategori |
| Environment sayısı | 4 | 4 |
| Paket bağımlılıkları | Özdeş | Özdeş |
