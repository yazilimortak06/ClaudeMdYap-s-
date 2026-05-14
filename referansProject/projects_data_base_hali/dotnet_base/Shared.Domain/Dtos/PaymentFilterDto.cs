using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.BankDto.PaymentDtos
{
    public class PaymentFilterDto
    {
#nullable enable
        public long? Id { get; set; }
        public string? PaymentGuiId { get; set; }
        public string? SecurityKey { get; set; }
        public PaymentStatusEnum? PaymentStatus { get; set; }
        public PaymentMethodEnum? PaymentMethod { get; set; }
        public PaymentReasonEnum? PaymentReason { get; set; }
        public List<PaymentStatusEnum>? PaymentStatusList { get; set; }
        public string? ChargeGuiId { get; set; }
        public string? WalletProcessGuiId { get; set; }
#nullable disable
    }
}
