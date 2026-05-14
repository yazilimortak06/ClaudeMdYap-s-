// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\KategoriInfo.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class KategoriInfo
    {
        public int ID { get; set; }
        public Nullable<int> KATEGORI_ID { get; set; }
        public string KATEGORI_ADI { get; set; }
        public Nullable<int> LANGUAGE_ID { get; set; }
        public Nullable<int> STATE { get; set; }
        public string TITLE { get; set; }
        public Nullable<int> RESIM_ID { get; set; }
        public Nullable<int> SUB_RESIM_ID { get; set; }
    }
}
