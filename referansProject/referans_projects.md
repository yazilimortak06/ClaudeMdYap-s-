# Referans Projeler

Bu dosya tüm referans projelerin master listesidir.
Ham klasörler `projects_data/<ProjeAdı>/` altındadır.
Her proje için detay: `referansProject/<anonim>/analiz.md` ve `rules.md`

**Base Projeler:** `projects_data_base_hali/angular_base/` ve `projects_data_base_hali/dotnet_base/`

---

## Base Projeler

### angular_base
- **Baz Alınan:** sarj_qr_web_angular
- **Platform:** Web - Angular 13
- **Tech Stack:** Angular 13, Material 11, @auth0/angular-jwt, SignalR, ApexCharts, jsPDF, xlsx, lottie
- **Mimari:** Feature Modules, Core/Shared ayrımı, Guard + Interceptor
- **Klasör:** projects_data_base_hali/angular_base/
- **Açıklama:** Yeni Angular projelerine başlangıç noktası. Auth, routing, interceptor, guard, shared components hazır.

### dotnet_base
- **Baz Alınan:** sarj_backend_dotnet
- **Platform:** Backend - .NET 5
- **Tech Stack:** .NET 5, Autofac, AutoMapper, EF Core 5 (SQL Server), MassTransit+RabbitMQ, FluentValidation, Swagger, Refit, SignalR
- **Mimari:** FrameworkCore + Layered (Presentation/Application/Persistence) + Microservice-ready
- **Klasör:** projects_data_base_hali/dotnet_base/
- **Açıklama:** Yeni .NET backend projelerine başlangıç noktası. FrameworkCore, Autofac DI, docker-compose, API versioning hazır.

---

## Tüm Referans Projeler

### sarj_ev_panel_angular
- **Platform:** Web - Angular 13 Admin Panel
- **Tech Stack:** Angular 13, Material 11, @auth0/angular-jwt, SignalR, ApexCharts, Metronic template
- **Tip:** Admin Panel (Lazy Loading, AuthGuard, HTTP Interceptor)
- **Mimari:** Core/Shared_Admin/Feature (28 kategori)
- **Sektör:** EV Şarj
- **Ham Kaynak:** projects_data/EvTechPanelAltunkaya/
- **Analiz:** referansProject/sarj_ev_panel_angular/analiz.md
- **Rules:** referansProject/sarj_ev_panel_angular/rules.md
- **Özet:** EV şarj yönetim admin paneli. Metronic template, HashLocationStrategy, 4 environment, JWT localStorage auth.

### crm_backend
- **Platform:** Backend - .NET 5 Microservice
- **Tech Stack:** .NET 5, EF Core, MassTransit+RabbitMQ, SQL Server, MongoDB (log)
- **Tip:** Microservice Backend
- **Mimari:** Layered (Domain/Application/Persistence/API) + Microservices + Event-driven
- **Sektör:** CRM
- **Ham Kaynak:** projects_data/PixdinnCrm/
- **Analiz:** referansProject/crm_backend/analiz.md
- **Rules:** referansProject/crm_backend/rules.md
- **Özet:** CRM backend. 6 microservice: main API, log, notification, file, worker, token. RabbitMQ event bus.

### dijital_menu_angular
- **Platform:** Web - Angular 11 Public Site
- **Tech Stack:** Angular 11, Material 9, @ng-bootstrap, emailjs-com, ng-recaptcha
- **Tip:** Public Website (Eager Loading, No Auth)
- **Mimari:** Single Module (Pages/Layouts ayrımı)
- **Sektör:** Restoran/Yemek
- **Ham Kaynak:** projects_data/PixdinnNewMenu/
- **Analiz:** referansProject/dijital_menu_angular/analiz.md
- **Rules:** referansProject/dijital_menu_angular/rules.md
- **Özet:** Restoran listeleme ve menü public website. emailJS ile backendless form, reCAPTCHA, tek modül yapısı.

### perde_iot_backend
- **Platform:** Backend - .NET 8 MVC
- **Tech Stack:** .NET 8, Autofac 6.3, AutoMapper 11, Docker
- **Tip:** Monolithic MVC
- **Mimari:** MVC + BaseStartup inheritance + Service/ServiceInterface
- **Sektör:** IoT/Perde Kontrol
- **Ham Kaynak:** projects_data/PixdinnPerdeci/
- **Analiz:** referansProject/perde_iot_backend/analiz.md
- **Rules:** referansProject/perde_iot_backend/rules.md
- **Özet:** IoT perde kontrol sistemi. .NET 8, Autofac dinamik DLL yükleme, BaseStartup pattern.

### yonetim_panel_angular
- **Platform:** Web - Angular 13 Admin Panel
- **Tech Stack:** Angular 13, Material 11, @auth0/angular-jwt, SignalR, ApexCharts, Metronic template
- **Tip:** Admin Panel (Lazy Loading, AuthGuard, HTTP Interceptor)
- **Mimari:** Core/Shared_Admin/Feature (6 kategori)
- **Sektör:** Restoran/Yönetim
- **Ham Kaynak:** projects_data/PixdinnYonetimPanel/
- **Analiz:** referansProject/yonetim_panel_angular/analiz.md
- **Rules:** referansProject/yonetim_panel_angular/rules.md
- **Özet:** Restoran yönetim paneli. sarj_ev_panel_angular ile aynı template, daha az feature kategorisi.

### qr_menu_mvc
- **Platform:** Web - ASP.NET MVC 5
- **Tech Stack:** .NET Framework 4.7.2, ASP.NET MVC 5, EF 6.4.0, Web API
- **Tip:** Monolithic MVC (Legacy)
- **Mimari:** Traditional MVC + DBservices katmanı
- **Sektör:** Restoran/QR Menü
- **Ham Kaynak:** projects_data/QrMenu/
- **Analiz:** referansProject/qr_menu_mvc/analiz.md
- **Rules:** referansProject/qr_menu_mvc/rules.md
- **Özet:** Eski QR menü sistemi. Database-first EF6, MVC 5 pattern. Modernizasyon referansı.

### restoran_panel_angular
- **Platform:** Web - Angular 14 Scaffold
- **Tech Stack:** Angular 14, TypeScript strict
- **Tip:** Boş Scaffold (Başlangıç Noktası)
- **Mimari:** Angular 14 minimal setup
- **Sektör:** Restoran
- **Ham Kaynak:** projects_data/RestaurantSystemPanel/
- **Analiz:** referansProject/restoran_panel_angular/analiz.md
- **Rules:** referansProject/restoran_panel_angular/rules.md
- **Özet:** Angular 14 ile başlatılmış boş proje. Strict mode aktif, modern Angular başlangıç referansı.

### sarj_backend_dotnet
- **Platform:** Backend - .NET 5 Microservice
- **Tech Stack:** .NET 5, Autofac, AutoMapper, EF Core 5, MassTransit+RabbitMQ, FluentValidation, Swagger, Refit, SignalR, Docker
- **Tip:** Microservice Backend (20+ API)
- **Mimari:** FrameworkCore + Layered (Presentation/Application/Persistence) + Shared.Domain + FirmIntegration
- **Sektör:** EV Şarj
- **Ham Kaynak:** projects_data/RotaWattBackEnd/
- **Analiz:** referansProject/sarj_backend_dotnet/analiz.md
- **Rules:** referansProject/sarj_backend_dotnet/rules.md
- **Özet:** En kapsamlı .NET backend. 20+ microservice API, özel FrameworkCore, InnerRequestAttribute pattern, 19 Docker servisi.

### sarj_qr_web_angular
- **Platform:** Web - Angular 13 Portal
- **Tech Stack:** Angular 13, Material 11, SignalR, jsPDF, xlsx, lottie-web, ngx-image-cropper, ng2-currency-mask
- **Tip:** User Portal (Feature Modules, Custom UI Bileşenleri)
- **Mimari:** Core/Modules ayrımı, custom UI component seti
- **Sektör:** EV Şarj
- **Ham Kaynak:** projects_data/rotawattqrweb-master/
- **Analiz:** referansProject/sarj_qr_web_angular/analiz.md
- **Rules:** referansProject/sarj_qr_web_angular/rules.md
- **Özet:** EV şarj kullanıcı portalı ve QR web uygulaması. Custom UI seti, PDF/Excel export, SignalR real-time.

### sarj_vm_backend_dotnet
- **Platform:** Backend - .NET Microservice
- **Tech Stack:** .NET, FrameworkCore, Ocelot API Gateway, MassTransit+RabbitMQ, Docker (4 env)
- **Tip:** Microservice Backend (VM Odaklı, 4 Servis)
- **Mimari:** FrameworkCore + Layered + API Gateway (Ocelot)
- **Sektör:** EV Şarj / VM Yönetimi
- **Ham Kaynak:** projects_data/rotawattvmbackend-develop/
- **Analiz:** referansProject/sarj_vm_backend_dotnet/analiz.md
- **Rules:** referansProject/sarj_vm_backend_dotnet/rules.md
- **Özet:** VM bazlı şarj yönetim backend. Ocelot API Gateway, 4 environment (Dev/Local/Test/Prod), OCPP protokolü.

### sarj_pro_backend_dotnet
- **Platform:** Backend - .NET 5 Microservice
- **Tech Stack:** sarj_backend_dotnet ile aynı
- **Tip:** Microservice Backend (13+ API) — sarj_backend_dotnet fork'u
- **Mimari:** sarj_backend_dotnet ile aynı mimari
- **Sektör:** EV Şarj
- **Ham Kaynak:** projects_data/SarjAllPro/
- **Analiz:** referansProject/sarj_pro_backend_dotnet/analiz.md
- **Rules:** referansProject/sarj_pro_backend_dotnet/rules.md
- **Özet:** sarj_backend_dotnet'in fork'u. Daha az API, farklı payment config (MOKA aktif), aynı FrameworkCore.

### sarj_pro_panel_angular
- **Platform:** Web - Angular 13 Admin Panel
- **Tech Stack:** sarj_ev_panel_angular ile aynı (Metronic template)
- **Tip:** Admin Panel — sarj_ev_panel_angular fork'u (8 kategori)
- **Mimari:** Core/Shared_Admin/Feature
- **Sektör:** EV Şarj
- **Ham Kaynak:** projects_data/SarjAllProPanel/
- **Analiz:** referansProject/sarj_pro_panel_angular/analiz.md
- **Rules:** referansProject/sarj_pro_panel_angular/rules.md
- **Özet:** Şarj platform yönetim paneli. sarj_ev_panel_angular'ın daha az feature'lı, daha zengin model/enum katmanlı versiyonu.
