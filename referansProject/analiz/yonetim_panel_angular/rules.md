# yonetim_panel_angular — Çıkarılan Kurallar

Bu dosya sarj_ev_panel_angular/rules.md ile büyük ölçüde örtüşür.
Aşağıda yalnızca bu projeden öğrenilen ek veya farklı kurallar yer alır.
Tüm temel kurallar için `sarj_ev_panel_angular/rules.md`'ye bakınız.

---

## 1. MVP / Küçük Panel İçin Basitleştirme Kuralları

**Kural:** Eğer proje MVP veya internal tool ise environment sayısını 2'ye indir: `environment.ts` + `environment.prod.ts`. Local ve test ortamları için `.env` dosyası veya runtime konfigürasyon kullanılabilir.

**Kural:** Feature kategorisi sayısı 10'un altındaysa single feature module yeterli. Her kategori için ayrı lazy-load module oluşturmak overkill olabilir.

**Kural:** SignalR gibi realtime bağımlılıkları gerçekten kullanılmıyorsa `package.json`'dan çıkar — bundle size ve build time'ı etkiler.

---

## 2. Bootstrap Versiyon Kilitleme

**Kural:** Metronic ile Bootstrap kullanırken Bootstrap versiyonunu `package.json`'da explicit olarak sabitle:
```json
"bootstrap": "4.6.1"
```
Metronic ile Bootstrap arasındaki versiyon uyumsuzlukları CSS bozulmalarına yol açar.

**Kural:** `angular.json`'da Bootstrap CSS ve JS'ini styles/scripts sırasına doğru ekle:
```json
"styles": ["src/styles.scss", "node_modules/bootstrap/dist/css/bootstrap.min.css"],
"scripts": ["node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"]
```

---

## 3. Küçük Feature Scope Modeli

**Kural:** 6 kategorili küçük panel için feature routing şeması:

```typescript
// Minimal feature routing
const routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', loadChildren: () => import('./components/home/home.module')... },
  { path: 'entity-management', loadChildren: ... },
  { path: 'category-management', loadChildren: ... },
  { path: 'authority-management', loadChildren: ... },
  { path: 'admin-management', loadChildren: ... },
];
```

**Kural:** Küçük panellerde bile lazy loading tercih edilmeli. Başlangıçta gereksiz görünse de proje büyüdüğünde refactor maliyetini önler.

---

## 4. Angular Admin Panel İçin Ortak Başlangıç Şablonu

Bu projeden çıkarılan en önemli kural şu: üç proje de aynı core/shared altyapısını paylaşıyor.

**Kural:** Birden fazla Angular admin panel projesi geliştiriyorsan, paylaşılan altyapıyı (core, shared/layout, interceptor, guard) bir "Angular Admin Starter" template'i olarak yönet. Her projede sadece feature modülünü değiştir.

**Kural:** Starter template'in içermesi gerekenler:
- `core/` (adapters, bases, directives, pipes, external-components)
- `shared/` (layout componentleri, interceptor, guard, material module)
- Root routing şeması (auth + layout + error)
- 4 environment dosyası şablonu
- `tsconfig.json` path alias'ları

**Kural:** Feature modülünün adını domain'e özgü yap (`evtech/`, `pixdinnRestaurantSystem/`, `sarjAllPro/`) ya da generic tut (`feature/`). Her iki yaklaşım da çalışır; konsistans önemli.
