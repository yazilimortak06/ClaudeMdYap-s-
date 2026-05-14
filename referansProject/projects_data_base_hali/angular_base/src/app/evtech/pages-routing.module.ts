// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\pages-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from '../shared_admin/partials/layout/layout.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', loadChildren: () => import('./components/home/home-module').then((m) => m.HomeModule) },
      { path: 'announcement', loadChildren: () => import('./components/announcement/announcement.module').then((m) => m.AnnouncementModule) },
      { path: 'company', loadChildren: () => import('./components/company/company-module').then((m) => m.CompanyModule) },
      { path: 'campaign', loadChildren: () => import('./components/campaign/campaign.module').then((m) => m.CampaignModule) },
      { path: 'log', loadChildren: () => import('./components/log/log-module').then((m) => m.LogModule) },
      { path: 'messagesender', loadChildren: () => import('./components/messagesender/messagesender-module').then((m) => m.MessagesenderModule) },
      { path: 'paramaters', loadChildren: () => import('./components/paramaters/paramaters-module').then((m) => m.ParamatersModule) },
      { path: 'payments', loadChildren: () => import('./components/payments/payments-module').then((m) => m.PaymentsModule) },
      { path: 'support', loadChildren: () => import('./components/support/support-module').then((m) => m.SupportModule) },
      { path: 'reporting', loadChildren: () => import('./components/reporting/reporting-module').then((m) => m.ReportingModule) },
      { path: 'stations', loadChildren: () => import('./components/stations/stations-module').then((m) => m.StationsModule) },
      { path: 'chargeDevices', loadChildren: () => import('./components/chargeDevice/chargeDevice-module').then((m) => m.ChargeDeviceModule) },
      { path: 'authority', loadChildren: () => import('./components/authority-management/authority-module').then((m) => m.AuthorityModule) },
      { path: 'technicalSupport', loadChildren: () => import('./components/technical-support/technical-support-module').then((m) => m.TechnicalSupportModule) },
      { path: 'users', loadChildren: () => import('./components/users/users-module').then((m) => m.UsersModule) },
      { path: 'versionManagement', loadChildren: () => import('./components/version-management/version-management-module').then((m) => m.VersionManagementModule) },
      { path: 'language', loadChildren: () => import('./components/language/language.module').then((m) => m.LanguageModule) },
      { path: 'panelAdmin', loadChildren: () => import('./components/panelAdmin/panelAdmin-module').then((m) => m.PanelAdminModule) },
      { path: 'chargeDeviceReservation', loadChildren: () => import('./components/chargeDeviceReservation/chargeDeviceReservation-module').then((m) => m.ChargeDeviceReservationModule) },
      { path: 'wallet', loadChildren: () => import('./components/wallet/wallet-module').then((m) => m.WalletModule) },
      { path: 'policy', loadChildren: () => import('./components/policy/policy-module').then((m) => m.PolicyModule) },
      { path: 'chargeManagment', loadChildren: () => import('./components/chargeManagment/chargeManagment-module').then((m) => m.ChargeManagmentModule) },
      { path: 'test', loadChildren: () => import('./components/test/test.module').then((m) => m.TestModule) },
      { path: '**', redirectTo: 'error/404' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule { }
