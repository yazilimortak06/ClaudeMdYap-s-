# qr_menu_mvc — Klasör Yapısı

## Genel Bakış
ASP.NET MVC 5 + .NET Framework 4.7.2 tabanlı QR kod menü sistemi. Database-first Entity Framework 6 kullanılmaktadır. Veri erişim katmanı `DBservices` klasöründe ayrı tutulmuştur.

---

## Ana Proje Yapısı

```
QrMenu/
│
├── PixDinn/                        — Ana MVC web uygulaması
│   │
│   ├── App_Start/
│   │   ├── BundleConfig.cs         — CSS/JS bundle ve minification
│   │   ├── FilterConfig.cs         — Global action filter'lar
│   │   ├── RouteConfig.cs          — URL routing konfigürasyonu
│   │   └── WebApiConfig.cs         — Web API routing (ApiController'lar için)
│   │
│   ├── Controllers/
│   │   ├── AdminLoginController.cs     — Yönetici girişi ve çıkışı
│   │   ├── AdminMekanController.cs     — Mekan (restoran) CRUD işlemleri
│   │   ├── AdminKategoriController.cs  — Menü kategorisi CRUD
│   │   ├── MenuController.cs           — Menü görüntüleme ve yönetimi
│   │   ├── ProductController.cs        — Ürün (yemek) CRUD
│   │   ├── RaporController.cs          — Raporlama ekranları
│   │   ├── AdminUsersController.cs     — Yönetici kullanıcı yönetimi
│   │   └── MenuPublisController.cs     — Menü yayınlama ve QR oluşturma
│   │
│   ├── Models/
│   │   ├── [EF Database-First Entities]  — EDMX'ten otomatik üretilen modeller
│   │   │   ├── QrMenuEntities.cs         — EF ObjectContext / DbContext
│   │   │   ├── Mekan.cs
│   │   │   ├── Menu.cs
│   │   │   ├── Kategori.cs
│   │   │   ├── Urun.cs
│   │   │   └── AdminUser.cs
│   │   └── ViewModels/
│   │       ├── LoginViewModel.cs
│   │       ├── MekanViewModel.cs
│   │       ├── MenuViewModel.cs
│   │       └── UrunViewModel.cs
│   │
│   ├── Views/
│   │   ├── AdminLogin/
│   │   │   └── Index.cshtml            — Giriş formu
│   │   ├── AdminMekan/
│   │   │   ├── Index.cshtml            — Mekan listesi
│   │   │   ├── Create.cshtml
│   │   │   └── Edit.cshtml
│   │   ├── AdminKategori/
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   └── Edit.cshtml
│   │   ├── Menu/
│   │   │   ├── Index.cshtml            — Menü listesi (admin)
│   │   │   └── Public.cshtml           — QR ile erişilen public menü
│   │   ├── Product/
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   └── Edit.cshtml
│   │   ├── Rapor/
│   │   │   └── Index.cshtml
│   │   └── Shared/
│   │       ├── _Layout.cshtml          — Admin panel layout
│   │       ├── _PublicLayout.cshtml    — Public menü layout (QR sayfası)
│   │       └── Error.cshtml
│   │
│   ├── Content/
│   │   ├── css/
│   │   │   ├── bootstrap.css
│   │   │   ├── site.css
│   │   │   └── menu.css                — QR menü özel stilleri
│   │   └── images/
│   │       └── default-product.png
│   │
│   ├── Scripts/
│   │   ├── jquery-3.x.min.js
│   │   ├── bootstrap.js
│   │   └── menu-public.js              — Public menü JavaScript
│   │
│   ├── QrMenuDB.edmx                   — Database-first EF model
│   ├── Global.asax
│   ├── Global.asax.cs
│   └── Web.config
│
└── DBservices/                         — Veri erişim katmanı
    │
    ├── MekanDBService.cs               — Mekan sorgu/komut metodları
    ├── MenuDBService.cs                — Menü sorgu/komut metodları
    ├── KategoriDBService.cs            — Kategori sorgu/komut metodları
    ├── UrunDBService.cs                — Ürün sorgu/komut metodları
    ├── AdminUserDBService.cs           — Admin kullanıcı işlemleri
    ├── RaporDBService.cs               — Raporlama sorguları
    └── packages.config                 — NuGet bağımlılıkları
```

---

## Veritabanı Tabloları (Tahmini)

```
[dbo].[Mekanlar]
├── Id (PK)
├── Ad
├── MekanKod (QR URL için unique slug)
├── Adres
├── Telefon
├── LogoPath
└── IsActive

[dbo].[Kategoriler]
├── Id (PK)
├── MekanId (FK → Mekanlar)
├── Ad
├── SiraNo
└── IsActive

[dbo].[Menuler]
├── Id (PK)
├── MekanId (FK → Mekanlar)
├── Ad
├── Aciklama
└── IsActive

[dbo].[Urunler]
├── Id (PK)
├── KategoriId (FK → Kategoriler)
├── MenuId (FK → Menuler)
├── Ad
├── Aciklama
├── Fiyat
├── ResimPath
├── SiraNo
└── IsActive

[dbo].[AdminUsers]
├── Id (PK)
├── MekanId (FK → Mekanlar)
├── Username
├── PasswordHash
└── IsActive
```

---

## Genel Notlar

| Bileşen | Açıklama |
|---------|---------|
| `PixDinn/` | MVC uygulama projesi — web arayüzü |
| `DBservices/` | Veri erişim katmanı — Controller'lar bu servisleri kullanır |
| `App_Start/` | Routing, bundle ve filter konfigürasyonları |
| `QrMenuDB.edmx` | Database-first EF modeli — değiştirilmemelidir |
| `Web.config` | Connection string ve uygulama ayarları (hassas bilgi içerir) |
| `Content/` ve `Scripts/` | Static dosyalar — Bundle'a dahil edilir |
