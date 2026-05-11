using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.BankDto.PaymentDebitCardVerificationDtos;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.HttpClients.HttpClientInterfaces.BankApiInterfaces
{
    public interface IPaymentClientService
    {
        Task<Result<PaymentWalletResponseDto>> PaymentWallet(PaymentWalletRequestDto paymentWalletRequest);
        Task<Result<PaymentDirectDebitCardResponseDto>> PaymentDirectDebitCard(PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest);
        Task<Result<PaymentStartDebitCard3DResponseDto>> PaymentStartDebitCard3D(PaymentStartDebitCard3DRequestDto paymentStartDebitCard3DRequest);
        Task<Result<PaymentCompleteDebitCard3DResponseDto>> PaymentCompleteDebitCard3D(PaymentCompleteDebitCard3DRequestDto paymentCompleteDebitCard3DRequest);
        Task<Result<GetPaymentStatusResponseDto>> GetPaymentStatus(GetPaymentStatusRequestDto getPaymentStatusRequest);
        Task<Result<GetPaymentDetailForArchiveAndInvoiceResponseDto>> GetPaymentDetailForArchiveAndInvoice(GetPaymentDetailForArchiveAndInvoiceRequestDto getPaymentDetailForArchiveAndInvoiceRequest);
    }
}
