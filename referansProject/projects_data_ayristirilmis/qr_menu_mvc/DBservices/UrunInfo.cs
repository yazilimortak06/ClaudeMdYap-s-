// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\UrunInfo.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class UrunInfo
    {
        public int ID { get; set; }
        public Nullable<int> URUN_ID { get; set; }
        public string URUN_ISMI { get; set; }
        public Nullable<double> FIYAT { get; set; }
        public string FIYAT_BIRIM { get; set; }
        public string URUN_ACIKLAMA { get; set; }
        public string ADET_TIPI { get; set; }
        public Nullable<int> LANGUAGE_ID { get; set; }
        public Nullable<int> STATE { get; set; }
        public string FIYAT_METIN { get; set; }
        public Nullable<int> RESIM_ID { get; set; }
    }
}
