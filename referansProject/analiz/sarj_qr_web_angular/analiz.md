# sarj_qr_web_angular — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | Angular 13 |
| UI Kütüphanesi | Angular Material 11 |
| Realtime | @microsoft/signalr |
| PDF Export | jsPDF + html2canvas |
| Excel Export | xlsx |
| Animasyon | lottie-web |
| Görsel Kırpma | ngx-image-cropper |
| Para Formatı | ng2-currency-mask |
| Routing | Feature modules, Guard-based |

## Genel Bakış

`sarj_qr_web_angular` (rotawattqrweb), EV şarj kullanıcı portalı ve QR-based web uygulamasıdır. Kullanıcılar QR kod tarayarak şarj başlatır, şarj oturumunu real-time izler, fatura ve işlem geçmişini görebilir. Özel UI component seti içerir.

## Mimari Pattern

**Core/Modules ayrımı + Feature Modules + Guard-based Routing**

```
src/app/
├── core/
│   ├── models/         — Domain modelleri
│   ├── enums/          — Enum tanımları
│   ├── services/       — API servisleri
│   └── guards/         — Route guard'lar
└── modules/
    ├── auth/           — Kimlik doğrulama modülü
    ├── charge/         — Şarj işlemleri
    ├── billing/        — Fatura yönetimi
    ├── invoice/        — Fatura belgesi
    ├── user/           — Kullanıcı profili
    ├── transaction/    — İşlem geçmişi
    └── shared/         — Ortak UI component'lar (custom UI)
```

## Core Katmanı

### Models
Her domain için ayrı model klasörü:
```
core/models/
├── Auth/           — Login, Register, Token modelleri
├── Billing/        — Ödeme, kart bilgileri modelleri
├── Charge/         — Şarj oturumu, cihaz modelleri
├── Invoice/        — Fatura modelleri
├── User/           — Kullanıcı profil modelleri
└── Transaction/    — İşlem modelleri
```

### Enums
```
core/enums/
├── ChargeAmountType    — Şarj miktarı tipi (TL veya kWh bazlı)
├── ChargeState         — Şarj durumu (Idle, Charging, Finished, Error)
├── SessionState        — Oturum durumu
└── TransactionStatus   — İşlem durumu (Pending, Completed, Failed, Refunded)
```

### Guards
Guard-based routing ile korumalı sayfalar:
- `AuthGuard` — giriş yapılmamışsa login'e yönlendir
- `NoAuthGuard` — giriş yapılmışsa dashboard'a yönlendir (login sayfasına erişimi engelle)

## Özel UI Component Seti

`rotawatt-*` prefix'li özel UI component'lar:

| Component | Açıklama |
|---|---|
| `rotawatt-button` | Özelleştirilmiş button (loading state, variant) |
| `rotawatt-input` | Form input wrapper (validasyon, error display) |
| `rotawatt-select` | Dropdown select component |
| `rotawatt-datetime` | Tarih-saat seçici |
| `rotawatt-data-table` | Sayfalama ve sıralama destekli tablo |

Bu component'lar Angular Material üzerine inşa edilmiş veya bağımsız olabilir. Tutarlı UX için tüm sayfalar bu set'i kullanır.

## Real-time Şarj Takibi (SignalR)

`@microsoft/signalr` ile WebSocket bağlantısı kurularak:
- Şarj oturumu durumu anlık güncellenir
- kWh tüketimi canlı gösterilir
- Bağlantı kopuklukları otomatik reconnect

```typescript
// SignalR hub bağlantısı
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${environment.signalrUrl}/chargeHub`, {
    accessTokenFactory: () => this.authService.getToken()
  })
  .withAutomaticReconnect()
  .build();

connection.on('ChargeStatusUpdated', (data: ChargeStatusDto) => {
  this.chargeStatus$.next(data);
});
```

## PDF Export (jsPDF + html2canvas)

Fatura ve işlem raporu PDF olarak export edilebilir:
- `html2canvas` ile HTML elementini canvas'a dönüştür
- `jsPDF` ile canvas'ı PDF'e çevir
- Kullanıcıya indirme teklif et

Bu yaklaşım şablon esnekliği sağlar: HTML/CSS ile tasarlanan fatura birebir PDF'e aktarılır.

## Excel Export (xlsx)

İşlem geçmişi Excel formatında dışa aktarılabilir. `xlsx` paketi browser-side çalışır, backend gerektirmez.

## Lottie Animasyon

`lottie-web` ile JSON tabanlı animasyonlar:
- Şarj başlatma/tamamlanma animasyonları
- Yükleme ekranı animasyonu
- Boş state animasyonları

## Görsel Kırpma (ngx-image-cropper)

Kullanıcı profil fotoğrafı yükleme akışında görsel kırpma:
1. Kullanıcı görsel seçer
2. ngx-image-cropper ile kırpma / yeniden boyutlandırma
3. Base64 veya blob olarak API'ye gönderim

## Para Formatı (ng2-currency-mask)

Ödeme formlarında TL formatı otomatik uygulanır: `₺1.234,56` görünümü, gerçek değer `1234.56`.

## QR Kod Akışı

Kullanıcı şarj istasyonundaki QR kodu tarar → URL'de station ID + connector ID → web uygulaması şarj başlatma akışını tetikler. Deep link veya query parameter ile cihaz bilgisi aktarılır.

## Dikkat Çeken Noktalar

### Olumlu
- Özel UI component seti tutarlı UX sağlar
- SignalR ile real-time gerçekten faydalı (şarj takibi kritik)
- PDF/Excel export browser-side, backend yük azaltır
- Enum-based state management okunabilir

### İyileştirme Alanları
- Angular 13 → 17+ upgrade öncelikli
- `html2canvas` + `jsPDF` yaklaşımı CSS desteği kısıtlı; modern PDF için server-side render veya dedicated PDF library tercih edilebilir
- lottie-web bundle size artırır; lazy load edilmeli

## Sonuç

Bu proje, EV şarj kullanıcı portal uygulamasıdır. Real-time SignalR entegrasyonu, özel UI component seti ve PDF/Excel export özellikleri üretim kalitesinde implementasyon gösteriyor. Angular 13 ile geliştirilmiş; upgrade ve bundle optimizasyonu iyileştirme fırsatları sunuyor.
