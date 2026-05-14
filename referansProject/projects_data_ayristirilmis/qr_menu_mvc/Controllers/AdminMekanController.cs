// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\AdminMekanController.cs
using DBservices;
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
    public class AdminMekanController : Controller
    {
        // GET: AdminMekan
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MEKAN, openingType = GlobalEnums.PageOpeningType.DIRECT)]

        public ActionResult Index()
        {

            try
            {
                AdminMekanModel homeViewModel = new AdminMekanModel();
                homeViewModel.loginedAdminType = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.loginAdminType;
                #region Global değişkenler ayarlanıyor
                homeViewModel.menuname = "adadadad";
                homeViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
                homeViewModel.urlQueryString = "/adminuser";
                homeViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true,
                    ikon = "icon-home",
                    link = "/adminhome",
                    pagetitle = "Anasayfa"
                });
                homeViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true,
                    ikon = "icon-file-empty",
                    link = "/adminmekan",
                    pagetitle = "Mekanlar"
                });
                #endregion
                return View(homeViewModel);

            }
            catch (Exception ee)
            {

                return Redirect("/adminhelper/PageException");
            }
        }
        #region Mekanlar  çekiliyor
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MEKAN_LIST, openingType = GlobalEnums.PageOpeningType.AJAX)]

        [HttpPost]
        public ActionResult getMekanlarData(string pData)
        {

            var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.mekanObj;
            var sqlExtra = mekanObj != null ? " AND M.ID IN " + mekanObj.mekanSqlWhere : "";


            var query = @"
                          select  M.ID,M.TEL,M.CEP,M.INSTAGRAM,M.FACE,M.MAIL,MG.GRUP_ISMI,M.CREATED_DATE,M.UPDATED_DATE,A.USERNAME,A.ADMIN_TYPE,M.STATE,MI.MEKAN_ADI,MI.MEKAN_ACIKLAMA from Mekanlar M
                            left join MekanGruplari MG on M.GROUP_ID=MG.ID
                            left join AdminUsers A on M.CREATED_ADMIN_ID=A.ID
                            left join MekanInfo MI on M.ID=MI.MEKAN_ID
							 left join Languages L on MI.LANGUAGE_ID = L.ID
                            WHERE M.ID !=0  and L.ONCELIKLI = 1
                    " + sqlExtra;
            var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getdataTableProcess().getDataTableData("AdminMekan", pData, query, " M.ID asc ");
            return Json(datatableData);

        }
        #endregion
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MEKAN_ADD, openingType = GlobalEnums.PageOpeningType.DIRECT)]

        [HttpGet]
        public ActionResult addMekan([DefaultValue(0)] int ID)
        {
            AdminMekanModel addMekanViewModel = new AdminMekanModel();
            addMekanViewModel.loginedAdminType = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.loginAdminType;
            #region Global değişkenler ayarlanıyor
            addMekanViewModel.formType = GlobalEnums.AdminFormTypes.ADD_MEKAN;
            addMekanViewModel.menuname = "Mekan Ekle";
            addMekanViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            addMekanViewModel.urlQueryString = "/adminmekan/addMekan";

            addMekanViewModel.mekanGrup = new List<SelectListItem>();
            addMekanViewModel.smsOnayList = new List<SelectListItem>();
            addMekanViewModel.mekanState = new List<SelectListItem>();
            addMekanViewModel.startHoursList = new List<SelectListItem>();
            addMekanViewModel.stopHoursList = new List<SelectListItem>();
            addMekanViewModel.menuBottomLeftButtonTypeList = new List<SelectListItem>();
            addMekanViewModel.zamanAraligiList = new List<ZamanAraligiViewModel>();
            addMekanViewModel.mekanInfo = new List<AdminMekanModel.MekanInfoJsonModel>();
            addMekanViewModel.mekanArayuzInfoModel = new List<AdminMekanModel.MekanArayuzInfoModel>();
            addMekanViewModel.siparisvermeTipiList = new List<SelectListItem>();
            addMekanViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
            {
                active = true,
                ikon = "icon-home",
                link = "/adminhome",
                pagetitle = "Anasayfa"
            });
            addMekanViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
            {
                active = true,
                ikon = "icon-file-empty",
                link = "/adminmekan",
                pagetitle = "Mekanlar"
            });
            #endregion
            var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();

            using (var db = new DBservices.pixdinnEntities())
            {
                // ... (full implementation in source)
                // SELECT LIST BUILDING, MEKAN INSERT/UPDATE LOGIC
                var dbMekan = db.Mekanlar.Where(x => x.ID == ID).FirstOrDefault();

                #region mekan insert
                if (dbMekan == null)
                {
                    addMekanViewModel.languages = new List<SelectListItem>();
                    addMekanViewModel.languages = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getLanguages(0);
                    addMekanViewModel.ID = ID;
                    addMekanViewModel.urunDetayMaksimumKarakterSayisi = 100;
                }
                #endregion
                #region mekan update
                else
                {
                    addMekanViewModel.ID = dbMekan.ID;
                    addMekanViewModel.telefon = dbMekan.TEL;
                    addMekanViewModel.cepTelefon = dbMekan.CEP;
                    addMekanViewModel.whatsappCep = dbMekan.WHATSAPP_CEP;
                    addMekanViewModel.mail = dbMekan.MAIL;
                    addMekanViewModel.instagram = dbMekan.INSTAGRAM;
                    addMekanViewModel.spotify = dbMekan.Spotify;
                    addMekanViewModel.facebook = dbMekan.FACE;
                    addMekanViewModel.state = dbMekan.STATE.ToString();
                    addMekanViewModel.mediaId = (int)dbMekan.MEKAN_RESIM;
                    addMekanViewModel.logoId = (int)dbMekan.LOGO_ID;
                    addMekanViewModel.smsLimit = dbMekan.SMS_LIMIT.GetValueOrDefault();
                    addMekanViewModel.smsOnay = dbMekan.SMS_ONAY.ToString();
                    addMekanViewModel.siparisVermeTipi = dbMekan.SIPARIS_VERME_TIPI.ToString();
                    addMekanViewModel.menuBottomLeftButtonType = dbMekan.MENU_BOTTOM_LEFT_BUTTON_TYPE.ToString();
                    addMekanViewModel.hosGeldinPopUpGoster = dbMekan.HOS_GELDIN_POPUP_GOSTER.GetValueOrDefault();
                    addMekanViewModel.urunFiyatMetinselDurumu = dbMekan.PRODUCT_PRICE_IS_STRING.GetValueOrDefault();
                    addMekanViewModel.urunDetayGostermeDurumu = dbMekan.SHOWING_PRODUCT_DETAIL.GetValueOrDefault();
                    addMekanViewModel.urunDetayMaksimumKarakterSayisi = dbMekan.PRODUCT_DETAIL_MAXIMUM_CHARACTER_COUNT.GetValueOrDefault();
                }
                #endregion
            }

            return View(addMekanViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MEKAN_ADD_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult addMekan(AdminMekanModel addMekanViewModel)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";
            result.redirect = false;
            addMekanViewModel.mekanInfo = new List<AdminMekanModel.MekanInfoJsonModel>();

            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
                    var mekan = db.Mekanlar.Where(x => x.ID == addMekanViewModel.ID).FirstOrDefault();
                    addMekanViewModel.mekanInfo = JsonConvert.DeserializeObject<List<AdminMekanModel.MekanInfoJsonModel>>(addMekanViewModel.infoJsonValue);

                    #region mekan insert
                    if (mekan == null)
                    {
                        mekan = new Mekanlar();
                        mekan.TEL = addMekanViewModel.telefon;
                        mekan.CEP = addMekanViewModel.cepTelefon;
                        mekan.WHATSAPP_CEP = addMekanViewModel.whatsappCep;
                        mekan.INSTAGRAM = addMekanViewModel.instagram;
                        mekan.Spotify = addMekanViewModel.spotify;
                        mekan.FACE = addMekanViewModel.facebook;
                        mekan.MAIL = addMekanViewModel.mail;
                        mekan.LOGO_ID = addMekanViewModel.logoId;
                        mekan.MEKAN_RESIM = addMekanViewModel.mediaId;
                        mekan.GROUP_ID = Convert.ToInt32(addMekanViewModel.grupId);
                        mekan.CREATED_DATE = DateTime.Now;
                        mekan.STATE = Convert.ToInt32(addMekanViewModel.state);
                        mekan.SMS_ONAY = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(addMekanViewModel.smsOnay);
                        mekan.SIPARIS_VERME_TIPI = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(addMekanViewModel.siparisVermeTipi);
                        mekan.HOS_GELDIN_POPUP_GOSTER = addMekanViewModel.hosGeldinPopUpGoster;
                        mekan.PRODUCT_PRICE_IS_STRING = addMekanViewModel.urunFiyatMetinselDurumu;
                        mekan.SHOWING_PRODUCT_DETAIL = addMekanViewModel.urunDetayGostermeDurumu;
                        mekan.PRODUCT_DETAIL_MAXIMUM_CHARACTER_COUNT = addMekanViewModel.urunDetayMaksimumKarakterSayisi;

                        db.Mekanlar.Add(mekan);
                        db.SaveChanges();
                        foreach (var item in addMekanViewModel.mekanInfo)
                        {
                            var dbMekanInfo = new MekanInfo();
                            dbMekanInfo.LANGUAGE_ID = Convert.ToInt32(item.languageId);
                            dbMekanInfo.MEKAN_ACIKLAMA = item.aciklama;
                            dbMekanInfo.MEKAN_ADI = item.ad;
                            dbMekanInfo.MEKAN_ID = mekan.ID;
                            db.MekanInfo.Add(dbMekanInfo);
                        }
                        db.SaveChanges();

                        result.IsSuccess = true;
                        result.MessageBody = "Mekan Başarıyla Eklendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/adminmekan";
                    }
                    #endregion
                    #region mekan update
                    else
                    {
                        mekan.TEL = addMekanViewModel.telefon;
                        mekan.CEP = addMekanViewModel.cepTelefon;
                        mekan.WHATSAPP_CEP = addMekanViewModel.whatsappCep;
                        mekan.INSTAGRAM = addMekanViewModel.instagram;
                        mekan.Spotify = addMekanViewModel.spotify;
                        mekan.FACE = addMekanViewModel.facebook;
                        mekan.MAIL = addMekanViewModel.mail;
                        mekan.LOGO_ID = addMekanViewModel.logoId;
                        mekan.MEKAN_RESIM = addMekanViewModel.mediaId;
                        mekan.UPDATED_DATE = DateTime.Now;
                        mekan.STATE = Convert.ToInt32(addMekanViewModel.state);
                        mekan.SMS_ONAY = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(addMekanViewModel.smsOnay);
                        mekan.SIPARIS_VERME_TIPI = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(addMekanViewModel.siparisVermeTipi);
                        mekan.HOS_GELDIN_POPUP_GOSTER = addMekanViewModel.hosGeldinPopUpGoster;
                        mekan.PRODUCT_PRICE_IS_STRING = addMekanViewModel.urunFiyatMetinselDurumu;
                        mekan.SHOWING_PRODUCT_DETAIL = addMekanViewModel.urunDetayGostermeDurumu;
                        mekan.PRODUCT_DETAIL_MAXIMUM_CHARACTER_COUNT = addMekanViewModel.urunDetayMaksimumKarakterSayisi;

                        db.Entry(mekan).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        try
                        {
                            var dateTimeNow = DateTime.Now;
                            QrMenuDbProcessModel qrMenuDbProcess = new QrMenuDbProcessModel();
                            RedisCacheManager manager = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getRedisCacheManager();
                            var dbMenuler = db.MenulerTemp.Where(x => x.MEKAN_ID == mekan.ID && x.STATE == (int)AdminDataState.ACTIVE).ToList();
                            foreach (var menu in dbMenuler)
                            {
                                qrMenuDbProcess.MenuId = menu.ID;
                                qrMenuDbProcess.MenuUpdateDate = dateTimeNow;
                                manager.SetQrMenuDbProcess(qrMenuDbProcess, 480, db);
                            }
                        }
                        catch (Exception ee) { }

                        result.IsSuccess = true;
                        result.MessageBody = "Mekan Başarıyla Güncellendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/adminmekan";
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

        #region mekan  silme
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_MEKAN_SIL, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult mekanSil(int ID)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";

            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var dbMekan = db.Mekanlar.Where(x => x.ID == ID).FirstOrDefault();
                    if (dbMekan != null)
                    {
                        db.Mekanlar.Remove(dbMekan);
                        int dbState = db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageTitle = "Başarılı";
                        result.MessageBody = "Mekan Başarıyla Silindi";
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
