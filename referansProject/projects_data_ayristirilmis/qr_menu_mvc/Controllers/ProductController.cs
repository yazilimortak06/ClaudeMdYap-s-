// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\ProductController.cs
using Newtonsoft.Json;
using PixDinn.Models.AdminModels.AdminGlobalModels;
using PixDinn.Models.AdminModels.AdminViewModels;
using PixDinn.Processes;
using PixDinn.Processes.AdminGlobalProcesses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PixDinn.Processes.GlobalEnums;
using PixDinn.Processes.RedisManagment;
using PixDinn.Models.AdminModels;

namespace PixDinn.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_PRODUCT, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        public ActionResult Index()
        {
            try
            {
                ProductListViewModel productListViewModel = new ProductListViewModel();
                productListViewModel.menuname = "adadadad";
                productListViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
                productListViewModel.urlQueryString = "/Product";
                productListViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-home", link = "/adminhome", pagetitle = "Anasayfa"
                });
                productListViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-file-empty", link = "/Product", pagetitle = "Ürünler"
                });
                productListViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
                return View(productListViewModel);
            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageException");
            }
        }

        #region Ürünler  çekiliyor
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_PRODUCT_LIST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getUrunlerData(string pData)
        {
            var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.mekanObj;
            var sqlExtra = mekanObj != null ? " AND U.MEKAN_ID IN " + mekanObj.mekanSqlWhere : "";
            var query = @"
                          select KI.KATEGORI_ADI,KI.ID as kateg,L3.ONCELIKLI, MU.ID as MenuUrunId,U.ID,UI.URUN_ISMI,UI.FIYAT,UI.FIYAT_BIRIM,UI.URUN_ACIKLAMA,UI.FIYAT_METIN,UI.ADET_TIPI,U.URUN_KODU,U.PISME_SURESI,U.KALORI,MI.MEKAN_ADI,U.CREATED_DATE,U.UPDATED_DATE,A.USERNAME,A.ADMIN_TYPE,U.STATE from Urunler U
                            left join AdminUsers A on U.CREATED_ADMIN_ID=A.ID
                            left join MekanInfo MI on U.MEKAN_ID=MI.MEKAN_ID
                            left join UrunInfo UI on U.ID=UI.URUN_ID
							left join Languages L on UI.LANGUAGE_ID = L.ID
							left join Languages L2 on MI.LANGUAGE_ID = L2.ID
							left join MenuUrunlerTemp MU on MU.ID = (SELECT TOP 1 MU2.ID FROM MenuUrunlerTemp MU2 WHERE MU2.URUN_ID = U.ID ORDER BY MU2.ID DESC)
							left join Kategoriler K on K.ID = MU.KATEGORI_ID
							left join KategoriInfo KI on K.ID=KI.KATEGORI_ID
							left join Languages L3 on KI.LANGUAGE_ID = L3.ID
							WHERE L.ONCELIKLI = 1 and L2.ONCELIKLI = 1 and (L3.ONCELIKLI = 1 or L3.ID is null) AND U.STATE !=3
                    " + sqlExtra;
            var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getdataTableProcess().getDataTableData("Urunler", pData, query, " U.ID asc ");
            return Json(datatableData);
        }
        #endregion

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_PRODUCT, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        [HttpGet]
        public ActionResult addProduct([DefaultValue(0)] int ID, [DefaultValue((int)GlobalEnums.OpeningType.NORMAL)] int OPENING_TYPE, [DefaultValue(0)] int mekanId)
        {
            AdminProductViewModel adminProductView = new AdminProductViewModel();
            adminProductView.yetkiler = new Dictionary<string, Models.AdminModels.AdminLoginSessionResponse.YetkiKontrolData>();
            adminProductView.metinselFiyatDurumu = false;
            var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();

            var isYetkili = loginState.loginedadmin.loginAdminType == GlobalEnums.LoginAdminType.MEKAN_GRUP_USER
                ? loginState.YetkiKontrolGrup("FIYAT_DEGISIKLIGI_YAPMA", mekanId)
                : loginState.YetkiKontrol("FIYAT_DEGISIKLIGI_YAPMA");
            adminProductView.isYetkiliForPrice = isYetkili;
            adminProductView.yetkiler = loginState.yetkiler;
            adminProductView.formType = GlobalEnums.AdminFormTypes.ADD_URUN;
            adminProductView.menuname = "Ürün Ekle";
            adminProductView.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            adminProductView.urlQueryString = "/Product/addProduct";
            adminProductView.urunState = new List<SelectListItem>();
            adminProductView.languages = new List<SelectListItem>();
            adminProductView.mekanlar = new List<SelectListItem>();
            adminProductView.urunIcons = new List<AdminProductViewModel.UrunIcon>();
            adminProductView.urunInfo = new List<AdminProductViewModel.UrunInfo>();

            using (var db = new DBservices.pixdinnEntities())
            {
                adminProductView.urunState.Add(new SelectListItem { Selected = true, Text = "Durum Seçiniz", Value = "-1" });
                adminProductView.urunState.Add(new SelectListItem { Text = "Aktif", Value = ((int)GlobalEnums.AdminDataState.ACTIVE).ToString() });
                adminProductView.urunState.Add(new SelectListItem { Text = "Pasif", Value = ((int)GlobalEnums.AdminDataState.PASSIVE).ToString() });
                adminProductView.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();

                if (mekanId != 0) adminProductView.mekanId = "" + mekanId;

                var dbUrun = db.Urunler.Where(x => x.ID == ID).FirstOrDefault();

                #region Ürün insert
                if (dbUrun == null)
                {
                    var dbMekan = db.Mekanlar.Where(x => x.ID == mekanId).FirstOrDefault();
                    if (dbMekan != null)
                        adminProductView.metinselFiyatDurumu = dbMekan.PRODUCT_PRICE_IS_STRING.GetValueOrDefault();

                    for (int i = 1; i <= 21; i++)
                    {
                        adminProductView.urunIcons.Add(new AdminProductViewModel.UrunIcon { name = i + "", state = false, url = "/UrunlerIcons/" + i + ".png" });
                    }
                    adminProductView.mekanId = mekanId + "";
                }
                #endregion
                #region Ürün update
                else
                {
                    var dbMekan = db.Mekanlar.Where(x => x.ID == dbUrun.MEKAN_ID).FirstOrDefault();
                    if (dbMekan != null)
                        adminProductView.metinselFiyatDurumu = dbMekan.PRODUCT_PRICE_IS_STRING.GetValueOrDefault();

                    adminProductView.id = dbUrun.ID;
                    adminProductView.urunResimId = (int)dbUrun.URUN_RESIM_ID.GetValueOrDefault();
                    adminProductView.urunResimBuyukId = (int)dbUrun.URUN_RESIM_BUYUK_ID.GetValueOrDefault();
                    adminProductView.pismeSuresi = (int)dbUrun.PISME_SURESI;
                    adminProductView.kalori = (float)dbUrun.KALORI;
                    adminProductView.mekanId = mekanId + "";
                    adminProductView.urunKodu = dbUrun.URUN_KODU;
                    adminProductView.pluNo = dbUrun.PLU_NO;
                    adminProductView.mekanId = dbUrun.MEKAN_ID.ToString();
                    adminProductView.state = dbUrun.STATE.ToString();
                    adminProductView.pismeSuresiTipi = dbUrun.sureTipi;

                    var resimData = db.MediaData.Where(mm => mm.ID == dbUrun.URUN_RESIM_ID).FirstOrDefault();
                    if (resimData != null) adminProductView.urunResimUrl = resimData.MEDIA_DATA_URL;

                    var dbUrunInfo = db.UrunInfo.Where(x => x.URUN_ID == dbUrun.ID).ToList();
                    foreach (var item in dbUrunInfo)
                    {
                        var resimInfo = db.MediaData.Where(mm => mm.ID == item.RESIM_ID).FirstOrDefault();
                        adminProductView.urunInfo.Add(new AdminProductViewModel.UrunInfo()
                        {
                            languageId = item.LANGUAGE_ID.ToString(),
                            urunName = item.URUN_ISMI,
                            fiyat = (float)item.FIYAT,
                            fiyatBirim = item.FIYAT_BIRIM,
                            urunAciklama = item.URUN_ACIKLAMA,
                            adetTipi = item.ADET_TIPI,
                            id = item.ID,
                            metinselFiyat = item.FIYAT_METIN,
                            resimId = item.RESIM_ID.GetValueOrDefault(),
                            resimUrl = resimInfo != null ? resimInfo.MEDIA_DATA_URL : null
                        });
                    }
                    adminProductView.infoJsonValue = JsonConvert.SerializeObject(adminProductView.urunInfo);

                    for (int i = 1; i <= 21; i++)
                    {
                        var dbUrunIcon = db.UrunlerIcons.Where(x => x.ICON_NAME == i + "" && x.URUN_ID == dbUrun.ID && x.MEKAN_ID == dbUrun.MEKAN_ID && x.STATE == (int)GlobalEnums.AdminDataState.ACTIVE).FirstOrDefault();
                        adminProductView.urunIcons.Add(new AdminProductViewModel.UrunIcon
                        {
                            name = i + "", state = dbUrunIcon != null, url = "/UrunlerIcons/" + i + ".png"
                        });
                    }
                }
                #endregion

                if (mekanId != 0)
                    adminProductView.languages = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguages(mekanId);
                else
                    adminProductView.languages = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguages(Convert.ToInt32(adminProductView.mekanId));
            }

            return View(adminProductView);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_PRODUCT_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult addProduct(AdminProductViewModel adminProductView)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";
            result.redirect = false;
            adminProductView.urunInfo = new List<AdminProductViewModel.UrunInfo>();

            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var urun = db.Urunler.Where(x => x.ID == adminProductView.id).FirstOrDefault();
                    adminProductView.urunInfo = JsonConvert.DeserializeObject<List<AdminProductViewModel.UrunInfo>>(adminProductView.infoJsonValue);

                    if (urun == null)
                    {
                        urun = new DBservices.Urunler();
                        urun.MEKAN_ID = Convert.ToInt32(adminProductView.mekanId);
                        urun.URUN_RESIM_ID = adminProductView.urunResimId;
                        urun.URUN_RESIM_BUYUK_ID = adminProductView.urunResimBuyukId;
                        urun.PISME_SURESI = adminProductView.pismeSuresi;
                        urun.KALORI = adminProductView.kalori;
                        urun.URUN_KODU = adminProductView.urunKodu;
                        urun.PLU_NO = adminProductView.pluNo;
                        urun.STATE = Convert.ToInt32(adminProductView.state);
                        urun.CREATED_DATE = DateTime.Now;
                        urun.sureTipi = adminProductView.pismeSuresiTipi;
                        db.Urunler.Add(urun);
                        db.SaveChanges();

                        foreach (var item in adminProductView.urunInfo)
                        {
                            var dbUrunInfo = new DBservices.UrunInfo();
                            dbUrunInfo.URUN_ID = urun.ID;
                            dbUrunInfo.URUN_ISMI = item.urunName;
                            dbUrunInfo.FIYAT = item.fiyat;
                            dbUrunInfo.FIYAT_BIRIM = item.fiyatBirim;
                            dbUrunInfo.URUN_ACIKLAMA = item.urunAciklama;
                            dbUrunInfo.ADET_TIPI = item.adetTipi;
                            dbUrunInfo.LANGUAGE_ID = Convert.ToInt32(item.languageId);
                            dbUrunInfo.FIYAT_METIN = item.metinselFiyat;
                            dbUrunInfo.RESIM_ID = item.resimId;
                            db.UrunInfo.Add(dbUrunInfo);
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Ürün Başarıyla Eklendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/Product";
                    }
                    else
                    {
                        urun.MEKAN_ID = Convert.ToInt32(adminProductView.mekanId);
                        urun.URUN_RESIM_ID = adminProductView.urunResimId;
                        urun.URUN_RESIM_BUYUK_ID = adminProductView.urunResimBuyukId;
                        urun.PISME_SURESI = adminProductView.pismeSuresi;
                        urun.KALORI = adminProductView.kalori;
                        urun.URUN_KODU = adminProductView.urunKodu;
                        urun.PLU_NO = adminProductView.pluNo;
                        urun.STATE = Convert.ToInt32(adminProductView.state);
                        urun.UPDATED_DATE = DateTime.Now;
                        urun.sureTipi = adminProductView.pismeSuresiTipi;
                        db.Entry(urun).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        foreach (var item in adminProductView.urunInfo)
                        {
                            var dbUrunInfo = db.UrunInfo.Where(ii => ii.ID == item.id).FirstOrDefault();
                            if (dbUrunInfo == null)
                            {
                                dbUrunInfo = new DBservices.UrunInfo();
                                dbUrunInfo.URUN_ID = urun.ID;
                                dbUrunInfo.URUN_ISMI = item.urunName;
                                dbUrunInfo.FIYAT = item.fiyat;
                                dbUrunInfo.FIYAT_BIRIM = item.fiyatBirim;
                                dbUrunInfo.URUN_ACIKLAMA = item.urunAciklama;
                                dbUrunInfo.ADET_TIPI = item.adetTipi;
                                dbUrunInfo.LANGUAGE_ID = Convert.ToInt32(item.languageId);
                                dbUrunInfo.FIYAT_METIN = item.metinselFiyat;
                                dbUrunInfo.RESIM_ID = item.resimId;
                                db.UrunInfo.Add(dbUrunInfo);
                            }
                            else
                            {
                                dbUrunInfo.URUN_ISMI = item.urunName;
                                dbUrunInfo.FIYAT = item.fiyat;
                                dbUrunInfo.FIYAT_BIRIM = item.fiyatBirim;
                                dbUrunInfo.URUN_ACIKLAMA = item.urunAciklama;
                                dbUrunInfo.FIYAT_METIN = item.metinselFiyat;
                                dbUrunInfo.RESIM_ID = item.resimId;
                                db.Entry(dbUrunInfo).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Ürün Başarıyla Güncellendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/Product";
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
