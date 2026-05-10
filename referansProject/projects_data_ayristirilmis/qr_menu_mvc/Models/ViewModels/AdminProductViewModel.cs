// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Models\AdminModels\AdminViewModels\AdminProductViewModel.cs
using PixDinn.Models.AdminModels.AdminGlobalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixDinn.Models.AdminModels.AdminViewModels
{
    public class AdminProductViewModel : AdminPageGlobalModel
    {
        public int id { get; set; }
        public int urunResimId { get; set; }
        public String urunResimUrl { get; set; }
        public int urunResimBuyukId { get; set; }
        public int pismeSuresi { get; set; }
        public String pismeSuresiTipi { get; set; }
        public float kalori { get; set; }
        public string mekanId { get; set; }
        public List<SelectListItem> mekanlar { get; set; }
        public string urunKodu { get; set; }
        public string pluNo { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
        public int createdAdminId { get; set; }
        public int createdAdminType { get; set; }
        public string state { get; set; }
        public List<SelectListItem> urunState { get; set; }
        public List<SelectListItem> languages { get; set; }
        public List<UrunInfo> urunInfo { get; set; }
        public String infoJsonValue { get; set; }
        public List<UrunIcon> urunIcons { get; set; }
        public string urunIconJson { get; set; }
        public bool isYetkiliForPrice { get; set; }
        public bool metinselFiyatDurumu { get; set; }
        public bool fiyatiTumDillerdeKullan { get; set; }
        public Dictionary<string, AdminLoginSessionResponse.YetkiKontrolData> yetkiler { get; set; }

        public class UrunInfo
        {
            public int id { get; set; }
            public int urunId { get; set; }
            public string urunName { get; set; }
            public float fiyat { get; set; }
            public string fiyatBirim { get; set; }
            public string urunAciklama { get; set; }
            public string adetTipi { get; set; }
            public string languageId { get; set; }
            public string state { get; set; }
            public string metinselFiyat { get; set; }
            public int resimId { get; set; }
            public string resimUrl { get; set; }
        }

        public class UrunIcon
        {
            public string name { get; set; }
            public string url { get; set; }
            public bool state { get; set; }
        }
    }
}
