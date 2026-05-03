# qr_menu_mvc — Çıkarılan Kurallar

Bu dosya, projeyi analiz ederek çıkarılan tekrar edilebilir kural ve pattern'leri içerir.
Aynı zamanda bu teknoloji yığınından kaçınılması veya modernizasyon yapılırken referans alınabilir.

---

## 1. ASP.NET MVC 5 Controller Pattern

**Kural:** MVC 5 controller'ı şu şekilde yapılandır:

```csharp
public class MenuController : Controller
{
    private readonly MenuDBService _menuService;
    private readonly KategoriDBService _kategoriService;

    public MenuController()
    {
        _menuService = new MenuDBService();
        _kategoriService = new KategoriDBService();
    }

    // GET: /Menu/Index
    public ActionResult Index(int mekanId)
    {
        var menus = _menuService.GetMenusByMekan(mekanId);
        return View(menus);
    }

    // POST: /Menu/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(MenuViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _menuService.CreateMenu(model);
        return RedirectToAction("Index", new { mekanId = model.MekanId });
    }

    // AJAX / Web API çağrısı için JSON action
    public JsonResult GetMenuJson(int mekanId)
    {
        var menus = _menuService.GetMenusByMekan(mekanId);
        return Json(menus, JsonRequestBehavior.AllowGet);
    }
}
```

**Kural:** `[ValidateAntiForgeryToken]` tüm POST action'larına ekle. CSRF koruması zorunlu.

**Kural:** Admin controller'ları `[Authorize]` attribute ile koru. Public (müşteri-facing) sayfalar authorize gerektirmesin.

---

## 2. DBservices Katmanı Pattern

**Kural:** EF context'i doğrudan controller'da kullanma. `DBservices` katmanında sarmalayan servis sınıfı oluştur:

```csharp
// DBservices/MenuDBService.cs
public class MenuDBService
{
    private QrMenuEntities GetContext() => new QrMenuEntities();

    public List<Menu> GetActiveMenus(int mekanId)
    {
        using (var context = GetContext())
        {
            return context.Menus
                          .Where(m => m.MekanId == mekanId && m.IsActive == true)
                          .OrderBy(m => m.SiraNo)
                          .ToList();
        }
    }

    public void CreateMenu(Menu menu)
    {
        using (var context = GetContext())
        {
            context.Menus.Add(menu);
            context.SaveChanges();
        }
    }

    public bool UpdateMenu(Menu menu)
    {
        using (var context = GetContext())
        {
            var existing = context.Menus.Find(menu.Id);
            if (existing == null) return false;
            // güncelle
            context.Entry(existing).CurrentValues.SetValues(menu);
            context.SaveChanges();
            return true;
        }
    }
}
```

**Kural:** Her `using (var context = GetContext())` bloğu transaction sınırını belirler. Bağlantı yönetimini açık tut.

**Kural:** DBservice sınıfları context'i alan constructor ile de desteklesin (test için):
```csharp
public class MenuDBService
{
    private readonly Func<QrMenuEntities> _contextFactory;

    public MenuDBService() : this(() => new QrMenuEntities()) { }
    public MenuDBService(Func<QrMenuEntities> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
```

---

## 3. Database-First EF 6 Kullanımı

**Kural:** Otomatik üretilen Entity sınıflarını değiştirme. Partial class kullan:
```csharp
// Otomatik üretilen (değiştirme):
// public partial class Menu { ... }

// Kendi partial class'ın (değiştirilebilir):
public partial class Menu
{
    // EF modelinde olmayan hesaplanmış property
    public string DisplayName => $"{SiraNo}. {Ad}";

    // Validation attribute'lar
    [Display(Name = "Menü Adı")]
    public string Ad { get; set; }  // Bu alanı override et
}
```

**Kural:** EDMX model değiştirildiğinde otomatik üretilen `*.Designer.cs` ve `*.Context.cs` dosyaları üzerine yazılır. Özel kodun hepsini partial class'larda tut.

---

## 4. Oturum Yönetimi (Session)

**Kural:** MVC 5'te admin oturumu Session ile yönet:
```csharp
// Login
Session["AdminUser"] = adminUser;
Session["MekanId"] = adminUser.MekanId;

// Logout
Session.Clear();
Session.Abandon();

// Check
if (Session["AdminUser"] == null)
    return RedirectToAction("Login", "AdminLogin");
```

**Kural:** Custom `[AuthorizeAdmin]` filter attribute oluştur:
```csharp
public class AuthorizeAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session["AdminUser"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "AdminLogin", action = "Login" }));
        }
        base.OnActionExecuting(filterContext);
    }
}
```

---

## 5. Web.config Yapısı

**Kural:** Connection string'leri `<connectionStrings>` bölümüne koy:
```xml
<connectionStrings>
  <add name="QrMenuEntities"
       connectionString="metadata=...;provider=System.Data.SqlClient;
       provider connection string=&quot;data source=[SERVER];
       initial catalog=[DB];persist security info=True;
       user id=[USER];password=[PASSWORD];App=EntityFramework&quot;"
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

**Kural:** Uygulama ayarları `<appSettings>` bölümüne:
```xml
<appSettings>
  <add key="SiteTitle" value="QR Menü Sistemi" />
  <add key="DefaultImagePath" value="/Content/images/default-product.png" />
  <add key="MaxUploadSizeMB" value="5" />
</appSettings>
```

**Kural:** Production `Web.config`'i kaynak kontrolüne ekleme. `Web.config.template` versiyonunu ekle, gerçek connection string'i production sunucusunda ayrı tut.

---

## 6. QR Kod Oluşturma Pattern

**Kural:** QR kod URL yapısı tutarlı ve kısa olsun:
```
https://menu.domain.com/m/{mekanKod}
https://menu.domain.com/m/{mekanKod}/{kategoriId}
```

**Kural:** Her mekanın benzersiz bir `mekanKod` (slug) alanı olsun. ID yerine kod kullan — tahmin edilemez ve değiştirilebilir.

**Kural:** QR kod görseli için `QRCoder` veya `ZXing.Net` NuGet paketi kullan. Server-side üret, binary/base64 olarak döndür:
```csharp
public byte[] GenerateQrCode(string url)
{
    using var generator = new QRCodeGenerator();
    var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
    using var code = new PngByteQRCode(data);
    return code.GetGraphic(10);
}
```

---

## 7. Modernizasyon Rehberi

Bu proje modernize edilecekse şu adımları takip et:

**Kural — Adım 1: .NET Framework → .NET 8**
- Projeyi `<Project Sdk="Microsoft.NET.Sdk.Web">` SDK-style'a çevir
- `packages.config` → `PackageReference` formatına geç
- `Web.config` → `appsettings.json`'a taşı

**Kural — Adım 2: EF 6 Database-First → EF Core Code-First**
- Mevcut şemayı `dotnet ef dbcontext scaffold` ile tersine mühendis et
- EDMX'i kaldır, EF Core migration'larına geç
- `QrMenuEntities` → `QrMenuDbContext` (EF Core DbContext)

**Kural — Adım 3: MVC 5 → ASP.NET Core MVC**
- Controller'lar `Microsoft.AspNetCore.Mvc.Controller`'dan türesin
- Session → Cookie auth + JWT
- `Web.config` action filter'ları → ASP.NET Core middleware

**Kural — Adım 4: DBservices → Repository Pattern**
- `DBservices` sınıfları → `IMenuRepository`, `IProductRepository` interface'leri
- Autofac veya built-in DI ile inject et
- `new MenuDBService()` → constructor injection

**Kural:** Modernizasyon aşamalı yapılmalı. Tüm sistemi aynı anda değiştirme; önce veri katmanını, sonra iş mantığını, son olarak presentation'ı güncelle.
