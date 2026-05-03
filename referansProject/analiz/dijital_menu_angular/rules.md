# dijital_menu_angular — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
Backend bağımlılığı olmayan public Angular site geliştirirken referans olarak kullanılabilir.

---

## 1. Public Site — Backend Gerektirmeyen Yapı

**Kural:** Statik içerik sunan public sitelerde backend API maliyetinden kaçınmak için emailJS veya benzeri SaaS form servislerini kullan.

**Kural:** emailJS entegrasyon şablonu:
```typescript
import emailjs from 'emailjs-com';

sendEmail(formData: ContactForm): Observable<boolean> {
  return from(
    emailjs.send(
      environment.emailjsServiceId,
      environment.emailjsTemplateId,
      {
        from_name: formData.name,
        reply_to: formData.email,
        message: formData.message
      },
      environment.emailjsUserId
    )
  ).pipe(
    map(() => true),
    catchError(() => of(false))
  );
}
```

**Kural:** emailJS credentials (serviceId, templateId, userId) `environment.ts` ve `environment.prod.ts`'de tutulsun. Component içinde hardcode edilmemeli.

**Kural:** emailJS rate limit vardır (free plan: 200 email/ay). Yüksek trafik beklenen formlarda ücretli plan veya alternatif değerlendir.

---

## 2. reCAPTCHA Entegrasyonu

**Kural:** Public formlarda `ng-recaptcha` ile Google reCAPTCHA v2 ekle:

```html
<re-captcha (resolved)="onCaptchaResolved($event)" [siteKey]="siteKey">
</re-captcha>
```

```typescript
onCaptchaResolved(token: string) {
  this.captchaToken = token;
}

submitForm() {
  if (!this.captchaToken) {
    // CAPTCHA tamamlanmadan gönderme
    return;
  }
  // form gönder
}
```

**Kural:** reCAPTCHA site key `environment.ts`'de sakla. Secret key backend'de olmalı — client-side'da kullanılmaz.

---

## 3. Monolithic Module — Ne Zaman Kabul Edilebilir

**Kural:** Şu koşullarda tek AppModule (eager loading) tercih edilebilir:
- 10'dan az sayfa
- Route tabanlı code splitting gerekmiyorsa
- Initial bundle boyutu < 500KB ise
- Kullanıcı tüm sayfaları ziyaret edecekse

**Kural:** Site 20+ sayfaya büyüyecekse baştan lazy loading mimarisini kur. Sonradan geçiş maliyetlidir.

**Kural:** Sayfa sayısı > 15 olursa feature module ayrımı yap:
```
AppModule
├── RestaurantModule (lazy)
├── MenuModule (lazy)
├── ContactModule (lazy)
└── SharedModule (eager — layout component'ları)
```

---

## 4. PathLocationStrategy — Sunucu Konfigürasyonu

**Kural:** `useHash: false` (PathLocationStrategy) kullanan Angular uygulamaları için mutlaka sunucu konfigürasyonu ekle.

**Kural:** Nginx:
```nginx
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

**Kural:** Apache (.htaccess):
```apache
<IfModule mod_rewrite.c>
    RewriteEngine On
    RewriteBase /
    RewriteRule ^index\.html$ - [L]
    RewriteCond %{REQUEST_FILENAME} !-f
    RewriteCond %{REQUEST_FILENAME} !-d
    RewriteRule . /index.html [L]
</IfModule>
```

**Kural:** GitHub Pages, Netlify, Vercel gibi static hosting platformları bunu otomatik yönetir. `_redirects` (Netlify) veya `vercel.json` konfigürasyonu ekle.

---

## 5. Pages ve Layouts Ayrımı

**Kural:** Public sitede `pages/` ve `layouts/` klasörlerini ayır:

```
src/app/
├── pages/          — Rota component'ları (bir URL = bir sayfa component)
├── layouts/        — Çerçeve component'ları (header, footer, sidebar, nav)
└── shared/         — Tekrar kullanılan UI parçaları (card, button vb.)
```

**Kural:** Layout component'ları `<router-outlet>` içersin. Sayfalar layout içine yüklenir:
```typescript
const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,  // layouts/ içinden
    children: [
      { path: '', component: HomePageComponent },       // pages/ içinden
      { path: 'menu', component: MenuPageComponent },
      { path: 'contact', component: ContactPageComponent }
    ]
  }
];
```

**Kural:** Farklı sayfa grupları için farklı layout kullanılabilir (örn: tam ekran menü sayfası, yan menüsüz galeri sayfası).

---

## 6. Template Bazlı Geliştirme

**Kural:** HTML/CSS template kullanılıyorsa Angular'a entegrasyon sırasında şunlara dikkat et:
- Template'deki ID ve class adlarını değiştirme (CSS bozulur)
- Template'deki global script referanslarını `angular.json`'da `scripts[]` array'ine ekle
- `assets/` klasörüne template media dosyalarını ekle

**Kural:** Template'den gelen CSS `styles.scss`'e import et:
```scss
@import "assets/template/css/bootstrap.min.css";
@import "assets/template/css/theme.css";
```

**Kural:** Template'deki jQuery bağımlılıklarını Angular direktiflere dönüştür. jQuery ile Angular DOM manipülasyonu çakışır.

---

## 7. UI Kütüphanesi Seçimi

**Kural:** Angular Material ve @ng-bootstrap'i birlikte kullanmaktan kaçın. Birini seç:
- `@angular/material` — Material Design, Google ekosistemi
- `@ng-bootstrap` — Bootstrap tabanlı, jQuery bağımlılığı yok

**Kural:** Public site (kullanıcı-facing) için template uyumlu kütüphane seç. Admin panelde Material, public sitede Bootstrap daha yaygın kullanılır.

---

## 8. SEO için Başlık ve Meta Tag Yönetimi

**Kural:** Public site SEO gerektirir. Her sayfa için başlık ve meta description güncelle:
```typescript
constructor(
  private title: Title,
  private meta: Meta
) {}

ngOnInit() {
  this.title.setTitle('Restoran Adı - Dijital Menü');
  this.meta.updateTag({
    name: 'description',
    content: 'Lezzetli yemekler, online menü...'
  });
}
```

**Kural:** Social media paylaşımları için Open Graph tag'leri ekle:
```typescript
this.meta.updateTag({ property: 'og:title', content: 'Restoran Adı' });
this.meta.updateTag({ property: 'og:image', content: 'https://...' });
```
