# sarj_ev_panel_angular — Kapsamlı Analiz

## 1. Platform & Tech Stack

| Katman | Teknoloji | Detay |
|---|---|---|
| Framework | Angular | 13+ |
| Dil | TypeScript | ES2020 target, ES2015 output |
| UI Template | Metronic 8 (KT) | KTLayoutHeader, KTLayoutContent, KTUtil |
| UI Kütüphane | Angular Material | MatTable, MatPaginator, MatSort, MatDialog |
| Harita | @agm/core | Google Maps Angular entegrasyonu |
| Real-time | SignalR | @microsoft/signalr — destek bildirimleri |
| Kimlik Doğrulama | JWT | @auth0/angular-jwt, BehaviorSubject akışı |
| HTTP | HttpClient + Interceptor | Bearer token enjeksiyonu, 401 yönetimi |
| Form | ReactiveFormsModule | FormBuilder, FormGroup, Validators |
| Tarih | moment.js + MomentDateAdapter | Türkçe locale, DD/MM/YYYY formatı |
| Grafik | ApexCharts + Chart.js | ng-apexcharts, chart.js/auto |
| Dosya Yükleme | ngx-dropzone | PixdinnDropzone wrapper bileşeni |
| Para Formatı | ng2-currency-mask | TL formatı (1.234,56) |
| i18n | @ngx-translate/core | Çok dil desteği |
| Routing | Angular Router | Lazy loading, HashLocationStrategy |
| Durum | BehaviorSubject (RxJS) | Signal yok — klasik RxJS akışı |
| Protokol | OCPP 1.6 | Şarj cihazı iletişim protokolü |
| Build | Angular CLI | tsconfig.json, angular.json |

---

## 2. Proje Klasör Yapısı

```
EvTechPanelAltunkaya/
├── src/
│   ├── environments/
│   │   ├── environment.ts              (geliştirme — localhost)
│   │   ├── environment.local.ts        (yerel alternatif)
│   │   ├── environment.test.ts         (test sunucu — 213.159.5.123)
│   │   └── environment.prod.ts         (production — localhost https)
│   └── app/
│       ├── app.module.ts               (kök modül — APP_INITIALIZER, JwtModule)
│       ├── app-routing.module.ts       (3 ana rota: auth | layout | error)
│       ├── app.component.ts
│       ├── _fake/                      (sahte veri servisleri)
│       ├── core/                       (teknik altyapı)
│       │   ├── adapters/               (MomentDateAdapter)
│       │   ├── bases/
│       │   │   ├── base-datatable/     (DataTableBase<TFilter,DataModel>)
│       │   │   ├── base-tree-table/    (TreeDataTableControlBase)
│       │   │   └── base-tree/          (TreeControlBase)
│       │   ├── configs/                (CurrencyMask config)
│       │   ├── date-core/              (MatDateModule)
│       │   ├── directives/             (7 directive)
│       │   ├── external-components/    (PixdinnDropzone, HtmlEditor)
│       │   ├── pipes/                  (6 pipe)
│       │   ├── services/               (CoreUtilsService)
│       │   ├── wrapper-core/           (Request/Response wrapper modeller)
│       │   └── core.module.ts
│       ├── shared_admin/               (yönetim paneli altyapısı)
│       │   ├── auth/
│       │   │   ├── auth.guard.ts
│       │   │   ├── auth.module.ts
│       │   │   └── authentication-service.ts
│       │   ├── partials/
│       │   │   ├── aside/              (sidebar — menü)
│       │   │   ├── dialogs/            (action-notification, evet-hayir, genel-sil, yardim)
│       │   │   ├── footer/
│       │   │   ├── header/             (header + header-menu)
│       │   │   ├── header-mobile/
│       │   │   ├── layout/             (layout.component + layout.service — SignalR)
│       │   │   ├── layout-other-partials/  (scroll-top, support-notification, user)
│       │   │   ├── subheader/          (breadcrumb sistemi)
│       │   │   └── topbar/
│       │   ├── template/
│       │   │   ├── error-pages/        (404, 401)
│       │   │   └── splash-screen/
│       │   ├── utils/
│       │   │   ├── enums/              (message-type, menu-icon-path, yes-no)
│       │   │   ├── interseptors/       (http-event-interseptor.ts)
│       │   │   ├── pipes/
│       │   │   │   ├── domain-pipes/   (30+ alan pipe'ı — charge-state, connector-state vb.)
│       │   │   │   └── general-pipes/  (active-passive, month, yes-no vb.)
│       │   │   ├── services/           (UtilsService — snackbar, dialog)
│       │   │   └── wrapper-models/     (DatatableRequestWrapper, ServerResultModel)
│       │   ├── general-material.module.ts  (tüm Angular Material export'ları)
│       │   ├── layout.module.ts
│       │   └── shared.module.ts        (tüm pipe + dialog declarations + interceptor)
│       └── evtech/                     (iş mantığı — feature module)
│           ├── components/             (22 lazy-loaded alt modül)
│           │   ├── auth/login/
│           │   ├── announcement/
│           │   ├── auth-management/
│           │   ├── authority-management/
│           │   ├── campaign/
│           │   ├── chargeDevice/       (CRUD + OCPP uzak yönetim)
│           │   ├── chargeDeviceReservation/
│           │   ├── chargeManagment/    (şarj işlemleri listesi + detay)
│           │   ├── company/
│           │   ├── home/               (dashboard + Google Maps)
│           │   ├── language/
│           │   ├── log/                (exception + request-response loglar)
│           │   ├── messagesender/
│           │   ├── panelAdmin/
│           │   ├── paramaters/
│           │   ├── payments/
│           │   ├── policy/
│           │   ├── reporting/
│           │   ├── stations/
│           │   ├── support/
│           │   ├── technical-support/
│           │   ├── users/
│           │   ├── version-management/
│           │   └── wallet/
│           ├── enums/                  (40+ enum, her biri Türkçe mapping ile)
│           ├── models/                 (200+ model dosyası — request/response/list/detail)
│           ├── services/               (35 servis dosyası)
│           └── pages-routing.module.ts (22 lazy route)
├── tsconfig.json
└── angular.json
```

---

## 3. app.module.ts — Tam Kod + Açıklama

```typescript
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtModule } from '@auth0/angular-jwt';
import { ClipboardModule } from 'ngx-clipboard';
import { TranslateModule } from '@ngx-translate/core';
import { InlineSVGModule } from 'ng-inline-svg';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
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

// JWT token getter — localStorage'dan ham token string döndürür
export function tokenGetter() {
  return localStorage.getItem('auth_token');
}

// APP_INITIALIZER fabrikası — uygulama bootstrap olmadan önce çağrılır.
// Token varsa BehaviorSubject doldurulur, yoksa logout yönlendirmesi yapılır.
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
    MatDateModule,              // Moment tarih adaptörü (Türkçe DD/MM/YYYY)
    TranslateModule.forRoot(),
    HttpClientModule,
    HighlightModule,
    ClipboardModule,
    NgxUiLoaderModule,
    ProgressSpinnerModule,
    InlineSVGModule.forRoot(),
    NgbModule,
    CoreModule,                 // Pipe'lar, directive'ler, MomentDateAdapter
    AuthModule,                 // AuthGuard, AuthenticationService
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        // whitelist/blacklist boş — interceptor manuel Bearer enjekte eder
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
    // Hash routing (#) — sunucu tarafı konfigürasyon gerektirmez
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

**Kritik Kararlar:**
- `HashLocationStrategy` — URL'ler `/#/path` biçimindedir. Sunucu her isteği index.html'e yönlendirmek zorunda kalmaz.
- `APP_INITIALIZER` — Bootstrap öncesi token kontrolü. Sayfa yenilenmesinde oturum kaybolmaz.
- `JwtModule` kayıtlı ama interceptor token'ı manuel enjekte eder — çakışma yok çünkü JwtModule whitelist boş.

---

## 4. app-routing.module.ts — Tam Kod + Açıklama

```typescript
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './shared_admin/auth/auth.guard';

export const routes: Routes = [
  {
    // Kimlik doğrulama — AuthGuard yok, login sayfası buradan yüklenir
    path: 'auth',
    loadChildren: () =>
      import('./evtech/components/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    // Ana uygulama — AuthGuard koruması altında
    // LayoutModule → PagesRoutingModule → 22 feature modülü
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./shared_admin/layout.module').then((m) => m.LayoutModule),
  },
  {
    // Eşleşmeyen tüm rotalar hata sayfasına yönlenir
    path: '**',
    loadChildren: () =>
      import('./shared_admin/template/error-pages/error-page.module')
        .then((m) => m.ErrorPageModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
```

**Rota Zinciri:**
```
AppRoutingModule
 ├── /auth  → AuthModule (login sayfası)
 ├── /      → LayoutModule (AuthGuard)
 │            └── PagesRoutingModule
 │                ├── /              → HomeModule (dashboard)
 │                ├── /stations      → StationsModule
 │                ├── /chargeDevices → ChargeDeviceModule
 │                ├── /chargeManagment → ChargeManagmentModule
 │                ├── /payments      → PaymentsModule
 │                ├── /support       → SupportModule
 │                └── ... (22 modül)
 └── /**    → ErrorPageModule
```

---

## 5. Core Modülü — Her Dosya Detaylı

### 5.1. adapters/ — MomentDateAdapter

`core/adapters/MOMENT_DATE_FORMATS.ts` — Angular Material için Türkçe tarih adaptörü:

```typescript
import { Inject, Injectable, Optional } from '@angular/core';
import { DateAdapter, MAT_DATE_LOCALE, MatDateFormats } from '@angular/material/core';
import * as _moment from 'moment';
import { default as _rollupMoment, Moment } from 'moment';
import 'moment/locale/tr';

const moment = _rollupMoment || _moment;

// Angular Material datepicker için format tanımları
export const MOMENT_DATE_FORMATS: MatDateFormats = {
  parse:   { dateInput: 'DD/MM/YYYY' },
  display: {
    dateInput:       'DD/MM/YYYY',
    monthYearLabel:  'MMMM Y',
    dateA11yLabel:   'LL',
    monthYearA11yLabel: 'MMMM Y'
  }
};

function range<T>(length: number, valueFunction: (index: number) => T): T[] {
  const valuesArray = Array(length);
  for (let i = 0; i < length; i++) valuesArray[i] = valueFunction(i);
  return valuesArray;
}

@Injectable()
export class MomentDateAdapter extends DateAdapter<Moment> {
  private _localeData: {
    firstDayOfWeek: number; longMonths: string[]; shortMonths: string[];
    dates: string[]; longDaysOfWeek: string[]; shortDaysOfWeek: string[];
    narrowDaysOfWeek: string[];
  };

  constructor(@Optional() @Inject(MAT_DATE_LOCALE) dateLocale: string) {
    super();
    this.setLocale(dateLocale || moment.locale());
  }

  setLocale(locale: string) {
    super.setLocale(locale);
    let momentLocaleData = moment.localeData(locale);
    this._localeData = {
      firstDayOfWeek: momentLocaleData.firstDayOfWeek(),
      longMonths:     momentLocaleData.months(),
      shortMonths:    momentLocaleData.monthsShort(),
      dates: range(31, (i) => this.createDate(2017, 0, i + 1).format('D')),
      longDaysOfWeek:   momentLocaleData.weekdays(),
      shortDaysOfWeek:  momentLocaleData.weekdaysShort(),
      narrowDaysOfWeek: momentLocaleData.weekdaysMin(),
    };
  }

  getYear(date: Moment): number   { return this.clone(date).year(); }
  getMonth(date: Moment): number  { return this.clone(date).month(); }
  getDate(date: Moment): number   { return this.clone(date).date(); }
  getDayOfWeek(date: Moment): number { return this.clone(date).day(); }

  getMonthNames(style: 'long' | 'short' | 'narrow'): string[] {
    return style == 'long' ? this._localeData.longMonths : this._localeData.shortMonths;
  }
  getDateNames(): string[] { return this._localeData.dates; }
  getDayOfWeekNames(style: 'long' | 'short' | 'narrow'): string[] {
    if (style == 'long')  return this._localeData.longDaysOfWeek;
    if (style == 'short') return this._localeData.shortDaysOfWeek;
    return this._localeData.narrowDaysOfWeek;
  }
  getYearName(date: Moment): string    { return this.clone(date).format('YYYY'); }
  getFirstDayOfWeek(): number          { return this._localeData.firstDayOfWeek; }
  getNumDaysInMonth(date: Moment): number { return this.clone(date).daysInMonth(); }
  clone(date: Moment): Moment          { return date.clone().locale(this.locale); }

  createDate(year: number, month: number, date: number): Moment {
    if (month < 0 || month > 11) throw Error(`Invalid month index "${month}".`);
    if (date < 1) throw Error(`Invalid date "${date}".`);
    let result = moment({ year, month, date }).locale(this.locale);
    if (!result.isValid()) throw Error(`Invalid date "${date}" for month "${month}".`);
    return result;
  }

  today(): Moment { return moment().locale(this.locale); }

  parse(value: any, parseFormat: string | string[]): Moment | null {
    if (value && typeof value == 'string')
      return moment(value, parseFormat, this.locale);
    return value ? moment(value).locale(this.locale) : null;
  }

  format(date: Moment, displayFormat: string): string {
    date = this.clone(date);
    if (!this.isValid(date)) throw Error('MomentDateAdapter: Cannot format invalid date.');
    return date.format(displayFormat);
  }

  addCalendarYears(date: Moment, years: number): Moment  { return this.clone(date).add({ years }); }
  addCalendarMonths(date: Moment, months: number): Moment { return this.clone(date).add({ months }); }
  addCalendarDays(date: Moment, days: number): Moment    { return this.clone(date).add({ days }); }
  toIso8601(date: Moment): string { return this.clone(date).format(); }

  deserialize(value: any): Moment | null {
    let date;
    if (value instanceof Date)    date = moment(value);
    if (typeof value === 'string') {
      if (!value) return null;
      date = moment(value, moment.ISO_8601).locale(this.locale);
    }
    if (date && this.isValid(date)) return date;
    return super.deserialize(value);
  }

  isDateInstance(obj: any): boolean { return moment.isMoment(obj); }
  isValid(date: Moment): boolean    { return this.clone(date).isValid(); }
  invalid(): Moment                 { return moment.invalid(); }
}
```

### 5.2. base-datatable/ — Generic DataTableBase Sınıfı

`core/bases/base-datatable/base-datatable-base-model.ts`:
```typescript
export class BaseDataTableBaseModel {
  rowUniqueId: number;
  isSelected: boolean;
  isDetailOpened: boolean;
}
```

`core/bases/base-datatable/base-datatable.ts` — Tüm listeleme componentlerinin miras alacağı temel sınıf:
```typescript
import { BaseDataTableBaseModel } from 'src/app/core/bases/base-datatable/base-datatable-base-model';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { merge } from 'rxjs';
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

  constructor(showingColumnNames: string[], obj: any, callback: any) {
    this.showingColumnNames = showingColumnNames;
    this.obj = obj;
    this.receivedDataFromServer = callback;
  }

  // ngAfterViewInit'te çağrılır — sort + paginator event stream'leri bağlanır
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

  // API cevabını tabloya yazar
  public setDataSource(response: DatatableResponseWrapperCore<DataModel[]>): void {
    this.totalRecordSize = response.recordCount;
    this.dataList.data = response.data;
  }

  // Filter + sort + paginator bilgilerini tek istek nesnesinde toplar
  public getFilterData(filterData: TFilter): DatatableRequestWrapperCore<TFilter> {
    this.searchCriter.data = filterData;
    if (this.sortTable) {
      this.searchCriter.orderDirective = this.sortTable.direction;
      this.searchCriter.orderProperty  = this.sortTable.active;
    }
    if (this.paginatorTable) {
      this.searchCriter.recordPerPage = this.paginatorTable.pageSize;
      this.searchCriter.pageNumber    = this.paginatorTable.pageIndex;
    } else {
      this.searchCriter.recordPerPage = 10;
      this.searchCriter.pageNumber    = 0;
    }
    return this.searchCriter;
  }
}
```

**Wrapper Modeller:**
```typescript
// wrapper-core/datatable-request-core-model.ts
export class DatatableRequestWrapperCore<T> {
  constructor() { this.data = {} as T; }
  data: T;
  orderDirective: string;  // 'asc' | 'desc'
  orderProperty:  string;  // sütun adı
  pageNumber:     number;
  recordPerPage:  number;
  offset:         number;
}

// wrapper-core/datatable-result-core.model.ts
export class DatatableResponseWrapperCore<T> {
  data:        T;
  recordCount: number;
}

// wrapper-core/server-result-core-model.ts
export class ServerResultCoreModel {
  resultType:    ServerResultType;
  errorCode:     number;
  errorMessage:  string;
}
enum ServerResultType {
  Ok              = "Ok",
  Error           = "Error",
  ValidationError = "ValidationError",
  Unauthorized    = "Unauthorized",
  Exception       = "Exception"
}
```

### 5.3. directives/ — Her Directive Tam Kod

```typescript
// cell-template.directive.ts — dinamik hücre şablonu bağlama
import { Directive, Input, TemplateRef } from '@angular/core';
@Directive({ selector: 'ng-template[cellTemplate]' })
export class CellTemplateDirective {
  @Input('cellTemplate') name: string;
  constructor(public template: TemplateRef<any>) { }
}

// only-number.directive.ts — yalnızca rakam girişine izin verir
@Directive({ selector: 'input[numbersOnly]' })
export class NumberDirective {
  private navigationKeys = [
    'Backspace','Delete','Tab','Escape','Enter',
    'Home','End','ArrowLeft','ArrowRight','Clear','Copy','Paste',
  ];
  constructor(private _el: ElementRef) { }
  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (
      this.navigationKeys.indexOf(e.key) > -1 ||
      (e.key === 'a' && e.ctrlKey) || (e.key === 'c' && e.ctrlKey) ||
      (e.key === 'v' && e.ctrlKey) || (e.key === 'x' && e.ctrlKey) ||
      (e.key === 'a' && e.metaKey) || (e.key === 'c' && e.metaKey) ||
      (e.key === 'v' && e.metaKey) || (e.key === 'x' && e.metaKey) ||
      (e.key === ',')
    ) { return; }
    if (e.key === ' ' || isNaN(Number(e.key))) e.preventDefault();
  }
}

// only-text.directive.ts — yalnızca harf girişine izin verir (rakam bloklar)
@Directive({ selector: 'input[textOnly]' })
export class TextDirective {
  // aynı navigationKeys listesi ...
  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    // navigation keys geçer
    // FARK: !isNaN ise engelle (rakam ve boşluk bloklanır)
    if (e.key === ' ' || !isNaN(Number(e.key))) e.preventDefault();
  }
}

// only-text-or-number.directive.ts — Türkçe dahil harf+rakam, özel karakter bloklar
@Directive({ selector: 'input[textOrNumbersOnly]' })
export class TextOrNumberDirective {
  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    // navigation keys geçer
    // Türkçe karakterler: ç,ğ,ı,ö,ş,ü,İ,Ç,Ğ,Ö,Ş,Ü dahil hardcode whitelist
    // Diğer: e.preventDefault() ile bloklanır
  }
}

// date-time.directive.ts — datepicker değerini İstanbul timezone'una çevirir
@Directive({ selector: '[dateTime]' })
export class DateTimeDirective {
  @Input() sendType: string;  // 'get' | diğer

  @HostListener('dateChange', ['$event'])
  onDateChange = (gelen: any) => {
    this.convertToTurkishTimezone(gelen.target.value);
  };

  constructor(private control: NgControl) {}

  convertToTurkishTimezone(date: moment.Moment): any {
    if (date == null) return;
    if (this.sendType == 'get') {
      // UTC'ye çevir
      var momentDateGet = moment(date).utcOffset(0, false).format();
      this.control.control.setValue(momentDateGet);
    } else {
      // İstanbul timezone offset ekle
      var momentDate = moment(date);
      var targetDate = momentDate.clone()
        .tz("Europe/Istanbul").utc()
        .add(momentDate.utcOffset(), 'm').toDate();
      this.control.control.setValue(targetDate);
    }
  }
}

// label-control.directive.ts — FormGroup kontrolünün değerini <label>'da gösterir
@Directive({
  selector: 'label[formControlName]',
  providers: [{ provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => LabelControlDirective), multi: true }]
})
export class LabelControlDirective {
  @Input() formControlName: string;
  constructor(@Optional() private parent: ControlContainer) { }
  @HostBinding('textContent')
  get controlValue() {
    return this.parent ? this.parent.control.get(this.formControlName).value : '';
  }
  writeValue() { }
  registerOnChange() { }
  registerOnTouched() { }
}

// two-digit-decimal-number.directive.ts — max 2 ondalık basamak
@Directive({ selector: '[appTwoDigitDecimaNumber]' })
export class TwoDigitDecimalNumberDirective {
  private regex: RegExp = new RegExp(/^\d*\.?\d{0,2}$/g);
  private specialKeys = ['Backspace','Tab','End','Home','-','ArrowLeft','ArrowRight','Del','Delete'];
  constructor(private el: ElementRef) {}
  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    if (this.specialKeys.indexOf(event.key) !== -1) return;
    let current: string = this.el.nativeElement.value;
    const position = this.el.nativeElement.selectionStart;
    const next: string = [
      current.slice(0, position),
      event.key == 'Decimal' ? '.' : event.key,
      current.slice(position)
    ].join('');
    if (next && !String(next).match(this.regex)) event.preventDefault();
  }
}
```

### 5.4. pipes/ — Her Pipe Tam Kod

```typescript
// first-letter.pipe.ts — "Ad Soyad" → "AS" (avatar için)
@Pipe({ name: 'firstLetter' })
export class FirstLetterPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return value.split(' ').map((n) => n[0]).join('');
  }
}

// safe-html.pipe.ts — HTML string'i güvenli render eder
@Pipe({ name: 'safeHtml' })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitized: DomSanitizer) { }
  transform(value: any) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
}

// safe.pipe.ts — tip bazlı güvenli değer (html/style/script/url/resourceUrl)
@Pipe({ name: 'safe' })
export class SafePipe implements PipeTransform {
  constructor(protected _sanitizer: DomSanitizer) {}
  transform(value: string, type: string): SafeHtml | SafeStyle | SafeScript | SafeUrl | SafeResourceUrl {
    switch (type) {
      case 'html':        return this._sanitizer.bypassSecurityTrustHtml(value);
      case 'style':       return this._sanitizer.bypassSecurityTrustStyle(value);
      case 'script':      return this._sanitizer.bypassSecurityTrustScript(value);
      case 'url':         return this._sanitizer.bypassSecurityTrustUrl(value);
      case 'resourceUrl': return this._sanitizer.bypassSecurityTrustResourceUrl(value);
      default:            return this._sanitizer.bypassSecurityTrustHtml(value);
    }
  }
}

// uppercase-turkish.pipe.ts — toLocaleUpperCase() Türkçe büyük harf
@Pipe({ name: 'uppercaseTurkish' })
export class UppercaseturkishPipe implements PipeTransform {
  transform(value: string) {
    if (value === null || value === undefined || value == '') return value;
    return value.toLocaleUpperCase();
  }
}

// remove-whitespaces.pipe.ts — boşlukları siler (IBAN, telefon gösterimi)
@Pipe({ name: 'removewhitespaces' })
export class RemovewhitespacesPipe implements PipeTransform {
  transform(value: string, args?: any): string {
    if (value === null || value === undefined) return '';
    return value.replace(/ /g, '');
  }
}
```

### 5.5. services/ — CoreUtilsService

```typescript
// core/services/core-utils.service.ts
@Injectable()
export class CoreUtilsService {
  constructor() { }

  // File → Base64 DataURL (dropzone preview + sunucuya gönderim için)
  public async readFile(file: File): Promise<string | ArrayBuffer> {
    return new Promise<string | ArrayBuffer>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = e => resolve((e.target as FileReader).result);
      reader.onerror = e => {
        console.error(`FileReader failed on file ${file.name}.`);
        return reject(null);
      };
      if (!file) { console.error('No file to read.'); return reject(null); }
      reader.readAsDataURL(file);
    });
  }

  // Türkçe ay listesi — select box bileşenlerine beslenir
  getMonths(): {} {
    return [
      {name:"Ocak",value:1},  {name:"Şubat",value:2},   {name:"Mart",value:3},
      {name:"Nisan",value:4}, {name:"Mayıs",value:5},    {name:"Haziran",value:6},
      {name:"Temmuz",value:7},{name:"Ağustos",value:8},  {name:"Eylül",value:9},
      {name:"Ekim",value:10}, {name:"Kasım",value:11},   {name:"Aralık",value:12}
    ];
  }
}
```

### 5.6. external-components/ — PixdinnDropzoneComponent

```typescript
// core/external-components/pixdinn-dropzone/pixdinn-dropzone.component.ts
@Component({
  selector: 'PixdinnDropzone',
  templateUrl: './pixdinn-dropzone.component.html',
  styleUrls: ['./pixdinn-dropzone.component.scss'],
  providers: [{ provide: NgxDropzoneComponent, useExisting: PixdinnDropzoneComponent }]
})
export class PixdinnDropzoneComponent implements OnInit {
  fileData: FileDropZoneData[] = [];
  fileDataDeleted: FileDropZoneData[] = [];
  @Input() multiple: boolean;
  @Input() isSingleFileSelectable: boolean; // true = tek dosya modu
  @Input() accept: string;
  @Input() title: string;
  index: number = 1;
  onFileSelected: any = (fileData: FileDropZoneData) => { };
  @ViewChild('buttonForBug') bugButton: ElementRef<HTMLElement>;

  constructor(private coreUtilService: CoreUtilsService) { }

  // Güncelleme ekranında mevcut dosyaları yüklemek için çağrılır
  initFileData(fileData: FileDropZoneData[]) {
    this.fileData = fileData;
    this.fileData.forEach(item => {
      item.oldfile = true;
      item.index = this.index++;
    });
    this.bugButtonClick();
  }

  onSelect(event) {
    if (this.isSingleFileSelectable) {
      // Tek dosya modunda: varolan dosyaları "silindi" listesine ekle
      this.fileData.forEach(item => {
        if (item.fileId != null && item.fileId > 0)
          this.fileDataDeleted.push(item);
      });
      this.fileData = [];
    }
    event.addedFiles.forEach(element => {
      let fileDrop = new FileDropZoneData(DropZoneTempImage.Loading, "", this.getfileType(element));
      fileDrop.setFile(element);
      fileDrop.onUploaded = false;
      fileDrop.fileRelativeName = fileDrop.file.name;
      fileDrop.index = this.index++;
      fileDrop.oldfile = false;
      this.fileData.push(fileDrop);
      if (this.onFileSelected != null) this.onFileSelected(fileDrop);
    });
  }

  onFileUploaded(url, index, guid) {
    this.fileData.forEach(item => {
      if (index == item.index) {
        item.onUploaded = true;
        item.guid = guid;
        item.url = this.getFileUrl(item.fileType, url);
        item.absoluteFileUrl = url;
        item.uploadError = false;
        this.bugButtonClick();
      }
    });
  }

  onFileUploadedError(index, message) {
    this.fileData.forEach(item => {
      if (index == item.index) {
        if (item.fileType == DropZoneFiletype.DropImage) {
          this.coreUtilService.readFile(item.file).then(fileContents => {
            item.url = fileContents.toString();
            item.uploadError = true;
            item.uploadErrorMessage = message;
            this.bugButtonClick();
          });
        } else {
          item.url = this.getFileUrl(item.fileType, "");
          item.uploadError = true;
          item.uploadErrorMessage = message;
          this.bugButtonClick();
        }
      }
    });
  }

  // ngx-dropzone change detection bug'ı için ghost button tıklaması
  bugButtonClick() { this.bugButton.nativeElement.click(); }

  getInsertedFiles()           { return this.fileData.filter(f => !f.oldfile && f.onUploaded); }
  getDeletedFiles(): FileDropZoneData[] { return this.fileDataDeleted; }

  onRemove(event) {
    let data = event as FileDropZoneData;
    if (data.fileId != null && data.fileId > 0) this.fileDataDeleted.push(data);
    this.fileData.splice(this.fileData.indexOf(event), 1);
  }

  getfileType(file: File): DropZoneFiletype {
    if (file.type.startsWith("image"))           return DropZoneFiletype.DropImage;
    if (file.type.startsWith("application/pdf")) return DropZoneFiletype.DropPdf;
    return DropZoneFiletype.DropOther;
  }

  getFileUrl(type: DropZoneFiletype, url: string): string {
    if (type == DropZoneFiletype.DropImage) return url;
    if (type == DropZoneFiletype.DropPdf)   return DropZoneTempImage.Pdf;
    return DropZoneTempImage.Other;
  }
}
```

### 5.7. wrapper-core/ ve configs/

```typescript
// configs/custom-currency-mask.config.ts
export const CustomCurrencyMaskConfig: CurrencyMaskConfig = {
  align: "left",
  allowNegative: false,
  decimal: ",",       // Türkçe ondalık ayracı
  precision: 2,
  prefix: "",
  suffix: "",
  thousands: "."      // Türkçe binlik ayracı → 1.234,56
};
```

---

## 6. Shared Admin Modülü — Her Dosya Detaylı

### 6.1. HTTP Interceptor — Tam Kod + Açıklama

```typescript
// shared_admin/utils/interseptors/http-event-interseptor.ts
@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {
  private authLocalStorageToken = "auth_token";

  constructor(
    private authService: AuthenticationService,
    public srvProgressSpinner: ProgressSpinnerService
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authenticationModel: AuthenticationModel =
      JSON.parse(localStorage.getItem(this.authLocalStorageToken));

    if (authenticationModel) {
      request = this.addTokenHeader(request, authenticationModel);
    }

    return next.handle(request).pipe(
      tap(
        (event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
            this.srvProgressSpinner.hide(); // Başarılı yanıt — spinner kapat
          }
        },
        (error: any) => {
          if (error.status === 400) {
            this.srvProgressSpinner.hide();
          } else if (error instanceof HttpErrorResponse && error.status === 401) {
            return this.handle401Error(request, next);
          } else {
            this.srvProgressSpinner.hide();
          }
        }
      )
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    const authenticationModel: AuthenticationModel =
      JSON.parse(localStorage.getItem(this.authLocalStorageToken));
    if (authenticationModel) {
      this.authService.clearTokenFromLocalStorage();
      this.authService.logout(); // Token sil + /auth/login yönlendir
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
        return request.clone({ headers });
      } catch {
        this.authService.logout();
      }
    } else {
      const headers = new HttpHeaders()
        .set('Authorization', 'Bearer ' + authenticationModel.token.token);
      return request.clone({ headers });
    }
  }
}

// Module kayıt için hazır provider nesnesi
export const httpEventInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: HttpEventInterceptor,
  multi: true
};
```

### 6.2. AuthGuard — Tam Kod + Açıklama

```typescript
// shared_admin/auth/auth.guard.ts
@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthenticationService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentUser = this.authService.adminValue; // BehaviorSubject değeri

    // BehaviorSubject doluysa VE localStorage token varsa geçir
    if (currentUser && this.authService.getAuthFromLocalStorage()) {
      return true;
    }

    // Token yoksa login'e yönlendir
    this.authService.logout();
    return false;
  }
}
```

**Bilinen Eksiklik:** `canActivate` `boolean` döndürüyor, `Observable<boolean>` döndürmesi gerekir. `getAuthFromLocalStorage()` bir Observable ama burada senkron kullanılıyor.

### 6.3. AuthenticationService — Tam Kod

```typescript
// shared_admin/auth/authentication-service.ts
@Injectable()
export class AuthenticationService {
  public adminSubject: BehaviorSubject<AuthenticationModel> =
    new BehaviorSubject<AuthenticationModel>(null);
  isLoadingSubject: BehaviorSubject<boolean>;
  private authLocalStorageToken = "auth_token";
  private authUserInfo = "user_info";
  private srvUrl: string;

  constructor(private http: HttpClient, private router: Router) {
    this.adminSubject = new BehaviorSubject<AuthenticationModel>(null);
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.srvUrl = environment.apiUrl + "Authentication/";
  }

  public get adminValue(): AuthenticationModel { return this.adminSubject.value; }

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
    if (this.authLocalStorageToken != null &&
        this.authLocalStorageToken != "auth_token") {
      this.userLogout().subscribe(auth => { });
    }
    localStorage.removeItem(this.authLocalStorageToken);
    localStorage.removeItem(this.authUserInfo);
    this.adminSubject = new BehaviorSubject<AuthenticationModel>(undefined);
    this.router.navigate(['/auth/login']);
  }

  // APP_INITIALIZER'dan çağrılır — token varsa BehaviorSubject'i doldurur
  getAdminByToken(): Observable<AuthenticationModel> {
    const auth = this.getAuthFromLocalStorage();
    if (!auth) return of(undefined);
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

  clearTokenFromLocalStorage()       { localStorage.removeItem(this.authLocalStorageToken); }

  // İki adımlı login — önce form hazırla (CSRF benzeri key al)
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

### 6.4. Layout Component — Tam Kod

```typescript
// shared_admin/partials/layout/layout.component.ts
@Component({ selector: 'app-layout', ... })
export class LayoutComponent implements OnInit, AfterViewInit {
  supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]> =
    new BehaviorSubject<SupportNotificationItemModel[]>([]);

  admin: AuthenticationModel;
  offset: number = 0;
  searchCriterOtherSupport: SupportRequestModel = new SupportRequestModel();

  @ViewChild('ktAside', { static: true }) ktAside: ElementRef;
  @ViewChild('ktHeaderMobile', { static: true }) ktHeaderMobile: ElementRef;
  @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

  constructor(
    private initService: LayoutInitService,
    public layout: LayoutService,
    private srvSupportService: SupportService,
  ) {
    this.initService.init(); // Metronic layout konfigürasyonunu başlatır
  }

  ngOnInit(): void {
    // Metronic CSS sınıfları layout service'den alınır
    this.selfLayout = this.layout.getProp('self.layout');
    this.subheaderDisplay = this.layout.getProp('subheader.display');
    this.contentClasses = this.layout.getStringCSSClasses('content');
    this.asideCSSClasses = this.layout.getStringCSSClasses('aside');
    this.headerCSSClasses = this.layout.getStringCSSClasses('header');
    // ... diğer CSS sınıfları

    this.admin = JSON.parse(localStorage.getItem("auth_token"));

    // Yalnızca ADMIN ve ROOT_ADMIN kullanıcılar destek bildirimlerini görür
    if (this.admin.panelAdminUserType == PanelAdminUserType.ADMIN_USER ||
        this.admin.panelAdminUserType == PanelAdminUserType.ROOT_ADMIN_USER) {
      this.supportNotificationList$ = this.layout.supportNotificationList$;
      this.layout.supportNotification$.subscribe(support => {
        this.layout.supportNotificationsSubscribeCallback(support);
      });
      this.initSupportNotification();
    }
  }

  // SignalR bağlantısı başlatılır + açık destek talepleri yüklenir
  initSupportNotification() {
    return new Promise<void>((resolve, reject) => {
      this.srvSupportService.listForNotification(this.searchCriterOtherSupport)
        .subscribe((response: GetSupportNotificationItemResponseModel) => {
          this.layout.supportNotificationList$.next(
            response.supports.sort((a, b) => a.lastUpdateDate > b.lastUpdateDate ? -1 : 1)
          );
          this.layout.startConnectionSupportNotification();
          this.layout.addListenerSupport();
          resolve();
        }, (error) => { reject(); });
    });
  }

  ngOnDestroy() {
    this.layout.supportNotification$.unsubscribe();
    this.layout.supportNotificationList$.unsubscribe();
  }

  ngAfterViewInit(): void {
    // Metronic HTML niteliklerini DOM elementlerine yaz
    if (this.ktAside) {
      for (const key in this.asideHTMLAttributes) {
        if (this.asideHTMLAttributes.hasOwnProperty(key)) {
          this.ktAside.nativeElement.attributes[key] = this.asideHTMLAttributes[key];
        }
      }
    }
    KTLayoutContent.init('kt_content');
  }
}
```

### 6.5. Aside/Sidebar Component — Tam Kod

```typescript
// shared_admin/partials/aside/aside.component.ts
@Component({ selector: 'app-aside', ... })
export class AsideComponent implements OnInit {
  private authLocalStorageToken = "auth_token";
  authenticationData: AuthenticationModel = new AuthenticationModel();
  panelAdminUserType = PanelAdminUserType; // template'de enum karşılaştırması için

  constructor(
    private layout: LayoutService,
    private authService: AuthenticationService,
    private loc: Location
  ) {
    // Constructor'da token oku — template binding'ler hemen hazır olsun
    this.authenticationData = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
  }

  ngOnInit(): void {
    this.brandSkin = this.layout.getProp('brand.self.theme');
    this.headerLogo = this.getLogo();
    this.ulCSSClasses = this.layout.getProp('aside_menu_nav');
    this.asideMenuCSSClasses = this.layout.getStringCSSClasses('aside_menu');
    this.asideMenuHTMLAttributes = this.layout.getHTMLAttributes('aside_menu');
    this.asideMenuDropdown = this.layout.getProp('aside.menu.dropdown') ? '1' : '0';
    this.asideSelfMinimizeToggle = this.layout.getProp('aside.self.minimize.toggle');
    this.location = this.loc;
  }

  // Tema (light/dark) göre logo seçimi
  private getLogo() {
    return this.brandSkin === 'light'
      ? './assets/project-assets/evtechlogo.svg'
      : './assets/project-assets/evtechlogowhite.svg';
  }
}
```

**Template kullanım örneği (aside.component.html):**
```html
<!-- ROOT_ADMIN yalnızca görür -->
<li *ngIf="authenticationData.panelAdminUserType == panelAdminUserType.ROOT_ADMIN_USER">
  <a routerLink="/panelAdmin/list">Panel Admin</a>
</li>
```

### 6.6. Header/Topbar — Tam Kod

```typescript
// shared_admin/partials/header/header.component.ts
@Component({ selector: 'app-header', ... })
export class HeaderComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input() supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]>;
  @ViewChild('ktHeaderMenu', { static: true }) ktHeaderMenu: ElementRef;

  private loaderSubject: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  loader$: Observable<number>;
  private unsubscribe: Subscription[] = [];
  private routerLoaderTimout: any;

  constructor(private layout: LayoutService, private router: Router) {
    this.loader$ = this.loaderSubject;

    // Router event'lerden ilerleme yüzdesini hesapla (header progress bar)
    const routerSubscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart)     this.loaderSubject.next(10);
      if (event instanceof RouteConfigLoadStart) this.loaderSubject.next(65);
      if (event instanceof RouteConfigLoadEnd)   this.loaderSubject.next(90);
      if (event instanceof NavigationEnd || event instanceof NavigationCancel) {
        this.loaderSubject.next(100);
        if (this.routerLoaderTimout) clearTimeout(this.routerLoaderTimout);
        this.routerLoaderTimout = setTimeout(() => {
          this.loaderSubject.next(0);
        }, 300);
      }
    });
    this.unsubscribe.push(routerSubscription);
  }

  ngAfterViewInit(): void {
    KTUtil.ready(() => {
      KTLayoutHeader.init('kt_header', 'kt_header_mobile');
      KTLayoutHeaderMenu.init('kt_header_menu', 'kt_header_menu_wrapper');
    });
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
    if (this.routerLoaderTimout) clearTimeout(this.routerLoaderTimout);
  }
}
```

### 6.7. Material Module — Tam Kod

```typescript
// shared_admin/general-material.module.ts
@NgModule({
  exports: [
    MatInputModule,       MatFormFieldModule,   MatDatepickerModule,
    MatAutocompleteModule,MatListModule,         MatSliderModule,
    MatCardModule,        MatSelectModule,       MatButtonModule,
    MatIconModule,        MatNativeDateModule,   MatSlideToggleModule,
    MatCheckboxModule,    MatMenuModule,         MatTabsModule,
    MatTooltipModule,     MatSidenavModule,      MatProgressBarModule,
    MatProgressSpinnerModule, MatSnackBarModule, MatTableModule,
    MatGridListModule,    MatToolbarModule,      MatBottomSheetModule,
    MatExpansionModule,   MatDividerModule,      MatSortModule,
    MatStepperModule,     MatChipsModule,        MatPaginatorModule,
    MatDialogModule,      MatRippleModule,       MatRadioModule,
    MatTreeModule,        MatButtonToggleModule,
    FormsModule,          ReactiveFormsModule,   MatFileUploadModule,
    NgxDropzoneModule,    NgbModule,
  ],
  providers: [
    MatIconRegistry,
    { provide: MatBottomSheetRef, useValue: {} },
    { provide: MAT_BOTTOM_SHEET_DATA, useValue: {} },
    { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
    { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
    { provide: STEPPER_GLOBAL_OPTIONS, useValue: { displayDefaultIndicatorType: false } },
  ],
  declarations: [],
})
export class GeneralMaterialModule { }
```

### 6.8. Layout Module — Tam Kod

```typescript
// shared_admin/layout.module.ts
@NgModule({
  declarations: [
    LayoutComponent, ScriptsInitComponent, HeaderMobileComponent,
    AsideComponent,  FooterComponent,      HeaderComponent,
    HeaderMenuComponent, TopbarComponent,
  ],
  imports: [
    CommonModule,
    PagesRoutingModule,       // evtech/pages-routing.module — 22 lazy rota
    InlineSVGModule,
    LayoutOtherPartialsModule,
    NgbDropdownModule,
    NgbProgressbarModule,
    CoreModule,
    SubheaderModule,
    ProgressSpinnerModule,
    SharedModule,             // interceptor, pipe'lar, dialog'lar
  ],
})
export class LayoutModule { }
```

---

## 7. Feature Modülü (evtech/) — Her Dosya Detaylı

### 7.1. Routing — pages-routing.module.ts Tam Kod

```typescript
// evtech/pages-routing.module.ts
const routes: Routes = [
  {
    path: '',
    component: LayoutComponent, // Tüm feature sayfaları LayoutComponent içinde
    children: [
      { path: '', loadChildren: () => import('./components/home/home-module').then(m => m.HomeModule) },
      { path: 'announcement',    loadChildren: () => import('./components/announcement/announcement.module').then(m => m.AnnouncementModule) },
      { path: 'company',         loadChildren: () => import('./components/company/company-module').then(m => m.CompanyModule) },
      { path: 'campaign',        loadChildren: () => import('./components/campaign/campaign.module').then(m => m.CampaignModule) },
      { path: 'log',             loadChildren: () => import('./components/log/log-module').then(m => m.LogModule) },
      { path: 'messagesender',   loadChildren: () => import('./components/messagesender/messagesender-module').then(m => m.MessagesenderModule) },
      { path: 'paramaters',      loadChildren: () => import('./components/paramaters/paramaters-module').then(m => m.ParamatersModule) },
      { path: 'payments',        loadChildren: () => import('./components/payments/payments-module').then(m => m.PaymentsModule) },
      { path: 'support',         loadChildren: () => import('./components/support/support-module').then(m => m.SupportModule) },
      { path: 'reporting',       loadChildren: () => import('./components/reporting/reporting-module').then(m => m.ReportingModule) },
      { path: 'stations',        loadChildren: () => import('./components/stations/stations-module').then(m => m.StationsModule) },
      { path: 'chargeDevices',   loadChildren: () => import('./components/chargeDevice/chargeDevice-module').then(m => m.ChargeDeviceModule) },
      { path: 'authority',       loadChildren: () => import('./components/authority-management/authority-module').then(m => m.AuthorityModule) },
      { path: 'technicalSupport',loadChildren: () => import('./components/technical-support/technical-support-module').then(m => m.TechnicalSupportModule) },
      { path: 'users',           loadChildren: () => import('./components/users/users-module').then(m => m.UsersModule) },
      { path: 'versionManagement', loadChildren: () => import('./components/version-management/version-management-module').then(m => m.VersionManagementModule) },
      { path: 'language',        loadChildren: () => import('./components/language/language.module').then(m => m.LanguageModule) },
      { path: 'panelAdmin',      loadChildren: () => import('./components/panelAdmin/panelAdmin-module').then(m => m.PanelAdminModule) },
      { path: 'chargeDeviceReservation', loadChildren: () => import('./components/chargeDeviceReservation/chargeDeviceReservation-module').then(m => m.ChargeDeviceReservationModule) },
      { path: 'wallet',          loadChildren: () => import('./components/wallet/wallet-module').then(m => m.WalletModule) },
      { path: 'policy',          loadChildren: () => import('./components/policy/policy-module').then(m => m.PolicyModule) },
      { path: 'chargeManagment', loadChildren: () => import('./components/chargeManagment/chargeManagment-module').then(m => m.ChargeManagmentModule) },
      { path: 'test',            loadChildren: () => import('./components/test/test.module').then(m => m.TestModule) },
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

### 7.2. Models/Interfaces — Kritik Modeller Tam Kod

```typescript
// evtech/models/authentication/authentication-model.ts
import { PanelAdminUserType } from "../../enums/authentication/panel-admin-user-type-enum";
import { AccessTokenModel } from "./access-token-model";

export class AuthenticationModel {
  token:               AccessTokenModel;       // JWT access token + expiry
  name:                string;
  surname:             string;
  phone:               string;
  mail:                string;
  companyName:         string;
  connectionId:        string;                 // SignalR bağlantı ID
  companyId:           number;                 // Firma yetki filtresi
  panelAdminUserType:  PanelAdminUserType;      // Menü görünürlük kontrolü
}

// evtech/models/chargeDevice/chargeDevice-list-model.ts
export class ChargeDeviceListModel {
  id:                          number;
  state:                       ChargeDeviceState;
  identifier:                  string;           // OCPP kimlik kodu
  instantState:                ChargeDeviceInstantState;
  lastInstantStateUpdatedDate: Date;
  chargeDeviceMarkId:          number;
  chargeDeviceMarkText:        string;
  stationManagmentId:          number;
  stationManagmentName:        string;
  name:                        string;
  ocppUrl:                     string;
  connectStateInfo:            boolean;          // SignalR bağlantı durumu
  guiId:                       string;           // Guid
  acTypeCount:                 number;
  dcTypeCount:                 number;
  hearthBeatDate:              Date;
  connector:                   ConnectorListModel[];
}

// evtech/models/chargeDevice/chargeDevice-model.ts
export class ChargeDeviceModel {
  id:                number;
  state:             ChargeDeviceState;
  price:             number;
  kdv:               number;
  includeKdv:        boolean;
  priceWithKdv:      number;
  kW:                number;
  stationManagmentId:number;
  chargeDeviceMark:  ChargeDeviceMarkModel;
  Connector:         ConnectorModel[];
}
```

### 7.3. Enums — Her Enum Tam Kod

```typescript
// evtech/enums/authentication/panel-admin-user-type-enum.ts
export enum PanelAdminUserType {
  ADMIN_USER         = "ADMIN_USER",
  COMPANY_ADMIN_USER = "COMPANY_ADMIN_USER",
  ROOT_ADMIN_USER    = "ROOT_ADMIN_USER",
  TEST_ADMIN_USER    = "TEST_ADMIN_USER"
}
export enum AdminManagmentType {
  MAIN_ADMIN     = "MAIN_ADMIN",
  COMPANY_ADMIN  = "COMPANY_ADMIN",
  TEST_ADMIN_USER= "TEST_ADMIN_USER"
}
export const adminManagmentTypeMapping: Record<keyof typeof AdminManagmentType, string> = {
  MAIN_ADMIN:     "Genel Admin",
  COMPANY_ADMIN:  "Firma Admin",
  TEST_ADMIN_USER:"Test Admin"
};

// evtech/enums/chargeDevice/chargeDevice-type-enum.ts
export enum ChargeDeviceState {
  ACTIVE    = "ACTIVE",
  IN_CARE   = "IN_CARE",
  DEFECTIVE = "DEFECTIVE"
}
export const chargeDeviceStateMapping: Record<keyof typeof ChargeDeviceState, string> = {
  ACTIVE:    "Aktif",
  IN_CARE:   "Bakımda",
  DEFECTIVE: "Arızalı"
};
export enum ChargeDevicePowerType { AC = 1, DC = 2 }
export const chargeDevicePowerTypeMapping: Record<keyof typeof ChargeDevicePowerType, string> = {
  AC: "Ac", DC: "Dc"
};
export enum OcppMessageType { SENDER = "SENDER", RECEIVED = "RECEIVED" }
export const ocppMessageTypeMapping: Record<keyof typeof OcppMessageType, string> = {
  SENDER: "Gönderilen", RECEIVED: "Alınan"
};

// evtech/enums/charge/charge-state-enum.ts
export enum ChargeState {
  PROCESS_STARTING = "PROCESS_STARTING",
  PROCESS_START    = "PROCESS_START",
  PROCESS_ENDING   = "PROCESS_ENDING",
  PAYMENT_WAIT     = "PAYMENT_WAIT",
  PAYMENT_FAIL     = "PAYMENT_FAIL",
  COMPLETED        = "COMPLETED",
  FAILED           = "FAILED",
  CALCULATING      = "CALCULATING"
}
export const chargeStateMapping: Record<keyof typeof ChargeState, string> = {
  PROCESS_STARTING: "İşlem Başlatılıyor",
  PROCESS_START:    "Şarj Oluyor",
  PROCESS_ENDING:   "İşlem Durduruluyor",
  PAYMENT_WAIT:     "Ödeme Bekleniyor",
  PAYMENT_FAIL:     "Ödeme Başarısız",
  COMPLETED:        "İşlem Başarılı",
  FAILED:           "İşlem Başarısız",
  CALCULATING:      "İşlem Hesaplanıyor"
};

// evtech/enums/support/support-state-enum.ts
export enum SupportState {
  WAITING  = "WAITING",
  ANSWERED = "ANSWERED",
  CLOSED   = "CLOSED"
}
export const supportStateMapping: Record<keyof typeof SupportState, string> = {
  WAITING:  "Beklemede",
  ANSWERED: "Cevaplandı",
  CLOSED:   "Tamamlandı"
};

// evtech/enums/connector/connector-state-enum.ts
export enum ConnectorState { ACTIVE = "ACTIVE", PASSIVE = "PASSIVE" }
export const connectorStateMapping: Record<keyof typeof ConnectorState, string> = {
  ACTIVE: "Aktif", PASSIVE: "Pasif"
};

// evtech/enums/stationManagment/stationManagment-type.ts
export enum StationManagmentType { PUBLIC = "PUBLIC", PRIVATE = "PRIVATE" }
export const stationManagmentTypeMapping: Record<keyof typeof StationManagmentType, string> = {
  PUBLIC: "Halka Açık", PRIVATE: "Özel"
};
```

### 7.4. Services — 5 Kritik Service Tam Kod

**ChargeDeviceService:**
```typescript
@Injectable()
export class ChargeDeviceService {
  private srvUrl: string;
  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "ChargeDevice/";
  }
  list(searchCriter: DatatableRequestWrapper<ChargeDeviceFilterModel>)
    : Observable<DatatableResponseWrapper<ChargeDeviceListModel[]>> {
    return this.http.post<DatatableResponseWrapper<ChargeDeviceListModel[]>>(
      this.srvUrl + "GetChargeDeviceDataTablePanel", searchCriter
    );
  }
  getChargeDeviceById(id: number): Observable<ChargeDeviceUpdateModel> {
    return this.http.get<ChargeDeviceUpdateModel>(this.srvUrl + "GetChargeDeviceById", {
      params: { id: id.toString() }
    });
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
    return this.http.post<ChargeDeviceInsertFormPrepareModel>(
      this.srvUrl + "ChargeDevicePrepareInsertForm", null
    );
  }
}
```

**ChargeDeviceRemoteService (OCPP 1.6 uzak yönetim):**
```typescript
@Injectable()
export class ChargeDeviceRemoteService {
  private srvUrl: string;
  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "ChargeDeviceRemote/";
  }
  startTransaction(request: ChargeDeviceRemoteStartTransactionRequestModel)
    : Observable<ChargeDeviceRemoteStartTransactionResponseModel> {
    return this.http.post<ChargeDeviceRemoteStartTransactionResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteStartTransaction", request
    );
  }
  stopTransaction(request: ChargeDeviceRemoteStopTransactionRequestModel)
    : Observable<ChargeDeviceRemoteStopTransactionResponseModel> {
    return this.http.post<ChargeDeviceRemoteStopTransactionResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteStopTransaction", request
    );
  }
  changeAvailability(request: ChargeDeviceRemoteChangeAvailabilityRequestModel)
    : Observable<ChargeDeviceRemoteChangeAvailabilityResponseModel> {
    return this.http.post<ChargeDeviceRemoteChangeAvailabilityResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteChangeAvailability", request
    );
  }
  resetDevice(request: ChargeDeviceRemoteResetRequestModel)
    : Observable<ChargeDeviceRemoteResetResponseModel> {
    return this.http.post<ChargeDeviceRemoteResetResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteResetDevice", request
    );
  }
  prepareChangeConfigurationDeviceForm(request: PrepareChangeConfigurationDeviceFormRequestModel)
    : Observable<PrepareChangeConfigurationDeviceFormResponseModel> {
    return this.http.post<PrepareChangeConfigurationDeviceFormResponseModel>(
      this.srvUrl + "PrepareChangeConfigurationDeviceForm", request
    );
  }
  changeConfigurationDevice(request: ChargeDeviceRemoteChangeConfigurationRequestModel)
    : Observable<ChargeDeviceRemoteChangeConfigurationResponseModel> {
    return this.http.post<ChargeDeviceRemoteChangeConfigurationResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteChangeConfiguration", request
    );
  }
  chargeDeviceRemoteTriggerMessage(request: ChargeDeviceRemoteTriggerMessageRequestModel)
    : Observable<ChargeDeviceRemoteTriggerMessageResponseModel> {
    return this.http.post<ChargeDeviceRemoteTriggerMessageResponseModel>(
      this.srvUrl + "ChargeDeviceRemoteTriggerMessage", request
    );
  }
}
```

**DashboardService:**
```typescript
@Injectable()
export class DashboardService {
  private srvUrl: string;
  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "Dashboard/";
  }
  getStations(searchCriter: StationManagmentDashboardFilterModel)
    : Observable<StationManagmentDashboardItemModel[]> {
    return this.http.post<StationManagmentDashboardItemModel[]>(
      this.srvUrl + "GetStations", searchCriter
    );
  }
  getStationDetail(searchCriter: StationManagmentDashboardFilterModel)
    : Observable<StationManagmentDashboardItemModel> {
    return this.http.post<StationManagmentDashboardItemModel>(
      this.srvUrl + "GetStationDetail", searchCriter
    );
  }
  getMonthlyChargeProcess(searchCriter: ChargePrcessDashboardFilterModel)
    : Observable<MonthlyChargeProcessDashboardModel> {
    return this.http.post<MonthlyChargeProcessDashboardModel>(
      this.srvUrl + "GetMonthlyChargeProcess", searchCriter
    );
  }
  getMonthlyReservation(searchCriter: ReservationDashboardFilterModel)
    : Observable<MonthlyReservationDashboardModel[]> {
    return this.http.post<MonthlyReservationDashboardModel[]>(
      this.srvUrl + "GetMonthlyReservation", searchCriter
    );
  }
  getSumOfProcess(searchCriter: SumOfProcessRequestModel): Observable<SumOfProcessModel> {
    return this.http.post<SumOfProcessModel>(this.srvUrl + "GetSumOfProcess", searchCriter);
  }
}
```

**SupportService:**
```typescript
@Injectable()
export class SupportService {
  private srvUrl: string;
  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "SupportManagment/";
  }
  listDataTable(searchCriter: DatatableRequestWrapper<SupportDatatableRequestModel>)
    : Observable<DatatableResponseWrapper<SupportDatatableItemModel[]>> {
    return this.http.post<DatatableResponseWrapper<SupportDatatableItemModel[]>>(
      this.srvUrl + "GetSupportForDatatablePanel", searchCriter
    );
  }
  list(searchCriter: DatatableRequestWrapper<SupportRequestModel>): Observable<SupportItemModel[]> {
    return this.http.post<SupportItemModel[]>(this.srvUrl + "GetSupportList", searchCriter);
  }
  getSupportDetail(searchCriter: SupportRequestModel): Observable<SupportItemModel> {
    return this.http.post<SupportItemModel>(this.srvUrl + "GetSupportDetail", searchCriter);
  }
  answerSupport(searchCriter: SupportAnswerRequestModel): Observable<SupportAnswerResponseModel> {
    return this.http.post<SupportAnswerResponseModel>(this.srvUrl + "AnswerSupport", searchCriter);
  }
  closeSupport(searchCriter: SupportCloseRequestModel): Observable<SupportCloseResponseModel> {
    return this.http.post<SupportCloseResponseModel>(this.srvUrl + "CloseSupport", searchCriter);
  }
  listForNotification(searchCriter: SupportRequestModel)
    : Observable<GetSupportNotificationItemResponseModel> {
    return this.http.post<GetSupportNotificationItemResponseModel>(
      this.srvUrl + "GetSupportListForNotification", searchCriter
    );
  }
}
```

**PaymentService:**
```typescript
@Injectable()
export class PaymentService {
  private srvUrl: string;
  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "UserPaymentManagment/";
  }
  getUserPayments(searchCriter: UserPaymentFilterModel): Observable<UserPaymentListItemModel[]> {
    return this.http.post<UserPaymentListItemModel[]>(this.srvUrl + "GetUserPayments", searchCriter);
  }
  getUserPaymentDetail(searchCriter: GetUserPaymentDetailRequestModel)
    : Observable<UserPaymentDetailModel> {
    return this.http.post<UserPaymentDetailModel>(
      this.srvUrl + "GetUserPaymentDetailForPanel", searchCriter
    );
  }
  refundPayment(request: UserPaymentRefundRequestModel): Observable<UserPaymentRefundResponseModel> {
    return this.http.post<UserPaymentRefundResponseModel>(
      this.srvUrl + "RefundUserPayment", request
    );
  }
  getUserPaymentsDataTable(searchCriter: DatatableRequestWrapper<UserPaymentDataTableFilterModel>)
    : Observable<DatatableResponseWrapper<UserPaymentListItemModel[]>> {
    return this.http.post<DatatableResponseWrapper<UserPaymentListItemModel[]>>(
      this.srvUrl + "GetUserPaymentsDatatable", searchCriter
    );
  }
  setPaymentToChargeProcess(searchCriter: SetPaymentToChargeProcessRequestModel)
    : Observable<SetPaymentToChargeProcessResponseModel> {
    return this.http.post<SetPaymentToChargeProcessResponseModel>(
      this.srvUrl + "SetPaymentToChargeProcess", searchCriter
    );
  }
}
```

### 7.5. Components — 5 Component Tam Kod

**1. HomeComponent (Dashboard + Google Maps):**
```typescript
// evtech/components/home/home.component.ts
@Component({ selector: 'app-home', ... })
export class HomeComponent implements OnInit {
  @ViewChild(DashboardMonthlyReservationComponent)   childDashboardMonthlyReservation;
  @ViewChild(DashboardMonthlyChargeProcessComponent) childDashboardMonthlyChargeProcess;
  @ViewChild(DashboardSumOfProcessComponent)         childDashboardSumOfProcess;

  admin: AuthenticationModel;
  companyList: CompanySelectListModel[] = [];
  companyFilter: CompanyFilterModel = new CompanyFilterModel();
  stationList: StationManagmentDashboardItemModel[] = [];
  stationListFilter: StationManagmentDashboardFilterModel = new StationManagmentDashboardFilterModel();
  public lat: number = 39.333439;  // Türkiye merkezi
  public lng: number = 35.831485;
  zoom: number = 5.7;

  constructor(
    private subheader: SubHeaderService,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvDashboard: DashboardService,
    private srvCompanyService: CompanyService,
    private srvUtils: UtilsService,
    public gMaps: GoogleMapsAPIWrapper,
    private router: Router,
  ) {
    this.stationListFilter.hasProcess = null;
    this.stationListFilter.isActive = null;
  }

  ngOnInit() {
    this.subheader.setBreadcrumbs([{ title: 'Ana Sayfa', linkPath: '', linkText: '', isActive: false }]);
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    this.companyFilter.id = this.admin.companyId;
    this.getCompanies();
  }

  getCompanies() {
    this.srvCompanyService.getCompanyForSelectList(this.companyFilter)
      .subscribe((response: CompanySelectListModel[]) => {
        this.companyList = response;
        this.initData();
        // Child component'ları başlat — @ViewChild referansları ile
        this.childDashboardMonthlyChargeProcess.getMonthlyChargeProcess();
        this.childDashboardMonthlyReservation.getMonthlyReservation();
        this.childDashboardSumOfProcess.getSumOfProcess();
      });
  }

  initData() {
    this.stationListFilter.companyId = this.companyFilter.id;
    this.srvDashboard.getStations(this.stationListFilter)
      .subscribe((response: StationManagmentDashboardItemModel[]) => {
        this.stationList = response;
        this.setMarkerIcon();
        this.allStationCountForInfo = this.stationList.length;
        this.allStationInProcessCountForInfo = this.stationList.filter(x => x.hasProcess).length;
        this.allAvailableStationCountForInfo = this.stationList.filter(x => x.isActive).length;
      });
  }

  // İstasyon durumuna göre harita marker rengi
  setMarkerIcon() {
    this.stationList.map((item) => {
      item.iconUrl = item.isActive
        ? "../../../../assets/project-assets/green_marker.svg"
        : "../../../../assets/project-assets/red_marker.svg";
      if (item.hasProcess)
        item.iconUrl = "../../../../assets/project-assets/blue_marker.svg";
    });
  }

  markerClicked(id) {
    this.stationList.filter(x => x.id == id).map((item) => {
      if (item.isOpen) {
        item.isOpen = false;
      } else {
        this.getStationDetail(id);
        item.isOpen = true;
      }
    });
  }
}
```

**2. ChargeDeviceListComponent (Liste + Sort + Paginator):**
```typescript
// evtech/components/chargeDevice/chargeDevice-list/chargeDevice-list.component.ts
@Component({ selector: 'app-chargeDevice-list', ... })
export class ChargeDeviceListComponent implements OnInit {
  admin: AuthenticationModel;
  showingColumnNamesChargeDeviceTable: string[] = [
    'Select','Id','StationManagmentName','Name','State','InstantState','Edit'
  ];
  chargeDeviceList: MatTableDataSource<ChargeDeviceListModel> = new MatTableDataSource();
  totalRecordSizeChargeDeviceTable: number;
  @ViewChild(MatPaginator) paginatorChargeDeviceTable: MatPaginator;
  @ViewChild(MatSort)      sortChargeDeviceTable: MatSort;
  searchCriterChargeDevice: DatatableRequestWrapper<ChargeDeviceFilterModel> =
    new DatatableRequestWrapper();

  chargeDeviceStateMapping       = chargeDeviceStateMapping;
  chargeDeviceInstantStateMapping = chargeDeviceInstantStateMapping;

  constructor(
    private srvProgressSpinner: ProgressSpinnerService,
    private srvChargeDeviceService: ChargeDeviceService,
    private srvStationManagmentService: StationManagmentService,
    private srvUtils: UtilsService,
    private router: Router,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    public yesNoDialog: MatDialog,
    private srvCompanyService: CompanyService,
    private subheader: SubHeaderService
  ) {}

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: '', linkText: '', isActive: true },
      { title: 'Şarj Cihaz Listesi', linkPath: null, linkText: '', isActive: false },
    ]);
    this.idSubscription = this.route.params.subscribe(params => {
      this.searchCriterChargeDevice.data = new ChargeDeviceFilterModel();
      this.stationId = Number(params['stationManagmentId']);
      this.admin = JSON.parse(localStorage.getItem("auth_token"));
      this.companyFilter.id = this.admin.companyId;
      this.srvProgressSpinner.show();
      this.getCompanies();
    });
  }

  getChargeDevices(hasFilter: boolean) {
    this.chargeDeviceList.data = [];
    this.srvProgressSpinner.show();
    if (hasFilter) this.paginatorChargeDeviceTable.pageIndex = 0;
    // Sort
    if (this.sortChargeDeviceTable?.direction && this.sortChargeDeviceTable?.active) {
      this.searchCriterChargeDevice.orderDirective = this.sortChargeDeviceTable.direction;
      this.searchCriterChargeDevice.orderProperty  = this.sortChargeDeviceTable.active;
    } else {
      this.searchCriterChargeDevice.orderDirective = "desc";
      this.searchCriterChargeDevice.orderProperty  = "Id";
    }
    // Paginator
    if (this.paginatorChargeDeviceTable) {
      this.searchCriterChargeDevice.recordPerPage = this.paginatorChargeDeviceTable.pageSize;
      this.searchCriterChargeDevice.pageNumber    = this.paginatorChargeDeviceTable.pageIndex;
    } else {
      this.searchCriterChargeDevice.recordPerPage = 15;
    }
    this.srvChargeDeviceService.list(this.searchCriterChargeDevice)
      .subscribe((response: DatatableResponseWrapper<ChargeDeviceListModel[]>) => {
        this.chargeDeviceList.data = response.data;
        this.totalRecordSizeChargeDeviceTable = response.recordCount;
        this.srvProgressSpinner.hide();
      }, (error) => {
        this.srvProgressSpinner.hide();
        let errorData = this.srvUtils.getServerErrorRequest(error);
        this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
      });
  }

  ngAfterViewInit(): void {
    this.sortChargeDeviceTable.sortChange.subscribe(() => {
      this.paginatorChargeDeviceTable.pageIndex = 0;
    });
    merge(this.sortChargeDeviceTable.sortChange, this.paginatorChargeDeviceTable.page)
      .pipe(tap(() => { this.getChargeDevices(false); }))
      .subscribe(() => {}, () => null);
  }

  openConnectorListDialog(deviceId, deviceName): void {
    this.dialog.open(ChargeDeviceConnectorListPartialComponent, {
      width: '1200px', height: '90vh',
      data: { chargeDeviceId: deviceId, deviceName: deviceName },
    });
  }
}
```

**3. AddChargeDeviceComponent (Form — Connector ekleme):**
```typescript
// evtech/components/chargeDevice/chargeDevice-add/chargeDevice-add.component.ts
@Component({ selector: 'app-chargeDevice-add', ... })
export class AddChargeDeviceComponent implements OnInit {
  chargeDeviceInsertModel: ChargeDeviceInsertModel = new ChargeDeviceInsertModel();
  chargeDeviceInsertFormPrepareModel: ChargeDeviceInsertFormPrepareModel =
    new ChargeDeviceInsertFormPrepareModel();
  connectorNumber: number = 0;

  constructor(
    private router: Router, private route: ActivatedRoute,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvChargeDevice: ChargeDeviceService,
    private srvUtils: UtilsService,
    public yesNoDialog: MatDialog,
    private subheader: SubHeaderService
  ) {
    this.chargeDeviceInsertModel.chargeDeviceConnector = [];
  }

  ngOnInit() {
    this.idSubscription = this.route.params.subscribe(params => {
      this.chargeDeviceInsertModel.stationManagmentId = params['stationManagmentId'];
      this.chargeDeviceInsertFormPrepare();
    });
  }

  chargeDeviceInsertFormPrepare() {
    this.srvProgressSpinner.show();
    this.srvChargeDevice.prepareInsertForm()
      .subscribe((response: ChargeDeviceInsertFormPrepareModel) => {
        this.chargeDeviceInsertFormPrepareModel = response;
      }, (error) => { this.srvProgressSpinner.hide(); });
  }

  addConnector() {
    this.connectorNumber++;
    this.chargeDeviceInsertModel.chargeDeviceConnector.push({
      id: 0, connectorNo: this.connectorNumber,
      state: ConnectorState.ACTIVE, identifier: "", disabled: null,
      price: 0, includeKdv: false, kdv: 0,
      processState: ConnectorProcessState.AVAILABLE, priceWithKdv: 0,
      chargeDevicePowerTypeId: 1, chargeDeviceSocketTypeId: 1,
      kW: 0, name: "", serialNumber: "", socketNumber: "",
      waitingKdv: 0, waitingPrice: 0,
      waitingPriceState: ConnectorWaitingPriceState.NONE,
      waitingPriceWithKdv: 0, confirmedPriceDate: null
    });
  }

  removeConnectorDialog(connectorNo: number) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "Bağlayıcıyı Kaldırmak İstediğinize Emin misiniz ?" }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.chargeDeviceInsertModel.chargeDeviceConnector =
          this.chargeDeviceInsertModel.chargeDeviceConnector
            .filter(x => x.connectorNo != connectorNo);
        // Sırala ve connectorNo yeniden ata
        this.connectorNumber = 0;
        this.chargeDeviceInsertModel.chargeDeviceConnector.forEach(item => {
          this.connectorNumber++;
          item.connectorNo = this.connectorNumber;
        });
      }
    });
  }

  save() {
    this.srvProgressSpinner.show();
    this.srvChargeDevice.add(this.chargeDeviceInsertModel).subscribe((response: any) => {
      this.srvUtils.showActionNotification("Başarıyla Eklendi", EnumMessageType.Success, 8000);
      setTimeout(() => { this.router.navigate(['chargeDevices/list']); }, 2500);
    }, (error) => { this.srvProgressSpinner.hide(); });
  }
}
```

**4. ChargeListComponent (Şarj İşlemleri):**
```typescript
// evtech/components/chargeManagment/charge-list/charge-list.component.ts
@Component({ selector: 'app-charge-list', ... })
export class ChargeListComponent implements OnInit {
  showingColumnNamesChargeTable: string[] = [
    'Id','StationName','DeviceName','ConnectorName',
    'StartingDate','LastUpdateDate','LoadedKw','PaidPrice',
    'State','ApplicationType','Edit'
  ];
  chargeList: MatTableDataSource<ChargeDatatableItemModel> = new MatTableDataSource();
  searchCriterCharge: DatatableRequestWrapper<ChargeDatatableFilterModel> =
    new DatatableRequestWrapper();
  chargeStateMapping           = chargeStateMapping;
  chargeApplicationTypeMapping = chargeApplicationTypeMapping;

  getCharges(hasFilter: boolean) {
    this.srvProgressSpinner.show();
    if (hasFilter) this.paginatorChargeTable.pageIndex = 0;
    // sort + paginator ayarları (aynı pattern) ...
    this.srvChargeManagmentService.list(this.searchCriterCharge)
      .subscribe((response: DatatableResponseWrapper<ChargeDatatableItemModel[]>) => {
        this.chargeList.data = response.data;
        this.totalRecordSizeChargeTable = response.recordCount;
        this.srvProgressSpinner.hide();
      });
  }

  // Ödeme durumunu kontrol et — tam liste refresh yapmadan sadece ilgili satırı güncelle
  checkChargePayment(id, guiId) {
    this.checkChargePaymentRequestModel.id    = id;
    this.checkChargePaymentRequestModel.guiId = guiId;
    this.srvProgressSpinner.show();
    this.srvChargeManagmentService.checkChargePayment(this.checkChargePaymentRequestModel)
      .subscribe((response: CheckChargePaymentResponseModel) => {
        this.chargeList.data.filter(p => p.id == id).map(charge => {
          charge.state = response.state;  // Sadece state güncellenir
        });
        this.srvProgressSpinner.hide();
      });
  }

  stopChargeProcess(guiId) {
    this.stopChargeProcessRequestModel.chargeProcessGuiId = guiId;
    this.srvProgressSpinner.show();
    this.srvChargeManagmentService.stopChargeProcess(this.stopChargeProcessRequestModel)
      .subscribe((response: StopChargeProcessResponseModel) => {
        if (response.state != null) {
          this.chargeList.data.filter(p => p.guiId == guiId).map(charge => {
            charge.state = response.state;
          });
        }
        this.srvProgressSpinner.hide();
      });
  }

  openChargeDetailDialog(id, guiId, stationId): void {
    this.dialog.open(ChargeDetailPartialComponent, {
      width: '1200px', height: '90vh',
      data: { id, guiId, stationId }
    });
  }
}
```

**5. LoginComponent (İki Adımlı Giriş):**
```typescript
// evtech/components/auth/login/login.component.ts
@Component({ selector: 'app-login', ... })
export class LoginComponent implements OnInit {
  prepareLoginFormRequestModel: PrepareLoginFormRequestModel =
    new PrepareLoginFormRequestModel();
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
    // Zaten giriş yapılmışsa ana sayfaya yönlendir
    if (this.srvAuthService.adminValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.splashScreenService.hide(); // Splash screen'i kapat
    this.initForm();
  }

  initForm() {
    this.loginForm = this.fb.group({
      userName: [null, Validators.compose([Validators.required])],
      password: [null, Validators.compose([Validators.required])]
    });
    this.prepareLoginForm(); // 1. Adım: CSRF benzeri form anahtarı al
  }

  prepareLoginForm() {
    this.srvProgressSpinner.show();
    this.srvAuthService.prepareLoginForm(this.prepareLoginFormRequestModel)
      .subscribe((response: PrepareLoginFormResponseModel) => {
        // loginFormKey login isteğine eklenir
        this.loginRequestModel.loginFormKey = response.loginFormKey;
      }, (error) => {
        this.srvProgressSpinner.hide();
        let errorData = this.srvUtils.getServerErrorRequest(error);
        this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
      });
  }

  login() {
    this.srvProgressSpinner.show();
    this.srvAuthService.isLoadingSubject.next(true);
    if (this.loginForm.invalid) {
      Object.keys(this.loginForm.controls).forEach(c =>
        this.loginForm.controls[c].markAsTouched()
      );
      this.srvProgressSpinner.hide();
      return;
    }
    // 2. Adım: Kullanıcı adı + şifre + loginFormKey ile giriş
    const loginSubscr = this.srvAuthService.logIn(this.loginRequestModel)
      .subscribe((auth: LoginResponseModel) => {
        if (auth.accessToken) {
          var authenticationModel = new AuthenticationModel();
          authenticationModel.token             = auth.accessToken;
          authenticationModel.name              = auth.name;
          authenticationModel.surname           = auth.surname;
          authenticationModel.phone             = auth.phone;
          authenticationModel.mail              = auth.mail;
          authenticationModel.connectionId      = auth.connectionId;
          authenticationModel.companyName       = auth.companyName;
          authenticationModel.companyId         = auth.companyId;
          authenticationModel.panelAdminUserType= auth.panelAdminUserType;

          this.srvAuthService.setAuthFromLocalStorage(authenticationModel);
          this.srvAuthService.adminSubject =
            new BehaviorSubject<AuthenticationModel>(authenticationModel);
          this.router.navigate(['']);
        }
        this.srvAuthService.isLoadingSubject.next(false);
      }, (error) => {
        let errorData = this.srvUtils.getServerErrorRequest(error);
        this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
        this.srvAuthService.isLoadingSubject.next(false);
        this.srvProgressSpinner.hide();
      });
    this.unsubscribe.push(loginSubscr);
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
```

---

## 8. Environment Dosyaları — Tam Kod

```typescript
// environment.ts — Geliştirme (localhost portları)
export const environment = {
  production:              false,
  appVersion:              'v717demo1',
  isTest:                  false,
  apiUrl:                  'http://localhost:9180/v1/',
  webSiteApiUrl:           'http://localhost:35573/v1/',
  tockenUrl:               'http://localhost:14493/v1/',
  fileApiUrl:              'http://localhost:56301/v1/',
  testApiUrl:              'http://localhost:54123/v1/',
  imageUrl:                'http://localhost:25500/v1/FileView/GetFile?code=',
  logApiUrl:               'http://localhost:9622/v1/',
  notificationApiUrl:      'http://localhost:52440/',    // SignalR hub URL'i
  stationApiUrl:           'http://localhost:14258/v1/',
  ocppApiUrl:              'http://localhost:59439/v1/',
  stationNotificationApiUrl: 'http://localhost:14258/'
};

// environment.test.ts — Test sunucu (213.159.5.123)
export const environment = {
  production:              true,
  appVersion:              'v717demo1',
  isTest:                  false,
  apiUrl:                  'http://213.159.5.123:8001/v1/',
  tockenUrl:               'http://213.159.5.123:8097/v1/',
  fileApiUrl:              'http://213.159.5.123:8099/v1/',
  testApiUrl:              'http://213.159.5.123:54123/v1/',
  imageUrl:                'http://213.159.5.123:8099/v1/FileView/GetFile?code=',
  logApiUrl:               'http://213.159.5.123:8004/v1/',
  notificationApiUrl:      'http://213.159.5.123:8057/',
  stationApiUrl:           'http://213.159.5.123:8095/v1/',
  ocppApiUrl:              'http://213.159.5.123:8093/v1/',
  stationNotificationApiUrl: 'http://213.159.5.123:14258/'
};

// environment.prod.ts — Production (henüz localhost HTTPS — canlıya taşınmamış)
export const environment = {
  production:  true,
  appVersion:  'v717demo1',
  isTest:      false,
  apiUrl:      'https://localhost:44312/v1/',
  tockenUrl:   'https://localhost:44320/v1/',
  fileApiUrl:  'http://localhost:56301/v1/',
  imageUrl:    'http://localhost:25500/v1/FileView/GetFile?code=',
  logApiUrl:   'https://localhost:44313/v1/',
  notificationApiUrl: 'http://localhost:52440/',
  stationApiUrl:  'https://localhost:44325/v1/',
  ocppApiUrl:     'https://localhost:44389/v1/',
  stationNotificationApiUrl: 'http://localhost:14258/'
};
```

**API Port Haritası:**

| Port | Servis | Ortam |
|---|---|---|
| 9180 | Ana API | Dev |
| 9622 | Log API | Dev |
| 52440 | SignalR (Notification) | Dev |
| 14258 | Station Notification | Dev |
| 59439 | OCPP API | Dev |
| 8001 | Ana API | Test |
| 8004 | Log API | Test |
| 8057 | SignalR | Test |
| 8093 | OCPP API | Test |
| 8095 | Station API | Test |
| 8099 | File API | Test |

---

## 9. Mimari Kararlar ve Dikkat Noktaları

### 9.1. Mikroservis Backend Mimarisi
7 farklı servis URL tanımlanmış: ana API, log API, token API, dosya API, SignalR notification, station API, OCPP API. Backend mikroservis tabanlıdır ve her servis bağımsız port/container üzerinde çalışır.

### 9.2. OCPP 1.6 Protokolü Entegrasyonu
`ChargeDeviceRemoteService` üzerinden OCPP 1.6 mesajları gönderilir: `StartTransaction`, `StopTransaction`, `ChangeAvailability`, `Reset` (Hard/Soft), `TriggerMessage`, `ChangeConfiguration`, `GetConfiguration`. Bu işlemler `ocppApiUrl` endpoint'ine gider ve cihaza OCPP protokolü ile iletilir.

### 9.3. SignalR Destek Bildirimi Akışı
```
LayoutComponent.initSupportNotification()
  → SupportService.listForNotification()     (mevcut açık talepler)
  → LayoutService.startConnectionSupportNotification()  (hub bağlantısı kur)
  → LayoutService.addListenerSupport()       (hub'ı dinlemeye başla)
  → supportNotification$ BehaviorSubject'e push
  → LayoutComponent subscribe → supportNotificationsSubscribeCallback()
  → supportNotificationList$ BehaviorSubject güncelle
  → Header'daki bildirim sayacı yenilenir
```

### 9.4. Firma Bazlı Yetki Filtresi
Her liste componentinde: `this.admin = JSON.parse(localStorage.getItem("auth_token"))` → `companyFilter.id = this.admin.companyId`. ROOT_ADMIN companyId = null ile tüm firmaları görür, COMPANY_ADMIN yalnızca kendi firmasını.

### 9.5. İki Adımlı Login (PrepareLoginForm)
Giriş `PrepareLoginForm` → `Login` zinciriyle yapılır. İlk çağrı bir `loginFormKey` döndürür ve bu key ikinci istekte gönderilir. Bu tekrar dönen form saldırılarına (replay attack) karşı basit bir koruma sağlar.

### 9.6. Enum + Türkçe Mapping Pattern
Proje genelinde tutarlı kullanılan pattern:
```typescript
// 1. Enum (string değerli)
export enum ChargeState { COMPLETED = "COMPLETED", FAILED = "FAILED" }

// 2. Türkçe etiket haritası
export const chargeStateMapping: Record<keyof typeof ChargeState, string> = {
  COMPLETED: "İşlem Başarılı",
  FAILED:    "İşlem Başarısız"
};

// 3. Template kullanımı
// {{ chargeStateMapping[item.state] }}   → "İşlem Başarılı"
// {{ item.state | chargeStateCss }}      → "badge badge-success"
```

### 9.7. LayoutService içinde SignalR
`LayoutService` (`providedIn: 'root'`) hem layout CSS yapılandırmasını hem de SignalR destek bildirimlerini yönetiyor. İki farklı sorumluluğun aynı serviste olması Single Responsibility prensibini çiğniyor.

---

## 10. Sorunlar ve İyileştirme Alanları

### 10.1. Subscription Sızıntıları
`ngOnDestroy`'da unsubscribe edilmeyen `merge().subscribe()` çağrıları. Her liste componentinde bu pattern tekrarlanıyor:
```typescript
// MEVCUT — sızıntı var:
merge(this.sortTable.sortChange, this.paginatorTable.page)
  .pipe(tap(() => { this.getChargeDevices(false); }))
  .subscribe(() => {}, () => null);  // hiç unsubscribe edilmiyor

// OLMASI GEREKEN:
private destroy$ = new Subject<void>();
merge(this.sortTable.sortChange, this.paginatorTable.page)
  .pipe(
    tap(() => { this.getChargeDevices(false); }),
    takeUntil(this.destroy$)
  )
  .subscribe();
ngOnDestroy() { this.destroy$.next(); this.destroy$.complete(); }
```

### 10.2. localStorage Bağımlılığı Her Component'te
Her component kendi başına `JSON.parse(localStorage.getItem("auth_token"))` yapıyor. Bu mantık `AuthenticationService.adminValue` getter'ına taşınmalıdır.

### 10.3. OnPush Change Detection Yok
Hiçbir component `ChangeDetectionStrategy.OnPush` kullanmıyor. Özellikle büyük listeler (150+ cihaz) için performans kaybı oluşur.

### 10.4. console.log Production Build'de
Birçok dosyada development debug için eklenmiş `console.log()` çağrıları temizlenmemiş. `angular.json`'da `"optimization": true` ile `tsconfig` üzerinden otomatik kaldırılabilir.

### 10.5. AuthGuard Observable Dönmüyor
`canActivate` senkron `boolean` döndürüyor. `getAuthFromLocalStorage()` Observable döndürüyor ama bekleniyor. Doğru implementasyon:
```typescript
canActivate(): Observable<boolean> {
  return this.authService.getAuthFromLocalStorage().pipe(
    map(auth => {
      if (auth) return true;
      this.authService.logout();
      return false;
    })
  );
}
```

### 10.6. environment.prod.ts Canlı URL İçermiyor
Production build hala `localhost` URL'leri kullanıyor — gerçek production deploy yapılmamış veya `angular.json` `fileReplacements` konfigürasyonu eksik.

### 10.7. TextOrNumberDirective Hardcode Karakter Listesi
150+ satır `if` zinciri yerine:
```typescript
// MEVCUT:
if (e.key !== 'a' && e.key !== 'b' && ... ) e.preventDefault();

// OLMASI GEREKEN:
private allowedPattern = /^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9]$/;
if (!this.allowedPattern.test(e.key)) e.preventDefault();
```

### 10.8. LayoutService Çift Sorumluluk
Hem Metronic CSS konfigürasyonu hem de SignalR bağlantı yönetimi tek serviste. `SupportNotificationService` ayrı bir servis olarak çıkarılmalı.

### Lazy Loading — Tüm Feature Modülleri
Auth, Layout ve Error sayfaları hepsi lazy load ediliyor:
```typescript
loadChildren: () => import('./evtech/auth/auth.module')...
```
İlk yükleme süresini minimize eder, admin panel büyüklüğü göz önüne alındığında kritik.

### Dual Chart Library
Hem `apexcharts`/`ng-apexcharts` hem de `chart.js`/`ng2-charts` kullanılıyor. İki farklı chart kütüphanesi olması muhtemelen farklı zamanlarda eklenen componentlerden kaynaklanıyor. Bundle size artırır, tek kütüphaneye geçiş tercih edilir.

### @microsoft/signalr
Live monitoring componenti için SignalR WebSocket kullanılıyor. Real-time cihaz durumu takibi için.

## Feature Modülü Organizasyonu

28 component kategorisi — en büyük panel. Kategoriler şu prensiple ayrılmış:

- **İşlevsel gruplar:** şarj cihazı, istasyon, rezervasyon, ödeme kendi klasörlerinde
- **Cross-cutting:** auth, authority, panelAdmin genel yönetim için ayrı
- **Support:** log, reporting, technical-support izleme için ayrı
- **Config:** paramaters, system-parameter, language, version-management konfigürasyon için ayrı

Bu granülarite büyük projelerde iyi çalışır, her kategori kendi modülüne lazy load edilebilir.

## Core / Shared / Feature Ayrımı

### core/
- `adapters/` — data dönüşüm katmanı (API response → component model)
- `bases/base-datatable/`, `bases/base-tree-table/` — tablo componentleri için abstract base class'lar
- `directives/` — 7 directive (form validation, input masking, vb.)
- `pipes/` — 6 pipe (format dönüşümleri)
- `external-components/` — özelleştirilmiş üçüncü parti wrapper'lar (dropzone, html editor)
- `date-core/` — tarih işlemleri için özel servis/util
- `configs/` — global konfigürasyon nesneleri
- `wrapper-core/` — dış servis wrapper'ları

Core modülü `AppModule`'e import edilir, uygulama boyunca tekil instance sağlar.

### shared_admin/
- Layout componentleri (Metronic'in aside, header, footer, topbar yapısı)
- Auth guard ve authentication service
- HTTP interceptor
- Material modülü — Angular Material import consolidation

### evtech/ (feature)
- Tüm business logic
- Domain'e ait model, service ve component'lar
- Kendi içinde alt routing modülü

## Interceptor / Guard Yaklaşımı

### HTTP Interceptor (http-event-interseptor.ts)
- `shared_admin/utils/interseptors/` altında (typo: "interseptor")
- `AppModule.providers`'a HTTP_INTERCEPTORS token ile sağlanır
- Her request'e Bearer token ekler
- 401'de localStorage temizleme + redirect

### AuthGuard
- `shared_admin/auth/` altında
- `CanActivate` implement eder
- localStorage token varlığı kontrolü
- Token yoksa `/auth/login`'e yönlendirir, returnUrl query param ile

## Metronic Template Kullanımı

Metronic v7.1.7, Keenthemes'in ticari Angular admin template'i. Bu proje Metronic'i bazı şekillerde kullanıyor:

1. **Layout sistemi** — aside, header, footer, subheader, topbar componentleri Metronic'ten alınmış, özelleştirilmiş
2. **SCSS değişkenleri** — renk şeması, tipografi Metronic tema sistemi üzerinden
3. **shared_admin/** klasörü — Metronic'in önerdiği klasör yapısına yakın
4. **Splash screen** — Metronic'in yükleme ekranı komponenti

Metronic, lisanslı bir template olduğundan doğrudan npm paketi olarak gelmez; kaynak koduna entegre edilir.

## Projeler Arası Benzerlikler

### sarj_ev_panel_angular vs sarj_pro_panel_angular
Bu iki proje neredeyse aynı altyapıya sahip:
- Özdeş `core/` yapısı
- Özdeş `shared_admin/` yapısı
- Özdeş `package.json` bağımlılıkları
- 4 environment dosyası (aynı ortam stratejisi)
- Sadece feature modülü (evtech vs sarjAllPro) ve scope farklı

Bu durum muhtemelen sarj_pro'nun sarj_ev'den fork edildiğine ya da aynı template'ten başlatıldığına işaret ediyor.

### sarj_ev_panel_angular vs yonetim_panel_angular
- Core ve shared_admin yapısı yine özdeş
- yonetim_panel daha az feature kategorisine sahip (6 vs 28)
- yonetim_panel sadece 2 environment dosyasına sahip
- yonetim_panel Bootstrap'i explicit bağımlılık olarak ekliyor

## Sonuç

Bu proje, tek developer veya küçük ekibin Metronic template üzerine hızla geliştirdiği, kapsamlı bir EV şarj yönetim paneli. Mimari kararlar pragmatik: lazy loading doğru kullanılmış, environment yönetimi iyi, ancak çift chart kütüphanesi ve localStorage JWT gibi iyileştirilebilecek noktalar var. Feature modülünün 28 kategoriye büyümesi, projenin zamanla scope'unu genişlettiğini gösteriyor.
