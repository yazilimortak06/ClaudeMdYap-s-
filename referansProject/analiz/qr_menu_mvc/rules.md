# qr_menu_mvc — Çıkarılan Kurallar (Kod Örnekleriyle)

Bu dosya, QrMenu projesini (ASP.NET MVC 5 / .NET 4.7.2) tam okuyarak çıkarılan
tekrar edilebilir pattern, anti-pattern ve mimari kararları içerir.
Modern .NET geliştirirken referans alınacak "bu projedeki yaklaşım neydi" kataloğu.

---

## 1. Controller Yapısı (MVC 5 Pattern)

### Referans projede nasıl yapılmış:

```csharp
// Her controller: Controller base'den türer, DI yok, Singleton kullanır
public class MenuController : Controller
{
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MENU,
                               openingType = GlobalEnums.PageOpeningType.DIRECT)]
    public ActionResult Index()
    {
        ListMenuViewModel vm = new ListMenuViewModel();
        vm.menuname = "Menüler";
        vm.pagetitles = new List<AdminPageGlobalModel.PageHeaderItem>();
        vm.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
        return View(vm);
    }

    [HttpPost]
    [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_MENU_POST,
                               openingType = GlobalEnums.PageOpeningType.AJAX)]
    public ActionResult addMenu(AddMenuViewModel addMenu)
    {
        AdminJsonModel result = new AdminJsonModel();
        result.IsSuccess = false;
        try
        {
            using (var db = new DBservices.pixdinnEntities())
            {
                // iş mantığı buraya
                db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                result.IsSuccess = true;
                result.redirect = true;
                result.redirectUrl = "/menu";
            }
        }
        catch (Exception ee)
        {
            return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
        }
        return Json(result, JsonRequestBehavior.AllowGet);
    }
}
```

**Çıkarılan kurallar:**
- Her action kendi `using (var db = new pixdinnEntities())` açar.
- Her action başında `AdminJsonModel result = new AdminJsonModel();` ile dönüş modeli hazırlanır.
- Exception handling hep `catch (Exception ee)` + `getExceptionError(ee)` ile yapılır.
- Yetki kontrolü attribute + içeride manuel `YetkiKontrol()/YetkiKontrolGrup()` ile çift katmanlı.

---

## 2. DbContext Kullanım Pattern'i

### Referans projede:

```csharp
// Her yerde aynı using pattern:
using (var db = new DBservices.pixdinnEntities())
{
    // Okuma:
    var urun = db.Urunler
        .Where(u => u.ID == id && u.STATE != 3)
        .FirstOrDefault();

    // Ekleme:
    var yeni = new DBservices.Urunler();
    yeni.MEKAN_ID = mekanId;
    yeni.STATE = (int)GlobalEnums.AdminDataState.ACTIVE;
    yeni.CREATED_DATE = DateTime.Now;
    db.Urunler.Add(yeni);
    db.SaveChanges();

    // Güncelleme:
    urun.STATE = (int)GlobalEnums.AdminDataState.PASSIVE;
    urun.UPDATED_DATE = DateTime.Now;
    db.Entry(urun).State = System.Data.Entity.EntityState.Modified;
    db.SaveChanges();

    // Silme (soft delete — STATE = 3):
    urun.STATE = (int)GlobalEnums.AdminDataState.DELETED;
    db.Entry(urun).State = System.Data.Entity.EntityState.Modified;
    db.SaveChanges();
}
```

**Çıkarılan kurallar:**
- Fiziksel silme yoktur. `STATE = 3 (DELETED)` ile soft delete.
- `STATE` değerleri: `1=ACTIVE, 2=PASSIVE, 3=DELETED`.
- Update için mutlaka `db.Entry(entity).State = EntityState.Modified; db.SaveChanges();` çağrısı.
- Repository pattern yoktur — DbContext doğrudan controller'dan kullanılır.

---

## 3. Ham SQL + DataTable Pattern (Raporlama)

### Referans projede:

```csharp
// Tek SQL sorgusuyla DataTable çekme:
var query = @"
    select MOVING_ID as ID, count(MOVING_ID) as COUNT_DATA
    from UserMoving
    where YEAR(MOVING_DATE) = " + DateTime.Now.Year + @"
      and MONTH(MOVING_DATE) = " + DateTime.Now.Month + @"
      and MOVING_NAME = 'KATEGORI'
      and MOVING_ID != 0
      and MENU_ID = " + menuId + @"
    group by MOVING_ID";

var result = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getDataTableData(
        db,
        query,
        new List<string>() { "ID", "COUNT_DATA" },
        new List<SqlParameter>());

// result: List<Dictionary<string, string>>
result.ForEach(dd => {
    var id = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
        .getInteger(dd["ID"]);
    var count = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
        .getInteger(dd["COUNT_DATA"]);
});
```

**DataTables server-side için:**
```csharp
[HttpPost]
public ActionResult getSiparislerData(string pData)
{
    var query = @"select S.ID, S.TOTAL_PRICE, U.NAME, U.PHONE
                  from Siparisler S LEFT join Users U on U.ID=S.USER_ID
                  WHERE U.ID!=0" + sqlExtra;

    var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
        .getdataTableProcess()
        .getDataTableData("Siparisler", pData, query, "S.ID asc");

    return Json(datatableData);
}
```

**Çıkarılan kurallar:**
- Karmaşık raporlar EF LINQ yerine ham SQL ile yapılır.
- `pData` parametresi DataTables'ın server-side JSON request payload'ı.
- SQL string concatenation ile integer değerler birleştirilir (string injection için hala riskli).
- `getDataTableData` metodu `pData` içindeki sayfa/sıralama/arama bilgisini parse eder.

---

## 4. Multi-Tenancy (Mekan Bazlı Veri İzolasyonu)

### Referans projede:

```csharp
// Her sorguda mekan filtresi:
var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess()
    .getLoginState()
    .loginedadmin.mekanObj;

// mekanObj == null ise ROOT/NORMAL admin (tüm mekanlar)
// mekanObj != null ise MEKAN_GRUP_USER (sadece kendi mekanlari)
var sqlExtra = mekanObj != null
    ? " AND M.ID IN " + mekanObj.mekanSqlWhere   // "(1,2,3)" formatı
    : "";

var query = @"SELECT ... FROM MenulerTemp MT
              LEFT JOIN Mekanlar M ON MT.MEKAN_ID = M.ID
              WHERE MT.STATE != 3"
              + sqlExtra;  // Her sorguda eklenir
```

**Yetki kontrolü (controller içi):**
```csharp
var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess().getLoginState();

var isYetkili = false;
if (loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER)
{
    // Grup kullanıcısı: mekan bazlı yetki (dictionary key: mekanId)
    isYetkili = loginState.YetkiKontrolGrup("MENU_ISLEMLERI", mekanId);
}
else
{
    // Normal/Root admin: global yetki
    isYetkili = loginState.YetkiKontrol("MENU_ISLEMLERI");
}

if (!isYetkili)
{
    result.IsSuccess = false;
    result.MessageBody = "Yetkiniz Bulunmamaktadır";
    return Json(result);
}
```

**Çıkarılan kurallar:**
- Veri izolasyonu SQL seviyesinde `AND MEKAN_ID IN (...)` ile yapılır.
- `mekanObj.mekanSqlWhere` string'i login sırasında oluşturulur, her request'te tekrar hesaplanmaz.
- Yetki isimleri string sabitlerdir: `"MENU_ISLEMLERI"`, `"MEKAN_KULLANICI_VE_YETKILENDIRME_ISLEMLERI"`.

---

## 5. Authentication Pattern (Session + Cookie Tokenları)

### Referans projede:

```csharp
// Login sırasında token çiftleri üretilir:
String cookieKey  = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
String cookieId   = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
String sessionKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
String sessionId  = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);

// DB'ye yazılır:
var insertedSession = new AdminLoginSessions();
insertedSession.SESSION_ID  = sessionId;
insertedSession.SESSION_KEY = sessionKey;
insertedSession.COOKIE_ID   = cookieId;
insertedSession.COOKIE_KEY  = cookieKey;
insertedSession.USER_ID     = loginSessionRequest.userId;
insertedSession.USER_TYPE   = (int)loginSessionRequest.adminType;
insertedSession.STATE       = (int)GlobalEnums.AdminLoginSessionState.ACTIVE;
db.AdminLoginSessions.Add(insertedSession);
db.SaveChanges();

// Singleton'a da kaydedilir:
Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess()
    .saveSession(loginSessionRequest);

// Logout: tokenları sıfırla (fiziksel silme yok)
db.AdminLoginSessions.Where(pp =>
    pp.USER_ID == userId && pp.USER_TYPE == (int)adminType)
    .ToList().ForEach(ii => {
        ii.COOKIE_KEY  = Singleton.getSingleton(...).getRandomString(120);
        ii.SESSION_ID  = Singleton.getSingleton(...).getRandomString(120);
        ii.COOKIE_ID   = Singleton.getSingleton(...).getRandomString(120);
        ii.SESSION_KEY = Singleton.getSingleton(...).getRandomString(120);
    });
db.SaveChanges();
```

**Anti-replay koruması:**
```csharp
// Login sayfasına gelen random pageKey her gönderide sıfırlanır:
adminLogin.LOGIN_KEY_DATA = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getRandomString(120);
db.Entry(adminLogin).State = System.Data.Entity.EntityState.Modified;
db.SaveChanges();

// 3 saniye minimum bekleme (bot koruması):
var seconds = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .dateCompareSeconds(adminLogin.LOGIN_PAGE_STATE_DATE.Value, DateTime.Now);
if (seconds > 0 && seconds < 3)
{
    result.redirect = true;
    result.redirectUrl = "/adminlogin";
    return Json(result);
}
```

**Çıkarılan kurallar:**
- MD5 şifreleme (güvensiz): `Singleton...decryptMd5(password)` — modern sistemlerde BCrypt/Argon2.
- Her login denemesinde pageKey refresh zorunludur (tek kullanımlık).
- Logout token rotation yaklaşımı: session'ı silmek yerine tokenları değiştir.
- IP ban: `AdminLoginDevices.DEVICE_ACCESS_STATE = IP_BANNED`.

---

## 6. QR Kod Üretimi Pattern'i

### Referans projede:

```csharp
// Menü QR kodu üretimi — Base64 PNG olarak JSON'la döner
[HttpPost]
public ActionResult qrMenu([DefaultValue(0)] int geciciId)
{
    AddUrunTempResponseModel response = new AddUrunTempResponseModel();
    try
    {
        using (var db = new DBservices.pixdinnEntities())
        {
            var menu = db.MenulerTemp.Where(mt => mt.ID == geciciId).FirstOrDefault();
            if (menu != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // URL oluşturma (özel domain desteği)
                    string url = string.IsNullOrEmpty(menu.MENU_URL)
                        ? "https://pixdinn.com/viewmenu?data=" + menu.QR_CODE
                        : "https://" + menu.MENU_URL + "/viewmenu?data=" + menu.QR_CODE;

                    // QRCoder kütüphanesi
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);

                    // Bitmap -> MemoryStream -> Base64
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        response.data = "data:image/png;base64," +
                            Convert.ToBase64String(ms.ToArray());
                        response.success = true;
                        response.url = url;
                    }
                }
            }
        }
    }
    catch (Exception ee) { /* sessiz fail */ }
    return Json(response, JsonRequestBehavior.AllowGet);
}
```

**Token yönetimi:**
```csharp
// QR token oluşturma (menü ilk kaydedildiğinde):
inserted.QR_CODE = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getRandomString(120);  // 120-char random alfanümerik

// Sipariş QR kodu (isteğe bağlı, ayrı token):
if (string.IsNullOrEmpty(menu.QR_CODE_SIPARIS))
{
    menu.QR_CODE_SIPARIS = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
        .getRandomString(90);
    db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
    db.SaveChanges();
}
```

**Çıkarılan kurallar:**
- QR kod token'ı veritabanında saklanır (menülertemp.QR_CODE).
- Base64 PNG JSON response — frontend `<img src="data:image/png;base64,...">` ile gösterir.
- İki farklı QR tipi: menü görüntüleme + sipariş verebilir menü.
- `QRCodeGenerator.ECCLevel.Q` = %25 hata düzeltme kapasitesi.

---

## 7. Redis Cache Pattern'i

### Referans projede (menü cache döngüsü):

```csharp
// 1. Admin panelinde menü değişikliği sonrası cache invalidate:
try
{
    QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
    RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT)
        .getRedisCacheManager();
    qrMenuDbProcess.MenuId = menu.ID;
    qrMenuDbProcess.MenuUpdateDate = DateTime.Now;
    manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);  // 480 dk = 8 saat TTL
}
catch (Exception ee)
{
    // Redis çöksede uygulama çalışmaya devam etmeli
}

// 2. Public menü sayfasında (ViewMenuController):
string redisKey = "qrCode" + menuCode.ID + typePage + id + dilCookie + dilMekan;
bool isExistDbProcess = manager.IsExistQrMenuDbProcess(menuCode.ID, db);
bool isRedisUpdated   = manager.IsQrMenuRedisDataUpdated(menuCode.ID, redisKey, db);
bool isCacheSet       = manager.IsSet(redisKey);

if (manager.IsExistQrMenuRedisProcess(redisKey)
    && (!isExistDbProcess || isRedisUpdated)
    && isCacheSet
    && menuDesign == ""
    && isEntrance == false)
{
    // Cache HIT: Redis'ten oku
    var menuViewModel = manager.Get<MenuViewModel>(redisKey);
    // Kullanıcıya özel veri ekle (sepet, dil, kampanya)
    return View(menuViewModel);
}

// Cache MISS: DB'den oluştur, Redis'e yaz
var freshViewModel = BuildMenuViewModelFromDb(menuCode, db, ...);
manager.Set(redisKey, freshViewModel, TimeSpan.FromMinutes(480));
return View(freshViewModel);
```

**Çıkarılan kurallar:**
- Redis hataları sessizce (`try/catch`) yutulur; uygulama DB'den devam eder.
- Cache key: `"qrCode{menuId}{typePage}{id}{dilCookie}{dilMekan}"` — çok boyutlu key.
- Menü güncellenince DB'ye `QrMenuDbProcess` kaydı konur; viewer bu kaydı kontrol ederek cache'i bypass eder.
- Cache TTL: 480 dakika (8 saat).

---

## 8. Çoklu Dil (i18n) Pattern'i

### Referans projede:

```csharp
// Her mekanın öncelikli dili vardır (ONCELIKLI=1):
var lang = db.Languages.Where(l =>
    l.MEKAN_ID == mekanId &&
    l.STATE == (int)GlobalEnums.AdminDataState.ACTIVE &&
    l.ONCELIKLI == true)
    .FirstOrDefault();

// Singleton helper:
var langId = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getLanguageId(mekanId);

// Ürün ismi dile göre çekilir:
var urunInfo = db.UrunInfo.Where(ui =>
    ui.URUN_ID == item.PRODUCT_ID &&
    ui.LANGUAGE_ID == langId)
    .FirstOrDefault();

// SQL sorgularda JOIN ile:
var query = @"SELECT UI.URUN_ISMI, UI.FIYAT, ...
              FROM Urunler U
              LEFT JOIN UrunInfo UI ON U.ID=UI.URUN_ID
              LEFT JOIN Languages L ON UI.LANGUAGE_ID=L.ID
              WHERE L.ONCELIKLI = 1";  // Öncelikli dil filtresi
```

**Public menüde dil seçimi:**
```csharp
// Cookie'den dil okuma:
var dilCookie = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getInteger(Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess().GetCookie("language"));
var dilMekan = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getInteger(Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getAdminLoginProcess().GetCookie("languageMekan"));
```

**Çıkarılan kurallar:**
- Her içerik tablosu `LANGUAGE_ID` sütunu taşır (`UrunInfo`, `KategoriInfo`, `MekanInfo`).
- `Languages.ONCELIKLI = 1` bir mekanın varsayılan dilini işaretler.
- Kullanıcının dil tercihi cookie'de saklanır.
- Dil anahtarı `LANGUAGE_ID` integer — string locale kod kullanılmaz.

---

## 9. Ürün Gruplama Pattern'i (MenuUrunlerTemp)

### Referans projede:

```csharp
// Birden fazla ürün seçilip gruplanır:
var randomStr = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getRandomString(150);  // Grup kimliği (random string)
var color = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
    .getRandomColor();      // "#RRGGBB" formatı

items.ForEach(ii =>
{
    var intv = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(ii);
    var data = db.MenuUrunlerTemp.Where(mt => mt.ID == intv).FirstOrDefault();
    if (data != null)
    {
        data.GROUP_NAME = randomStr;          // Aynı grup = aynı string
        data.GROUP_COLOR = color;             // Görsel renk
        data.GROUP_TYPE = grupTypeInt;        // STANDART veya FIYAT_BAZLI
        data.PICTURE_SHOWING_TYPE = pictureShowingTypeInt; // Resim gösterim tipi
        db.Entry(data).State = System.Data.Entity.EntityState.Modified;
    }
});

// Grup açıklamaları her dil için ayrı:
foreach (var item in grupAciklamalar)
{
    var dbGrupAck = new DBservices.MenuUrunGrupAck();
    dbGrupAck.CONTENT    = item.content;
    dbGrupAck.GROUP_NAME = randomStr;     // Grup ID ile ilişkilendirme
    dbGrupAck.LANGUAGE_ID = lngId;
    dbGrupAck.MENU_ID = geciciId;
    db.MenuUrunGrupAck.Add(dbGrupAck);
}
db.SaveChanges();
```

**Çıkarılan kurallar:**
- Grup kimliği integer ID değil, random 150-char string.
- Grup rengi otomatik rastgele renk atanır.
- Grup açıklamaları dile göre ayrı tabloda (`MenuUrunGrupAck`).
- Grup kaldırmak: `GROUP_NAME = null, GROUP_COLOR = null`.

---

## 10. JSON Response Pattern'i

### Referans projede (GET action):

```csharp
// View dönen action:
public ActionResult Index()
{
    var vm = new ListMenuViewModel();
    // ... veri doldurma
    return View(vm);
}

// AJAX JSON dönen action (POST):
[HttpPost]
public ActionResult addMenu(AddMenuViewModel model)
{
    AdminJsonModel result = new AdminJsonModel();
    result.IsSuccess = false;
    result.MessageBody = "";
    result.MessageTitle = "";
    result.redirect = false;

    try
    {
        // ... işlem
        result.IsSuccess = true;
        result.MessageBody = "Menü Başarıyla Güncellendi";
        result.MessageTitle = "İşlem Başarılı";
        result.redirect = false;
        result.redirectUrl = "/menu";
    }
    catch (Exception ee)
    {
        return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT)
            .getExceptionError(ee);
    }
    return Json(result, JsonRequestBehavior.AllowGet);
}
```

**AdminJsonModel alanları:**
```csharp
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
```

**Frontend pattern (AJAX):**
```javascript
$.ajax({
    url: '/menu/addmenu',
    type: 'POST',
    data: formData,
    success: function(response) {
        if (response.IsSuccess) {
            showSuccess(response.MessageTitle, response.MessageBody);
            if (response.redirect) {
                window.location.href = response.redirectUrl;
            }
        } else {
            showError(response.MessageTitle, response.MessageBody);
            if (response.redirect) {
                window.location.href = response.redirectUrl;
            }
        }
    }
});
```

**Çıkarılan kurallar:**
- Tüm POST/AJAX sonuçları `AdminJsonModel` ile döner — standart format.
- `IsSuccess`, `MessageTitle`, `MessageBody` her zaman set edilir.
- `redirect + redirectUrl` çifti: JS tarafında yönlendirme için.
- View dönen action'lar: `return View(viewModel)`.
- DataTables action'lar: `return Json(datatableData)`.

---

## 11. ViewModel Hiyerarşisi Pattern'i

### Referans projede:

```csharp
// TÜM admin page ViewModel'ları AdminPageGlobalModel'den türer:
public class AdminPageGlobalModel
{
    public List<PageHeaderItem> pagetitles { get; set; }   // Breadcrumb
    public String urlQueryString { get; set; }
    public String menuname { get; set; }                   // Sol menüde aktif item
    public GlobalEnums.AdminFormTypes formType { get; set; }
    public GlobalEnums.LoginAdminType loginedAdminType { get; set; }
    public Dictionary<string, YetkiKontrolData> yetkiler { get; set; }
    public Dictionary<int, Dictionary<string, YetkiKontrolData>> yetkilerGrup { get; set; }
}

// Sayfa ViewModel'ı base'i extend eder:
public class ListMenuViewModel : AdminPageGlobalModel
{
    public List<SelectListItem> mekanlar { get; set; }
    public String mekanId { get; set; }
}

public class AdminHomeViewModel : AdminPageGlobalModel
{
    public String seciliMenu { get; set; }
    public List<SelectListItem> menuler { get; set; }
}
```

**Controller'da doldurma:**
```csharp
var vm = new ListMenuViewModel();
vm.menuname   = "Menüler";
vm.urlQueryString = "/menu";
vm.pagetitles = new List<AdminPageGlobalModel.PageHeaderItem>();
vm.pagetitles.Add(new AdminPageGlobalModel.PageHeaderItem
{
    active    = true,
    ikon      = "icon-home",
    link      = "/adminhome",
    pagetitle = "Anasayfa"
});
vm.pagetitles.Add(new AdminPageGlobalModel.PageHeaderItem
{
    active    = true,
    ikon      = "icon-file-empty",
    link      = "/menu",
    pagetitle = "Menüler"
});
vm.loginedAdminType = loginState.loginedadmin.loginAdminType;
vm.yetkiler         = loginState.yetkiler;
vm.yetkilerGrup     = loginState.yetkilerGrup;
```

**Çıkarılan kurallar:**
- Base ViewModel kalıtımı ile tüm sayfalarda tutarlı breadcrumb, yetki, adminType bulunur.
- `menuname` → sol menüdeki aktif link.
- `pagetitles` → sayfanın üst breadcrumb listesi.
- `yetkiler` ve `yetkilerGrup` her ViewModel'a taşınır — View'da `Model.yetkiler["KEY"].isYetkili` kontrolü yapılır.

---

## 12. Sipariş Durum Makinesi Pattern'i

### Referans projede:

```csharp
// Sipariş state geçişleri:
// ONAY_BEKLEYEN (1) -> ONAYLANAN (2) | IPTAL_EDILEN (3)

public ActionResult ChangeOrderState([DefaultValue(0)] int orderId)
{
    using (var db = new DBservices.pixdinnEntities())
    {
        var dbOrder = db.Siparisler.Where(x => x.ID == orderId).FirstOrDefault();

        if (dbOrder != null &&
            dbOrder.STATE == (int)GlobalEnums.SiparisState.ONAY_BEKLEYEN)
        {
            dbOrder.STATE = (int)GlobalEnums.SiparisState.ONAYLANAN;
            dbOrder.UPDATED_DATE = DateTime.Now;
            db.Entry(dbOrder).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            // SiparisItem'ları da güncelle:
            var items = db.SiparisItem.Where(x => x.SIPARIS_ID == dbOrder.ID).ToList();
            foreach (var item in items)
            {
                if (item.STATE == (int)GlobalEnums.SiparisState.ONAY_BEKLEYEN)
                {
                    item.STATE = (int)GlobalEnums.SiparisState.ONAYLANAN;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            result.IsSuccess = true;
        }
        else
        {
            result.IsSuccess = false;
            result.MessageBody = "Siparişin Durumu Değişmiştir";
        }
    }
}
```

**Ödeme tipleri:**
```csharp
// ODEME_TIPI alanı:
if (dbUserOrder.ODEME_TIPI == 1) odemeTipi = "Nakit";
else if (dbUserOrder.ODEME_TIPI == 2) odemeTipi = "Kredi Kartı";
else if (dbUserOrder.ODEME_TIPI == 3)
{
    odemeTipi = "Yemek Kartı";
    if (!string.IsNullOrEmpty(dbUserOrder.ODEME_TIPI_TEXT))
        odemeTipi += " (" + dbUserOrder.ODEME_TIPI_TEXT + ")";
}
```

**Çıkarılan kurallar:**
- Sipariş state geçişi atomic değil — önce Siparisler, sonra SiparisItem güncellenir. Transaction yok.
- Race condition riski: iki admin aynı anda onaylayabilir (state check yapılıyor ama lock yok).
- `UPDATED_DATE = DateTime.Now` her state değişiminde set edilir.

---

## 13. View Render Pattern'i (Partial + RenderAction)

### Referans projede:

```html
<!-- Admin sayfa şablonu başı: -->
@model PixDinn.Models.AdminModels.AdminViewModels.ListMenuViewModel
@{ Layout = "~/Views/Shared/_AdminLayout.cshtml"; }

<!-- Breadcrumb partial: -->
@{Html.RenderAction("PageHeaderBlock", "AdminBlock", new AdminPageHeaderModel() {
    pagetitles = Model.pagetitles
});}

<!-- Alert dialog partial: -->
@{Html.RenderAction("AlertDialog", "AdminHelper");}

<!-- İç içe partial: -->
@{Html.RenderAction("UrunSelectDialog", "Menu");}

<!-- Conditional render (yetki bazlı): -->
@if (Model.yetkiler["MENU_ISLEMLERI"].isYetkili)
{
    <button onclick="addMenu()">Menü Ekle</button>
}
```

**Tam sayfa (Layout null):**
```html
<!-- Public ViewMenu ve Tanıtım sayfaları: -->
@{
    Layout = null;
    Response.Cache.SetCacheability(HttpCacheability.NoCache);
    Response.Cache.AppendCacheExtension("no-store, must-revalidate");
    Response.AppendHeader("Pragma", "no-cache");
    Response.AppendHeader("Expires", "0");
}
<!DOCTYPE html>
<html>...</html>
```

**Çıkarılan kurallar:**
- Admin sayfaları `~/Views/Shared/_AdminLayout.cshtml` kullanır.
- Public sayfalar `Layout = null` ile bağımsız HTML üretir.
- `Html.RenderAction()` ile child action partial render — her partial kendi controller action'ı.
- `Response.Cache.SetCacheability(HttpCacheability.NoCache)` — dinamik içeriklerde tarayıcı cache'i devre dışı.

---

## 14. Anti-Pattern Kataloğu (Modern .NET'te Kaçınılacaklar)

### 1. Singleton Global State
```csharp
// YAPMA: Test edilemez, thread-safety riski
Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState()

// YAP: DI ile inject et
public class MenuController : Controller
{
    private readonly IAdminLoginService _loginService;
    public MenuController(IAdminLoginService loginService)
        => _loginService = loginService;
}
```

### 2. MD5 Şifreleme
```csharp
// YAPMA:
String passwordMd5 = Singleton.getSingleton(...).decryptMd5(password);
db.AdminUsers.Where(ad => ad.USERNAME == userName && ad.PASSWORD == passwordMd5)

// YAP:
var result = await _signInManager.PasswordSignInAsync(userName, password, false, true);
// veya: BCrypt.Net.BCrypt.Verify(password, storedHash)
```

### 3. SQL String Concatenation
```csharp
// YAPMA (injection riski):
" AND MENU_ID=" + menuId + " group by MOVING_ID"

// YAP:
" AND MENU_ID=@menuId group by MOVING_ID"
// ile SqlParameter("@menuId", menuId) kullan
```

### 4. Her Yerde using(db)
```csharp
// YAPMA: Her action bağlantı açıp kapatıyor
using (var db = new pixdinnEntities()) { ... }

// YAP: Repository pattern + DI + DbContext per-request
public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _db;
    public MenuRepository(AppDbContext db) => _db = db;
    public async Task<Menu> GetByIdAsync(int id)
        => await _db.MenulerTemp.FindAsync(id);
}
```

### 5. State Magic Number
```csharp
// YAPMA:
u.STATE != 3

// YAP: Enum veya const
u.STATE != (int)AdminDataState.DELETED
// veya: u.State != DataState.Deleted (strongly typed)
```

### 6. Controller'da İş Mantığı
```csharp
// YAPMA: Controller 1000+ satır, DB + business logic karışık
public ActionResult addUrunTemp(...)
{
    // 50 satır yetki kontrolü
    // 100 satır DB işlemi
    // 30 satır Redis invalidation
}

// YAP: Service layer
public class MenuService : IMenuService
{
    public async Task<Result> AddUrunToMenuAsync(AddUrunRequest request)
    {
        await _yetkiService.CheckAsync(request.MekanId, "MENU_ISLEMLERI");
        await _menuRepository.AddUrunAsync(request);
        await _cacheService.InvalidateMenuAsync(request.MenuId);
        return Result.Success();
    }
}
```

---

## 15. Özet: Bu Projeden Öğrenilen Doğru Yaklaşımlar

| Pattern | Referans Projedeki Uygulama | Değerlendirme |
|---|---|---|
| QR Kod üretimi | QRCoder + Base64 PNG JSON response | Doğru yaklaşım |
| Redis Cache-Aside | SetQrMenuDbProcess + IsExistQrMenuDbProcess | Mantıklı invalidation stratejisi |
| Yetki sistemi | Enum tabanlı admin türleri + key-value yetki dict | İyi ama string key kırılgan |
| Anti-replay | pageKey rotation + 3 sn minimum bekleme | Düşük maliyetli bot engeli |
| Token logout | Token rotation (fiziksel silme değil) | Stateless çıkış, makul |
| Multi-tenancy | mekanSqlWhere string ile SQL filtresi | Çalışıyor ama injection riski |
| Soft delete | STATE=3 (DELETED) | Standart yaklaşım |
| DataTables server-side | getDataTableData wrapper | UI entegrasyonu temiz |
| Çoklu dil | LANGUAGE_ID + ONCELIKLI flag | Basit ama etkili |
| Ürün gruplama | Random string GROUP_NAME | Özgün çözüm, işe yarıyor |
