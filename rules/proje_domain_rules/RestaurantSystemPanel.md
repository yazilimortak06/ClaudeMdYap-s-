# RestaurantSystemPanel — Domain Kuralları

> **Kapsam:** Angular 14+ tabanlı restoran yönetim paneli (admin/personel kullanır).
> Bu belge modül sorumluluklarını, sayfa listelerini, model tanımlarını, servis kurallarını ve gerçek kod örneklerini kapsar.

---

## 1. Modül Listesi ve Sorumlulukları

| Modül | Sorumluluk |
|---|---|
| **AuthModule** | Giriş, şifre sıfırlama, token yönetimi |
| **DashboardModule** | Özet istatistikler, canlı sipariş akışı, günlük gelir kartları |
| **MenuManagementModule** | Kategori ve ürün CRUD, görsel yükleme, sıralama |
| **OrderManagementModule** | Aktif siparişler, sipariş detayı, durum güncelleme, mutfak ekranı |
| **TableManagementModule** | Masa tanımlama, masa planı görünümü, masa durumu |
| **QrManagementModule** | QR kod üretme, QR bağlama (masa ↔ QR), indirme/yazdırma |
| **UserManagementModule** | Kullanıcı CRUD, rol atama (Admin, Manager, Waiter, Kitchen, Cashier) |
| **ReportsModule** | Satış raporları, ürün bazlı analiz, ApexCharts grafikler |
| **SettingsModule** | Restoran profili, çalışma saatleri, ödeme ayarları, bildirim ayarları |

---

## 2. Sayfa Listesi (Route → Component Eşlemesi)

### 2.1 AuthModule
```
/auth/login            → LoginComponent
/auth/forgot-password  → ForgotPasswordComponent
/auth/reset-password   → ResetPasswordComponent
```

### 2.2 DashboardModule
```
/dashboard             → DashboardHomeComponent
```

### 2.3 MenuManagementModule
```
/menu/categories           → KategoriListComponent
/menu/categories/new       → KategoriDetailComponent  (yeni oluşturma)
/menu/categories/:id       → KategoriDetailComponent  (düzenleme)
/menu/items                → UrunListComponent
/menu/items/new            → UrunDetailComponent      (yeni oluşturma)
/menu/items/:id            → UrunDetailComponent      (düzenleme)
```

### 2.4 OrderManagementModule
```
/orders                → OrderListComponent          (aktif + geçmiş filtreli)
/orders/active         → OrderActiveComponent        (canlı görünüm, SignalR)
/orders/:id            → OrderDetailComponent
/orders/kitchen        → KitchenDisplayComponent     (mutfak ekranı, SignalR)
```

### 2.5 TableManagementModule
```
/tables                → TableListComponent
/tables/new            → TableDetailComponent
/tables/:id            → TableDetailComponent
/tables/floor-plan     → FloorPlanComponent
```

### 2.6 QrManagementModule
```
/qr                    → QrListComponent
/qr/generate           → QrGenerateComponent
/qr/:id                → QrDetailComponent
```

### 2.7 UserManagementModule
```
/users                 → UserListComponent
/users/new             → UserDetailComponent
/users/:id             → UserDetailComponent
```

### 2.8 ReportsModule
```
/reports/sales         → SalesReportComponent
/reports/products      → ProductReportComponent
/reports/daily         → DailyReportComponent
```

### 2.9 SettingsModule
```
/settings/profile      → RestaurantProfileComponent
/settings/hours        → WorkingHoursComponent
/settings/payment      → PaymentSettingsComponent
/settings/notifications → NotificationSettingsComponent
```

---

## 3. Angular Klasör Yapısı

```
src/
├── app/
│   ├── core/                          # Singleton servisler, interceptor, guard, model
│   │   ├── interceptors/
│   │   │   ├── auth.interceptor.ts    # Bearer token ekler
│   │   │   ├── spinner.interceptor.ts # HTTP isteklerinde spinner gösterir
│   │   │   └── error.interceptor.ts   # 401/403/500 yönetimi
│   │   ├── guards/
│   │   │   ├── auth.guard.ts          # Giriş kontrolü
│   │   │   └── role.guard.ts          # Rol bazlı erişim
│   │   ├── models/
│   │   │   ├── restaurant.model.ts
│   │   │   ├── menu-category.model.ts
│   │   │   ├── menu-item.model.ts
│   │   │   ├── order.model.ts
│   │   │   ├── table.model.ts
│   │   │   └── user.model.ts
│   │   ├── services/
│   │   │   ├── auth.service.ts
│   │   │   ├── signalr.service.ts     # SignalR hub bağlantısı
│   │   │   └── storage.service.ts
│   │   └── core.module.ts
│   │
│   ├── shared_admin/                  # Paylaşılan UI bileşenleri ve layout
│   │   ├── layout/
│   │   │   ├── main-layout/
│   │   │   │   ├── main-layout.component.ts
│   │   │   │   ├── main-layout.component.html
│   │   │   │   └── main-layout.component.scss
│   │   │   ├── sidebar/
│   │   │   └── topbar/
│   │   ├── components/
│   │   │   ├── confirm-dialog/
│   │   │   ├── image-upload/          # ngx-dropzone kullanan wrapper
│   │   │   ├── data-table/
│   │   │   └── skeleton-loader/
│   │   ├── pipes/
│   │   │   ├── currency-tr.pipe.ts
│   │   │   └── order-status.pipe.ts
│   │   ├── directives/
│   │   └── shared-admin.module.ts
│   │
│   ├── restaurant/                    # Feature modülleri (lazy loaded)
│   │   ├── auth/
│   │   │   ├── auth.module.ts
│   │   │   ├── auth-routing.module.ts
│   │   │   └── pages/
│   │   │       ├── login/
│   │   │       ├── forgot-password/
│   │   │       └── reset-password/
│   │   ├── dashboard/
│   │   │   ├── dashboard.module.ts
│   │   │   ├── dashboard-routing.module.ts
│   │   │   └── pages/
│   │   │       └── dashboard-home/
│   │   ├── menu-management/
│   │   │   ├── menu-management.module.ts
│   │   │   ├── menu-management-routing.module.ts
│   │   │   ├── services/
│   │   │   │   ├── menu.service.ts
│   │   │   │   └── menu-category.service.ts
│   │   │   └── pages/
│   │   │       ├── kategori-list/
│   │   │       ├── kategori-detail/
│   │   │       ├── urun-list/
│   │   │       └── urun-detail/
│   │   ├── order-management/
│   │   │   ├── order-management.module.ts
│   │   │   ├── order-management-routing.module.ts
│   │   │   ├── services/
│   │   │   │   └── order.service.ts
│   │   │   └── pages/
│   │   │       ├── order-list/
│   │   │       ├── order-active/
│   │   │       ├── order-detail/
│   │   │       └── kitchen-display/
│   │   ├── table-management/
│   │   ├── qr-management/
│   │   ├── user-management/
│   │   ├── reports/
│   │   └── settings/
│   │
│   ├── app-routing.module.ts
│   ├── app.module.ts
│   └── app.component.ts
│
├── assets/
│   ├── images/
│   ├── icons/
│   ├── sounds/                        # new-order.mp3 gibi bildirim sesleri
│   └── i18n/                          # Çoklu dil (tr.json, en.json)
│
└── environments/
    ├── environment.ts                 # Development
    ├── environment.staging.ts         # Staging
    └── environment.prod.ts            # Production
```

---

## 4. Model Tanımları (TypeScript Interfaces)

```typescript
// core/models/restaurant.model.ts

export interface Restaurant {
  id: number;
  name: string;
  slug: string;                  // URL dostu isim
  logoUrl: string | null;
  bannerUrl: string | null;
  phone: string;
  email: string;
  address: string;
  currency: string;              // 'TRY', 'USD', 'EUR'
  isActive: boolean;
  createdAt: string;             // ISO 8601
  updatedAt: string;
}

export interface RestaurantSettings {
  restaurantId: number;
  taxRate: number;               // KDV oranı (örn: 8, 18)
  serviceCharge: number;         // Servis ücreti yüzdesi
  minOrderAmount: number;
  orderNotificationEnabled: boolean;
}
```

```typescript
// core/models/menu-category.model.ts

export interface MenuCategory {
  id: number;
  restaurantId: number;
  name: string;
  nameEn: string | null;         // İngilizce çeviri
  description: string | null;
  imageUrl: string | null;
  sortOrder: number;             // Görüntüleme sırası
  isActive: boolean;
  itemCount?: number;            // Opsiyonel, liste görünümünde API'den gelir
  createdAt: string;
}

export interface MenuCategoryCreateDto {
  name: string;
  nameEn?: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
}

export interface MenuCategoryUpdateDto extends MenuCategoryCreateDto {
  id: number;
}
```

```typescript
// core/models/menu-item.model.ts

export interface MenuItem {
  id: number;
  restaurantId: number;
  categoryId: number;
  categoryName?: string;          // Join ile gelen, sadece listede bulunur
  name: string;
  nameEn: string | null;
  description: string | null;
  descriptionEn: string | null;
  imageUrl: string | null;
  price: number;
  discountedPrice: number | null; // null ise indirim yok
  preparationTime: number;        // Dakika cinsinden tahmini süre
  calories: number | null;
  isAvailable: boolean;           // Anlık stok/erişilebilirlik durumu
  isActive: boolean;
  tags: string[];                 // ['vegan', 'gluten-free', 'spicy']
  sortOrder: number;
  createdAt: string;
}

export interface MenuItemCreateDto {
  categoryId: number;
  name: string;
  nameEn?: string;
  description?: string;
  price: number;
  discountedPrice?: number;
  preparationTime?: number;
  calories?: number;
  isAvailable?: boolean;
  tags?: string[];
  sortOrder?: number;
}
```

```typescript
// core/models/order.model.ts

export type OrderStatus =
  | 'Pending'      // Bekliyor (müşteri verdi, henüz onaylanmadı)
  | 'Confirmed'    // Onaylandı
  | 'Preparing'    // Mutfakta hazırlanıyor
  | 'Ready'        // Hazır, servise çıkmayı bekliyor
  | 'Delivered'    // Masaya teslim edildi
  | 'Cancelled'    // İptal edildi
  | 'Completed';   // Ödeme alındı, tamamlandı

export interface OrderItem {
  id: number;
  menuItemId: number;
  menuItemName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  note: string | null;           // Müşteri notu (örn: "az baharatlı")
}

export interface Order {
  id: number;
  restaurantId: number;
  tableId: number;
  tableNumber: string;
  orderCode: string;             // Kısa insan-okunabilir kod (örn: "ORD-1042")
  status: OrderStatus;
  items: OrderItem[];
  subtotal: number;
  taxAmount: number;
  serviceCharge: number;
  totalAmount: number;
  customerNote: string | null;
  createdAt: string;
  updatedAt: string;
  estimatedReadyAt: string | null;
}

export interface OrderStatusUpdateDto {
  orderId: number;
  status: OrderStatus;
}
```

```typescript
// core/models/table.model.ts

export type TableStatus = 'Available' | 'Occupied' | 'Reserved' | 'OutOfService';

export interface Table {
  id: number;
  restaurantId: number;
  number: string;             // "1", "2", "VIP-1"
  name: string | null;        // Özel isim (örn: "Balkon Masası")
  capacity: number;
  status: TableStatus;
  qrCode: string | null;      // QR kod değeri (UUID veya kısa kod)
  qrImageUrl: string | null;  // Üretilmiş QR görseli sunucu URL'si
  floorSection: string | null; // "Zemin Kat", "Balkon", "Bahçe"
  isActive: boolean;
}
```

```typescript
// core/models/user.model.ts

export type UserRole = 'Admin' | 'Manager' | 'Waiter' | 'Kitchen' | 'Cashier';

export interface User {
  id: number;
  restaurantId: number;
  firstName: string;
  lastName: string;
  fullName: string;            // Computed: firstName + ' ' + lastName
  email: string;
  phone: string | null;
  role: UserRole;
  avatarUrl: string | null;
  isActive: boolean;
  lastLoginAt: string | null;
  createdAt: string;
}

export interface AuthUser {
  id: number;
  email: string;
  fullName: string;
  role: UserRole;
  restaurantId: number;
  restaurantName: string;
  token: string;
  refreshToken: string;
  tokenExpiry: string;         // ISO 8601 — bu tarihten önce token geçerli
}
```

---

## 5. Service Naming Convention

```
Modül servisleri → src/app/restaurant/<modul>/services/ altında yaşar
Core servisler   → src/app/core/services/ altında yaşar

Dosya adı   : <kaynak>.service.ts        (kebab-case)
Sınıf adı   : <Kaynak>Service            (PascalCase)
Method adları: getAll | getById | create | update | delete | updateStatus

Örnekler:
  menu.service.ts           → MenuService           (ürün CRUD)
  menu-category.service.ts  → MenuCategoryService   (kategori CRUD)
  order.service.ts          → OrderService          (sipariş işlemleri)
  table.service.ts          → TableService          (masa işlemleri)
  qr.service.ts             → QrService             (QR üretim ve yönetim)
  user.service.ts           → UserService           (kullanıcı CRUD)
  report.service.ts         → ReportService         (rapor endpoint'leri)
  auth.service.ts           → AuthService           (core — login/logout/token)
  signalr.service.ts        → SignalrService        (core — hub bağlantısı)
  storage.service.ts        → StorageService        (core — localStorage wrapper)
```

---

## 6. HTTP Interceptor Kuralları

### 6.1 AuthInterceptor (JWT ekleme + 401 yönetimi)
- Her HTTP isteğine `Authorization: Bearer <token>` header'ı eklenir.
- `/auth/` path'i içeren isteklere token eklenmez (login/reset endpoint'leri).
- 401 yanıtı geldiğinde: token silinir, kullanıcı `/auth/login` adresine yönlendirilir.
- 403 yanıtı geldiğinde: toast mesajı gösterilir ("Bu işlem için yetkiniz yok").

### 6.2 SpinnerInterceptor
- Her istek başladığında `NgxSpinnerService.show()` çağrılır.
- İstek tamamlandığında (başarı veya hata) `NgxSpinnerService.hide()` çağrılır.
- Spinner göstermesi istenmeyen istekler için isteğe `X-Skip-Spinner: true` header'ı eklenir.

### 6.3 ErrorInterceptor
- 500 ve üstü HTTP hatalarında merkezi toast bildirimi gösterilir.
- Network error (0 status) durumunda kullanıcıya "Bağlantı hatası" mesajı gösterilir.
- Hata detayları `console.error` ile loglanır (production'da kapatılır).

---

## 7. Guard Yapısı

### 7.1 AuthGuard
- `canActivate` ile korunan tüm layout route'larına uygulanır.
- `AuthService.isAuthenticated()` → false ise `/auth/login?returnUrl=...` yönlendir.
- `AuthService.isTokenExpired()` → true ise logout yapıp login'e gönder.

### 7.2 RoleGuard
- `canActivate` ile çalışır, route `data['roles']` alanını okur.
- Kullanıcı rolü izin verilen roller içinde değilse `/dashboard` yönlendir.
- Kullanıcı yönetimine sadece `Admin` erişebilir.
- Raporlara `Admin` ve `Manager` erişebilir.
- `data['roles']` boşsa veya yoksa erişime izin verilir.

---

## 8. Routing Yapısı

```typescript
// app-routing.module.ts

const routes: Routes = [
  // Auth modülü — layout dışında
  {
    path: 'auth',
    loadChildren: () =>
      import('./restaurant/auth/auth.module').then(m => m.AuthModule)
  },
  // Ana layout — tüm korumalı sayfalar buranın altında
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./restaurant/dashboard/dashboard.module')
            .then(m => m.DashboardModule)
      },
      {
        path: 'menu',
        loadChildren: () =>
          import('./restaurant/menu-management/menu-management.module')
            .then(m => m.MenuManagementModule)
      },
      {
        path: 'orders',
        loadChildren: () =>
          import('./restaurant/order-management/order-management.module')
            .then(m => m.OrderManagementModule)
      },
      {
        path: 'tables',
        loadChildren: () =>
          import('./restaurant/table-management/table-management.module')
            .then(m => m.TableManagementModule)
      },
      {
        path: 'qr',
        loadChildren: () =>
          import('./restaurant/qr-management/qr-management.module')
            .then(m => m.QrManagementModule)
      },
      {
        path: 'users',
        canActivate: [RoleGuard],
        data: { roles: ['Admin'] },
        loadChildren: () =>
          import('./restaurant/user-management/user-management.module')
            .then(m => m.UserManagementModule)
      },
      {
        path: 'reports',
        canActivate: [RoleGuard],
        data: { roles: ['Admin', 'Manager'] },
        loadChildren: () =>
          import('./restaurant/reports/reports.module')
            .then(m => m.ReportsModule)
      },
      {
        path: 'settings',
        loadChildren: () =>
          import('./restaurant/settings/settings.module')
            .then(m => m.SettingsModule)
      }
    ]
  },
  // Bilinmeyen route → dashboard'a yönlendir
  { path: '**', redirectTo: 'dashboard' }
];

// app.module.ts içinde:
// RouterModule.forRoot(routes, { useHash: true })   → HashStrategy
```

---

## 9. Real-Time Kurallar (SignalR)

### 9.1 Hub Bağlantısı
- Hub URL: `environment.signalrUrl + '/hubs/order'`
- Bağlantı açılırken JWT token HTTP header olarak gönderilir.
- `SignalrService` core'da singleton olarak yaşar (`providedIn: 'root'`).
- `withAutomaticReconnect([0, 2000, 5000, 10000])` — bağlantı kopunca otomatik yeniden bağlanır.

### 9.2 Server → Client Event Listesi

| Event Adı | Payload Tipi | Açıklama |
|---|---|---|
| `OrderCreated` | `Order` | Yeni sipariş oluşturuldu |
| `OrderStatusChanged` | `{ orderId: number; status: OrderStatus }` | Sipariş durumu değişti |
| `OrderItemAdded` | `{ orderId: number; item: OrderItem }` | Siparişe yeni ürün eklendi |
| `TableStatusChanged` | `{ tableId: number; status: TableStatus }` | Masa durumu değişti |

### 9.3 Hangi Componentler Hangi Event'leri Dinliyor?

| Component | Dinlediği Event'ler |
|---|---|
| `DashboardHomeComponent` | `OrderCreated`, `OrderStatusChanged` |
| `OrderActiveComponent` | `OrderCreated`, `OrderStatusChanged`, `OrderItemAdded` |
| `KitchenDisplayComponent` | `OrderCreated`, `OrderStatusChanged` |
| `TableListComponent` | `TableStatusChanged` |
| `TopbarComponent` (bildirim zili) | `OrderCreated` |

### 9.4 Bağlantı Yaşam Döngüsü
- `MainLayoutComponent.ngOnInit()` → `SignalrService.startConnection()` çağrılır.
- `AuthService.logout()` → `SignalrService.stopConnection()` çağrılır.
- Her component `ngOnDestroy`'da sadece kendi subscription'ını iptal eder; hub bağlantısını kapatmaz.

---

## 10. Environment Dosyası Yapısı

```typescript
// environments/environment.ts  (development — varsayılan)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  fileApiUrl: 'http://localhost:5000',      // Görsel URL'leri için base (model.imageUrl önüne eklenir)
  signalrUrl: 'http://localhost:5000',      // SignalR hub base URL
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

**Zorunlu key'ler:** `production`, `apiUrl`, `fileApiUrl`, `signalrUrl`
Yeni ortam eklendiğinde bu 4 key mutlaka bulunmalıdır.

---

## 11. Gerçek Angular Kod Örnekleri

### 11.1 menu.service.ts — Tam Örnek

```typescript
// restaurant/menu-management/services/menu.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MenuItem, MenuItemCreateDto } from '../../../core/models/menu-item.model';

/** API'den dönen sayfalı liste yapısı */
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

/** Ürün listeleme filtre parametreleri */
export interface MenuItemFilter {
  categoryId?: number;
  isActive?: boolean;
  isAvailable?: boolean;
  search?: string;
  pageNumber?: number;
  pageSize?: number;
}

@Injectable()
// MenuManagementModule içinde providers dizisine eklenir (root değil)
// Sebep: modül lazy load edildiğinde servis de birlikte yüklenir ve yok edilir
export class MenuService {
  private readonly apiUrl = `${environment.apiUrl}/menu-items`;

  constructor(private http: HttpClient) {}

  /**
   * Ürün listesini filtreli ve sayfalı getirir.
   * Kategori filtresi, arama ve sayfalama desteklenir.
   */
  getAll(filter?: MenuItemFilter): Observable<PagedResult<MenuItem>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.categoryId != null) {
        params = params.set('categoryId', String(filter.categoryId));
      }
      if (filter.isActive != null) {
        params = params.set('isActive', String(filter.isActive));
      }
      if (filter.isAvailable != null) {
        params = params.set('isAvailable', String(filter.isAvailable));
      }
      if (filter.search?.trim()) {
        params = params.set('search', filter.search.trim());
      }
      params = params.set('pageNumber', String(filter.pageNumber ?? 1));
      params = params.set('pageSize', String(filter.pageSize ?? 20));
    }

    return this.http.get<PagedResult<MenuItem>>(this.apiUrl, { params });
  }

  /**
   * Tek ürün getirir (düzenleme formunu doldurmak için).
   */
  getById(id: number): Observable<MenuItem> {
    return this.http.get<MenuItem>(`${this.apiUrl}/${id}`);
  }

  /**
   * Yeni ürün oluşturur.
   * imageFile parametresi varsa FormData ile gönderilir, yoksa JSON gider.
   */
  create(dto: MenuItemCreateDto, imageFile?: File): Observable<MenuItem> {
    if (imageFile) {
      return this.http.post<MenuItem>(this.apiUrl, this.buildFormData(dto, imageFile));
    }
    return this.http.post<MenuItem>(this.apiUrl, dto);
  }

  /**
   * Mevcut ürünü günceller.
   */
  update(id: number, dto: Partial<MenuItemCreateDto>, imageFile?: File): Observable<MenuItem> {
    if (imageFile) {
      return this.http.put<MenuItem>(
        `${this.apiUrl}/${id}`,
        this.buildFormData(dto, imageFile)
      );
    }
    return this.http.put<MenuItem>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Ürünü siler (soft delete — backend isActive=false yapar).
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Stok/erişilebilirlik durumunu hızlıca değiştirir.
   * Liste sayfasındaki toggle butonu bunu kullanır.
   */
  toggleAvailability(id: number, isAvailable: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/availability`, { isAvailable });
  }

  /**
   * Sıralama güncelleme: drag-drop sonrası yeni sırayı kaydeder.
   */
  updateSortOrder(items: Array<{ id: number; sortOrder: number }>): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/sort-order`, { items });
  }

  /** DTO nesnesini ve dosyayı FormData'ya çevirir */
  private buildFormData(dto: object, imageFile?: File): FormData {
    const formData = new FormData();

    Object.entries(dto).forEach(([key, value]) => {
      if (value == null) return;
      if (Array.isArray(value)) {
        // tags gibi array alanlar: her eleman ayrı append edilir
        value.forEach(v => formData.append(key, String(v)));
      } else {
        formData.append(key, String(value));
      }
    });

    if (imageFile) {
      formData.append('image', imageFile, imageFile.name);
    }

    return formData;
  }
}
```

---

### 11.2 OrderActiveComponent — SignalR Entegrasyonu

```typescript
// restaurant/order-management/pages/order-active/order-active.component.ts

import {
  Component,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SignalrService } from '../../../../core/services/signalr.service';
import { OrderService } from '../../services/order.service';
import { Order, OrderStatus } from '../../../../core/models/order.model';

@Component({
  selector: 'app-order-active',
  templateUrl: './order-active.component.html',
  styleUrls: ['./order-active.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush  // Performans: sadece markForCheck çağrıldığında render
})
export class OrderActiveComponent implements OnInit, OnDestroy {
  /** Duruma göre ayrılmış sipariş listeleri */
  pendingOrders: Order[] = [];
  preparingOrders: Order[] = [];
  readyOrders: Order[] = [];

  isLoading = true;

  /** RxJS takeUntil unsubscribe paterni */
  private readonly destroy$ = new Subject<void>();

  constructor(
    private signalrService: SignalrService,
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadActiveOrders();
    this.subscribeToSignalR();
  }

  ngOnDestroy(): void {
    // Tüm subscription'ları tek seferde iptal et
    this.destroy$.next();
    this.destroy$.complete();
    // Not: SignalR hub bağlantısını burada kapatmıyoruz,
    //      çünkü diğer componentler hâlâ kullanıyor olabilir.
  }

  /** HTTP ile mevcut aktif siparişleri ilk kez yükler */
  private loadActiveOrders(): void {
    this.orderService
      .getActiveOrders()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: orders => {
          this.categorizeOrders(orders);
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  /** SignalR event'lerini dinlemeye başlar */
  private subscribeToSignalR(): void {
    // Yeni sipariş: listeye ekle ve sesli bildir
    this.signalrService
      .on<Order>('OrderCreated')
      .pipe(takeUntil(this.destroy$))
      .subscribe(order => {
        this.pendingOrders = [order, ...this.pendingOrders];
        this.playNotificationSound();
        this.cdr.markForCheck();
      });

    // Durum değişikliği: ilgili siparişi güncelle
    this.signalrService
      .on<{ orderId: number; status: OrderStatus }>('OrderStatusChanged')
      .pipe(takeUntil(this.destroy$))
      .subscribe(({ orderId, status }) => {
        this.updateOrderStatus(orderId, status);
        this.cdr.markForCheck();
      });

    // Siparişe yeni ürün eklendi
    this.signalrService
      .on<{ orderId: number; item: import('../../../../core/models/order.model').OrderItem }>('OrderItemAdded')
      .pipe(takeUntil(this.destroy$))
      .subscribe(({ orderId, item }) => {
        const all = [...this.pendingOrders, ...this.preparingOrders, ...this.readyOrders];
        const order = all.find(o => o.id === orderId);
        if (order) {
          order.items = [...order.items, item];
          this.cdr.markForCheck();
        }
      });
  }

  /** Siparişleri durumuna göre ilgili listelere atar */
  private categorizeOrders(orders: Order[]): void {
    this.pendingOrders   = orders.filter(o => o.status === 'Pending');
    this.preparingOrders = orders.filter(o => o.status === 'Preparing' || o.status === 'Confirmed');
    this.readyOrders     = orders.filter(o => o.status === 'Ready');
  }

  /** Tüm listelerdeki bir siparişin durumunu günceller */
  private updateOrderStatus(orderId: number, status: OrderStatus): void {
    const all = [...this.pendingOrders, ...this.preparingOrders, ...this.readyOrders];
    const order = all.find(o => o.id === orderId);
    if (!order) return;

    order.status = status;

    // 'Delivered' veya 'Cancelled' ise aktif listelerden çıkar
    const activeStatuses: OrderStatus[] = ['Pending', 'Confirmed', 'Preparing', 'Ready'];
    const filtered = all.filter(o => activeStatuses.includes(o.status));
    this.categorizeOrders(filtered);
  }

  /** Garson/mutfak butonundan durum günceller; gerçek yansıma SignalR'dan gelir */
  onStatusChange(orderId: number, newStatus: OrderStatus): void {
    this.orderService
      .updateStatus({ orderId, status: newStatus })
      .pipe(takeUntil(this.destroy$))
      .subscribe();
    // Optimistic update yapılmıyor — SignalR 'OrderStatusChanged' event'i tetikler
  }

  /** Yeni sipariş sesli bildirimi */
  private playNotificationSound(): void {
    const audio = new Audio('assets/sounds/new-order.mp3');
    audio.play().catch(() => {
      // Tarayıcı autoplay politikası sesi engelleyebilir, sessizce geç
    });
  }

  /** ngFor performans optimizasyonu */
  trackByOrderId(_index: number, order: Order): number {
    return order.id;
  }
}
```

---

### 11.3 AuthGuard

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
  providedIn: 'root'  // Guard'lar her zaman root'ta sağlanır
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

    // 1. Token var mı?
    if (!this.authService.isAuthenticated()) {
      // returnUrl: giriş sonrası kullanıcıyı istediği sayfaya yönlendirmek için
      return this.router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    // 2. Token süresi dolmuş mu?
    if (this.authService.isTokenExpired()) {
      this.authService.logout();  // Storage temizlenir
      return this.router.createUrlTree(['/auth/login']);
    }

    return true;
  }
}
```

---

### 11.4 RoleGuard

```typescript
// core/guards/role.guard.ts

import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  Router,
  UrlTree
} from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    // Route data'dan izin verilen rol listesini oku
    const allowedRoles: UserRole[] = route.data['roles'] ?? [];

    // Rol kısıtlaması tanımlanmamışsa geçiş serbest
    if (allowedRoles.length === 0) {
      return true;
    }

    const currentUser = this.authService.getCurrentUser();

    if (!currentUser) {
      return this.router.createUrlTree(['/auth/login']);
    }

    if (allowedRoles.includes(currentUser.role)) {
      return true;
    }

    // Rol yetersiz → dashboard'a yönlendir (401 sayfası yerine sessiz yönlendirme)
    return this.router.createUrlTree(['/dashboard']);
  }
}
```

---

## 12. Genel Kurallar

1. **Form validasyonu** her zaman `ReactiveFormsModule` ile yapılır; template-driven form kullanılmaz.
2. **Görsel URL'leri** backend'den göreli path gelir: `environment.fileApiUrl + model.imageUrl` ile tam URL oluşturulur.
3. **Para birimi** template'de `currency` pipe ile formatlanır: `{{ item.price | currency:'TRY':'symbol':'1.2-2':'tr' }}`.
4. **HTTP hataları** interceptor yakaladığı için component'lerde `catchError` / try-catch yazılmaz.
5. **Silme işlemleri** her zaman `ConfirmDialogComponent` onayı alındıktan sonra tetiklenir.
6. **Yeni modül/sayfa eklendiğinde** önce bu dosyaya eklenir, ardından kod yazılır.
