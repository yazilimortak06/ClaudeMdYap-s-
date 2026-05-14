// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\GeneralEnums\ApiName.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.GeneralEnums
{
    /// <summary>
    /// Sistemdeki tüm mikro-servislerin (API'ların) enum karşılığı.
    /// InnerRequestAttribute'da hangi API'dan istek kabul edileceğini belirtmek için kullanılır.
    /// Her yeni servis eklendiğinde buraya eklenmeli.
    /// </summary>
    public enum ApiName
    {
        BANK = 1,
        FILE = 2,
        GOOGLE_SERVICE = 3,
        LOG = 4,
        MAIL_SMS = 5,
        MOBIL = 6,
        NOTIFICATION = 7,
        STATION = 8,
        TOKEN = 9,
        WEB = 10,
        WORKER_SERVICE = 11,
        INTEGRATION = 12,
        OCPP = 13,
        VM = 14,
    }
}
