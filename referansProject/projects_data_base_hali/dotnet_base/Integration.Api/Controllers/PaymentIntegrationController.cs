using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Integration.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.IntegrationDto.PaymentIntegrationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.PaymentIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class PaymentIntegrationController : ControllerBase
    {
        private readonly IPaymentIntegrationService _paymentIntegrationService;
        private readonly ILogger<PaymentIntegrationController> _logger;
        public PaymentIntegrationController(
            ILogger<PaymentIntegrationController> logger,
            IPaymentIntegrationService paymentIntegrationService)
        {
            _logger = logger;
            _paymentIntegrationService = paymentIntegrationService;
        }
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentDirectIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentDirectIntegration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> PaymentDirectIntegration(PaymentDirectIntegrationRequestDto paymentDirectIntegrationRequest)
        {
            var result = await _paymentIntegrationService.PaymentDirectIntegration(paymentDirectIntegrationRequest);
            return this.FromHttpClientResult(result);
        }
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentStart3DIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentStart3DIntegration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> PaymentStart3DIntegration(PaymentStart3DIntegrationRequestDto paymentStart3DIntegrationRequest)
        {
            var result = await _paymentIntegrationService.PaymentStart3DIntegration(paymentStart3DIntegrationRequest);
            return this.FromHttpClientResult(result);
        }
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentRefundIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentRefundIntegration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.BANK })]
        public async Task<IActionResult> PaymentRefundIntegration(PaymentRefundIntegrationRequestDto paymentRefundIntegrationRequest)
        {
            var result = await _paymentIntegrationService.PaymentRefundIntegration(paymentRefundIntegrationRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
