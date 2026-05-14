# RestaurantSystemQr — Domain Kuralları

> **Kapsam:** Angular 14+ tabanlı QR menü uygulaması. Müşteri, masadaki QR kodu okutarak erişir.
> Sadece okuma değil, **sipariş verme** özelliği de bulunur. Garson çağırma yoktur.

---

## 1. Uygulama Kapsamı

### Yapılanlar (in-scope)
- Restoran menüsünü kategoriler ve ürünler halinde görüntülemek
- Ürün detayı ve görseli görüntülemek
- Sepete ürün eklemek, çıkarmak, miktarını değiştirmek
- Sipariş oluşturmak (masaya sipariş vermek)
- Verilen siparişin durumunu takip etmek

### Yapılmayanlar (out-of-scope)
- Kullanıcı kaydı veya girişi (anonim kullanım)
- Ödeme alma (ödeme kasada yapılır)
- Garson çağırma
- Masa rezervasyonu
- Admin/yönetim işlevleri

---

## 2. Sayfa Listesi

```
/qr/:tableId               → QrLandingComponent      (QR okutunca açılan ilk sayfa)
/menu                      → MenuHomeComponent        (kategori listesi ana sayfa)
/menu/category/:id         → CategoryDetailComponent  (kategori ürün listesi)
/menu/item/:id             → ItemDetailComponent      (ürün detay + sepete ekle)
/cart                      → CartComponent            (sepet özeti)
/order/confirm             → OrderConfirmComponent    (sipariş onaylama)
/order/status/:orderCode   → OrderStatusComponent     (sipariş takip ekranı)
```

---

## 3. URL Yapısı ve Routing

QR kod içeriği: `https://qr.restoranim.com/#/qr/T-14`  (tableId = "T-14")

Kullanıcı kodu okutunca:
1. `QrLandingComponent` açılır, `tableId` parametresini okur.
2. `TableService.getTableInfo(tableId)` çağrılır → masa doğrulanır.
3. `tableId` `localStorage`'a kaydedilir (oturum boyunca geçerli).
4. Kullanıcı `/menu` sayfasına yönlendirilir.

```typescript
// app-routing.module.ts (QR uygulaması)

const routes: Routes = [
  // QR giriş noktası
  {
    path: 'qr/:tableId',
    loadComponent: () =>
      import('./pages/qr-landing/qr-landing.component')
        .then(m => m.QrLandingComponent)
  },
  // Menü ve sipariş sayfaları
  {
    path: 'menu',
    loadComponent: () =>
      import('./pages/menu-home/menu-home.component')
        .then(m => m.MenuHomeComponent)
  },
  {
    path: 'menu/category/:id',
    loadComponent: () =>
      import('./pages/category-detail/category-detail.component')
        .then(m => m.CategoryDetailComponent)
  },
  {
    path: 'menu/item/:id',
    loadComponent: () =>
      import('./pages/item-detail/item-detail.component')
        .then(m => m.ItemDetailComponent)
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./pages/cart/cart.component')
        .then(m => m.CartComponent)
  },
  {
    path: 'order/confirm',
    loadComponent: () =>
      import('./pages/order-confirm/order-confirm.component')
        .then(m => m.OrderConfirmComponent)
  },
  {
    path: 'order/status/:orderCode',
    loadComponent: () =>
      import('./pages/order-status/order-status.component')
        .then(m => m.OrderStatusComponent)
  },
  // Geçersiz URL → menüye yönlendir
  { path: '', redirectTo: 'menu', pathMatch: 'full' },
  { path: '**', redirectTo: 'menu' }
];

// main.ts içinde:
// bootstrapApplication(AppComponent, {
//   providers: [
//     provideRouter(routes, withHashLocation()),  // HashStrategy
//     provideHttpClient()
//   ]
// });
```

---

## 4. Angular Yapısı

### Standalone Components (module-free)
Bu uygulama Angular 14+ **standalone component** mimarisi kullanır. `NgModule` yoktur.
Her component kendi `imports` dizisini bildirir.

### Mimari Tercihler
- **Standalone components** — NgModule yok, daha hafif bundle
- **HashStrategy** — `withHashLocation()` ile  (panel ile aynı politika)
- **Mobile-first** — tüm tasarım 320px → 768px için optimize
- **PWA desteği** — `@angular/pwa` eklenebilir (ilerleyen sprintte değerlendirilecek)
- **localStorage sepet** — sayfa yenilenince sepet kaybolmaz
- **Lazy loading** — her sayfa ayrı `loadComponent()` ile yüklenir

### Klasör Yapısı

```
src/
├── app/
│   ├── core/
│   │   ├── models/
│   │   │   ├── table-info.model.ts
│   │   │   ├── menu-category.model.ts
│   │   │   ├── menu-item.model.ts
│   │   │   ├── cart-item.model.ts
│   │   │   └── order.model.ts
│   │   ├── services/
│   │   │   ├── table.service.ts       # Masa bilgisi doğrulama
│   │   │   ├── menu.service.ts        # Kategori ve ürün listeleri
│   │   │   ├── cart.service.ts        # Sepet state yönetimi (BehaviorSubject)
│   │   │   └── order.service.ts       # Sipariş oluşturma ve durum sorgulama
│   │   └── guards/
│   │       └── table-guard.ts         # Masa seçilmeden menüye erişim engeli
│   │
│   ├── shared/
│   │   ├── components/
│   │   │   ├── skeleton-card/         # Yükleme iskelet ekranı
│   │   │   ├── quantity-selector/     # + / - miktar butonu
│   │   │   ├── price-display/         # Fiyat + indirim gösterimi
│   │   │   └── bottom-cart-bar/       # Her sayfanın altında sepet özeti çubuğu
│   │   └── pipes/
│   │       └── currency-tr.pipe.ts
│   │
│   ├── pages/
│   │   ├── qr-landing/
│   │   ├── menu-home/
│   │   ├── category-detail/
│   │   ├── item-detail/
│   │   ├── cart/
│   │   ├── order-confirm/
│   │   └── order-status/
│   │
│   ├── app.component.ts               # Standalone root component
│   └── app.routes.ts                  # Route tanımları
│
├── assets/
│   ├── images/
│   └── icons/
└── environments/
    ├── environment.ts
    └── environment.prod.ts
```

---

## 5. Model Tanımları

```typescript
// core/models/table-info.model.ts

export interface TableInfo {
  id: number;
  tableId: string;           // QR'daki değer (örn: "T-14")
  number: string;            // Görünen masa numarası (örn: "14")
  name: string | null;       // Özel ad (örn: "Bahçe Masası")
  restaurantId: number;
  restaurantName: string;
  restaurantLogoUrl: string | null;
  currency: string;          // 'TRY'
  isActive: boolean;         // Masa aktif mi, servis var mı?
}
```

```typescript
// core/models/menu-category.model.ts  (QR uygulaması için basitleştirilmiş)

export interface MenuCategory {
  id: number;
  name: string;
  imageUrl: string | null;
  sortOrder: number;
  itemCount: number;
}
```

```typescript
// core/models/menu-item.model.ts  (QR uygulaması için)

export interface MenuItem {
  id: number;
  categoryId: number;
  name: string;
  description: string | null;
  imageUrl: string | null;
  price: number;
  discountedPrice: number | null;
  effectivePrice: number;      // discountedPrice ?? price — hesaplanmış fiyat
  preparationTime: number;
  calories: number | null;
  isAvailable: boolean;
  tags: string[];
}
```

```typescript
// core/models/cart-item.model.ts

export interface CartItem {
  menuItemId: number;
  name: string;
  imageUrl: string | null;
  unitPrice: number;           // Ödeme anındaki fiyat (anlık fiyat değişimine karşı)
  quantity: number;
  note: string;                // Müşteri notu (boş string olabilir)
  get totalPrice(): number;    // quantity * unitPrice
}
```

```typescript
// core/models/order.model.ts  (QR uygulaması için)

export type QrOrderStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Preparing'
  | 'Ready'
  | 'Delivered'
  | 'Cancelled';

export interface CreateOrderDto {
  tableId: string;             // Masa kimliği
  items: Array<{
    menuItemId: number;
    quantity: number;
    note?: string;
  }>;
  customerNote?: string;
}

export interface OrderStatusResponse {
  orderCode: string;
  status: QrOrderStatus;
  statusLabel: string;         // Türkçe durum metni (API'den gelir)
  estimatedReadyAt: string | null;
  items: Array<{
    name: string;
    quantity: number;
  }>;
  totalAmount: number;
  createdAt: string;
}
```

---

## 6. API Çağrıları

Tüm istekler `environment.apiUrl` + `/qr/` prefix'i ile gider.
Bu endpoint'ler public'tir; kimlik doğrulama gerektirmez.

| Method | Endpoint | Açıklama |
|---|---|---|
| `GET` | `/qr/table/:tableId` | Masa bilgisini doğrula ve getir |
| `GET` | `/qr/menu/:restaurantId/categories` | Aktif kategorileri listele |
| `GET` | `/qr/menu/category/:categoryId/items` | Kategorideki aktif ürünleri listele |
| `GET` | `/qr/menu/item/:itemId` | Ürün detayını getir |
| `POST` | `/qr/orders` | Yeni sipariş oluştur |
| `GET` | `/qr/orders/:orderCode/status` | Sipariş durumunu sorgula |

---

## 7. UX Kuralları

### 7.1 Mobile-First
- Minimum ekran genişliği: 320px
- Maksimum içerik genişliği: 480px (tablet'te de dar kolon görünümü)
- Alt navigasyon çubuğu: sepet ikonu + toplam fiyat
- Büyük dokunma alanları: butonlar min 44px yüksekliğinde

### 7.2 Hızlı Yükleme
- İlk yüklemede sadece kategoriler çekilir (ürünler lazy)
- Görsel `loading="lazy"` ile yüklenir
- API cevabı gelene kadar skeleton ekran gösterilir
- HTTP cache header'ları menü verisini kısa süre önbelleğe alır

### 7.3 Skeleton Screen
- Kategori listesi yüklenirken: 4 adet kart iskelet
- Ürün listesi yüklenirken: 6 adet ürün iskelet
- Ürün detayı yüklenirken: görsel + metin iskelet

### 7.4 Görsel Lazy Loading
```html
<img
  [src]="item.imageUrl ? (environment.fileApiUrl + item.imageUrl) : 'assets/images/no-image.png'"
  [alt]="item.name"
  loading="lazy"
  class="item-image"
/>
```

---

## 8. Gerçek Angular Kod Örnekleri

### 8.1 MenuHomeComponent (Standalone)

```typescript
// pages/menu-home/menu-home.component.ts

import { Component, OnInit, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { MenuService } from '../../core/services/menu.service';
import { CartService } from '../../core/services/cart.service';
import { MenuCategory } from '../../core/models/menu-category.model';
import { SkeletonCardComponent } from '../../shared/components/skeleton-card/skeleton-card.component';
import { BottomCartBarComponent } from '../../shared/components/bottom-cart-bar/bottom-cart-bar.component';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-menu-home',
  standalone: true,                    // Standalone — NgModule yok
  imports: [
    CommonModule,
    RouterModule,
    SkeletonCardComponent,
    BottomCartBarComponent
  ],
  templateUrl: './menu-home.component.html',
  styleUrls: ['./menu-home.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MenuHomeComponent implements OnInit {
  // Angular 14+ signal ile reaktif state (opsiyonel, BehaviorSubject de kullanılabilir)
  readonly categories = signal<MenuCategory[]>([]);
  readonly isLoading = signal<boolean>(true);
  readonly error = signal<string | null>(null);

  readonly fileApiUrl = environment.fileApiUrl;

  // Sepet sayısı — CartService'den reaktif
  readonly cartItemCount = this.cartService.itemCount;

  constructor(
    private menuService: MenuService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // tableId localStorage'da yoksa QR sayfasına geri gönder
    const tableId = localStorage.getItem('tableId');
    if (!tableId) {
      this.router.navigate(['/qr']);
      return;
    }

    this.loadCategories(tableId);
  }

  private loadCategories(tableId: string): void {
    // tableId üzerinden restaurantId bulunur (table info'dan)
    const restaurantId = Number(localStorage.getItem('restaurantId'));

    this.menuService.getCategories(restaurantId).subscribe({
      next: cats => {
        this.categories.set(cats);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Menü yüklenemedi. Lütfen tekrar deneyin.');
        this.isLoading.set(false);
      }
    });
  }

  /** Kategori kartına tıklanınca detay sayfasına git */
  onCategoryClick(categoryId: number): void {
    this.router.navigate(['/menu/category', categoryId]);
  }

  /** Görsel hata durumunda varsayılan resim göster */
  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = 'assets/images/no-image.png';
  }

  trackByCategoryId(_index: number, cat: MenuCategory): number {
    return cat.id;
  }
}
```

---

### 8.2 CartService

```typescript
// core/services/cart.service.ts

import { Injectable, computed, signal } from '@angular/core';
import { CartItem } from '../models/cart-item.model';

const CART_STORAGE_KEY = 'restaurant_cart';

@Injectable({
  providedIn: 'root'  // Uygulama genelinde tek instance
})
export class CartService {
  /** Sepet içeriği: signal tabanlı reaktif state */
  private readonly _items = signal<CartItem[]>(this.loadFromStorage());

  /** Public readonly erişim */
  readonly items = this._items.asReadonly();

  /** Toplam ürün adedi (sepet ikonundaki sayı) */
  readonly itemCount = computed(() =>
    this._items().reduce((sum, item) => sum + item.quantity, 0)
  );

  /** Toplam tutar */
  readonly totalAmount = computed(() =>
    this._items().reduce((sum, item) => sum + item.unitPrice * item.quantity, 0)
  );

  /** Sepet boş mu? */
  readonly isEmpty = computed(() => this._items().length === 0);

  /**
   * Sepete ürün ekler.
   * Ürün zaten sepette varsa miktarını artırır.
   */
  addItem(item: Omit<CartItem, 'quantity'>, quantity = 1): void {
    const current = this._items();
    const existingIndex = current.findIndex(i => i.menuItemId === item.menuItemId);

    let updated: CartItem[];

    if (existingIndex >= 0) {
      // Mevcut ürün: miktarı artır
      updated = current.map((i, idx) =>
        idx === existingIndex
          ? { ...i, quantity: i.quantity + quantity }
          : i
      );
    } else {
      // Yeni ürün: listeye ekle
      updated = [...current, { ...item, quantity }];
    }

    this._items.set(updated);
    this.saveToStorage(updated);
  }

  /**
   * Ürün miktarını günceller.
   * 0 verilirse ürünü sepetten çıkarır.
   */
  updateQuantity(menuItemId: number, quantity: number): void {
    let updated: CartItem[];

    if (quantity <= 0) {
      updated = this._items().filter(i => i.menuItemId !== menuItemId);
    } else {
      updated = this._items().map(i =>
        i.menuItemId === menuItemId ? { ...i, quantity } : i
      );
    }

    this._items.set(updated);
    this.saveToStorage(updated);
  }

  /**
   * Ürüne not ekler veya günceller.
   */
  updateNote(menuItemId: number, note: string): void {
    const updated = this._items().map(i =>
      i.menuItemId === menuItemId ? { ...i, note } : i
    );
    this._items.set(updated);
    this.saveToStorage(updated);
  }

  /**
   * Ürünü sepetten çıkarır.
   */
  removeItem(menuItemId: number): void {
    const updated = this._items().filter(i => i.menuItemId !== menuItemId);
    this._items.set(updated);
    this.saveToStorage(updated);
  }

  /**
   * Sepeti tamamen temizler (sipariş sonrası çağrılır).
   */
  clearCart(): void {
    this._items.set([]);
    localStorage.removeItem(CART_STORAGE_KEY);
  }

  /** localStorage'dan sepeti yükler (sayfa yenileme dayanıklılığı) */
  private loadFromStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(CART_STORAGE_KEY);
      return raw ? (JSON.parse(raw) as CartItem[]) : [];
    } catch {
      return [];
    }
  }

  /** Sepeti localStorage'a kaydeder */
  private saveToStorage(items: CartItem[]): void {
    localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(items));
  }
}
```

---

### 8.3 QrLandingComponent

```typescript
// pages/qr-landing/qr-landing.component.ts

import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TableService } from '../../core/services/table.service';
import { TableInfo } from '../../core/models/table-info.model';

type LandingState = 'loading' | 'success' | 'error' | 'inactive';

@Component({
  selector: 'app-qr-landing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './qr-landing.component.html',
  styleUrls: ['./qr-landing.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QrLandingComponent implements OnInit {
  readonly state = signal<LandingState>('loading');
  readonly errorMessage = signal<string>('');
  readonly tableInfo = signal<TableInfo | null>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tableService: TableService
  ) {}

  ngOnInit(): void {
    // URL'den tableId parametresini oku (/qr/:tableId)
    const tableId = this.route.snapshot.paramMap.get('tableId');

    if (!tableId) {
      this.state.set('error');
      this.errorMessage.set('Geçersiz QR kodu. Lütfen QR kodu tekrar okutun.');
      return;
    }

    this.validateTable(tableId);
  }

  private validateTable(tableId: string): void {
    this.tableService.getTableInfo(tableId).subscribe({
      next: info => {
        if (!info.isActive) {
          // Masa servise kapalı
          this.state.set('inactive');
          return;
        }

        // Masa bilgilerini oturuma kaydet
        localStorage.setItem('tableId', info.tableId);
        localStorage.setItem('tableNumber', info.number);
        localStorage.setItem('restaurantId', String(info.restaurantId));
        localStorage.setItem('restaurantName', info.restaurantName);

        this.tableInfo.set(info);
        this.state.set('success');

        // Kısa süre hoşgeldin ekranını göster, sonra menüye yönlendir
        setTimeout(() => {
          this.router.navigate(['/menu'], { replaceUrl: true });
        }, 1500);
      },
      error: err => {
        this.state.set('error');
        if (err.status === 404) {
          this.errorMessage.set('Bu QR kodu geçersiz veya artık kullanılmıyor.');
        } else {
          this.errorMessage.set('Bağlantı hatası. İnternet bağlantınızı kontrol edin.');
        }
      }
    });
  }
}
```

---

## 9. Genel Kurallar

1. **Masa doğrulaması** olmadan `/menu` sayfasına erişim engellenir (`TableGuard`).
2. **Sepet durumu** `localStorage`'da tutulur; ağ kesintisi veya sayfa yenileme sepeti kaybettirmez.
3. **Sipariş sonrası** `CartService.clearCart()` ve ardından `/order/status/:orderCode` yönlendirmesi yapılır.
4. **Stoka kapalı ürünler** (`isAvailable: false`) sepete eklenemez; buton disabled gösterilir.
5. **Görsel yükleme hataları** `(error)` event handler ile varsayılan görsele düşürülür.
6. **Sipariş durumu sayfası** her 10 saniyede polling yaparak durumu günceller (SignalR şu an yok).
7. **PWA** ilerleyen sprintte değerlendirileceğinden `manifest.json` ve service worker hazırlığı yapılabilir.
