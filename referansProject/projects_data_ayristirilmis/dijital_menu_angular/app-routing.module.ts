import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Pages
import { HomeComponent } from './pages/home/home.component';
import { Homepage1Component } from './pages/homepage1/homepage1.component';
import { Homepage2Component } from './pages/homepage2/homepage2.component';
import { Homepage3Component } from './pages/homepage3/homepage3.component';
import { Homepage4Component } from './pages/homepage4/homepage4.component';
import { ListviewComponent } from './pages/listview/listview.component';
import { RestaurantComponent } from './pages/restaurant/restaurant.component';
import { Restaurantstyle1Component } from './pages/restaurantstyle1/restaurantstyle1.component';
import { Restaurantstyle2Component } from './pages/restaurantstyle2/restaurantstyle2.component';
import { AddrestaurantComponent } from './pages/addrestaurant/addrestaurant.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrderdetailsComponent } from './pages/orderdetails/orderdetails.component';
import { ExdealsComponent } from './pages/exdeals/exdeals.component';
import { GeolocatorComponent } from './pages/geolocator/geolocator.component';
import { BlogComponent } from './pages/blog/blog.component';
import { BlogdetailsComponent } from './pages/blogdetails/blogdetails.component';
import { Blogstyle2Component } from './pages/blogstyle2/blogstyle2.component';
import { AboutComponent } from './pages/about/about.component';
import { ContactComponent } from './pages/contact/contact.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ErrorPageComponent } from './pages/error-page/error-page.component';

// Eager loading — tüm routes doğrudan component'e bağlı
// PathLocationStrategy kullanılıyor (HTML5 history mode)
const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'homepage1', component: Homepage1Component },
  { path: 'homepage2', component: Homepage2Component },
  { path: 'homepage3', component: Homepage3Component },
  { path: 'homepage4', component: Homepage4Component },
  { path: 'listview', component: ListviewComponent },
  { path: 'restaurant', component: RestaurantComponent },
  { path: 'restaurantstyle1', component: Restaurantstyle1Component },
  { path: 'restaurantstyle2', component: Restaurantstyle2Component },
  { path: 'addrestaurant', component: AddrestaurantComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'orderdetails', component: OrderdetailsComponent },
  { path: 'exdeals', component: ExdealsComponent },
  { path: 'geolocator', component: GeolocatorComponent },
  { path: 'blog', component: BlogComponent },
  { path: 'blogdetails', component: BlogdetailsComponent },
  { path: 'blogstyle2', component: Blogstyle2Component },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '**', component: ErrorPageComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
