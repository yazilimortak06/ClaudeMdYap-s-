// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\Urunler.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class Urunler
    {
        public int ID { get; set; }
        public Nullable<int> URUN_RESIM_ID { get; set; }
        public Nullable<int> URUN_RESIM_BUYUK_ID { get; set; }
        public Nullable<int> PISME_SURESI { get; set; }
        public Nullable<double> KALORI { get; set; }
        public Nullable<int> MEKAN_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public Nullable<int> CREATED_ADMIN_ID { get; set; }
        public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
        public Nullable<int> STATE { get; set; }
        public string sureTipi { get; set; }
        public string URUN_KODU { get; set; }
        public string PLU_NO { get; set; }
    }
}
