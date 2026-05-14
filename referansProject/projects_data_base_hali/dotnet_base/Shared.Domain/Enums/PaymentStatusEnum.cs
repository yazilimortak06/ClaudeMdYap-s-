using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Enums.ApiEnums
{
    public enum PaymentStatusEnum
    {
        [Description("Beklemede")]
        WAITING = 1,
        [Description("Başarılı")]
        SUCCESSFUL = 2,
        [Description("Başarısız")]
        FAILURE = 3,
        [Description("Provizyonda")]
        IN_PROVISION = 4,
        [Description("İade")]
        REFUNDED = 5,
        [Description("İptal")]
        CANCELED = 6,
    }
    public enum PaymentMethodEnum
    {
        [Description("Kredi Kartı")]
        DEBIT_CARD = 1,
        [Description("Cüzdan")]
        WALLET = 2
    }
}
