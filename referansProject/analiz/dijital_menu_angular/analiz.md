# dijital_menu_angular — Kapsamlı Analiz

## 1. Platform & Tech Stack

| Kategori | Detay |
|---|---|
| Framework | Angular (NgModule mimarisi) |
| Dil | TypeScript |
| UI Kit | Bootstrap + NgbModule (ng-bootstrap) |
| Form | FormsModule + ReactiveFormsModule |
| HTTP | HttpClientModule |
| Routing | RouterModule (PathLocationStrategy) |
| Animasyon | BrowserAnimationsModule |
| Dialog | MatDialogModule (@angular/material/dialog) |
| reCAPTCHA | ng-recaptcha (RecaptchaModule + RecaptchaFormsModule) |
| Contact Backend | metropolitanhost.com/scripts/sendmail.php (POST) |
| Slider | Swiper.js (assets üzerinden statik vanilla JS) |
| Icons | Font Awesome, Flaticon |

## 2. Proje Yapısı (Klasör Ağacı)

```
src/
  app/
    app.component.ts / .html
    app.module.ts
    app-routing.module.ts
    helper/
      contact/
        contact-helper.service.ts
        contact-helper.service.spec.ts
    models/
      contact/
        contact.ts
        contact.spec.ts
    layouts/
      navbar/
      footer1/
      footer2/
      advertisementbanner/
      advertisementbanner1/
      advertisementbanner2/
      blogleftsidebar/
      blogrightsidebar/
      restaurantleftsidebar/
      restaurantrightsidebar/
    pages/
      home/
      homepage1/
      homepage2/
      homepage3/
      homepage4/
      about/
      addrestaurant/
      blog/
      blogdetails/
      blogstyle2/
      checkout/
      contact/
      error-page/
      exdeals/
      geolocator/
      listview/
      login/
      orderdetails/
      register/
      restaurant/
      restaurantstyle1/
      restaurantstyle2/
  environments/
    environment.ts
    environment.prod.ts
```

## 3. app.module.ts — Tam Kod

```typescript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RecaptchaModule, RecaptchaFormsModule } from 'ng-recaptcha';

import { AppRoutingModule } from './app-routing.module';
import { MatDialogModule } from '@angular/material/dialog';
import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { Homepage1Component } from './pages/homepage1/homepage1.component';
import { Homepage2Component } from './pages/homepage2/homepage2.component';
import { Homepage3Component } from './pages/homepage3/homepage3.component';
import { Homepage4Component } from './pages/homepage4/homepage4.component';
import { AboutComponent } from './pages/about/about.component';
import { AddrestaurantComponent } from './pages/addrestaurant/addrestaurant.component';
import { BlogComponent } from './pages/blog/blog.component';
import { BlogdetailsComponent } from './pages/blogdetails/blogdetails.component';
import { Blogstyle2Component } from './pages/blogstyle2/blogstyle2.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { ExdealsComponent } from './pages/exdeals/exdeals.component';
import { GeolocatorComponent } from './pages/geolocator/geolocator.component';
import { ListviewComponent } from './pages/listview/listview.component';
import { LoginComponent } from './pages/login/login.component';
import { OrderdetailsComponent } from './pages/orderdetails/orderdetails.component';
import { RegisterComponent } from './pages/register/register.component';
import { RestaurantComponent } from './pages/restaurant/restaurant.component';
import { Restaurantstyle1Component } from './pages/restaurantstyle1/restaurantstyle1.component';
import { Restaurantstyle2Component } from './pages/restaurantstyle2/restaurantstyle2.component';
import { NavbarComponent } from './layouts/navbar/navbar.component';
import { Footer1Component } from './layouts/footer1/footer1.component';
import { Footer2Component } from './layouts/footer2/footer2.component';
import { AdvertisementbannerComponent } from './layouts/advertisementbanner/advertisementbanner.component';
import { BlogleftsidebarComponent } from './layouts/blogleftsidebar/blogleftsidebar.component';
import { BlogrightsidebarComponent } from './layouts/blogrightsidebar/blogrightsidebar.component';
import { RestaurantleftsidebarComponent } from './layouts/restaurantleftsidebar/restaurantleftsidebar.component';
import { RestaurantrightsidebarComponent } from './layouts/restaurantrightsidebar/restaurantrightsidebar.component';
import { Advertisementbanner1Component } from './layouts/advertisementbanner1/advertisementbanner1.component';
import { Advertisementbanner2Component } from './layouts/advertisementbanner2/advertisementbanner2.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ContactComponent } from './pages/contact/contact.component';
import { ErrorPageComponent } from './pages/error-page/error-page.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    Homepage1Component,
    Homepage2Component,
    Homepage3Component,
    Homepage4Component,
    AboutComponent,
    AddrestaurantComponent,
    BlogComponent,
    BlogdetailsComponent,
    Blogstyle2Component,
    CheckoutComponent,
    ExdealsComponent,
    GeolocatorComponent,
    ListviewComponent,
    LoginComponent,
    OrderdetailsComponent,
    RegisterComponent,
    RestaurantComponent,
    Restaurantstyle1Component,
    Restaurantstyle2Component,
    NavbarComponent,
    Footer1Component,
    Footer2Component,
    AdvertisementbannerComponent,
    BlogleftsidebarComponent,
    BlogrightsidebarComponent,
    RestaurantleftsidebarComponent,
    RestaurantrightsidebarComponent,
    Advertisementbanner1Component,
    Advertisementbanner2Component,
    ContactComponent,
    ErrorPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatDialogModule,
    NgbModule,
    ReactiveFormsModule,
    FormsModule,
    RecaptchaModule,
    RecaptchaFormsModule
  ],
  providers: [
    Location, {
      provide: LocationStrategy,
      useClass: PathLocationStrategy
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## 4. app-routing.module.ts — Tam Kod

```typescript
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { Homepage1Component } from './pages/homepage1/homepage1.component';
import { Homepage2Component } from './pages/homepage2/homepage2.component';
import { Homepage3Component } from './pages/homepage3/homepage3.component';
import { Homepage4Component } from './pages/homepage4/homepage4.component';
import { BlogComponent } from './pages/blog/blog.component';
import { Blogstyle2Component } from './pages/blogstyle2/blogstyle2.component';
import { BlogdetailsComponent } from './pages/blogdetails/blogdetails.component';
import { ExdealsComponent } from './pages/exdeals/exdeals.component';
import { AboutComponent } from './pages/about/about.component';
import { RestaurantComponent } from './pages/restaurant/restaurant.component';
import { Restaurantstyle1Component } from './pages/restaurantstyle1/restaurantstyle1.component';
import { Restaurantstyle2Component } from './pages/restaurantstyle2/restaurantstyle2.component';
import { AddrestaurantComponent } from './pages/addrestaurant/addrestaurant.component';
import { ListviewComponent } from './pages/listview/listview.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrderdetailsComponent } from './pages/orderdetails/orderdetails.component';
import { GeolocatorComponent } from './pages/geolocator/geolocator.component';
import { ContactComponent } from './pages/contact/contact.component';
import { ErrorPageComponent } from './pages/error-page/error-page.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'homepage1', component: Homepage1Component },
  { path: 'homepage2', component: Homepage2Component },
  { path: 'homepage3', component: Homepage3Component },
  { path: 'homepage4', component: Homepage4Component },
  { path: 'blog', component: BlogComponent },
  { path: 'blog-style-2', component: Blogstyle2Component },
  { path: 'blog-details', component: BlogdetailsComponent },
  { path: 'ex-deals', component: ExdealsComponent },
  { path: 'about', component: AboutComponent },
  { path: 'restaurant', component: RestaurantComponent },
  { path: 'restaurant-style-1', component: Restaurantstyle1Component },
  { path: 'restaurant-style-2', component: Restaurantstyle2Component },
  { path: 'add-restaurant', component: AddrestaurantComponent },
  { path: 'listview', component: ListviewComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'orderdetails', component: OrderdetailsComponent },
  { path: 'geolocator', component: GeolocatorComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'error-page', component: ErrorPageComponent },
  { path: '**', component: ErrorPageComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

**Dikkat:** `relativeLinkResolution: 'legacy'` Angular 13+ sürümlerinde kaldırıldı — yeni projede kullanma.

## 5. Pages — Her Sayfa Kodları

### 5.1 HomeComponent

**home.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public closeBanner : boolean | undefined;
  constructor() { }
  toggleBanner(){
    this.closeBanner = !this.closeBanner;
  }
  ngOnInit(): void {
  }
}
```

**home.component.html (yapısal özet):**
```html
<app-navbar></app-navbar>

<!-- Swiper Slider (3 slide) -->
<section class="about-us-slider swiper-container p-relative">
  <div class="swiper-wrapper">
    <div class="swiper-slide slide-item">
      <img src="assets/img/about/blog/1920x700/banner-1.jpg" class="img-fluid full-width" alt="Banner">
      <div class="transform-center">
        <div class="container">
          <div class="row justify-content-start">
            <div class="col-lg-7 align-self-center">
              <div class="right-side-content">
                <h1 class="text-custom-white fw-600">Increase takeout sales by 50%</h1>
                <h3 class="text-custom-white fw-400">with the largest delivery platform in the U.S. and Canada</h3>
                <a routerLink="/restaurant" class="btn-second btn-submit">Learn More.</a>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="overlay overlay-bg"></div>
    </div>
    <!-- 2 slide daha — justify-content-center ve justify-content-end versiyonları -->
  </div>
  <div class="swiper-button-next"></div>
  <div class="swiper-button-prev"></div>
</section>

<!-- Browse by category (Swiper) -->
<section class="browse-cat u-line section-padding">
  <div class="container">
    <div class="row">
      <div class="col-12">
        <div class="section-header-left">
          <h3 class="text-light-black header-title title">Browse by cuisine
            <span class="fs-14"><a routerLink="/restaurant">See all restaurant</a></span>
          </h3>
        </div>
      </div>
      <div class="col-12">
        <div class="category-slider swiper-container">
          <div class="swiper-wrapper">
            <!-- 9 cuisine swiper-slide: Brooklyn, Italian, Thai, Chinese, Mexican, Indian, Lebanese, Japanese, American -->
          </div>
          <div class="swiper-button-next"></div>
          <div class="swiper-button-prev"></div>
        </div>
      </div>
    </div>
  </div>
</section>

<!-- Previous Orders -->
<section class="recent-order section-padding">
  <!-- 4 adet product-box: Chilli Chicken Pizza, Hakka Noodles, Vegan Burger, Sticky Date Cake -->
  <!-- Her birinde routerLink="/restaurant" ve routerLink="/orderdetails" -->
</section>

<!-- Explore collection — 3 reklam banner arasına serpiştirilen ürün grid leri -->
<section class="ex-collection section-padding">
  <div class="container">
    <!-- 2 ex-collection-box (collection-1, collection-2) -->
    <!-- col-lg-3 large-product-box + col-lg-9 6 product-box grid -->
    <app-advertisementbanner></app-advertisementbanner>
    <!-- col-lg-9 6 product-box + col-lg-3 large-product-box -->
    <app-advertisementbanner1></app-advertisementbanner1>
    <!-- col-lg-3 large-product-box + col-lg-9 6 product-box -->
    <app-advertisementbanner2></app-advertisementbanner2>
    <!-- 2 ex-collection-box (collection-3, collection-4) -->
  </div>
</section>

<!-- Dismissible Banner -->
<div class="banner-adv-bg" [ngClass]="closeBanner ? 'd-none':''">
  <div id="banner-adv" class="banner-adv">
    <div class="flex-adv">
      <a href="https://themeforest.net/item/costic-food-dashboard-template/27823635" target="_blank">
        <i class="fas fa-gift"></i>
        <span class="text">Get FREE CRM Dashboard with Quickmunch.</span>
      </a>
      <a href="https://themeforest.net/item/costic-food-dashboard-template/27823635" target="_blank"
         class="btn-second btn-submit">View Dashboard here</a>
    </div>
    <span class="close-banner" (click)="toggleBanner()"></span>
  </div>
</div>

<app-footer1></app-footer1>
```

### 5.2 ContactComponent

**contact.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';
import { ContactService } from 'src/app/helper/contact/contact-helper.service';
import { Contact } from '../../models/contact/contact';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {
  model = new Contact;
  submitted = false;
  error: {} | undefined;
  constructor(private contactService: ContactService) { }
  onSubmit() {
    this.submitted = true;
    return this.contactService.contactForm(this.model).subscribe(
      data => this.model = data,
      error => this.error = error
    );
  };
  resolved(captchaResponse: string) {
    console.log(`Resolved response token: ${captchaResponse}`);
  }
  ngOnInit(): void {
  }
}
```

**contact.component.html:**
```html
<app-navbar></app-navbar>
<section class="section-padding contact-top bg-light-theme">
    <div class="container">
        <div class="row">
            <div class="col-lg-4 col-sm-6">
                <div class="contact-info-box mb-md-40">
                    <i class="fas fa-map-marker-alt"></i>
                    <h6 class="text-theme fw-600">13th Street. 47 W 13th St,<br> New York, NY 10011, USA</h6>
                </div>
            </div>
            <div class="col-lg-4 col-sm-6">
                <div class="contact-info-box mb-md-40">
                    <i class="fas fa-phone-alt"></i>
                    <h6 class="text-theme fw-600"><a href="#" class="text-theme">(+347) 123 4567 890</a><br> Mon-Sat 9:00am-5:00pm</h6>
                </div>
            </div>
            <div class="col-lg-4 col-sm-6">
                <div class="contact-info-box">
                    <i class="fas fa-envelope"></i>
                    <h6 class="text-theme fw-600"><a href="#" class="text-theme">info@domain.com</a><br> 24 X 7 online support</h6>
                </div>
            </div>
        </div>
    </div>
</section>
<section class="section-padding contact-form">
    <div class="container">
        <div class="row">
            <div class="col-12">
                <div class="section-header-left title">
                    <h3 class="text-light-black header-title">Get In Touch</h3>
                </div>
                <form (ngSubmit)="onSubmit()" #contactForm="ngForm">
                    <div class="row clearfix">
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" name="name" [(ngModel)]="model.name"
                                    class="form-control form-control-submit" placeholder="Name" required>
                            </div>
                            <div class="form-group">
                                <input type="email" name="email" [(ngModel)]="model.email"
                                    class="form-control form-control-submit" placeholder="Email" required>
                            </div>
                            <div class="form-group">
                                <input type="text" name="phone" [(ngModel)]="model.phone"
                                    class="form-control form-control-submit" placeholder="Phone No." required>
                            </div>
                            <div class="form-group">
                                <input type="text" name="subject" [(ngModel)]="model.subject"
                                    class="form-control form-control-submit" placeholder="Subject" required>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <textarea name="message" [(ngModel)]="model.message"
                                    class="form-control form-control-submit" placeholder="Message" required></textarea>
                            </div>
                        </div>
                        <div class="col-12">
                            <re-captcha (resolved)="resolved($event)" name="recaptcha" [(ngModel)]="model.recaptcha"
                                siteKey="6LdxUhMaAAAAAIrQt-_6Gz7F_58S4FlPWaxOh5ib" size="invisible"></re-captcha>
                            <button type="submit" class="btn-second btn-submit"
                                [disabled]="!contactForm.form.valid">Submit</button>
                            <div class="service-error mt-4 w-100" *ngIf="error">
                                <ngb-alert type="danger" class="mb-0 w-100" [dismissible]="false">
                                    <strong>Oops!</strong> Something bad happened. Please try again later.
                                </ngb-alert>
                            </div>
                            <div [hidden]="!submitted" class="contact-message w-100">
                                <div *ngIf="model.id" class="contact-success w-100 mt-4">
                                    <ngb-alert type="success" class="mb-0 w-100" [dismissible]="false">
                                        <strong>Success!</strong> Contact form has been successfully submitted.
                                    </ngb-alert>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</section>
<div class="contact-map">
    <iframe
        src="https://maps.google.com/maps?q=university%20of%20san%20francisco&amp;t=&amp;z=13&amp;ie=UTF8&amp;iwloc=&amp;output=embed"></iframe>
</div>
<app-footer2></app-footer2>
```

### 5.3 Diğer Sayfalar — Sayfa / Layout Eşleştirmesi

| Sayfa | Kullandığı Layout Bileşenleri |
|---|---|
| home | navbar, footer1, advertisementbanner x3 |
| homepage1-4 | navbar, footer1 veya footer2 |
| blog | navbar, blogleftsidebar, blogrightsidebar, footer2 |
| blogdetails | navbar, blogleftsidebar, footer2 |
| blogstyle2 | navbar, blogleftsidebar, footer2 |
| restaurant | navbar, restaurantleftsidebar, restaurantrightsidebar, footer1 |
| restaurantstyle1 | navbar, restaurantleftsidebar, footer1 |
| restaurantstyle2 | navbar, restaurantrightsidebar, footer1 |
| contact | navbar, footer2 |
| about | navbar, footer1 |
| login, register | navbar, footer2 |
| checkout, orderdetails | navbar, footer1 |
| geolocator | navbar, footer1 |
| listview | navbar, footer1 |
| addrestaurant | navbar, footer1 |
| exdeals | navbar, footer1 |
| error-page | navbar, footer1 |

## 6. Layouts — Her Layout Kodu

### 6.1 NavbarComponent

**navbar.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  constructor() { }
  ngOnInit(): void {
  }
}
```

**navbar.component.html (yapısal özet):**
```html
<div class="header">
  <header class="full-width">
    <div class="container-fluid">
      <div class="row">
        <div class="col-12 mainNavCol">
          <!-- Logo -->
          <div class="logo mainNavCol">
            <a routerLink="/"><img src="assets/img/logo.png" class="img-fluid" alt="Logo"></a>
          </div>
          <!-- Arama Formu — konum + yemek adı -->
          <div class="main-search mainNavCol">
            <form class="main-search search-form full-width">
              <div class="row">
                <div class="col-lg-6 col-md-5">
                  <a href="javascript:void(0)" class="delivery-add p-relative">
                    <span class="icon"><i class="fas fa-map-marker-alt"></i></span>
                    <span class="address">Brooklyn, NY</span>
                  </a>
                  <div class="location-picker">
                    <input type="text" class="form-control" placeholder="Enter a new address">
                  </div>
                </div>
                <div class="col-lg-6 col-md-7">
                  <div class="search-box padding-10">
                    <input type="text" class="form-control" placeholder="Pizza, Burger, Chinese">
                  </div>
                </div>
              </div>
            </form>
          </div>
          <!-- Sağ grup: Mega Menu + Kullanıcı + Bildirim + Sepet -->
          <div class="right-side fw-700 mainNavCol">
            <!-- Mega Menu (Pages dropdown) -->
            <div class="catring parent-megamenu">
              <a href="javascript:void(0)"><span>Pages <i class="fas fa-caret-down"></i></span></a>
              <div class="megamenu">
                <div class="row">
                  <div class="col-sm-12">
                    <div class="row">
                      <!-- 4 sütun: Home Pages | Inner Pages | Related Pages | Additional Pages -->
                      <!-- Her biri routerLink + routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" -->
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <!-- Kullanıcı dropdown -->
            <div class="user-details p-relative">
              <a href="javascript:void(0)" class="text-light-white fw-500">
                <img src="assets/img/user-1.png" class="rounded-circle" alt="userimg"> <span>Hi, Kate</span>
              </a>
              <div class="user-dropdown">
                <ul>
                  <li><a routerLink="/orderdetails">Past Orders</a></li>
                  <li><a routerLink="/orderdetails">Upcoming Orders</a></li>
                  <!-- Saved, Gift cards, Refer a friend, Perks, Account, Help -->
                </ul>
                <div class="user-footer"><span class="text-light-black">Not Jhon?</span> <a href="javascript:void(0)">Sign Out</a></div>
              </div>
            </div>
            <!-- Bildirim -->
            <div class="cart-btn notification-btn">
              <a href="javascript:void(0)" class="text-light-green fw-700">
                <i class="fas fa-bell"></i>
                <span class="user-alert-notification"></span>
              </a>
              <!-- notification-dropdown içinde son sipariş rating kutusu -->
            </div>
            <!-- Sepet -->
            <div class="cart-btn cart-dropdown">
              <a href="javascript:void(0)" class="text-light-green fw-700">
                <i class="fas fa-shopping-bag"></i>
                <span class="user-alert-cart">3</span>
              </a>
              <div class="cart-detail-box">
                <div class="card">
                  <div class="card-header padding-15">Your Order</div>
                  <div class="card-body no-padding">
                    <!-- 3 statik ürün: Chilli Chicken, loaded cheese, Tortia Chicken, hepsi $2.25 -->
                    <div class="item-total">
                      <div class="total-price border-0">
                        <span class="text-dark-white fw-700">Items subtotal:</span>
                        <span class="text-dark-white fw-700">$9.99</span>
                      </div>
                      <div class="empty-bag padding-15"><a href="javascript:void(0)">Empty bag</a></div>
                    </div>
                  </div>
                  <div class="card-footer padding-15">
                    <a routerLink="/checkout" class="btn-first green-btn text-custom-white full-width fw-500">Proceed to Checkout</a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <!-- Mobile search row -->
        <div class="col-sm-12 mobile-search">
          <div class="mobile-address">
            <a href="javascript:void(0)" class="delivery-add" data-toggle="modal" data-target="#address-box">
              <span class="address">Brooklyn, NY</span>
            </a>
          </div>
          <div class="sorting-addressbox">
            <span class="full-address text-light-green">Brooklyn, NY 10041</span>
            <div class="btns">
              <div class="filter-btn">
                <button type="button"><i class="fas fa-sliders-h text-light-green fs-18"></i></button>
                <span class="text-light-green">Sort</span>
              </div>
              <div class="filter-btn">
                <button type="button"><i class="fas fa-filter text-light-green fs-18"></i></button>
                <span class="text-light-green">Filter</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </header>
</div>
<div class="main-sec"></div>
```

### 6.2 Footer1Component

```typescript
// footer1.component.ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-footer1',
  templateUrl: './footer1.component.html',
  styleUrls: ['./footer1.component.css']
})
export class Footer1Component implements OnInit {
  constructor() { }
  ngOnInit(): void {
  }
}
```

**footer1.component.html (yapısal özet):**
```html
<!-- footer-top: 6 icon kutusu -->
<div class="footer-top section-padding bg-black">
  <div class="container-fluid">
    <div class="row">
      <!-- 100% Payment Secured | Support lots of Payments | 24 hours/7 days Support |
           Free Delivery with $50 | Best Price Guaranteed | Mobile Apps Ready -->
    </div>
  </div>
</div>

<!-- footer: Instagram slider + 6 sütun link + sosyal medya -->
<footer class="section-padding bg-light-theme pt-0 u-line bg-black">
  <div class="u-line instagram-slider swiper-container">
    <ul class="hm-list hm-instagram swiper-wrapper">
      <!-- 8 instagram görseli: insta-1.jpg ... insta-8.jpg -->
    </ul>
  </div>
  <div class="container-fluid">
    <div class="row">
      <!-- Need Help | Get to Know Us | Let Us Help You | Doing Business | Download Apps | Newsletter -->
      <!-- Newsletter subscribe formu -->
      <!-- Sosyal medya: Facebook, Twitter, Instagram, Pinterest, YouTube -->
    </div>
  </div>
</footer>

<!-- copyright: Ödeme logoları + "Made with Love" + © metni -->
<div class="copyright bg-black">
  <div class="container-fluid">
    <div class="row">
      <div class="col-lg-4">
        <!-- Visa, Mastercard, Amex logoları -->
      </div>
      <div class="col-lg-4 text-center medewithlove align-self-center">
        <a href="https://metropolitanhost.com/" class="text-custom-white">Made with Real <i class="fas fa-heart"></i> Metropolitanthemes</a>
      </div>
      <div class="col-lg-4">
        <div class="copyright-text">
          <span class="text-light-white">© Metropolitanthemes - 2022 | All Right Reserved</span>
        </div>
      </div>
    </div>
  </div>
</div>
```

### 6.3 Advertisement Banner Bileşenleri

Üç adet bağımsız layout bileşeni (advertisementbanner, advertisementbanner1, advertisementbanner2) — hepsi statik HTML içerir, @Input / @Output yoktur. Home sayfasında ürün gridleri arasına yerleştirilir.

## 7. Contact Backend Entegrasyonu — Tam Kod

Bu projede emailJS kullanılmamaktadır. Contact formu `metropolitanhost.com/scripts/sendmail.php` adresine POST atar.

### Contact Model

```typescript
// src/app/models/contact/contact.ts
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

### ContactService

```typescript
// src/app/helper/contact/contact-helper.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Contact } from '../../models/contact/contact';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  ServerUrl = 'https://metropolitanhost.com/scripts/sendmail.php';
  errorData: {} | undefined;

  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };

  constructor( private http: HttpClient ) { }

  contactForm(formdata: Contact) {
    return this.http.post<Contact>(this.ServerUrl, formdata, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      console.error('An error occurred:', error.error.message);
    } else {
      console.error(`Backend returned code ${error.status}, body was: ${error.error}`);
    }
    this.errorData = {
      errorTitle: 'Oops! Request for document failed',
      errorDesc: 'Something bad happened. Please try again later.'
    };
    return throwError(this.errorData);
  }
}
```

**Veri akışı:**
1. `ContactComponent.onSubmit()` → `submitted = true`
2. `ContactService.contactForm(model)` → HTTP POST → PHP mailer endpoint
3. Başarı: `model = data` (backend id döndürürse ngb-alert success gösterilir)
4. Hata: `error = error` (ngb-alert danger gösterilir)

## 8. reCAPTCHA Kullanımı — Tam Kod

Projede **invisible reCAPTCHA** kullanılmaktadır.

**Modül kaydı (app.module.ts):**
```typescript
import { RecaptchaModule, RecaptchaFormsModule } from 'ng-recaptcha';
// imports[]'a eklenen:
RecaptchaModule,
RecaptchaFormsModule
```

**Template kullanımı:**
```html
<re-captcha
  (resolved)="resolved($event)"
  name="recaptcha"
  [(ngModel)]="model.recaptcha"
  siteKey="6LdxUhMaAAAAAIrQt-_6Gz7F_58S4FlPWaxOh5ib"
  size="invisible">
</re-captcha>
```

**Component metodu:**
```typescript
resolved(captchaResponse: string) {
  console.log(`Resolved response token: ${captchaResponse}`);
}
```

- `size="invisible"` — kullanıcı görmez, form submit sırasında otomatik tetiklenir
- Token `model.recaptcha` alanına bağlanır ve POST body içinde sunucuya gönderilir
- Gerçek doğrulama PHP backend tarafında yapılır

## 9. Environment Dosyaları

```typescript
// src/environments/environment.ts
export const environment = {
  production: false
};
```

**Dikkat:** Environment dosyası son derece minimal. API URL, reCAPTCHA siteKey gibi değerler environment'a taşınmamış; doğrudan service ve HTML template içine hardcoded olarak yazılmış. Bu önemli bir teknik borçtur.

## 10. Mimari Kararlar ve Dikkat Noktaları

### Mimari Kararlar

1. **Tek modül (AppModule) mimarisi** — Feature module veya lazy loading yoktur. Tüm component'lar tek AppModule içinde declare edilmiştir. Demo/template proje için kabul edilebilir ama büyük uygulamalarda ölçeklenemez.

2. **PathLocationStrategy** — Hash-based URL yerine path-based URL kullanılmış (`/contact`, `/restaurant`). Production'da server-side URL rewriting (nginx rewrite veya Apache .htaccess) gerektirir.

3. **Layout bileşenleri her sayfaya gömülü** — Her page kendi `<app-navbar>` ve `<app-footer1/2>` selector'larını template içine koyar. Ortak layout wrapper (router-outlet ile tek bir shell component) yoktur.

4. **Tamamen statik veri** — Navbar kullanıcı adı ("Hi, Kate"), sepet içeriği ($9.99), ürün listesi hepsi hardcoded HTML'dir. Gerçek API bağlantısı yoktur; bu bir UI tema template'idir.

5. **Swiper.js vanilla JS entegrasyonu** — `assets/js/` klasöründen script tag ile yüklenir. Angular paketi (ngx-swiper-wrapper vb.) kullanılmamıştır. Angular change detection ile çakışma riski var.

6. **3 ayrı Advertisement Banner component** — advertisementbanner, advertisementbanner1, advertisementbanner2. Tek parametrik (@Input ile) bir component daha temiz olurdu.

7. **providedIn: 'root' servis** — ContactService root seviyesinde sağlanmış, doğru pratik.

### Dikkat Noktaları

- `relativeLinkResolution: 'legacy'` — Angular 13+ sürümlerinde kaldırıldı; yeni projede kullanma
- reCAPTCHA `siteKey` HTML template içinde hardcoded — `environment.ts`'e taşı
- `ServerUrl` service içinde hardcoded — `environment.ts`'e taşı
- Contact form başarı kontrolü `model.id` varlığına bağlı — backend mutlaka id döndürmeli, kırılgan tasarım
- `private handleError(error)` metodu: `this.errorData` erişiminde `this` bağlamı kaybolabilir — arrow function kullan
- Navbar ve footer state'i tamamen statik — gerçek uygulamada auth service + store ile yönetilmeli
- `throwError(this.errorData)` — RxJS 7+ sürümlerinde observable factory gerekir: `throwError(() => this.errorData)`
