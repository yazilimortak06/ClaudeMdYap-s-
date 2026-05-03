# restoran_panel_angular — Çıkarılan Kurallar

Bu dosya, Angular 14 strict mode scaffold'dan çıkarılan tekrar edilebilir kural ve standartları içerir.
Yeni bir Angular admin panel projesi başlatırken bu kuralları temel al.

---

## 1. Yeni Proje Başlatma Standartları

**Kural:** Angular 14+ ile yeni proje oluştururken strict mode aktif et:
```bash
ng new my-project --strict --routing --style=scss
```

**Kural:** Proje oluşturulur oluşturulmaz şu konfigürasyonları doğrula:
- `tsconfig.json`: `"strict": true`
- `angular.json`: `"aot": true`, `"buildOptimizer": true`
- `tsconfig.json`: `"target": "ES2017"` veya üstü

**Kural:** `--style=scss` ile başla. CSS'den SCSS'e geçiş sonradan zahmetlidir.

---

## 2. Strict TypeScript — Zorunlu Kurallar

**Kural:** `strictNullChecks: true` ile çalışırken şunlara dikkat et:

```typescript
// YANLIŞ
user: User;  // hata: Property 'user' has no initializer

// DOĞRU (seçenekler)
user: User | undefined;          // undefined olabilir
user!: User;                     // kesinlikle atanacak (non-null assertion)
user: User = {} as User;         // başlangıç değeri
```

**Kural:** `strictPropertyInitialization` için constructor'da initialize et veya `!` assertion kullan:
```typescript
@Component(...)
export class MyComponent implements OnInit {
  // Constructor'da değil, ngOnInit'te atanacak:
  data!: MyData;  // "!" ile "kesinlikle atanacak" garantisi

  ngOnInit() {
    this.data = { ... };  // burada atanıyor
  }
}
```

**Kural:** `any` tipinden kaçın. Tip bilgisi yoksa `unknown` kullan ve type guard ile daralt:
```typescript
// YANLIŞ
parseResponse(data: any) { ... }

// DOĞRU
parseResponse(data: unknown) {
  if (typeof data === 'object' && data !== null) {
    // type safe işlemler
  }
}
```

---

## 3. Angular 14+ Feature'ları

**Kural:** Angular 14'te standalone components kullanabilirsin (opsiyonel). Yeni projede tercih et:
```typescript
@Component({
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule],
  template: `...`
})
export class MyStandaloneComponent {}
```

**Kural:** Standalone API ile NgModule kullanımını azalt. Yeni Angular 17+ ile NgModule'siz uygulama mümkün.

**Kural:** TypedForms (Angular 14+) kullan. Artık form değerleri tip güvenli:
```typescript
// Angular 14+ — TypedForms
const form = new FormGroup({
  name: new FormControl<string>('', Validators.required),
  email: new FormControl<string>('', [Validators.required, Validators.email]),
});
// form.value.name artık string | null, any değil
```

---

## 4. Module Yapısı — Başlangıç Şablonu

**Kural:** Admin panel için önerilen başlangıç module yapısı:

```
AppModule
├── CoreModule (singleton servisler, interceptor, guard)
├── SharedModule (ortak component, directive, pipe)
└── AppRoutingModule
    ├── /auth → AuthModule (lazy)
    └── /      → LayoutModule (lazy, canActivate: AuthGuard)
                 └── feature routes (lazy)
```

**Kural:** `AppModule` minimal kalsın. Sadece uygulama başlatma için gerekli import'ları içersin.

**Kural:** `CoreModule`'ü constructor guard ile koru:
```typescript
@NgModule({ ... })
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule?: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in AppModule only.');
    }
  }
}
```

---

## 5. Routing Başlangıç Yapısı

**Kural:** `app-routing.module.ts` başlangıç şablonu:

```typescript
const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./modules/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./modules/layout/layout.module').then(m => m.LayoutModule)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
```

**Kural:** Hash routing (`useHash: true`) kullanma. Admin paneller genellikle server-side routing desteğiyle deploy edilir.

---

## 6. Environment Yapısı

**Kural:** Angular 14 için environment dosyaları:
```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',
  fileApiUrl: 'http://localhost:5001',
  appVersion: '1.0.0-dev'
};

// environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.domain.com',
  fileApiUrl: 'https://files.domain.com',
  appVersion: '1.0.0'
};
```

**Kural:** `angular.json`'da her ortam için `fileReplacements` ve `optimization` konfigürasyonu şu şekilde olsun:
```json
"production": {
  "optimization": true,
  "outputHashing": "all",
  "sourceMap": false,
  "namedChunks": false,
  "aot": true,
  "buildOptimizer": true
}
```

---

## 7. Restoran Paneli Özel Kurallar

**Kural:** Restoran paneli özellikleri geliştirilirken şu öncelik sırasını takip et:
1. Auth (giriş, oturum yönetimi) — önce bu olmadan diğerleri çalışmaz
2. Layout (sidebar, header, routing) — temel navigasyon
3. Dashboard (özet) — ana sayfa
4. CRUD feature'lar (menü, sipariş, masa) — iş mantığı

**Kural:** Her feature module şu klasör yapısını takip etsin:
```
modules/menu-management/
├── components/           — Sayfa component'ları
├── models/               — Feature'a özgü modeller
├── services/             — API servis sınıfı
├── menu-management.module.ts
└── menu-management-routing.module.ts
```

**Kural:** Restoran verisi gerçek zamanlı olabilir (yeni siparişler, masa durumu). SignalR veya polling mekanizması düşün.
