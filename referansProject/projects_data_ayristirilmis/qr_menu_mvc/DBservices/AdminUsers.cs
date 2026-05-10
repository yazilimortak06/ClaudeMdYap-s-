// KAYNAK: E:\Projeler\Backend\QrMenu\pixdinn2\PixdinnQrMenu\DBservices\AdminUsers.cs
// Auto-generated EF entity

namespace DBservices
{
    using System;
    using System.Collections.Generic;

    public partial class AdminUsers
    {
        public int ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<int> CREATED_USER_ID { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string MAIL { get; set; }
        public string PHONE_CODE { get; set; }
        public string PHONE { get; set; }
        public Nullable<int> STATE { get; set; }
        public Nullable<int> AUTH_ID { get; set; }
        public Nullable<int> ADMIN_TYPE { get; set; }
    }
}
