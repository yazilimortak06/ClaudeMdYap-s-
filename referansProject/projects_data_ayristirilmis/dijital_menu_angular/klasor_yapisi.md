# dijital_menu_angular — Klasör Yapısı

## Genel Bilgi
- Framework: Angular 11.2.14
- Tür: Public restoran listeleme / menü websitesi
- Mimari: Template-based, Single Module (eager loading)

---

## Tam Dizin Ağacı

```
dijital_menu_angular/
├── src/
│   ├── app/
│   │   ├── pages/
│   │   │   ├── about/
│   │   │   ├── addrestaurant/
│   │   │   ├── blog/
│   │   │   ├── blogdetails/
│   │   │   ├── blogstyle2/
│   │   │   ├── checkout/
│   │   │   ├── contact/
│   │   │   ├── error-page/
│   │   │   ├── exdeals/
│   │   │   ├── geolocator/
│   │   │   ├── home/
│   │   │   ├── homepage1/
│   │   │   ├── homepage2/
│   │   │   ├── homepage3/
│   │   │   ├── homepage4/
│   │   │   ├── listview/
│   │   │   ├── login/
│   │   │   ├── orderdetails/
│   │   │   ├── register/
│   │   │   ├── restaurant/
│   │   │   ├── restaurantstyle1/
│   │   │   └── restaurantstyle2/
│   │   ├── layouts/
│   │   │   ├── advertisementbanner1/
│   │   │   ├── advertisementbanner2/
│   │   │   ├── advertisementbanner3/
│   │   │   ├── blogleftsidebar/
│   │   │   ├── blogrightsidebar/
│   │   │   ├── footer1/
│   │   │   ├── footer2/
│   │   │   ├── navbar/
│   │   │   ├── restaurantleftsidebar/
│   │   │   └── restaurantrightsidebar/
│   │   ├── models/
│   │   │   └── contact/
│   │   │       └── contact.ts
│   │   ├── helper/
│   │   │   └── contact/
│   │   │       └── contact-helper.service.ts
│   │   ├── app-routing.module.ts
│   │   └── app.module.ts
│   ├── assets/
│   │   ├── css/
│   │   ├── js/
│   │   └── images/
│   └── environments/
│       ├── environment.ts
│       └── environment.prod.ts
├── angular.json
├── package.json
├── tsconfig.json
└── tsconfig.app.json
```

---

## Klasör Açıklamaları

| Klasör | Açıklama |
|--------|----------|
| `pages/` | Sayfa bileşenleri — her route için ayrı klasör |
| `layouts/` | Tekrar kullanılan layout bileşenleri (header, footer, sidebar) |
| `models/` | TypeScript interface/model tanımları |
| `helper/` | Yardımcı servisler (emailJS entegrasyonu vb.) |
| `assets/` | Statik dosyalar (CSS, JS, görseller) |
| `environments/` | Ortam değişkenleri (dev/prod) |

---

## Mimari Notlar

- **Tek AppModule**: Tüm bileşenler tek module'de tanımlı, lazy loading yok
- **Eager Loading**: Tüm routes doğrudan component'e bağlı
- **Pages + Layouts Ayrımı**: Sayfa = içerik, Layout = tekrar eden yapı
- **Public Site**: Authentication guard yok
- **emailjs-com**: Backend olmadan form gönderimi
- **ng-recaptcha**: Form koruması

---

## Bağımlılıklar Özeti

| Paket | Versiyon | Amaç |
|-------|----------|------|
| @angular/core | 11.2.14 | Temel framework |
| @angular/material | ^9.2.4 | UI bileşenleri |
| @ng-bootstrap/ng-bootstrap | ^10.0.0 | Bootstrap entegrasyonu |
| emailjs-com | ^2.4.1 | Email gönderimi (backend'siz) |
| ng-recaptcha | ^8.0.1 | Form koruması |
