# dijital_menu_angular — Geliştirme Kuralları

Bu dosya, gerçek projeden çıkarılan, yeni proje geliştirirken uygulanması gereken kuralları ve code pattern'lerini içerir.

---

## Kural 1: Environment'a Hardcoded Değer Yazma

Her API URL, siteKey, endpoint mutlaka `environment.ts` üzerinden okunmalıdır.

**YANLIS:**
```typescript
ServerUrl = 'https://metropolitanhost.com/scripts/sendmail.php';
```

**DOGRU:**
```typescript
// environment.ts
export const environment = {
  production: false,
  contactApiUrl: 'https://metropolitanhost.com/scripts/sendmail.php',
  recaptchaSiteKey: '6LdxUhMaAAAAAIrQt-_6Gz7F_58S4FlPWaxOh5ib'
};

// service içinde:
import { environment } from 'src/environments/environment';
ServerUrl = environment.contactApiUrl;
```

```html
<!-- Template içinde de: -->
<re-captcha [siteKey]="siteKey" ...></re-captcha>
<!-- component.ts'de: siteKey = environment.recaptchaSiteKey; -->
```

---

## Kural 2: Contact Form — Model-Driven Yaklaşım ile Yazılmalı

Template-driven form (`[(ngModel)]`) ile reactive form (`FormGroup`) karıştırılmamalı. Reactive form tercih edilmeli.

**Bu projede kullanılan pattern (template-driven, kabul edilebilir):**
```typescript
export class Contact {
    id: number | undefined;
    name: string | undefined;
    email: string | undefined;
    phone?: string | undefined;
    subject: string | undefined;
    message: string | undefined;
    recaptcha: string | undefined;
}
```
```html
<form (ngSubmit)="onSubmit()" #contactForm="ngForm">
  <input type="text" name="name" [(ngModel)]="model.name" required>
  <button [disabled]="!contactForm.form.valid">Submit</button>
</form>
```

**Tercih edilen reactive form alternatifi:**
```typescript
this.contactForm = this.fb.group({
  name: ['', Validators.required],
  email: ['', [Validators.required, Validators.email]],
  phone: ['', Validators.required],
  subject: ['', Validators.required],
  message: ['', Validators.required],
  recaptcha: ['', Validators.required]
});
```

---

## Kural 3: HTTP Service handleError Arrow Function Kullan

`private handleError(error)` metodu class method olarak tanımlandığında `this` bağlamı kaybolur.

**YANLIS:**
```typescript
private handleError(error: HttpErrorResponse) {
    // this.errorData burada undefined olabilir
    this.errorData = { ... };
    return throwError(this.errorData);
}
```

**DOGRU:**
```typescript
private handleError = (error: HttpErrorResponse) => {
    this.errorData = { ... };
    return throwError(() => this.errorData);
};
```

---

## Kural 4: reCAPTCHA invisible Entegrasyon Pattern'i

Invisible reCAPTCHA kullanımı için standart pattern:

```typescript
// app.module.ts imports[]:
RecaptchaModule,
RecaptchaFormsModule

// Contact model'e ekle:
recaptcha: string | undefined;

// Component metodu:
resolved(captchaResponse: string) {
  this.model.recaptcha = captchaResponse;
  // Gerekirse console.log kaldırılmalı:
  // console.log(`Resolved response token: ${captchaResponse}`);
}
```

```html
<re-captcha
  (resolved)="resolved($event)"
  name="recaptcha"
  [(ngModel)]="model.recaptcha"
  [siteKey]="recaptchaSiteKey"
  size="invisible">
</re-captcha>
```

**Not:** `size="invisible"` — kullanıcı arayüzde görmez, form submit'te otomatik tetiklenir.

---

## Kural 5: Layout Bileşenlerini Merkezi Shell'e Taşı

Her sayfa içine `<app-navbar>` ve `<app-footer>` koymak yerine merkezi bir shell component kullan.

**Bu projede (tekrarlayan, kötü pattern):**
```html
<!-- her page template'inde: -->
<app-navbar></app-navbar>
<!-- sayfa içeriği -->
<app-footer1></app-footer1>
```

**Tercih edilen pattern:**
```typescript
// shell.component.html
<app-navbar></app-navbar>
<router-outlet></router-outlet>
<app-footer1></app-footer1>

// routing:
{
  path: '',
  component: ShellComponent,
  children: [
    { path: '', component: HomeComponent },
    { path: 'contact', component: ContactComponent },
  ]
}
```

---

## Kural 6: Contact Form Başarı Kontrolü id'ye Değil status'e Bağla

**YANLIS (kırılgan):**
```html
<div *ngIf="model.id">...</div>
```
Backend id dönmezse başarı mesajı hiç gösterilmez.

**DOGRU:**
```typescript
// component.ts
submitted = false;
submitSuccess = false;
error: any = null;

onSubmit() {
  this.contactService.contactForm(this.model).subscribe({
    next: () => { this.submitSuccess = true; },
    error: (err) => { this.error = err; }
  });
}
```
```html
<div *ngIf="submitSuccess" class="contact-success">
  <ngb-alert type="success" [dismissible]="false">
    <strong>Success!</strong> Contact form submitted.
  </ngb-alert>
</div>
<div *ngIf="error" class="service-error">
  <ngb-alert type="danger" [dismissible]="false">
    <strong>Oops!</strong> Something bad happened.
  </ngb-alert>
</div>
```

---

## Kural 7: Routing — Wildcard Route En Sonda Olmalı

```typescript
const routes: Routes = [
  { path: '', component: HomeComponent },
  // ... diğer rotalar
  { path: 'error-page', component: ErrorPageComponent },
  { path: '**', component: ErrorPageComponent }  // MUTLAKA EN SONDA
];
```

---

## Kural 8: PathLocationStrategy — Server Rewrite Unutma

`PathLocationStrategy` (hash-less URL) kullandığında nginx veya Apache konfigürasyonu gerekir:

**nginx örneği:**
```nginx
location / {
  try_files $uri $uri/ /index.html;
}
```

**Alternatif: HashLocationStrategy (daha basit deploy):**
```typescript
// app.module.ts providers[]:
{ provide: LocationStrategy, useClass: HashLocationStrategy }
// URL: example.com/#/contact
```

---

## Kural 9: Tekrarlayan Banner Component'larını Parametrik Yap

**YANLIS (3 ayrı component, kod tekrarı):**
```html
<app-advertisementbanner></app-advertisementbanner>
<app-advertisementbanner1></app-advertisementbanner1>
<app-advertisementbanner2></app-advertisementbanner2>
```

**DOGRU (tek parametrik component):**
```typescript
@Component({ selector: 'app-advertisement-banner' })
export class AdvertisementBannerComponent {
  @Input() imageUrl: string;
  @Input() linkUrl: string;
  @Input() altText: string;
}
```
```html
<app-advertisement-banner imageUrl="..." linkUrl="..." altText="..."></app-advertisement-banner>
```

---

## Kural 10: throwError RxJS 7+ Uyumlu Kullanım

```typescript
// YANLIS (RxJS 7+ deprecated):
return throwError(this.errorData);

// DOGRU:
return throwError(() => this.errorData);
// veya:
return throwError(() => new Error('Something went wrong'));
```

---

## Kural 11: routerLinkActive ile Exact Match

Aktif rota için hem `routerLinkActive` hem `routerLinkActiveOptions` kullan:

```html
<a routerLink="/"
   routerLinkActive="active"
   [routerLinkActiveOptions]="{exact: true}">
  Landing Page
</a>

<!-- exact: true olmadan "/" tüm alt rotaları da aktif görür -->
<a routerLink="/contact"
   routerLinkActive="active"
   [routerLinkActiveOptions]="{exact: true}">
  Contact
</a>
```

---

## Kural 12: NgbModule — Alert Bileşeni Kullanımı

ng-bootstrap alert bileşeni için `[dismissible]="false"` ile kullanıcı kapatmasını engelle:

```html
<!-- Hata mesajı -->
<ngb-alert type="danger" class="mb-0 w-100" [dismissible]="false">
  <strong>Oops!</strong> Something bad happened. Please try again later.
</ngb-alert>

<!-- Başarı mesajı -->
<ngb-alert type="success" class="mb-0 w-100" [dismissible]="false">
  <strong>Success!</strong> Form has been successfully submitted.
</ngb-alert>
```

`type` değerleri: `"success"`, `"danger"`, `"warning"`, `"info"`, `"primary"`, `"secondary"`.

---

## Kural 13: providedIn: 'root' Servis Tanımı

Uygulama genelinde kullanılacak servisler `providedIn: 'root'` ile tanımlanmalı — AppModule providers[] içine eklemeye gerek kalmaz:

```typescript
@Injectable({
  providedIn: 'root'
})
export class ContactService {
  constructor(private http: HttpClient) { }
  // ...
}
```

---

## Kural 14: AppModule'da Location Strategy Kayıt Biçimi

```typescript
providers: [
  Location,
  {
    provide: LocationStrategy,
    useClass: PathLocationStrategy  // veya HashLocationStrategy
  }
]
```

`Location` servisi de providers'a eklenmeli — bazı component'lar Location servisini inject edebilir.

---

## Özet — Yeni Projede Yapılacaklar

| Konu | Eylem |
|---|---|
| API URL | environment.ts'e taşı |
| reCAPTCHA siteKey | environment.ts'e taşı |
| handleError | Arrow function yap |
| throwError | `() =>` factory kullan |
| Başarı kontrolü | id yerine boolean flag kullan |
| Layout | Shell component + router-outlet pattern |
| Banner bileşenleri | Tek parametrik component yap |
| Slider | ngx-swiper-wrapper veya Angular uyumlu paket kullan |
| relativeLinkResolution | Kaldır (Angular 13+ desteklemiyor) |
| Server deploy | nginx try_files ekle |
