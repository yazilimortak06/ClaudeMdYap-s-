using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.BankEntities.PaymentDebitCardVerificationModule;
using Shared.Domain.Entities.BankEntities.WalletModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.IntegrationEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.BankEntities.PaymentModule
{
    [Table("Payment", Schema = "RotaWatt")]
    public class Payment : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string GuiId { get; set; }
        public string FirmGuiId { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal Discount { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public PaymentReasonEnum PaymentReason { get; set; }
        public string SecurityKey { get; set; }
#nullable enable
        public DateTime? CompletedDate { get; set; }
        public DateTime? RefundedDate { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string? ProcessingUserAdressGuiId { get; set; }
        public string? UserAdressGuiId { get; set; }
        public string? ChargeGuiId { get; set; } // şarj işlem verisi guiId
        public string? WalletProcessGuiId { get; set; } // cüzdana işlemi guiId
        public decimal? RefundedPrice { get; set; }
        public double? Kdv { get; set; }
        public string? DebitCardGuiId { get; set; }
        public string? UserGuiId { get; set; }
        public long? ChargeId { get; set; }
        public long? ProcessingUserAdressId { get; set; }
        public long? UserAdressId { get; set; }
        public long? DebitCardId { get; set; }
        public long? UserId { get; set; }
        public string? PaymentChargeInfoJsonBase64 { get; set; }
        public string? UserAdressJsonBase64 { get; set; }
        public PaymentCallbackData? PaymentCallbackData { get; set; }
        public PaymentDebitCardVerification? PaymentDebitCardVerification { get; set; }
        public WalletSpendMoney? WalletSpendMoney { get; set; }
        public WalletPushMoney? WalletPushMoney { get; set; }
        //public Invoice? Invoice { get; set; }
        //public PaymentMoka? PaymentMoka { get; set; }
        //public WalletPushMoney? WalletPushMoney { get; set; } // cüzdana para aktarımı olursa oluşacak
        //public WalletSpendMoney? WalletSpendMoney { get; set; } // cüzdan ile harcama yapılırsa oluşacak
#nullable disable
    }
}
