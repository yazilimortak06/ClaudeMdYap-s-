import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

// Feature module'leri lazy loading ile yüklenir
// Her module kendi routing'ini yönetir
const routes: Routes = [
  {
    path: '',
    redirectTo: 'charge',
    pathMatch: 'full'
  },
  {
    path: 'charge',
    loadChildren: () =>
      import('./modules/charge/charge.module').then(m => m.ChargeModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'refund',
    loadChildren: () =>
      import('./modules/refund/refund.module').then(m => m.RefundModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'transaction',
    loadChildren: () =>
      import('./modules/transaction/transaction.module').then(m => m.TransactionModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'support-ticket',
    loadChildren: () =>
      import('./modules/support-ticket/support-ticket.module').then(m => m.SupportTicketModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./modules/auth/auth.module').then(m => m.AuthModule)
    // auth module'de guard yok — login sayfası public
  },
  {
    path: '**',
    redirectTo: 'charge'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
