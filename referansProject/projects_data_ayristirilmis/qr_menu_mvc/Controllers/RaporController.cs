// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Controllers\RaporController.cs
using Newtonsoft.Json;
using PixDinn.Models.AdminModels.AdminViewModels;
using PixDinn.Processes;
using PixDinn.Processes.AdminGlobalProcesses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PixDinn.Models.AdminModels.AdminViewModels.RaporMainViewModel;
using static PixDinn.Models.AdminModels.AdminViewModels.RaporMainViewModel.RaporDetayViewModel;
using static PixDinn.Models.AdminModels.AdminViewModels.RaporMainViewModel.RaporDetayViewModel.HareketRaporuGrafikViewModel;
using static PixDinn.Models.AdminModels.AdminViewModels.RaporMainViewModel.RaporDetayViewModel.SayiRaporuResponse;
using static PixDinn.Processes.GlobalEnums;

namespace PixDinn.Controllers
{
    public class RaporController : Controller
    {
        // GET: Rapor
        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.RAPOR, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        public ActionResult Index()
        {
            try
            {
                RaporMainViewModel raporViewModel = new RaporMainViewModel();
                raporViewModel.pagetitles = new List<Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem>();
                raporViewModel.urlQueryString = "/Rapor";
                raporViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-home", link = "/adminhome", pagetitle = "Anasayfa"
                });
                raporViewModel.pagetitles.Add(new Models.AdminModels.AdminGlobalModels.AdminPageGlobalModel.PageHeaderItem
                {
                    active = true, ikon = "icon-file-empty", link = "/rapor", pagetitle = "Raporlar"
                });

                raporViewModel.menuler = new List<SelectListItem>();
                var mekanObj = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getAdminLoginProcess().getLoginState().loginedadmin.mekanObj;
                var sqlExtra = mekanObj != null ? " AND M.ID IN " + mekanObj.mekanSqlWhere : "";

                var query = @"
                         select MT.ID,MI.MEKAN_ADI,MT.ACK,MI.MEKAN_ID as MEKAN_ID from MenulerTemp MT
                                left join Mekanlar M on MT.MEKAN_ID = M.ID
                                left join MekanInfo MI on M.ID = MI.MEKAN_ID
                                left join Languages L on MI.LANGUAGE_ID = L.ID
                                where L.ONCELIKLI = 1
                    " + sqlExtra;

                var sqlParameters = new List<SqlParameter> { };
                using (var db = new DBservices.pixdinnEntities())
                {
                    DataTable tab = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getdataTableProcess().QueryToTable(db, query, sqlParameters);
                    foreach (DataRow row in tab.Rows)
                    {
                        int id = Singleton.getSingleton(GlobalEnums.ObjectEnums.OLD_OBJECT).getInteger(row["ID"] + "");
                        String ack = row["ACK"] + "";
                        raporViewModel.menuler.Add(new SelectListItem() { Value = id + "", Text = ack });
                    }
                }

                raporViewModel.raporTipleri = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getRaporTipleri();
                return View(raporViewModel);
            }
            catch (Exception ee)
            {
                return Redirect("/adminhelper/PageException");
            }
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.RAPOR_PARTIAL, openingType = GlobalEnums.PageOpeningType.DIRECT)]
        [HttpPost]
        public ActionResult getRaporPartial([DefaultValue(0)] int menuId, [DefaultValue("")] string raporTipi)
        {
            DetayliRaporTipleri tip = (DetayliRaporTipleri)Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getInteger(raporTipi);
            RaporDetayViewModel raporDetayViewModel = new RaporDetayViewModel();
            raporDetayViewModel.raporTipi = tip;
            raporDetayViewModel.menuId = menuId;
            return PartialView(raporDetayViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.RAPOR_PARTIAL_HAREKET_GRAFIK, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getHareketRaporuGrafikPartial(RaporDetayViewModel raporDetayViewModel)
        {
            HareketRaporuGrafikViewModel hareketRaporuGrafikViewModel = new HareketRaporuGrafikViewModel();
            hareketRaporuGrafikViewModel.state = false;
            using (var db = new DBservices.pixdinnEntities())
            {
                List<SqlParameter> sqlParametersSayi = new List<SqlParameter>();
                sqlParametersSayi.Add(new SqlParameter("@M" + 1, raporDetayViewModel.menuId));
                sqlParametersSayi.Add(new SqlParameter("@M" + 2, raporDetayViewModel.islem));
                sqlParametersSayi.Add(new SqlParameter("@M" + 3, raporDetayViewModel.islemSon));
                sqlParametersSayi.Add(new SqlParameter("@M" + 4, raporDetayViewModel.baslangicTarihi + " " + raporDetayViewModel.baslangicSaati + ":00"));
                sqlParametersSayi.Add(new SqlParameter("@M" + 5, raporDetayViewModel.bitisTarihi + " " + raporDetayViewModel.bitisSaati + ":59"));

                List<HareketRaporuGrafikViewModelItem> listData = new List<HareketRaporuGrafikViewModelItem>();
                String sayiQuery = "";

                if (raporDetayViewModel.altTip == "SAYI")
                {
                    sayiQuery = @"
                     select DATA_NAME, MOVING_ID, MOVING_NAME, COUNT(MOVING_ID) as SAYI, RN
                     from (
                        select ID, DATEDATA, USER_ID, MOVING_NAME, MOVING_ID, MENU_ID, DATA_NAME,
                        rank() over(partition by USER_ID,DATEDATA order by ID) as RN
                        from (
                        (SELECT
                        CONCAT(YEAR(UM.MOVING_DATE),'-',MONTH(UM.MOVING_DATE),'-',DAY(UM.MOVING_DATE),' ', DATEPART(HOUR, UM.MOVING_DATE),':00') as DATEDATA,
                        UM.ID, UM.USER_ID, UM.MOVING_NAME, UM.MOVING_ID, UM.MENU_ID, UI.URUN_ISMI as DATA_NAME
                        FROM UserMoving UM
                        left join Urunler U on U.ID = UM.MOVING_ID
                        left join UrunInfo UI on UI.URUN_ID = U.ID
                        left join Languages L on UI.LANGUAGE_ID = L.ID
                        where UM.MOVING_ID != 0 and UM.MOVING_NAME ='URUN' AND L.ONCELIKLI = 1 AND UM.MENU_ID = @M1
                        and UM.MOVING_DATE >= CAST(@M4 as datetime) and UM.MOVING_DATE <= CAST(@M5 as datetime))
                        UNION
                        (SELECT
                        CONCAT(YEAR(UM.MOVING_DATE),'-',MONTH(UM.MOVING_DATE),'-',DAY(UM.MOVING_DATE),' ', DATEPART(HOUR, UM.MOVING_DATE),':00') as DATEDATA,
                        UM.ID, UM.USER_ID, UM.MOVING_NAME, UM.MOVING_ID, UM.MENU_ID, KI.KATEGORI_ADI as DATA_NAME
                        FROM UserMoving UM
                        left join Kategoriler K on K.ID = UM.MOVING_ID
                        left join KategoriInfo KI on KI.KATEGORI_ID = K.ID
                        left join Languages L on KI.LANGUAGE_ID = L.ID
                        where UM.MOVING_ID != 0 and UM.MOVING_NAME ='KATEGORI' AND L.ONCELIKLI = 1 AND UM.MENU_ID = @M1
                        and UM.MOVING_DATE >= CAST(@M4 as datetime) and UM.MOVING_DATE <= CAST(@M5 as datetime))
                        ) T
                     ) T2 where T2.RN >= @M2 AND T2.RN <= @M3
                     group by DATA_NAME, MOVING_ID, MOVING_NAME, RN
                     order by RN";

                    var kullaniciIlkIslemTable = Singleton.getSingleton(ObjectEnums.NEW_OBJECT).QueryToTable(db, sayiQuery, sqlParametersSayi);
                    foreach (DataRow item in kullaniciIlkIslemTable.Rows)
                    {
                        listData.Add(new HareketRaporuGrafikViewModelItem()
                        {
                            dataName = (String)item["DATA_NAME"],
                            movingId = Singleton.getSingleton(ObjectEnums.NEW_OBJECT).getInteger(item["MOVING_ID"] + ""),
                            movingName = (String)item["MOVING_NAME"],
                            sayi = Singleton.getSingleton(ObjectEnums.NEW_OBJECT).getInteger(item["SAYI"] + ""),
                            rn = Singleton.getSingleton(ObjectEnums.NEW_OBJECT).getInteger(item["RN"] + ""),
                        });
                    }
                }

                if (listData.Count > 0) hareketRaporuGrafikViewModel.state = true;
                hareketRaporuGrafikViewModel.grafikItemsJsonArray = Singleton.getSingleton(ObjectEnums.OLD_OBJECT).getMainSerialize().Serialize(listData);
                hareketRaporuGrafikViewModel.request = raporDetayViewModel;
            }
            return PartialView(hareketRaporuGrafikViewModel);
        }

        [AdminLoginFilterAttribute(pageName = GlobalEnums.PageNameAdmin.RAPOR_PARTIAL_HAREKET_LISTE, openingType = GlobalEnums.PageOpeningType.AJAX)]
        [HttpPost]
        public ActionResult getHareketRaporuListePartial(RaporDetayViewModel raporDetayViewModel)
        {
            HareketRaporuListeViewModel hareketRaporuListeViewModel = new HareketRaporuListeViewModel();
            hareketRaporuListeViewModel.request = new RaporDetayViewModel();
            hareketRaporuListeViewModel.request.menuId = raporDetayViewModel.menuId;
            hareketRaporuListeViewModel.request.altTip = raporDetayViewModel.altTip;
            hareketRaporuListeViewModel.extraQuery = raporDetayViewModel.baslangicTarihi + "/" + raporDetayViewModel.bitisTarihi + "/" + raporDetayViewModel.baslangicSaati + "/" + raporDetayViewModel.bitisSaati + "/" + raporDetayViewModel.menuId;
            return PartialView(hareketRaporuListeViewModel);
        }
    }
}
