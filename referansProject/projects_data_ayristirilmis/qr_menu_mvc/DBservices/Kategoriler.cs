// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\Kategoriler.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class Kategoriler
    {
        public int ID { get; set; }
        public Nullable<int> MEKAN_ID { get; set; }
        public Nullable<int> KATEGORI_RESMI { get; set; }
        public Nullable<int> KATEGORI_LEVEL { get; set; }
        public Nullable<int> UST_KATEGORI_ID { get; set; }
        public Nullable<int> LANGUAGE_ID { get; set; }
        public Nullable<int> STATE { get; set; }
        public string KATEGORI_URL { get; set; }
    }
}
