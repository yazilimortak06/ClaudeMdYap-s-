# dijital_menu_angular — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | Angular 11 |
| UI Kütüphanesi | Angular Material 9 |
| Form E-posta | emailjs-com |
| CAPTCHA | ng-recaptcha |
| Bootstrap | @ng-bootstrap |
| Routing | PathLocationStrategy (HTML5) |
| Modül yapısı | Tek AppModule, Eager Loading |

## Genel Bakış

`dijital_menu_angular` (PixdinnNewMenu), restoran listeleme ve menü sunum amaçlı bir public web sitesidir. Template bazlı bir yaklaşım benimsenmiştir. Backend gerektirmez — form gönderimi emailJS ile yönetilir.

## Mimari Pattern

**Single Page Application — Monolithic Angular Module**

Standart Angular best-practice'ların aksine, tüm component'lar tek `AppModule`'e dahil edilmiştir. Feature module ayrımı yoktur; tüm sayfa ve layout component'ları eager load edilir.

Bu tercih küçük, statik bir site için makul: derleme karmaşıklığını azaltır, lazy loading gerektirmeyen küçük bundle boyutu için kabul edilebilir.

## Routing — PathLocationStrategy

```typescript
RouterModule.forRoot(routes)   // useHash yok → PathLocationStrategy
```

HTML5 History API kullanılır. Bu, sunucu tarafında wildcard redirect konfigürasyonu gerektirir. Nginx veya Apache'de `try_files $uri $uri/ /index.html` gibi bir ayar zorunludur.

## Proje Yapısı

```
src/app/
├── pages/          (21 sayfa component'ı)
│   ├── home/
│   ├── restaurant-list/
│   ├── restaurant-detail/
│   ├── menu/
│   ├── about/
│   ├── contact/
│   └── ...
├── layouts/        (10 layout component'ı)
│   ├── header/
│   ├── footer/
│   ├── sidebar/
│   └── ...
└── app.module.ts   (tüm component'lar buraya)
```

## Backend Gerektirmeyen Form Gönderimi (emailJS)

`emailjs-com` paketi ile form verileri doğrudan emailJS SaaS servisine gönderilir, herhangi bir backend endpoint'e değil. Bu yaklaşım:

**Avantajlar:**
- Backend kurulum gerektirmez
- Statik hosting (GitHub Pages, Netlify, Vercel) üzerine deploy edilebilir
- CORS sorunu yok

**Dezavantajlar:**
- emailJS API key client-side kodda görünür
- emailJS bağımlılığı (3. parti SaaS)
- Form gönderim geçmişi kendi sistemde tutulmaz

## reCAPTCHA Entegrasyonu

`ng-recaptcha` paketi ile Google reCAPTCHA v2 entegrasyonu yapılmış. Contact formu gibi halka açık formlarda bot koruması sağlar. Site key `environment.ts`'de tutulmalı.

## @ng-bootstrap Kullanımı

Angular Material'e ek olarak `@ng-bootstrap` da kullanılmış. Bu ikili kullanım:
- Farklı UI bileşenler için farklı kütüphaneler (modal için ng-bootstrap, input için Material)
- Template'ten gelen hazır component'ların korunması

İdealde tek bir UI kütüphanesi kullanılmalı.

## Sayfa ve Layout Ayrımı

21 page component, belirli layout'lara yerleştirilmiş:
- Üst menü ve footer sabit
- Layout component'ları sayfalar için wrapper görevi görür
- Angular router'da layout-page hiyerarşisi kurulmuş

## Auth / Kullanıcı Girişi

**Bu projede authentication yoktur.** Tamamen public site. Kullanıcı kaydı, giriş veya korumalı sayfa bulunmaz. Tüm sayfalar herkese açık.

## Deployment

PathLocationStrategy nedeniyle web sunucusu konfigürasyonu gerekir. Nginx örneği:
```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

Static hosting platformları (Netlify, Vercel) bunu otomatik yapar.

## Dikkat Çeken Noktalar

### Olumlu
- Sıfır backend bağımlılığı — statik site olarak deploy edilebilir
- emailJS ile operasyonel form, backend maliyeti yok
- Küçük proje için single module yeterli ve daha basit

### İyileştirme Alanları
- Angular 11 → 14+ upgrade öncelikli (Angular 11 destek dışında)
- Single module → lazy loading ile küçük module'lere bölme (site büyürse)
- emailJS API key `environment.ts`'de tutulmalı, component içinde hardcode olmamalı
- Çift UI kütüphanesi (Material + ng-bootstrap) birleştirilmeli

## Projeler Arası Benzerlikler

### dijital_menu_angular vs restoran_panel_angular
- dijital_menu: Angular 11, public site, içerik dolu
- restoran_panel: Angular 14, admin panel scaffold, minimal
- İkisi de restoran sistemleri için ancak farklı roller (public vs admin)

## Sonuç

Bu proje, restoran/menü tanıtım sitesi için geliştirilmiş, backend bağımlılığı olmayan bir Angular uygulamasıdır. Template bazlı yapı ve emailJS entegrasyonu ile hızlı kurulum sağlanmış. Küçük scope için mimari kararlar pragmatik ve uygun.
