// controller-pattern.cs
// qr_menu_mvc — ASP.NET MVC 5 Controller Yapısal Pattern
// .NET Framework 4.7.2 + Entity Framework 6 Database-First

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DBservices;

namespace PixDinn.Controllers
{
    // ---------------------------------------------------------------
    // Admin Controller — Session korumalı
    // ---------------------------------------------------------------
    [AuthorizeAdmin]    // Özel attribute — session kontrolü yapar
    public class ProductController : Controller
    {
        private readonly UrunDBService _urunService;
        private readonly KategoriDBService _kategoriService;

        public ProductController()
        {
            _urunService = new UrunDBService();
            _kategoriService = new KategoriDBService();
        }

        // GET: /Product/Index
        public ActionResult Index(int mekanId)
        {
            var urunler = _urunService.GetByMekan(mekanId);
            ViewBag.MekanId = mekanId;
            return View(urunler);
        }

        // GET: /Product/Create
        public ActionResult Create(int mekanId)
        {
            ViewBag.Kategoriler = _kategoriService.GetByMekan(mekanId);
            ViewBag.MekanId = mekanId;
            return View(new UrunViewModel());
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UrunViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kategoriler = _kategoriService.GetByMekan(model.MekanId);
                return View(model);
            }

            try
            {
                // Resim yükleme
                if (Request.Files["ProductImage"] != null && Request.Files["ProductImage"].ContentLength > 0)
                {
                    var file = Request.Files["ProductImage"];
                    var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    var savePath = Server.MapPath("~/Content/images/products/" + fileName);
                    file.SaveAs(savePath);
                    model.ResimPath = "/Content/images/products/" + fileName;
                }

                _urunService.Create(model);
                TempData["Success"] = "Ürün başarıyla eklendi.";
                return RedirectToAction("Index", new { mekanId = model.MekanId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında hata: " + ex.Message);
                ViewBag.Kategoriler = _kategoriService.GetByMekan(model.MekanId);
                return View(model);
            }
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            var urun = _urunService.GetById(id);
            if (urun == null) return HttpNotFound();
            ViewBag.Kategoriler = _kategoriService.GetByMekan(urun.KategoriId);
            return View(urun);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UrunViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _urunService.Update(model);
            TempData["Success"] = "Ürün güncellendi.";
            return RedirectToAction("Index", new { mekanId = model.MekanId });
        }

        // POST: /Product/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _urunService.SoftDelete(id);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Silme işlemi başarısız." });
            }
        }

        // ---------------------------------------------------------------
        // AJAX endpoint — dropdown doldurma
        // ---------------------------------------------------------------
        [HttpGet]
        public JsonResult GetKategoriler(int mekanId)
        {
            var kategoriler = _kategoriService.GetByMekan(mekanId);
            return Json(kategoriler, JsonRequestBehavior.AllowGet);
        }
    }

    // ---------------------------------------------------------------
    // Public Controller — Auth gerektirmeyen (QR menü sayfası)
    // ---------------------------------------------------------------
    public class MenuController : Controller
    {
        private readonly MenuDBService _menuService;
        private readonly UrunDBService _urunService;

        public MenuController()
        {
            _menuService = new MenuDBService();
            _urunService = new UrunDBService();
        }

        // GET: /m/{mekanKod}  — QR kod ile erişilen public menü
        [Route("m/{mekanKod}")]
        public ActionResult Public(string mekanKod)
        {
            var menu = _menuService.GetPublicMenu(mekanKod);
            if (menu == null) return HttpNotFound("Menü bulunamadı.");
            return View("Public", menu);
        }

        // GET: /m/{mekanKod}/{kategoriId}  — Kategori filtreli menü
        [Route("m/{mekanKod}/{kategoriId:int}")]
        public ActionResult Category(string mekanKod, int kategoriId)
        {
            var menu = _menuService.GetMenuByCategory(mekanKod, kategoriId);
            return View("Public", menu);
        }
    }

    // ---------------------------------------------------------------
    // Login Controller
    // ---------------------------------------------------------------
    public class AdminLoginController : Controller
    {
        private readonly AdminUserDBService _userService;

        public AdminLoginController()
        {
            _userService = new AdminUserDBService();
        }

        // GET: /AdminLogin
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Session["AdminUser"] != null)
                return RedirectToAction("Index", "AdminMekan");
            return View();
        }

        // POST: /AdminLogin/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = _userService.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View("Index", model);
            }

            Session["AdminUser"] = user;
            Session["MekanId"] = user.MekanId;
            Session["Username"] = user.Username;

            return RedirectToAction("Index", "AdminMekan");
        }

        // GET: /AdminLogin/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}

// ---------------------------------------------------------------
// AuthorizeAdmin — Özel Action Filter
// ---------------------------------------------------------------
/*
public class AuthorizeAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session["AdminUser"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "AdminLogin",
                    action = "Index",
                    returnUrl = filterContext.HttpContext.Request.Url?.AbsolutePath
                }));
        }
        base.OnActionExecuting(filterContext);
    }
}
*/

// ---------------------------------------------------------------
// Notlar:
//
// new UrunDBService()
//   → DI yok, constructor'da doğrudan nesne oluşturma.
//   → Modernizasyonda Autofac/built-in DI ile değiştirilmeli.
//
// [ValidateAntiForgeryToken]
//   → Her POST action'ına ekle. CSRF koruması.
//
// TempData["Success"]
//   → Redirect sonrası kullanıcıya başarı mesajı göster.
//   → View'da: @if (TempData["Success"] != null) { ... }
//
// JsonRequestBehavior.AllowGet
//   → JSON GET endpoint'leri için zorunlu (MVC 5 güvenlik kısıtlaması).
//   → MVC 5'e özgü, ASP.NET Core'da bu sorun yok.
// ---------------------------------------------------------------
