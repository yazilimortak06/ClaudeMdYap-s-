import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Scaffold: routes boş — proje geliştirilirken doldurulacak
// Lazy loading ile feature module'ler eklenecek
const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
