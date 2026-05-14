// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\AdminLoginController.cs
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
        // GET: AdminLogin
        #region Login formu
        public ActionResult Index()
        {
            try
            {
                #region Kullanıcı girişi varsa yönlendir
                var loginState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
                if (loginState.loginstate == Models.AdminModels.AdminLoginSessionResponse.Loginstateenum.basarili)
                {
                    return Redirect("/adminhome");
                }
                #endregion

                #region Sayfanın verileri oluşturuluyor
                AdminLoginPageModel adminLoginPageModel = new AdminLoginPageModel();
                adminLoginPageModel.errorState = 0;
                adminLoginPageModel.errorMessage = "Hata";
                String pageKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                #endregion

                #region Cihaz bilgileri alınıp kaydediliyor
                String ipAddress = HttpContext.Request.UserHostAddress;
                String userAgentData = Request.UserAgent;

                using (var db = new pixdinnEntities())
                {
                    #region İp engellemesi kontrol ediliyor
                    var ipData = db.AdminLoginDevices.Where(ld => ld.IP_ADDRESS == ipAddress && ld.DEVICE_ACCESS_STATE == (int)GlobalEnums.DeviceAccessState.IP_BANNED).FirstOrDefault();
                    if (ipData != null)
                    {
                        return Redirect("/adminhelper/PageAccessNotFound");
                    }
                    #endregion

                    var deviceData = db.AdminLoginDevices.Where(vv => vv.IP_ADDRESS == ipAddress && vv.BROWSER_DATA == userAgentData).FirstOrDefault();
                    if (deviceData == null)
                    {
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

                        adminLoginPageModel.errorState = GlobalEnums.PageGeneralError.SUCCESS;
                        adminLoginPageModel.pageKey = pageKey;
                    }
                    else
                    {
                        if (deviceData.DEVICE_ACCESS_STATE == (int)GlobalEnums.DeviceAccessState.SUCCESS)
                        {
                            deviceData.BROWSER_DATA = userAgentData;
                            deviceData.DEVICE_STATE = (int)GlobalEnums.LoginDeviceState.ONLOGINPAGE;
                            deviceData.STATE_DATE = DateTime.Now;
                            db.Entry(deviceData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            var loginData = db.AdminLogins.Where(ll => ll.DEVICE_ID == deviceData.ID).FirstOrDefault();
                            if (loginData != null)
                            {
                                loginData.DEVICE_ID = deviceData.ID;
                                loginData.LOGIN_PAGE_STATE_DATE = DateTime.Now;
                                loginData.LOGIN_KEY_DATA = pageKey;
                                loginData.LOGIN_PAGE_STATE = (int)GlobalEnums.LoginPageState.NORMAL;
                                db.Entry(loginData).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var insertedLoginData = new AdminLogins();
                                insertedLoginData.DEVICE_ID = deviceData.ID;
                                insertedLoginData.LOGIN_PAGE_STATE_DATE = DateTime.Now;
                                insertedLoginData.LOGIN_FAIL_COUNT = 0;
                                insertedLoginData.LOGIN_KEY_DATA = pageKey;
                                insertedLoginData.LOGIN_PAGE_STATE = (int)GlobalEnums.LoginPageState.NORMAL;
                                db.AdminLogins.Add(insertedLoginData);
                                db.SaveChanges();
                            }

                            adminLoginPageModel.errorState = GlobalEnums.PageGeneralError.SUCCESS;
                            adminLoginPageModel.pageKey = pageKey;
                        }
                    }
                }
                #endregion

                return View(adminLoginPageModel);
            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageAccessNotFound");
            }
        }
        #endregion

        #region Login formu post işlemi
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
                String passwordMd5 = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).decryptMd5(password);

                AdminLoginJsonModel result = new AdminLoginJsonModel();
                result.IsSuccess = false;
                result.MessageBody = "";
                result.MessageTitle = "";

                AdminUsers adminUser = null;
                RootAdmin rootAdmin = null;
                AdminLogins adminLogin = null;
                AdminLoginDevices loginDevice = null;
                AdminLoginSessionRequest loginSessionRequest = null;
                DateTime nowDate = DateTime.Now;

                using (var db = new pixdinnEntities())
                {
                    loginDevice = db.AdminLoginDevices.Where(ll => ll.BROWSER_DATA == userAgentData && ll.IP_ADDRESS == ipAddress).FirstOrDefault();
                    if (loginDevice == null || loginDevice.DEVICE_ACCESS_STATE != (int)GlobalEnums.DeviceAccessState.SUCCESS)
                    {
                        result.IsSuccess = false;
                        result.MessageBody = "Giriş Bilgileriniz Hatalı Lütfen Tekrar Deneyiniz";
                        result.MessageTitle = "Hatalı Giriş";
                    }
                    else
                    {
                        adminLogin = db.AdminLogins.Where(al => al.DEVICE_ID == loginDevice.ID && al.LOGIN_KEY_DATA == pageKey && al.LOGIN_PAGE_STATE == (int)GlobalEnums.LoginPageState.NORMAL).FirstOrDefault();
                        if (adminLogin != null)
                        {
                            adminUser = db.AdminUsers.Where(ad => ad.USERNAME == userName && ad.PASSWORD == passwordMd5).FirstOrDefault();
                            rootAdmin = db.RootAdmin.Where(ad => ad.USERNAME == userName && ad.PASSWORD == passwordMd5).FirstOrDefault();

                            String cookieKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                            String cookieId = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                            String sessionKey = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                            String sessionId = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);

                            if (adminUser == null && rootAdmin == null)
                            {
                                result.IsSuccess = false;
                                result.MessageBody = "Giriş Bilgileriniz Hatalı Lütfen Tekrar Deneyiniz";
                                result.MessageTitle = "Hatalı Giriş";
                            }
                            else
                            {
                                if (rootAdmin != null)
                                {
                                    loginSessionRequest = new AdminLoginSessionRequest();
                                    loginSessionRequest.userId = rootAdmin.ID;
                                    loginSessionRequest.adminType = GlobalEnums.LoginAdminType.ROOT;
                                }
                                else if (adminUser != null)
                                {
                                    loginSessionRequest = new AdminLoginSessionRequest();
                                    loginSessionRequest.userId = adminUser.ID;
                                    loginSessionRequest.adminType = (GlobalEnums.LoginAdminType)adminUser.ADMIN_TYPE;
                                }

                                if (loginSessionRequest != null)
                                {
                                    var loginSession = db.AdminLoginSessions.Where(als => als.LOGIN_ID == adminLogin.ID && als.USER_ID == adminLogin.USER_ID).FirstOrDefault();
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
                                        loginSession.STATE = (int)GlobalEnums.AdminLoginSessionState.ACTIVE;
                                        loginSession.USER_ID = loginSessionRequest.userId;
                                        loginSession.USER_TYPE = (int)loginSessionRequest.adminType;
                                        loginSession.COOKIE_ID = cookieId;
                                        loginSession.COOKIE_KEY = cookieKey;
                                        loginSessionRequest.loginSessionId = loginSession.ID;
                                        db.Entry(loginSession).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    loginSessionRequest.deviceId = loginDevice.ID;
                                    loginSessionRequest.cookieId = cookieId;
                                    loginSessionRequest.cookieKey = cookieKey;
                                    loginSessionRequest.loginId = adminLogin.ID;
                                    loginSessionRequest.sessionId = sessionId;
                                    loginSessionRequest.sessionKey = sessionKey;

                                    Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().saveSession(loginSessionRequest);
                                    result.IsSuccess = true;
                                    result.MessageBody = "Giriş Bilgileriniz Doğru Anasayfaya Yönlendiriliyorsunuz";
                                    result.MessageTitle = "Giriş Başarılı";
                                    result.redirect = true;
                                    result.redirectUrl = "/adminhome";
                                }
                            }

                            adminLogin.LOGIN_KEY_DATA = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getRandomString(120);
                            db.Entry(adminLogin).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ee)
            {
                return Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getExceptionError(ee);
            }
        }
        #endregion

        #region Çıkış Yap
        public ActionResult Cikis()
        {
            var loginedState = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState();
            if (loginedState.loginstate == Models.AdminModels.AdminLoginSessionResponse.Loginstateenum.basarili)
            {
                String cookieId = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String cookieKey = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String sessionId = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);
                String sessionKey = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRandomString(120);

                using (var db = new DBservices.pixdinnEntities())
                {
                    db.AdminLoginSessions.Where(pp => pp.USER_ID == loginedState.loginedadmin.id && pp.USER_TYPE == (int)loginedState.loginedadmin.loginAdminType).ToList().ForEach(ii =>
                    {
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
        #endregion
    }
}
