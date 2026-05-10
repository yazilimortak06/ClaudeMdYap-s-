// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\AdminUsersController.cs
using DBservices;
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

namespace PixDinn.Controllers
{
    public class AdminUsersController : Controller
    {
        // GET: AdminUsers
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_USERS, openingType = GlobalEnums.PageOpeningType.DIRECT)]

        public ActionResult Index()
        {
            try
            {
                AdminUsersViewModel homeViewModel = new AdminUsersViewModel();
                homeViewModel.menuname = "adadadad";
                homeViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
                homeViewModel.urlQueryString = "/adminusers";
                homeViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-home", link = "/adminhome", pagetitle = "Anasayfa"
                });
                homeViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-file-empty", link = "/adminusers", pagetitle = "Kullanıcılar"
                });
                return View(homeViewModel);
            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageException");
            }
        }

        #region Kullanıcılar  çekiliyor
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_USERS_LIST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getKullanicilarData(string pData)
        {
            var query = @"
                          select A.ID,A.USERNAME,A.CREATED_DATE,A.STATE,A.ADMIN_TYPE from AdminUsers A
                            WHERE A.ID !=0
                    ";
            var datatableData = Singleton.getSingleton(GlobalEnums.ObjectEnums.NEW_OBJECT).getdataTableProcess().getDataTableData("AdminUsers", pData, query, " A.ID asc ");
            return Json(datatableData);
        }
        #endregion

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_USERS_ADD, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        [HttpGet]
        public ActionResult addUser([DefaultValue(0)]int ID)
        {
            AdminUsersViewModel addUserViewModel = new AdminUsersViewModel();
            addUserViewModel.formType = GlobalEnums.AdminFormTypes.ADD_USER;
            addUserViewModel.menuname = "Kullanıcı Ekle";
            addUserViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
            addUserViewModel.urlQueryString = "/adminusers/adduser";
            addUserViewModel.adminUserState = new List<SelectListItem>();
            addUserViewModel.adminTypeList = new List<SelectListItem>();

            using (var db = new DBservices.pixdinnEntities())
            {
                addUserViewModel.adminUserState.Add(new SelectListItem { Selected = true, Text = "Durum Seçiniz", Value = "-1" });
                addUserViewModel.adminUserState.Add(new SelectListItem { Text = "Aktif", Value = ((int)GlobalEnums.AdminDataState.ACTIVE).ToString() });
                addUserViewModel.adminUserState.Add(new SelectListItem { Text = "Pasif", Value = ((int)GlobalEnums.AdminDataState.PASSIVE).ToString() });

                var dbAdminUser = db.AdminUsers.Where(x => x.ID == ID).FirstOrDefault();

                if (dbAdminUser == null)
                {
                    addUserViewModel.id = 0;
                    addUserViewModel.password = "";
                    addUserViewModel.passwordRepeat = "";
                }
                else
                {
                    addUserViewModel.id = dbAdminUser.ID;
                    addUserViewModel.password = "1234";
                    addUserViewModel.passwordRepeat = "1234";
                    addUserViewModel.userName = dbAdminUser.USERNAME;
                    addUserViewModel.name = dbAdminUser.NAME;
                    addUserViewModel.surname = dbAdminUser.SURNAME;
                    addUserViewModel.mail = dbAdminUser.MAIL;
                    addUserViewModel.phoneCode = dbAdminUser.PHONE_CODE;
                    addUserViewModel.phone = dbAdminUser.PHONE;
                    addUserViewModel.state = dbAdminUser.STATE.ToString();
                    addUserViewModel.adminType = dbAdminUser.ADMIN_TYPE.ToString();
                    addUserViewModel.passwordWillChange = false;
                }
            }
            return View(addUserViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_USERS_ADD_POST, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult addUser(AdminUsersViewModel addUserViewModel)
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
                    var adminUser = db.AdminUsers.Where(x => x.ID == addUserViewModel.id).FirstOrDefault();
                    if (adminUser == null)
                    {
                        adminUser = new AdminUsers();
                        adminUser.USERNAME = addUserViewModel.userName;
                        adminUser.PASSWORD = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).decryptMd5(addUserViewModel.password);
                        adminUser.CREATED_DATE = DateTime.Now;
                        adminUser.NAME = addUserViewModel.name;
                        adminUser.SURNAME = addUserViewModel.userName;
                        adminUser.MAIL = addUserViewModel.mail;
                        adminUser.PHONE_CODE = addUserViewModel.phoneCode;
                        adminUser.PHONE = addUserViewModel.phone;
                        adminUser.STATE = Convert.ToInt32(addUserViewModel.state);
                        adminUser.ADMIN_TYPE = (int)GlobalEnums.LoginAdminType.NORMAL;
                        adminUser.AUTH_ID = 0;
                        db.AdminUsers.Add(adminUser);
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Kullanıcı Başarıyla Eklendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/adminusers";
                    }
                    else
                    {
                        adminUser.USERNAME = addUserViewModel.userName;
                        adminUser.PHONE_CODE = addUserViewModel.phoneCode;
                        adminUser.STATE = Convert.ToInt32(addUserViewModel.state);
                        adminUser.ADMIN_TYPE = (int)GlobalEnums.LoginAdminType.NORMAL;
                        adminUser.MAIL = addUserViewModel.mail;
                        adminUser.NAME = addUserViewModel.name;
                        adminUser.PHONE = addUserViewModel.phone;
                        adminUser.SURNAME = addUserViewModel.surname;
                        if (addUserViewModel.passwordWillChange)
                            adminUser.PASSWORD = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).decryptMd5(addUserViewModel.password);
                        db.Entry(adminUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.MessageBody = "Kullanıcı Başarıyla Güncellendi";
                        result.MessageTitle = "İşlem Başarılı";
                        result.redirect = true;
                        result.redirectUrl = "/adminusers";
                    }
                }
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Admin user  silme
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.ADMIN_USERS_SIL, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult adminUserSil(int id)
        {
            AdminJsonModel result = new AdminJsonModel();
            result.IsSuccess = false;
            result.MessageBody = "";
            result.MessageTitle = "";

            try
            {
                using (var db = new DBservices.pixdinnEntities())
                {
                    var dbAdminUser = db.AdminUsers.Where(x => x.ID == id).FirstOrDefault();
                    if (dbAdminUser != null)
                    {
                        db.AdminUsers.Remove(dbAdminUser);
                        int dbState = db.SaveChanges();
                        if (dbState > 0)
                        {
                            var dbAdminMekanUser = db.MekanKullanicilari.Where(x => x.USER_ID == dbAdminUser.ID).FirstOrDefault();
                            if (dbAdminMekanUser != null)
                            {
                                db.MekanKullanicilari.Remove(dbAdminMekanUser);
                                db.SaveChanges();
                            }
                            result.IsSuccess = true;
                            result.MessageTitle = "Başarılı";
                            result.MessageBody = "Admin  Kullanıcısı Başarıyla Silindi";
                        }
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
