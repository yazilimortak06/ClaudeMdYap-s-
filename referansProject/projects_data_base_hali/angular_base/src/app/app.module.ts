// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\app.module.ts

import { NgModule, APP_INITIALIZER } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// import { HttpClientModule } from '@angular/common/http';
import { HttpClientInMemoryWebApiModule } from 'angular-in-memory-web-api';
import { JwtModule } from '@auth0/angular-jwt';
import { ClipboardModule } from 'ngx-clipboard';
import { TranslateModule } from '@ngx-translate/core';
import { InlineSVGModule } from 'ng-inline-svg';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { environment } from 'src/environments/environment';
// Highlight JS
import { HighlightModule, HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';

import { HashLocationStrategy, LocationStrategy } from '@angular/common';
// import { CommonService } from './core/services/common.service';
import { SharedModule } from './shared_admin/shared.module';
import { FormBuilder } from '@angular/forms';
import { NgxUiLoaderModule } from 'ngx-ui-loader';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { SplashScreenModule } from './shared_admin/template/splash-screen/splash-screen.module';
import { ProgressSpinnerModule } from './shared_admin/partials/dialogs/progress-spinner/progress-spinner.module';
import { CoreModule } from './core/core.module';
import { NgApexchartsModule } from 'ng-apexcharts/lib/ng-apexcharts.module';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { LoginComponent } from './evtech/components/auth/login/login.component';
import { AuthModule } from './shared_admin/auth/auth.module';
import { AuthenticationService } from './shared_admin/auth/authentication-service';
import { MatDateModule } from './core/date-core/mat-date.module';
// #fake-end#

// function appInitializer(authService: AuthService) {
//   return () => {
//     return new Promise((resolve) => {
//       authService.getUserByToken().subscribe().add(resolve);
//     });
//   };
// }
export function tokenGetter() {
  return localStorage.getItem('auth_token');
}

function appInitializer(authService: AuthenticationService) {
  return () => {
    return new Promise<void>((resolve,reject) => {
      authService.getAdminByToken().subscribe().add(resolve);
    });
  };
}
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SplashScreenModule,
    MatDateModule,
    TranslateModule.forRoot(),
    HttpClientModule,
    HighlightModule,
    ClipboardModule,
    NgxUiLoaderModule,
    ProgressSpinnerModule,
    InlineSVGModule.forRoot(),
    NgbModule,
    CoreModule,
    AuthModule,
    JwtModule.forRoot({
      config: {
        // eslint-disable-next-line object-shorthand
        tokenGetter: tokenGetter,
        // whitelistedDomains: ['hsysapitest.saglik.gov.tr', 'hsysapiv2.saglik.gov.tr', 'localhost:5000', 'localhost:5002', 'localhost:5004', 'localhost:5006', 'localhost:5008', 'localhost:5010', 'localhost:5012', 'localhost:5014', 'localhost:5016', 'localhost:5018'],
        // blacklistedRoutes: []
      }
    }),
    AppRoutingModule,

  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      multi: true,
      deps: [AuthenticationService],
    },
    {provide: LocationStrategy, useClass: HashLocationStrategy},
    DatePipe, FormBuilder,
    {
      provide: HIGHLIGHT_OPTIONS,
      useValue: {
        coreLibraryLoader: () => import('highlight.js/lib/core')

      },
    },

  ],
  bootstrap: [AppComponent],

})
export class AppModule { }
