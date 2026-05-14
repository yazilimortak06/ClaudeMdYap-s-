// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\AdminKategoriController.cs
using Newtonsoft.Json;
using PixDinn.Models.AdminModels;
using PixDinn.Models.AdminModels.AdminGlobalModels;
using PixDinn.Models.AdminModels.AdminViewModels;
using PixDinn.Processes;
using PixDinn.Processes.AdminGlobalProcesses;
using PixDinn.Processes.RedisManagment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PixDinn.Processes.GlobalEnums;

namespace PixDinn.Controllers
{
    public class AdminKategoriController : Controller
    {
        // GET: AdminKategori
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_KATEGORI, openingType = GlobalEnums.PageOpeningType.DIRECT)]

        public ActionResult Index([DefaultValue(0)]int UstID)
        {
            try
            {
                AdminKategoriViewModel kategoriViewModel = new AdminKategoriViewModel();
                String kategoriAdi = "";
                using(var db = new DBservices.pixdinnEntities())
                {
                    var dbKAtegori = db.Kategoriler.Where(kk => kk.ID == UstID).FirstOrDefault();
                    if(dbKAtegori != null)
                    {
                        var languageId = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguageId((int)dbKAtegori.MEKAN_ID);
                        var kategoriInfo = db.KategoriInfo.Where(kk => kk.KATEGORI_ID == dbKAtegori.ID && kk.LANGUAGE_ID == languageId).FirstOrDefault();
                        if(kategoriInfo != null)
                        {
                            kategoriAdi = kategoriInfo.KATEGORI_ADI;
                        }
                        kategoriViewModel.mekanlar = new List<SelectListItem>();
                        var mekan = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar().Where(vv => vv.Value == dbKAtegori.MEKAN_ID + "").FirstOrDefault();
                        kategoriViewModel.mekanlar.Add(mekan);
                    }
                    else
                    {
                        kategoriViewModel.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
                    }
                }

                #region Global değişkenler ayarlanıyor
                kategoriViewModel.ustId = UstID;
                kategoriViewModel.menuname = "Kategoriler";
                kategoriViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
                kategoriViewModel.urlQueryString = "/AdminKategori";
                kategoriViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true,
                    ikon = "icon-home",
                    link = "/adminhome",
                    pagetitle = "Anasayfa"
                });
                if (UstID!=0)
                {
                    kategoriViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                    {
                        active = true,
                        ikon = "icon-file-empty",
                        link = "/AdminKategori",
                        pagetitle = "Kategoriler"
                    });
                    kategoriViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                    {
                        active = true,
                        ikon = "icon-file-empty",
                        link = "/AdminKategori/Index/?UstID="+ UstID,
                        pagetitle = kategoriAdi
                    });
                }
                else
                {
                    kategoriViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                    {
                        active = true,
                        ikon = "icon-file-empty",
                        link = "/AdminKategori",
                        pagetitle = "Kategoriler"
                    });
                }
                #endregion
                return View(kategoriViewModel);

            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageException");
            }
        }

        #region Kategoriler  çekiliyor
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_KATEGORI_LIST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getKategorilerrData(string pData)
        {
            var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.mekanObj;
            var sqlExtra = mekanObj != null ? " AND K.MEKAN_ID IN " + mekanObj.mekanSqlWhere : "";

            var query = @"
                          select K.ID,KI.KATEGORI_ADI,MI.MEKAN_ADI,K.KATEGORI_LEVEL,K.UST_KATEGORI_ID,K.STATE,KI2.KATEGORI_ADI as UST_KATEGORI_ADI  from Kategoriler K
                            left join KategoriInfo KI on K.ID=KI.KATEGORI_ID
                            left join MekanInfo MI on K.MEKAN_ID=MI.MEKAN_ID
							left join Languages L on KI.LANGUAGE_ID = L.ID
		                    left join Languages L2 on MI.LANGUAGE_ID = L2.ID
                            left join KategoriInfo KI2 on K.UST_KATEGORI_ID = KI2.KATEGORI_ID
                            left join Languages L12 on KI2.LANGUAGE_ID = L12.ID
							WHERE L.ONCELIKLI = 1 and L2.ONCELIKLI = 1  AND K.STATE !=3
                    " + sqlExtra;
            var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getdataTableProcess().getDataTableData("AdminKategoriler", pData, query, " K.ID asc ");
            return Json(datatableData);
        }
        #endregion

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_KATEGORI, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        [HttpGet]
        public ActionResult addKategori([DefaultValue(0)]int ID, [DefaultValue(0)]int IdUst, [DefaultValue(0)]int mekanId)
        {
            AdminKategoriViewModel adminKategoriView = new AdminKategoriViewModel();
            adminKategoriView.formType = GlobalEnums.AdminFormTypes.ADD_KATEGORI;
            adminKategoriView.menuname = "Kategori Ekle";
            adminKategoriView.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            adminKategoriView.urlQueryString = "/AdminKategori/addKategori";
            adminKategoriView.kategoriState = new List<SelectListItem>();
            adminKategoriView.mekanlar = new List<SelectListItem>();
            adminKategoriView.UstKategoriGrup = new List<SelectListItem>();
            adminKategoriView.kategoriInfo = new List<AdminKategoriViewModel.KategoriInfo>();
            adminKategoriView.languages = new List<SelectListItem>();

            using (var db = new DBservices.pixdinnEntities())
            {
                adminKategoriView.mekanlar = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getMekanlar();
                var dbKategori = db.Kategoriler.Where(x => x.ID == ID).FirstOrDefault();

                #region Kategori insert
                if (dbKategori == null)
                {
                    List<DBservices.KategoriInfo> kategorilers = db.KategoriInfo.Where(x => x.LANGUAGE_ID == 1).ToList();
                    adminKategoriView.UstKategoriGrup.Add(new SelectListItem { Text = null, Value = "0" });
                    foreach (var item in kategorilers)
                    {
                        adminKategoriView.UstKategoriGrup.Add(new SelectListItem { Text = item.KATEGORI_ADI, Value = item.KATEGORI_ID.ToString() });
                    }
                    adminKategoriView.ustId = IdUst;
                    if(mekanId != 0) adminKategoriView.mekanId = ""+mekanId;
                }
                #endregion
                #region Kategori update
                else
                {
                    List<DBservices.KategoriInfo> kategorilers = db.KategoriInfo.Where(x => x.LANGUAGE_ID == 1 && x.KATEGORI_ID != dbKategori.ID).ToList();
                    adminKategoriView.UstKategoriGrup.Add(new SelectListItem { Text = null, Value = "0" });
                    foreach (var item in kategorilers)
                    {
                        adminKategoriView.UstKategoriGrup.Add(new SelectListItem { Text = item.KATEGORI_ADI, Value = item.KATEGORI_ID.ToString() });
                    }
                    adminKategoriView.id = dbKategori.ID;
                    adminKategoriView.kategoriResmi = (int)dbKategori.KATEGORI_RESMI;
                    adminKategoriView.kategoriLevel = (int)dbKategori.KATEGORI_LEVEL;
                    adminKategoriView.kategoriUrl = dbKategori.KATEGORI_URL;
                    adminKategoriView.ustId = (int)dbKategori.UST_KATEGORI_ID;
                    adminKategoriView.state = dbKategori.STATE.ToString();
                    adminKategoriView.mekanId = dbKategori.MEKAN_ID.ToString();

                    var dbKategoriInfo = db.KategoriInfo.Where(x => x.KATEGORI_ID == dbKategori.ID).ToList();
                    foreach (var item in dbKategoriInfo)
                    {
                        adminKategoriView.kategoriInfo.Add(new AdminKategoriViewModel.KategoriInfo()
                        {
                            languageId = item.LANGUAGE_ID.ToString(),
                            kategoriAdi = item.KATEGORI_ADI,
                            title = item.TITLE,
                            id = item.ID,
                            resimId = item.RESIM_ID.GetValueOrDefault(),
                            subResimId = item.SUB_RESIM_ID.GetValueOrDefault(),
                        });
                    }
                    adminKategoriView.infoJsonValue = JsonConvert.SerializeObject(adminKategoriView.kategoriInfo);
                }
                #endregion

                #region Diller ekleniyor
                if (mekanId != 0)
                    adminKategoriView.languages = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguages(mekanId);
                else
                    adminKategoriView.languages = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getLanguages(Convert.ToInt32(adminKategoriView.mekanId));
                #endregion
            }

            return View(adminKategoriView);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_ADD_KATEGORI_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult addKategori(AdminKategoriViewModel adminKategoriView)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";
            result.redirect = false;
            adminKategoriView.kategoriInfo = new List<AdminKategoriViewModel.KategoriInfo>();

            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var kategori = db.Kategoriler.Where(x => x.ID == adminKategoriView.id).FirstOrDefault();
                    adminKategoriView.kategoriInfo = JsonConvert.DeserializeObject<List<AdminKategoriViewModel.KategoriInfo>>(adminKategoriView.infoJsonValue);
                    #region kategori insert
                    if (kategori == null)
                    {
                        kategori = new DBservices.Kategoriler();
                        kategori.MEKAN_ID = Convert.ToInt32(adminKategoriView.mekanId);
                        kategori.KATEGORI_RESMI = adminKategoriView.kategoriResmi;
                        kategori.KATEGORI_LEVEL = adminKategoriView.ustId != 0 ?
                            (db.Kategoriler.Where(x => x.ID == adminKategoriView.ustId).FirstOrDefault()?.KATEGORI_LEVEL ?? 0) + 1 : 1;
                        kategori.UST_KATEGORI_ID = adminKategoriView.ustId;
                        kategori.STATE = Convert.ToInt32(adminKategoriView.state);
                        kategori.KATEGORI_URL = adminKategoriView.kategoriUrl;
                        db.Kategoriler.Add(kategori);
                        db.SaveChanges();
                        foreach (var item in adminKategoriView.kategoriInfo)
                        {
                            var dbKategoriInfo = new DBservices.KategoriInfo();
                            dbKategoriInfo.KATEGORI_ID = kategori.ID;
                            dbKategoriInfo.KATEGORI_ADI = item.kategoriAdi;
                            dbKategoriInfo.RESIM_ID = item.resimId;
                            dbKategoriInfo.SUB_RESIM_ID = item.subResimId;
                            dbKategoriInfo.LANGUAGE_ID = Convert.ToInt32(item.languageId);
                            db.KategoriInfo.Add(dbKategoriInfo);
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Kategori Başarıyla Eklendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/AdminKategori";
                    }
                    #endregion
                    #region Kategori update
                    else
                    {
                        kategori.MEKAN_ID = Convert.ToInt32(adminKategoriView.mekanId);
                        kategori.KATEGORI_RESMI = adminKategoriView.kategoriResmi;
                        kategori.KATEGORI_LEVEL = adminKategoriView.kategoriLevel;
                        kategori.UST_KATEGORI_ID = adminKategoriView.ustId;
                        kategori.STATE = Convert.ToInt32(adminKategoriView.state);
                        kategori.KATEGORI_URL = adminKategoriView.kategoriUrl;
                        db.Entry(kategori).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        foreach (var item in adminKategoriView.kategoriInfo)
                        {
                            var dbKategoriInfo = db.KategoriInfo.Where(ii => ii.ID == item.id).FirstOrDefault();
                            if (dbKategoriInfo == null)
                            {
                                dbKategoriInfo = new DBservices.KategoriInfo();
                                dbKategoriInfo.KATEGORI_ID = kategori.ID;
                                dbKategoriInfo.KATEGORI_ADI = item.kategoriAdi;
                                dbKategoriInfo.TITLE = item.title;
                                dbKategoriInfo.LANGUAGE_ID = Convert.ToInt32(item.languageId);
                                dbKategoriInfo.RESIM_ID = item.resimId;
                                dbKategoriInfo.SUB_RESIM_ID = item.subResimId;
                                db.KategoriInfo.Add(dbKategoriInfo);
                            }
                            else
                            {
                                dbKategoriInfo.KATEGORI_ADI = item.kategoriAdi;
                                dbKategoriInfo.TITLE = item.title;
                                dbKategoriInfo.RESIM_ID = item.resimId;
                                dbKategoriInfo.SUB_RESIM_ID = item.subResimId;
                                db.Entry(dbKategoriInfo).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Kategori Başarıyla Güncellendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/AdminKategori";
                    }
                    #endregion
                }
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region kategori  silme
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.KATEGORI_SIL, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult kategoriSil([DefaultValue(0)]int ID)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";
            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var dbKategori = db.Kategoriler.Where(x => x.ID == ID).FirstOrDefault();
                    if (dbKategori != null)
                    {
                        dbKategori.STATE = (int)GlobalEnums.AdminDataState.DELETED;
                        db.Entry(dbKategori).State = System.Data.Entity.EntityState.Modified;
                        var dbMenuUrunler = db.MenuUrunlerTemp.Where(x => x.KATEGORI_ID == dbKategori.ID).ToList();
                        foreach (var urunler in dbMenuUrunler)
                        {
                            urunler.STATE = (int)GlobalEnums.AdminDataState.DELETED;
                            db.Entry(urunler).State = System.Data.Entity.EntityState.Modified;
                        }
                        int dbState = db.SaveChanges();
                        try
                        {
                            var dateTimeNow = DateTime.Now;
                            QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
                            RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getRedisCacheManager();
                            var dbMenuler = db.MenulerTemp.Where(x => x.MEKAN_ID == dbKategori.MEKAN_ID && x.STATE == (int)AdminDataState.ACTIVE).ToList();
                            foreach (var menu in dbMenuler)
                            {
                                qrMenuDbProcess.MenuId = menu.ID;
                                qrMenuDbProcess.MenuUpdateDate = dateTimeNow;
                                manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);
                            }
                        }
                        catch (Exception ee) { }
                        result.IsSuccess = true;
                        result.MessageTitle = "Başarılı";
                        result.MessageBody = "Kategori Başarıyla Silindi";
                    }
                }
            }
            catch (Exception ee)
            {
                result.MessageBody = ee.Message;
                result.MessageTitle = "Hata";
                result.IsSuccess = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
