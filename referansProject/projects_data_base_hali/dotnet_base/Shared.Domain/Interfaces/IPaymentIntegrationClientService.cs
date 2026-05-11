using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.IntegrationDto.PaymentIntegrationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.HttpClients.HttpClientInterfaces.IntegrationApiInterfaces
{
    public interface IPaymentIntegrationClientService
    {
        Task<Result<PaymentDirectIntegrationResponseDto>> PaymentDirectIntegration(PaymentDirectIntegrationRequestDto paymentDirectIntegrationRequest);
        Task<Result<PaymentStart3DIntegrationResponseDto>> PaymentStart3DIntegration(PaymentStart3DIntegrationRequestDto paymentStart3DIntegrationRequest);
        Task<Result<PaymentRefundIntegrationResponseDto>> PaymentRefundIntegration(PaymentRefundIntegrationRequestDto paymentRefundIntegrationRequest);
    }
}
