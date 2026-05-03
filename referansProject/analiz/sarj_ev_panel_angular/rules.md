# sarj_ev_panel_angular — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
Yeni bir Angular admin panel projesi başlatırken referans olarak kullanılabilir.

---

## 1. Genel Angular Admin Panel Yapısı

**Kural:** Angular admin panel projelerinde 3 ana katman kullan:
- `core/` — teknik altyapı, singleton servisler, directive, pipe
- `feature/` (veya domain adı) — business logic, sayfalar, feature servisleri
- `shared_admin/` (veya `shared/`) — layout, guard, interceptor, Material module

**Kural:** `feature/` modülünü kendi `pages-routing.module.ts` / `feature-routing.module.ts` ile konfigüre et. App routing sadece lazy load entry point'lerini içersin.

**Kural:** `_fake/` klasörünü geliştirme ortamı için sakla, production build'e dahil etme.

---

## 2. Routing Yapısı

**Kural:** Root routing şeması her zaman şu 3 rotayı içersin:

```typescript
const routes: Routes = [
  { path: 'auth', loadChildren: () => import('./feature/auth/auth.module')... },
  { path: '', canActivate: [AuthGuard], loadChildren: () => import('./shared/layout.module')... },
  { path: '**', loadChildren: () => import('./shared/template/error-pages/...')... },
];
```

**Kural:** `useHash: true` ile HashLocationStrategy kullan — static file server ve CDN deploy için zorunlu.

**Kural:** Feature altındaki tüm sayfalar lazy load ile yüklensin. Hiçbir sayfa component'i eager load edilmemeli.

---

## 3. Interceptor Pattern

**Kural:** HTTP interceptor her Angular admin panel projesinde tek dosya olarak `shared/interceptors/` altında tutulsun.

**Kural:** Interceptor şu sorumlulukları üstlensin:
1. Her request'e `Authorization: Bearer <token>` header ekle
2. 401 yanıtında localStorage'ı temizle ve `/auth/login`'e redirect et
3. 403 yanıtında hata sayfasına yönlendir
4. `finalize()` operatörünü progress spinner için kullan (SpinnerService entegrasyonu)

```typescript
// Pattern
intercept(request, next) {
  const token = localStorage.getItem('token');
  if (token) {
    request = request.clone({ setHeaders: { Authorization: `Bearer ${token}` }});
  }
  return next.handle(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) { /* logout + redirect */ }
      return throwError(() => error);
    }),
    finalize(() => { /* spinner hide */ })
  );
}
```

**Kural:** Interceptor'ı `AppModule.providers`'a şöyle ekle:
```typescript
{ provide: HTTP_INTERCEPTORS, useClass: HttpEventInterceptor, multi: true }
```

---

## 4. Guard Pattern

**Kural:** AuthGuard `shared/auth/` altında tutulsun, `CanActivate` interface'ini implement etsin.

**Kural:** Guard'ın token kontrolü basit olsun — localStorage'da token varlığını kontrol et. JWT expiry kontrolü için `@auth0/angular-jwt`'nin `JwtHelperService` kullanılabilir.

**Kural:** Guard, orijinal URL'i `returnUrl` query parameter olarak saklasın:
```typescript
return this.router.createUrlTree(['/auth/login'], {
  queryParams: { returnUrl: redirectUrl }
});
```

**Kural:** Guard `providedIn: 'root'` ile tanımlanmalı, modüle import edilmemeli.

---

## 5. Environment Yapısı

**Kural:** Ortam sayısına göre environment dosyaları:
- Minimum 2: `environment.ts` (dev) + `environment.prod.ts`
- Tam set 4: `environment.ts` + `environment.prod.ts` + `environment.local.ts` + `environment.test.ts`

**Kural:** `angular.json`'da her environment için `fileReplacements` konfigürasyonu yapılsın.

**Kural:** Environment dosyasında hiçbir zaman şifre veya secret token tutulmasın. Sadece URL'ler ve versiyon string'i olmalı.

**Kural:** Environment şablonu (key yapısı):
```typescript
export const environment = {
  production: boolean,
  appVersion: string,      // 'v1.0.0-dev' gibi
  apiUrl: string,          // Ana backend
  fileApiUrl: string,      // Dosya servisi (varsa)
  imageUrl: string,        // CDN veya image base URL
  // + proje özelinde ek servisler
};
```

**Kural:** Birden fazla backend servisi varsa her biri için ayrı URL key'i ekle (`logApiUrl`, `notificationApiUrl` vs.)

---

## 6. Core Modülü

**Kural:** Core modülü şunları içersin:

| Klasör | İçerik |
|---|---|
| `adapters/` | API response → component model dönüşümü |
| `bases/base-datatable/` | Tüm liste componentleri bu abstract class'tan türesin |
| `bases/base-tree-table/` | Ağaç yapısı gerektiren tablolar için |
| `configs/` | Global sabitler ve konfigürasyon nesneleri |
| `date-core/` | Tarih formatlama ve işleme yardımcıları |
| `directives/` | Özel directive'ler (form, display, interaction) |
| `external-components/` | 3. parti kütüphane wrapper component'leri |
| `pipes/` | Özel pipe'lar (format, transform) |
| `services/` | Singleton utility servisler |
| `wrapper-core/` | Dış API/servis wrapper'ları |

**Kural:** Core modülü sadece `AppModule`'e import edilsin. Feature modülleri core'a bağımlı olabilir ama core feature'a bağımlı olamaz.

**Kural:** `CoreModule`'ü tekrar import engeli ile koru:
```typescript
constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
  if (parentModule) {
    throw new Error('CoreModule already loaded. Import only in AppModule.');
  }
}
```

---

## 7. Feature Modülü Organizasyonu

**Kural:** Feature modülü içindeki componentleri işlevsel kategorilere göre klasörle:
- Her kategori kendi klasörü: `category-name/`
- Her kategori kendi `NgModule`'ü olabilir (lazy loading için)

**Kural:** Feature klasörü şu alt yapıyı içersin:
```
feature/
├── components/     (UI katmanı — kategorilere ayrılmış)
├── models/         (data modelleri, ayrı klasörler önerilir)
├── services/       (API servis katmanı)
├── enums/          (sabitler ve enum'lar)
├── pipes/          (feature-specific pipe'lar)
└── pages-routing.module.ts
```

**Kural:** Feature scope büyüdükçe (10+ kategori) model katmanını alt klasörlere ayır. Örnek: `models/authentication/`, `models/mediaFile/` vs.

**Kural:** Feature routing, `LayoutModule`'ün child route'ları olarak tanımlanmalı:
```
/ (Layout)
  ├── /home
  ├── /resource-list
  └── /admin-management
```

---

## 8. Shared Admin Layout Modülü

**Kural:** `shared_admin/` (veya `shared/`) şunları içersin:

```
shared_admin/
├── auth/
│   ├── auth.guard.ts          — route koruma
│   └── authentication-service.ts  — login/logout/token
├── partials/
│   ├── aside/                 — sidebar navigation
│   ├── header/                — üst nav bar
│   ├── footer/
│   ├── topbar/                — kullanıcı menüsü (sağ üst)
│   ├── subheader/             — breadcrumb + başlık
│   ├── layout/                — sayfa wrapper
│   └── dialogs/               — global dialog componentleri
├── template/
│   ├── error-pages/           — 404, 403, 500
│   └── splash-screen/         — başlangıç yükleme ekranı
├── utils/interseptors/        — HTTP interceptor
├── general-material.module.ts — Angular Material export consolidation
├── layout.module.ts           — ana layout NgModule
└── shared.module.ts           — paylaşılan componentler
```

**Kural:** `LayoutModule` feature modülünü child olarak yüklesin:
```typescript
// layout routing
{ path: '', loadChildren: () => import('../feature/feature.module')... }
```

**Kural:** `general-material.module.ts` tüm kullanılan Angular Material modüllerini import ve export etsin. Feature ve shared modülleri sadece bu modülü import etsin.

---

## 9. Paket Seçimi

**Kural:** Bu mimari için standart paket seti:

| Kategori | Paket |
|---|---|
| Auth | @auth0/angular-jwt |
| Realtime | @microsoft/signalr |
| i18n | @ngx-translate/core + http-loader |
| Chart | ApexCharts (ng-apexcharts) — tercih edilmeli |
| Tarih | Moment.js (ya da migrate: date-fns) |
| HTML Editör | ngx-editor |
| Dosya Upload | ngx-dropzone |
| Loading | ngx-spinner |

**Kural:** Birden fazla chart kütüphanesi kullanmaktan kaçın. ApexCharts veya Chart.js'den birini seç.

---

## 10. Genel Kalite Kuralları

**Kural:** `tsconfig.json`'da `strict: false` — pragmatik tercih, özellikle Metronic gibi hazır template'ler üzerinde çalışırken. Yeni projede `strict: true` başlatılabilir.

**Kural:** Path alias'ları `tsconfig.json`'da tanımla:
```json
"paths": {
  "@app/*": ["src/app/*"],
  "@env/*": ["src/environments/*"],
  "@core/*": ["src/app/core/*"],
  "@shared/*": ["src/app/shared_admin/*"]
}
```

**Kural:** SCSS kullan, CSS değil. Her component kendi scoped style'ına sahip olsun.

**Kural:** `production: true` ortamında `outputHashing: "all"` aktif olsun — cache busting için.
