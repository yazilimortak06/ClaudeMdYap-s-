using FrameworkCore.Bases.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.BankEntities.WalletModule
{
    [Table("Wallet", Schema = "RotaWatt")]
    public class Wallet : BaseEntity
    {
        public string GuiId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal WalletAmount { get; set; }
        public string WalletTockenGuiId { get; set; } // Guiıd + MobilUserGuiId + TockenKey(appsettings de bulunan) => base64
        public string AmountTockenGuiId { get; set; } // guiId - tutar - walletTockenGuiId
        public string ProcessKey { get; set; } // her cüzdan işleminde güncellenecek olan key
        public virtual ICollection<WalletPushMoney> WalletPushMoney { get; set; }
        public virtual ICollection<WalletReductionMoney> WalletReductionMoney { get; set; }
    }
}
