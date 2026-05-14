# sarj_ev_panel_angular Analiz

## Genel Bakış

EvTechPanelAltunkaya, elektrikli araç şarj istasyonu yönetim paneli için geliştirilmiş bir Angular 13 admin uygulamasıdır. Metronic v7.1.7 admin şablonu üzerine inşa edilmiştir. Sistem şarj cihazlarını (ChargeDevice), şarj istasyonlarını (Station), şarj işlemlerini (ChargeManagement), kullanıcıları, rezervasyonları, kampanyaları, ödemeleri ve duyuruları yönetir. OCPP (Open Charge Point Protocol) protokolü üzerinden gerçek zamanlı şarj cihazı uzak yönetimi (remote management) ve SignalR ile anlık durum bildirimleri sunmaktadır.

**Temel Özellikler:**
- Şarj istasyonu ve şarj cihazı CRUD yönetimi
- OCPP protokolü üzerinden uzaktan şarj cihazı kontrolü (start/stop transaction, reset, changeAvailability, changeConfiguration, triggerMessage)
- SignalR ile gerçek zamanlı cihaz durum bildirimleri
- JWT tabanlı kimlik doğrulama (HashLocationStrategy ile hash routing)
- Dashboard: aylık şarj istatistikleri, rezervasyon verileri, harita üzerinde istasyon görüntüleme
- Çok şirketli yapı (companyId bazlı yetkilendirme)
- Cüzdan (wallet), ödeme ve kampanya yönetimi

---

## Teknoloji Stack

**package.json'dan:**

```json
{
  "name": "metronic-angular-demo1",
  "version": "7.1.7",
  "dependencies": {
    "@angular/core": "~13.1.2",
    "@angular/router": "~13.1.2",
    "@angular/forms": "~13.1.2",
    "@angular/material": "^11.0.0",
    "@angular/cdk": "^11.0.0",
    "@agm/core": "^1.1.0",
    "@auth0/angular-jwt": "^5.0.2",
    "@microsoft/signalr": "^6.0.1",
    "@ng-bootstrap/ng-bootstrap": "^8.0.0",
    "@ngx-translate/core": "^13.0.0",
    "apexcharts": "^3.20.0",
    "ng-apexcharts": "^1.5.1",
    "bootstrap": "^4.6.1",
    "chart.js": "^4.2.1",
    "moment": "^2.27.0",
    "moment-timezone": "^0.5.27",
    "ng2-currency-mask": "^13.0.3",
    "ngx-dropzone": "^3.0.0",
    "ngx-image-zoom": "^0.6.0",
    "ngx-ui-loader": "^11.0.0",
    "rxjs": "~7.4.0",
    "ng-inline-svg": "^10.1.0",
    "ngx-clipboard": "^13.0.1",
    "ngx-perfect-scrollbar": "^9.0.0",
    "ngx-highlightjs": "^4.1.3",
    "ts-md5": "^1.2.11"
  },
  "devDependencies": {
    "@angular/cli": "^13.1.3",
    "typescript": "~4.5.4"
  }
}
```

**Özet:**
- Angular 13 + TypeScript 4.5
- Angular Material 11 (UI bileşenleri)
- Angular CDK (koleksiyon, ağaç yapısı)
- Metronic 7.1.7 admin şablonu
- SignalR (@microsoft/signalr 6.0.1) — gerçek zamanlı bildirimler
- JWT (@auth0/angular-jwt) — token tabanlı auth
- Google Maps (@agm/core) — istasyon harita görünümü
- ApexCharts (ng-apexcharts) — dashboard grafikleri
- Bootstrap 4 + ng-bootstrap
- ngx-translate — i18n desteği
- moment/moment-timezone — tarih işlemleri
- ngx-dropzone — dosya yükleme
- ng2-currency-mask — para birimi input maskesi

---

## Klasör Yapısı

```
src/app/
├── core/                          # Genel altyapı katmanı
│   ├── core.module.ts
│   ├── adapters/
│   │   └── MOMENT_DATE_FORMATS.ts
│   ├── bases/
│   │   ├── base-datatable/
│   │   │   ├── base-datatable-base-model.ts
│   │   │   └── base-datatable.ts
│   │   ├── base-tree/
│   │   └── base-tree-table/
│   ├── configs/
│   │   └── custom-currency-mask.config.ts
│   ├── date-core/
│   │   └── mat-date.module.ts
│   ├── directives/
│   │   ├── cell-template.directive.ts
│   │   ├── date-time.directive.ts
│   │   ├── label-control.directive.ts
│   │   ├── only-number.directive.ts
│   │   ├── only-text-or-number.directive.ts
│   │   ├── only-text.directive.ts
│   │   └── two-digit-decimal-number.directive.ts
│   ├── external-components/
│   │   ├── dropzone-shared.module.ts
│   │   ├── html-editor-shared.module.ts
│   │   ├── pixdinn-dropzone/
│   │   └── pixdinn-html-editor/
│   ├── pipes/
│   │   ├── first-letter.pipe.ts
│   │   ├── remove-whitespaces.pipe.ts
│   │   ├── replace-line-breaks.pipe.ts
│   │   ├── safe-html.pipe.ts
│   │   ├── safe.pipe.ts
│   │   └── uppercase-turkish.pipe.ts
│   ├── services/
│   │   └── core-utils.service.ts
│   └── wrapper-core/
│       ├── datatable-request-core-model.ts
│       ├── datatable-result-core.model.ts
│       └── server-result-core-model.ts
│
├── shared_admin/                  # Admin şablon katmanı (Metronic)
│   ├── auth/
│   │   ├── auth.guard.ts
│   │   ├── auth.module.ts
│   │   └── authentication-service.ts
│   ├── partials/
│   │   ├── aside/
│   │   ├── dialogs/
│   │   │   ├── action-natification/
│   │   │   ├── evet-hayir-dialog/
│   │   │   ├── genel-sil-dialog/
│   │   │   ├── progress-spinner/
│   │   │   └── yardim-dialog/
│   │   ├── footer/
│   │   ├── header/
│   │   ├── header-mobile/
│   │   ├── layout/
│   │   ├── layout-other-partials/
│   │   ├── subheader/
│   │   └── topbar/
│   ├── template/
│   │   ├── error-pages/
│   │   └── splash-screen/
│   ├── utils/
│   │   ├── enums/
│   │   ├── interseptors/
│   │   │   └── http-event-interseptor.ts
│   │   ├── pipes/
│   │   │   ├── domain-pipes/  (30+ domain-specific pipe)
│   │   │   └── general-pipes/
│   │   ├── services/
│   │   │   └── utils.service.ts
│   │   └── wrapper-models/
│   ├── general-material.module.ts
│   ├── layout.module.ts
│   └── shared.module.ts
│
├── evtech/                        # Domain / feature katmanı
│   ├── components/
│   │   ├── announcement/
│   │   ├── auth/
│   │   │   └── login/
│   │   ├── auth-management/
│   │   ├── authority-management/
│   │   ├── campaign/
│   │   ├── chargeDevice/          # Şarj cihazı yönetimi (en kapsamlı)
│   │   ├── chargeDeviceReservation/
│   │   ├── chargeManagment/
│   │   ├── company/
│   │   ├── home/                  # Dashboard
│   │   ├── language/
│   │   ├── live-monitoring/
│   │   ├── log/
│   │   ├── messagesender/
│   │   ├── panelAdmin/
│   │   ├── paramaters/
│   │   ├── payments/
│   │   ├── policy/
│   │   ├── reporting/
│   │   ├── stations/              # İstasyon yönetimi
│   │   ├── support/
│   │   ├── system-parameter/
│   │   ├── technical-support/
│   │   ├── test/
│   │   ├── users/
│   │   ├── version-management/
│   │   ├── wallet/
│   │   └── webSite-contact/
│   ├── enums/                     # Domain enum'ları
│   ├── models/                    # Domain model sınıfları
│   └── services/                  # HTTP servis katmanı
│
├── _fake/                         # Metronic demo/fake veriler
├── app.component.ts
├── app.module.ts
└── app-routing.module.ts
```

---

## app.module.ts

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
import { LoginComponent } from './evtech/components/auth/login/login.component';
import { AuthModule } from './shared_admin/auth/auth.module';
import { AuthenticationService } from './shared_admin/auth/authentication-service';
import { MatDateModule } from './core/date-core/mat-date.module';

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
```

---

## app-routing.module.ts

```typescript
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

---

## Core Katmanı

### core.module.ts

```typescript
import { LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FirstLetterPipe } from './pipes/first-letter.pipe';
import { SafePipe } from './pipes/safe.pipe';
import { CellTemplateDirective } from './directives/cell-template.directive';
import { TextOrNumberDirective } from './directives/only-text-or-number.directive';
import { TextDirective } from './directives/only-text.directive';
import { RemovewhitespacesPipe } from './pipes/remove-whitespaces.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { UppercaseturkishPipe } from './pipes/uppercase-turkish.pipe';
import { NumberDirective } from './directives/only-number.directive';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter } from '@angular/material/core';
import { MOMENT_DATE_FORMATS } from './adapters/MOMENT_DATE_FORMATS';
import { NgxDropzoneModule, NgxDropzoneComponent } from 'ngx-dropzone';

@NgModule({
  declarations: [
    FirstLetterPipe,
    SafePipe,
    NumberDirective,
    TextOrNumberDirective,
    TextDirective,
    CellTemplateDirective,
    RemovewhitespacesPipe,
    UppercaseturkishPipe,
    SafeHtmlPipe,
  ],
  imports: [CommonModule],
  exports: [
    FirstLetterPipe, SafePipe, NumberDirective,
    TextOrNumberDirective, TextDirective,
    RemovewhitespacesPipe, UppercaseturkishPipe, SafeHtmlPipe
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'tr-TR' },
    { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
    { provide: MAT_DATE_FORMATS, useValue: MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentDateAdapter },
  ]
})
export class CoreModule { }
```

### base-datatable-base-model.ts

```typescript
export class BaseDataTableBaseModel {
    rowUniqueId: number;
    isSelected: boolean;
    isDetailOpened: boolean;
}
```

### base-datatable.ts

```typescript
import { BaseDataTableBaseModel } from 'src/app/core/bases/base-datatable/base-datatable-base-model';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { BehaviorSubject, merge } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DatatableRequestWrapperCore } from '../../wrapper-core/datatable-request-core-model';
import { DatatableResponseWrapperCore } from '../../wrapper-core/datatable-result-core.model';

export class DataTableBase<TFilter, DataModel extends BaseDataTableBaseModel> {

    showingColumnNames: string[] = [];
    dataList: MatTableDataSource<DataModel> = new MatTableDataSource();
    totalRecordSize: number;
    paginatorTable: MatPaginator;
    sortTable: MatSort;
    searchCriter: DatatableRequestWrapperCore<TFilter> = new DatatableRequestWrapperCore();
    obj: any;
    receivedDataFromServer = (callback: any, obj: any) => {};

    constructor(
        showingColumnNames: string[],
        obj: any,
        callback: any
    ) {
        this.showingColumnNames = showingColumnNames;
        this.obj = obj;
        this.receivedDataFromServer = callback;
    }

    public pagingInit(sortTable: MatSort, paginatorTable: MatPaginator) {
        this.sortTable = sortTable;
        this.paginatorTable = paginatorTable;

        this.sortTable.sortChange.subscribe(
            () => { this.paginatorTable.pageIndex = 0 }
        );
        merge(this.sortTable.sortChange, this.paginatorTable.page)
            .pipe(tap(() => { this.dataCallback(); }))
            .subscribe(() => {}, () => null);
    }

    public dataCallback() {
        this.receivedDataFromServer(() => {}, this.obj);
    }

    public setDataSource(response: DatatableResponseWrapperCore<DataModel[]>): void {
        this.totalRecordSize = response.recordCount;
        this.dataList.data = response.data;
    }

    public getFilterData(filterData: TFilter): DatatableRequestWrapperCore<TFilter> {
        this.searchCriter.data = filterData;

        if (this.sortTable) {
            this.searchCriter.orderDirective = this.sortTable.direction;
            this.searchCriter.orderProperty = this.sortTable.active;
        }

        if (this.paginatorTable) {
            this.searchCriter.recordPerPage = this.paginatorTable.pageSize;
            this.searchCriter.pageNumber = this.paginatorTable.pageIndex;
        } else {
            this.searchCriter.recordPerPage = 10;
            this.searchCriter.pageNumber = 0;
        }

        return this.searchCriter;
    }
}
```

### datatable-request-core-model.ts

```typescript
export class DatatableRequestWrapperCore<T> {
    constructor() {
        this.data = {} as T;
    }
    data: T;
    orderDirective: string;
    orderProperty: string;
    pageNumber: number;
    recordPerPage: number;
    offset: number;
}
```

### datatable-result-core.model.ts

```typescript
export class DatatableResponseWrapperCore<T> {
    data: T;
    recordCount: number;
}
```

### server-result-core-model.ts

```typescript
export class ServerResultCoreModel {
    resultType: ServerResultType;
    errorCode: number;
    errorMessage: string;
}

enum ServerResultType {
    Ok = "Ok",
    Error = "Error",
    ValidationError = "ValidationError",
    Unauthorized = "Unauthorized",
    Exception = "Exception"
}
```

### only-number.directive.ts

```typescript
import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: 'input[numbersOnly]'
})
export class NumberDirective {
  private navigationKeys = [
    'Backspace', 'Delete', 'Tab', 'Escape', 'Enter',
    'Home', 'End', 'ArrowLeft', 'ArrowRight', 'Clear', 'Copy', 'Paste',
  ];
  constructor(private _el: ElementRef) { }

  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (
      this.navigationKeys.indexOf(e.key) > -1 ||
      (e.key === 'a' && e.ctrlKey === true) ||
      (e.key === 'c' && e.ctrlKey === true) ||
      (e.key === 'v' && e.ctrlKey === true) ||
      (e.key === 'x' && e.ctrlKey === true) ||
      (e.key === 'a' && e.metaKey === true) ||
      (e.key === 'c' && e.metaKey === true) ||
      (e.key === 'v' && e.metaKey === true) ||
      (e.key === ',') ||
      (e.key === 'x' && e.metaKey === true)
    ) {
      return;
    }
    if (e.key === ' ' || isNaN(Number(e.key))) {
      e.preventDefault();
    }
  }
}
```

### safe-html.pipe.ts

```typescript
import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({ name: 'safeHtml' })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitized: DomSanitizer) { }
  transform(value: any) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
}
```

### cell-template.directive.ts

```typescript
import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
    selector: 'ng-template[cellTemplate]'
})
export class CellTemplateDirective {
    @Input('cellTemplate') name: string;
    constructor(public template: TemplateRef<any>) { }
}
```

### core-utils.service.ts

```typescript
import { Injectable } from '@angular/core';

@Injectable()
export class CoreUtilsService {
    constructor() { }

    public async readFile(file: File): Promise<string | ArrayBuffer> {
        return new Promise<string | ArrayBuffer>((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = e => resolve((e.target as FileReader).result);
            reader.onerror = e => {
                console.error(`FileReader failed on file ${file.name}.`);
                return reject(null);
            };
            if (!file) {
                console.error('No file to read.');
                return reject(null);
            }
            reader.readAsDataURL(file);
        });
    }

    getMonths(): {} {
        return [
            {name:"Ocak",value:1},   {name:"Şubat",value:2},
            {name:"Mart",value:3},   {name:"Nisan",value:4},
            {name:"Mayıs",value:5},  {name:"Haziran",value:6},
            {name:"Temmuz",value:7}, {name:"Ağustos",value:8},
            {name:"Eylül",value:9},  {name:"Ekim",value:10},
            {name:"Kasım",value:11}, {name:"Aralık",value:12}
        ];
    }
}
```

---

## Shared Admin Katmanı

### authentication-service.ts

```typescript
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, finalize, map, Observable, of } from "rxjs";
import { environment } from "src/environments/environment";
import { AuthenticationModel } from "../../evtech/models/authentication/authentication-model";
import { LoginRequestModel } from "../../evtech/models/authentication/login-request-model";
import { LoginResponseModel } from "../../evtech/models/authentication/login-response-model";
import { PrepareLoginFormRequestModel } from "../../evtech/models/authentication/prepare-login-form-request-model";
import { PrepareLoginFormResponseModel } from "../../evtech/models/authentication/prepare-login-form-response-model";
import { PanelAdminModel } from "../../evtech/models/panelAdmin/panelAdmin-model";

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
            this.userLogout().subscribe(auth => {});
        }
        localStorage.removeItem(this.authLocalStorageToken);
        localStorage.removeItem(this.authUserInfo);
        this.adminSubject = new BehaviorSubject<AuthenticationModel>(undefined);
        this.router.navigate(['/auth/login']);
    }

    getAdminByToken(): Observable<AuthenticationModel> {
        const auth = this.getAuthFromLocalStorage();
        if (!auth) { return of(undefined); }
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
            const authData: PanelAdminModel = JSON.parse(localStorage.getItem(this.authUserInfo));
            return of(authData);
        } catch (error) {
            this.logout();
            return of(undefined);
        }
    }

    getNewTokenWithRefreshToken() {
        if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
            const token: AuthenticationModel = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
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

### auth.guard.ts

```typescript
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthenticationService) {}

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

### http-event-interseptor.ts

```typescript
import {
    HttpErrorResponse, HttpEvent, HttpHandler, HttpHeaders,
    HttpInterceptor, HttpRequest, HttpResponse, HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ProgressSpinnerService } from '../../partials/dialogs/progress-spinner/progress-spinner.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';

@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {
    private authLocalStorageToken = "auth_token";

    constructor(
        private authService: AuthenticationService,
        public srvProgressSpinner: ProgressSpinnerService
    ) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authenticationModel: AuthenticationModel = JSON.parse(
            localStorage.getItem(this.authLocalStorageToken)
        );
        console.log(authenticationModel);
        if (authenticationModel) {
            request = this.addTokenHeader(request, authenticationModel);
        }
        return next.handle(request).pipe(
            tap((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                    console.log(event);
                    this.srvProgressSpinner.hide();
                }
            }, (error: any) => {
                if (error.status === 400) {
                    const applicationError = error.error;
                    this.srvProgressSpinner.hide();
                } else if (error instanceof HttpErrorResponse && error.status === 401) {
                    return this.handle401Error(request, next);
                } else {
                    const applicationError = error.error;
                    this.srvProgressSpinner.hide();
                }
            })
        );
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        const authenticationModel: AuthenticationModel = JSON.parse(
            localStorage.getItem(this.authLocalStorageToken)
        );
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
                    .set('Authorization', 'Bearer ' + authenticationModel.token.token);
                return request = request.clone({ headers });
            } catch {
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

### utils.service.ts

```typescript
import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActionNotificationComponent } from '../../partials/dialogs/action-natification/action-notification.component';
import { EvetHayirDialogComponent } from '../../partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { GenelSilDialogComponent } from '../../partials/dialogs/genel-sil-dialog/genel-sil-dialog.component';
import { YardimDialogComponent } from '../../partials/dialogs/yardim-dialog/yardim-dialog.component';
import { EnumMessageType } from '../enums/message-type.enum';
import { ServerResultModel } from '../wrapper-models/server.result.model';

@Injectable()
export class UtilsService {
    constructor(private snackBar: MatSnackBar, private dialog: MatDialog) {}

    roundAccurately(number, decimalPlaces) {
        return Number(Math.round(Number(number + "e" + decimalPlaces)) + "e-" + decimalPlaces);
    }

    showActionNotification(
        message: string,
        type: EnumMessageType = EnumMessageType.Info,
        duration: number = 3000,
        showCloseButton: boolean = true,
        showUndoButton: boolean = false,
        undoButtonDuration: number = 3000,
        verticalPosition: 'top' | 'bottom' = 'top',
    ) {
        return this.snackBar.openFromComponent(ActionNotificationComponent, {
            duration: duration,
            panelClass: [
                type == EnumMessageType.Success ? 'green-snackbar' :
                type == EnumMessageType.Info ? 'blue-snackbar' :
                type == EnumMessageType.Error ? 'red-snackbar' :
                type == EnumMessageType.Warning ? 'yellow-snackbar' : 'mat-snack-bar-container'
            ],
            data: {
                message, snackBar: this.snackBar,
                showCloseButton, showUndoButton,
                undoButtonDuration, verticalPosition, type, action: 'Undo'
            },
            verticalPosition: verticalPosition
        });
    }

    generalDeleteDialog(title: string = '', description: string = '', waitDesciption: string = '', service: any, id: number) {
        return this.dialog.open(GenelSilDialogComponent, {
            data: { title, description, waitDesciption, service, id },
            width: '440px'
        });
    }

    yesNoDialog(title: string = '', description: string = '', waitDesciption: string = '') {
        return this.dialog.open(EvetHayirDialogComponent, {
            data: { title, description, waitDesciption },
            width: '440px'
        });
    }

    helperDialog(description: string = '') {
        event.stopPropagation();
        return this.dialog.open(YardimDialogComponent, {
            data: { description },
            width: '440px'
        });
    }

    convertStringListToString(mesajList: []): string {
        let mesaj: string = "<ul>";
        mesajList.forEach((element: string) => { mesaj += "<li>" + element + "</li>"; });
        return mesaj + "</ul>";
    }

    getServerErrorRequest(data: any): ServerResultModel {
        let dataError = data as HttpErrorResponse;
        return dataError.error as ServerResultModel;
    }

    showServerError(data: ServerResultModel) {
        this.showActionNotification(data.errorMessage, EnumMessageType.Error);
    }

    parseSelectValueJson(jsonString) {
        return JSON.parse(jsonString);
    }

    parseBoolean(value): boolean {
        try {
            value = value + "";
            return JSON.parse(value.toLowerCase());
        } catch (err) {
            return false;
        }
    }

    isEmptyOrSpaces(str) {
        return str === null || str.trim() === '';
    }
}
```

### layout.module.ts

```typescript
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
import { AuthenticationService } from './auth/authentication-service';
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
```

### shared.module.ts — Domain Pipe Listesi

SharedModule, şarj sistemi domain'ine özel 30+ pipe içerir:

| Pipe | Açıklama |
|------|----------|
| `ConnectionStatePipe` | Şarj cihazı bağlantı durumu metni |
| `ConnectionStateCssPipe` | Bağlantı durumu CSS sınıfı |
| `ChargeDeviceOcppStatePipe` | OCPP durum metni (AVAILABLE/UNAVAILABLE/FAULTED) |
| `ChargeDeviceOcppStateCssPipe` | OCPP durum CSS sınıfı |
| `ChargeDeviceInstantStatePipe` | Anlık cihaz durumu metni |
| `ChargeDeviceInstantStateCssPipe` | Anlık cihaz durumu CSS |
| `ConnectorStatePipe` | Konnektör durum metni |
| `ConnectorStateCssPipe` | Konnektör durum CSS |
| `ChargeDeviceConnectorOcppStatePipe` | Konnektör OCPP durum metni |
| `ChargeDeviceConnectorInstantStatePipe` | Konnektör anlık durum |
| `PaymentStatusCssPipe` | Ödeme durumu CSS |
| `WalletProcessStateCssPipe` | Cüzdan işlem durumu CSS |
| `WalletPullMoneyStateCssPipe` | Para çekme durumu CSS |
| `ChargeStateCssPipe` | Şarj süreci durumu CSS |
| `ChargeApplicationTypeCssPipe` | Şarj uygulama tipi CSS |
| `StationTypeCssPipe` | İstasyon tipi CSS |
| `SupportStateCssPipe` | Destek talebi durum CSS |
| `ImageSrcPipe` | Dosya sunucusundan resim URL'i oluşturur |
| `GoogleMapMarkerCssPipe` | Harita marker CSS |
| `PanelAdminManagmentTypePipe` | Panel admin yönetim tipi metni |
| `VersionPuplicationCssPipe` | Versiyon yayın durumu CSS |
| `OcppMessageTypeCssPipe` | OCPP mesaj tipi CSS |

---

## Feature Modülleri

### pages-routing.module.ts

```typescript
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
```

### auth-module.ts

```typescript
@NgModule({
    imports: [
        SharedModule, DropzoneSharedModule, ReactiveFormsModule,
        MatButtonModule, MatInputModule, MatFormFieldModule, MatCheckboxModule,
        CurrencyMaskModule, FormsModule, NgxUiLoaderModule, ProgressSpinnerModule,
        MatProgressBarModule, MatProgressSpinnerModule, CommonModule,
        PagesRoutingModule, InlineSVGModule, LayoutOtherPartialsModule,
        NgbDropdownModule, NgbProgressbarModule, CoreModule, SplashScreenModule,
        AgmCoreModule.forRoot({ apiKey: 'AIzaSyAnlZ6NTAvHnJJBjoej7vU_CT-wWbl0zPU&sensor' }),
        RouterModule.forChild([
            { path: 'login', component: LoginComponent }
        ])
    ],
    providers: [UtilsService, ProgressSpinnerService, MatSnackBar, httpEventInterceptorProvider],
    declarations: [LoginComponent]
})
export class AuthModule { }
```

### login.component.ts

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Subscription } from 'rxjs';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { SplashScreenService } from 'src/app/shared_admin/template/splash-screen/splash-screen.service';
import { PrepareLoginFormRequestModel } from 'src/app/evtech/models/authentication/prepare-login-form-request-model';
import { LoginRequestModel } from 'src/app/evtech/models/authentication/login-request-model';
import { LoginResponseModel } from 'src/app/evtech/models/authentication/login-response-model';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';

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
        this.srvAuthService.prepareLoginForm(this.prepareLoginFormRequestModel)
            .subscribe((response: PrepareLoginFormResponseModel) => {
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
        const loginSubscr = this.srvAuthService.logIn(this.loginRequestModel).pipe()
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
                    authenticationModel.companyId = auth.companyId;
                    authenticationModel.panelAdminUserType = auth.panelAdminUserType;
                    this.srvAuthService.setAuthFromLocalStorage(authenticationModel);
                    this.srvAuthService.adminSubject = new BehaviorSubject<AuthenticationModel>(authenticationModel);
                    this.router.navigate(['']);
                }
                this.srvAuthService.isLoadingSubject.next(false);
            }, (error) => {
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
        if (!control) { return false; }
        return control.hasError(validationType) && (control.dirty || control.touched);
    }
}
```

### home-module.ts (Dashboard)

```typescript
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { HomeComponent } from './home.component';
import { AgmCoreModule, GoogleMapsAPIWrapper } from '@agm/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { DashboardService } from '../../services/dashboard/dashboard-service';
import { CompanyService } from '../../services/company/company-service';
import { DashboardMonthlyChargeProcessComponent } from './dashboardStats/dashboard-monthly-charge-process/dashboard-monthly-charge-process.component';
import { DashboardMonthlyReservationComponent } from './dashboardStats/dashboard-monthly-reservation/dashboard-monthly-reservation.component';
import { DashboardSumOfProcessComponent } from './dashboardStats/dashboard-sum-of-process/dashboard-sum-of-process.component';

@NgModule({
    imports: [
        CommonModule, SharedModule, InlineSVGModule, NgApexchartsModule, NgbDropdownModule,
        AgmCoreModule.forRoot({ apiKey: 'AIzaSyAnlZ6NTAvHnJJBjoej7vU_CT-wWbl0zPU&sensor' }),
        RouterModule.forChild([{ path: '', component: HomeComponent }])
    ],
    providers: [UtilsService, DashboardService, GoogleMapsAPIWrapper, LayoutService, CompanyService],
    declarations: [
        HomeComponent,
        DashboardMonthlyChargeProcessComponent,
        DashboardMonthlyReservationComponent,
        DashboardSumOfProcessComponent
    ]
})
export class HomeModule { }
```

### chargeDevice-module.ts (En Kapsamlı Feature)

```typescript
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ChargeDeviceListComponent } from "./chargeDevice-list/chargeDevice-list.component";
import { UpdateChargeDeviceComponent } from "./chargeDevice-update/chargeDevice-update.component";
import { AddChargeDeviceComponent } from "./chargeDevice-add/chargeDevice-add.component";
import { ChargeDeviceRemoteManagmentComponent } from "./chargeDevice-remote-managment/chargeDevice-remote-managment.component";
import { RemoteChargeManagmentComponent } from "./remote-charge-managment/remote-charge-managment.component";
import { ChargeDeviceCommandComponent } from "./chargeDevice-command/chargeDevice-command.component";
import { ChargeDeviceCommandListComponent } from "./chargeDevice-command-list/chargeDevice-command-list.component";
import { ChargeDeviceConnectorListPartialComponent } from "./chargeDevice-connector-list-partial/chargeDevice-connector-list-partial.component";
import { ConnectorRemoteManagmentPartialComponent } from "./connector-remote-managment-partial/connector-remote-managment-partial.component";
import { ChargeDeviceRemoteResetModalComponent } from "./chargeDevice-remote-reset-modal/chargeDevice-remote-reset-modal.component";
import { ConnectorPriceRequestInfoComponent } from "./connector-price-request-info/connector-price-request-info.component";
import { ChargeDeviceChangeConfigurationComponent } from "./chargeDevice-change-configuration/chargeDevice-change-configuration.component";
import { ChargeDeviceTriggerMessageComponent } from "./chargeDevice-trigger-message/chargeDevice-trigger-message.component";

@NgModule({
    imports: [
        CommonModule, SharedModule, DropzoneSharedModule, CurrencyMaskModule,
        AgmCoreModule.forRoot({ apiKey: 'AIzaSyAnlZ6NTAvHnJJBjoej7vU_CT-wWbl0zPU&sensor' }),
        RouterModule.forChild([
            { path: 'list', component: ChargeDeviceListComponent },
            { path: 'add', component: AddChargeDeviceComponent },
            { path: 'update', component: UpdateChargeDeviceComponent },
            { path: 'remoteManagment', component: ChargeDeviceRemoteManagmentComponent },
            { path: 'remoteChargeManagment', component: RemoteChargeManagmentComponent }
        ])
    ],
    providers: [
        UtilsService, StationManagmentService, ChargeDeviceService, GoogleMapsAPIWrapper,
        ContentLanguageService, CountryCityAndTownService,
        ChargeDeviceRemoteManagmentService, ChargeDeviceStatusNotificationService,
        ChargeDeviceCommandManagmentService, ChargeDeviceRemoteService,
        ConnectorService, CompanyService, UserService, RemoteChargeManagmentService,
        { provide: CURRENCY_MASK_CONFIG, useValue: CustomCurrencyMaskConfig }
    ],
    declarations: [
        ChargeDeviceListComponent, AddChargeDeviceComponent, UpdateChargeDeviceComponent,
        ChargeDeviceRemoteManagmentComponent, ChargeDeviceCommandComponent,
        ChargeDeviceCommandListComponent, ChargeDeviceConnectorListPartialComponent,
        ConnectorRemoteManagmentPartialComponent, ChargeDeviceRemoteResetModalComponent,
        ConnectorPriceRequestInfoComponent, RemoteChargeManagmentComponent,
        ChargeDeviceChangeConfigurationComponent, ChargeDeviceTriggerMessageComponent
    ]
})
export class ChargeDeviceModule { }
```

### stations-module.ts

```typescript
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { StationListComponent } from "./station-list/station-list.component";
import { AddStationComponent } from "./station-add/station-add.component";
import { UpdateStationComponent } from "./station-update/station-update.component";
import { ChargeTransactionSelectableOptionComponent } from "./charge-transaction-selectable-options-list/charge-transaction-selectable-options-list.component";
import { ChargeTransactionSelectableOptionAddComponent } from "./charge-transaction-hours-add/charge-transaction-selectable-option-add.component";
import { ChargeTransactionSelectableKwComponent } from "./charge-transaction-selectable-kw-list/charge-transaction-selectable-kw-list.component";
import { ChargeTransactionSelectablePriceComponent } from "./charge-transaction-selectable-price-list/charge-transaction-selectable-price-list.component";

@NgModule({
    imports: [
        CommonModule, SharedModule, DropzoneSharedModule, CurrencyMaskModule,
        AgmCoreModule.forRoot({ apiKey: 'AIzaSyAnlZ6NTAvHnJJBjoej7vU_CT-wWbl0zPU&sensor' }),
        RouterModule.forChild([
            { path: 'list', component: StationListComponent },
            { path: 'add', component: AddStationComponent },
            { path: 'update', component: UpdateStationComponent },
            { path: 'chargeTransactionSelectableOptions', component: ChargeTransactionSelectableOptionComponent }
        ])
    ],
    providers: [
        UtilsService, StationManagmentService, GoogleMapsAPIWrapper,
        ContentLanguageService, CountryCityAndTownService,
        ChargeTransactionSelectableOptionsService, CompanyService, AuthenticationService,
        { provide: CURRENCY_MASK_CONFIG, useValue: CustomCurrencyMaskConfig }
    ],
    declarations: [
        StationListComponent, AddStationComponent, UpdateStationComponent,
        ChargeTransactionSelectableOptionComponent,
        ChargeTransactionSelectableOptionAddComponent,
        ChargeTransactionSelectableKwComponent,
        ChargeTransactionSelectablePriceComponent
    ]
})
export class StationsModule { }
```

---

## Servisler (evtech/services)

### chargeDevice-service.ts

```typescript
@Injectable()
export class ChargeDeviceService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ChargeDevice/";
    }

    list(searchCriter: DatatableRequestWrapper<ChargeDeviceFilterModel>): Observable<DatatableResponseWrapper<ChargeDeviceListModel[]>> {
        return this.http.post<DatatableResponseWrapper<ChargeDeviceListModel[]>>(this.srvUrl + "GetChargeDeviceDataTablePanel", searchCriter);
    }
    chargeDevicePaymentList(searchCriter: DatatableRequestWrapper<ChargeDeviceFilterModel>): Observable<DatatableResponseWrapper<StationPaymentListModel[]>> {
        return this.http.post<DatatableResponseWrapper<StationPaymentListModel[]>>(this.srvUrl + "GetChargeDeviceListForPanel", searchCriter);
    }
    getChargeDeviceById(id: number): Observable<ChargeDeviceUpdateModel> {
        return this.http.get<ChargeDeviceUpdateModel>(this.srvUrl + "GetChargeDeviceById", { params: { id: id.toString() } });
    }
    update(entity: ChargeDeviceUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }
    changeState(request: ChargeDeviceChangeStateRequestModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeState", request);
    }
    add(entity: ChargeDeviceInsertModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }
    updatePrice(request: ChargeDeviceUpdatePriceRequestModel[]): Observable<any> {
        return this.http.post<any>(this.srvUrl + "UpdateChargeDevicePrice", request);
    }
    prepareInsertForm(): Observable<ChargeDeviceInsertFormPrepareModel> {
        return this.http.post<ChargeDeviceInsertFormPrepareModel>(this.srvUrl + "ChargeDevicePrepareInsertForm", null);
    }
}
```

### chargeDevice-remote-service.ts (OCPP Remote Komutlar)

```typescript
@Injectable()
export class ChargeDeviceRemoteService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ChargeDeviceRemote/";
    }

    startTransaction(request: ChargeDeviceRemoteStartTransactionRequestModel): Observable<ChargeDeviceRemoteStartTransactionResponseModel> {
        return this.http.post<ChargeDeviceRemoteStartTransactionResponseModel>(this.srvUrl + "ChargeDeviceRemoteStartTransaction", request);
    }
    stopTransaction(request: ChargeDeviceRemoteStopTransactionRequestModel): Observable<ChargeDeviceRemoteStopTransactionResponseModel> {
        return this.http.post<ChargeDeviceRemoteStopTransactionResponseModel>(this.srvUrl + "ChargeDeviceRemoteStopTransaction", request);
    }
    changeAvailability(request: ChargeDeviceRemoteChangeAvailabilityRequestModel): Observable<ChargeDeviceRemoteChangeAvailabilityResponseModel> {
        return this.http.post<ChargeDeviceRemoteChangeAvailabilityResponseModel>(this.srvUrl + "ChargeDeviceRemoteChangeAvailability", request);
    }
    resetDevice(request: ChargeDeviceRemoteResetRequestModel): Observable<ChargeDeviceRemoteResetResponseModel> {
        return this.http.post<ChargeDeviceRemoteResetResponseModel>(this.srvUrl + "ChargeDeviceRemoteResetDevice", request);
    }
    prepareChangeConfigurationDeviceForm(request: PrepareChangeConfigurationDeviceFormRequestModel): Observable<PrepareChangeConfigurationDeviceFormResponseModel> {
        return this.http.post<PrepareChangeConfigurationDeviceFormResponseModel>(this.srvUrl + "PrepareChangeConfigurationDeviceForm", request);
    }
    changeConfigurationDevice(request: ChargeDeviceRemoteChangeConfigurationRequestModel): Observable<ChargeDeviceRemoteChangeConfigurationResponseModel> {
        return this.http.post<ChargeDeviceRemoteChangeConfigurationResponseModel>(this.srvUrl + "ChargeDeviceRemoteChangeConfiguration", request);
    }
    chargeDeviceRemoteTriggerMessage(request: ChargeDeviceRemoteTriggerMessageRequestModel): Observable<ChargeDeviceRemoteTriggerMessageResponseModel> {
        return this.http.post<ChargeDeviceRemoteTriggerMessageResponseModel>(this.srvUrl + "ChargeDeviceRemoteTriggerMessage", request);
    }
}
```

### chargeDevice-status-notification-service.ts (SignalR)

```typescript
import * as signalR from "@microsoft/signalr";
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable()
export class ChargeDeviceStatusNotificationService {

    public notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
    public notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
    public notificationViewTime$ = new BehaviorSubject<number>(3000);
    private hubConnection: signalR.HubConnection;

    public getConnectionId(): string {
        return this.hubConnection.connectionId;
    }

    public startConnection = () => {
        this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
        this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${environment.notificationApiUrl}chargeDeviceConnectionStateNotification`)
            .build();
        this.hubConnection.start()
            .then(() => { console.log("hub connection startd") })
            .catch(err => { console.log(err) });
    }

    public addListener = () => {
        this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
        this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
        this.hubConnection.on("chargeDeviceConnectionStateNotification", data => {
            this.notification$.next(data);
            this.notifications$.next([data, ...this.notifications$.value]);
            this.notificationViewTime$.next(3);
            this.clearNotification(data);
        });
    }

    public clearNotification = (data: ChargeDeviceNotificationStatusModel) => {
        setTimeout(() => {
            const notifyArray: any[] = this.notifications$.getValue();
            notifyArray.forEach((item, index) => {
                if (item === data) { notifyArray.splice(index, 1); }
            });
            this.notifications$.next(notifyArray);
        }, this.notifications$.getValue().length * 2000 > 8000 ? 8000 : this.notifications$.getValue().length * 2000);
    }

    constructor() { }

    ngOnDestroy() {
        this.hubConnection.stop();
        this.hubConnection = null;
        this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
        this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
        this.notifications$.unsubscribe();
        this.notification$.unsubscribe();
    }
}
```

### chargeManagment-service.ts

```typescript
@Injectable()
export class ChargeManagmentService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ChargeManagment/";
    }

    list(searchCriter: DatatableRequestWrapper<ChargeDatatableFilterModel>): Observable<DatatableResponseWrapper<ChargeDatatableItemModel[]>> {
        return this.http.post<DatatableResponseWrapper<ChargeDatatableItemModel[]>>(this.srvUrl + "GetChargeProcessDatatable", searchCriter);
    }
    checkChargePayment(searchCriter: CheckChargePaymentRequestModel): Observable<CheckChargePaymentResponseModel> {
        return this.http.post<CheckChargePaymentResponseModel>(this.srvUrl + "CheckChargePaymentFromPanel", searchCriter);
    }
    getChargeDetail(searchCriter: ChargeDetailRequestModel): Observable<ChargeDetailResponseModel> {
        return this.http.post<ChargeDetailResponseModel>(this.srvUrl + "GetChargeDetail", searchCriter);
    }
    stopChargeProcess(searchCriter: StopChargeProcessRequestModel): Observable<StopChargeProcessResponseModel> {
        return this.http.post<StopChargeProcessResponseModel>(this.srvUrl + "StopChargeProcess", searchCriter);
    }
}
```

### stationManagment-service.ts

```typescript
@Injectable()
export class StationManagmentService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "StationManagment/";
    }

    getStationManagmentDataTablePanel(searchCriter: DatatableRequestWrapper<StationManagmentFilterModel>): Observable<DatatableResponseWrapper<StationManagmentListModel[]>> {
        return this.http.post<DatatableResponseWrapper<StationManagmentListModel[]>>(this.srvUrl + "GetStationManagmentDataTablePanel", searchCriter);
    }
    getStationsForSelectList(searchCriter: StationManagmentFilterModel): Observable<StationManagmentSelectListModel[]> {
        return this.http.post<StationManagmentSelectListModel[]>(this.srvUrl + "GetStationsForSelectList", searchCriter);
    }
    getStationManagmentById(id: number): Observable<StationManagmentUpdateFormModel> {
        return this.http.get<StationManagmentUpdateFormModel>(this.srvUrl + "GetStationManagmentById", { params: { id: id.toString() } });
    }
    update(entity: StationManagmentUpdateRequestModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }
    changeIsActive(setOnAirModel: StationManagmentIsActiveUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeActiveState", setOnAirModel);
    }
    prepareInsertForm(): Observable<StationInsertFormPrepare> {
        return this.http.post<StationInsertFormPrepare>(this.srvUrl + "StationPrepareInsertForm", null);
    }
    add(entity: StationManagmentInsertModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }
    removeStation(entity: StationRemoveRequestModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "RemoveStation", entity);
    }
    addFile(fileDropZone: FileDropZoneData): Observable<StationManagmentFileUploadResponseModel> {
        var formData = new FormData();
        var mediaFileGroup: MediaFileGroupModel[] = [];
        mediaFileGroup.push({ group: PictureType["preview"], compressRate: 0 });
        mediaFileGroup.push({ group: PictureType["detailed"], compressRate: 70 });
        formData.append('fileGroup', JSON.stringify(mediaFileGroup));
        formData.append("file", fileDropZone.file, fileDropZone.file.name);
        let headers = new HttpHeaders();
        headers.append('Content-Type', 'multipart/form-data');
        headers.append('Accept', 'application/json');
        return this.http.post<any>(this.srvUrl + "AddStationPicture", formData, { headers: headers });
    }
}
```

### dashboard-service.ts

```typescript
@Injectable()
export class DashboardService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "Dashboard/";
    }

    getStations(searchCriter: StationManagmentDashboardFilterModel): Observable<StationManagmentDashboardItemModel[]> {
        return this.http.post<StationManagmentDashboardItemModel[]>(this.srvUrl + "GetStations", searchCriter);
    }
    getStationDetail(searchCriter: StationManagmentDashboardFilterModel): Observable<StationManagmentDashboardItemModel> {
        return this.http.post<StationManagmentDashboardItemModel>(this.srvUrl + "GetStationDetail", searchCriter);
    }
    getMonthlyChargeProcess(searchCriter: ChargePrcessDashboardFilterModel): Observable<MonthlyChargeProcessDashboardModel> {
        return this.http.post<MonthlyChargeProcessDashboardModel>(this.srvUrl + "GetMonthlyChargeProcess", searchCriter);
    }
    getMonthlyReservation(searchCriter: ReservationDashboardFilterModel): Observable<MonthlyReservationDashboardModel[]> {
        return this.http.post<MonthlyReservationDashboardModel[]>(this.srvUrl + "GetMonthlyReservation", searchCriter);
    }
    getSumOfProcess(searchCriter: SumOfProcessRequestModel): Observable<SumOfProcessModel> {
        return this.http.post<SumOfProcessModel>(this.srvUrl + "GetSumOfProcess", searchCriter);
    }
}
```

---

## Domain Enum'ları (Önemli Olanlar)

### authentication/panel-admin-user-type-enum.ts

```typescript
export enum PanelAdminUserType {
    ADMIN_USER = "ADMIN_USER",
    COMPANY_ADMIN_USER = "COMPANY_ADMIN_USER",
    ROOT_ADMIN_USER = "ROOT_ADMIN_USER",
    TEST_ADMIN_USER = "TEST_ADMIN_USER"
}

export enum AdminManagmentType {
    MAIN_ADMIN = "MAIN_ADMIN",
    COMPANY_ADMIN = "COMPANY_ADMIN",
    TEST_ADMIN_USER = "TEST_ADMIN_USER"
}

export const adminManagmentTypeMapping: Record<keyof typeof AdminManagmentType, string> = {
    MAIN_ADMIN: "Genel Admin",
    COMPANY_ADMIN: "Firma Admin",
    TEST_ADMIN_USER: "Test Admin"
};
```

### chargeDevice/chargeDevice-type-enum.ts

```typescript
export enum ChargeDeviceState {
    ACTIVE = "ACTIVE",
    IN_CARE = "IN_CARE",
    DEFECTIVE = "DEFECTIVE"
}
export const chargeDeviceStateMapping: Record<keyof typeof ChargeDeviceState, string> = {
    ACTIVE: "Aktif",
    IN_CARE: "Bakımda",
    DEFECTIVE: "Arızalı"
};

export enum ChargeDevicePowerType {
    AC = 1,
    DC = 2
}
export const chargeDevicePowerTypeMapping: Record<keyof typeof ChargeDevicePowerType, string> = {
    AC: "Ac",
    DC: "Dc"
};

export enum OcppMark {
    ABB = 1,
    PIWIN = 2
}
export const ocppMarkMapping: Record<keyof typeof OcppMark, string> = {
    ABB: "Abb",
    PIWIN: "Piwin"
};

export enum OcppMessageType {
    SENDER = "SENDER",
    RECEIVED = "RECEIVED"
}
export const ocppMessageTypeMapping: Record<keyof typeof OcppMessageType, string> = {
    SENDER: "Gönderilen",
    RECEIVED: "Alınan"
};
```

### chargeDevice/charge-device-ocpp-state-enum.ts

```typescript
export enum ChargeDeviceOcppState {
    AVAILABLE = "AVAILABLE",
    UNAVAILABLE = "UNAVAILABLE",
    FAULTED = "FAULTED"
}
export const chargeDeviceOcppStateMapping: Record<keyof typeof ChargeDeviceOcppState, string> = {
    AVAILABLE: "Uygun",
    UNAVAILABLE: "Uygun Değil",
    FAULTED: "Arızalı"
};
```

### authentication/authentication-model.ts

```typescript
import { PanelAdminUserType } from "../../enums/authentication/panel-admin-user-type-enum";
import { AccessTokenModel } from "./access-token-model";

export class AuthenticationModel {
    token: AccessTokenModel;
    name: string;
    surname: string;
    phone: string;
    mail: string;
    companyName: string;
    connectionId: string;
    companyId: number;
    panelAdminUserType: PanelAdminUserType;
}
```

---

## Environment

```typescript
export const environment = {
    production: false,
    appVersion: 'v717demo1',
    isTest: false,
    apiUrl: 'http://localhost:9180/v1/',
    webSiteApiUrl: 'http://localhost:35573/v1/',
    tockenUrl: 'http://localhost:14493/v1/',
    fileApiUrl: 'http://localhost:56301/v1/',
    testApiUrl: 'http://localhost:54123/v1/',
    imageUrl: 'http://localhost:25500/v1/FileView/GetFile?code=',
    logApiUrl: 'http://localhost:9622/v1/',
    notificationApiUrl: 'http://localhost:52440/',
    stationApiUrl: 'http://localhost:14258/v1/',
    ocppApiUrl: 'http://localhost:59439/v1/',
    stationNotificationApiUrl: 'http://localhost:14258/'
};
```

---

## Dikkat Edilmesi Gereken Noktalar

### 1. Routing Mimarisi
- `HashLocationStrategy` kullanılıyor — URL'ler `#` ile başlar. Standart `PathLocationStrategy`'den farklı olarak sunucu tarafı konfigürasyonu gerektirmez ama modern uygulamalar için tercih edilmez.
- Ana routing 3 katmanlı lazy loading: `AppRoutingModule` -> `LayoutModule` (shared_admin) -> `PagesRoutingModule` (evtech) -> feature modülleri.

### 2. Authentication Akışı
- Login öncesi `prepareLoginForm()` çağrısı ile sunucudan `loginFormKey` alınır. Bu CSRF benzeri bir güvenlik mekanizmasıdır — form key olmadan login isteği geçersizdir.
- JWT token, `AuthenticationModel` nesnesi olarak JSON serialize edilerek `auth_token` key'i ile localStorage'da saklanır.
- `APP_INITIALIZER` ile uygulama başlarken `getAdminByToken()` çağrılır ve localStorage'daki token ile oturum restore edilir.
- Token yenileme (`getNewTokenWithRefreshToken`) metodundaki `authLocalStorageToken != "auth_token"` kontrolü hiçbir zaman true olmaz — token sabit olarak "auth_token" key'i kullanır. Bu kod asla çalışmaz; potansiyel bir bug.

### 3. HTTP Interceptor
- `HttpEventInterceptor` tüm isteklere `Authorization: Bearer <token>` header'ı ekler.
- 401 hata durumunda token silinir ve `/auth/login`'e yönlendirilir.
- `srvProgressSpinner.hide()` yanıt geldiğinde çağrılır, ancak `show()` interceptor tarafından değil her component tarafından manuel çağrılmak zorundadır — tutarsız bir tasarım.

### 4. SignalR Entegrasyonu
- `ChargeDeviceStatusNotificationService`, `chargeDeviceConnectionStateNotification` hub event'ini dinler.
- Bildirimler BehaviorSubject ile reaktif olarak yayınlanır ve timeout sonrası otomatik temizlenir.
- Bildirim görüntülenme süresi dinamik hesaplanır: `min(count * 2000ms, 8000ms)`.

### 5. Datatable Altyapısı
- `DataTableBase<TFilter, DataModel>` generic sınıf: MatSort, MatPaginator entegrasyonunu standartlaştırır.
- `merge(sortTable.sortChange, paginatorTable.page)` pattern'i ile sıralama ve sayfalama değişikliğinde otomatik veri çekimi yapılır.
- `getFilterData()` metodu sayfalama ve sıralama bilgilerini filtre modeline ekler.

### 6. OCPP Remote Management
- Tam OCPP komut seti: StartTransaction, StopTransaction, ChangeAvailability, ResetDevice, ChangeConfiguration, TriggerMessage.
- Her komut ayrı request/response model çifti ile tiplendirilmiştir.
- `ChargeDeviceRemoteManagmentService` ile `ChargeDeviceCommandManagmentService` ayrı sorumluluk alanlarına sahiptir.

### 7. Çok Şirketli Yapı
- `AuthenticationModel.companyId` field'ı ile kullanıcının şirket bağlamı belirlenir.
- `PanelAdminUserType.ROOT_ADMIN_USER` tüm şirketleri görebilir; `COMPANY_ADMIN_USER` sadece kendi şirketini.
- `TEST_ADMIN_USER` rolü test ortamı için ayrılmıştır.

### 8. Dosya Yükleme
- `StationManagmentService.addFile()` multipart/form-data ile dosya yükler.
- Her fotoğraf iki boyutta işlenir: `preview` (sıkıştırmasız) ve `detailed` (%70 sıkıştırma oranıyla).
- `imageUrl` environment değişkeni ile dosya sunucusundan resim URL'i oluşturulur.

### 9. Para Birimi
- `ng2-currency-mask` + `CustomCurrencyMaskConfig` ile para girişleri standardize edilir.
- `CurrencyMaskModule` sadece ihtiyaç duyan modüllere (stations, chargeDevice) inject edilir.

### 10. Yerel Ayarlar
- `LOCALE_ID: 'tr-TR'`, `MAT_DATE_LOCALE: 'tr-TR'`, `MomentDateAdapter` kullanılıyor.
- `registerLocaleData(localeTr, 'tr')` shared.module'de çağrılır.
- Uygulama Türkçe kullanıcı arayüzüne sahip; ngx-translate altyapısı mevcut ama aktif çok dil desteği kullanılmamaktadır.
