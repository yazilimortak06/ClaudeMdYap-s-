// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\app.module.ts
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
