# qr_menu_mvc — Analiz

## Platform ve Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | ASP.NET MVC 5 |
| .NET Platformu | .NET Framework 4.7.2 |
| ORM | Entity Framework 6.4.0 |
| DB Yaklaşımı | Database-first (edmx modeli) |
| API | ASP.NET Web API (aynı proje) |
| Konfigürasyon | Web.config |
| Bağımlılık Yönetimi | packages.config / NuGet |

## Genel Bakış

`qr_menu_mvc` (QrMenu), restoran için QR kod menü sistemidir. ASP.NET MVC 5 ve .NET Framework 4.7.2 ile geliştirilmiş eski nesil bir web uygulamasıdır. Veritabanı erişimi için Entity Framework 6 database-first yaklaşımı kullanılmaktadır.

## Proje Yapısı

```
QrMenu/
├── PixDinn/                    — Ana MVC uygulaması
│   ├── Controllers/
│   │   ├── AdminLoginController.cs
│   │   ├── AdminMekanController.cs
│   │   ├── AdminKategoriController.cs
│   │   ├── MenuController.cs
│   │   ├── ProductController.cs
│   │   ├── RaporController.cs
│   │   ├── AdminUsersController.cs
│   │   └── MenuPublisController.cs
│   ├── Models/
│   │   ├── [Entity Models — EF Database-first]
│   │   └── ViewModels/
│   ├── Views/
│   │   ├── AdminLogin/
│   │   ├── AdminMekan/
│   │   ├── AdminKategori/
│   │   ├── Menu/
│   │   ├── Product/
│   │   ├── Rapor/
│   │   └── Shared/
│   ├── App_Start/
│   │   ├── BundleConfig.cs
│   │   ├── FilterConfig.cs
│   │   └── RouteConfig.cs
│   └── Web.config
└── DBservices/                 — Veri erişim katmanı
    ├── [Service sınıfları]
    └── packages.config
```

## Kontrolcü Analizi

| Kontrolcü | Sorumluluk |
|---|---|
| AdminLoginController | Yönetici girişi ve oturum yönetimi |
| AdminMekanController | Mekan (restoran) yönetimi — CRUD |
| AdminKategoriController | Menü kategorisi yönetimi |
| MenuController | Menü listeleme ve yönetimi |
| ProductController | Ürün (yemek) yönetimi |
| RaporController | Raporlama sayfaları |
| AdminUsersController | Kullanıcı yönetimi |
| MenuPublisController | Menü yayınlama / QR oluşturma |

## Database-First Entity Framework

EF 6 database-first yaklaşımı: veritabanı şeması önce tanımlanır, ardından `edmx` modeli oluşturulur ve entity sınıfları otomatik üretilir. Bu yaklaşım:

**Avantajlar:**
- Var olan veritabanıyla hızlı entegrasyon
- DBA'nın schema kontrolü korunur
- Görsel EDMX tasarımcısı

**Dezavantajlar:**
- Kod migration desteği yok (Code-First migrasyonlar)
- Otomatik üretilen kod değiştirilemez (regenerate olur)
- .NET Core ile uyumsuz (EF 6 sadece .NET Framework)

## DBservices Katmanı

`DBservices/` klasörü, EF context üzerinden veri erişimini saran bir servis katmanıdır. Controller'lar doğrudan EF context kullanmak yerine bu servis sınıflarını çağırır. Repository pattern'in basit bir versiyonu olarak düşünülebilir.

```csharp
// Tipik DBservice pattern
public class MenuDBService
{
    private readonly QrMenuEntities _context;

    public MenuDBService()
    {
        _context = new QrMenuEntities();
    }

    public List<Menu> GetAllMenus(int mekanId)
    {
        return _context.Menus
                       .Where(m => m.MekanId == mekanId && m.IsActive)
                       .OrderBy(m => m.SiraNo)
                       .ToList();
    }
}
```

## Web API Entegrasyonu

Aynı proje hem MVC hem de Web API içerebilir. QR kod okutulduğunda JavaScript'in Web API endpoint'lerini çağırarak menü verisi alması tipik pattern'dir:

```
QR Kod → /menu/detail/[mekanId] (MVC sayfa)
         ↓
         JavaScript → /api/product/list?kategoriId=X (Web API)
```

## Web.config Yapısı

.NET Framework uygulamaları konfigürasyon için `Web.config` kullanır:
- Connection string: `<connectionStrings>` bölümünde
- AppSettings: `<appSettings>` bölümünde
- Routing, auth konfigürasyonu: App_Start/ klasöründe kod olarak

## Teknik Borç

Bu proje önemli teknik borç içermektedir:
- .NET Framework 4.7.2 — Microsoft desteği 2031'e kadar var ancak modern cloud deployment için uygun değil
- EF 6 Database-First — Code-First migration yok, şema değişiklikleri yönetimi zorlu
- MVC 5 — ASP.NET Core MVC'ye kıyasla daha az performanslı, cross-platform değil
- packages.config — modern SDK-style proje değil
- Bağımlılık enjeksiyonu yok (built-in DI) — `new` ile nesne oluşturma yaygın

## Projeler Arası Karşılaştırma

| | qr_menu_mvc | dijital_menu_angular |
|---|---|---|
| Teknoloji | ASP.NET MVC 5 (.NET FW 4.7.2) | Angular 11 |
| QR Menü | Evet (admin + public) | Hayır (sadece listeleme/tanıtım) |
| Auth | Admin login | Yok (public) |
| Veri | DB-first EF 6 | API bağımlı değil |
| Yaş | Eski (~2018-2020) | Daha yeni |

## Sonuç

Bu proje, .NET Framework dönemine ait, database-first EF 6 ile geliştirilmiş geleneksel bir MVC web uygulamasıdır. QR menü sistemi için tüm admin fonksiyonlarını (mekan, kategori, ürün, menü yayınlama) barındırıyor. Modern geliştirme standartlarına göre önemli teknik borç içeriyor; ancak işlevsel bir sistem olarak çalışıyor olabilir. Modernize edilecekse ASP.NET Core + Angular/React + EF Core Code-First mimarisine geçiş önerilir.
