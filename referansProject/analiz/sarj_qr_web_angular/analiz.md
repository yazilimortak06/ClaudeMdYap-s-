# sarj_qr_web_angular — Kitap Kalitesinde Analiz

**Kaynak:** `E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\`

---

## 1. Platform & Tech Stack

| Özellik | Değer |
|---|---|
| Framework | Angular 13.1.2 |
| Dil | TypeScript 4.5.4 |
| UI Kütüphanesi | Angular Material 11, Bootstrap 4.6.1, ng-bootstrap 8 |
| HTTP | HttpClient + @auth0/angular-jwt 5 |
| Gerçek Zamanlı | @microsoft/signalr 6.0.1 (planlı, şu an mock) |
| PDF Export | jspdf 4.0.0 + html2canvas 1.4.1 |
| Excel | xlsx 0.18.5 |
| Animasyon | lottie-web 5.13.0 (direkt, ngx-lottie değil) |
| Çevirme | @ngx-translate/core 13 |
| Grafik | apexcharts + ng-apexcharts, chart.js 4 |
| Para Maskesi | ng2-currency-mask 13 |
| Tarih | moment 2 + @angular/material-moment-adapter |
| Hash | ts-md5 |
| Routing | Hash tabanlı (useHash: true) |
| Build | Angular CLI 13.1.3 |

---

## 2. Klasör Yapısı (Tam Ağaç)

```
src/
└── app/
    ├── app.component.{ts,html,scss}
    ├── app.module.ts
    ├── app-routing.module.ts
    │
    ├── core/
    │   ├── adapters/
    │   │   └── MOMENT_DATE_FORMATS.ts
    │   ├── bases/
    │   │   ├── base-datatable/
    │   │   │   ├── base-datatable-base-model.ts
    │   │   │   └── base-datatable.ts
    │   │   ├── base-tree/
    │   │   │   └── tree-contro.base.ts
    │   │   └── base-tree-table/
    │   │       ├── tree-control-datatable-base.ts
    │   │       ├── tree-control-table-data-interface.ts
    │   │       └── tree-control-table-filter-model.ts
    │   ├── configs/
    │   │   └── custom-currency-mask.config.ts
    │   ├── core.module.ts
    │   ├── date-core/
    │   │   ├── mat-date.module.ts
    │   │   └── MOMENT_DATE_FORMATS.ts
    │   ├── directives/
    │   │   ├── cell-template.directive.ts
    │   │   ├── date-time.directive.ts
    │   │   ├── label-control.directive.ts
    │   │   ├── only-number.directive.ts
    │   │   ├── only-text-or-number.directive.ts
    │   │   ├── only-text.directive.ts
    │   │   └── two-digit-decimal-number.directive.ts
    │   ├── enums/
    │   │   ├── index.ts                        ← barrel export
    │   │   ├── charge-amount-type.enum.ts
    │   │   ├── charge-state.enum.ts
    │   │   ├── charge-status-type.enum.ts
    │   │   ├── invoice-status.enum.ts
    │   │   ├── login-type.enum.ts
    │   │   ├── notification-type.enum.ts
    │   │   ├── refund-status.enum.ts
    │   │   ├── server-result-type.enum.ts
    │   │   ├── session-state.enum.ts
    │   │   ├── socket-status-type.enum.ts
    │   │   ├── sort-field.enum.ts
    │   │   ├── start-charge-type.enum.ts
    │   │   ├── support-ticket-filter.enum.ts
    │   │   ├── ticket-status.enum.ts
    │   │   ├── transaction-status.enum.ts
    │   │   ├── user-invoice-filter.enum.ts
    │   │   ├── user-refund-filter.enum.ts
    │   │   └── user-transaction-filter.enum.ts
    │   ├── external-components/
    │   │   ├── dropzone-shared.module.ts
    │   │   ├── html-editor-shared.module.ts
    │   │   ├── enums/
    │   │   │   └── file-dropzone.enum.ts
    │   │   ├── models/
    │   │   │   └── file-dropzone-model.ts
    │   │   ├── pixdinn-dropzone/
    │   │   │   ├── image-cropper-modal/
    │   │   │   ├── pixdinn-dropzone-preview/
    │   │   │   └── pixdinn-dropzone.component.{ts,html,scss}
    │   │   ├── pixdinn-html-editor/
    │   │   ├── rotawatt-button/
    │   │   ├── rotawatt-data-table/
    │   │   ├── rotawatt-datetime/
    │   │   ├── rotawatt-input/
    │   │   ├── rotawatt-select/
    │   │   └── rotawatt-year-month-picker/
    │   ├── guards/
    │   │   └── auth.guard.ts
    │   ├── i18n/
    │   │   └── strings.ts
    │   ├── models/
    │   │   ├── auth/           ← AuthToken, Login/SMS DTOs
    │   │   ├── billing/        ← UserInvoiceItem
    │   │   ├── charge/         ← ChargeStatus, Socket, Start/Stop DTOs
    │   │   ├── device/         ← DeviceHeader, FirstRequest (eski)
    │   │   ├── notification/   ← UserNotificationItem
    │   │   ├── security/       ← FirstRequest (yeni), QrPayload, DeviceInfo, ActiveProcessingSession
    │   │   ├── session/        ← PrepareLogin, SocketList, PriceKw, ChargeProcessing DTOs
    │   │   ├── support/        ← SupportTicketItem, TechnicalSupportMessage
    │   │   └── transaction/    ← UserTransactionItem
    │   └── services/
    │       ├── core-utils.service.ts
    │       ├── api/
    │       │   ├── auth-api.service.ts
    │       │   ├── charge-api.service.ts
    │       │   ├── device-api.service.ts
    │       │   └── session-api.service.ts
    │       └── domain/
    │           ├── auth-storage.service.ts
    │           ├── charge-session.service.ts
    │           ├── core-security.service.ts
    │           ├── device-header.service.ts
    │           ├── device-info-collector.service.ts
    │           └── qr-web-security.service.ts
    │
    └── features/
        ├── auth/
        │   ├── auth.module.ts
        │   └── login/
        │       └── login.component.ts
        ├── billing-address/
        │   ├── billing-address.module.ts
        │   └── billing-address.component.ts
        ├── charge/
        │   ├── charge.module.ts               ← startCharge rotası
        │   ├── start-charge/
        │   │   └── start-charge.component.ts  ← QR parse + FirstRequest
        │   ├── socket-list/
        │   │   ├── socket-list.module.ts
        │   │   └── socket-list.component.ts
        │   ├── charging-status/
        │   │   ├── charging-status.module.ts
        │   │   └── charging-status.component.ts ← Lottie + SignalR (mock)
        │   ├── charge-result/
        │   │   ├── charge-result.module.ts
        │   │   └── charge-result.component.ts
        │   └── charge-summary/
        │       ├── charge-summary.module.ts
        │       └── charge-summary.component.ts
        ├── home/
        │   ├── home.module.ts
        │   └── home.component.ts
        ├── notifications/
        │   ├── notifications.module.ts
        │   ├── notifications.component.ts
        │   └── notification-detail/
        ├── payment/
        │   ├── payment.module.ts
        │   └── payment.component.ts
        ├── profile/
        │   ├── profile.module.ts
        │   └── profile.component.ts
        ├── refunds/
        │   ├── refunds.module.ts
        │   └── refunds.component.ts
        ├── saved-cards/
        │   ├── saved-cards.module.ts
        │   └── saved-cards.component.ts
        ├── support/
        │   ├── support.module.ts
        │   ├── support.component.ts
        │   ├── new-ticket/
        │   └── ticket-detail/
        └── transactions/
            ├── transactions.module.ts
            └── transactions.component.ts
```

---

## 3. app.module.ts + routing (Tam Kod)

### app.module.ts

```typescript
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtModule } from '@auth0/angular-jwt';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthStorageService } from './core/services/domain/auth-storage.service';

export function tokenGetter() {
  return localStorage.getItem('auth_token');
}

function appInitializer(authStorageService: AuthStorageService) {
  return () =>
    new Promise<void>((resolve) => {
      authStorageService.restoreFromStorage();
      resolve();
    });
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    JwtModule.forRoot({ config: { tokenGetter } }),
    AppRoutingModule,
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      multi: true,
      deps: [AuthStorageService],
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
```

### app-routing.module.ts

```typescript
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Akış: startCharge → login → socketList → payment → chargingStatus → chargeResult → chargeSummary
const routes: Routes = [
  { path: 'startCharge',           loadChildren: () => import('./features/charge/charge.module').then(m => m.ChargeModule) },
  { path: 'startChargeSocketList', loadChildren: () => import('./features/charge/socket-list/socket-list.module').then(m => m.SocketListModule) },
  { path: 'payment',               loadChildren: () => import('./features/payment/payment.module').then(m => m.PaymentModule) },
  { path: 'chargingStatus',        loadChildren: () => import('./features/charge/charging-status/charging-status.module').then(m => m.ChargingStatusModule) },
  { path: 'chargeResult',          loadChildren: () => import('./features/charge/charge-result/charge-result.module').then(m => m.ChargeResultModule) },
  { path: 'chargeSummary',         loadChildren: () => import('./features/charge/charge-summary/charge-summary.module').then(m => m.ChargeSummaryModule) },
  { path: 'login',                 loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthFeatureModule) },
  { path: 'support',               loadChildren: () => import('./features/support/support.module').then(m => m.SupportModule) },
  { path: 'transactions',          loadChildren: () => import('./features/transactions/transactions.module').then(m => m.TransactionsModule) },
  { path: 'savedCards',            loadChildren: () => import('./features/saved-cards/saved-cards.module').then(m => m.SavedCardsModule) },
  { path: 'billingAddress',        loadChildren: () => import('./features/billing-address/billing-address.module').then(m => m.BillingAddressModule) },
  { path: 'refunds',               loadChildren: () => import('./features/refunds/refunds.module').then(m => m.RefundsModule) },
  { path: 'notifications',         loadChildren: () => import('./features/notifications/notifications.module').then(m => m.NotificationsModule) },
  { path: 'profile',               loadChildren: () => import('./features/profile/profile.module').then(m => m.ProfileModule) },
  { path: 'home',                  loadChildren: () => import('./features/home/home.module').then(m => m.HomeModule) },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
```

---

## 4. Core — Models (Tüm Tam Kod)

### models/auth/auth-token.model.ts
```typescript
export interface AuthToken {
  token?: string;
  refreshToken?: string;
  expiresIn?: number;
}
```

### models/auth/login-request.dto.ts
```typescript
import { LoginType } from 'src/app/core/enums';

export interface LoginRequestDto {
  userDevice?: string;
  chargeTempSession?: string;
  loginFormKey?: string;
  loginType?: LoginType;
  phone?: string;
  googleIdToken?: string;
}
```

### models/auth/login-response.dto.ts
```typescript
export interface LoginResponseDto {
  // TODO: Backend ile belirlenecek alanlar
}
```

### models/auth/prepare-login-form-request.dto.ts
```typescript
import { LoginType } from 'src/app/core/enums';

export interface PrepareLoginFormRequestDto {
  userDevice?: string;
  chargeTempSession?: string;
  loginType?: LoginType;
}
```

### models/auth/prepare-login-form-response.dto.ts
```typescript
export interface PrepareLoginFormResponseDto {
  loginFormKey?: string;
}
```

### models/auth/validate-sms-code-request.dto.ts
```typescript
export interface ValidateSmsCodeRequestDto {
  userDevice?: string;
  chargeTempSession?: string;
  loginFormKey?: string;
  phone?: string;
  smsCode?: string;
}
```

### models/auth/validate-sms-code-response.dto.ts
```typescript
export interface ValidateSmsCodeResponseDto {
  token?: string;
  refreshToken?: string;
  expiresIn?: number;
}
```

### models/billing/user-invoice-item.model.ts
```typescript
import { InvoiceStatus } from 'src/app/core/enums';

export interface UserInvoiceItem {
  id?: string;
  date?: string;
  amount?: number;
  status?: InvoiceStatus;
  description?: string;
}
```

### models/charge/charge-status.dto.ts
```typescript
import { ChargeStatusType } from 'src/app/core/enums';

export interface ChargeStatusDto {
  sessionId?: string;
  socketName?: string;
  status?: ChargeStatusType;
  chargePercent?: number;
  chargePower?: number;
  chargeEnergy?: number;
  startTime?: string;
  elapsedMinutes?: number;
  totalPrice?: number;
}
```

### models/charge/check-active-charge-request.dto.ts
```typescript
export interface CheckActiveChargeRequestDto {
  chargeSessionToken?: string;
  deviceHeader?: string;
}
```

### models/charge/check-active-charge-response.dto.ts
```typescript
import { ChargeStatusDto } from './charge-status.dto';

export interface CheckActiveChargeResponseDto {
  hasActiveCharge?: boolean;
  chargeStatus?: ChargeStatusDto;
}
```

### models/charge/saved-card-item.model.ts
```typescript
export interface UserSavedCardItem {
  id?: string;
  cardName?: string;
  cardHolder?: string;
  lastFour?: string;
  brand?: string;
  isDefault?: boolean;
}
```

### models/charge/socket-list-item.dto.ts
```typescript
import { SocketStatusType } from 'src/app/core/enums';

export interface SocketListItemDto {
  socketId?: string;
  socketName?: string;
  socketType?: string;
  power?: number;
  pricePerKwh?: number;
  status?: SocketStatusType;
}
```

### models/charge/start-charge-from-device-request.dto.ts
```typescript
export interface StartChargeFromDeviceRequestDto {
  deviceCode?: string;
  userDevice?: string;
  chargeTempSession?: string;
}
```

### models/charge/start-charge-from-device-response.dto.ts
```typescript
import { SocketListItemDto } from './socket-list-item.dto';

export interface StartChargeFromDeviceResponseDto {
  isMultipleSocket?: boolean;
  tempChargeSession?: string;
  deviceName?: string;
  deviceSerialNo?: string;
  deviceLocation?: string;
  sockets?: SocketListItemDto[];
}
```

### models/charge/stop-charge-request.dto.ts
```typescript
export interface StopChargeRequestDto {
  chargeSessionToken?: string;
  sessionId?: string;
}
```

### models/charge/stop-charge-response.dto.ts
```typescript
import { ChargeStatusDto } from './charge-status.dto';

export interface StopChargeResponseDto {
  success?: boolean;
  finalStatus?: ChargeStatusDto;
}
```

### models/charge/user-refund-item.model.ts
```typescript
import { RefundStatus } from 'src/app/core/enums';

export interface UserRefundItem {
  id?: string;
  date?: string;
  amount?: number;
  reason?: string;
  status?: RefundStatus;
  transactionId?: string;
}
```

### models/device/device-header.model.ts
```typescript
export interface DeviceHeader {
  deviceHeaderId: string;
  createdAt: string;
}
```

### models/notification/user-notification-item.model.ts
```typescript
import { NotificationType } from 'src/app/core/enums';

export interface UserNotificationItem {
  id?: string;
  type?: NotificationType;
  title?: string;
  description?: string;
  date?: string;
  isRead?: boolean;
}
```

### models/security/device-info.model.ts
```typescript
// navigator/screen API'lerinden toplanır, FirstRequest request içinde gönderilir
export interface DeviceInfo {
  userAgent: string;
  screenResolution: string;  // "${screen.width}x${screen.height}"
  language: string;
  platform: string;
  timeZone: string;
}
```

### models/security/active-processing-session.model.ts
```typescript
import { SessionState, ChargeState } from 'src/app/core/enums';

// FirstRequest response içinde döner — resume için hangi adımda kalındığını belirler
// null = aktif oturum yok, dolu = kaldığı yerden devam gerek
export interface ActiveProcessingSession {
  id: number;
  guiId?: string | null;
  state: SessionState;
  startedAt: string;

  // Adım flag'leri
  hasSocketSelected: boolean;
  hasAmountSelected: boolean;
  hasPrePaid: boolean;
  hasChargeStarted: boolean;

  // Soket bilgisi (hasSocketSelected=true ise dolu)
  chargeDeviceConnectorId?: number | null;
  chargeDeviceConnectorName?: string | null;
  socketNumber?: string | null;

  // Tutar/ödeme bilgisi (hasAmountSelected=true ise dolu)
  tempSelectedAmount?: number | null;
  tempSelectedKw?: number | null;
  tempPaidAmount?: number | null;
  tempPaymentDate?: string | null;

  // Şarj bilgisi (hasChargeStarted=true ise dolu)
  chargeId?: number | null;
  chargeState?: ChargeState | null;
}
```

### models/security/first-request-request.dto.ts
```typescript
import { DeviceInfo } from './device-info.model';

// POST /v1/QrWebFirstRequest/FirstRequest — Auth YOK
export interface FirstRequestRequestDto {
  deviceInfo: DeviceInfo;
  localStorageDeviceKey: string;
  versionNumber: number;
  versionKey: string;
  isAuth: boolean;
  encryptedDeviceData: string;
  requestValidtyStr: string;
  devicePublicKey: string;
}
```

### models/security/first-request-response.dto.ts
```typescript
import { ActiveProcessingSession } from './active-processing-session.model';

export interface FirstRequestResponseDto {
  deviceSecurityApiId: number;
  deviceSecurityApiKey: string;
  tempHash: string;
  isLoggedIn: boolean;
  jwtTocken?: string | null;
  tokenVersion?: number | null;
  isUpdated: boolean;
  serverTimestamp: number;
  activeProcessingSession?: ActiveProcessingSession | null;
}
```

### models/security/qr-payload.model.ts
```typescript
import { StartChargeType } from 'src/app/core/enums';

// QR URL: /startCharge?type=device&data=BASE64(StartChargeData)
export interface StartChargeData {
  deviceSerialNo?: string;
  deviceGuid?: string;
  connectorSerialNo?: string;
  connectorGuid?: string;
}

// Sayfalar arası taşıma: base64 encode JSON
export interface QrPayload {
  startCharge: StartChargeData;
}
```

### models/session/connector-item.dto.ts
```typescript
export interface ConnectorItemDto {
  id: number;
  guiId?: string;
  name: string;
  socketNumber: string;
  kW: number;
  price: number;           // KDV hariç birim fiyat (TL/kWh)
  priceWithKdv: number;    // KDV dahil birim fiyat
  state: number;           // ChargeDeviceConnectorStateEnum (1=ACTIVE)
  instantState?: number | null;  // OCPP: 1=AVAILABLE, 3=PREPARING, 4=IN_PROCESS, 5=FAULTED, 6=OCCUPIED
  isAvailableForCharge: boolean;
}
```

### models/session/prepare-login-request.dto.ts
```typescript
import { DeviceInfo } from '../security/device-info.model';

// POST /v1/QrWebLogin/PrepareLoginWithTempRegisterForm
export interface PrepareLoginRequestDto {
  deviceInfo: DeviceInfo;
  localStorageDeviceKey: string;
  deviceSecurityApiKey: string;
  deviceSecurityApiId: number;
  tempHash: string;
  encryptedDeviceData: string;
  requestValidtyStr: string;
  devicePublicKey: string;
}
```

### models/session/prepare-login-response.dto.ts
```typescript
export interface PrepareLoginResponseDto {
  loginSessionHash?: string;
  registerSessionHash?: string;
  rsaPublicKey?: string;
  tempDeviceUpdated?: boolean;
  availableLoginMethods?: number[];  // [1]=PHONE, [2]=GMAIL
  welcomeMessage?: string;
  privacyPolicyUrl?: string;
  termsOfServiceUrl?: string;
  helpContactPhone?: string;
  helpContactEmail?: string;
}
```

### models/session/prepare-socket-list-request.dto.ts
```typescript
// POST /v1/QrWebProcessingSession/PrepareSocketList
export interface PrepareSocketListRequestDto {
  qrWebUserId: number;
  type: number;    // 1=DEVICE
  serialNo: string;
  guid: string;
}
```

### models/session/prepare-socket-list-response.dto.ts
```typescript
import { ConnectorItemDto } from './connector-item.dto';

export interface PrepareSocketListResponseDto {
  chargeDeviceId: number;
  chargeDeviceName: string;
  chargeDeviceGuiId: string;
  stationId?: number | null;
  connectors: ConnectorItemDto[];
}
```

### models/session/prepare-price-kw-request.dto.ts
```typescript
// POST /v1/QrWebProcessingSession/PreparePriceKwSelect
export interface PreparePriceKwRequestDto {
  qrWebUserId: number;
  type?: number | null;
  serialNo?: string | null;
  guid?: string | null;
}
```

### models/session/prepare-price-kw-response.dto.ts
```typescript
import { SessionState, ChargeState } from 'src/app/core/enums';

export interface PreparePriceKwResponseDto {
  id: number;
  guiId?: string | null;
  state: SessionState;
  startedAt: string;
  hasSocketSelected: boolean;
  hasAmountSelected: boolean;
  hasPrePaid: boolean;
  hasChargeStarted: boolean;
  chargeDeviceConnectorId?: number | null;
  chargeDeviceConnectorName?: string | null;
  socketNumber?: string | null;
  connectorKw?: number | null;
  connectorPrice?: number | null;
  connectorPriceWithKdv?: number | null;
  tempSelectedAmount?: number | null;
  tempSelectedKw?: number | null;
  tempPaidAmount?: number | null;
  tempPaymentDate?: string | null;
  chargeId?: number | null;
  chargeState?: ChargeState | null;
}
```

### models/session/select-socket-request.dto.ts
```typescript
// POST /v1/QrWebProcessingSession/SelectSocket
export interface SelectSocketRequestDto {
  qrWebUserId: number;
  connectorId: number;
}
```

### models/session/select-price-kw-request.dto.ts
```typescript
// POST /v1/QrWebProcessingSession/SelectPriceKw
// Kural: selectedAmount XOR selectedKw (biri null olmalı)
export interface SelectPriceKwRequestDto {
  qrWebUserId: number;
  selectedAmount?: number | null;
  selectedKw?: number | null;
}
```

### models/session/charge-processing-request.dto.ts
```typescript
// POST /v1/QrWebProcessingSession/ChargeProcessing
export interface ChargeProcessingRequestDto {
  qrWebUserId: number;
}
```

### models/session/charge-processing-response.dto.ts
```typescript
import { ChargeState } from 'src/app/core/enums';

export interface ChargeProcessingResponseDto {
  chargeId: number;
  chargeState: ChargeState;
  startTime?: string | null;
  endTime?: string | null;
  loadedKw?: number | null;
  calculatedPrice?: number | null;
  chargePercentage?: number | null;
  tempPaidAmount?: number | null;
  estimatedRefundAmount?: number | null;
}
```

### models/support/support-ticket-item.model.ts
```typescript
import { TicketStatus } from 'src/app/core/enums';

export interface SupportTicketItem {
  id?: string;
  subject?: string;
  date?: string;
  status?: TicketStatus;
  rating?: number;
  lastMessage?: string;
}
```

### models/support/technical-support-message.model.ts
```typescript
export interface TechnicalSupportMessage {
  id?: string;
  text?: string;
  date?: string;
  isUser?: boolean;
}
```

### models/transaction/user-transaction-item.model.ts
```typescript
import { TransactionStatus } from 'src/app/core/enums';

export interface UserTransactionItem {
  id?: string;
  requestedAmount?: number;
  requestedType?: 'kwh' | 'tl';
  actualEnergy?: number;
  actualPrice?: number;
  paidAmount?: number;
  refundAmount?: number;
  isRefunded?: boolean;
  location?: string;
  startDate?: string;
  endDate?: string;
  status?: TransactionStatus;
}
```

---

## 5. Core — Enums (Tüm Tam Kod)

### enums/index.ts (barrel)
```typescript
export { LoginType }                from './login-type.enum';
export { SocketStatusType }         from './socket-status-type.enum';
export { ChargeStatusType }         from './charge-status-type.enum';
export { TransactionStatus }        from './transaction-status.enum';
export { TicketStatus }             from './ticket-status.enum';
export { NotificationType }         from './notification-type.enum';
export { InvoiceStatus }            from './invoice-status.enum';
export { RefundStatus }             from './refund-status.enum';
export { ServerResultType }         from './server-result-type.enum';
export { UserTransactionFilter }    from './user-transaction-filter.enum';
export { UserRefundFilter }         from './user-refund-filter.enum';
export { UserInvoiceFilter }        from './user-invoice-filter.enum';
export { SupportTicketFilter }      from './support-ticket-filter.enum';
export { ChargeAmountType }         from './charge-amount-type.enum';
export { SortField, SortDirection } from './sort-field.enum';
export { SessionState }             from './session-state.enum';
export { ChargeState }              from './charge-state.enum';
export { StartChargeType }          from './start-charge-type.enum';
```

### enums/login-type.enum.ts
```typescript
export enum LoginType {
  Phone  = 1,
  Google = 2,
}
```

### enums/start-charge-type.enum.ts
```typescript
// QR kod tipi — cihaz mı soket mi
export enum StartChargeType {
  Device    = 'device',
  Connector = 'connector',
}
```

### enums/socket-status-type.enum.ts
```typescript
export enum SocketStatusType {
  Available    = 1,
  PluggedIn    = 2,
  Occupied     = 3,
  OutOfService = 4,
}
```

### enums/charge-amount-type.enum.ts
```typescript
export enum ChargeAmountType {
  Kwh   = 'kwh',
  Price = 'tl',
}
```

### enums/charge-status-type.enum.ts
```typescript
export enum ChargeStatusType {
  Starting  = 0,
  Charging  = 1,
  Stopping  = 2,
  Completed = 3,
  Error     = 4,
}
```

### enums/charge-state.enum.ts
```typescript
// OCPP tarafındaki state — activeProcessingSession.chargeState
export enum ChargeState {
  ProcessStarting      = 1,
  ProcessStart         = 2,
  ProcessEnding        = 3,
  PaymentBeingReceived = 4,
  PaymentFail          = 5,
  Completed            = 6,
  Failed               = 7,
  Calculating          = 8,
}
```

### enums/session-state.enum.ts
```typescript
// Şarj oturumu durumu — activeProcessingSession.state
export enum SessionState {
  Active        = 1,
  Completed     = 2,
  PaymentFailed = 3,
  ChargeFailed  = 4,
  Canceled      = 5,
}
```

### enums/transaction-status.enum.ts
```typescript
export enum TransactionStatus {
  PaymentProcessing = 1,
  Failed            = 2,
  Completed         = 3,
  Calculating       = 4,
  PaymentFailed     = 5,
}
```

### enums/ticket-status.enum.ts
```typescript
export enum TicketStatus {
  NewMessage = 1,
  Waiting    = 2,
  Completed  = 3,
}
```

### enums/notification-type.enum.ts
```typescript
export enum NotificationType {
  Campaign       = 1,
  Standard       = 2,
  Reply          = 3,
  ChargeComplete = 4,
  Birthday       = 5,
  Maintenance    = 6,
}
```

### enums/invoice-status.enum.ts
```typescript
export enum InvoiceStatus {
  Created    = 1,
  Pending    = 2,
  NotCreated = 3,
}
```

### enums/refund-status.enum.ts
```typescript
export enum RefundStatus {
  Pending   = 1,
  Completed = 2,
  Rejected  = 3,
}
```

### enums/server-result-type.enum.ts
```typescript
export enum ServerResultType {
  Ok              = 'Ok',
  Error           = 'Error',
  ValidationError = 'ValidationError',
  Unauthorized    = 'Unauthorized',
  Exception       = 'Exception',
}
```

### enums/user-transaction-filter.enum.ts
```typescript
export enum UserTransactionFilter {
  All        = 'all',
  Completed  = 'completed',
  Failed     = 'failed',
  Processing = 'processing',
}
```

### enums/user-refund-filter.enum.ts
```typescript
export enum UserRefundFilter {
  All       = 'all',
  Pending   = 'pending',
  Completed = 'completed',
  Rejected  = 'rejected',
}
```

### enums/user-invoice-filter.enum.ts
```typescript
export enum UserInvoiceFilter {
  All        = 'all',
  Created    = 'created',
  Pending    = 'pending',
  NotCreated = 'notCreated',
}
```

### enums/support-ticket-filter.enum.ts
```typescript
export enum SupportTicketFilter {
  All        = 'all',
  NewMessage = 'new',
  Waiting    = 'waiting',
  Completed  = 'completed',
}
```

### enums/sort-field.enum.ts
```typescript
export enum SortField {
  Date   = 'date',
  Amount = 'amount',
}

export enum SortDirection {
  Asc  = 'asc',
  Desc = 'desc',
}
```

---

## 6. Core — Services (Tüm Tam Kod)

### services/api/device-api.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { FirstRequestRequestDto, FirstRequestResponseDto } from 'src/app/core/models/security';

const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

@Injectable({ providedIn: 'root' })
export class DeviceApiService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = (environment.apiUrl || '');
  }

  // POST /v1/QrWebFirstRequest/FirstRequest — Auth YOK, tarayıcı açıldığında ilk çağrılan
  firstRequest(dto: FirstRequestRequestDto): Observable<FirstRequestResponseDto> {
    // Gerçek: return this.http.post<{ data: FirstRequestResponseDto }>(
    //   this.srvUrl + 'v1/QrWebFirstRequest/FirstRequest', dto, httpOptions
    // ).pipe(map(res => res.data));

    console.log('[MOCK] FirstRequest gönderildi:', dto);
    const mockResponse: FirstRequestResponseDto = {
      deviceSecurityApiId:  123,
      deviceSecurityApiKey: 'mock-device-key-' + Date.now(),
      tempHash:             'mock-temp-hash',
      isLoggedIn:           false,
      jwtTocken:            null,
      tokenVersion:         null,
      isUpdated:            true,
      serverTimestamp:      Math.floor(Date.now() / 1000),
      activeProcessingSession: null,
    };
    return of(mockResponse).pipe(delay(1000));
  }
}
```

### services/api/auth-api.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  PrepareLoginFormRequestDto, PrepareLoginFormResponseDto,
  LoginRequestDto, LoginResponseDto,
  ValidateSmsCodeRequestDto, ValidateSmsCodeResponseDto,
} from 'src/app/core/models/auth';
import { StartChargeFromDeviceRequestDto, StartChargeFromDeviceResponseDto } from 'src/app/core/models/charge';
import { SocketStatusType } from 'src/app/core/enums';

const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = (environment.apiUrl || '');
  }

  prepareLoginForm(dto: PrepareLoginFormRequestDto): Observable<PrepareLoginFormResponseDto> {
    console.log('[MOCK] prepareLoginForm:', dto);
    return of({ loginFormKey: 'lfk-' + Date.now() }).pipe(delay(500));
  }

  login(dto: LoginRequestDto): Observable<LoginResponseDto> {
    console.log('[MOCK] login:', dto);
    return of({}).pipe(delay(1000));
  }

  validateSmsCode(dto: ValidateSmsCodeRequestDto): Observable<ValidateSmsCodeResponseDto> {
    console.log('[MOCK] validateSmsCode:', dto);
    return of({
      token:        'mock-jwt-' + Date.now(),
      refreshToken: 'mock-refresh-' + Date.now(),
      expiresIn:    3600,
    }).pipe(delay(1000));
  }

  startChargeFromDevice(dto: StartChargeFromDeviceRequestDto): Observable<StartChargeFromDeviceResponseDto> {
    console.log('[MOCK] startChargeFromDevice:', dto);
    return of({
      isMultipleSocket:  true,
      tempChargeSession: 'tcs-' + Date.now(),
      deviceName:        'Rotawatt DC Fast Charger',
      deviceSerialNo:    'R0026',
      deviceLocation:    'Istanbul, Kadikoy',
      sockets: [
        { socketId: 'skt-1', socketName: 'R0026-1', socketType: 'DC CSS', power: 120, pricePerKwh: 7.79, status: SocketStatusType.Available },
        { socketId: 'skt-2', socketName: 'R0026-2', socketType: 'DC CSS', power: 120, pricePerKwh: 7.79, status: SocketStatusType.PluggedIn },
      ],
    }).pipe(delay(1500));
  }
}
```

### services/api/charge-api.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  CheckActiveChargeRequestDto, CheckActiveChargeResponseDto,
  ChargeStatusDto, StopChargeRequestDto, StopChargeResponseDto,
} from 'src/app/core/models/charge';
import { ChargeStatusType } from 'src/app/core/enums';

const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

@Injectable({ providedIn: 'root' })
export class ChargeApiService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = (environment.stationApiUrl || environment.apiUrl || '') + 'Charge/';
  }

  // POST /Charge/checkActiveCharge
  checkActiveCharge(dto: CheckActiveChargeRequestDto): Observable<CheckActiveChargeResponseDto> {
    console.log('[MOCK] checkActiveCharge:', dto);
    const mockStatus: ChargeStatusDto = {
      sessionId: 'session-mock-123', socketName: 'R0026-1',
      status: ChargeStatusType.Charging, chargePercent: 67,
      chargePower: 19.2, chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59', elapsedMinutes: 20, totalPrice: 44.8,
    };
    return of({ hasActiveCharge: true, chargeStatus: mockStatus }).pipe(delay(1000));
  }

  // POST /Charge/stopCharge
  stopCharge(dto: StopChargeRequestDto): Observable<StopChargeResponseDto> {
    console.log('[MOCK] stopCharge:', dto);
    const finalStatus: ChargeStatusDto = {
      sessionId: dto.sessionId, socketName: 'R0026-1',
      status: ChargeStatusType.Completed, chargePercent: 67,
      chargePower: 0, chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59', elapsedMinutes: 20, totalPrice: 44.8,
    };
    return of({ success: true, finalStatus }).pipe(delay(1500));
  }

  // TODO: SignalR ile değiştirilecek
  getChargeStatus(sessionId: string): Observable<ChargeStatusDto> {
    console.log('[MOCK] getChargeStatus:', sessionId);
    const mock: ChargeStatusDto = {
      sessionId, socketName: 'R0026-1', status: ChargeStatusType.Charging,
      chargePercent: 67, chargePower: 19.2, chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59', elapsedMinutes: 20, totalPrice: 44.8,
    };
    return of(mock).pipe(delay(500));
  }
}
```

### services/api/session-api.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  PrepareLoginRequestDto, PrepareLoginResponseDto,
  PrepareSocketListRequestDto, PrepareSocketListResponseDto,
  PreparePriceKwRequestDto, PreparePriceKwResponseDto,
  ChargeProcessingRequestDto, ChargeProcessingResponseDto,
  SelectSocketRequestDto, SelectPriceKwRequestDto,
} from 'src/app/core/models/session';
import { ChargeState, SessionState } from 'src/app/core/enums';

const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

@Injectable({ providedIn: 'root' })
export class SessionApiService {
  private baseUrl: string;
  private loginUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl  = (environment.apiUrl || '') + 'v1/QrWebProcessingSession/';
    this.loginUrl = (environment.apiUrl || '') + 'v1/QrWebLogin/';
  }

  // 1. PrepareLogin — /v1/QrWebLogin/PrepareLoginWithTempRegisterForm
  prepareLogin(dto: PrepareLoginRequestDto): Observable<PrepareLoginResponseDto> {
    console.log('[MOCK] PrepareLogin:', dto);
    return of({
      loginSessionHash:    'mock-login-hash-' + Date.now(),
      registerSessionHash: 'mock-register-hash',
      rsaPublicKey:        'mock-rsa-key',
      tempDeviceUpdated:   true,
      availableLoginMethods: [1],
      welcomeMessage:      'Hoşgeldiniz',
      privacyPolicyUrl:    'https://rotawatt.com/privacy',
      termsOfServiceUrl:   'https://rotawatt.com/terms',
      helpContactPhone:    '+90 850 000 00 00',
      helpContactEmail:    'destek@rotawatt.com',
    }).pipe(delay(800));
  }

  // 2. PrepareSocketList — /v1/QrWebProcessingSession/PrepareSocketList
  prepareSocketList(dto: PrepareSocketListRequestDto): Observable<PrepareSocketListResponseDto> {
    console.log('[MOCK] PrepareSocketList:', dto);
    return of({
      chargeDeviceId:    123,
      chargeDeviceName:  'Sarj Cihazi A1',
      chargeDeviceGuiId: dto.guid,
      stationId:         45,
      connectors: [
        { id: 999,  guiId: 'conn-guid-1', name: 'Soket A', socketNumber: 'A1',
          kW: 22, price: 8.50, priceWithKdv: 10.03, state: 1, instantState: 1, isAvailableForCharge: true },
        { id: 1000, guiId: 'conn-guid-2', name: 'Soket B', socketNumber: 'B1',
          kW: 50, price: 12.00, priceWithKdv: 14.16, state: 1, instantState: 4, isAvailableForCharge: false },
      ],
    }).pipe(delay(1000));
  }

  // 3. PreparePriceKwSelect — /v1/QrWebProcessingSession/PreparePriceKwSelect
  preparePriceKwSelect(dto: PreparePriceKwRequestDto): Observable<PreparePriceKwResponseDto> {
    console.log('[MOCK] PreparePriceKwSelect:', dto);
    return of({
      id: 100, guiId: null, state: SessionState.Active, startedAt: new Date().toISOString(),
      hasSocketSelected: true, hasAmountSelected: false, hasPrePaid: false, hasChargeStarted: false,
      chargeDeviceConnectorId: 999, chargeDeviceConnectorName: 'Soket A', socketNumber: 'A1',
      connectorKw: 22, connectorPrice: 8.50, connectorPriceWithKdv: 10.03,
      tempSelectedAmount: null, tempSelectedKw: null, tempPaidAmount: null, tempPaymentDate: null,
      chargeId: null, chargeState: null,
    }).pipe(delay(800));
  }

  // 4. ChargeProcessing — /v1/QrWebProcessingSession/ChargeProcessing
  chargeProcessing(dto: ChargeProcessingRequestDto): Observable<ChargeProcessingResponseDto> {
    console.log('[MOCK] ChargeProcessing:', dto);
    return of({
      chargeId: 555, chargeState: ChargeState.ProcessStart,
      startTime: '2026-04-28T10:30:00Z', endTime: null,
      loadedKw: 12.5, calculatedPrice: 125.50, chargePercentage: 65.0,
      tempPaidAmount: 500.00, estimatedRefundAmount: 374.50,
    }).pipe(delay(1000));
  }

  // 5. SelectSocket — /v1/QrWebProcessingSession/SelectSocket
  selectSocket(dto: SelectSocketRequestDto): Observable<PreparePriceKwResponseDto> {
    console.log('[MOCK] SelectSocket:', dto);
    return of({
      id: 100, guiId: null, state: SessionState.Active, startedAt: new Date().toISOString(),
      hasSocketSelected: true, hasAmountSelected: false, hasPrePaid: false, hasChargeStarted: false,
      chargeDeviceConnectorId: dto.connectorId, chargeDeviceConnectorName: 'Soket A', socketNumber: 'A1',
      connectorKw: 22, connectorPrice: 8.50, connectorPriceWithKdv: 10.03,
      tempSelectedAmount: null, tempSelectedKw: null, tempPaidAmount: null, tempPaymentDate: null,
      chargeId: null, chargeState: null,
    }).pipe(delay(800));
  }

  // 6. SelectPriceKw — selectedAmount XOR selectedKw
  selectPriceKw(dto: SelectPriceKwRequestDto): Observable<PreparePriceKwResponseDto> {
    console.log('[MOCK] SelectPriceKw:', dto);
    return of({
      id: 100, guiId: null, state: SessionState.Active, startedAt: new Date().toISOString(),
      hasSocketSelected: true, hasAmountSelected: true, hasPrePaid: false, hasChargeStarted: false,
      chargeDeviceConnectorId: 999, chargeDeviceConnectorName: 'Soket A', socketNumber: 'A1',
      connectorKw: 22, connectorPrice: 8.50, connectorPriceWithKdv: 10.03,
      tempSelectedAmount: dto.selectedAmount, tempSelectedKw: dto.selectedKw,
      tempPaidAmount: null, tempPaymentDate: null,
      chargeId: null, chargeState: null,
    }).pipe(delay(800));
  }
}
```

### services/domain/auth-storage.service.ts
```typescript
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AuthToken } from 'src/app/core/models/auth';

@Injectable({ providedIn: 'root' })
export class AuthStorageService {
  public userSubject      = new BehaviorSubject<AuthToken | null>(null);
  public isLoadingSubject = new BehaviorSubject<boolean>(false);
  private storageKey      = 'auth_token';

  get userValue(): AuthToken | null { return this.userSubject.value; }
  get isLoggedIn(): boolean         { return !!this.userValue?.token; }

  setAuth(auth: AuthToken): boolean {
    if (auth && auth.token) {
      localStorage.setItem(this.storageKey, JSON.stringify(auth));
      this.userSubject.next(auth);
      return true;
    }
    return false;
  }

  getAuth(): AuthToken | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      return raw ? (JSON.parse(raw) as AuthToken) : null;
    } catch {
      this.clearAuth();
      return null;
    }
  }

  clearAuth(): void {
    localStorage.removeItem(this.storageKey);
    this.userSubject.next(null);
  }

  // APP_INITIALIZER tarafından çağrılır
  restoreFromStorage(): Observable<AuthToken | null> {
    const auth = this.getAuth();
    if (auth) { this.userSubject.next(auth); }
    return of(auth);
  }
}
```

### services/domain/charge-session.service.ts
```typescript
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ChargeSessionService {
  private storageKey = 'chargeSessionToken';

  setToken(token: string): void { localStorage.setItem(this.storageKey, token); }
  getToken(): string | null     { return localStorage.getItem(this.storageKey); }
  clearToken(): void            { localStorage.removeItem(this.storageKey); }
  hasActiveSession(): boolean   { return !!this.getToken(); }
}
```

### services/domain/core-security.service.ts
```typescript
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CoreSecurityService {

  base64Encode(data: any): string { return btoa(JSON.stringify(data)); }

  base64Decode<T>(encoded: string): T | null {
    try { return JSON.parse(atob(encoded)) as T; }
    catch { return null; }
  }

  rsaEncrypt(data: string, publicKey: string): string { return data; }  // TODO: Security.Api

  generateHash(data: string): string { return data; }  // TODO: Security.Api

  generateUuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
```

### services/domain/device-header.service.ts
```typescript
import { Injectable } from '@angular/core';
import { DeviceHeader } from 'src/app/core/models/device';

@Injectable({ providedIn: 'root' })
export class DeviceHeaderService {
  private storageKey = 'deviceHeader';

  getOrCreate(): DeviceHeader {
    const stored = localStorage.getItem(this.storageKey);
    if (stored) {
      try { return JSON.parse(stored) as DeviceHeader; } catch {}
    }
    const newHeader: DeviceHeader = {
      deviceHeaderId: 'dh-' + Date.now() + '-' + Math.random().toString(36).substring(2, 10),
      createdAt: new Date().toISOString(),
    };
    localStorage.setItem(this.storageKey, JSON.stringify(newHeader));
    return newHeader;
  }
}
```

### services/domain/device-info-collector.service.ts
```typescript
import { Injectable } from '@angular/core';
import { DeviceInfo } from 'src/app/core/models/security';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class DeviceInfoCollectorService {

  collect(): DeviceInfo {
    return {
      userAgent:        navigator.userAgent || '',
      screenResolution: `${screen.width}x${screen.height}`,
      language:         navigator.language || 'tr-TR',
      platform:         navigator.platform || '',
      timeZone:         Intl.DateTimeFormat().resolvedOptions().timeZone || 'Europe/Istanbul',
    };
  }

  getLocalStorageDeviceKey(): string { return localStorage.getItem('deviceSecurityApiKey') || ''; }

  saveSecurityKeys(deviceSecurityApiId: number, deviceSecurityApiKey: string): void {
    localStorage.setItem('deviceSecurityApiId',  String(deviceSecurityApiId));
    localStorage.setItem('deviceSecurityApiKey', deviceSecurityApiKey);
  }

  isAuthenticated(): boolean  { return !!localStorage.getItem('auth_token'); }
  getVersionNumber(): number  { return 1; }
  getVersionKey(): string     { return environment.appVersion || '1.0.0'; }

  // TODO: Security.Api JS modülü entegre edilince aşağıdakiler gerçek implementasyonla değişir
  getEncryptedDeviceData(): string { return 'placeholder-encrypted-device-data'; }
  getRequestValidityStr(): string  { return 'placeholder-request-validity'; }
  getDevicePublicKey(): string     { return 'placeholder-device-public-key'; }
}
```

### services/domain/qr-web-security.service.ts
```typescript
import { Injectable } from '@angular/core';
import { DeviceInfoCollectorService } from './device-info-collector.service';

@Injectable({ providedIn: 'root' })
export class QrWebSecurityService {

  constructor(private deviceInfoCollector: DeviceInfoCollectorService) {}

  private readonly KEYS = {
    deviceSecurityApiId:  'deviceSecurityApiId',
    deviceSecurityApiKey: 'deviceSecurityApiKey',
    tempHash:             'tempHash',
    loginSessionHash:     'loginSessionHash',
    qrWebUserId:          'qrWebUserId',
  };

  getDeviceSecurityApiId(): number  { return Number(localStorage.getItem(this.KEYS.deviceSecurityApiId)) || 0; }
  getDeviceSecurityApiKey(): string { return localStorage.getItem(this.KEYS.deviceSecurityApiKey) || ''; }
  getTempHash(): string             { return localStorage.getItem(this.KEYS.tempHash) || ''; }
  getLoginSessionHash(): string     { return localStorage.getItem(this.KEYS.loginSessionHash) || ''; }
  getQrWebUserId(): number          { return Number(localStorage.getItem(this.KEYS.qrWebUserId)) || 0; }

  saveFirstRequestKeys(deviceSecurityApiId: number, deviceSecurityApiKey: string, tempHash: string): void {
    localStorage.setItem(this.KEYS.deviceSecurityApiId,  String(deviceSecurityApiId));
    localStorage.setItem(this.KEYS.deviceSecurityApiKey, deviceSecurityApiKey);
    localStorage.setItem(this.KEYS.tempHash, tempHash);
  }

  saveLoginSessionHash(hash: string): void  { localStorage.setItem(this.KEYS.loginSessionHash, hash); }
  saveQrWebUserId(userId: number): void     { localStorage.setItem(this.KEYS.qrWebUserId, String(userId)); }

  clearAll(): void { Object.values(this.KEYS).forEach(key => localStorage.removeItem(key)); }

  getEncryptedDeviceData(): string { return this.deviceInfoCollector.getEncryptedDeviceData(); }
  getRequestValidityStr(): string  { return this.deviceInfoCollector.getRequestValidityStr(); }
  getDevicePublicKey(): string     { return this.deviceInfoCollector.getDevicePublicKey(); }
}
```

---

## 7. Core — Guards (Tam Kod)

### core/guards/auth.guard.ts

```typescript
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthStorageService } from 'src/app/core/services/domain/auth-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private authStorageService: AuthStorageService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this.authStorageService.userValue;
    if (currentUser && this.authStorageService.getAuth()) {
      return true;
    }
    this.router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }
}
```

**Dikkat:** `AuthGuard` tanımlıdır ama şu an hiçbir route'a bağlı değildir.

---

## 8. Modules — Her Modül Routing + Component (Tam Kod)

### features/charge/charge.module.ts + start-charge.component.ts

```typescript
// charge.module.ts
@NgModule({
  declarations: [StartChargeComponent],
  imports: [CommonModule, RouterModule.forChild([{ path: '', component: StartChargeComponent }])],
})
export class ChargeModule {}
```

```typescript
// start-charge.component.ts
// QR URL: /startCharge?type=device&data=BASE64(StartChargeData)
// Akış: parse → FirstRequest → 4 farklı sayfadan birine yönlendir
@Component({ selector: 'app-start-charge', ... })
export class StartChargeComponent implements OnInit {
  loadingMessage = 'Yükleniyor...';
  hasError = false;
  errorMessage = '';
  private startChargeType: StartChargeType | null = null;
  private startChargeData: StartChargeData | null = null;

  constructor(
    private route: ActivatedRoute, private router: Router,
    private deviceApiService: DeviceApiService,
    private deviceInfoCollector: DeviceInfoCollectorService
  ) {}

  ngOnInit(): void {
    const type    = this.route.snapshot.queryParamMap.get('type');
    const rawData = this.route.snapshot.queryParamMap.get('data');

    if (!type || !rawData) {
      this.hasError = true;
      this.errorMessage = 'Geçersiz QR kod.';
      return;
    }

    this.startChargeType = type as StartChargeType;

    try {
      this.startChargeData = JSON.parse(atob(rawData)) as StartChargeData;
    } catch {
      this.hasError = true;
      this.errorMessage = 'QR kod okunamadı.';
      return;
    }

    localStorage.setItem('startChargeType', this.startChargeType);
    localStorage.setItem('startChargeData', JSON.stringify(this.startChargeData));
    this.callFirstRequest();
  }

  private callFirstRequest(): void {
    this.loadingMessage = 'Sunucuya bağlanılıyor...';
    this.deviceApiService.firstRequest({
      deviceInfo:           this.deviceInfoCollector.collect(),
      localStorageDeviceKey: this.deviceInfoCollector.getLocalStorageDeviceKey(),
      versionNumber:        this.deviceInfoCollector.getVersionNumber(),
      versionKey:           this.deviceInfoCollector.getVersionKey(),
      isAuth:               this.deviceInfoCollector.isAuthenticated(),
      encryptedDeviceData:  this.deviceInfoCollector.getEncryptedDeviceData(),
      requestValidtyStr:    this.deviceInfoCollector.getRequestValidityStr(),
      devicePublicKey:      this.deviceInfoCollector.getDevicePublicKey(),
    }).subscribe({
      next:  (res) => this.handleResponse(res),
      error: ()    => { this.hasError = true; this.errorMessage = 'Bağlantı kurulamadı.'; },
    });
  }

  private handleResponse(res: FirstRequestResponseDto): void {
    this.deviceInfoCollector.saveSecurityKeys(res.deviceSecurityApiId, res.deviceSecurityApiKey);
    localStorage.setItem('tempHash', res.tempHash || '');

    const queryParams = {
      type: this.startChargeType,
      data: btoa(JSON.stringify({ startCharge: this.startChargeData })),
    };

    if (!res.isLoggedIn)               { this.router.navigate(['/login'],                { queryParams }); return; }
    if (res.activeProcessingSession)   { this.router.navigate(['/chargingStatus'],        { queryParams }); return; }
    if (this.startChargeType === StartChargeType.Connector) { this.router.navigate(['/payment'],           { queryParams }); return; }
    this.router.navigate(['/startChargeSocketList'], { queryParams });
  }
}
```

### features/charge/socket-list — socket-list.component.ts

```typescript
@Component({ selector: 'app-socket-list', ... })
export class SocketListComponent implements OnInit {
  deviceSerialNo = '';
  deviceName = '';
  connectors: ConnectorItemDto[] = [];
  selectedConnectorGuiId: string | null = null;
  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    // type + data → parse → loadSocketList()
  }

  private loadSocketList(): void {
    this.sessionApiService.prepareSocketList({
      qrWebUserId: this.qrWebSecurityService.getQrWebUserId(),
      type: 1, serialNo: this.startChargeData.deviceSerialNo,
      guid: this.startChargeData.deviceGuid,
    }).subscribe({ next: res => { this.connectors = res.connectors; this.loading = false; } });
  }

  selectSocket(guiId: string): void {
    const connector = this.connectors.find(c => c.guiId === guiId);
    if (connector?.isAvailableForCharge) { this.selectedConnectorGuiId = guiId; }
  }

  confirm(): void {
    // SelectSocket API → /payment
    const connector = this.connectors.find(c => c.guiId === this.selectedConnectorGuiId);
    this.sessionApiService.selectSocket({
      qrWebUserId: this.qrWebSecurityService.getQrWebUserId(),
      connectorId: connector.id,
    }).subscribe({ next: () => this.router.navigate(['/payment'], { queryParams: { ... } }) });
  }

  getStatusText(c: ConnectorItemDto): string {
    if (c.isAvailableForCharge) return 'Uygun';
    switch (c.instantState) {
      case 1: return 'Uygun'; case 3: return 'Hazırlanıyor';
      case 4: return 'Meşgul'; case 5: return 'Arızalı';
      case 6: return 'Dolu'; default: return 'Servis Dışı';
    }
  }

  getStatusClass(c: ConnectorItemDto): string {
    if (c.isAvailableForCharge) return 'status-available';
    switch (c.instantState) {
      case 1: return 'status-available'; case 3: return 'status-plugged';
      case 4: return 'status-occupied';  case 5: return 'status-out';
      case 6: return 'status-occupied';  default: return 'status-out';
    }
  }
}
```

### features/payment/payment.component.ts (Tam)

```typescript
@Component({ selector: 'app-payment', ... })
export class PaymentComponent implements OnInit {
  ChargeAmountType = ChargeAmountType;

  amountType: ChargeAmountType = ChargeAmountType.Kwh;
  chargeAmount: number | null = null;
  pricePerKwh = 0;
  connectorKw: number | null = null;
  connectorName: string | null = null;
  socketNumber: string | null = null;

  get estimatedPrice(): number | null {
    if (!this.chargeAmount || this.chargeAmount <= 0) return null;
    return this.amountType === ChargeAmountType.Kwh
      ? +(this.chargeAmount * this.pricePerKwh).toFixed(2)
      : this.chargeAmount;
  }

  get estimatedKwh(): number | null {
    if (!this.chargeAmount || this.chargeAmount <= 0 || !this.pricePerKwh) return null;
    return this.amountType === ChargeAmountType.Price
      ? +(this.chargeAmount / this.pricePerKwh).toFixed(1)
      : this.chargeAmount;
  }

  cardHolder = ''; cardNumber = ''; expiryMonth = ''; expiryYear = ''; cvc = ''; saveCard = false;

  savedCards: UserSavedCardItem[] = [
    { id: 'card-1', cardName: 'Kişisel Kartım', lastFour: '4523' },
    { id: 'card-2', cardName: 'İş Kartım', lastFour: '8871' },
  ];
  selectedSavedCardId: string | null = null;

  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    // type + data parse → callPreparePriceKwSelect()
    this.callPreparePriceKwSelect();
  }

  private callPreparePriceKwSelect(): void {
    const isConnector = this.startChargeType === StartChargeType.Connector;
    this.sessionApiService.preparePriceKwSelect({
      qrWebUserId: this.qrWebSecurityService.getQrWebUserId(),
      type:    isConnector ? 2 : null,
      serialNo: isConnector ? (this.startChargeData?.connectorSerialNo || null) : null,
      guid:     isConnector ? (this.startChargeData?.connectorGuid || null) : null,
    }).subscribe({
      next: (res) => {
        this.connectorKw    = res.connectorKw || null;
        this.pricePerKwh    = res.connectorPriceWithKdv || 0;
        this.connectorName  = res.chargeDeviceConnectorName || null;
        this.socketNumber   = res.socketNumber || null;
        this.loading = false;
      }
    });
  }

  selectSavedCard(cardId: string): void {
    this.selectedSavedCardId = this.selectedSavedCardId === cardId ? null : cardId;
    if (this.selectedSavedCardId) {
      this.cardHolder = ''; this.cardNumber = '';
      this.expiryMonth = ''; this.expiryYear = ''; this.cvc = '';
    }
  }

  formatCardNumber(): void {
    let clean = this.cardNumber.replace(/\D/g, '');
    if (clean.length > 16) clean = clean.substring(0, 16);
    this.cardNumber = clean.replace(/(.{4})/g, '$1 ').trim();
    this.selectedSavedCardId = null;
  }

  submit(): void {
    if (!this.chargeAmount || this.chargeAmount <= 0) {
      this.errorMessage = 'Lütfen şarj miktarını giriniz.'; return;
    }
    if (!this.selectedSavedCardId) {
      if (!this.cardHolder || !this.cardNumber || !this.expiryMonth || !this.expiryYear || !this.cvc) {
        this.errorMessage = 'Lütfen kart bilgilerini doldurunuz.'; return;
      }
    }

    this.loading = true;
    const isKwh = this.amountType === ChargeAmountType.Kwh;

    // Adım 1: SelectPriceKw
    this.sessionApiService.selectPriceKw({
      qrWebUserId:    this.qrWebSecurityService.getQrWebUserId(),
      selectedAmount: isKwh ? null : this.chargeAmount,
      selectedKw:     isKwh ? this.chargeAmount : null,
    }).subscribe({
      next: () => {
        // Adım 2: PayCharge — TODO
        this.chargeSessionService.setToken('mock-charge-session-' + Date.now());
        this.loading = false;
        this.router.navigate(['/chargingStatus']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.errorMessage || 'İşlem başarısız.';
      },
    });
  }
}
```

### features/auth/login/login.component.ts (Tam)

```typescript
type LoginStep = 'choose' | 'phone-input' | 'sms-verify' | 'processing';

@Component({ selector: 'app-login', ... })
export class LoginComponent implements OnInit {
  step: LoginStep = 'choose';
  phone = '';
  smsCode = '';
  loading = false;
  errorMessage = '';
  processingMessage = '';

  welcomeMessage = '';
  availableLoginMethods: number[] = [];
  privacyPolicyUrl = '';
  termsOfServiceUrl = '';
  helpContactPhone = '';
  helpContactEmail = '';
  loginSessionHash = '';

  private startChargeData: StartChargeData | null = null;
  private startChargeType: StartChargeType | null = null;

  constructor(
    private authApiService: AuthApiService,
    private sessionApiService: SessionApiService,
    private authStorageService: AuthStorageService,
    private chargeSessionService: ChargeSessionService,
    private qrWebSecurityService: QrWebSecurityService,
    private deviceInfoCollector: DeviceInfoCollectorService,
    private coreSecurityService: CoreSecurityService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Query params'dan type + data al → parse → PrepareLogin çağır
    const typeParam = this.route.snapshot.queryParams['type'] || localStorage.getItem('startChargeType') || '';
    const dataParam = this.route.snapshot.queryParams['data'] || localStorage.getItem('startChargeData') || '';

    if (typeParam) { this.startChargeType = typeParam as StartChargeType; }
    if (dataParam) {
      const payload = this.coreSecurityService.base64Decode<QrPayload>(dataParam);
      if (payload?.startCharge) { this.startChargeData = payload.startCharge; }
    }

    this.callPrepareLogin();
  }

  private callPrepareLogin(): void {
    this.loading = true;
    this.sessionApiService.prepareLogin({
      deviceInfo:           this.deviceInfoCollector.collect(),
      localStorageDeviceKey: this.deviceInfoCollector.getLocalStorageDeviceKey(),
      deviceSecurityApiKey:  this.qrWebSecurityService.getDeviceSecurityApiKey(),
      deviceSecurityApiId:   this.qrWebSecurityService.getDeviceSecurityApiId(),
      tempHash:              this.qrWebSecurityService.getTempHash(),
      encryptedDeviceData:   this.qrWebSecurityService.getEncryptedDeviceData(),
      requestValidtyStr:     this.qrWebSecurityService.getRequestValidityStr(),
      devicePublicKey:       this.qrWebSecurityService.getDevicePublicKey(),
    }).subscribe({
      next: (res: PrepareLoginResponseDto) => {
        this.loading = false;
        if (res.loginSessionHash) {
          this.loginSessionHash = res.loginSessionHash;
          this.qrWebSecurityService.saveLoginSessionHash(res.loginSessionHash);
        }
        this.welcomeMessage        = res.welcomeMessage || '';
        this.availableLoginMethods = res.availableLoginMethods || [];
        this.privacyPolicyUrl      = res.privacyPolicyUrl || '';
        this.termsOfServiceUrl     = res.termsOfServiceUrl || '';
        this.helpContactPhone      = res.helpContactPhone || '';
        this.helpContactEmail      = res.helpContactEmail || '';
      },
      error: () => { this.loading = false; this.errorMessage = 'Bağlantı hatası.'; },
    });
  }

  goToPhoneLogin(): void { this.step = 'phone-input'; this.errorMessage = ''; }

  loginWithGoogle(): void {
    this.loading = true;
    this.authApiService.login({ loginType: LoginType.Google, googleIdToken: 'mock-google-id-token' }).subscribe({
      next: () => this.afterLoginSuccess(),
      error: () => { this.loading = false; this.errorMessage = 'Google giriş başarısız.'; },
    });
  }

  sendPhoneLogin(): void {
    if (!this.phone || this.phone.length < 10) {
      this.errorMessage = 'Geçerli bir telefon numarası giriniz.'; return;
    }
    this.loading = true;
    this.authApiService.login({ loginType: LoginType.Phone, phone: this.phone }).subscribe({
      next: () => { this.loading = false; this.step = 'sms-verify'; },
      error: () => { this.loading = false; this.errorMessage = 'SMS gönderilemedi.'; },
    });
  }

  verifySmsCode(): void {
    if (!this.smsCode || this.smsCode.length < 4) {
      this.errorMessage = 'Doğrulama kodunu giriniz.'; return;
    }
    this.loading = true;
    this.authApiService.validateSmsCode({ phone: this.phone, smsCode: this.smsCode }).subscribe({
      next: (res) => {
        this.authStorageService.setAuth({ token: res.token, refreshToken: res.refreshToken, expiresIn: res.expiresIn });
        this.afterLoginSuccess();
      },
      error: () => { this.loading = false; this.errorMessage = 'Kod hatalı.'; },
    });
  }

  private afterLoginSuccess(): void {
    this.step = 'processing';
    this.processingMessage = 'Şarj işlemi hazırlanıyor...';
    if (this.startChargeType === StartChargeType.Connector) {
      this.router.navigate(['/payment'], { queryParams: { type: this.startChargeType, data: this.route.snapshot.queryParams['data'] } });
    } else {
      this.router.navigate(['/startChargeSocketList'], { queryParams: { type: this.startChargeType, data: this.route.snapshot.queryParams['data'] } });
    }
  }

  goBack(): void {
    this.step = this.step === 'sms-verify' ? 'phone-input' : 'choose';
    this.errorMessage = '';
  }
}
```

### features/home/home.component.ts

```typescript
type HomeStatus = 'loading' | 'active-charge' | 'last-charge' | 'no-charge';

@Component({ selector: 'app-home', ... })
export class HomeComponent implements OnInit {
  status: HomeStatus = 'loading';
  chargeStatus: ChargeStatusDto | null = null;
  lastChargeSuccess = true;
  targetType: 'kwh' | 'tl' = 'kwh';
  targetAmount: number = 10;

  get targetProgress(): number {
    if (!this.targetAmount || !this.chargeStatus) return 0;
    return Math.min(100, (this.currentTargetValue / this.targetAmount) * 100);
  }

  get currentTargetValue(): number {
    if (!this.chargeStatus) return 0;
    return this.targetType === 'kwh' ? (this.chargeStatus.chargeEnergy || 0) : (this.chargeStatus.totalPrice || 0);
  }

  constructor(
    private router: Router,
    private chargeApiService: ChargeApiService,
    private chargeSessionService: ChargeSessionService
  ) {}

  ngOnInit(): void { this.checkChargeStatus(); }

  private checkChargeStatus(): void {
    const token = this.chargeSessionService.getToken();
    if (!token) { this.status = 'no-charge'; return; }

    this.chargeApiService.checkActiveCharge({ chargeSessionToken: token }).subscribe({
      next: (res) => {
        if (res.hasActiveCharge && res.chargeStatus) {
          this.chargeStatus = res.chargeStatus;
          this.status = res.chargeStatus.status === ChargeStatusType.Completed ? 'last-charge' : 'active-charge';
        } else {
          this.status = 'no-charge';
        }
      },
      error: () => { this.status = 'no-charge'; },
    });
  }

  goToChargingStatus(): void { this.router.navigate(['/chargingStatus']); }
  goToLastCharge(): void     { this.router.navigate(['/chargeSummary']); }

  getStatusText(): string {
    switch (this.chargeStatus?.status) {
      case ChargeStatusType.Charging:  return 'Şarj Oluyor';
      case ChargeStatusType.Starting:  return 'Başlatılıyor';
      case ChargeStatusType.Completed: return 'Tamamlandı';
      default: return '';
    }
  }
}
```

### features/transactions/transactions.component.ts

```typescript
@Component({ selector: 'app-transactions', ... })
export class TransactionsComponent implements OnInit {
  UserTransactionFilter = UserTransactionFilter;
  SortField = SortField;
  SortDirection = SortDirection;

  statusFilter: UserTransactionFilter = UserTransactionFilter.All;
  sortField: SortField = SortField.Date;
  sortDir: SortDirection = SortDirection.Desc;
  showFilterDialog = false;
  showSortDialog = false;

  transactions: UserTransactionItem[] = [ /* 7 mock item */ ];

  get totalTransactions(): number { return this.transactions.length; }
  get totalSpent(): number { return this.transactions.filter(t => t.status === TransactionStatus.Completed).reduce((s, t) => s + (t.actualPrice || 0), 0); }
  get totalEnergy(): number { return this.transactions.filter(t => t.status === TransactionStatus.Completed).reduce((s, t) => s + (t.actualEnergy || 0), 0); }
  get avgSpent(): string { const c = this.transactions.filter(t => t.status === TransactionStatus.Completed); return c.length ? (this.totalSpent / c.length).toFixed(0) : '0'; }
  get avgEnergy(): string { const c = this.transactions.filter(t => t.status === TransactionStatus.Completed); return c.length ? (this.totalEnergy / c.length).toFixed(1) : '0'; }

  get filteredTransactions(): UserTransactionItem[] {
    let list = [...this.transactions];
    if (this.statusFilter !== UserTransactionFilter.All) {
      const map: Record<string, TransactionStatus[]> = {
        [UserTransactionFilter.Completed]:  [TransactionStatus.Completed],
        [UserTransactionFilter.Failed]:     [TransactionStatus.Failed, TransactionStatus.PaymentFailed],
        [UserTransactionFilter.Processing]: [TransactionStatus.PaymentProcessing, TransactionStatus.Calculating],
      };
      list = list.filter(t => map[this.statusFilter]?.includes(t.status!));
    }
    list.sort((a, b) => {
      let valA: any = this.sortField === SortField.Date ? (a.startDate || '') : (a.actualPrice || a.paidAmount || 0);
      let valB: any = this.sortField === SortField.Date ? (b.startDate || '') : (b.actualPrice || b.paidAmount || 0);
      const cmp = valA > valB ? 1 : valA < valB ? -1 : 0;
      return this.sortDir === SortDirection.Desc ? -cmp : cmp;
    });
    return list;
  }

  toggleSort(field: SortField): void {
    if (this.sortField === field) {
      this.sortDir = this.sortDir === SortDirection.Desc ? SortDirection.Asc : SortDirection.Desc;
    } else { this.sortField = field; this.sortDir = SortDirection.Desc; }
  }

  getStatusText(status?: TransactionStatus): string {
    switch (status) {
      case TransactionStatus.PaymentProcessing: return 'Ödeme Alınıyor';
      case TransactionStatus.Failed:            return 'İşlem Başarısız';
      case TransactionStatus.Completed:         return 'Tamamlandı';
      case TransactionStatus.Calculating:       return 'Hesaplanıyor';
      case TransactionStatus.PaymentFailed:     return 'Ödeme Başarısız';
      default: return '';
    }
  }

  getStatusClass(status?: TransactionStatus): string {
    switch (status) {
      case TransactionStatus.Completed:         return 'badge-green';
      case TransactionStatus.PaymentProcessing:
      case TransactionStatus.Calculating:       return 'badge-orange';
      case TransactionStatus.Failed:
      case TransactionStatus.PaymentFailed:     return 'badge-red';
      default: return '';
    }
  }
}
```

### features/support/support.component.ts

```typescript
@Component({ selector: 'app-support', ... })
export class SupportComponent implements OnInit {
  SupportTicketFilter = SupportTicketFilter;
  statusFilter: SupportTicketFilter = SupportTicketFilter.All;
  showFilterDialog = false;

  tickets: SupportTicketItem[] = [ /* 4 mock ticket */ ];

  get filteredTickets(): SupportTicketItem[] {
    if (this.statusFilter === SupportTicketFilter.All) return this.tickets;
    const map: Record<string, TicketStatus> = {
      [SupportTicketFilter.NewMessage]: TicketStatus.NewMessage,
      [SupportTicketFilter.Waiting]:    TicketStatus.Waiting,
      [SupportTicketFilter.Completed]:  TicketStatus.Completed,
    };
    return this.tickets.filter(t => t.status === map[this.statusFilter]);
  }

  createTicket(): void  { this.router.navigate(['/support/new']); }
  openTicket(t: SupportTicketItem): void { this.router.navigate(['/support/detail'], { queryParams: { ticketId: t.id } }); }
  openLiveChat(): void  { /* TODO: Canlı destek entegrasyonu */ }
  getStars(r: number): boolean[] { return Array(5).fill(false).map((_, i) => i < r); }

  getStatusText(status?: TicketStatus): string {
    switch (status) {
      case TicketStatus.NewMessage: return 'Yeni Mesaj';
      case TicketStatus.Waiting:    return 'Bekliyor';
      case TicketStatus.Completed:  return 'Tamamlandı';
      default: return '';
    }
  }

  getStatusClass(status?: TicketStatus): string {
    switch (status) {
      case TicketStatus.NewMessage: return 'status-new';
      case TicketStatus.Waiting:    return 'status-waiting';
      case TicketStatus.Completed:  return 'status-completed';
      default: return '';
    }
  }
}
```

### features/support/new-ticket/new-ticket.component.ts

```typescript
const TOPIC_OPTIONS = [
  { id: 'other',   label: 'Diğer',                   hasTextarea: true },
  { id: 'charge',  label: 'Şarj İşlemi',             hasTextarea: false },
  { id: 'payment', label: 'Ödeme İşlemi',             hasTextarea: false },
  { id: 'vehicle', label: 'Araç İşgali',              hasTextarea: false },
  { id: 'station', label: 'İstasyon Talebi',          hasTextarea: false },
  { id: 'device',  label: 'Cihaz Arıza Bildirimi',   hasTextarea: false },
];

const BRAND_OPTIONS = [
  { id: 'promaster',  label: 'Promaster' },
  { id: 'checkpoint', label: 'Checkpoint' },
  { id: 'solargize',  label: 'Solargize' },
  { id: 'greendrive', label: 'Greendrive' },
  { id: 'tripy',      label: 'Tripy' },
  { id: 'sarjplus',   label: 'Şarjplus' },
  { id: 'voltgo',     label: 'Voltgo' },
  { id: 'tora',       label: 'Tora' },
];

@Component({ selector: 'app-new-ticket', ... })
export class NewTicketComponent {
  selectedTopicId = '';
  selectedTopicLabel = '';
  otherTopicText = '';
  selectedBrandId = '';
  selectedBrandLabel = '';
  message = '';
  showTopicDialog = false;
  showBrandDialog = false;
  topicSearch = '';
  brandSearch = '';
  loading = false;

  topicOptions = TOPIC_OPTIONS;
  brandOptions = BRAND_OPTIONS;

  get filteredTopics() {
    return !this.topicSearch ? this.topicOptions
      : this.topicOptions.filter(t => t.label.toLowerCase().includes(this.topicSearch.toLowerCase()));
  }

  get filteredBrands() {
    return !this.brandSearch ? this.brandOptions
      : this.brandOptions.filter(b => b.label.toLowerCase().includes(this.brandSearch.toLowerCase()));
  }

  selectTopic(topic: any): void {
    this.selectedTopicId = topic.id;
    this.selectedTopicLabel = topic.label;
    if (!topic.hasTextarea) { this.showTopicDialog = false; }
  }

  selectBrand(brand: any): void {
    this.selectedBrandId = brand.id;
    this.selectedBrandLabel = brand.label;
    this.showBrandDialog = false;
  }

  submit(): void {
    if (!this.selectedTopicId || !this.message) return;
    this.loading = true;
    // TODO: Backend'e talep gönder
    setTimeout(() => {
      this.loading = false;
      this.router.navigate(['/support']);
    }, 1500);
  }
}
```

---

## 9. SignalR Entegrasyonu (Tam Kod)

`@microsoft/signalr 6.0.1` paketi kurulu. `ChargingStatusComponent` içinde **şu an 3s setInterval mock** kullanılmaktadır.

### Mevcut — charging-status.component.ts (Tam)

```typescript
import { Component, OnInit, OnDestroy, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionApiService } from 'src/app/core/services/api/session-api.service';
import { QrWebSecurityService } from 'src/app/core/services/domain/qr-web-security.service';
import { ChargeApiService } from 'src/app/core/services/api/charge-api.service';
import { ChargeSessionService } from 'src/app/core/services/domain/charge-session.service';
import { ChargeProcessingResponseDto } from 'src/app/core/models/session';
import { ChargeStatusDto } from 'src/app/core/models/charge';
import { ChargeStatusType, ChargeAmountType, ChargeState } from 'src/app/core/enums';

@Component({
  selector: 'app-charging-status',
  templateUrl: './charging-status.component.html',
  styleUrls: ['./charging-status.component.scss'],
})
export class ChargingStatusComponent implements OnInit, OnDestroy, AfterViewInit {
  ChargeAmountType = ChargeAmountType;

  @ViewChild('lottieContainer', { static: false }) lottieContainer!: ElementRef;
  private lottieAnim: any;

  chargeStatus: ChargeStatusDto | null = null;
  tempPaidAmount: number | null = null;
  estimatedRefundAmount: number | null = null;
  chargeState: ChargeState | null = null;

  targetType: ChargeAmountType = ChargeAmountType.Kwh;
  targetAmount: number = 10;

  get targetProgress(): number {
    if (!this.targetAmount || !this.chargeStatus) return 0;
    return Math.min(100, (this.currentTargetValue / this.targetAmount) * 100);
  }

  get currentTargetValue(): number {
    if (!this.chargeStatus) return 0;
    return this.targetType === ChargeAmountType.Kwh
      ? this.chargeStatus.chargeEnergy || 0
      : this.chargeStatus.totalPrice || 0;
  }

  get remainingTargetValue(): number {
    return Math.max(0, +(this.targetAmount - this.currentTargetValue).toFixed(1));
  }

  get estimatedRemainingMinutes(): number {
    if (!this.chargeStatus?.chargePower || !this.chargeStatus?.chargeEnergy
        || !this.chargeStatus?.elapsedMinutes) return 0;
    const remainingKwh = this.targetType === ChargeAmountType.Kwh
      ? this.remainingTargetValue
      : this.remainingTargetValue / (this.chargeStatus.totalPrice && this.chargeStatus.chargeEnergy
          ? (this.chargeStatus.totalPrice / this.chargeStatus.chargeEnergy) : 7.79);
    return Math.max(0, Math.round((remainingKwh / this.chargeStatus.chargePower) * 60));
  }

  loading = true;
  stopping = false;
  errorMessage = '';
  private mockInterval: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private sessionApiService: SessionApiService,
    private qrWebSecurityService: QrWebSecurityService,
    private chargeApiService: ChargeApiService,
    private chargeSessionService: ChargeSessionService
  ) {}

  ngOnInit(): void { this.loadChargeProcessing(); }
  ngAfterViewInit(): void {}

  // Lottie — lottie-web direkt (ngx-lottie Angular 13 uyumsuz)
  private initLottie(): void {
    if (!this.lottieContainer?.nativeElement) return;
    import('lottie-web').then((lottie: any) => {
      const lottieLib = lottie.default || lottie;
      this.lottieAnim = lottieLib.loadAnimation({
        container: this.lottieContainer.nativeElement,
        renderer: 'svg',
        loop: true,
        autoplay: true,
        path: 'assets/animations/chargeanimation.json',
      });
    });
  }

  ngOnDestroy(): void {
    if (this.lottieAnim) { this.lottieAnim.destroy(); }
    if (this.mockInterval) { clearInterval(this.mockInterval); }
  }

  private loadChargeProcessing(): void {
    this.sessionApiService.chargeProcessing({
      qrWebUserId: this.qrWebSecurityService.getQrWebUserId(),
    }).subscribe({
      next: (res: ChargeProcessingResponseDto) => {
        this.chargeStatus = {
          sessionId:    String(res.chargeId),
          status:       this.mapChargeStateToStatusType(res.chargeState),
          chargePercent: res.chargePercentage || 0,
          chargeEnergy:  res.loadedKw || 0,
          totalPrice:    res.calculatedPrice || 0,
          chargePower:   0,
          startTime:     res.startTime || '',
          elapsedMinutes: 0,
        };
        this.tempPaidAmount        = res.tempPaidAmount || null;
        this.estimatedRefundAmount = res.estimatedRefundAmount || null;
        this.chargeState           = res.chargeState;
        this.loading = false;
        setTimeout(() => this.initLottie(), 100);
        this.startMockUpdates();
      },
      error: () => { this.loading = false; this.errorMessage = 'Şarj durumu alınamadı.'; },
    });
  }

  private mapChargeStateToStatusType(chargeState: ChargeState): ChargeStatusType {
    switch (chargeState) {
      case ChargeState.ProcessStarting:     return ChargeStatusType.Starting;
      case ChargeState.ProcessStart:        return ChargeStatusType.Charging;
      case ChargeState.ProcessEnding:       return ChargeStatusType.Stopping;
      case ChargeState.Completed:           return ChargeStatusType.Completed;
      case ChargeState.Failed:
      case ChargeState.PaymentFail:         return ChargeStatusType.Error;
      default:                              return ChargeStatusType.Charging;
    }
  }

  // TODO: SignalR ile değiştirilecek
  private startMockUpdates(): void {
    this.mockInterval = setInterval(() => {
      if (this.chargeStatus && this.chargeStatus.status === ChargeStatusType.Charging) {
        this.chargeStatus = {
          ...this.chargeStatus,
          chargePercent:  Math.min(100, (this.chargeStatus.chargePercent || 0) + 1),
          chargeEnergy:   +(((this.chargeStatus.chargeEnergy || 0) + 0.1).toFixed(1)),
          elapsedMinutes: (this.chargeStatus.elapsedMinutes || 0) + 1,
          totalPrice:     +(((this.chargeStatus.totalPrice || 0) + 0.78).toFixed(1)),
          chargePower:    +(18 + Math.random() * 4).toFixed(1),
        };
      }
    }, 3000);
  }

  stopCharge(): void {
    if (!this.chargeStatus?.sessionId) return;
    this.stopping = true;
    this.chargeApiService.stopCharge({
      chargeSessionToken: this.chargeSessionService.getToken() || '',
      sessionId:          this.chargeStatus.sessionId,
    }).subscribe({
      next: (res) => {
        this.stopping = false;
        if (res.success && res.finalStatus) {
          this.chargeStatus = res.finalStatus;
          if (this.mockInterval) { clearInterval(this.mockInterval); }
          this.chargeSessionService.clearToken();
        }
      },
      error: () => { this.stopping = false; this.errorMessage = 'Şarj durdurulamadı.'; },
    });
  }

  getStatusText(): string {
    switch (this.chargeStatus?.status) {
      case ChargeStatusType.Starting:  return 'Başlatılıyor';
      case ChargeStatusType.Charging:  return 'Şarj Oluyor';
      case ChargeStatusType.Stopping:  return 'Durduruluyor';
      case ChargeStatusType.Completed: return 'Tamamlandı';
      case ChargeStatusType.Error:     return 'Hata';
      default: return '';
    }
  }

  isCharging(): boolean { return this.chargeStatus?.status === ChargeStatusType.Charging; }
  goBack(): void        { window.history.back(); }
}
```

### Hedef SignalR Mimarisi

```typescript
// core/services/domain/signalr.service.ts — oluşturulacak
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { ChargeStatusDto } from 'src/app/core/models/charge';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  chargeStatus$ = new BehaviorSubject<ChargeStatusDto | null>(null);

  async startConnection(hubUrl: string, token: string): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ChargeStatusUpdated', (status: ChargeStatusDto) => {
      this.chargeStatus$.next(status);
    });

    await this.hubConnection.start();
  }

  async stopConnection(): Promise<void> {
    await this.hubConnection?.stop();
  }
}
```

---

## 10. PDF Export (Altyapı Hazır)

`jspdf 4.0.0` + `html2canvas 1.4.1` kurulu, aktif kullanım henüz kodlanmamıştır.

```typescript
// charge-summary component içine eklenecek
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

async exportPdf(): Promise<void> {
  const element = document.getElementById('summary-card');
  const canvas  = await html2canvas(element);
  const imgData = canvas.toDataURL('image/png');
  const pdf     = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });
  const imgWidth  = 210;
  const imgHeight = (canvas.height * imgWidth) / canvas.width;
  pdf.addImage(imgData, 'PNG', 0, 0, imgWidth, imgHeight);
  pdf.save(`sarj-ozeti-${this.summary.sessionId}.pdf`);
}
```

---

## 11. Custom UI Components

### Lottie Animasyon — Direkt lottie-web

Angular 13 ile `ngx-lottie` uyumsuzluğu nedeniyle dinamik import kullanılır:

```typescript
private initLottie(): void {
  if (!this.lottieContainer?.nativeElement) return;
  import('lottie-web').then((lottie: any) => {
    const lottieLib = lottie.default || lottie;
    this.lottieAnim = lottieLib.loadAnimation({
      container: this.lottieContainer.nativeElement,
      renderer: 'svg',
      loop: true,
      autoplay: true,
      path: 'assets/animations/chargeanimation.json',
    });
  });
}

// ngOnDestroy içinde mutlaka yok et:
ngOnDestroy(): void {
  if (this.lottieAnim) { this.lottieAnim.destroy(); }
}
```

### Custom Component Listesi

| Component | Selector | Açıklama |
|---|---|---|
| RotawattButtonComponent | app-rotawatt-button | Yükleme durumlu buton |
| RotawattInputComponent | app-rotawatt-input | ControlValueAccessor form input |
| RotawattSelectComponent | app-rotawatt-select | Mat-select sarmalayıcı |
| RotawattDataTableComponent | app-rotawatt-data-table | ColumnConfig+TableConfig tabanlı tablo |
| RotawattDatetimeComponent | app-rotawatt-datetime | Mat-datepicker + saat seçici |
| RotawattYearMonthPickerComponent | app-rotawatt-year-month-picker | Yıl/ay seçici |
| PixdinnDropzoneComponent | app-pixdinn-dropzone | ngx-dropzone + image-cropper |
| PixdinnHtmlEditorComponent | app-pixdinn-html-editor | ngx-editor sarmalayıcı |

---

## 12. Mimari Kararlar + Dikkat Noktaları

### Mimari Kararlar

| Karar | Açıklama |
|---|---|
| Hash Routing | `useHash: true` — QR URL'leri bozulmaz, deploy kolaylığı |
| Lazy Loading | Tüm feature modülleri — ilk yükleme hızı |
| 2 Katmanlı Service | `api/` (HTTP) + `domain/` (iş mantığı) — sorumluluk ayrımı |
| Barrel Export | `core/enums/index.ts` — tek import noktası |
| Mock-First | Tüm API'ler mock, gerçek endpoint comment'te |
| Base64 QR Payload | `?data=BASE64(QrPayload)` — URL üzerinden sayfa geçişi |
| localStorage Yedek | `startChargeType` + `startChargeData` saklı — sayfa yenileme |
| Lottie Dinamik Import | Angular 13 uyumu için lazy `import('lottie-web')` |
| ChargeState→StatusType Map | OCPP enum'unu UI enum'una çeviren katman |
| APP_INITIALIZER | Token localStorage'dan BehaviorSubject'e restore |

### Dikkat Noktaları

1. **AuthGuard bağlı değil** — Guard yazılmış ama route'lara eklenmemiştir.
2. **Tüm API'ler mock** — Gerçek HTTP çağrıları comment'tedir.
3. **Security.Api placeholder** — `getEncryptedDeviceData()` vs. placeholder string döndürür.
4. **SignalR mock** — 3s interval, üretimde değiştirilecek.
5. **XOR kuralı** — `SelectPriceKwRequestDto`: `selectedAmount` XOR `selectedKw`.
6. **Çift FirstRequest DTO** — `models/device/` (eski) ve `models/security/` (aktif).
7. **Refunded enum yok** — İade `Completed` içinde `isRefunded: true` ile gösterilir.
8. **PDF/Excel altyapı hazır** — `jspdf`, `xlsx` kurulu, kod yazılmamış.
9. **qrWebUserId** — localStorage'dan okunur, backend'den tam akış sonra gelecek.
10. **İki API tabanı** — `environment.apiUrl` (genel) + `environment.stationApiUrl` (şarj).
