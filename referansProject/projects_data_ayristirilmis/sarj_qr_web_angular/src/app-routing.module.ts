// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\app-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// ============================================================
// Uygulama Route Yapisi
// Akis: startCharge → login → socketList → payment → chargingStatus → chargeResult → chargeSummary
// Anasayfa: /home
// ============================================================
const routes: Routes = [
  // QR okutma sonrasi ilk sayfa
  {
    path: 'startCharge',
    loadChildren: () =>
      import('./features/charge/charge.module').then((m) => m.ChargeModule),
  },
  // Soket secim sayfasi
  {
    path: 'startChargeSocketList',
    loadChildren: () =>
      import('./features/charge/socket-list/socket-list.module').then((m) => m.SocketListModule),
  },
  // Odeme sayfasi
  {
    path: 'payment',
    loadChildren: () =>
      import('./features/payment/payment.module').then((m) => m.PaymentModule),
  },
  // Sarj oluyor sayfasi
  {
    path: 'chargingStatus',
    loadChildren: () =>
      import('./features/charge/charging-status/charging-status.module').then((m) => m.ChargingStatusModule),
  },
  // Sarj sonucu (basarili/basarisiz)
  {
    path: 'chargeResult',
    loadChildren: () =>
      import('./features/charge/charge-result/charge-result.module').then((m) => m.ChargeResultModule),
  },
  // Sarj ozeti (detayli bilgiler)
  {
    path: 'chargeSummary',
    loadChildren: () =>
      import('./features/charge/charge-summary/charge-summary.module').then((m) => m.ChargeSummaryModule),
  },
  // Login sayfasi
  {
    path: 'login',
    loadChildren: () =>
      import('./features/auth/auth.module').then((m) => m.AuthFeatureModule),
  },
  // Teknik destek
  {
    path: 'support',
    loadChildren: () =>
      import('./features/support/support.module').then((m) => m.SupportModule),
  },
  // Son islemlerim
  {
    path: 'transactions',
    loadChildren: () =>
      import('./features/transactions/transactions.module').then((m) => m.TransactionsModule),
  },
  // Kayitli Kartlarim
  {
    path: 'savedCards',
    loadChildren: () =>
      import('./features/saved-cards/saved-cards.module').then((m) => m.SavedCardsModule),
  },
  // Fatura Adresim
  {
    path: 'billingAddress',
    loadChildren: () =>
      import('./features/billing-address/billing-address.module').then((m) => m.BillingAddressModule),
  },
  // Iadelerim
  {
    path: 'refunds',
    loadChildren: () =>
      import('./features/refunds/refunds.module').then((m) => m.RefundsModule),
  },
  // Bildirimlerim
  {
    path: 'notifications',
    loadChildren: () =>
      import('./features/notifications/notifications.module').then((m) => m.NotificationsModule),
  },
  // Profilim
  {
    path: 'profile',
    loadChildren: () =>
      import('./features/profile/profile.module').then((m) => m.ProfileModule),
  },
  // Anasayfa
  {
    path: 'home',
    loadChildren: () =>
      import('./features/home/home.module').then((m) => m.HomeModule),
  },
  // Varsayilan: login'e yonlendir
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
