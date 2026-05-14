# qr_menu_mvc — Kapsamlı Kod Analizi

## 1. Platform & Tech Stack

| Bileşen | Versiyon / Detay |
|---|---|
| Framework | ASP.NET MVC 5 + ASP.NET Web API 2 |
| .NET Platformu | .NET Framework 4.7.2 |
| ORM | Entity Framework 6 — Database-First (edmx / auto-generated partial classes) |
| Veritabanı | SQL Server (provider: `System.Data.SqlClient`) |
| QR Kod | QRCoder (NuGet) + System.Drawing |
| Redis | StackExchange.Redis (modern `IRedisCacheService` + legacy `RedisCacheManager` Singleton) |
| DI Container | Autofac + Autofac.Integration.Mvc (`DependencyConfig.cs`) |
| JSON | Newtonsoft.Json 12.x |
| Excel | SpreadsheetLight + DocumentFormat.OpenXml + Microsoft.Office.Interop.Excel |
| JS/CSS | jQuery, Bootstrap, Modernizr (BundleConfig) |
| Auth | Session + Cookie tabanlı (özel Singleton tabanlı mekanizma, ASP.NET Identity yok) |
| Server-Side | Razor (.cshtml), DataTables entegrasyonu |

---

## 2. Proje Yapısı (tam klasör ağacı)

```
QrMenu/
├── pixdinn2/
│   └── PixdinnQrMenu/
│       ├── DBservices/                   — Ayrı bir class library projesi (EF Database-First entities)
│       │   ├── Model1.cs                 — Auto-generated (boş, sadece header)
│       │   ├── Class1.cs                 — Boş placeholder
│       │   ├── AdminLoginDevices.cs
│       │   ├── AdminLogins.cs
│       │   ├── AdminLoginSessions.cs
│       │   ├── AdminNotifications.cs
│       │   ├── AdminUsers.cs
│       │   ├── AnketCevap.cs
│       │   ├── AnketCevapContext.cs
│       │   ├── AnketCevapResult.cs
│       │   ├── Anketler.cs
│       │   ├── AnketSoruContent.cs
│       │   ├── AnketSorular.cs
│       │   ├── BasketItem.cs
│       │   ├── Baskets.cs
│       │   ├── Blog.cs
│       │   ├── DemoTalebi.cs
│       │   ├── GeneralPage*.cs
│       │   ├── GorusOneri.cs
│       │   ├── GorusOneriMenu.cs
│       │   ├── GorusOneriUser.cs
│       │   ├── GrupKullaniciYetki.cs
│       │   ├── Icindekiler.cs
│       │   ├── KampanyaItemData.cs
│       │   ├── KategoriOneCikarmaItem.cs
│       │   ├── Kategoriler.cs
│       │   ├── Languages.cs
│       │   ├── Media.cs
│       │   ├── MekanAnket.cs
│       │   ├── MekanGaleri.cs
│       │   ├── MekanGruplari.cs
│       │   ├── MekanInfo.cs
│       │   ├── MekanKampanya.cs
│       │   ├── MekanKullanicilari.cs
│       │   ├── MekanZamanAraligi.cs
│       │   ├── MenuDesignerCategorieOrder.cs
│       │   ├── MenuDesignerProductOrder.cs
│       │   ├── MenuDesignerTemp.cs
│       │   ├── MenuUrunGrupAck.cs
│       │   ├── MenuUrunlerTemp.cs
│       │   ├── OrderMainTempData.cs
│       │   ├── ProductZamanAraligi.cs
│       │   ├── RootAdmin.cs
│       │   ├── SayiRaporuGetSaniyeFunction_Result.cs
│       │   ├── SiparisItem.cs
│       │   ├── SiparisKontrolQr.cs
│       │   ├── Siparisler.cs
│       │   ├── SiparisSms.cs
│       │   ├── TanitimAnket.cs
│       │   ├── TanitimAnketCevapResult.cs
│       │   ├── TanitimAnketUser.cs
│       │   ├── UrunCesniler.cs
│       │   ├── UrunInfo.cs
│       │   ├── UrunOneCikarma.cs
│       │   ├── Urunler.cs
│       │   ├── UrunlerIcons.cs
│       │   ├── UrunStokAck.cs
│       │   ├── UserAdres.cs
│       │   ├── UserDevices.cs
│       │   ├── UserMoving.cs
│       │   ├── Users.cs
│       │   ├── Yetki.cs
│       │   └── YetkiItems.cs
│       │
│       └── PixDinn/                      — Ana ASP.NET MVC projesi (namespace: PixDinn)
│           ├── App_Start/
│           │   ├── BundleConfig.cs
│           │   ├── DependencyConfig.cs   — Autofac DI kayıt
│           │   ├── FilterConfig.cs
│           │   ├── RouteConfig.cs
│           │   └── WebApiConfig.cs
│           ├── Controllers/
│           │   ├── AdminAnketYonetController.cs
│           │   ├── AdminBlockController.cs
│           │   ├── AdminDilController.cs
│           │   ├── AdminHelperController.cs
│           │   ├── AdminHomeController.cs
│           │   ├── AdminKategoriController.cs
│           │   ├── AdminLoginController.cs
│           │   ├── AdminMekanController.cs
│           │   ├── AdminMekanGrupController.cs
│           │   ├── AdminMekanUserController.cs
│           │   ├── AdminNotificationController.cs
│           │   ├── AdminUsersController.cs
│           │   ├── AnketController.cs
│           │   ├── BlogController.cs
│           │   ├── DatatableController.cs
│           │   ├── GorusOneriController.cs
│           │   ├── GorusVeOnerilerController.cs
│           │   ├── HomeController.cs
│           │   ├── IstatistikController.cs
│           │   ├── KampanyalarController.cs
│           │   ├── MediaController.cs
│           │   ├── MenuController.cs
│           │   ├── MenuPublisController.cs
│           │   ├── ProductController.cs
│           │   ├── RaporController.cs
│           │   ├── SiteApiController.cs
│           │   ├── SiteBlockController.cs
│           │   ├── TanitimSitesiAnketController.cs
│           │   ├── TanitimSitesiAnketCevaplarController.cs
│           │   ├── UploadFileController.cs
│           │   ├── UserBlockController.cs
│           │   ├── UserDetailReportController.cs
│           │   ├── ViewMenuController.cs        — Public QR menü görüntüleme
│           │   └── YetkiController.cs
│           ├── Models/
│           │   ├── AdminModels/
│           │   │   ├── AdminGlobalModels/       — Global JSON/response modeller
│           │   │   ├── AdminViewModels/         — Sayfa bazlı ViewModel'lar
│           │   │   ├── AdminLoginSessionResponse.cs
│           │   │   ├── AdminLoginedUserModel.cs
│           │   │   └── YetkilendirmeJsonModel.cs
│           │   ├── SiteApiModels/               — Public API request/response DTO'lar
│           │   └── SiteModels/                  — Public site model'leri
│           ├── Services/
│           │   ├── Cache/
│           │   │   ├── IRedisCacheService.cs
│           │   │   └── RedisCacheService.cs
│           │   └── Menu/
│           │       ├── IMenuService.cs
│           │       └── MenuService.cs
│           ├── Views/
│           │   ├── AdminBlock/                  — Header, Footer, Menu, PageHeader partial'lar
│           │   ├── AdminDil/, AdminHome/, AdminKategori/, AdminMekan/
│           │   ├── AdminMekanGrup/, AdminMekanUser/, AdminUsers/
│           │   ├── Anket/, Blog/, Datatable/
│           │   ├── GorusOneri/, GorusVeOneriler/
│           │   ├── Home/                        — Tanitim sitesi (Index_old, BlogQrMenu vb.)
│           │   ├── Istatistik/
│           │   ├── Kampanyalar/
│           │   ├── Media/, Menu/, MenuDesigner/
│           │   ├── Product/
│           │   ├── Rapor/
│           │   ├── Shared/_Layout.cshtml
│           │   ├── SiteBlock/
│           │   ├── TanitimSitesiAnket/, TanitimSitesiAnketCevaplar/
│           │   ├── UserBlock/
│           │   ├── ViewMenu/                    — Public QR menü sayfaları
│           │   └── Yetki/
│           ├── Global.asax.cs
│           └── Web.config
```

---

## 3. Veritabanı Modeli (entity açıklamaları)

DBservices projesi EF Database-First ile üretilen partial class'lardan oluşur. `pixdinnEntities` DbContext tüm tablolara erişim noktasıdır. Bağlantı: `data source=213.x.x.x;initial catalog=pixdinn`.

### Kullanıcı & Auth Tabloları

```csharp
// Sistem yöneticisi süper kullanıcı
public partial class RootAdmin
{
    public int ID { get; set; }
    public string USERNAME { get; set; }
    public string PASSWORD { get; set; }   // MD5 hash
    public string NAME { get; set; }
    public string SURNAME { get; set; }
    public string MAIL { get; set; }
    public string PHONE_CODE { get; set; }
    public string PHONE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> ROOT_TYPE { get; set; }
}

// Mekan/Admin kullanıcılar
public partial class AdminUsers
{
    public int ID { get; set; }
    public string USERNAME { get; set; }
    public string PASSWORD { get; set; }   // MD5
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<int> CREATED_USER_ID { get; set; }
    public string NAME { get; set; }
    public string SURNAME { get; set; }
    public string MAIL { get; set; }
    public string PHONE_CODE { get; set; }
    public string PHONE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> AUTH_ID { get; set; }
    public Nullable<int> ADMIN_TYPE { get; set; }   // enum: ROOT, NORMAL, MEKAN_USER, MEKAN_GRUP_USER
}

// Giriş yapan cihaz kaydı (IP + UserAgent)
public partial class AdminLoginDevices
{
    public int ID { get; set; }
    public string IP_ADDRESS { get; set; }
    public string BROWSER_DATA { get; set; }
    public Nullable<int> DEVICE_STATE { get; set; }
    public Nullable<System.DateTime> STATE_DATE { get; set; }
    public Nullable<int> DEVICE_ACCESS_STATE { get; set; }  // SUCCESS, IP_BANNED
}

// Giriş denemesi + pageKey anti-CSRF mekanizması
public partial class AdminLogins
{
    public int ID { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public Nullable<int> DEVICE_ID { get; set; }
    public Nullable<int> LOGIN_STATE { get; set; }       // LOGIN_NEEDY, LOGIN_SUCCESS
    public Nullable<int> LOGIN_PAGE_STATE { get; set; }  // NORMAL, LOGIN_POST
    public Nullable<int> LOGIN_FAIL_COUNT { get; set; }
    public Nullable<System.DateTime> LOGIN_STATE_DATE { get; set; }
    public Nullable<System.DateTime> LOGIN_PAGE_STATE_DATE { get; set; }
    public string REPATCHA_VALUE { get; set; }
    public string LOGIN_KEY_DATA { get; set; }    // 120-char random pageKey (anti-replay)
    public Nullable<System.DateTime> LOGIN_START_DATE { get; set; }
    public Nullable<System.DateTime> LOGIN_END_DATE { get; set; }
}

// Aktif oturum (Session + Cookie çifti)
public partial class AdminLoginSessions
{
    public int ID { get; set; }
    public Nullable<int> USER_TYPE { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public Nullable<int> LOGIN_ID { get; set; }
    public Nullable<System.DateTime> LOGIN_DATE { get; set; }
    public Nullable<int> STATE { get; set; }
    public string SESSION_ID { get; set; }    // 120-char random
    public string SESSION_KEY { get; set; }   // 120-char random
    public string COOKIE_ID { get; set; }     // 120-char random
    public string COOKIE_KEY { get; set; }    // 120-char random
}
```

### Mekan / Organizasyon Tabloları

```csharp
// Mekan metadata (dil bazlı)
public partial class MekanInfo
{
    public int ID { get; set; }
    public string MEKAN_ADI { get; set; }
    public string MEKAN_ACIKLAMA { get; set; }
    public Nullable<int> LANGUAGE_ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> STATE { get; set; }
}

// Mekan grupları (çoklu mekan yönetimi)
public partial class MekanGruplari
{
    public int ID { get; set; }
    public string GRUP_ISMI { get; set; }
    public Nullable<int> STATE { get; set; }
}

// Mekan – kullanıcı ilişkisi + yetki
public partial class MekanKullanicilari
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> MEKAN_GRUP_ID { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    public Nullable<int> CREATED_ADMIN_ID { get; set; }
    public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> YETKI_ID { get; set; }
}
```

### Dil Tablosu

```csharp
public partial class Languages
{
    public int ID { get; set; }
    public string LANGUAGE_NAME { get; set; }
    public Nullable<int> LANGUAGE_ICON_ID { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<bool> ONCELIKLI { get; set; }   // Öncelikli dil flag'i
}
```

### Ürün & Kategori Tabloları

```csharp
// Ürün ana kaydı (dil-bağımsız meta)
public partial class Urunler
{
    public int ID { get; set; }
    public Nullable<int> URUN_RESIM_ID { get; set; }
    public Nullable<int> URUN_RESIM_BUYUK_ID { get; set; }
    public Nullable<int> PISME_SURESI { get; set; }
    public Nullable<double> KALORI { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    public Nullable<int> CREATED_ADMIN_ID { get; set; }
    public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
    public Nullable<int> STATE { get; set; }    // 1=Aktif, 2=Pasif, 3=Silindi
    public string sureTipi { get; set; }
    public string URUN_KODU { get; set; }
    public string PLU_NO { get; set; }
}

// Ürün dil bazlı bilgisi (isim, fiyat, açıklama)
public partial class UrunInfo
{
    public int ID { get; set; }
    public Nullable<int> URUN_ID { get; set; }
    public string URUN_ISMI { get; set; }
    public Nullable<double> FIYAT { get; set; }
    public string FIYAT_BIRIM { get; set; }
    public string URUN_ACIKLAMA { get; set; }
    public string ADET_TIPI { get; set; }
    public Nullable<int> LANGUAGE_ID { get; set; }
    public Nullable<int> STATE { get; set; }
    public string FIYAT_METIN { get; set; }     // Metinsel fiyat (örn: "Market Fiyatı")
    public Nullable<int> RESIM_ID { get; set; }
}

// Kategori ana kaydı
public partial class Kategoriler
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> KATEGORI_RESMI { get; set; }
    public Nullable<int> KATEGORI_LEVEL { get; set; }    // Hiyerarşi seviyesi (1=üst)
    public Nullable<int> UST_KATEGORI_ID { get; set; }   // Parent kategori
    public Nullable<int> LANGUAGE_ID { get; set; }
    public Nullable<int> STATE { get; set; }
    public string KATEGORI_URL { get; set; }
}
```

### Menü Tabloları

```csharp
// Menü ana kaydı (MenulerTemp — "Temp" isim geçici, aslında canlı kayıt)
// EF entity tam kodu bulunmuyor (auto-generated içi boş); alan listesi SQL sorgulardan çıkarılmıştır:
// ID, MEKAN_ID, ACK (menü adı), QR_CODE (120-char token), QR_CODE_SIPARIS,
// MENU_URL, STATE, SIPARIS_DURUMU (bool), MENU_SAYFA_YAZI_DURUMU (bool),
// SHOWING_ENTRANCE_PAGE (bool), CREATED_DATE

// Menüde yer alan ürünler (geçici/aktif)
// MenuUrunlerTemp: ID, MENU_ID, URUN_ID, KATEGORI_ID, MEKAN_ID, CREATED_DATE,
//                  GROUP_NAME (string), GROUP_COLOR, GROUP_TYPE, PICTURE_SHOWING_TYPE
```

### Sipariş Tabloları

```csharp
public partial class Siparisler
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> MENU_ID { get; set; }
    public Nullable<int> STATE { get; set; }          // ONAY_BEKLEYEN, ONAYLANAN, IPTAL_EDILEN
    public Nullable<int> USER_STATE { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE_USER { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public Nullable<int> ODEME_TIPI { get; set; }     // 1=Nakit, 2=Kredi Kartı, 3=Yemek Kartı
    public string NOTE { get; set; }
    public Nullable<int> ADRED_ID { get; set; }
    public Nullable<int> CAMPAIGN_ID { get; set; }
    public Nullable<double> TOTAL_PRICE { get; set; }
    public Nullable<double> LONGTITUDE { get; set; }
    public Nullable<double> LATITUDE { get; set; }
    public Nullable<int> USER_DEVICE_ID { get; set; }
    public Nullable<int> POINT { get; set; }
    public Nullable<int> BASKET_ID { get; set; }
    public string ODEME_TIPI_TEXT { get; set; }
}

public partial class SiparisItem
{
    public int ID { get; set; }
    public Nullable<int> SIPARIS_ID { get; set; }
    public Nullable<int> PRODUCT_ID { get; set; }
    public Nullable<int> QUANTITY { get; set; }
    public Nullable<double> PRICE { get; set; }
    public Nullable<int> STATE { get; set; }
    public string NOTE { get; set; }
}

// Sepet (siparis verilmeden önce)
public partial class Baskets
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> MENU_ID { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> USER_STATE { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    public string BASKET_KEY { get; set; }    // Cookie'de tutulan sepet anahtarı
}

public partial class BasketItem
{
    public int ID { get; set; }
    public Nullable<int> BASKET_ID { get; set; }
    public Nullable<int> PRODUCT_ID { get; set; }
    public Nullable<int> QUANTITY { get; set; }
    public Nullable<double> PRICE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    public string NOTE { get; set; }
}
```

### Anket Tabloları

```csharp
public partial class Anketler
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public string ANKET_NAME { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> SHOWING_TYPE { get; set; }
    public Nullable<int> SHOWING_START_TYPE { get; set; }
    public Nullable<int> CLOSING_TYPE { get; set; }
    public Nullable<int> SHOWING_DATE_TYPE { get; set; }
    public Nullable<System.DateTime> SHOWING_DATE_START { get; set; }
    public Nullable<System.DateTime> SHOWING_DATE_END { get; set; }
    public string SHOWING_HOUR_START { get; set; }
    public string SHOWING_HOUR_END { get; set; }
    public string BACKGROUND_IMAGE_URL { get; set; }
    public Nullable<int> REPEAT_COUNT_PER_USER { get; set; }
    public Nullable<int> REPEAT_TTYPE { get; set; }
    public Nullable<int> STYLE_TYPE { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<int> CEVAP_GORUNUM_TYPE { get; set; }
    // Tasarım renkleri:
    public string BACKGROUND_COLOR { get; set; }
    public string QUESTION_FONT_COLOR { get; set; }
    public string ANSWER_FONT_COLOR { get; set; }
    public string ANSWER_BACK_COLOR { get; set; }
    public string GONDER_BUTTON_BACK { get; set; }
    public string GONDER_BUTTON_FONT { get; set; }
    public string SECILEN_CEVAP_BACK { get; set; }
}

public partial class AnketSorular
{
    public int ID { get; set; }
    public Nullable<int> ANKET_ID { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> SORU_TYPE { get; set; }
    public Nullable<int> NUMBER { get; set; }   // Soru sırası
}

public partial class AnketCevap
{
    public int ID { get; set; }
    public Nullable<int> SORU_ID { get; set; }
    public string IMAGE_ICON_URL { get; set; }
    public Nullable<int> STATE { get; set; }
}
```

### Diğer Önemli Tablolar

```csharp
// Kullanıcı hareketi (rapor/istatistik için)
public partial class UserMoving
{
    public int ID { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public Nullable<System.DateTime> MOVING_DATE { get; set; }
    public string MOVING_SCOPE { get; set; }
    public string MOVING_NAME { get; set; }    // "KATEGORI" veya "URUN"
    public Nullable<int> MOVING_ID { get; set; }
    public Nullable<int> MENU_ID { get; set; }
    public string IP_ADRESS { get; set; }
    public string CITY { get; set; }
    public Nullable<double> LATITUDE { get; set; }
    public Nullable<double> LONGTITUDE { get; set; }
    public string ADRESS { get; set; }
}

// Görüş ve öneri
public partial class GorusOneri
{
    public int ID { get; set; }
    public string IP_ADDRESS { get; set; }
    public string DEVICE_TOCKEN { get; set; }
    public string MASSAGE { get; set; }
    public Nullable<System.DateTime> MESSAGE_DATE { get; set; }
    public string ADMIN_NOTE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> USER_ID { get; set; }
    public string CITY { get; set; }
    public Nullable<double> LATITUDE { get; set; }
    public Nullable<double> LONGTITUDE { get; set; }
    public string ADRESS { get; set; }
}

// Medya dosyası kaydı
public partial class Media
{
    public int ID { get; set; }
    public string MEDIA_TITLE { get; set; }
    public Nullable<System.DateTime> CREATED_DATE { get; set; }
    public Nullable<int> CREATED_USER_ID { get; set; }
    public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
    public Nullable<int> MEDIA_DATA_TYPE { get; set; }
    public Nullable<int> MEDIA_TYPE { get; set; }
    public Nullable<int> STATE { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
}

// Yetki tanımı
public partial class Yetki
{
    public int ID { get; set; }
    public Nullable<int> MEKAN_ID { get; set; }
    public Nullable<int> TYPE { get; set; }
    public Nullable<int> STATE { get; set; }
    public string DESCRIPTION { get; set; }
    public string NAME { get; set; }    // örn: "MENU_ISLEMLERI", "MEKAN_KULLANICI_VE_YETKILENDIRME_ISLEMLERI"
}

public partial class YetkiItems
{
    public int ID { get; set; }
    public Nullable<int> YETKI_ID { get; set; }
    public Nullable<int> TYPE { get; set; }
    public string NAME { get; set; }
    public string VALUE { get; set; }
}

// Son kullanıcılar (menü ziyaretçileri)
public partial class Users
{
    public int ID { get; set; }
    public string PHONE { get; set; }
    public string NAME { get; set; }
    public string SURNAME { get; set; }
    public string MAIL { get; set; }
    public Nullable<int> MENU_ID { get; set; }
}
```

---

## 4. DBservices Katmanı

DBservices ayrı bir class library projesidir (`namespace DBservices`). Tüm entity sınıfları EF Database-First ile `pixdinnEntities` DbContext üzerinden otomatik üretilmiştir. Uygulama kodunda her yerde `using (var db = new DBservices.pixdinnEntities())` pattern'i kullanılır — repository veya service abstraction yoktur (sadece yeni eklenen `MenuService` ve `RedisCacheService` hariç).

**DbContext Kullanım Pattern'i (tüm projedeki standart):**
```csharp
using (var db = new DBservices.pixdinnEntities())
{
    // LINQ sorgusu
    var urun = db.Urunler.Where(u => u.ID == id && u.STATE != 3).FirstOrDefault();

    // Insert
    var inserted = new DBservices.Urunler();
    inserted.MEKAN_ID = mekanId;
    inserted.STATE = (int)GlobalEnums.AdminDataState.ACTIVE;
    db.Urunler.Add(inserted);
    db.SaveChanges();

    // Update
    urun.STATE = (int)GlobalEnums.AdminDataState.PASSIVE;
    db.Entry(urun).State = System.Data.Entity.EntityState.Modified;
    db.SaveChanges();

    // Raw SQL (DataTable)
    DataTable tab = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
        .getdataTableProcess()
        .QueryToTable(db, query, sqlParameters);
}
```

**Ham SQL ile DataTable çekme (rapor/istatistik için yaygın):**
```csharp
var query = @"
    select MOVING_ID as ID, count(MOVING_ID) as COUNT_DATA
    from UserMoving
    where YEAR(MOVING_DATE) = " + DateTime.Now.Year + @"
    and MOVING_NAME = 'URUN'
    and MENU_ID = " + menuId + @"
    group by MOVING_ID";

var result = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getDataTableData(db, query, new List<string>() { "ID", "COUNT_DATA" }, sqlParameters);
```

**Uyarı:** SQL string concatenation ile `SqlParameter` listesi beraber kullanılıyor — parametreler bazı sorgularda etkin değil (SQL injection riski).

---

## 5. Controllers (TÜM controller'lar tam kod)

### AdminLoginController.cs
**Konum:** `PixDinn/Controllers/AdminLoginController.cs`
**Namespace:** `PixDinn.Controllers`

```csharp
using DBservices;
using PixDinn.Models.AdminModels;
using PixDinn.Models.AdminModels.AdminGlobalModels;
using PixDinn.Models.AdminModels.AdminViewModels;
using PixDinn.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PixDinn.Processes.GlobalEnums;

namespace PixDinn.Controllers
{
    public class AdminLoginController : Controller
    {
        // GET: AdminLogin — Login formu
        public ActionResult Index()
        {
            try
            {
                // Kullanıcı zaten giriş yaptıysa yönlendir
                var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT)
                    .getAdminLoginProcess().getLoginState();
                if (loginState.loginstate == AdminLoginSessionResponse.Loginstateenum.basarili)
                {
                    return Redirect("/adminhome");
                }

                AdminLoginPageModel adminLoginPageModel = new AdminLoginPageModel();
                adminLoginPageModel.errorState = 0;
                String pageKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                    .getRandomString(120);

                String ipAddress = HttpContext.Request.UserHostAddress;
                String userAgentData = Request.UserAgent;

                using (var db = new pixdinnEntities())
                {
                    // IP ban kontrolü
                    var ipData = db.AdminLoginDevices.Where(ld =>
                        ld.IP_ADDRESS == ipAddress &&
                        ld.DEVICE_ACCESS_STATE == (int)GlobalEnums.DeviceAccessState.IP_BANNED)
                        .FirstOrDefault();
                    if (ipData != null)
                        return Redirect("/adminhelper/PageAccessNotFound");

                    var deviceData = db.AdminLoginDevices.Where(vv =>
                        vv.IP_ADDRESS == ipAddress &&
                        vv.BROWSER_DATA == userAgentData).FirstOrDefault();

                    if (deviceData == null)
                    {
                        // Yeni cihaz kaydı oluştur
                        var insertedDevice = new AdminLoginDevices();
                        insertedDevice.BROWSER_DATA = userAgentData;
                        insertedDevice.DEVICE_ACCESS_STATE = (int)GlobalEnums.DeviceAccessState.SUCCESS;
                        insertedDevice.DEVICE_STATE = (int)GlobalEnums.LoginDeviceState.ONLOGINPAGE;
                        insertedDevice.IP_ADDRESS = ipAddress;
                        insertedDevice.STATE_DATE = DateTime.Now;
                        db.AdminLoginDevices.Add(insertedDevice);
                        db.SaveChanges();

                        var insertedLoginData = new AdminLogins();
                        insertedLoginData.DEVICE_ID = insertedDevice.ID;
                        insertedLoginData.LOGIN_PAGE_STATE_DATE = DateTime.Now;
                        insertedLoginData.LOGIN_FAIL_COUNT = 0;
                        insertedLoginData.LOGIN_KEY_DATA = pageKey;
                        insertedLoginData.LOGIN_PAGE_STATE = (int)GlobalEnums.LoginPageState.NORMAL;
                        insertedLoginData.LOGIN_STATE = (int)GlobalEnums.LoginState.LOGIN_NEEDY;
                        db.AdminLogins.Add(insertedLoginData);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (deviceData.DEVICE_ACCESS_STATE == (int)GlobalEnums.DeviceAccessState.SUCCESS)
                        {
                            deviceData.DEVICE_STATE = (int)GlobalEnums.LoginDeviceState.ONLOGINPAGE;
                            deviceData.STATE_DATE = DateTime.Now;
                            db.Entry(deviceData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            var loginData = db.AdminLogins
                                .Where(ll => ll.DEVICE_ID == deviceData.ID).FirstOrDefault();
                            if (loginData != null)
                            {
                                loginData.LOGIN_KEY_DATA = pageKey;
                                loginData.LOGIN_PAGE_STATE = (int)GlobalEnums.LoginPageState.NORMAL;
                                db.Entry(loginData).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var insertedLoginData = new AdminLogins();
                                insertedLoginData.DEVICE_ID = deviceData.ID;
                                insertedLoginData.LOGIN_FAIL_COUNT = 0;
                                insertedLoginData.LOGIN_KEY_DATA = pageKey;
                                insertedLoginData.LOGIN_PAGE_STATE = (int)GlobalEnums.LoginPageState.NORMAL;
                                db.AdminLogins.Add(insertedLoginData);
                                db.SaveChanges();
                            }
                        }
                    }

                    adminLoginPageModel.errorState = GlobalEnums.PageGeneralError.SUCCESS;
                    adminLoginPageModel.pageKey = pageKey;
                }

                return View(adminLoginPageModel);
            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageAccessNotFound");
            }
        }

        // POST: Login formu işlemi
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LoginPost(AdminLoginPageModel loginFormPostModel)
        {
            try
            {
                var userName = loginFormPostModel.userName;
                var password = loginFormPostModel.password;
                var pageKey = loginFormPostModel.pageKey;
                String ipAddress = HttpContext.Request.UserHostAddress;
                String userAgentData = Request.UserAgent;
                String passwordMd5 = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                    .decryptMd5(password);   // MD5 hash

                AdminLoginJsonModel result = new AdminLoginJsonModel();
                result.IsSuccess = false;

                AdminLoginSessionRequest loginSessionRequest = null;
                DateTime nowDate = DateTime.Now;

                using (var db = new pixdinnEntities())
                {
                    var loginDevice = db.AdminLoginDevices.Where(ll =>
                        ll.BROWSER_DATA == userAgentData &&
                        ll.IP_ADDRESS == ipAddress).FirstOrDefault();

                    if (loginDevice != null &&
                        loginDevice.DEVICE_ACCESS_STATE == (int)GlobalEnums.DeviceAccessState.SUCCESS)
                    {
                        var adminLogin = db.AdminLogins.Where(al =>
                            al.DEVICE_ID == loginDevice.ID &&
                            al.LOGIN_KEY_DATA == pageKey &&
                            al.LOGIN_PAGE_STATE == (int)GlobalEnums.LoginPageState.NORMAL)
                            .FirstOrDefault();

                        if (adminLogin != null)
                        {
                            // 3 saniyeden az sürede giriş denemeleri reddedilir (bot koruması)
                            var seconds = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                                .dateCompareSeconds(adminLogin.LOGIN_PAGE_STATE_DATE.Value, nowDate);

                            if (seconds >= 3)
                            {
                                var adminUser = db.AdminUsers.Where(ad =>
                                    ad.USERNAME == userName && ad.PASSWORD == passwordMd5)
                                    .FirstOrDefault();
                                var rootAdmin = db.RootAdmin.Where(ad =>
                                    ad.USERNAME == userName && ad.PASSWORD == passwordMd5)
                                    .FirstOrDefault();

                                String cookieKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                                String cookieId  = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                                String sessionKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                                String sessionId  = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);

                                if (adminUser == null && rootAdmin == null)
                                {
                                    result.IsSuccess = false;
                                    result.MessageBody = "Giriş Bilgileriniz Hatalı";
                                }
                                else if (rootAdmin != null)
                                {
                                    loginSessionRequest = new AdminLoginSessionRequest();
                                    loginSessionRequest.userId = rootAdmin.ID;
                                    loginSessionRequest.adminType = GlobalEnums.LoginAdminType.ROOT;
                                }
                                else
                                {
                                    loginSessionRequest = new AdminLoginSessionRequest();
                                    loginSessionRequest.userId = adminUser.ID;
                                    loginSessionRequest.adminType = (GlobalEnums.LoginAdminType)adminUser.ADMIN_TYPE;
                                }

                                if (loginSessionRequest != null)
                                {
                                    // Session DB'ye yazılıyor
                                    var loginSession = db.AdminLoginSessions.Where(als =>
                                        als.LOGIN_ID == adminLogin.ID &&
                                        als.USER_ID == adminLogin.USER_ID).FirstOrDefault();

                                    if (loginSession == null)
                                    {
                                        var insertedSession = new AdminLoginSessions();
                                        insertedSession.LOGIN_DATE = nowDate;
                                        insertedSession.LOGIN_ID = adminLogin.ID;
                                        insertedSession.SESSION_ID = sessionId;
                                        insertedSession.SESSION_KEY = sessionKey;
                                        insertedSession.STATE = (int)GlobalEnums.AdminLoginSessionState.ACTIVE;
                                        insertedSession.USER_ID = loginSessionRequest.userId;
                                        insertedSession.USER_TYPE = (int)loginSessionRequest.adminType;
                                        insertedSession.COOKIE_ID = cookieId;
                                        insertedSession.COOKIE_KEY = cookieKey;
                                        db.AdminLoginSessions.Add(insertedSession);
                                        db.SaveChanges();
                                        loginSessionRequest.loginSessionId = insertedSession.ID;
                                    }
                                    else
                                    {
                                        loginSession.SESSION_ID = sessionId;
                                        loginSession.SESSION_KEY = sessionKey;
                                        loginSession.COOKIE_ID = cookieId;
                                        loginSession.COOKIE_KEY = cookieKey;
                                        loginSession.USER_ID = loginSessionRequest.userId;
                                        loginSession.USER_TYPE = (int)loginSessionRequest.adminType;
                                        loginSessionRequest.loginSessionId = loginSession.ID;
                                        db.Entry(loginSession).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    loginSessionRequest.cookieId = cookieId;
                                    loginSessionRequest.cookieKey = cookieKey;
                                    loginSessionRequest.sessionId = sessionId;
                                    loginSessionRequest.sessionKey = sessionKey;

                                    // Singleton session'a kaydediliyor
                                    Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                                        .getAdminLoginProcess().saveSession(loginSessionRequest);

                                    result.IsSuccess = true;
                                    result.redirect = true;
                                    result.redirectUrl = "/adminhome";
                                }
                            }
                        }
                    }

                    // pageKey sıfırlanıyor (replay saldırısı engeli)
                    if (adminLogin != null)
                    {
                        adminLogin.LOGIN_KEY_DATA = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                        db.Entry(adminLogin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
        }

        // Çıkış yap: session/cookie tokenları sıfırlanır
        public ActionResult Cikis()
        {
            var loginedState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT)
                .getAdminLoginProcess().getLoginState();
            if (loginedState.loginstate == AdminLoginSessionResponse.Loginstateenum.basarili)
            {
                String cookieKey = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String cookieId  = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String sessionId = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String sessionKey = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);

                using (var db = new DBservices.pixdinnEntities())
                {
                    db.AdminLoginSessions.Where(pp =>
                        pp.USER_ID == loginedState.loginedadmin.id &&
                        pp.USER_TYPE == (int)loginedState.loginedadmin.loginAdminType)
                        .ToList().ForEach(ii => {
                            ii.COOKIE_KEY = cookieKey;
                            ii.SESSION_ID = sessionId;
                            ii.COOKIE_ID = cookieId;
                            ii.SESSION_KEY = sessionKey;
                        });
                    db.SaveChanges();
                }
            }
            return Redirect("/adminlogin");
        }
    }
}
```

---

### AdminHomeController.cs (özet — tam önemli metodlar)

```csharp
public class AdminHomeController : Controller
{
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_HOME,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult Index()
    {
        AdminHomeViewModel homeViewModel = new AdminHomeViewModel();
        homeViewModel.menuler = new List<SelectListItem>();
        homeViewModel.menuname = "Anasayfa";
        homeViewModel.pagetitles = new List<...>();
        homeViewModel.urlQueryString = "/adminhome/";

        using (var db = new DBservices.pixdinnEntities())
        {
            var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                .getAdminLoginProcess().getLoginState().loginedadmin.mekanObj;
            var sqlExtra = mekanObj != null ? " AND M.ID IN " + mekanObj.mekanSqlWhere : "";

            var query = @"select MT.ID, MI.MEKAN_ADI, MT.ACK, MI.MEKAN_ID as MEKAN_ID
                from MenulerTemp MT left join Mekanlar M on MT.MEKAN_ID = M.ID
                left join MekanInfo MI on M.ID = MI.MEKAN_ID
                left join Languages L on MI.LANGUAGE_ID = L.ID
                where L.ONCELIKLI = 1" + sqlExtra;

            DataTable tab = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
                .getdataTableProcess().QueryToTable(db, query, new List<SqlParameter>());

            foreach (DataRow row in tab.Rows)
            {
                homeViewModel.menuler.Add(new SelectListItem() {
                    Value = row["ID"] + "",
                    Text = row["ACK"] + ""
                });
            }
        }
        return View(homeViewModel);
    }

    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.GET_DASHBOARD,
                               openingType = GlobalEnums.PageOpeningType.AJAX)]
    public ActionResult getDashBoardData([DefaultValue(0)] int menuId)
    {
        // Bu/ay ve bugün en çok görüntülenen kategori/ürünler
        // + anket sonuçları hesaplanıyor
        AdminHomePartialModel homeViewModel = new AdminHomePartialModel();
        using (var db = new DBservices.pixdinnEntities())
        {
            // UserMoving tablosundan GROUP BY ile sayım
            var buAyEnCokGoruntulenenKategorilerMap = Singleton...getDataTableData(db,
                @"select MOVING_ID as ID, count(MOVING_ID) as COUNT_DATA from UserMoving where
                YEAR(MOVING_DATE)=" + DateTime.Now.Year + " and MONTH(MOVING_DATE)=" + DateTime.Now.Month +
                " and MOVING_NAME='KATEGORI' and MOVING_ID!=0 and MENU_ID=" + menuId +
                " group by MOVING_ID", new List<string>() {"ID","COUNT_DATA"}, new List<SqlParameter>());

            // ... benzer sorgular bugun/tumZaman için de çekilir
            // Anket cevapları da SQL join ile çekilir:
            string queryCevapResult = @"
                select COUNT(T.CEVAP_ID) as SAYI, T.CEVAP_ID as CEVAP from (
                    select AC.ID as CEVAP_ID from AnketSorular ASO
                    left join AnketCevap AC on AC.SORU_ID = ASO.ID
                    left join AnketCevapResult AR on AR.CEVAP_ID = AC.ID
                    where ANKET_ID = " + anket.ID + " and AR.CEVAP_DATA = 'True'
                ) T group by T.CEVAP_ID";
        }
        return View(homeViewModel);
    }

    [HttpPost]
    public ActionResult getSiparislerData(string pData)
    {
        // DataTables server-side: Siparisler JOIN Users
        var query = @"select S.ID, S.MEKAN_ID, S.MENU_ID, S.STATE, S.TOTAL_PRICE,
            S.CREATED_DATE, S.USER_ID, U.NAME, U.SURNAME, U.PHONE
            from Siparisler S LEFT join Users U on U.ID=S.USER_ID
            WHERE U.ID!=0" + sqlExtra +
            " GROUP BY S.ID, S.MEKAN_ID, S.MENU_ID, S.TOTAL_PRICE, S.STATE, ...";
        var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
            .getdataTableProcess().getDataTableData("Siparisler", pData, query, "S.ID asc");
        return Json(datatableData);
    }

    [HttpPost]
    public ActionResult ChangeOrderState([DefaultValue(0)] int menuId, [DefaultValue(0)] int orderId)
    {
        // ONAY_BEKLEYEN -> ONAYLANAN state geçişi
        using (var db = new DBservices.pixdinnEntities())
        {
            var dbOrder = db.Siparisler.Where(x => x.ID == orderId).FirstOrDefault();
            if (dbOrder != null && dbOrder.STATE == (int)GlobalEnums.SiparisState.ONAY_BEKLEYEN)
            {
                dbOrder.STATE = (int)GlobalEnums.SiparisState.ONAYLANAN;
                dbOrder.UPDATED_DATE = DateTime.Now;
                db.Entry(dbOrder).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // SiparisItem'lar da güncelleniyor
            }
        }
        return Json(result, JsonRequestBehavior.AllowGet);
    }
}
```

---

### MenuController.cs (ana metodlar)

```csharp
public class MenuController : Controller
{
    // GET /menu — Menü listesi
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MENU,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult Index()
    {
        ListMenuViewModel listMenuViewModel = new ListMenuViewModel();
        listMenuViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
        return View(listMenuViewModel);
    }

    // GET /menu/addmenu — Menü oluşturma/düzenleme formu
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_MENU,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult AddMenu([DefaultValue(0)] int mekanId, [DefaultValue(0)] int geciciMenuId)
    {
        // Menu verisi + kategori listesi + yetki verileri yükleniyor
        // ...
    }

    // POST — QR kod üretimi (BASE64 PNG)
    [HttpPost]
    public ActionResult qrMenu([DefaultValue(0)] int geciciId)
    {
        using (var db = new DBservices.pixdinnEntities())
        {
            var menu = db.MenulerTemp.Where(mt => mt.ID == geciciId).FirstOrDefault();
            if (menu != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    string url = "https://pixdinn.com/viewmenu?data=" + menu.QR_CODE;
                    if (!string.IsNullOrEmpty(menu.MENU_URL))
                        url = "https://" + menu.MENU_URL + "/viewmenu?data=" + menu.QR_CODE;

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);

                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        addUrunTempResponseModel.data = "data:image/png;base64," +
                            Convert.ToBase64String(ms.ToArray());
                        addUrunTempResponseModel.url = url;
                        addUrunTempResponseModel.success = true;
                    }
                }
            }
        }
        return Json(addUrunTempResponseModel, JsonRequestBehavior.AllowGet);
    }

    // POST — Menüye ürün ekleme (geçici kayıt)
    [HttpPost]
    public ActionResult addUrunTemp([DefaultValue(0)] int menuId,
                                    [DefaultValue(0)] int geciciId,
                                    [DefaultValue(0)] int kategoriId,
                                    [DefaultValue("")] String selectedItemsJson, ...)
    {
        List<AddUrunTempRequestModel.SelectedJsonItem> urunler =
            Singleton...getMainSerialize()
            .Deserialize<List<AddUrunTempRequestModel.SelectedJsonItem>>(selectedItemsJson);

        using (var db = new DBservices.pixdinnEntities())
        {
            // Yetki kontrolü
            var isYetkili = loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER
                ? loginState.YetkiKontrolGrup("MENU_ISLEMLERI", mekanId)
                : loginState.YetkiKontrol("MENU_ISLEMLERI");

            if (isYetkili)
            {
                // MenulerTemp insert/update
                // MenuUrunlerTemp insert
                // Redis cache invalidate:
                QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
                RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
                    .getRedisCacheManager();
                qrMenuDbProcess.MenuId = menuId;
                qrMenuDbProcess.MenuUpdateDate = DateTime.Now;
                manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);
            }
        }
        return Json(addUrunTempResponseModel, JsonRequestBehavior.AllowGet);
    }

    // POST — Ürün gruplama
    [HttpPost]
    public ActionResult grupYap([DefaultValue(0)] int menuId,
                                [DefaultValue(0)] int geciciId,
                                [DefaultValue("")] String selectedItemsJson,
                                [DefaultValue("")] String grupYapAckJson,
                                [DefaultValue("1")] string grupType,
                                [DefaultValue("1")] string pictureShowingType,
                                [DefaultValue(0)] int mekanId)
    {
        // Seçilen MenuUrunlerTemp kayıtlarına GROUP_NAME (random string) + GROUP_COLOR atanır
        // MenuUrunGrupAck tablosuna her dil için açıklama eklenir
    }
}
```

---

### ProductController.cs (özet)

```csharp
public class ProductController : Controller
{
    // GET /product — Ürün listesi
    public ActionResult Index()
    {
        productListViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
        return View(productListViewModel);
    }

    // POST — DataTables server-side ürün listesi (JOIN'li komplex sorgu)
    [HttpPost]
    public ActionResult getUrunlerData(string pData)
    {
        var query = @"select KI.KATEGORI_ADI, MU.ID as MenuUrunId, U.ID,
            UI.URUN_ISMI, UI.FIYAT, UI.FIYAT_BIRIM, UI.URUN_ACIKLAMA, UI.FIYAT_METIN,
            UI.ADET_TIPI, U.URUN_KODU, U.PISME_SURESI, U.KALORI, MI.MEKAN_ADI,
            U.CREATED_DATE, U.UPDATED_DATE, A.USERNAME, U.STATE
            from Urunler U
            left join AdminUsers A on U.CREATED_ADMIN_ID=A.ID
            left join MekanInfo MI on U.MEKAN_ID=MI.MEKAN_ID
            left join UrunInfo UI on U.ID=UI.URUN_ID
            left join Languages L on UI.LANGUAGE_ID = L.ID
            left join Languages L2 on MI.LANGUAGE_ID = L2.ID
            left join MenuUrunlerTemp MU on MU.ID = (
                SELECT TOP 1 MU2.ID FROM MenuUrunlerTemp MU2
                WHERE MU2.URUN_ID = U.ID ORDER BY MU2.ID DESC)
            left join Kategoriler K on K.ID = MU.KATEGORI_ID
            left join KategoriInfo KI on K.ID=KI.KATEGORI_ID
            left join Languages L3 on KI.LANGUAGE_ID = L3.ID
            WHERE L.ONCELIKLI=1 and L2.ONCELIKLI=1 and (L3.ONCELIKLI=1 or L3.ID is null)
            AND U.STATE!=3" + sqlExtra;

        var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
            .getdataTableProcess().getDataTableData("Urunler", pData, query, "U.ID asc");
        return Json(datatableData);
    }
}
```

---

### ViewMenuController.cs (public QR menü — özet)

```csharp
public class ViewMenuController : Controller
{
    // GET /viewmenu?data={qrCode} — Public menü sayfası
    public async Task<ActionResult> Index(
        [DefaultValue("")] String data,         // QR code token
        [DefaultValue("KATEGORI")] String typePage,
        [DefaultValue(0)] int id,
        [DefaultValue("")] String menuDesign,
        [DefaultValue("")] String anketDesign,
        [DefaultValue("")] String urunKategoriDesign,
        [DefaultValue(false)] bool isEntrance)
    {
        String ipAddress = HttpContext.Request.UserHostAddress;
        String userAgentData = HttpContext.Request.UserAgent;

        RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
            .getRedisCacheManager();

        // IP ban kontrolü
        var ipBanCheck = manager.CheckBannedIpAddres(ipAddress);
        if (!ipBanCheck)
            return Redirect("/ViewMenu/unlicensedMenu");

        using (var db = new DBservices.pixdinnEntities())
        {
            // QR kod ile menü bulunuyor
            menuCode = db.MenulerTemp.Where(mm =>
                mm.QR_CODE == data && mm.STATE == (int)AdminDataState.ACTIVE)
                .AsNoTracking().FirstOrDefault();

            if (menuCode == null)
            {
                menuCode = db.MenulerTemp.Where(ii =>
                    ii.QR_CODE_SIPARIS == data && ii.STATE == (int)AdminDataState.ACTIVE)
                    .AsNoTracking().FirstOrDefault();
                if (menuCode != null) siparisState = true;
                else return Redirect("/ViewMenu/unlicensedMenu");
            }

            redisKey = "qrCode" + menuCode.ID + typePage + id + dilCookie + dilMekan;
            isExistQrMenuDbProcess = manager.IsExistQrMenuDbProcess(menuCode.ID, db);
            isQrMenuRedisDataUpdated = manager.IsQrMenuRedisDataUpdated(menuCode.ID, redisKey, db);
        }

        // Redis cache kontrolü: Cache hit ise doğrudan MenuViewModel döndür
        if (manager.IsExistQrMenuRedisProcess(redisKey) &&
            (!isExistQrMenuDbProcess || isQrMenuRedisDataUpdated) &&
            manager.IsSet(redisKey) && menuDesign == "" && isEntrance == false)
        {
            var menuViewModel = manager.Get<MenuViewModel>(redisKey);
            // Kullanıcı hareketi, sepet, kampanya, anket bilgileri ekleniyor
            // Ürün sıralama (sure/fiyat/tasarım sırası)
            return View(menuViewModel);
        }

        // Cache miss: DB'den MenuViewModel oluştur, Redis'e yaz
        // ...
    }
}
```

---

### RaporController.cs (özet)

```csharp
public class RaporController : Controller
{
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.RAPOR,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult Index()
    {
        // Menü listesi çekiliyor
        // UserMoving tablosundan hareket/sayı raporları için endpoint'ler:
        // - getRaporPartial: Tarih aralığı bazlı
        // - getSayiRaporuPartial: Toplam sayı (kategori/ürün/kullanıcı/menü)
        // - getZamanBazliRaporPartial: Zaman bazlı grafik
        // - getHareketRaporuListePartial: Kullanıcı detay listesi
        return View(raporViewModel);
    }
}
```

---

### AnketController.cs (özet)

```csharp
public class AnketController : Controller
{
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ANKET,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult Index()
    {
        // Anket listesi, mekan filtreli
        return View(anketMainViewModel);
    }

    public ActionResult AddAnket([DefaultValue(0)] int mekanId, [DefaultValue(0)] int anketId)
    {
        // Anket oluşturma/düzenleme
        // Soru tipi (SORU_TYPE), gösterim zamanı, renk tasarımı yönetimi
    }

    // Redis cache invalidate tetikleniyor her kayıtta
    [HttpPost]
    public ActionResult saveAnket(...)
    {
        // Anketler, AnketSorular, AnketCevap, AnketSoruContent, AnketCevapContext tabloları
        RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
            .getRedisCacheManager();
        manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);
    }
}
```

**Diğer Controller'lar (işlev özeti):**

| Controller | Görev |
|---|---|
| `AdminMekanController` | Mekan CRUD, galerisi, zaman aralığı |
| `AdminKategoriController` | Kategori hiyerarşisi yönetimi |
| `AdminDilController` | Mekan dil yönetimi |
| `AdminUsersController` | AdminUsers CRUD |
| `AdminMekanGrupController` | MekanGruplari CRUD |
| `AdminMekanUserController` | MekanKullanicilari yönetimi |
| `YetkiController` | Yetki tanımı + YetkiItems |
| `KampanyalarController` | Kampanya yönetimi |
| `MediaController` | Medya yükleme/listeleme |
| `UploadFileController` | Dosya upload (SpreadsheetLight Excel import) |
| `IstatistikController` | Sipariş istatistikleri |
| `GorusOneriController` | Görüş/öneri admin tarafı |
| `GorusVeOnerilerController` | Görüş/öneri toplu listeleme |
| `SiteBlockController` | Header/Footer/Anket block partial'lar |
| `SiteApiController` | Public JSON API (mobil için) |
| `UserBlockController` | Kullanıcı tarafı block render |
| `AdminBlockController` | Admin paneli partial render |
| `AdminHelperController` | Hata sayfaları, alert dialog |
| `BlogController` | Blog CRUD |
| `DatatableController` | Genel DataTables endpoint |
| `MenuPublisController` | Menü yayınlama işlemleri |
| `TanitimSitesiAnketController` | Tanıtım sitesi anketi |
| `HomeController` | Tanıtım sitesi public sayfaları |
| `UserDetailReportController` | Kullanıcı detay rapor |
| `AdminNotificationController` | Bildirim yönetimi |
| `AdminAnketYonetController` | Anket yönetim paneli |

---

## 6. Models/ViewModels (tam kod)

### AdminPageGlobalModel.cs — Tüm ViewModel'ların base class'ı

```csharp
namespace PixDinn.Models.AdminModels.AdminGlobalModels
{
    public class AdminPageGlobalModel
    {
        public List<PageHeaderItem> pagetitles { get; set; }
        public String urlQueryString { get; set; }
        public String menuname { get; set; }
        public String errorMessage { get; set; }
        public GlobalEnums.AdminFormTypes formType { get; set; }
        public GlobalEnums.OpeningType openingType { get; set; }
        public GlobalEnums.PageNameAdmin pageName { get; set; }
        public GlobalEnums.LoginAdminType loginedAdminType { get; set; }
        public Dictionary<string, YetkiKontrolData> yetkiler { get; set; }
        public Dictionary<int, Dictionary<string, YetkiKontrolData>> yetkilerGrup { get; set; }

        public class PageHeaderItem
        {
            public string pagetitle { get; set; }
            public string link { get; set; }
            public bool active { get; set; }
            public string ikon { get; set; }
        }
    }
}
```

### AdminJsonModel.cs — Standart JSON yanıt modeli

```csharp
namespace PixDinn.Models.AdminModels.AdminGlobalModels
{
    public class AdminJsonModel
    {
        public bool IsSuccess { get; set; }
        public string MessageBody { get; set; }
        public string MessageTitle { get; set; }
        public string Path { get; set; }
        public String redirectUrl { get; set; }
        public bool redirect { get; set; }
        public string type { get; set; }  // "login", "validation", "exception", "auth"
    }
}
```

### AdminLoginSessionResponse.cs — Oturum durumu + yetki

```csharp
namespace PixDinn.Models.AdminModels
{
    public class AdminLoginSessionResponse
    {
        public Loginstateenum loginstate { get; set; }
        public AdminLoginedUserModel loginedadmin { get; set; }
        public Dictionary<string, YetkiKontrolData> yetkiler { get; set; }
        public Dictionary<int, Dictionary<string, YetkiKontrolData>> yetkilerGrup { get; set; }

        public enum Loginstateenum { basarisiz, basarili }

        public class YetkiKontrolData
        {
            public bool isYetkili { get; set; }
        }

        // Normal admin yetki kontrolü
        public bool YetkiKontrol(string key)
        {
            try { return this.yetkiler[key].isYetkili; }
            catch { return false; }
        }

        // Mekan grup bazlı yetki kontrolü
        public bool YetkiKontrolGrup(string key, int marketId)
        {
            try { return this.yetkilerGrup[marketId][key].isYetkili; }
            catch { return false; }
        }
    }
}
```

### AdminLoginedUserModel.cs — Oturumdaki admin verisi

```csharp
namespace PixDinn.Models.AdminModels
{
    public class AdminLoginedUserModel
    {
        public int id { get; set; }
        public String isim { get; set; }
        public String soyisim { get; set; }
        public String userName { get; set; }
        public LoginAdminType loginAdminType { get; set; }
        public MekanObj mekanObj { get; set; }

        public class MekanObj
        {
            public String mekanSqlWhere { get; set; }   // "(1,2,3)" formatında SQL IN clause
            public List<Int32> mekanlar { get; set; }
        }
    }
}
```

### AdminHomeViewModel.cs

```csharp
public class AdminHomeViewModel : AdminPageGlobalModel
{
    public String seciliMenu { get; set; }
    public List<SelectListItem> menuler { get; set; }
}
```

### AdminHomePartialModel.cs — Dashboard partial için

```csharp
public class AdminHomePartialModel
{
    public int menuId { get; set; }
    public String menuAdi { get; set; }
    public EnCokGoruntulenenItem bugunEnCokGoruntulenenKategoriler { get; set; }
    public EnCokGoruntulenenItem buAyEnCokGoruntulenenKategoriler { get; set; }
    public EnCokGoruntulenenItem bugunEnCokGoruntulenenUrunler { get; set; }
    public EnCokGoruntulenenItem buAyEnCokGoruntulenenUrunler { get; set; }
    public EnCokGoruntulenenItem enCokGoruntulenenKategoriler { get; set; }
    public EnCokGoruntulenenItem enCokGoruntulenenUrunler { get; set; }
    public Dictionary<Int32, AnketResultSorular> anketResultJsonItems { get; set; }
    public int loginAdminType { get; set; }

    public class EnCokGoruntulenenItem
    {
        public int sayi { get; set; }
        public String isim { get; set; }
        public int indis { get; set; }
    }

    public class AnketResultSorular
    {
        public int id { get; set; }
        public String question { get; set; }
        public String valueText { get; set; }
        public List<AnketResultJsonItem> anketResultJsonItems { get; set; }
    }

    public class AnketResultJsonItem
    {
        public int soruId { get; set; }
        public String cevap { get; set; }
        public int yuzde { get; set; }
        public int cevapId { get; set; }
    }
}
```

### ListMenuViewModel.cs

```csharp
public class ListMenuViewModel : AdminPageGlobalModel
{
    public List<SelectListItem> mekanlar { get; set; }
    public String mekanId { get; set; }
}
```

**Diğer ViewModel'lar (özet):**

| Model | İçerik |
|---|---|
| `AddMenuViewModel` | Menü form: ack, menuUrl, qrCode, kategoriItems, geciciId |
| `MenuPartialViewModel` | Menü kategori gezinme partial |
| `ProductListViewModel` | Ürün listesi + mekanlar |
| `AdminProductViewModel` | Ürün detay/düzenleme |
| `AnketMainViewModel` | Anket listesi |
| `AnketAddViewModel` | Anket oluşturma formu |
| `IstatistikViewModel` | İstatistik ana sayfa |
| `RaporMainViewModel` | Rapor ana sayfa + detay |
| `AdminUsersViewModel` | Admin kullanıcı listesi |
| `AdminMekanGrupViewModel` | Mekan grup yönetimi |
| `AddUrunTempRequestModel` | Menüye ürün ekleme request |
| `AddUrunTempResponseModel` | QR PNG base64, geciciId, resultType |
| `UrunGrupYapPartialViewModel` | Ürün gruplama partial |
| `OrderProductsDetailModel` | Sipariş detay (ürün listesi) |
| `GorusOneriViewModel` | Görüş/öneri görüntüleme |
| `KampanyaViewModel` / `KampanyaAddViewModel` | Kampanya yönetimi |
| `BlogViewModel` | Blog listesi/ekleme |
| `TanitimSitesiAnketViewModel` | Tanıtım sitesi anketi |
| `ProductExcellModel` | Excel toplu ürün import/export |
| `ZamanAraligiViewModel` | Tarih aralığı filtresi |

---

## 7. Views (önemli view'lar)

### Views/AdminHome/Index.cshtml

```html
@model PixDinn.Models.AdminModels.AdminViewModels.AdminHomeViewModel
@{ Layout = "~/Views/Shared/_AdminLayout.cshtml"; }

@{Html.RenderAction("PageHeaderBlock", "AdminBlock", new AdminPageHeaderModel() { pagetitles = Model.pagetitles });}
@{Html.RenderAction("AlertDialog", "AdminHelper");}

<div id="OrderModalContainer"></div>
<div class="page-container">
    <div class="page-content">
        <div class="content-wrapper">
            <div class="row">
                <div class="col-md-6">
                    @Html.DropDownListFor(x => x.seciliMenu, Model.menuler,
                        new { @class = "select", @id = "seciliMenu", @onchange = "getDashBoardData()" })
                </div>
            </div>
            <br/>
            <div class="row" id="containerDashBoard"></div>
        </div>
    </div>
</div>

<script>
    $(function() {
        // Dashboard: menü seçilince AJAX ile getDashBoardData() action'ı çağrılır
        // Sonuç #containerDashBoard içine render edilir
    });
</script>
```

### Views/Menu/AddMenu.cshtml (kısmi yapı)

```html
@model PixDinn.Models.AdminModels.AdminViewModels.AddMenuViewModel
@{ Layout = "~/Views/Shared/_AdminLayout.cshtml"; }

@{
    // Yetki kontrolü: MEKAN_GRUP_USER ise grup bazlı, değilse global
    var menuDuzenlemeYetki = Model.loginedAdminType == LoginAdminType.MEKAN_GRUP_USER
        ? Model.yetkilerGrup[mekanId]["MENU_ISLEMLERI"]
        : Model.yetkiler["MENU_ISLEMLERI"];
}
@{Html.RenderAction("PageHeaderBlock", "AdminBlock", ...);}
@{Html.RenderAction("AlertDialog", "AdminHelper");}
@{Html.RenderAction("UrunSelectDialog", "Menu");}

<div class="page-container">
    <div class="page-content">
        <div class="content-wrapper">
            <!-- Menü adı, URL, QR code alanları -->
            <!-- Kategori ağacı (AJAX ile alt kategori/ürün seçimi) -->
            <!-- Ürün grupları düzenleme alanı -->
            <!-- QR kod görüntüleme butonu -->
        </div>
    </div>
</div>
```

### Views/ViewMenu/UrunlerGet.cshtml — Public ürün listesi

```html
@model List<PixDinn.Models.UserModels.MenuViewModel.UrunItem>
@{
    var urlHelper = new UrlHelper(Html.ViewContext.RequestContext);
    string baseUrl = urlHelper.Content("~");
}

@foreach (var item in Model)
{
    <div class="row">
        <div class="col-md-12">
            <input id="item-desc@(item.id)" style="display:none;" value="@(Html.Raw(item.ack))"/>
            <input id="item-url@(item.id)" style="display:none;" value="@(baseUrl)@(item.resimUrl)"/>
            <input id="item-title@(item.id)" style="display:none;" value="@(Html.Raw(item.name))"/>
            <input id="item-icons@(item.id)" style="display:none;" value="@(Html.Raw(item.iconsData))"/>

            <a class="btn btn-default form-control genel_button"
               href="javascript:void(0)" onclick="openModal(@item.id)">
                @(item.name)
                <strong class="price_button" style="float:right;">
                    @if (!item.metinselFiyatDurumu) {
                        @(item.fiyat + " " + item.fiyatTipi)
                    } else {
                        @(item.metinselFiyat + " " + item.fiyatTipi)
                    }
                </strong>
            </a>
        </div>
    </div>
}
```

### Views/ViewMenu/Product.cshtml — Mobil dark-theme menü UI

```html
<!-- Layout = null; Bağımsız tam sayfa HTML -->
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <!-- No-cache headers (HttpCacheability.NoCache) -->
    <style>
        body { background-color: #333333; }
        .kategori-item { border: 1px solid white; margin-bottom: 12px; }
        .urun-item { background-color: #414040 !important }
        .urun-item-inner { width: 100%; display: grid; min-height: 85px; }
        .urun-badgediv { width: 100%; background: #2998e9; height: 30px; }
        /* Koyu tema: #333333 bg, #2998e9 accent */
    </style>
</head>
<body>
    <!-- Ürün kartları: image + isim + fiyat + süre -->
    <!-- Modal: ürün detayı (büyük görsel + açıklama) -->
    <!-- Sıralama select: süre/fiyat ASC/DESC -->
</body>
</html>
```

### Views/AdminBlock/MenuBlock.cshtml — Admin sol menü

```html
@model PixDinn.Models.AdminModels.AdminViewModels.AdminGeneralBlockViewModel
@{ Layout = null; }

<div class="navbar navbar-default header_menu" id="navbar-second">
    <ul class="nav navbar-nav navbar-nav-material">
        <a class="navbar-brand" href="index.html">
            <img src="@(baseUrl)Content/MainTemplate/img/pixdinn.svg" />
        </a>
        <li><a href="/adminhome"><i class="icon-home position-left"></i> Anasayfa</a></li>
        <li class="dropdown">
            <a href="#" class="dropdown-toggle">Mekan Yönetimi</a>
            <ul class="dropdown-menu">
                <li><a href="/adminMekan/Index">Mekanlar</a></li>
                <li><a href="/adminDil/Index">Diller</a></li>
                @if (Model.yetkiler["MEKAN_KULLANICI_VE_YETKILENDIRME_ISLEMLERI"].isYetkili)
                {
                    <li><a href="/Yetki/Index">Yetkilendirme</a></li>
                    <li><a href="/AdminMekanUser/Index">Mekan Kullanıcılar</a></li>
                }
                @if (Model.adminType == (int)LoginAdminType.NORMAL || Model.adminType == (int)LoginAdminType.ROOT)
                {
                    <li><a href="/AdminMekanGrup/Index">Mekan Grupları</a></li>
                }
                <li><a href="/rapor/Index">Raporlar</a></li>
            </ul>
        </li>
        <!-- Menü Yönetimi, Ürün Yönetimi, Anket, Kampanya, Blog dropdown'ları -->
    </ul>
</div>
```

### Views/Shared/_Layout.cshtml

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - My ASP.NET Application</title>
    @Styles.Render("~/Content/css")
    @Scripts.Render("~/bundles/modernizr")
</head>
<body>
    <div class="navbar navbar-inverse navbar-fixed-top">
        <!-- Standard MVC default navbar -->
        @Html.ActionLink("Application name", "Index", "Home", ...)
    </div>
    <div class="container body-content">
        @RenderBody()
        <footer><p>&copy; @DateTime.Now.Year - My ASP.NET Application</p></footer>
    </div>
    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/bootstrap")
    @RenderSection("scripts", required: false)
</body>
</html>
```

*Not: Admin sayfaları `~/Views/Shared/_AdminLayout.cshtml` kullanır (ayrı layout, daha zengin). Tanıtım sitesi ve ViewMenu ise `Layout = null` ile tam bağımsız HTML üretir.*

---

## 8. App_Start Konfigürasyonu (tam kod)

### RouteConfig.cs

```csharp
namespace PixDinn
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();   // Attribute routing etkin

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
```

### WebApiConfig.cs

```csharp
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );
    }
}
```

### FilterConfig.cs

```csharp
public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HandleErrorAttribute());
        // AdminLoginFilterAttribute global olarak kayıtlı DEĞİL,
        // her action/controller üzerinde [AdminLoginFilterAttribute(...)] ile manuel uygulanır
    }
}
```

### BundleConfig.cs

```csharp
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jquery")
            .Include("~/Scripts/jquery-{version}.js"));
        bundles.Add(new ScriptBundle("~/bundles/jqueryval")
            .Include("~/Scripts/jquery.validate*"));
        bundles.Add(new ScriptBundle("~/bundles/modernizr")
            .Include("~/Scripts/modernizr-*"));
        bundles.Add(new ScriptBundle("~/bundles/bootstrap")
            .Include("~/Scripts/bootstrap.js"));
        bundles.Add(new StyleBundle("~/Content/css")
            .Include("~/Content/bootstrap.css", "~/Content/site.css"));
    }
}
```

### DependencyConfig.cs — Autofac DI

```csharp
namespace PixDinn.App_Start
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // MVC controller'ları Autofac'a kayıt
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterFilterProvider();
            builder.RegisterSource(new ViewRegistrationSource());

            // Redis Cache — Singleton (tek bağlantı havuzu)
            builder.RegisterType<RedisCacheService>()
                .As<IRedisCacheService>()
                .SingleInstance();

            // EF DbContext — InstancePerRequest (request başına yeni instance)
            builder.RegisterType<DBservices.pixdinnEntities>()
                .InstancePerRequest();

            // MenuService ve Repositories yorumda — henüz tam implement edilmemiş

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
```

### Global.asax.cs

```csharp
namespace PixDinn
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // DependencyConfig.RegisterDependencies() — çağrılmış olabilir, Global.asax'ta görünmüyor
        }
    }
}
```

---

## 9. Web.config Yapısı

```xml
<configuration>
  <configSections>
    <!-- EF 6 konfigürasyon section -->
    <section name="entityFramework"
             type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0 ..."/>
  </configSections>

  <appSettings>
    <add key="webpages:Version" value="3.0.0.0"/>
    <add key="webpages:Enabled" value="false"/>
    <add key="ClientValidationEnabled" value="true"/>
    <add key="UnobtrusiveJavaScriptEnabled" value="true"/>
  </appSettings>

  <system.web>
    <compilation debug="true" targetFramework="4.7.2"/>
    <httpRuntime targetFramework="4.7.2" maxRequestLength="1048576"/>  <!-- 1 GB max upload -->
  </system.web>

  <connectionStrings>
    <!-- Üretim sunucusu (IP ve credentials maskele) -->
    <add name="pixdinnEntities"
         connectionString="metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;
                           provider=System.Data.SqlClient;
                           provider connection string=&quot;
                             data source=***.***.*.3;
                             initial catalog=pixdinn;
                             user id=pixdinn;
                             password=***MASKED***;
                             MultipleActiveResultSets=True;
                             App=EntityFramework&quot;"
         providerName="System.Data.EntityClient"/>
    <!--
    Yorum satırında local geliştirme bağlantısı:
    data source=DESKTOP-QSS6NQF\MSSQLSERVERALI;integrated security=True;...
    -->
  </connectionStrings>

  <entityFramework>
    <providers>
      <provider invariantName="System.Data.SqlClient"
                type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer"/>
    </providers>
  </entityFramework>

  <system.webServer>
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="1073741824"/>  <!-- 1 GB IIS -->
      </requestFiltering>
    </security>
    <handlers>
      <!-- Extensionless URL handler (MVC routing için) -->
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*"
           type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,..."/>
    </handlers>
  </system.webServer>

  <runtime>
    <!-- Assembly binding redirects: Newtonsoft.Json 12.x, MVC 5.2.7.0, EF SqlServer vs. -->
  </runtime>

  <system.codedom>
    <!-- Roslyn compiler (C# 7.x özellikler için) -->
  </system.codedom>
</configuration>
```

---

## 10. QR Kod Üretimi

**Kütüphane:** `QRCoder` (NuGet paketi, `QRCoder.dll` bin içinde)  
**Kullanım yeri:** `MenuController.qrMenu()` ve `MenuController.qrMenuSiparis()`

```csharp
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// URL oluşturma
string url;
if (string.IsNullOrEmpty(menu.MENU_URL))
    url = "https://pixdinn.com/viewmenu?data=" + menu.QR_CODE;
else
    url = "https://" + menu.MENU_URL + "/viewmenu?data=" + menu.QR_CODE;

// QR PNG üretimi
using (MemoryStream ms = new MemoryStream())
{
    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
    QRCode qrCode = new QRCode(qrCodeData);

    using (Bitmap bitMap = qrCode.GetGraphic(20))  // 20 = piksel büyüklüğü
    {
        bitMap.Save(ms, ImageFormat.Png);
        // Base64 string olarak JSON ile döndürülüyor
        string base64 = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
    }
}
```

**QR Code token:** `menu.QR_CODE` — 120 karakter random string (`Singleton.getRandomString(120)`). URL: `https://pixdinn.com/viewmenu?data={token}` veya özel domain ile `https://{menuUrl}/viewmenu?data={token}`.

**İki QR tipi:**
- `QR_CODE` — Normal menü görüntüleme
- `QR_CODE_SIPARIS` — Sipariş verebilir menü (ayrı 90-char token)

---

## 11. Authentication Mekanizması (Session tabanlı)

Proje ASP.NET Identity veya Forms Authentication kullanmaz. Tamamen özel Session + Cookie tabanlı bir mekanizma vardır.

### Oturum Açma Akışı

```
1. GET /adminlogin
   → AdminLoginDevices tablosuna IP+UserAgent kaydedilir
   → AdminLogins tablosuna 120-char random pageKey oluşturulur
   → pageKey view'a gönderilir (hidden field)

2. POST /adminlogin/loginpost
   → pageKey doğrulanır (DB'deki ile karşılaştırma)
   → 3 saniye minimum bekleme koruması (bot engeli)
   → USERNAME + MD5(PASSWORD) veritabanında aranır
   → Başarılıysa AdminLoginSessions tablosuna:
       SESSION_ID, SESSION_KEY, COOKIE_ID, COOKIE_KEY (120-char random) yazılır
   → Singleton.getAdminLoginProcess().saveSession(...) çağrılır
   → pageKey sıfırlanır (replay saldırısı engeli)

3. Her request'te yetki kontrolü [AdminLoginFilterAttribute] ile yapılır
```

### Yetki Sistemi

```csharp
// Attribute kullanımı
[AdminLoginFilterAttribute(
    pageName = GlobalEnums.PageNameAdmin.ADMIN_MENU,
    openingType = GlobalEnums.PageOpeningType.DIRECT)]
public ActionResult Index() { ... }

// Controller içi yetki kontrolü (4 farklı admin türü)
var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess().getLoginState();

var isYetkili = loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER
    ? loginState.YetkiKontrolGrup("MENU_ISLEMLERI", mekanId)  // Grup bazlı
    : loginState.YetkiKontrol("MENU_ISLEMLERI");               // Global
```

### Admin Türleri (LoginAdminType enum)

| Değer | Açıklama |
|---|---|
| `ROOT` | Süper admin (RootAdmin tablosu) |
| `NORMAL` | Normal admin (tüm mekanlara erişim) |
| `MEKAN_USER` | Tek mekan kullanıcısı |
| `MEKAN_GRUP_USER` | Birden fazla mekan, grup bazlı yetki |

### Çıkış Mekanizması

Logout yaparken kullanıcının DB'deki tüm SESSION ve COOKIE tokenları yeni random değerlerle değiştirilerek geçersiz kılınır (token invalidation). Fiziksel session silme yerine token rotation.

---

## 12. Redis Cache Kullanımı

Projede iki ayrı Redis katmanı bulunmaktadır:

### Modern Katman: IRedisCacheService / RedisCacheService

```csharp
// Autofac Singleton olarak kayıtlı
public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisCacheService()
    {
        _settings = ConfigurationHelper.GetRedisSettings();
        _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions {
            EndPoints = { { _settings.Host, _settings.Port } },
            Password = _settings.Password,
            Ssl = _settings.Ssl,
            ConnectRetry = _settings.ConnectRetry,
            KeepAlive = 60,
            ReconnectRetryPolicy = new ExponentialRetry(5000)
        });
        _db = _redis.GetDatabase(_settings.DatabaseId);
    }

    // String, Hash, List, Set, SortedSet operasyonları (async + sync)
    // Cache-Aside pattern:
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null) return cached;
        var value = await factory();
        if (value != null) await SetAsync(key, value, expiry);
        return value;
    }
}
```

### Legacy Katman: RedisCacheManager (Singleton pattern)

Controller'larda yaygın kullanılan eski katman:

```csharp
RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
    .getRedisCacheManager();

// Menü değişikliği sonrası cache invalidate
QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
qrMenuDbProcess.MenuId = menu.ID;
qrMenuDbProcess.MenuUpdateDate = DateTime.Now;
manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);  // 480 dk TTL

// ViewMenu'da: cache var mı kontrolü
bool isExist = manager.IsExistQrMenuRedisProcess(redisKey);
bool isUpdated = manager.IsQrMenuRedisDataUpdated(menuCode.ID, redisKey, db);

// Cache'den MenuViewModel okuma
var menuViewModel = manager.Get<MenuViewModel>(redisKey);

// IP ban kontrolü
bool ipBanCheck = manager.CheckBannedIpAddres(ipAddress);
```

### Redis Cache Key Yapısı

```
"qrCode{menuId}{typePage}{id}{dilCookie}{dilMekan}"  — MenuViewModel cache
"qrMenuProcess:{menuId}"                              — DB değişiklik işareti
"menu:{id}"                                           — MenuService cache (yeni)
"menu:{id}:details"                                   — Detaylı menü
"menus:active"                                        — Aktif menüler listesi
"venue:{id}:menus"                                    — Mekan menüleri
```

### MenuService (Cache-Aside pattern — yeni katman)

```csharp
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IRedisCacheService _cache;

    public async Task<DBservices.Menu> GetMenuAsync(int menuId, bool bypassCache = false)
    {
        var cacheKey = CacheKeys.Menu(menuId);
        if (!bypassCache)
        {
            var cached = await _cache.GetAsync<DBservices.Menu>(cacheKey);
            if (cached != null) return cached;
        }
        var menu = await _menuRepository.GetByIdAsync(menuId);
        if (menu != null && !bypassCache)
        {
            await _cache.SetAsync(cacheKey, menu,
                TimeSpan.FromMinutes(_cacheSettings.MenuExpirationMinutes));
        }
        return menu;
    }

    public async Task InvalidateMenuCacheAsync(int menuId)
    {
        await _cache.RemoveAsync(CacheKeys.Menu(menuId));
        await _cache.RemoveAsync($"{CacheKeys.Menu(menuId)}:details");
        await _cache.RemoveAsync(CacheKeys.MenuProducts(menuId));
        await _cache.RemoveByPatternAsync($"menu:{menuId}:*");
    }
}
```

---

## 13. Mimari Kararlar

### 1. Singleton Tabanlı Global Erişim (Anti-pattern)
```csharp
// Tüm controller'larda bu pattern var:
Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess()
Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getRedisCacheManager()
```
`OLD_OBJECT` ve `NEW_OBJECT` enum'ları iki farklı Singleton instance'ı temsil eder. Session, yetki, yardımcı metodlar buradan erişilir. Test edilemez, stateful yapı.

### 2. `using (var db = new pixdinnEntities())` Her Yerde
Repository veya Unit of Work yoktur. Her controller method kendi DbContext açar-kapatır. Bu her request'te yeni bir DB bağlantısı demektir.

### 3. SQL String Concatenation (Güvenlik Riski)
```csharp
var query = @"... where YEAR(MOVING_DATE) = " + DateTime.Now.Year +
            " and MENU_ID=" + menuId + ...
```
Parametreler bazı yerlerde kullanılıyor ama int dönüşümlü değerlerde SQL injection riski sınırlı; ancak string değerler için tehlikeli.

### 4. Mekan Bazlı Multi-tenancy
```csharp
var mekanObj = loginState.loginedadmin.mekanObj;
var sqlExtra = mekanObj != null ? " AND M.ID IN " + mekanObj.mekanSqlWhere : "";
```
Her sorguda `mekanSqlWhere` string'i SQL'e eklenerek veri izolasyonu sağlanır. `mekanSqlWhere` "(1,2,3)" formatında sabit stringe compile edilir — dinamik SQL injection riski var.

### 5. Redis ile Menü Cache Stratejisi
Menü verisi değiştiğinde `QrMenuDbProcessModel` kaydedilir. ViewMenu her açıldığında bu kayıt kontrol edilir. Cache güncel ise DB'ye gidilmez — yüksek trafik için etkin strateji.

### 6. Karışık Mimari Geçişi
- Eski kod: Singleton + using(db) doğrudan
- Yeni eklenen: Autofac DI + IRedisCacheService + IMenuRepository + MenuService
- İki katman birlikte çalışıyor, refactor tamamlanmamış

### 7. DataTables Server-Side Entegrasyonu
```csharp
var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
    .getdataTableProcess()
    .getDataTableData("Urunler", pData, query, "U.ID asc");
return Json(datatableData);
```
Tüm listeleme işlemleri DataTables server-side yapılır. Pagination, sıralama, arama sunucu tarafında gerçekleşir.

---

## 14. Modern .NET ile Karşılaştırma

| Konu | Bu Proje (ASP.NET MVC 5 / .NET 4.7.2) | Modern (.NET 8 / ASP.NET Core) |
|---|---|---|
| ORM | EF 6 Database-First (partial class) | EF Core 8 Code-First / Dapper |
| DI | Autofac (kısmen) + Singleton anti-pattern | Built-in DI Container |
| Auth | Özel Session + Cookie + DB token | ASP.NET Core Identity / JWT / Cookie Auth |
| Password | MD5 (güvensiz) | BCrypt / ASP.NET Core Identity hashing |
| Routing | Convention + Attribute routing | Minimal API / Attribute routing |
| Async | Kısmen Task/async (ViewMenuController) | Tam async pipeline |
| Config | Web.config | appsettings.json + IOptions |
| Caching | Redis (StackExchange) — iki katman | IMemoryCache + IDistributedCache |
| Testing | Test edilemez (Singleton) | DI tabanlı, unit-testable |
| Deployment | IIS + .NET Framework | Docker / Kestrel / IIS |
| Bundling | System.Web.Optimization | LibMan / Vite / esbuild |
| JSON | Newtonsoft.Json | System.Text.Json (veya Newtonsoft) |
| Excel | SpreadsheetLight + Interop | EPPlus / ClosedXML (Interop'suz) |
| QR Kod | QRCoder + System.Drawing | QRCoder (cross-platform) + SkiaSharp |
