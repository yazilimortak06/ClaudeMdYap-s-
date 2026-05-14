// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\app-routing.module.ts

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './shared_admin/auth/auth.guard';


export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./evtech/components/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./shared_admin/layout.module').then((m) => m.LayoutModule),
  },
  { path: '**', loadChildren: () =>
  import('./shared_admin/template/error-pages/error-page.module').then((m) => m.ErrorPageModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
