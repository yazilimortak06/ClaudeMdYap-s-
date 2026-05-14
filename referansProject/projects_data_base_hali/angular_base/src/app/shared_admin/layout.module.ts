// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\layout.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg';
import { NgbDropdownModule, NgbProgressbarModule } from '@ng-bootstrap/ng-bootstrap';
import { ProgressSpinnerModule } from './partials/dialogs/progress-spinner/progress-spinner.module';
import { FooterComponent } from './partials/footer/footer.component';
import { HeaderMobileComponent } from './partials/header-mobile/header-mobile.component';
import { HeaderMenuComponent } from './partials/header/header-menu/header-menu.component';
import { HeaderComponent } from './partials/header/header.component';
import { LayoutComponent } from './partials/layout/layout.component';
import { TopbarComponent } from './partials/topbar/topbar.component';
import { PagesRoutingModule } from '../evtech/pages-routing.module';
import { CoreModule } from '../core/core.module';
import { AsideComponent } from './partials/aside/aside.component';
import { ScriptsInitComponent } from './partials/layout/scipts-init/scripts-init.component';
import { SubheaderModule } from './partials/subheader/subheader.module';
import { LayoutOtherPartialsModule } from './partials/layout-other-partials/layout-other-partials-module';
import { httpEventInterceptorProvider } from './utils/interseptors/http-event-interseptor';
import { SharedModule } from './shared.module';

@NgModule({
  declarations: [
    LayoutComponent,
    ScriptsInitComponent,
    HeaderMobileComponent,
    AsideComponent,
    FooterComponent,
    HeaderComponent,
    HeaderMenuComponent,
    TopbarComponent,
  ],
  providers: [],
  imports: [
    CommonModule,
    PagesRoutingModule,
    InlineSVGModule,
    LayoutOtherPartialsModule,
    NgbDropdownModule,
    NgbProgressbarModule,
    CoreModule,
    SubheaderModule,
    ProgressSpinnerModule,
    SharedModule,
  ],
})
export class LayoutModule { }
