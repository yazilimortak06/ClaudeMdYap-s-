# Angular Mimari Kuralları

> **DEĞİŞMEZ** — Bu dosyadaki kuralları değiştirmek için ayrı ve özel bir sohbet gereklidir. Normal geliştirme akışında bu dosya güncellenmez.

---

## 1. Klasör Yapısı Kuralları

Üç ana bölüm **zorunlu** ve **değiştirilemez**:

```
src/app/
├── core/              ← Singleton; uygulama genelinde bir kez yüklenir
├── shared_admin/      ← Paylaşılan UI; feature modüller import eder
└── <domain>/          ← Feature bölümü (panel: "restaurant/", qr: "pages/")
```

### core/ içinde ne bulunur?
- HTTP interceptor'lar (`interceptors/`)
- Guard'lar (`guards/`)
- Uygulama geneli model/interface'ler (`models/`)
- Singleton servisler (`services/`) — auth, signalr, storage
- `CoreModule` (NgModule tabanlı projeler için)

### shared_admin/ içinde ne bulunur?
- Layout bileşenleri (`layout/`) — MainLayout, Sidebar, Topbar
- Paylaşılan UI bileşenleri (`components/`) — ConfirmDialog, ImageUpload, DataTable
- Pipe'lar (`pipes/`)
- Directive'ler (`directives/`)
- `SharedAdminModule` — sık kullanılan modülleri export eder

### Yasak kullanımlar
- `core/` modülü feature katmanından **import edilemez** (sadece `app.module.ts` import eder).
- Feature bileşenleri başka feature klasörüne **doğrudan import yapamaz** (shared üzerinden geçmeli).
- `shared_admin/` içinde HTTP çağrısı yapan servis **olamaz**.

---

## 2. Routing Kuralları

### 2.1 Lazy Loading Zorunludur
Her feature modülü veya standalone component `loadChildren()` / `loadComponent()` ile yüklenir.
Doğrudan import yasaktır.

```typescript
// DOGRU — lazy loading
{
  path: 'menu',
  loadChildren: () =>
    import('./restaurant/menu-management/menu-management.module')
      .then(m => m.MenuManagementModule)
}

// YANLIS — direkt import (bundle boyutunu artırır, yasak)
{
  path: 'menu',
  component: MenuListComponent
}
```

### 2.2 HashLocationStrategy Zorunludur
```typescript
// NgModule tabanlı (panel)
RouterModule.forRoot(routes, { useHash: true })

// Standalone (QR)
provideRouter(routes, withHashLocation())
```

### 2.3 Üç Root Route Zorunludur

```typescript
const routes: Routes = [
  // 1. Auth (layout dışında, token gerektirmez)
  { path: 'auth', loadChildren: () => import(...).then(m => m.AuthModule) },

  // 2. Ana layout (korumalı, tüm feature sayfaları buranın altında)
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      // feature route'ları buraya
    ]
  },

  // 3. 404 / bilinmeyen route → güvenli sayfa
  { path: '**', redirectTo: 'dashboard' }
];
```

---

## 3. HTTP Interceptor — Tam Kod Örneği

### 3.1 AuthInterceptor

```typescript
// core/interceptors/auth.interceptor.ts

import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { StorageService } from '../services/storage.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private storageService: StorageService,
    private router: Router
  ) {}

  intercept(
    req: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {

    // /auth/ path'i olan isteklere token ekleme (login, refresh endpoint'leri)
    const isAuthEndpoint = req.url.includes('/auth/');
    const token = this.storageService.getToken();

    let authReq = req;

    if (!isAuthEndpoint && token) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Token geçersiz veya süresi dolmuş — oturumu kapat
          this.storageService.clearAll();
          this.router.navigate(['/auth/login']);
        }

        if (error.status === 403) {
          // Yetkisiz erişim — kullanıcıya bildir (toast interceptor'dan gelir)
          console.warn('Yetkisiz erişim denemesi:', req.url);
        }

        return throwError(() => error);
      })
    );
  }
}
```

### 3.2 SpinnerInterceptor

```typescript
// core/interceptors/spinner.interceptor.ts

import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable()
export class SpinnerInterceptor implements HttpInterceptor {
  // Eş zamanlı istek sayacı (birden fazla istek varsa spinner erken kapanmasın)
  private activeRequests = 0;

  constructor(private spinner: NgxSpinnerService) {}

  intercept(
    req: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {

    // X-Skip-Spinner header'ı varsa spinner atla
    if (req.headers.has('X-Skip-Spinner')) {
      // Header'ı backend'e göndermeden temizle
      const cleanReq = req.clone({ headers: req.headers.delete('X-Skip-Spinner') });
      return next.handle(cleanReq);
    }

    if (this.activeRequests === 0) {
      this.spinner.show();
    }
    this.activeRequests++;

    return next.handle(req).pipe(
      finalize(() => {
        this.activeRequests--;
        if (this.activeRequests === 0) {
          this.spinner.hide();
        }
      })
    );
  }
}
```

### 3.3 ErrorInterceptor

```typescript
// core/interceptors/error.interceptor.ts

import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let message = 'Bir hata oluştu.';

        if (error.status === 0) {
          // Network hatası — sunucuya ulaşılamıyor
          message = 'Bağlantı hatası. İnternet bağlantınızı kontrol edin.';
        } else if (error.status >= 500) {
          message = 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.';
        } else if (error.status === 422) {
          // Validation hatası — backend'den gelen mesajı kullan
          message = error.error?.message ?? 'Girdi doğrulama hatası.';
        }

        // Toast servisi burada çağrılır (örn: ToastrService.error(message))
        // Not: ToastrService inject edilebilir; şu an console.error ile gösteriliyor
        console.error(`[${error.status}] ${req.url}:`, message);

        return throwError(() => error);
      })
    );
  }
}
```

### 3.4 Interceptor Kayıt Sırası (app.module.ts)

```typescript
// app.module.ts

providers: [
  // Sıra önemli: Spinner → Auth → Error
  { provide: HTTP_INTERCEPTORS, useClass: SpinnerInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor,   multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor,  multi: true },
]
```

---

## 4. AuthGuard — Tam Kod Örneği

```typescript
// core/guards/auth.guard.ts

import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
  UrlTree
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    if (!this.authService.isAuthenticated()) {
      return this.router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    if (this.authService.isTokenExpired()) {
      this.authService.logout();
      return this.router.createUrlTree(['/auth/login']);
    }

    return true;
  }
}
```

---

## 5. Environment Dosyası Kuralları

### 5.1 Ortam Dosyaları (4 dosya zorunlu)

```
environments/
├── environment.ts           ← Development (varsayılan)
├── environment.staging.ts   ← Staging (CI/CD test ortamı)
├── environment.prod.ts      ← Production
└── environment.test.ts      ← Unit test (opsiyonel; mock URL'ler)
```

### 5.2 Zorunlu Key'ler

Her environment dosyasında şu 4 key **mutlaka** bulunmalıdır:

```typescript
export const environment = {
  production: boolean,       // ZORUNLU — boolean
  apiUrl: string,            // ZORUNLU — REST API base URL (/api dahil)
  fileApiUrl: string,        // ZORUNLU — dosya/resim sunucu base URL (/api yok)
  signalrUrl: string,        // ZORUNLU — SignalR hub base URL
  // Opsiyonel ek keyler (appVersion, featureFlags vb.) eklenebilir
};
```

### 5.3 Şablon

```typescript
// environments/environment.ts  (development)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  fileApiUrl: 'http://localhost:5000',
  signalrUrl: 'http://localhost:5000',
  appVersion: '1.0.0-dev'
};

// environments/environment.staging.ts
export const environment = {
  production: false,
  apiUrl: 'https://api-staging.restoranim.com/api',
  fileApiUrl: 'https://api-staging.restoranim.com',
  signalrUrl: 'https://api-staging.restoranim.com',
  appVersion: '1.0.0-staging'
};

// environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.restoranim.com/api',
  fileApiUrl: 'https://api.restoranim.com',
  signalrUrl: 'https://api.restoranim.com',
  appVersion: '1.0.0'
};
```

### 5.4 angular.json Yapılandırması
```json
"configurations": {
  "staging": {
    "fileReplacements": [
      {
        "replace": "src/environments/environment.ts",
        "with": "src/environments/environment.staging.ts"
      }
    ]
  },
  "production": {
    "fileReplacements": [
      {
        "replace": "src/environments/environment.ts",
        "with": "src/environments/environment.prod.ts"
      }
    ]
  }
}
```

---

## 6. Module Kuralları

### 6.1 CoreModule — Singleton Guard

```typescript
// core/core.module.ts

import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { SpinnerInterceptor } from './interceptors/spinner.interceptor';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: SpinnerInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor,   multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor,  multi: true },
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule?: CoreModule) {
    // CoreModule sadece AppModule tarafından import edilmeli
    // İkinci kez import edilirse hata fırlatır
    if (parentModule) {
      throw new Error(
        'CoreModule zaten yüklenmiş. Yalnızca AppModule import etmelidir.'
      );
    }
  }
}
```

### 6.2 SharedAdminModule — Export Consolidation

```typescript
// shared_admin/shared-admin.module.ts

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgxSpinnerModule } from 'ngx-spinner';

// Paylaşılan bileşenler
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { ImageUploadComponent } from './components/image-upload/image-upload.component';
import { SkeletonLoaderComponent } from './components/skeleton-loader/skeleton-loader.component';

// Paylaşılan pipe'lar
import { CurrencyTrPipe } from './pipes/currency-tr.pipe';
import { OrderStatusPipe } from './pipes/order-status.pipe';

const ANGULAR_MATERIAL = [
  MatButtonModule,
  MatInputModule,
  MatSelectModule,
  MatTableModule,
  MatDialogModule,
  MatSnackBarModule,
  MatProgressSpinnerModule,
];

const SHARED_COMPONENTS = [
  ConfirmDialogComponent,
  ImageUploadComponent,
  SkeletonLoaderComponent,
];

const SHARED_PIPES = [
  CurrencyTrPipe,
  OrderStatusPipe,
];

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    NgxSpinnerModule,
    ...ANGULAR_MATERIAL,
  ],
  declarations: [
    ...SHARED_COMPONENTS,
    ...SHARED_PIPES,
  ],
  exports: [
    // Angular modülleri
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    NgxSpinnerModule,
    // Material
    ...ANGULAR_MATERIAL,
    // Bileşenler ve pipe'lar
    ...SHARED_COMPONENTS,
    ...SHARED_PIPES,
  ]
})
export class SharedAdminModule {}
```

> Feature modüller sadece `SharedAdminModule` import eder; Angular Material modüllerini ayrıca import etmez.

---

## 7. Component Kuralları

### 7.1 ChangeDetectionStrategy.OnPush Tercihi

Tüm "akıllı" (smart/container) component'lerde `OnPush` tercih edilir.
"Saf" (presentational/dumb) component'lerde zorunludur.

```typescript
@Component({
  selector: 'app-urun-list',
  templateUrl: './urun-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush  // ZORUNLU — dumb componentlerde
})
```

**OnPush ile çalışmak için kurallar:**
- Observable'lar template'de `async` pipe ile kullanılır (manuel subscribe değil).
- Manuel subscribe yapıldıysa değişiklik sonrası `cdr.markForCheck()` çağrılır.
- Input'lar immutable (yeni referans) olarak değiştirilir.

### 7.2 OnDestroy — Unsubscribe Zorunludur

```typescript
export class MyComponent implements OnInit, OnDestroy {
  private readonly destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.someService.getData()
      .pipe(takeUntil(this.destroy$))   // takeUntil ile subscription yönetimi
      .subscribe(data => { /* ... */ });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

**Alternatif (Angular 16+):** `takeUntilDestroyed(this.destroyRef)` kullanılabilir.

### 7.3 Component Dosya Düzeni

```typescript
@Component({ ... })
export class MyComponent implements OnInit, OnDestroy {
  // 1. @Input() ve @Output() tanımları
  @Input() title: string = '';
  @Output() saved = new EventEmitter<void>();

  // 2. Public property'ler (template'de kullanılanlar)
  isLoading = false;
  items: Item[] = [];

  // 3. Private property'ler
  private readonly destroy$ = new Subject<void>();

  // 4. Constructor (sadece DI, iş mantığı yok)
  constructor(
    private itemService: ItemService,
    private router: Router
  ) {}

  // 5. Lifecycle hook'lar (ngOnInit → ngOnChanges → ngOnDestroy sırasıyla)
  ngOnInit(): void { this.loadData(); }
  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }

  // 6. Public metodlar (template'den çağrılanlar)
  onSave(): void { /* ... */ }
  trackById(_: number, item: Item): number { return item.id; }

  // 7. Private metodlar (iç mantık)
  private loadData(): void { /* ... */ }
}
```

---

## 8. Service Kuralları

### 8.1 `providedIn` Seçimi

| Durum | `providedIn` | Gerekçe |
|---|---|---|
| Uygulama geneli (auth, signalr, storage) | `'root'` | Singleton, her yerden erişilir |
| Feature'a özgü servis (menu.service, order.service) | `undefined` — modülde provide edilir | Modül unload olunca servis de yok edilir |
| Guard | `'root'` | Route sistemi tarafından inject edilir |
| Interceptor | Modülde `HTTP_INTERCEPTORS` olarak provide | `@Injectable()` (providedIn yok) |

```typescript
// Feature servisi — modülde provide edilir
@Injectable()   // providedIn belirtilmez
export class MenuService { ... }

// menu-management.module.ts içinde:
providers: [MenuService, MenuCategoryService]
```

### 8.2 Service Method İsimlendirmesi

```
getAll(filter?)    → Listeleme (sayfalı veya tüm)
getById(id)        → Tekil kayıt
create(dto)        → Yeni kayıt oluşturma
update(id, dto)    → Güncelleme
delete(id)         → Silme
updateStatus(dto)  → Sadece durum güncelleme
toggle(id, flag)   → Boolean alan toggle (isActive, isAvailable)
```

---

## 9. TypeScript Kuralları

### 9.1 `tsconfig.json` Zorunlu Ayarlar

```json
{
  "compilerOptions": {
    "strict": true,              // Tüm strict flag'ler aktif
    "noImplicitAny": true,       // any yasak
    "strictNullChecks": true,    // null/undefined güvenliği
    "noUnusedLocals": true,      // Kullanılmayan değişken → hata
    "noUnusedParameters": true   // Kullanılmayan parametre → hata
  }
}
```

### 9.2 `any` Kullanım Yasağı

```typescript
// YANLIS
function processData(data: any): any { ... }

// DOGRU
function processData(data: unknown): MenuItem[] {
  return data as MenuItem[];  // Assertion gerekirse açıkça yapılır
}
```

### 9.3 Interface Tercihi (type alias değil)

```typescript
// TERCIH EDILEN — interface (extend edilebilir, merge edilebilir)
export interface MenuItem {
  id: number;
  name: string;
}

// KACINILANLAR — type alias (gerekmedikçe)
type MenuItem = { id: number; name: string; };
```

`type` kullanım alanları: Union type, literal type (`OrderStatus`), utility type'lar.

### 9.4 Optional Chaining ve Nullish Coalescing

```typescript
// Tercih edilen
const price = item?.discountedPrice ?? item?.price ?? 0;
const logoUrl = restaurant?.logoUrl ?? 'assets/images/default-logo.png';
```

---

## 10. Paket Tercihleri

| Amaç | Paket | Notlar |
|---|---|---|
| UI bileşenleri | `@angular/material` | Form, tablo, dialog için |
| Grafikler | `ng-apexcharts` | ApexCharts Angular wrapper |
| Spinner | `ngx-spinner` | Interceptor ile global kullanım |
| Dosya yükleme | `ngx-dropzone` | Sürükle-bırak ve önizleme |
| Real-time | `@microsoft/signalr` | Panel'de zorunlu; QR'da opsiyonel |
| Notification/Toast | `ngx-toastr` | Hata, başarı bildirimleri |
| QR üretimi | `qrcode` veya `angularx-qrcode` | QR Management modülü için |
| Tarih işlemleri | `date-fns` | Moment.js yerine; tree-shaking uyumlu |

**Yasak paketler:** `moment.js` (ağır bundle), `jQuery` (Angular ile çakışma riski).

---

## 11. Kod Kalitesi ve Linting

```json
// .eslintrc.json — zorunlu kurallar
{
  "rules": {
    "@typescript-eslint/no-explicit-any": "error",
    "@typescript-eslint/explicit-function-return-type": "warn",
    "@angular-eslint/prefer-on-push-change-detection": "warn",
    "@angular-eslint/no-empty-lifecycle-method": "error",
    "no-console": ["warn", { "allow": ["error", "warn"] }]
  }
}
```

- `ng lint` CI pipeline'da çalıştırılır; lint hatası → PR reddedilir.
- `prettier` ile kod formatlaması otomatikleştirilir.
- Commit öncesi `husky` + `lint-staged` çalışır.
