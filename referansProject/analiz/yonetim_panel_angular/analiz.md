# yonetim_panel_angular — Kapsamlı Analiz

## 1. Platform & Tech Stack

| Kategori | Detay |
|---|---|
| Framework | Angular (NgModule + Lazy Loading mimarisi) |
| Dil | TypeScript |
| UI Tema | Metronic Admin Template (KTLayout) |
| HTTP | HttpClientModule + JWT Interceptor |
| Auth | @auth0/angular-jwt (JwtModule) + BehaviorSubject tabanlı özel AuthenticationService |
| Location Strategy | HashLocationStrategy (#/route) |
| Form | FormsModule + ReactiveFormsModule |
| Dialog | MatDialog (@angular/material) |
| Tablo | MatTableDataSource + MatPaginator + MatSort |
| Loading | ngx-ui-loader + özel ProgressSpinner servisi |
| Translate | @ngx-translate/core |
| SVG | ng-inline-svg |
| Highlight | ngx-highlightjs |
| Dosya Yükleme | Özel pixdinn-dropzone bileşeni |
| HTML Editor | Özel pixdinn-html-editor bileşeni |
| Grafik | ng-apexcharts |
| Görüntü Zoom | ngx-image-zoom |
| API URL | http://localhost:40543/v1/ |
| Token URL | http://localhost:14493/v1/ |
| File API URL | http://localhost:12036/v1/ |
| Image URL | http://localhost:25500/v1/FileView/GetFile?code= |

## 2. Proje Yapısı

```
PixdinnManagementPanel/src/app/
  app.component.ts
  app.module.ts
  app-routing.module.ts
  _fake/
    fake-data.ts
    fake-service.ts
  core/
    adapters/
    bases/
      base-datatable/
      base-tree/
      base-tree-table/
    configs/
    date-core/
    directives/
      cell-template.directive.ts
      date-time.directive.ts
      label-control.directive.ts
      only-number.directive.ts
      only-text-or-number.directive.ts
      only-text.directive.ts
      two-digit-decimal-number.directive.ts
    external-components/
      pixdinn-dropzone/
      pixdinn-html-editor/
    pipes/
      first-letter.pipe.ts
      remove-whitespaces.pipe.ts
      replace-line-breaks.pipe.ts
      safe-html.pipe.ts
      safe.pipe.ts
      uppercase-turkish.pipe.ts
    services/
      core-utils.service.ts
    wrapper-core/
      datatable-request-core-model.ts
      datatable-result-core.model.ts
      server-result-core-model.ts
    core.module.ts
  shared_admin/
    auth/
      auth.guard.ts
      auth.module.ts
      authentication-service.ts
    partials/
      aside/
        aside.component.ts / .html / .scss
      dialogs/
        action-natification/
        evet-hayir-dialog/
        genel-sil-dialog/
        progress-spinner/
        yardim-dialog/
      footer/
      header/
        header.component.ts
        header-menu/
      header-mobile/
      layout/
        layout.component.ts / .html
        layout-init.service.ts
        layout.service.ts
        scipts-init/
        default-layout.config.ts
      layout-other-partials/
        scroll-top-partial/
        support-notification-partial/
        user-partial/
      subheader/
        subheader.component.ts
        subheader.service.ts
      topbar/
    template/
      error-pages/
        not-found-page/
        unauthorized-page/
      splash-screen/
    utils/
      directives/
        phone-mask-directive.ts
      enums/
        environment-mode.ts
        menu-icon-path.enum.ts
        message-type.enum.ts
        yes-no.ts
      interseptors/
        http-event-interseptor.ts
      pipes/
        domain-pipes/ (9 adet)
        general-pipes/ (6 adet)
      services/
        utils.service.ts
      wrapper-models/
        datatable-request-wrapper.model.ts
        datatable-response-wrapper.model.ts
        server.result.model.ts
        tree-datatable-request.wrapper.model.ts
        tree-datatable-response.wrapper.ts
    general-material.module.ts
    layout.module.ts
    shared.module.ts
  pixdinnRestaurantSystem/
    components/
      generalSystem/
        auth/
          login/
          auth-module.ts
        auth-management/
          root-admin-list/
        authority-management/
          authority-list/
          authority-module.ts
        business/
          business-add/
          business-list/
          business-update/
          business-module.ts
        category/
          category-add/
          category-list/
          category-update/
          category-module.ts
        home/
          home.component.ts
          home-module.ts
        panelAdmin/
          panelAdmin-add/
          panelAdmin-list/
          panelAdmin-update/
          panelAdminType-add/
          panelAdminType-list/
          panelAdminType-update/
          panelAdmin-module.ts
        product/
          product-add/
          product-update/
          product-module.ts
      qrMenuSystem/
        qrMenu/
          qrMenu-add/
          qrMenu-layout/
          qrMenu-layout-category-panel/
          qrMenu-layout-product-panel/
          qrMenu-list/
          qrMenu-update/
          qrMenu-module.ts
    enums/
      generalSystem/
        authentication/ (panel-admin-user-type-enum.ts)
        mediaFile/ (4 enum)
        parameter/
        socialMedia/
        versionManagment/ (3 enum)
    models/
      generalSystem/
        authentication/ (7 model)
        authority/ (10 model)
        business/ (14 model)
        category/ (14 model)
        categoryContent/ (3 model)
        contentLanguage/ (2 model)
        currency/ (1 model)
        language/ (1 model)
        mediaFile/ (2 model)
        panelAdmin/ (10 model)
        panelAdminType/ (6 model)
        policy/ (7 model)
        product/ (10+ model)
        product-category/ (2 model)
      qrMenuSystem/
        qrMenu/ (10+ model)
        allergen/ allergenProduct/ tagProduct/ vb.
    services/
      generalSystem/
        adminUserTypeAuth/
        authority/
        business/business-service.ts
        category/category-service.ts
        panelAdmin/panelAdmin-service.ts
        panelAdminType/panelAdminType-service.ts
        product/product-service.ts
      qrMenuSystem/
        qrMenu/qrMenu-service.ts
        qrMenu/qrMenu-layout-service.ts
    pages-routing.module.ts
  environments/
    environment.ts
    environment.prod.ts
```

## 3. app.module.ts — Tam Kod

```typescript
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientInMemoryWebApiModule } from 'angular-in-memory-web-api';
import { JwtModule } from '@auth0/angular-jwt';
import { ClipboardModule } from 'ngx-clipboard';
import { TranslateModule } from '@ngx-translate/core';
import { InlineSVGModule } from 'ng-inline-svg';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { environment } from 'src/environments/environment';
import { HighlightModule, HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { SharedModule } from './shared_admin/shared.module';
import { FormBuilder } from '@angular/forms';
import { NgxUiLoaderModule } from 'ngx-ui-loader';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { SplashScreenModule } from './shared_admin/template/splash-screen/splash-screen.module';
import { ProgressSpinnerModule } from './shared_admin/partials/dialogs/progress-spinner/progress-spinner.module';
import { CoreModule } from './core/core.module';
import { NgApexchartsModule } from 'ng-apexcharts/lib/ng-apexcharts.module';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { AuthModule } from './shared_admin/auth/auth.module';
import { AuthenticationService } from './shared_admin/auth/authentication-service';

export function tokenGetter() {
  return localStorage.getItem('auth_token');
}

function appInitializer(authService: AuthenticationService) {
  return () => {
    return new Promise<void>((resolve, reject) => {
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
        tokenGetter: tokenGetter,
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
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    DatePipe,
    FormBuilder,
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
```

## 4. Routing — Tam Kod

```typescript
// app-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './shared_admin/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./pixdinnRestaurantSystem/components/generalSystem/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./shared_admin/layout.module').then((m) => m.LayoutModule),
  },
  {
    path: '**',
    loadChildren: () =>
      import('./shared_admin/template/error-pages/error-page.module').then((m) => m.ErrorPageModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
```

**Routing Mimarisi:**
- `auth/` → AuthModule (login sayfası, guard'sız)
- `''` → LayoutModule (AuthGuard korumalı, tüm feature sayfaları burada lazy load edilir)
- `**` → ErrorPageModule (404 vb.)

### pages-routing.module.ts (Feature Routing)

```typescript
// LayoutModule içinden PagesRoutingModule import edilir.
// Feature modüller lazy load ile yüklenir:
// business → BusinessModule
// category → CategoryModule
// product → ProductModule
// panelAdmin → PanelAdminModule
// authority → AuthorityModule
// qrMenu → QrMenuModule
// home → HomeModule
```

## 5. Shared Admin — Her Dosya Tam Kod

### 5.1 AuthGuard

```typescript
// shared_admin/auth/auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthenticationService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentUser = this.authService.adminValue;
    if (currentUser && this.authService.getAuthFromLocalStorage()) {
      return true;
    }
    this.authService.logout();
    return false;
  }
}
```

**Dikkat:** `getAuthFromLocalStorage()` Observable döndürüyor ama guard içinde Observable değil truthy/falsy olarak değerlendiriliyor — Observable her zaman truthy'dir. Bu potansiyel bir bug: logout asla çağrılmayabilir. Düzeltilmesi gerekiyor.

### 5.2 AuthenticationService — Tam Kod

```typescript
// shared_admin/auth/authentication-service.ts
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, finalize, map, Observable, of } from "rxjs";
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { AuthenticationModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/authentication-model";
import { LoginRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/login-request-model";
import { LoginResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/login-response-model";
import { PrepareLoginFormRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/prepare-login-form-request-model";
import { PrepareLoginFormResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/prepare-login-form-response-model";
import { PanelAdminModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/panelAdmin/panelAdmin-model";
import { environment } from "src/environments/environment";

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class AuthenticationService {

  public adminSubject: BehaviorSubject<AuthenticationModel> = new BehaviorSubject<AuthenticationModel>(null);
  isLoadingSubject: BehaviorSubject<boolean>;
  private authLocalStorageToken = "auth_token";
  private authUserInfo = "user_info";
  private srvUrl: string;

  constructor(private http: HttpClient, private router: Router) {
    this.adminSubject = new BehaviorSubject<AuthenticationModel>(null);
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.srvUrl = environment.apiUrl + "Authentication/";
  }

  public get adminValue(): AuthenticationModel {
    return this.adminSubject.value;
  }

  public setAuthFromLocalStorage(auth: AuthenticationModel): boolean {
    if (auth && auth.token) {
      localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      return true;
    }
    return false;
  }

  public getAuthFromLocalStorage(): Observable<AuthenticationModel> {
    try {
      const authData: AuthenticationModel = JSON.parse(
        localStorage.getItem(this.authLocalStorageToken)
      );
      return of(authData);
    } catch (error) {
      this.logout();
      return of(undefined);
    }
  }

  logout() {
    if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
      this.userLogout().subscribe(auth => { });
    }
    localStorage.removeItem(this.authLocalStorageToken);
    localStorage.removeItem(this.authUserInfo);
    this.adminSubject = new BehaviorSubject<AuthenticationModel>(undefined);
    this.router.navigate(['/auth/login']);
  }

  getAdminByToken(): Observable<AuthenticationModel> {
    const auth = this.getAuthFromLocalStorage();
    if (!auth) {
      return of(undefined);
    }
    this.isLoadingSubject.next(true);
    return this.getAuthFromLocalStorage().pipe(
      map((admin: AuthenticationModel) => {
        if (admin) {
          this.adminSubject = new BehaviorSubject<AuthenticationModel>(admin);
        } else {
          this.logout();
        }
        return admin;
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  public getAdminInfoFromLocalStorage(): Observable<PanelAdminModel> {
    try {
      const authData: PanelAdminModel = JSON.parse(
        localStorage.getItem(this.authUserInfo)
      );
      return of(authData);
    } catch (error) {
      this.logout();
      return of(undefined);
    }
  }

  getNewTokenWithRefreshToken() {
    if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
      const token: AuthenticationModel = JSON.parse(
        localStorage.getItem(this.authLocalStorageToken)
      );
      this.adminLoginRefreshToken(token).subscribe(auth => {
        this.setAuthFromLocalStorage(auth);
      });
    }
  }

  clearTokenFromLocalStorage() {
    localStorage.removeItem(this.authLocalStorageToken);
  }

  prepareLoginForm(loginRequest: PrepareLoginFormRequestModel) {
    return this.http.post<PrepareLoginFormResponseModel>(this.srvUrl + "LoginForm", loginRequest, httpOptions);
  }

  logIn(loginRequest: LoginRequestModel) {
    return this.http.post<LoginResponseModel>(this.srvUrl + "Login", loginRequest, httpOptions);
  }

  userLogout() {
    return this.http.post<any>(this.srvUrl + "LogOut", null, httpOptions);
  }

  adminLoginRefreshToken(refreshToken: AuthenticationModel): Observable<AuthenticationModel> {
    return this.http.post<any>(this.srvUrl + "RefreshTokenLogin", refreshToken, httpOptions);
  }

  userLoginRefreshToken(refreshToken: AuthenticationModel): Observable<AuthenticationModel> {
    return this.http.post<any>("RefreshTokenLogin", refreshToken, httpOptions);
  }
}
```

### 5.3 HTTP Interceptor — Tam Kod

```typescript
// shared_admin/utils/interseptors/http-event-interseptor.ts
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor,
         HttpRequest, HttpResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { ProgressSpinnerService } from '../../partials/dialogs/progress-spinner/progress-spinner.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/authentication-model';

@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {

  private authLocalStorageToken = "auth_token";

  constructor(
    private authService: AuthenticationService,
    public srvProgressSpinner: ProgressSpinnerService
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authenticationModel: AuthenticationModel = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
    console.log(authenticationModel)
    if (authenticationModel) {
      request = this.addTokenHeader(request, authenticationModel);
    }
    return next.handle(request).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          console.log(event)
          this.srvProgressSpinner.hide()
        }
      }, (error: any) => {
        if (error.status === 400) {
          const applicationError = error.error;
          this.srvProgressSpinner.hide()
        }
        else if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.handle401Error(request, next);
        } else {
          const applicationError = error.error;
          this.srvProgressSpinner.hide()
        }
      })
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    const authenticationModel: AuthenticationModel = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
    if (authenticationModel) {
      this.authService.clearTokenFromLocalStorage();
      this.authService.logout();
    } else {
      this.srvProgressSpinner.hide();
      this.authService.logout();
    }
  }

  private addTokenHeader(request: HttpRequest<any>, authenticationModel: AuthenticationModel) {
    if (this.authService.adminSubject !== undefined) {
      try {
        const headers = new HttpHeaders()
          .set('Authorization', 'Bearer ' + authenticationModel.token.token)
        return request = request.clone({ headers });
      }
      catch {
        this.authService.logout();
      }
    } else {
      const headers = new HttpHeaders()
        .set('Authorization', 'Bearer ' + authenticationModel.token.token);
      return request = request.clone({ headers });
    }
  }
}

export const httpEventInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: HttpEventInterceptor,
  multi: true
};
```

### 5.4 AsideComponent — Tam Kod

```typescript
// shared_admin/partials/aside/aside.component.ts
import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { LayoutService } from '../layout/layout.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { PanelAdminUserType } from 'src/app/pixdinnRestaurantSystem/enums/generalSystem/authentication/panel-admin-user-type-enum';
import { AuthenticationModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/authentication-model';

@Component({
  selector: 'app-aside',
  templateUrl: './aside.component.html',
  styleUrls: ['./aside.component.scss'],
})
export class AsideComponent implements OnInit {
  disableAsideSelfDisplay: boolean;
  headerLogo: string;
  brandSkin: string;
  ulCSSClasses: string;
  location: Location;
  asideMenuHTMLAttributes: any = {};
  asideMenuCSSClasses: string;
  asideMenuDropdown;
  brandClasses: string;
  asideMenuScroll = 1;
  asideSelfMinimizeToggle = false;

  private authLocalStorageToken = "auth_token";
  authenticationData: AuthenticationModel = new AuthenticationModel();
  panelAdminUserType = PanelAdminUserType;

  constructor(private layout: LayoutService, private authService: AuthenticationService, private loc: Location) {
    this.authenticationData = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
  }

  ngOnInit(): void {
    this.disableAsideSelfDisplay = this.layout.getProp('aside.self.display') === false;
    this.brandSkin = this.layout.getProp('brand.self.theme');
    this.headerLogo = this.getLogo();
    this.ulCSSClasses = this.layout.getProp('aside_menu_nav');
    this.asideMenuCSSClasses = this.layout.getStringCSSClasses('aside_menu');
    this.asideMenuHTMLAttributes = this.layout.getHTMLAttributes('aside_menu');
    this.asideMenuDropdown = this.layout.getProp('aside.menu.dropdown') ? '1' : '0';
    this.brandClasses = this.layout.getProp('brand');
    this.asideSelfMinimizeToggle = this.layout.getProp('aside.self.minimize.toggle');
    this.asideMenuScroll = this.layout.getProp('aside.menu.scroll') ? 1 : 0;
    this.location = this.loc;
  }

  private getLogo() {
    if (this.brandSkin === 'light') {
      // return './assets/project-assets/evtechlogo.svg';
    } else {
      // return './assets/project-assets/evtechlogowhite.svg';
    }
    return "";
  }
}
```

**Dikkat:** AsideComponent HTML template içinde `authenticationData.panelAdminUserType` değerine göre menü öğeleri koşullu gösterilir. Enum değerleri: `ADMIN_USER`, `PLACE_ADMIN_USER`, `ROOT_ADMIN_USER`.

### 5.5 LayoutComponent — Tam Kod

```typescript
// shared_admin/partials/layout/layout.component.ts
import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import KTLayoutContent from '../../../../assets/js/layout/base/content';
import { DatatableRequestWrapper } from '../../utils/wrapper-models/datatable-request-wrapper.model';
import { LayoutInitService } from './layout-init.service';
import { LayoutService } from './layout.service';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, filter, map, tap } from 'rxjs';
import { AuthenticationModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/authentication-model';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit, AfterViewInit {
  selfLayout = 'default';
  asideSelfDisplay: true;
  asideMenuStatic: false;
  contentClasses = '';
  contentContainerClasses = '';
  subheaderDisplay = true;
  contentExtended: false;
  asideCSSClasses: string;
  asideHTMLAttributes: any = {};
  headerMobileClasses = '';
  headerMobileAttributes = {};
  footerDisplay: boolean;
  footerCSSClasses: string;
  headerCSSClasses: string;
  headerHTMLAttributes: any = {};
  userPartialDisplay = true;

  @ViewChild('ktAside', { static: true }) ktAside: ElementRef;
  @ViewChild('ktHeaderMobile', { static: true }) ktHeaderMobile: ElementRef;
  @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

  admin: AuthenticationModel;

  constructor(
    private initService: LayoutInitService,
    public layout: LayoutService,
  ) {
    this.initService.init();
  }

  ngOnInit(): void {
    this.selfLayout = this.layout.getProp('self.layout');
    this.asideSelfDisplay = this.layout.getProp('aside.self.display');
    this.asideMenuStatic = this.layout.getProp('aside.menu.static');
    this.subheaderDisplay = this.layout.getProp('subheader.display');
    this.contentClasses = this.layout.getStringCSSClasses('content');
    this.contentContainerClasses = this.layout.getStringCSSClasses('content_container-fluid');
    this.contentExtended = this.layout.getProp('content.extended');
    this.asideHTMLAttributes = this.layout.getHTMLAttributes('aside');
    this.asideCSSClasses = this.layout.getStringCSSClasses('aside');
    this.headerMobileClasses = this.layout.getStringCSSClasses('header_mobile');
    this.headerMobileAttributes = this.layout.getHTMLAttributes('header_mobile');
    this.footerDisplay = this.layout.getProp('footer.display');
    this.footerCSSClasses = this.layout.getStringCSSClasses('footer');
    this.headerCSSClasses = this.layout.getStringCSSClasses('header');
    this.headerHTMLAttributes = this.layout.getHTMLAttributes('header');
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
  }

  ngOnDestroy() { }

  ngAfterViewInit(): void {
    if (this.ktAside) {
      for (const key in this.asideHTMLAttributes) {
        if (this.asideHTMLAttributes.hasOwnProperty(key)) {
          this.ktAside.nativeElement.attributes[key] = this.asideHTMLAttributes[key];
        }
      }
    }
    if (this.ktHeaderMobile) {
      for (const key in this.headerMobileAttributes) {
        if (this.headerMobileAttributes.hasOwnProperty(key)) {
          this.ktHeaderMobile.nativeElement.attributes[key] = this.headerMobileAttributes[key];
        }
      }
    }
    if (this.ktHeader) {
      for (const key in this.headerHTMLAttributes) {
        if (this.headerHTMLAttributes.hasOwnProperty(key)) {
          this.ktHeader.nativeElement.attributes[key] = this.headerHTMLAttributes[key];
        }
      }
    }
    KTLayoutContent.init('kt_content');
  }
}
```

### 5.6 LayoutModule

```typescript
// shared_admin/layout.module.ts
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
import { CoreModule } from '../core/core.module';
import { AsideComponent } from './partials/aside/aside.component';
import { ScriptsInitComponent } from './partials/layout/scipts-init/scripts-init.component';
import { SubheaderModule } from './partials/subheader/subheader.module';
import { LayoutOtherPartialsModule } from './partials/layout-other-partials/layout-other-partials-module';
import { httpEventInterceptorProvider } from './utils/interseptors/http-event-interseptor';
import { SharedModule } from './shared.module';
import { PagesRoutingModule } from '../pixdinnRestaurantSystem/pages-routing.module';

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
```

**Dikkat:** `httpEventInterceptorProvider` import edilmiş ama `providers[]`'a eklenmemiş — interceptor çalışmayabilir. Bu bir bug'dır.

## 6. Feature Modülü — Tam Kod

### 6.1 Authentication Models

**AuthenticationModel:**
```typescript
// models/generalSystem/authentication/authentication-model.ts
import { OperatorFunction, MonoTypeOperatorFunction } from "rxjs";
import { AccessTokenModel } from "./access-token-model";
import { PanelAdminUserType } from "src/app/pixdinnRestaurantSystem/enums/generalSystem/authentication/panel-admin-user-type-enum";

export class AuthenticationModel {
  pipe(arg0: OperatorFunction<AuthenticationModel, AuthenticationModel>, arg1: MonoTypeOperatorFunction<unknown>): import("rxjs").Observable<AuthenticationModel> {
    throw new Error("Method not implemented.");
  }
  token: AccessTokenModel;
  name: string;
  surname: string;
  phone: string;
  mail: string;
  companyName: string;
  connectionId: string;
  businessId: number;
  qrMenuId: number;
  panelAdminUserType: PanelAdminUserType;
}
```

**AccessTokenModel:**
```typescript
// models/generalSystem/authentication/access-token-model.ts
export class AccessTokenModel {
  token: string;
  expiration: Date;
  refreshToken: string;
  refreshTokenExpiration: Date;
}
```

**LoginRequestModel:**
```typescript
// models/generalSystem/authentication/login-request-model.ts
export class LoginRequestModel {
  userName: string;
  password: string;
  loginFormKey: string;
}
```

### 6.2 Business Models

**BusinessModel:**
```typescript
export class BusinessModel {
  id: number;
  name: string;
  aboutUs: string;
  isActive: boolean;
}
```

### 6.3 PanelAdmin Model

```typescript
export class PanelAdminModel {
  name: string;
  userName: string;
  surname: string;
  mail: string;
  phone: string;
  panelAdminTypeId: number;
  isActive: boolean;
  accessToken: string;
  refreshToken: string;
}
```

### 6.4 PanelAdminUserType Enum

```typescript
// enums/generalSystem/authentication/panel-admin-user-type-enum.ts
export enum PanelAdminUserType {
  ADMIN_USER = "ADMIN_USER",
  PLACE_ADMIN_USER = "PLACE_ADMIN_USER",
  ROOT_ADMIN_USER = "ROOT_ADMIN_USER"
}

export enum AdminManagmentType {
  MAIN_ADMIN = "MAIN_ADMIN",
  BUSINESS_ADMIN = "BUSINESS_ADMIN",
  MENU_ADMIN = "MENU_ADMIN"
}

export const adminManagmentTypeMapping: Record<keyof typeof AdminManagmentType, string> = {
  MAIN_ADMIN: "Genel Admin",
  BUSINESS_ADMIN: "İşletme Admin",
  MENU_ADMIN: "Menü Admin"
};
```

### 6.5 LoginComponent — Tam Kod

```typescript
// components/generalSystem/auth/login/login.component.ts
import { ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { BehaviorSubject, finalize, Observable, Subject, Subscription, takeUntil, tap } from 'rxjs';
import { SplashScreenService } from 'src/app/shared_admin/template/splash-screen/splash-screen.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/authentication-model';
import { LoginRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/login-request-model';
import { LoginResponseModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/login-response-model';
import { PrepareLoginFormRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/prepare-login-form-request-model';
import { PrepareLoginFormResponseModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/authentication/prepare-login-form-response-model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  prepareLoginFormRequestModel: PrepareLoginFormRequestModel = new PrepareLoginFormRequestModel();
  loginRequestModel: LoginRequestModel = new LoginRequestModel();
  private unsubscribe: Subscription[] = [];
  loginForm: FormGroup;
  errors: any = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvAuthService: AuthenticationService,
    private srvUtils: UtilsService,
    private router: Router,
    private splashScreenService: SplashScreenService,
  ) {
    if (this.srvAuthService.adminValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.splashScreenService.hide();
    this.initForm();
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  initForm() {
    this.loginForm = this.fb.group({
      userName: [null, Validators.compose([Validators.required])],
      password: [null, Validators.compose([Validators.required])]
    });
    this.prepareLoginForm();
  }

  prepareLoginForm() {
    this.srvProgressSpinner.show();
    this.srvAuthService.prepareLoginForm(this.prepareLoginFormRequestModel).subscribe((response: PrepareLoginFormResponseModel) => {
      this.loginRequestModel.loginFormKey = response.loginFormKey;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorPrepareLoginForm(error);
    });
  }

  onErrorPrepareLoginForm(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  login() {
    this.srvProgressSpinner.show();
    this.srvAuthService.isLoadingSubject.next(true);
    const controls = this.loginForm.controls;
    if (this.loginForm.invalid) {
      Object.keys(controls).forEach(controlName => controls[controlName].markAsTouched());
      this.srvProgressSpinner.hide();
      return;
    }
    const loginSubscr = this.srvAuthService
      .logIn(this.loginRequestModel).pipe()
      .subscribe((auth: LoginResponseModel) => {
        if (auth.accessToken) {
          var authenticationModel = new AuthenticationModel();
          authenticationModel.token = auth.accessToken;
          authenticationModel.name = auth.name;
          authenticationModel.surname = auth.surname;
          authenticationModel.phone = auth.phone;
          authenticationModel.mail = auth.mail;
          authenticationModel.connectionId = auth.connectionId;
          authenticationModel.companyName = auth.companyName;
          authenticationModel.businessId = auth.businessId;
          authenticationModel.qrMenuId = auth.qrMenuId;
          authenticationModel.panelAdminUserType = auth.panelAdminUserType;
          this.srvAuthService.setAuthFromLocalStorage(authenticationModel);
          this.srvAuthService.adminSubject = new BehaviorSubject<AuthenticationModel>(authenticationModel);
          this.router.navigate(['']);
        }
        this.srvAuthService.isLoadingSubject.next(false);
      },
        (error) => {
          this.onErrorLogin(error);
          this.srvAuthService.isLoadingSubject.next(false);
          this.srvProgressSpinner.hide();
        });
    this.unsubscribe.push(loginSubscr);
  }

  onErrorLogin(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  isControlHasError(controlName: string, validationType: string): boolean {
    const control = this.loginForm.controls[controlName];
    if (!control) {
      return false;
    }
    const result = control.hasError(validationType) && (control.dirty || control.touched);
    return result;
  }
}
```

**Login Akışı:**
1. `ngOnInit` → `initForm()` → `prepareLoginForm()` — backend'den loginFormKey alır (CSRF/form key)
2. Kullanıcı form doldurur → `login()` çağrılır
3. `srvAuthService.logIn(loginRequestModel)` → HTTP POST → backend
4. Başarı: AuthenticationModel oluşturulur, localStorage'a yazılır, adminSubject güncellenir, `router.navigate([''])` ile panel'e yönlendirilir
5. Hata: `showActionNotification()` ile hata mesajı gösterilir

### 6.6 BusinessListComponent — Tam Kod

```typescript
// components/generalSystem/business/business-list/business-list.component.ts
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { Router } from '@angular/router';
import { merge, tap } from 'rxjs';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { AddBusinessComponent } from '../business-add/business-add.component';
import { UpdateBusinessComponent } from '../business-update/business-update.component';
import { adminManagmentTypeMapping, AdminManagmentType } from 'src/app/pixdinnRestaurantSystem/enums/generalSystem/authentication/panel-admin-user-type-enum';
import { BusinessFilterModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/business/business-filter-model';
import { BusinessListModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/business/business-list-model';
import { ChangeStateBusinessRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/business/change-state-business-request-model';
import { RemoveBusinessRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/business/remove-business-request-model';
import { BusinessService } from 'src/app/pixdinnRestaurantSystem/services/generalSystem/business/business-service';

@Component({
  selector: 'app-business-list',
  templateUrl: './business-list.component.html',
  styleUrls: ['./business-list.component.scss']
})
export class BusinessListComponent implements OnInit {
  showingColumnNamesBusinessTable: string[] = ['Id', 'Name', 'CreatedDate', 'UpdatedDate', 'IsActive', 'Edit'];
  businessList: MatTableDataSource<BusinessListModel> = new MatTableDataSource();
  totalRecordSizeBusinessTable: number;
  @ViewChild(MatPaginator) paginatorBusinessTable: MatPaginator;
  @ViewChild(MatSort) sortBusinessTable: MatSort;
  searchCriterBusiness: DatatableRequestWrapper<BusinessFilterModel> = new DatatableRequestWrapper();

  adminManagmentTypeMapping = adminManagmentTypeMapping;
  adminManagmentTypes = Object.values(AdminManagmentType).filter(value => typeof value === 'string');
  adminManagmentType = AdminManagmentType;

  removeBusinessRequestModel: RemoveBusinessRequestModel = new RemoveBusinessRequestModel();
  changeStateBusinessRequestModel: ChangeStateBusinessRequestModel = new ChangeStateBusinessRequestModel();

  constructor(
    private srvProgressSpinner: ProgressSpinnerService,
    private srvBusinessService: BusinessService,
    private srvUtils: UtilsService,
    private router: Router,
    public dialog: MatDialog,
    private subheader: SubHeaderService,
    public yesNoDialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'İşletme Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'İşletmeler', linkPath: null, linkText: '', isActive: false },
    ]);
    this.searchCriterBusiness.data = new BusinessFilterModel();
    this.srvProgressSpinner.show();
    this.getBusinesss(false);
  }

  getBusinesss(hasFilter: boolean) {
    this.businessList.data = [];
    this.srvProgressSpinner.show();
    if (hasFilter) { this.paginatorBusinessTable.pageIndex = 0; }
    if (this.sortBusinessTable && this.sortBusinessTable.direction != "" && this.sortBusinessTable.active != "") {
      this.searchCriterBusiness.orderDirective = this.sortBusinessTable.direction;
      this.searchCriterBusiness.orderProperty = this.sortBusinessTable.active;
    } else {
      this.searchCriterBusiness.orderDirective = "desc";
      this.searchCriterBusiness.orderProperty = "Id";
    }
    if (this.paginatorBusinessTable) {
      this.searchCriterBusiness.recordPerPage = this.paginatorBusinessTable.pageSize;
      this.searchCriterBusiness.pageNumber = this.paginatorBusinessTable.pageIndex;
    } else {
      this.searchCriterBusiness.recordPerPage = 15;
    }
    this.srvBusinessService.list(this.searchCriterBusiness).subscribe((response: DatatableResponseWrapper<BusinessListModel[]>) => {
      this.businessList.data = response.data;
      this.totalRecordSizeBusinessTable = response.recordCount;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorBusinessList(error);
    });
  }

  ngAfterViewInit(): void {
    this.sortBusinessTable.sortChange.subscribe(() => { this.paginatorBusinessTable.pageIndex = 0 });
    merge(this.sortBusinessTable.sortChange, this.paginatorBusinessTable.page)
      .pipe(tap(() => { this.getBusinesss(false); }))
      .subscribe(() => { }, () => null);
  }

  onErrorBusinessList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  clearParameters() {
    this.searchCriterBusiness.data.name = null;
    this.searchCriterBusiness.data.isActive = null;
    this.getBusinesss(true);
  }

  onEditBtnClickedDialog(id): void {
    const dialogRef = this.dialog.open(UpdateBusinessComponent, {
      width: '1200px',
      panelClass: 'form-modal',
      data: { id: id },
    });
    const subUpdateEvent = dialogRef.componentInstance.onUpdate.subscribe(() => { this.getBusinesss(false); });
    dialogRef.afterClosed().subscribe(result => { subUpdateEvent.unsubscribe(); });
  }

  onNewBtnClickedDialog(): void {
    const dialogRef = this.dialog.open(AddBusinessComponent, {
      width: '1200px',
      panelClass: 'form-modal',
      data: {},
    });
    const subAddEvent = dialogRef.componentInstance.onAdd.subscribe(() => { this.getBusinesss(false); });
    dialogRef.afterClosed().subscribe(result => { subAddEvent.unsubscribe(); });
  }

  removeBusiness(id) {
    this.srvProgressSpinner.show();
    this.removeBusinessRequestModel.id = id;
    this.srvBusinessService.removeBusiness(this.removeBusinessRequestModel).subscribe((response: any) => {
      this.getBusinesss(false);
      this.srvUtils.showActionNotification("Başarıyla Kaldırıldı", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorRemoveBusiness(error);
    });
  }

  removeBusinessDialog(id) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "İşletmeyi kaldırmak istediğinize emin misiniz? " },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) { this.removeBusiness(id); }
    });
  }

  changeActiveState(id, isActive, ob: MatSlideToggleChange) {
    this.srvProgressSpinner.show();
    this.changeStateBusinessRequestModel.id = id;
    this.srvBusinessService.changeStateBusiness(this.changeStateBusinessRequestModel).subscribe((response: any) => {
      this.businessList.data.filter(x => x.id == id).map(item => { item.isActive = !isActive; });
      this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      ob.source.checked = isActive;
      this.onErrorChangeActiveState(error);
    });
  }

  changeActiveStateDialog(id, isActive, ob: MatSlideToggleChange) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "İşletmenin durumunu değiştirmek istediğinize emin misiniz? " },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) { this.changeActiveState(id, isActive, ob); }
      else { ob.source.checked = isActive; }
    });
  }

  onErrorRemoveBusiness(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  onErrorChangeActiveState(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }
}
```

### 6.7 BusinessService — Tam Kod

```typescript
// services/generalSystem/business/business-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
// ... tüm model importları

@Injectable()
export class BusinessService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "BusinessManagment/";
  }

  list(request: DatatableRequestWrapper<BusinessFilterModel>): Observable<DatatableResponseWrapper<BusinessListModel[]>> {
    return this.http.post<DatatableResponseWrapper<BusinessListModel[]>>(this.srvUrl + "GetBusinessDatatablePanel", request);
  }
  getBusinessForUpdate(request: GetBusinessForUpdateRequestModel): Observable<GetBusinessForUpdateResponseModel> {
    return this.http.post<GetBusinessForUpdateResponseModel>(this.srvUrl + "GetBusinessForUpdate", request);
  }
  getBusinessForSelectList(request: GetBusinessForSelectListRequestModel): Observable<GetBusinessForSelectListResponseModel> {
    return this.http.post<GetBusinessForSelectListResponseModel>(this.srvUrl + "GetBusinessForSelectList", request);
  }
  removeBusiness(request: RemoveBusinessRequestModel): Observable<RemoveBusinessResponseModel> {
    return this.http.post<RemoveBusinessResponseModel>(this.srvUrl + "RemoveBusiness", request);
  }
  changeStateBusiness(request: ChangeStateBusinessRequestModel): Observable<ChangeStateBusinessResponseModel> {
    return this.http.post<ChangeStateBusinessResponseModel>(this.srvUrl + "ChangeStateBusiness", request);
  }
  update(request: UpdateBusinessRequestModel): Observable<UpdateBusinessResponseModel> {
    return this.http.post<UpdateBusinessResponseModel>(this.srvUrl + "UpdateBusiness", request);
  }
  add(request: AddBusinessRequestModel): Observable<AddBusinessResponseModel> {
    return this.http.post<AddBusinessResponseModel>(this.srvUrl + "AddBusiness", request);
  }
}
```

### 6.8 QrMenuService — Tam Kod

```typescript
// services/qrMenuSystem/qrMenu/qrMenu-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
// ... tüm model importları

@Injectable()
export class QrMenuService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "QrMenuManagment/";
  }

  list(request: DatatableRequestWrapper<QrMenuFilterModel>): Observable<DatatableResponseWrapper<QrMenuListModel[]>> {
    return this.http.post<DatatableResponseWrapper<QrMenuListModel[]>>(this.srvUrl + "GetQrMenuDatatablePanel", request);
  }
  prepareQrMenuUpdateForm(request): Observable<PrepareQrMenuUpdateFormResponseModel> {
    return this.http.post<PrepareQrMenuUpdateFormResponseModel>(this.srvUrl + "PrepareQrMenuUpdateForm", request);
  }
  prepareQrMenuInsertForm(request): Observable<PrepareQrMenuInsertFormResponseModel> {
    return this.http.post<PrepareQrMenuInsertFormResponseModel>(this.srvUrl + "PrepareQrMenuInsertForm", request);
  }
  prepareQrMenuLayoutPage(request): Observable<PrepareQrMenuLayoutPageResponseModel> {
    return this.http.post<PrepareQrMenuLayoutPageResponseModel>(this.srvUrl + "PrepareQrMenuLayoutPage", request);
  }
  getQrMenuForSelectList(request): Observable<GetQrMenuForSelectListResponseModel> {
    return this.http.post<GetQrMenuForSelectListResponseModel>(this.srvUrl + "GetQrMenuForSelectList", request);
  }
  changeStateQrMenu(request): Observable<ChangeStateQrMenuResponseModel> {
    return this.http.post<ChangeStateQrMenuResponseModel>(this.srvUrl + "ChangeStateQrMenu", request);
  }
  changeProductRankingStatus(request): Observable<ChangeProductRankingStatusResponseModel> {
    return this.http.post<ChangeProductRankingStatusResponseModel>(this.srvUrl + "ChangeProductRankingStatus", request);
  }
  update(request): Observable<UpdateQrMenuResponseModel> {
    return this.http.post<UpdateQrMenuResponseModel>(this.srvUrl + "UpdateQrMenu", request);
  }
  add(request): Observable<AddQrMenuResponseModel> {
    return this.http.post<AddQrMenuResponseModel>(this.srvUrl + "AddQrMenu", request);
  }
}
```

### 6.9 ProductService — Tam Kod

```typescript
// services/generalSystem/product/product-service.ts
@Injectable()
export class ProductService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "ProductManagment/";
  }

  add(request: AddProductRequestModel): Observable<AddProductResponseModel> {
    return this.http.post<AddProductResponseModel>(this.srvUrl + "AddProduct", request);
  }
  update(request: UpdateProductRequestModel): Observable<UpdateProductResponseModel> {
    return this.http.post<UpdateProductRequestModel>(this.srvUrl + "UpdateProduct", request);
  }
  prepareProductInsertForm(request: PrepareProductInsertFormRequestModel): Observable<PrepareProductInsertFormResponseModel> {
    return this.http.post<PrepareProductInsertFormResponseModel>(this.srvUrl + "PrepareProductInsertForm", request);
  }
  changeStateProduct(request: ChangeStateProductRequestModel): Observable<ChangeStateProductResponseModel> {
    return this.http.post<ChangeStateProductResponseModel>(this.srvUrl + "ChangeStateProduct", request);
  }
  prepareProductUpdateForm(request: PrepareProductUpdateFormRequestModel): Observable<PrepareProductUpdateFormResponseModel> {
    return this.http.post<PrepareProductUpdateFormResponseModel>(this.srvUrl + "PrepareProductUpdateForm", request);
  }
}
```

### 6.10 Datatable Wrapper Modelleri

```typescript
// shared_admin/utils/wrapper-models/datatable-request-wrapper.model.ts
import { DatatableRequestWrapperCore } from "src/app/core/wrapper-core/datatable-request-core-model";

export class DatatableRequestWrapper<T> extends DatatableRequestWrapperCore<T> {
  // pageNumber, recordPerPage, orderProperty, orderDirective, data: T
  // core modelden miras alır
}

// shared_admin/utils/wrapper-models/datatable-response-wrapper.model.ts
import { DatatableResponseWrapperCore } from "src/app/core/wrapper-core/datatable-result-core.model";

export class DatatableResponseWrapper<T> extends DatatableResponseWrapperCore<T> {
  // data: T, recordCount: number
  // core modelden miras alır
}
```

## 7. Environment

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  appVersion: 'v717demo1',
  isTest: false,
  apiUrl: 'http://localhost:40543/v1/',
  tockenUrl: 'http://localhost:14493/v1/',
  fileApiUrl: 'http://localhost:12036/v1/',
  fileBaseUrl: 'http://localhost:12036',
  imageUrl: 'http://localhost:25500/v1/FileView/GetFile?code=',
  logApiUrl: "http://localhost:9622/v1/",
};
```

**Mimari:** Birden fazla mikroservis endpoint'i var — main API, token API, file API, image API, log API. Yerel geliştirme için localhost portları kullanılıyor.

## 8. Mimari Kararlar

### Temel Mimari Kararlar

1. **Lazy Loading Mimarisi** — `app-routing.module.ts`'de tüm feature modüller lazy load edilir. Auth ve Layout iki ana entry point. Feature modüller `PagesRoutingModule` içinden çözülür.

2. **HashLocationStrategy** — `#/route` formatı. Server-side rewrite gerektirmez. Admin panel için pratik.

3. **APP_INITIALIZER ile Token Restore** — Uygulama başladığında `AuthenticationService.getAdminByToken()` çağrılır, localStorage'dan token okunur ve adminSubject yenilenir. Sayfa yenilemede oturumu korur.

4. **PrepareForm Pattern** — Login öncesi `PrepareLoginForm` endpoint'i çağrılır ve `loginFormKey` alınır. Bu CSRF benzeri form key mekanizması — tüm form submit'lerde bu key gönderilir.

5. **BehaviorSubject tabanlı Auth State** — `adminSubject: BehaviorSubject<AuthenticationModel>` ile global auth state yönetilir. `adminValue` getter ile anlık değer okunur.

6. **Dialog-Based CRUD** — Ekleme ve güncelleme işlemleri route-based değil, `MatDialog` açılarak yapılır. Dialog kapandıktan sonra liste yenilenir.

7. **DatatableRequestWrapper generic pattern** — Tüm liste servis çağrıları generic `DatatableRequestWrapper<FilterModel>` ve `DatatableResponseWrapper<ListModel[]>` kullanır. Pagination, sorting ve filter tek modelde toplanır.

8. **SubheaderService ile Breadcrumb** — Her list component `ngOnInit()`'te `subheader.setBreadcrumbs([...])` çağırarak breadcrumb'ı günceller.

9. **ProgressSpinnerService** — HTTP öncesi `.show()`, response/error'da `.hide()` çağrılır. Interceptor da hide yapar.

10. **Çok Servis Mimarisi** — API, Token, File, Image, Log için ayrı URL'ler. Mikroservis tabanlı backend.

### Bilinen Sorunlar ve Dikkat Noktaları

- `AuthGuard.canActivate()` — `getAuthFromLocalStorage()` Observable döndürüyor, guard içinde doğrudan boolean olarak değerlendirilmesi yanlış
- `LayoutModule`'da `httpEventInterceptorProvider` import edilmiş ama providers[]'a eklenmemiş
- `console.log()` çağrıları production kodunda kalmış — temizlenmeli
- `AuthenticationModel.pipe()` metodu throw Error ile tanımlanmış — bu bir anti-pattern
- `category-list.component.ts` tamamen yorum satırı — incomplete feature
- `logout()` içindeki condition: `if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token")` — authLocalStorageToken her zaman "auth_token" stringine eşit, dolayısıyla `userLogout()` hiç çağrılmıyor
