# sarj_pro_panel_angular Analiz

## Genel Bakış

SarjAllProPanel, Angular 13 ile geliştirilmiş bir yönetim paneli uygulamasıdır. Metronic v7 Bootstrap teması üzerine inşa edilmiştir. Uygulama, EV şarj istasyonları veya benzer bir IoT/enerji altyapısını yöneten bir SaaS platformunun admin paneli işlevi görmektedir. JWT tabanlı kimlik doğrulama, rol bazlı yetkilendirme, çok dilli içerik yönetimi, parametre yönetimi, sözleşme (policy) yönetimi ve log izleme gibi temel modülleri barındırır.

**Kaynak dizin:** `E:\Projeler\Angular\SarjAllProPanel\`

---

## Teknoloji Stack

| Katman | Teknoloji | Versiyon |
|---|---|---|
| Framework | Angular | ~13.1.2 |
| Dil | TypeScript | ~4.5.4 |
| UI Bileşen | Angular Material | ^11.0.0 |
| CSS Framework | Bootstrap | ^4.6.1 |
| Tema | Metronic v7 | 7.1.7 |
| HTTP | HttpClient + JWT Interceptor | @auth0/angular-jwt ^5.0.2 |
| State | BehaviorSubject (RxJS) | ~7.4.0 |
| Tarih | Moment.js + Material Moment Adapter | ^2.27.0 |
| Sifreleme (HATALI) | ts-md5 (CLIENT-SIDE MD5) | ^1.2.11 |
| Grafik | ng-apexcharts | ^1.5.1 |
| SignalR | @microsoft/signalr | ^6.0.1 |
| Dosya Yukleme | ngx-dropzone | ^3.0.0 |
| Ceviri | @ngx-translate/core | ^13.0.0 |
| Loader | ngx-ui-loader | ^11.0.0 |
| Hash Routing | HashLocationStrategy | Angular built-in |

---

## Klasor Yapisi

```
src/
  app/
    app.module.ts                  # Root module
    app-routing.module.ts          # Root routing (hash strategy)
    app.component.ts/html          # Bootstrap component
    core/                          # Teknik altyapi (pipe, directive, base siniflar)
      adapters/
        MOMENT_DATE_FORMATS.ts
      bases/
        base-datatable/            # DataTableBase<TFilter, DataModel> generic sinifi
        base-tree/
        base-tree-table/
      configs/
        custom-currency-mask.config.ts
      date-core/
        mat-date.module.ts
        MOMENT_DATE_FORMATS.ts
      directives/                  # only-number, only-text, date-time, cell-template vs.
      external-components/         # pixdinn-dropzone, pixdinn-html-editor
        dropzone-shared.module.ts
        html-editor-shared.module.ts
      pipes/                       # first-letter, safe, safe-html, remove-whitespaces, uppercase-turkish
      services/
        core-utils.service.ts
      wrapper-core/                # Generic request/response wrapper modelleri
        datatable-request-core-model.ts
        datatable-result-core.model.ts
        server-result-core-model.ts
      core.module.ts
    shared_admin/                  # Admin-spesifik paylasilan katman
      auth/
        auth.guard.ts
        auth.module.ts
        authentication-service.ts
      general-material.module.ts   # Tum Angular Material modulleri tek noktada
      layout.module.ts
      shared.module.ts
      partials/
        aside/                     # Sol menu (HTML bos - menu items yok!)
        dialogs/                   # action-notification, evet-hayir, genel-sil, yardim, progress-spinner
        footer/
        header/
        header-mobile/
        layout/                    # LayoutComponent, LayoutService, LayoutInitService
        layout-other-partials/     # scroll-top, user-partial
        subheader/                 # breadcrumb servisi
        topbar/
      template/
        error-pages/               # 404, 401
        splash-screen/
      utils/
        enums/                     # environment-mode, menu-icon-path, message-type, yes-no
        interseptors/
          http-event-interseptor.ts  # HTTP interceptor (token enjeksiyonu + 401 yonetimi)
        pipes/                     # active-passive, active-passive-css, month, true-false-css, yes-no
        services/
          utils.service.ts         # Snackbar, dialog, hata yardimcilari
        wrapper-models/            # DatatableRequestWrapper, DatatableResponseWrapper, ServerResultModel
    sarjAllPro/                    # Uygulama feature modulleri
      components/
        auth/                      # Login
        authority-management/      # Yetki yonetimi
        language/                  # Dil yonetimi
        log/                       # Exception + request-response loglari
        panelAdmin/                # Admin CRUD (add/update/list + tip yonetimi)
        paramaters/                # Parametre yonetimi
        policy/                    # Sozlesme yonetimi (HTML editor)
        system-parameter/          # Sistem parametreleri
      enums/
        authentication/panel-admin-user-type-enum.ts
        mediaFile/
        parameter/parameter-type.ts
      models/                      # Tum DTO/request/response modelleri
        apiexception/
        authentication/
        authority/
        contentLanguage/
        countryCityAndTown/
        mediaFile/
        panelAdmin/
        panelAdminType/
        parameter/
        parameterGroup/
        parameterValue/
        policy/
        requestResponse/
      pipes/
        image-src.pipe.ts
      services/                    # Her feature icin ayri @Injectable() servis
        adminUserTypeAuth/
        authority/
        contentLanguage/
        countryCityAndTown/
        log/
        panelAdmin/
        panelAdminType/
        parameter/
        parameterGroup/
        parameterValue/
        policy/
      pages-routing.module.ts
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
import { SplashScreenModule } from './shared_admin/template/splash-screen/splash-screen.module';
import { ProgressSpinnerModule } from './shared_admin/partials/dialogs/progress-spinner/progress-spinner.module';
import { CoreModule } from './core/core.module';
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

**Dikkat:**
- `APP_INITIALIZER` ile uygulama baslamadan once token dogrulamasi yapiliyor.
- `HashLocationStrategy` kullanildigi icin URL'ler `/#/auth/login` formatinda.
- `angular-in-memory-web-api` import edilmis ama kullanilmiyor (olu bagimlilik).
- `NgApexchartsModule` ve `NgxImageZoomModule` import listesinde gorulmuyorlar ancak package.json'da mevcut.

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
      import('./sarjAllPro/components/auth/auth-module').then((m) => m.AuthModule),
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
      import('./shared_admin/template/error-pages/error-page.module').then(
        (m) => m.ErrorPageModule
      )
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
```

**Onemli:** Root routing lazy-loading kullanir. `AuthGuard` yalnizca root `''` rotasina uygulanmis. Feature modulleri `LayoutModule` araciligiyyla `pages-routing.module.ts` uzerinden yukleniyor.

---

## Core Katmani

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

### wrapper-core/datatable-request-core-model.ts

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

### wrapper-core/datatable-result-core.model.ts

```typescript
export class DatatableResponseWrapperCore<T> {
    data: T;
    recordCount: number;
}
```

### wrapper-core/server-result-core-model.ts

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

### bases/base-datatable/base-datatable.ts

Tum liste componentleri icin temel sinif. MatSort + MatPaginator kurulumunu soyutlar ve tekrar eden paginate/sort kodunu ortadan kaldirir.

```typescript
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
        this.sortTable.sortChange.subscribe(() => {
            this.paginatorTable.pageIndex = 0;
        });
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

### core/pipes/safe-html.pipe.ts

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

**Not:** `bypassSecurityTrustHtml` XSS riskine kapı açar. Kullanim oncesi kaynak güvenliği dogrulanmalidir.

### core/directives/only-number.directive.ts

```typescript
@Directive({ selector: 'input[numbersOnly]' })
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
        ) { return; }
        if (e.key === ' ' || isNaN(Number(e.key))) {
            e.preventDefault();
        }
    }
}
```

---

## Shared Admin Katmani

### shared_admin/auth/auth.guard.ts

```typescript
import { Injectable } from '@angular/core';
import {
    Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot
} from '@angular/router';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const currentUser = this.authService.adminValue;
        // BUG: getAuthFromLocalStorage() bir Observable<AuthenticationModel> doner.
        // Observable nesnesi her zaman truthy'dir; of(undefined) bile truthy'dir.
        // Dolayisiyla bu if, sadece currentUser uzerinden degerlendirilmektedir.
        if (currentUser && this.authService.getAuthFromLocalStorage()) {
            return true;
        }
        this.authService.logout();
        return false;
    }
}
```

### shared_admin/auth/authentication-service.ts

```typescript
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, finalize, map, Observable, of } from "rxjs";
import { AuthenticationModel } from "src/app/sarjAllPro/models/authentication/authentication-model";
import { environment } from "src/environments/environment";

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class AuthenticationService {

    public adminSubject: BehaviorSubject<AuthenticationModel> =
        new BehaviorSubject<AuthenticationModel>(null);
    isLoadingSubject: BehaviorSubject<boolean>;
    private authLocalStorageToken = "auth_token";  // Sabit string literal
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

    // BUG: authLocalStorageToken bir sabit string "auth_token"'dir.
    // (authLocalStorageToken != "auth_token") kosulu DAIMA false olur.
    // Bu, userLogout() API cagrisinin hicbir zaman yapilmadigini anlamina gelir.
    // Sunucuda oturum kapatilamaz, refresh token iptal edilemez.
    logout() {
        if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
            this.userLogout().subscribe(auth => { });
        }
        localStorage.removeItem(this.authLocalStorageToken);
        localStorage.removeItem(this.authUserInfo);
        this.adminSubject = new BehaviorSubject<AuthenticationModel>(undefined);
        this.router.navigate(['/auth/login']);
    }

    // getAdminByToken iki kez getAuthFromLocalStorage() cagiriyor
    getAdminByToken(): Observable<AuthenticationModel> {
        const auth = this.getAuthFromLocalStorage(); // 1. cagri - result kullanilmiyor
        if (!auth) { // Observable her zaman truthy, bu kontrol is yapmaz
            return of(undefined);
        }
        this.isLoadingSubject.next(true);
        return this.getAuthFromLocalStorage().pipe( // 2. cagri - gercekten kullanilan
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

    clearTokenFromLocalStorage() {
        localStorage.removeItem(this.authLocalStorageToken);
    }

    prepareLoginForm(loginRequest: PrepareLoginFormRequestModel) {
        return this.http.post<PrepareLoginFormResponseModel>(
            this.srvUrl + "LoginForm", loginRequest, httpOptions
        );
    }

    logIn(loginRequest: LoginRequestModel) {
        return this.http.post<LoginResponseModel>(
            this.srvUrl + "Login", loginRequest, httpOptions
        );
    }

    userLogout() {
        return this.http.post<any>(this.srvUrl + "LogOut", null, httpOptions);
    }

    adminLoginRefreshToken(refreshToken: AuthenticationModel): Observable<AuthenticationModel> {
        return this.http.post<any>(this.srvUrl + "RefreshTokenLogin", refreshToken, httpOptions);
    }
}
```

### shared_admin/utils/interseptors/http-event-interseptor.ts

```typescript
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor,
         HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ProgressSpinnerService } from '../../partials/dialogs/progress-spinner/progress-spinner.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/sarjAllPro/models/authentication/authentication-model';

@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {

    private authLocalStorageToken = "auth_token";

    constructor(
        private authService: AuthenticationService,
        public srvProgressSpinner: ProgressSpinnerService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authenticationModel: AuthenticationModel = JSON.parse(
            localStorage.getItem(this.authLocalStorageToken)
        );
        // BUG: Her HTTP isteğinde konsola auth model yaziliyor (production'da olmamali)
        console.log(authenticationModel);

        if (authenticationModel) {
            request = this.addTokenHeader(request, authenticationModel);
        }

        return next.handle(request).pipe(
            tap((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                    // BUG: Her basarili HTTP yaniti loglanıyor
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
        // BUG: adminSubject bir BehaviorSubject nesnesidir, hicbir zaman undefined olamaz.
        // Bu if kosulu her zaman true degerlendirilir; else blogu asla calismiyor.
        if (this.authService.adminSubject !== undefined) {
            try {
                const headers = new HttpHeaders()
                    .set('Authorization', 'Bearer ' + authenticationModel.token.token);
                return request = request.clone({ headers });
            } catch {
                this.authService.logout();
            }
        } else {
            // Bu blok hicbir zaman calismiyor
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

### shared_admin/utils/services/utils.service.ts

```typescript
import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EnumMessageType } from '../enums/message-type.enum';
import { ServerResultModel } from '../wrapper-models/server.result.model';

// BUG: providedIn: 'root' eksik. Her feature modulu kendi scope'unda
// ayri bir UtilsService instance'i olusturur. Singleton davranisi yok.
@Injectable()
export class UtilsService {
    constructor(
        private snackBar: MatSnackBar,
        private dialog: MatDialog
    ) { }

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
                            type == EnumMessageType.Warning ? 'yellow-snackbar' :
                                'mat-snack-bar-container'
            ],
            data: {
                message,
                snackBar: this.snackBar,
                showCloseButton,
                showUndoButton,
                undoButtonDuration,
                verticalPosition,
                type,
                action: 'Undo'
            },
            verticalPosition: verticalPosition
        });
    }

    generalDeleteDialog(title: string = '', description: string = '',
                        waitDesciption: string = '', service: any, id: number) {
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

    // BUG: Global 'event' nesnesi kullaniliyor (deprecated ve strict modda hata verir)
    helperDialog(description: string = '') {
        event.stopPropagation();
        return this.dialog.open(YardimDialogComponent, {
            data: { description },
            width: '440px'
        });
    }

    getServerErrorRequest(data: any): ServerResultModel {
        let dataError = data as HttpErrorResponse;
        return dataError.error as ServerResultModel;
    }

    convertStringListToString(mesajList: []): string {
        let mesaj: string = "<ul>";
        mesajList.forEach((element: string) => {
            mesaj += "<li>" + element + "</li>";
        });
        return mesaj + "</ul>";
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

### shared_admin/auth/auth.module.ts

```typescript
import { NgModule } from "@angular/core";
import { AuthenticationService } from "src/app/shared_admin/auth/authentication-service";
import { AuthGuard } from "./auth.guard";

@NgModule({
    imports: [],
    declarations: [],
    exports: [],
    providers: [
        AuthenticationService,
        AuthGuard
    ]
})
export class AuthModule { }
```

### shared_admin/general-material.module.ts

Tum Angular Material modullerini tek bir yerde birlestirir. Feature modulleri `SharedModule` uzerinden bu module erisir.

Export edilen moduller: `MatInputModule`, `MatFormFieldModule`, `MatDatepickerModule`, `MatAutocompleteModule`, `MatListModule`, `MatSliderModule`, `MatCardModule`, `MatSelectModule`, `MatButtonModule`, `MatIconModule`, `MatSlideToggleModule`, `MatCheckboxModule`, `MatMenuModule`, `MatTabsModule`, `MatTooltipModule`, `MatSidenavModule`, `MatProgressBarModule`, `MatProgressSpinnerModule`, `MatSnackBarModule`, `MatTableModule`, `MatGridListModule`, `MatToolbarModule`, `MatBottomSheetModule`, `MatExpansionModule`, `MatDividerModule`, `MatSortModule`, `MatStepperModule`, `MatChipsModule`, `MatPaginatorModule`, `MatDialogModule`, `MatRippleModule`, `MatRadioModule`, `MatTreeModule`, `MatButtonToggleModule`, `FormsModule`, `ReactiveFormsModule`, `MatFileUploadModule`, `NgxDropzoneModule`, `NgbModule`.

### shared_admin/partials/layout-other-partials/user-partial/user-partial.component.ts

```typescript
@Component({
    selector: 'app-user-partial',
    templateUrl: './user-partial.component.html',
})
export class UserPartialComponent implements OnInit {
    admin: AuthenticationModel;
    subscriptions: Subscription[] = [];

    constructor(
        private layout: LayoutService,
        private srvAuthService: AuthenticationService
    ) { }

    ngOnDestroy() {
        this.subscriptions.forEach(sb => sb.unsubscribe());
    }

    ngOnInit(): void {
        const sb = this.srvAuthService.adminSubject.asObservable().pipe(
            first(admin => !!admin)
        ).subscribe(admin => {
            this.admin = Object.assign({}, admin);
        });
        this.subscriptions.push(sb);
        // BUG: Subscribe asenkron; bu satir calistiginda this.admin henuz atanmamis olabilir
        console.log(this.admin);
    }

    logout() {
        this.srvAuthService.logout();
        // Gereksiz: logout() zaten router.navigate(['/auth/login']) yapiyor
        document.location.reload();
    }
}
```

---

## Feature Modulleri

### sarjAllPro/pages-routing.module.ts

```typescript
const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {
                path: 'log',
                loadChildren: () =>
                    import('./components/log/log-module').then((m) => m.LogModule),
            },
            {
                path: 'paramaters',
                loadChildren: () =>
                    import('./components/paramaters/paramaters-module').then((m) => m.ParamatersModule),
            },
            {
                path: 'authority',
                loadChildren: () =>
                    import('./components/authority-management/authority-module').then((m) => m.AuthorityModule),
            },
            {
                path: 'language',
                loadChildren: () =>
                    import('./components/language/language.module').then((m) => m.LanguageModule),
            },
            {
                path: 'panelAdmin',
                loadChildren: () =>
                    import('./components/panelAdmin/panelAdmin-module').then((m) => m.PanelAdminModule),
            },
            {
                path: 'policy',
                loadChildren: () =>
                    import('./components/policy/policy-module').then((m) => m.PolicyModule),
            },
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

**Dikkat:** `system-parameter` modulu routing'e eklenmemis; klasor var ama erisim yolu yok.

### Auth Module - Login Component

**login.component.ts**

```typescript
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
    prepareLoginFormRequestModel: PrepareLoginFormRequestModel = new PrepareLoginFormRequestModel();
    loginRequestModel: LoginRequestModel = new LoginRequestModel();
    private unsubscribe: Subscription[] = [];
    loginForm: FormGroup;

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

    login() {
        this.srvProgressSpinner.show();
        this.srvAuthService.isLoadingSubject.next(true);
        const controls = this.loginForm.controls;
        if (this.loginForm.invalid) {
            Object.keys(controls).forEach(controlName =>
                controls[controlName].markAsTouched()
            );
            this.srvProgressSpinner.hide();
            return;
        }
        // Sifre bu noktada plain-text olarak gonderiliyor (MD5 yok)
        const loginSubscr = this.srvAuthService.logIn(this.loginRequestModel)
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
                    this.srvAuthService.adminSubject =
                        new BehaviorSubject<AuthenticationModel>(authenticationModel);
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

    isControlHasError(controlName: string, validationType: string): boolean {
        const control = this.loginForm.controls[controlName];
        if (!control) { return false; }
        const result = control.hasError(validationType) && (control.dirty || control.touched);
        return result;
    }
}
```

**login.component.html (kisaltilmis)**

```html
<form class="form" [formGroup]="loginForm" novalidate="novalidate" id="kt_login_signin_form">
  <div class="form-group">
    <!-- BUG: formControlName iki kez yazilmis: once 'UserName' (gecersiz), sonra 'userName' (gecerli) -->
    <input [(ngModel)]="loginRequestModel.userName"
           formControlName="UserName"
           formControlName="userName"
           autocomplete="off"
           placeholder="Kullanici Adi" />
    <mat-error *ngIf="isControlHasError('userName','required')">
      <strong>*Kullanici adi alani bos birakilamaz.</strong>
    </mat-error>
  </div>
  <div class="form-group">
    <input [(ngModel)]="loginRequestModel.password"
           type="password"
           formControlName="password"
           autocomplete="off"
           placeholder="Sifre" />
    <mat-error *ngIf="isControlHasError('password','required')">
      <strong>*Sifre alani bos birakilamaz.</strong>
    </mat-error>
  </div>
  <button type="submit" (click)="login()">Giris</button>
</form>
```

### PanelAdmin Modulu

**panelAdmin-module.ts**

```typescript
@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        DropzoneSharedModule,
        RouterModule.forChild([
            { path: 'typeList', component: PanelAdminTypeListComponent },
            { path: 'typeUpdate', component: UpdatePanelAdminTypeComponent },
            { path: 'typeAdd', component: AddPanelAdminTypeComponent },
            { path: 'list', component: PanelAdminListComponent },
            { path: 'add', component: AddPanelAdminComponent },
            { path: 'update', component: UpdatePanelAdminComponent },
        ])
    ],
    providers: [
        UtilsService,          // BUG: Singleton degil, modul-scoped instance
        PanelAdminTypeService,
        PanelAdminService
    ],
    declarations: [
        PanelAdminTypeListComponent,
        UpdatePanelAdminTypeComponent,
        AddPanelAdminTypeComponent,
        PanelAdminListComponent,
        AddPanelAdminComponent,
        UpdatePanelAdminComponent
    ]
})
export class PanelAdminModule { }
```

**panelAdmin-add.component.ts — MD5 Guvenlik Acigi**

```typescript
import { Md5 } from 'ts-md5';

@Component({ selector: 'app-panelAdmin-add', ... })
export class AddPanelAdminComponent implements OnInit {
    md5 = new Md5();
    password: string;
    passwordAgain: string;

    save() {
        this.srvProgressSpinner.show();
        this.panelAdminInsertModel.isActive = true;

        // CRITICAL SECURITY BUG: Sifre client-side MD5 ile hashleniyor.
        // MD5, sifrelemek icin kullanilamaz (kriptografik olarak kirilmis).
        // Rainbow table ve collision saldirilarina karsi savunmasiz.
        // Sifre hashleme isleminin SUNUCU TARAFINDA bcrypt/argon2 ile yapilmasi gerekir.
        this.panelAdminInsertModel.password = this.md5.appendStr(this.password).end() + "";
        this.panelAdminInsertModel.passwordAgain = this.md5.appendStr(this.passwordAgain).end() + "";

        this.srvPanelAdmin.add(this.panelAdminInsertModel).subscribe((response: any) => {
            this.srvUtils.showActionNotification("Basariyla Eklendi", EnumMessageType.Success, 8000);
            setTimeout(() => {
                this.router.navigate(['panelAdmin/list']);
            }, 2500);
        }, (error) => {
            this.srvProgressSpinner.hide();
        });
    }

    checkPassword() {
        if (this.password != this.passwordAgain) {
            this.srvUtils.showActionNotification("Sifreler Uyusmamaktadir", EnumMessageType.Error, 3000);
            return false;
        }
        return true;
    }
}
```

**panelAdmin-list.component.ts**

```typescript
@Component({ selector: 'app-panelAdmin-list', ... })
export class PanelAdminListComponent implements OnInit {
    showingColumnNamesPanelAdminTable: string[] = [
        'Id', 'Name', 'Surname', 'TypeName',
        'AdminManagmentType', 'CompanyName', 'IsActive', 'Edit'
    ];
    panelAdminList: MatTableDataSource<PanelAdminListModel> = new MatTableDataSource();
    totalRecordSizePanelAdminTable: number;
    @ViewChild(MatPaginator) paginatorPanelAdminTable: MatPaginator;
    @ViewChild(MatSort) sortPanelAdminTable: MatSort;
    searchCriterPanelAdmin: DatatableRequestWrapper<PanelAdminFilterModel> =
        new DatatableRequestWrapper();
    panelAdminIsActiveUpdateModel: PanelAdminIsActiveUpdateModel =
        new PanelAdminIsActiveUpdateModel();

    getPanelAdmins(hasFilter: boolean) {
        this.panelAdminList.data = [];
        this.srvProgressSpinner.show();
        if (hasFilter) { this.paginatorPanelAdminTable.pageIndex = 0; }
        if (this.sortPanelAdminTable && this.sortPanelAdminTable.direction != "" &&
            this.sortPanelAdminTable.active != "") {
            this.searchCriterPanelAdmin.orderDirective = this.sortPanelAdminTable.direction;
            this.searchCriterPanelAdmin.orderProperty = this.sortPanelAdminTable.active;
        } else {
            this.searchCriterPanelAdmin.orderDirective = "desc";
            this.searchCriterPanelAdmin.orderProperty = "Id";
        }
        if (this.paginatorPanelAdminTable) {
            this.searchCriterPanelAdmin.recordPerPage = this.paginatorPanelAdminTable.pageSize;
            this.searchCriterPanelAdmin.pageNumber = this.paginatorPanelAdminTable.pageIndex;
        } else {
            this.searchCriterPanelAdmin.recordPerPage = 15;
        }
        this.srvPanelAdminService.list(this.searchCriterPanelAdmin)
            .subscribe((response: DatatableResponseWrapper<PanelAdminListModel[]>) => {
                this.panelAdminList.data = response.data;
                this.totalRecordSizePanelAdminTable = response.recordCount;
            }, (error) => {
                this.srvProgressSpinner.hide();
                this.onErrorPanelAdminList(error);
            });
    }

    ngAfterViewInit(): void {
        this.sortPanelAdminTable.sortChange.subscribe(() => {
            this.paginatorPanelAdminTable.pageIndex = 0;
        });
        merge(this.sortPanelAdminTable.sortChange, this.paginatorPanelAdminTable.page)
            .pipe(tap(() => { this.getPanelAdmins(false); }))
            .subscribe(() => {}, () => null);
    }
}
```

### Authority List Component (authority-list.component.ts)

Promise zinciri kullanilan yetki yonetimi.

```typescript
@Component({ selector: 'app-authority-list', ... })
export class AuthorityListComponent implements OnInit, AfterViewInit {

    authorityList: any[] = [];
    adminTypeList: PanelAdminTypeModel[] = [];

    // Promise zinciri - Angular Observable idiomuna uygun degil
    initAuthFormContent() {
        this.srvProgressSpinner.show();
        this.getAdminUserType().then(res => {
            this.getAuthorityList().then(res => {
                this.getAdminUserTypeAuthList().then(res => {
                    this.srvProgressSpinner.hide();
                });
            }).catch(err => {
                this.srvProgressSpinner.hide();
            });
        }).catch(err => {
            this.srvProgressSpinner.hide();
        });
    }

    save() {
        this.srvProgressSpinner.show();
        this.adminUserTypeAuthorityUpdate = [];
        this.authorityList.map((authGroup) => {
            authGroup.auth.map((auth) => {
                auth.adminUserTypeAuthority.adminUserTypeId = this.adminUserTypeId;
                this.adminUserTypeAuthorityUpdate.push(auth.adminUserTypeAuthority);
            });
        });
        this.srvAdminUserTypeAuth.add(this.adminUserTypeAuthorityUpdate).subscribe(
            (response: any) => {
                this.srvUtils.showActionNotification("Basariyla Guncellendi",
                    EnumMessageType.Success, 4000);
            }, (error) => {
                this.adminUserTypeAuthorityUpdate = [];
                this.srvProgressSpinner.hide();
            }
        );
    }
}
```

### Policy Management Component (policy-managment.component.ts)

HTML editor ile Base64 encoded icerik yonetimi.

```typescript
@Component({ selector: 'app-policy-managment', ... })
export class PolicyManagmentComponent implements OnInit, AfterViewInit {

    @ViewChild('policyHtmlEditor') policyHtmlEditor?: PixdinnHtmlEditorComponent;

    getPolicyDetail() {
        return new Promise<void>((resolve, reject) => {
            this.srvProgressSpinner.show();
            this.policyDetailRequestModel.policyName = this.selectedPolicyName;
            this.srvPolicyService.getPolicyDetail(this.policyDetailRequestModel)
                .subscribe((response: PolicyDetailModel) => {
                    this.policyDetailModel = response;
                    // Base64 decode: sunucudan gelen HTML icerik decode ediliyor
                    // escape()/unescape() deprecated API kullanimi
                    this.policyHtmlEditor.editor.setContent(
                        decodeURIComponent(escape(window.atob(this.policyDetailModel.policyContent)))
                    );
                    this.srvProgressSpinner.hide();
                    resolve();
                }, (error) => {
                    this.srvProgressSpinner.hide();
                    reject();
                });
        });
    }

    update() {
        this.policyUpdateRequestModel.policyName = this.policyDetailModel.policyName;
        // Base64 encode: HTML icerik sunucuya base64 olarak gonderiliyor
        this.policyUpdateRequestModel.policyContent =
            btoa(unescape(encodeURIComponent(this.policyHtmlEditor.editordoc)));
        this.srvProgressSpinner.show();
        this.srvPolicyService.update(this.policyUpdateRequestModel)
            .subscribe(async (response: UpdatePolicyResponseModel) => {
                this.srvUtils.showActionNotification("Basariyla Guncellendi",
                    EnumMessageType.Success, 2000);
                this.srvProgressSpinner.hide();
            }, (error) => {
                this.srvProgressSpinner.hide();
            });
    }
}
```

---

## Servisler

### PanelAdminService

```typescript
@Injectable()
export class PanelAdminService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "PanelAdmin/";
    }

    list(searchCriter: DatatableRequestWrapper<PanelAdminFilterModel>):
        Observable<DatatableResponseWrapper<PanelAdminListModel[]>> {
        return this.http.post<DatatableResponseWrapper<PanelAdminListModel[]>>(
            this.srvUrl + "list", searchCriter
        );
    }

    getPanelAdminTypeById(id: number): Observable<PanelAdminModel> {
        return this.http.get<PanelAdminModel>(this.srvUrl + "GetPanelAdminById", {
            params: { id: id.toString() }
        });
    }

    changeIsActive(setOnAirModel: PanelAdminIsActiveUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeActiveState", setOnAirModel);
    }

    update(entity: PanelAdminModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }

    add(entity: PanelAdminModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }
}
```

### AuthorityService

```typescript
@Injectable()
export class AuthorityService {
    private apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + "AuthorizeManagment/";
    }

    listAuthGroupWithAuth(): Observable<AuthorityGroupModel[]> {
        return this.http.post<AuthorityGroupModel[]>(
            this.apiUrl + "GetAuthGroupWithAuth", ""
        );
    }
}
```

### PolicyService

```typescript
@Injectable()
export class PolicyService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "PolicyManagment/";
    }

    update(request: UpdatePolicyRequestModel): Observable<UpdatePolicyResponseModel> {
        return this.http.post<UpdatePolicyResponseModel>(
            this.srvUrl + "UpdatePolicy", request
        );
    }

    getPolicyNames(request: PolicyNamesRequestModel): Observable<PolicyNamesModel> {
        return this.http.post<PolicyNamesModel>(this.srvUrl + "GetPolicyNames", request);
    }

    getPolicyDetail(request: PolicyDetailRequestModel): Observable<PolicyDetailModel> {
        return this.http.post<PolicyDetailModel>(this.srvUrl + "GetPolicyDetail", request);
    }
}
```

### ContentLanguageService

```typescript
@Injectable()
export class ContentLanguageService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ContentLanguage/";
    }

    list(): Observable<ContentLanguageModel[]> {
        return this.http.post<ContentLanguageModel[]>(this.srvUrl + "list", "");
    }

    listForPanel(searchCriter: DatatableRequestWrapper<ContentLanguageFilterModel>):
        Observable<DatatableResponseWrapper<ContentLanguageModel[]>> {
        return this.http.post<DatatableResponseWrapper<ContentLanguageModel[]>>(
            this.srvUrl + "ListForPanel", searchCriter
        );
    }

    setDefault(setDefaultModel: ContentLanguageSetDefaultModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "SetDefault", setDefaultModel);
    }
}
```

### ParameterService

```typescript
@Injectable()
export class ParameterService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "Parameter/";
    }

    list(searchCriter: ParameterFilterModel): Observable<ParameterListModel[]> {
        return this.http.post<ParameterListModel[]>(
            this.srvUrl + "GetParameters", searchCriter
        );
    }

    // Not: Metot ismi "add" ama aslinda guncelleme yapiyor (UpdateParameterValue endpoint)
    add(entity: ParameterListModel[]): Observable<any> {
        return this.http.post<any>(this.srvUrl + "UpdateParameterValue", entity);
    }
}
```

---

## Modeller

### authentication/authentication-model.ts

```typescript
export class AuthenticationModel {
    // BUG: Model sınıfı icinde Observable pipe() metodu tanimlanmis.
    // Bu, sinifin Observable gibi kullanilmasindan kaynakli yanlis bir tasarim.
    pipe(arg0: OperatorFunction<AuthenticationModel, AuthenticationModel>,
         arg1: MonoTypeOperatorFunction<unknown>): Observable<AuthenticationModel> {
        throw new Error("Method not implemented.");
    }
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

### authentication/access-token-model.ts

```typescript
export class AccessTokenModel {
    token: string;
    expiration: Date;
    refreshToken: string;
    refreshTokenExpiration: Date;
}
```

### enums/authentication/panel-admin-user-type-enum.ts

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

---

## Tespit Edilen Sorunlar

### Kritik / Guvenlik

| # | Sorun | Konum | Onem |
|---|---|---|---|
| 1 | MD5 ile client-side sifre hashleme | `panelAdmin-add.component.ts:81-82` | Kritik |
| 2 | `logout()` icindeki API cagrisi asla calismaz | `authentication-service.ts:58` | Yuksek |
| 3 | AuthGuard'da Observable truthy kontrolu hatali | `auth.guard.ts:11` | Yuksek |
| 4 | Her HTTP isteğinde `console.log` | `http-event-interseptor.ts:26,34` | Orta |
| 5 | UtilsService singleton degil (her modulde ayri instance) | Tum feature modul providers | Orta |

### Kod Kalitesi

| # | Sorun | Konum |
|---|---|---|
| 6 | `AuthenticationModel` icinde `pipe()` metodu tanimlanmis | `authentication-model.ts:5` |
| 7 | `escape()`/`unescape()` deprecated API kullanimi | `policy-managment.component.ts:81,127` |
| 8 | Promise zinciri yerine Observable zinciri kullanilmali | `authority-list`, `parameter-list` componentleri |
| 9 | Login HTML'de `formControlName` iki kez tanimlanmis | `login.component.html:53` |
| 10 | `getAdminByToken()` icinde `getAuthFromLocalStorage()` iki kez cagriliyor | `authentication-service.ts:66-72` |
| 11 | `console.log(this.admin)` subscribe'dan once cagriliyor | `user-partial.component.ts:33` |
| 12 | `angular-in-memory-web-api` import edilmis, kullanilmiyor | `app.module.ts:7` |
| 13 | `LanguageService` TypeScript'ten import edilmis (Angular servisi degil) | `language-list.component.ts:21` |
| 14 | Aside menu icerigi tamamen bos | `aside.component.html:33-35` |
| 15 | `system-parameter` modulu routing'e eklenmemis | `pages-routing.module.ts` |
| 16 | `ParameterService.add()` aslinda guncelleme yapiyor | `parameter-service.ts:21-23` |
| 17 | `getNewTokenWithRefreshToken()` asla calismayan if blogu | `authentication-service.ts:95-104` |
