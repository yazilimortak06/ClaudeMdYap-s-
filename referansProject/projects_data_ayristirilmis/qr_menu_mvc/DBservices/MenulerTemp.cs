// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\MenulerTemp.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class MenulerTemp
    {
        public int ID { get; set; }
        public string ACK { get; set; }
        public string QR_CODE { get; set; }
        public string MENU_SESSION_CODE { get; set; }
        public Nullable<int> MEKAN_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public Nullable<int> CREATED_ADMIN_ID { get; set; }
        public Nullable<int> CREATED_ADMIN_TYPE { get; set; }
        public Nullable<int> STATE { get; set; }
        public Nullable<int> PUBLISH_TYPE { get; set; }
        public string QR_CODE_SIPARIS { get; set; }
        public string MENU_URL { get; set; }
        public Nullable<bool> MENU_SAYFA_YAZI_DURUMU { get; set; }
        public Nullable<bool> SHOWING_ENTRANCE_PAGE { get; set; }
        public Nullable<bool> SIPARIS_DURUMU { get; set; }
    }
}
