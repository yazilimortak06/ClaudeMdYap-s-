// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\PixDinn\Models\AdminModels\AdminViewModels\AdminKategoriViewModel.cs
using PixDinn.Models.AdminModels.AdminGlobalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixDinn.Models.AdminModels.AdminViewModels
{
    public class AdminKategoriViewModel : AdminPageGlobalModel
    {
        public int id { get; set; }
        public int ustId { get; set; }
        public string mekanId { get; set; }
        public List<SelectListItem> mekanlar { get; set; }
        public int kategoriResmi { get; set; }
        public String kategoriResmiUrl { get; set; }
        public int kategoriLevel { get; set; }
        public string ustKategoriId { get; set; }
        public List<SelectListItem> UstKategoriGrup { get; set; }
        public List<SelectListItem> languages { get; set; }
        public string state { get; set; }
        public List<SelectListItem> kategoriState { get; set; }
        public List<KategoriInfo> kategoriInfo { get; set; }
        public List<AdminKategoriViewModel> childKategoriler { get; set; }
        public String infoJsonValue { get; set; }
        public string kategoriUrl { get; set; }

        public class KategoriInfo
        {
            public int id { get; set; }
            public int kategoriId { get; set; }
            public string kategoriAdi { get; set; }
            public string languageId { get; set; }
            public String title { get; set; }
            public int resimId { get; set; }
            public int subResimId { get; set; }
            public string resimUrl { get; set; }
            public string subResimInfoUrl { get; set; }
        }
    }
}
