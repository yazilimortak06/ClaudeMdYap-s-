// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\MenuController.cs
using Newtonsoft.Json;
using PixDinn.Models.AdminModels.AdminGlobalModels;
using PixDinn.Models.AdminModels.AdminViewModels;
using PixDinn.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using PixDinn.Processes.AdminGlobalProcesses;
using static PixDinn.Processes.GlobalEnums;
using PixDinn.Processes.RedisManagment;
using PixDinn.Models.AdminModels;

namespace PixDinn.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MENU, openingType = GlobalEnums.PageOpeningType.DIRECT)]

        public ActionResult Index()
        {
            ListMenuViewModel listMenuViewModel = new ListMenuViewModel();
            listMenuViewModel.formType = GlobalEnums.AdminFormTypes.ADD_URUN;
            listMenuViewModel.menuname = "Menüler";
            listMenuViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            listMenuViewModel.urlQueryString = "/menu";
            listMenuViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
            listMenuViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
            {
                active = true, ikon = "icon-home", link = "/adminhome", pagetitle = "Anasayfa"
            });
            listMenuViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
            {
                active = true, ikon = "icon-file-empty", link = "/menu", pagetitle = "Menüler"
            });
            return View(listMenuViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_MENU, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        public ActionResult AddMenu([DefaultValue(0)] int mekanId, [DefaultValue(0)] int geciciMenuId)
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            addMenuViewModel.yetkiler = new Dictionary<string, Models.AdminModels.AdminLoginSessionResponse.YetkiKontrolData>();
            addMenuViewModel.yetkilerGrup = new Dictionary<int, Dictionary<string, Models.AdminModels.AdminLoginSessionResponse.YetkiKontrolData>>();
            var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
            addMenuViewModel.yetkiler = loginState.yetkiler;
            addMenuViewModel.yetkilerGrup = loginState.yetkilerGrup;
            addMenuViewModel.loginedAdminType = loginState.loginedadmin.loginAdminType;
            addMenuViewModel.formType = GlobalEnums.AdminFormTypes.ADD_URUN;
            addMenuViewModel.menuname = "Menü Ekle";
            addMenuViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            addMenuViewModel.urlQueryString = "/menu/addMenu";
            addMenuViewModel.menuState = new List<System.Web.Mvc.SelectListItem>();

            #region Returning verileri set ediliyor
            addMenuViewModel.kategoriItems = new List<AddMenuViewModel.KategoriItem>();
            addMenuViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
            addMenuViewModel.mekanId = "" + mekanId;
            using (var db = new DBservices.pixdinnEntities())
            {
                var menu = db.MenulerTemp.Where(mt => mt.ID == geciciMenuId).FirstOrDefault();
                if (menu != null)
                {
                    addMenuViewModel.ack = menu.ACK;
                    addMenuViewModel.menuUrl = menu.MENU_URL;
                    addMenuViewModel.menuMainUrl = menu.QR_CODE;
                    addMenuViewModel.menuSiparisUrl = menu.QR_CODE_SIPARIS;
                    addMenuViewModel.menuSayfaYaziDurumu = menu.MENU_SAYFA_YAZI_DURUMU.GetValueOrDefault();
                    addMenuViewModel.showingEntrancePage = menu.SHOWING_ENTRANCE_PAGE.GetValueOrDefault();
                }
                var kategoriler = db.Kategoriler.Where(kk => kk.MEKAN_ID == mekanId && kk.KATEGORI_LEVEL == 1 && kk.STATE != (int)GlobalEnums.AdminDataState.DELETED).ToList();
                kategoriler.ForEach(ii =>
                {
                    var lang = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguageId(mekanId);
                    var kategoriInfo = db.KategoriInfo.Where(ki => ki.KATEGORI_ID == ii.ID && ki.LANGUAGE_ID == lang).FirstOrDefault();
                    var mediaData = db.MediaData.Where(md => md.ID == ii.KATEGORI_RESMI).FirstOrDefault();
                    if (kategoriInfo != null)
                    {
                        addMenuViewModel.kategoriItems.Add(new AddMenuViewModel.KategoriItem
                        {
                            id = ii.ID,
                            name = kategoriInfo.KATEGORI_ADI,
                            urlPic = mediaData != null ? mediaData.MEDIA_DATA_URL : ""
                        });
                    }
                });
            }
            #endregion

            addMenuViewModel.geciciId = geciciMenuId;
            return View(addMenuViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_MENU_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getMenuPartial([DefaultValue(0)] int scopeId, [DefaultValue(0)] int kategoriId,
                                           [DefaultValue(0)] int menuId, [DefaultValue(0)] int gecicimenuId, [DefaultValue(0)] int mekanId)
        {
            MenuPartialViewModel menuPartialViewModel = new MenuPartialViewModel();
            menuPartialViewModel.yetkiler = new Dictionary<string, Models.AdminModels.AdminLoginSessionResponse.YetkiKontrolData>();
            menuPartialViewModel.yetkilerGrup = new Dictionary<int, Dictionary<string, Models.AdminModels.AdminLoginSessionResponse.YetkiKontrolData>>();
            var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
            menuPartialViewModel.yetkiler = loginState.yetkiler;
            menuPartialViewModel.yetkilerGrup = loginState.yetkilerGrup;
            menuPartialViewModel.loginedAdminType = loginState.loginedadmin.loginAdminType;

            using (var db = new DBservices.pixdinnEntities())
            {
                var kategori = db.Kategoriler.Where(kk => kk.ID == kategoriId).FirstOrDefault();
                var lang = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguageId((int)kategori.MEKAN_ID);
                var kategoriInfoMain = db.KategoriInfo.Where(ki => ki.KATEGORI_ID == (int)kategori.ID && ki.LANGUAGE_ID == lang).FirstOrDefault();
                menuPartialViewModel.menuId = menuId;
                menuPartialViewModel.geciciId = gecicimenuId;
                if (kategori != null)
                {
                    menuPartialViewModel.mekanId = kategori.MEKAN_ID.GetValueOrDefault();
                    if (kategoriInfoMain != null)
                    {
                        menuPartialViewModel.kategoriAdi = kategoriInfoMain.KATEGORI_ADI;
                    }
                    var kategoriAltLevel = kategori.KATEGORI_LEVEL + 1;
                    var kategoriAltlar = db.Kategoriler.Where(kk => kk.STATE != (int)GlobalEnums.AdminDataState.DELETED && kk.KATEGORI_LEVEL == kategoriAltLevel && kk.UST_KATEGORI_ID == kategori.ID).ToList();
                    if (kategoriAltlar.Count == 0)
                    {
                        menuPartialViewModel.type = "ENALT";
                    }
                    else
                    {
                        menuPartialViewModel.type = "UST";
                        menuPartialViewModel.kategoriItems = new List<AddMenuViewModel.KategoriItem>();
                        kategoriAltlar.ForEach(ii =>
                        {
                            var kategoriInfo = db.KategoriInfo.Where(ki => ki.KATEGORI_ID == (int)ii.ID && ki.LANGUAGE_ID == lang).FirstOrDefault();
                            var mediaData = db.MediaData.Where(md => md.ID == ii.KATEGORI_RESMI).FirstOrDefault();
                            if (kategoriInfo != null)
                            {
                                menuPartialViewModel.kategoriItems.Add(new AddMenuViewModel.KategoriItem
                                {
                                    id = ii.ID,
                                    name = kategoriInfo.KATEGORI_ADI,
                                    urlPic = mediaData != null ? mediaData.MEDIA_DATA_URL : ""
                                });
                            }
                        });
                    }
                }
            }
            menuPartialViewModel.kategoriId = kategoriId;
            menuPartialViewModel.scopeId = scopeId + 1;
            return View(menuPartialViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_URUN_TEMP, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult addUrunTemp([DefaultValue(0)] int menuId, [DefaultValue(0)] int geciciId, [DefaultValue(0)] int kategoriId,
            [DefaultValue("")] String selectedItemsJson, [DefaultValue("")] String name, [DefaultValue("")] String sayfaYazisi,
            [DefaultValue(false)] bool sayfaYazisiDurumu, [DefaultValue(false)] bool showingEntrancePage)
        {
            AddUrunTempResponseModel addUrunTempResponseModel = new AddUrunTempResponseModel();
            try
            {
                var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
                List<AddUrunTempRequestModel.SelectedJsonItem> urunler =
                    Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMainSerialize().Deserialize<List<AddUrunTempRequestModel.SelectedJsonItem>>(selectedItemsJson);
                var nowDate = DateTime.Now;
                using (var db = new DBservices.pixdinnEntities())
                {
                    var selectedKategori = db.Kategoriler.Where(kk => kk.ID == kategoriId).FirstOrDefault();
                    if (selectedKategori != null)
                    {
                        var isYetkili = false;
                        if (loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER)
                            isYetkili = loginState.YetkiKontrolGrup("MENU_ISLEMLERI", selectedKategori.MEKAN_ID.GetValueOrDefault());
                        else
                            isYetkili = loginState.YetkiKontrol("MENU_ISLEMLERI");

                        if (isYetkili)
                        {
                            var geciciKayit = db.MenulerTemp.Where(ut => ut.ID == geciciId).FirstOrDefault();
                            if (geciciKayit == null)
                            {
                                var inserted = new DBservices.MenulerTemp();
                                inserted.STATE = (int)AdminDataState.ACTIVE;
                                inserted.MEKAN_ID = selectedKategori.MEKAN_ID;
                                inserted.ACK = name;
                                inserted.CREATED_DATE = nowDate;
                                inserted.QR_CODE = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                                inserted.MENU_SAYFA_YAZI_DURUMU = sayfaYazisiDurumu;
                                inserted.SHOWING_ENTRANCE_PAGE = showingEntrancePage;
                                db.MenulerTemp.Add(inserted);
                                db.SaveChanges();
                                urunler.ForEach(item =>
                                {
                                    db.MenuUrunlerTemp.Add(new DBservices.MenuUrunlerTemp()
                                    {
                                        CREATED_DATE = nowDate,
                                        MEKAN_ID = selectedKategori.MEKAN_ID,
                                        URUN_ID = item.id,
                                        KATEGORI_ID = selectedKategori.ID,
                                        MENU_ID = inserted.ID
                                    });
                                });
                                db.SaveChanges();
                                addUrunTempResponseModel.success = true;
                                addUrunTempResponseModel.geciciId = inserted.ID;
                                addUrunTempResponseModel.resultType = "GECICI";
                            }
                            else
                            {
                                urunler.ForEach(item =>
                                {
                                    db.MenuUrunlerTemp.Add(new DBservices.MenuUrunlerTemp()
                                    {
                                        CREATED_DATE = nowDate,
                                        MEKAN_ID = selectedKategori.MEKAN_ID,
                                        URUN_ID = item.id,
                                        KATEGORI_ID = selectedKategori.ID,
                                        MENU_ID = geciciKayit.ID
                                    });
                                });
                                db.SaveChanges();
                                addUrunTempResponseModel.success = true;
                                addUrunTempResponseModel.geciciId = geciciKayit.ID;
                                addUrunTempResponseModel.resultType = "GECICI";
                            }
                        }
                    }
                }
            }
            catch (Exception ee) { }
            return Json(addUrunTempResponseModel, JsonRequestBehavior.AllowGet);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_QR_MENU, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult qrMenu([DefaultValue(0)] int geciciId)
        {
            AddUrunTempResponseModel addUrunTempResponseModel = new AddUrunTempResponseModel();
            try
            {
                String url = "";
                using (var db = new DBservices.pixdinnEntities())
                {
                    var menu = db.MenulerTemp.Where(mt => mt.ID == geciciId).FirstOrDefault();
                    if (menu != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (menu.MENU_URL == "" || menu.MENU_URL == null)
                                url = "https://pixdinn.com/viewmenu?data=" + menu.QR_CODE;
                            else
                                url = "https://" + menu.MENU_URL + "/viewmenu?data=" + menu.QR_CODE;

                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            using (Bitmap bitMap = qrCode.GetGraphic(20))
                            {
                                bitMap.Save(ms, ImageFormat.Png);
                                addUrunTempResponseModel.data = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                addUrunTempResponseModel.success = true;
                                addUrunTempResponseModel.url = url;
                            }
                        }
                    }
                }
            }
            catch (Exception ee) { }
            return Json(addUrunTempResponseModel, JsonRequestBehavior.AllowGet);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_MENU_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult addMenu(AddMenuViewModel addMenu)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";
            result.redirect = false;
            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var menu = db.MenulerTemp.Where(x => x.ID == addMenu.geciciId).FirstOrDefault();
                    var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
                    var isYetkili = false;
                    if (loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER)
                        isYetkili = loginState.YetkiKontrolGrup("MENU_ISLEMLERI", Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getInteger(addMenu.mekanId));
                    else
                        isYetkili = loginState.YetkiKontrol("MENU_ISLEMLERI");

                    if (isYetkili)
                    {
                        if (menu == null)
                        {
                            result.IsSuccess = false;
                            result.MessageBody = "Menüye ürün eklemediniz";
                            result.MessageTitle = "İşlem Başarısız";
                        }
                        else
                        {
                            menu.MENU_URL = addMenu.menuUrl;
                            menu.ACK = addMenu.ack;
                            menu.MENU_SAYFA_YAZI_DURUMU = addMenu.menuSayfaYaziDurumu;
                            menu.SHOWING_ENTRANCE_PAGE = addMenu.showingEntrancePage;
                            if (menu.QR_CODE == null)
                                menu.QR_CODE = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                            db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            try
                            {
                                var dateTimeNow = DateTime.Now;
                                QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
                                RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getRedisCacheManager();
                                qrMenuDbProcess.MenuId = menu.ID;
                                qrMenuDbProcess.MenuUpdateDate = dateTimeNow;
                                manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);
                            }
                            catch (Exception ee) { }
                            result.IsSuccess = true;
                            result.MessageBody = "Menü Başarıyla Güncellendi";
                            result.MessageTitle = "İşlem Başarılı";
                            result.redirect = false;
                            result.redirectUrl = "/menu";
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.CHANGE_STATE_MENU, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult changeStateMenu(int menuId, int mekanId)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
                    var isYetkili = loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER
                        ? loginState.YetkiKontrolGrup("MENU_ISLEMLERI", mekanId)
                        : loginState.YetkiKontrol("MENU_ISLEMLERI");
                    if (isYetkili)
                    {
                        var menu = db.MenulerTemp.Where(x => x.ID == menuId && x.MEKAN_ID == mekanId && x.STATE != (int)AdminDataState.DELETED).FirstOrDefault();
                        if (menu != null)
                        {
                            menu.STATE = menu.STATE.GetValueOrDefault() == (int)AdminDataState.PASSIVE ? (int)AdminDataState.ACTIVE : (int)AdminDataState.PASSIVE;
                            db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            result.IsSuccess = true;
                            result.MessageBody = "Durumu Başarıyla Güncellendi";
                            result.MessageTitle = "İşlem Başarılı";
                            result.redirect = true;
                            result.redirectUrl = "/menu";
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
