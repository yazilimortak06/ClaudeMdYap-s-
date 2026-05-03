# restoran_panel_angular — Klasör Yapısı

## Genel Bilgi
- Framework: Angular 14.0.0
- Tür: Boş scaffold / başlangıç noktası
- Mimari: Minimal — sadece temel Angular yapısı, strict mode aktif

---

## Tam Dizin Ağacı

```
restoran_panel_angular/
├── src/
│   ├── app/
│   │   ├── app-routing.module.ts    ← routes: [] (boş)
│   │   ├── app.module.ts            ← minimal: BrowserModule + AppRoutingModule
│   │   ├── app.component.ts
│   │   ├── app.component.html
│   │   ├── app.component.css
│   │   └── app.component.spec.ts
│   ├── assets/
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   ├── index.html
│   ├── main.ts
│   ├── polyfills.ts
│   └── styles.css
├── angular.json
├── package.json
├── tsconfig.json
├── tsconfig.app.json
└── tsconfig.spec.json
```

---

## Scaffold Durumu

Bu proje bilinçli olarak boş bırakılmıştır. Amaç:
- Temiz Angular 14 başlangıç noktası sağlamak
- Strict TypeScript modunun aktif gelmesi
- Herhangi bir proje için hızlı scaffold olarak kullanım
- Gereksiz bağımlılık olmaksızın başlamak

---

## Strict Mode Özellikleri (tsconfig.json)

| Özellik | Durum |
|---------|-------|
| `strict` | true |
| `noImplicitOverride` | true |
| `noPropertyAccessFromIndexSignature` | true |
| `noImplicitReturns` | true |
| `noFallthroughCasesInSwitch` | true |
| `strictTemplates` | true |
| `strictInjectionParameters` | true |

---

## Bağımlılıklar Özeti

| Paket | Versiyon | Not |
|-------|----------|-----|
| @angular/core | 14.0.0 | Modern Angular |
| rxjs | ~7.5.0 | Reaktif programlama |
| zone.js | ~0.11.4 | Change detection |

Minimal bağımlılık — eklenecek paketler projeye göre belirlenir.
