// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\Mekanlar.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class Mekanlar
    {
        public int ID { get; set; }
        public string TEL { get; set; }
        public string CEP { get; set; }
        public string INSTAGRAM { get; set; }
        public string FACE { get; set; }
        public string MAIL { get; set; }
        public Nullable<int> LOGO_ID { get; set; }
        public Nullable<int> MEKAN_RESIM { get; set; }
        public Nullable<int> GROUP_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public Nullable<int> CREATED_ADMIN_ID { get; set; }
        public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
        public Nullable<int> STATE { get; set; }
        public Nullable<int> SMS_LIMIT { get; set; }
        public Nullable<int> SMS_ONAY { get; set; }
        public Nullable<bool> KULLANICI_YETKI_DURUMU { get; set; }
        public int SIPARIS_VERME_TIPI { get; set; }
        public Nullable<bool> HOS_GELDIN_POPUP_GOSTER { get; set; }
        public Nullable<bool> PRODUCT_PRICE_IS_STRING { get; set; }
        public Nullable<bool> ZAMAN_ARALIGI_KULLAN { get; set; }
        public string Spotify { get; set; }
        public string WHATSAPP_CEP { get; set; }
        public Nullable<bool> SHOWING_PRODUCT_DETAIL { get; set; }
        public Nullable<int> PRODUCT_DETAIL_MAXIMUM_CHARACTER_COUNT { get; set; }
        public Nullable<int> MENU_BOTTOM_LEFT_BUTTON_TYPE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGTITUDE { get; set; }
    }
}
