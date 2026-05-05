# sarj_qr_web_angular — Klasör Yapısı

## Genel Bilgi
- Framework: Angular 13.1.2
- Tür: EV şarj kullanıcı portal / QR uygulaması
- Mimari: Core/Modules ayrımı, Feature Modules, Guard-based auth

---

## Tam Dizin Ağacı

```
sarj_qr_web_angular/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/
│   │   │   │   ├── auth/
│   │   │   │   │   └── auth.model.ts
│   │   │   │   ├── billing/
│   │   │   │   │   └── billing.model.ts
│   │   │   │   ├── charge/
│   │   │   │   │   └── charge.model.ts
│   │   │   │   ├── invoice/
│   │   │   │   │   └── invoice.model.ts
│   │   │   │   ├── user/
│   │   │   │   │   └── user.model.ts
│   │   │   │   └── transaction/
│   │   │   │       └── transaction.model.ts
│   │   │   ├── enums/
│   │   │   │   ├── charge-amount-type.enum.ts
│   │   │   │   ├── charge-state.enum.ts
│   │   │   │   ├── session-state.enum.ts
│   │   │   │   └── transaction-status.enum.ts
│   │   │   ├── services/
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── charge.service.ts
│   │   │   │   ├── invoice.service.ts
│   │   │   │   └── user.service.ts
│   │   │   └── guards/
│   │   │       └── auth.guard.ts
│   │   ├── modules/
│   │   │   ├── charge/
│   │   │   │   ├── charge.module.ts
│   │   │   │   └── components/
│   │   │   ├── refund/
│   │   │   │   ├── refund.module.ts
│   │   │   │   └── components/
│   │   │   ├── transaction/
│   │   │   │   ├── transaction.module.ts
│   │   │   │   └── components/
│   │   │   └── support-ticket/
│   │   │       ├── support-ticket.module.ts
│   │   │       └── components/
│   │   ├── shared/
│   │   │   ├── components/
│   │   │   │   ├── rotawatt-button/
│   │   │   │   │   └── rotawatt-button.component.ts
│   │   │   │   ├── rotawatt-input/
│   │   │   │   │   └── rotawatt-input.component.ts
│   │   │   │   ├── rotawatt-select/
│   │   │   │   │   └── rotawatt-select.component.ts
│   │   │   │   ├── rotawatt-datetime/
│   │   │   │   │   └── rotawatt-datetime.component.ts
│   │   │   │   └── rotawatt-data-table/
│   │   │   │       └── rotawatt-data-table.component.ts
│   │   │   └── shared.module.ts
│   │   ├── app-routing.module.ts
│   │   ├── app.module.ts
│   │   └── app.component.ts
│   ├── assets/
│   │   ├── i18n/
│   │   │   ├── tr.json
│   │   │   └── en.json
│   │   └── images/
│   └── environments/
│       ├── environment.ts
│       └── environment.prod.ts
├── docs/
│   └── (dokümantasyon dosyaları)
├── angular.json
├── package.json
└── tsconfig.json
```

---

## Klasör Açıklamaları

| Klasör | Açıklama |
|--------|----------|
| `core/` | Uygulama geneli tekil yapılar (model, enum, service, guard) |
| `core/models/` | Domain model tanımları |
| `core/enums/` | Durum enum'ları (state machine benzeri) |
| `core/services/` | HTTP/business servisler |
| `core/guards/` | Route guard'ları (auth koruması) |
| `modules/` | Feature module'leri (lazy loading için hazır yapı) |
| `shared/` | Özel UI bileşen kütüphanesi |
| `assets/i18n/` | Çok dilli destek (ngx-translate) |
| `docs/` | Proje dokümantasyonu |

---

## Shared UI Bileşenleri

| Bileşen | Amaç |
|---------|------|
| `rotawatt-button` | Özel buton bileşeni |
| `rotawatt-input` | Form input wrapper |
| `rotawatt-select` | Dropdown select |
| `rotawatt-datetime` | Tarih/saat seçici |
| `rotawatt-data-table` | Veri tablosu (sort, filter, pagination) |

---

## Bağımlılıklar Özeti

| Paket | Versiyon | Amaç |
|-------|----------|------|
| @angular/core | 13.1.2 | Temel framework |
| @angular/material | ^11.0.0 | Material UI |
| @microsoft/signalr | latest | Real-time WebSocket |
| apexcharts | latest | Grafik/chart |
| jspdf | latest | PDF export |
| html2canvas | latest | HTML→canvas (PDF için) |
| moment | latest | Tarih işlemleri |
| @ngx-translate/core | latest | Çok dilli destek |
| ng2-currency-mask | latest | Para birimi input |
| ngx-image-cropper | latest | Görsel kırpma |
| ngx-image-zoom | latest | Görsel zoom |
| xlsx | latest | Excel export |
| lottie-web | latest | Lottie animasyonları |
