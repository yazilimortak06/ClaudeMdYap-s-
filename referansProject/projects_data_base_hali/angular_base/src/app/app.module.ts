import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';

import { JwtModule, JwtHelperService } from '@auth0/angular-jwt';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';

import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { LoadingInterceptor } from './core/interceptors/loading.interceptor';
import { AuthService } from './core/services/auth.service';
import { StorageService } from './core/services/storage.service';

import { environment } from '../environments/environment';

export function tokenGetter(): string | null {
  return localStorage.getItem('access_token');
}

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function initializeApp(authService: AuthService): () => Promise<void> {
  return () =>
    new Promise((resolve) => {
      authService.checkTokenValidity();
      resolve();
    });
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    MatSnackBarModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        allowedDomains: [environment.apiUrl],
        disallowedRoutes: []
      }
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      },
      defaultLanguage: 'tr'
    })
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AuthService],
      multi: true
    },
    JwtHelperService,
    StorageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
