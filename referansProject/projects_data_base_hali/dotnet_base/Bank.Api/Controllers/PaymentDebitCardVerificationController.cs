// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Bank.Api\Controllers\PaymentDebitCardVerificationController.cs
using Bank.Application.Filters;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.BankDto.PaymentDebitCardVerificationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.BankServiceInterfaces.PaymentDebitCardVerificationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Bank.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Produces("application/json")]
    public class PaymentDebitCardVerificationController : ControllerBase
    {
        private readonly IPaymentDebitCardVerificationService _paymentDebitCardVerificationService;
        public PaymentDebitCardVerificationController(IPaymentDebitCardVerificationService paymentDebitCardVerificationService)
        {
            _paymentDebitCardVerificationService = paymentDebitCardVerificationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentDebitCardVerificationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentDebitCardVerification")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> GetPaymentDebitCardVerification(GetPaymentDebitCardVerificationRequestDto getPaymentDebitCardVerificationRequest)
        {
            var result = await _paymentDebitCardVerificationService.GetPaymentDebitCardVerification(getPaymentDebitCardVerificationRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RefundPaymentDebitCardVerificationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RefundPaymentDebitCardVerification")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE })]
        public async Task<IActionResult> RefundPaymentDebitCardVerification(RefundPaymentDebitCardVerificationRequestDto refundPaymentDebitCardVerificationRequest)
        {
            var result = await _paymentDebitCardVerificationService.RefundPaymentDebitCardVerification(refundPaymentDebitCardVerificationRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
