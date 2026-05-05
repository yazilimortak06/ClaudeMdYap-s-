# sarj_qr_web_angular — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
Angular feature-module tabanlı portal / kullanıcı uygulaması geliştirirken referans olarak kullanılabilir.

---

## 1. Core/Modules Mimari Ayrımı

**Kural:** Uygulama `core/` ve `modules/` olmak üzere iki ana bölüme ayrılsın:

```
src/app/
├── core/           — Teknik altyapı (model, enum, service, guard)
│   ├── models/
│   ├── enums/
│   ├── services/
│   └── guards/
└── modules/        — Feature modüller (her iş birimi ayrı modül)
    ├── auth/
    ├── charge/
    ├── billing/
    └── shared/
```

**Kural:** `core/` başka hiçbir feature module'e bağımlı olmamalı. Sadece Angular framework ve temel paketlere bağımlı.

**Kural:** Feature module'ler birbirinden bağımsız olsun. Bir module'ün bir diğerini import etmesi yerine `shared/` module kullan.

---

## 2. Model Organizasyonu

**Kural:** Domain model'leri kategorilere göre `core/models/` içinde alt klasörlere ayır:
```
core/models/
├── Auth/
│   ├── login-request.model.ts
│   ├── register-request.model.ts
│   └── token-response.model.ts
├── Charge/
│   ├── charge-session.model.ts
│   └── charge-status.model.ts
└── Transaction/
    └── transaction.model.ts
```

**Kural:** Her model dosyası tek bir interface veya class içersin. Bir dosyada birden fazla model karmaşıklık yaratır.

**Kural:** API yanıt modelleri ile component modelleri ayrı tutulabilir. API'den gelen `ChargeSessionResponseDto` ile component'ın kullandığı `ChargeSessionViewModel` farklı olabilir.

---

## 3. Enum Yönetimi

**Kural:** State ve tip bilgilerini string literal yerine enum ile yönet:

```typescript
export enum ChargeState {
  Idle = 'IDLE',
  Preparing = 'PREPARING',
  Charging = 'CHARGING',
  SuspendedEV = 'SUSPENDED_EV',
  Finishing = 'FINISHING',
  Faulted = 'FAULTED'
}

export enum TransactionStatus {
  Pending = 'PENDING',
  Completed = 'COMPLETED',
  Failed = 'FAILED',
  Refunded = 'REFUNDED'
}
```

**Kural:** Enum değerlerini backend ile senkronize tut. Backend'deki sabit değer değişirse frontend enum da güncellenmeli.

**Kural:** Template'de enum değerleriyle karşılaştırma için enum'u component'a import et:
```typescript
// component.ts
ChargeState = ChargeState;  // template'de kullanmak için

// template
<span *ngIf="session.state === ChargeState.Charging">Şarj Devam Ediyor</span>
```

---

## 4. Özel UI Component Seti

**Kural:** Projede tutarlı UI için ortak input, button, select ve tablo component'larını `modules/shared/components/` altında oluştur. Tüm sayfalar bu component'ları kullansın, doğrudan Material veya HTML element kullanmasın.

**Kural:** Özel component'lar için tutarlı prefix kullan (örn: `app-`, `rotawatt-`, `rw-`):
```
rw-button         — Loading state, variant (primary/secondary/danger)
rw-input          — Error mesajı, label, hint text
rw-select         — Option grupları, search, async loading
rw-data-table     — Sayfalama, sıralama, kolon konfigürasyonu
rw-datetime       — Tarih ve saat seçici
```

**Kural:** Button component'ı loading state yönetsin:
```typescript
@Component({ selector: 'rw-button' })
export class RwButtonComponent {
  @Input() loading = false;
  @Input() disabled = false;
  @Input() variant: 'primary' | 'secondary' | 'danger' = 'primary';
  @Output() clicked = new EventEmitter<void>();
}
```

**Kural:** Input component'ı ReactiveForm ile entegre çalışsın (`ControlValueAccessor` implement etsin).

---

## 5. SignalR Real-time Entegrasyonu

**Kural:** SignalR servisini singleton olarak `CoreModule`'de sağla. Her component yeni bağlantı açmamalı.

**Kural:** SignalR servis şablonu:
```typescript
@Injectable({ providedIn: 'root' })
export class ChargeHubService {
  private connection: signalR.HubConnection;
  private chargeStatus$ = new BehaviorSubject<ChargeStatusDto | null>(null);

  constructor(private authService: AuthService) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.signalrUrl}/chargeHub`, {
        accessTokenFactory: () => this.authService.getToken()
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build();
  }

  connect(): Observable<void> {
    return from(this.connection.start());
  }

  disconnect(): Promise<void> {
    return this.connection.stop();
  }

  onChargeStatusUpdated(): Observable<ChargeStatusDto> {
    return new Observable(observer => {
      this.connection.on('ChargeStatusUpdated', (data) => observer.next(data));
    });
  }
}
```

**Kural:** `withAutomaticReconnect([0, 2000, 5000, 10000, 30000])` ile exponential backoff retry pattern kullan.

**Kural:** SignalR bağlantısını component `ngOnDestroy`'da sonlandır:
```typescript
ngOnDestroy() {
  this.chargeHubService.disconnect();
}
```

---

## 6. PDF Export Pattern

**Kural:** PDF export için `html2canvas` + `jsPDF` kombinasyonu:
```typescript
async exportToPdf(elementId: string, filename: string) {
  const element = document.getElementById(elementId);
  const canvas = await html2canvas(element, { scale: 2, useCORS: true });
  const imgData = canvas.toDataURL('image/png');
  const pdf = new jsPDF('p', 'mm', 'a4');
  const width = pdf.internal.pageSize.getWidth();
  const height = (canvas.height * width) / canvas.width;
  pdf.addImage(imgData, 'PNG', 0, 0, width, height);
  pdf.save(`${filename}.pdf`);
}
```

**Kural:** PDF olarak export edilecek element için ayrı component oluştur. Print/PDF görünümünü screen görünümünden ayır:
```css
.pdf-container {
  display: none;  /* normalde gizli */
}
.pdf-container.exporting {
  display: block;  /* export sırasında göster */
}
```

**Kural:** `html2canvas` web fontlarını doğru render etmeyebilir. PDF için system font veya embedded font kullan.

---

## 7. Excel Export Pattern

**Kural:** Liste verisini Excel'e export etmek için `xlsx` paketi:
```typescript
exportToExcel(data: Transaction[], filename: string) {
  const worksheet = XLSX.utils.json_to_sheet(
    data.map(t => ({
      'Tarih': this.datePipe.transform(t.date, 'dd.MM.yyyy HH:mm'),
      'İstasyon': t.stationName,
      'Tutar (₺)': t.amount,
      'kWh': t.energyConsumed,
      'Durum': t.status
    }))
  );
  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, 'İşlemler');
  XLSX.writeFile(workbook, `${filename}.xlsx`);
}
```

**Kural:** Export edilen kolon adları kullanıcı dostu Türkçe olsun. API field adları (camelCase) değil.

---

## 8. QR Kod Deep Link Akışı

**Kural:** QR kod URL yapısı:
```
https://app.domain.com/charge?stationId=X&connectorId=Y
```

**Kural:** Charge modülü, aktif route'un query parameter'larını okuyarak şarj akışını başlatsın:
```typescript
ngOnInit() {
  this.route.queryParams.subscribe(params => {
    this.stationId = params['stationId'];
    this.connectorId = params['connectorId'];
    if (this.stationId && this.connectorId) {
      this.loadStationInfo();
    }
  });
}
```

**Kural:** Kullanıcı giriş yapmamışsa `AuthGuard` query param'ları koruyarak login'e yönlendirsin (`returnUrl` ile).

---

## 9. Para Formatı

**Kural:** Ödeme ve fiyat input'larında `ng2-currency-mask` kullan:
```html
<input currencyMask
       [options]="{ prefix: '₺', thousands: '.', decimal: ',', precision: 2 }"
       formControlName="amount">
```

**Kural:** Form value'yu currency mask'siz al. Component'ta `amount.value` direkt sayısal değer döner; görsel format sadece input'ta uygulanır.

---

## 10. Lottie Animasyon

**Kural:** `lottie-web` animasyonlarını lazy load et. İlk yükleme performansını etkilememesi için:
```typescript
// Animasyon component'ı lazy import ile yükle
const LottieModule = await import('ngx-lottie');
```

**Kural:** Animasyon JSON dosyalarını `assets/animations/` klasöründe sakla ve `ng build` ile bundle'a dahil et.

**Kural:** Loading ve empty state için lottie animasyonu iyi deneyim sağlar. Her feature modülü kendi animasyonlarını lazy load edebilir.
