using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.DebitCardModule;
using Shared.Domain.Entities.ApiEntities.GuestUserModule;
using Shared.Domain.Entities.ApiEntities.UserAdressModule;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.Entities.ApiEntities.WalletInfoModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.IntegrationEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.PaymentInfoModule
{
    [Table("PaymentInfo", Schema = "RotaWatt")]
    public class PaymentInfo : BaseEntity
    {
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public string PaymentGuiId { get; set; }
        public decimal Discount { get; set; }
        public PaymentReasonEnum PaymentReason { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public PaymentIntegrationBankTypeEnum PaymentBankType { get; set; }
        public DateTime CompletedDate { get; set; }
#nullable enable
        public double? Kdv { get; set; }
        public DateTime? RefundDate { get; set; }
        public DateTime? CanceledDate { get; set; }
        public decimal? RefundedPrice { get; set; }
        public PaymentStatusEnum? PaymentStatus { get; set; }

        [ForeignKey("PaymentInfo_User")]
        public long? UserId { get; set; }
        public User? User { get; set; }
        [ForeignKey("PaymentInfo_UserAdress")]
        public long? UserAdressId { get; set; }
        public UserAdress? UserAdress { get; set; }
  
        [ForeignKey("PaymentInfo_Charge")]
        public long? ChargeId { get; set; }
        public Charge? Charge { get; set; }
        [ForeignKey("PaymentInfo_ProcessingUserAdress")]
        public long? ProcessingUserAdressId { get; set; }
        public ProcessingUserAdress? ProcessingUserAdress { get; set; } // işlem için o anki fatura adresinin kopyasını tutar
        [ForeignKey("PaymentInfo_DebitCard")]
        public long? DebitCardId { get; set; }
        public DebitCard? DebitCard { get; set; }
        public WalletPushMoneyInfo? WalletPushMoneyInfo { get; set; } // cüzdana para aktarımı olursa oluşacak
        public WalletSpendMoneyInfo? WalletSpendMoneyInfo { get; set; } // cüzdan ile harcama yapılırsa oluşacak
        //public UserPaymentRefund? UserPaymentRefund { get; set; } // ödeme iadesinde oluşturulacak

#nullable disable
        //public virtual ICollection<ChargeDeviceReservation> ChargeDeviceReservation { get; set; }

    }
}
