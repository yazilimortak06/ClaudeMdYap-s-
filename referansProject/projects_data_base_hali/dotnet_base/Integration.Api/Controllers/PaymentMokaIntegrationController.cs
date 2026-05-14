using Integration.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.ComponentModel;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.PaymentIntegrationServiceInterfaces;
using Shared.Domain.Dto.IntegrationDto.PaymentIntegrationDtos;
using Shared.Domain.Enums.IntegrationEnums;
using Shared.Domain.Entities.BankEntities.PaymentModule;
using Newtonsoft.Json;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PaymentMokaIntegrationController : ControllerBase
    {
        private readonly IPaymentIntegrationService _paymentIntegrationService;
        private readonly ILogger<PaymentMokaIntegrationController> _logger;

        public PaymentMokaIntegrationController(
            ILogger<PaymentMokaIntegrationController> logger,
            IPaymentIntegrationService paymentIntegrationService)
        {
            _logger = logger;
            _paymentIntegrationService = paymentIntegrationService;
        }
        [Route("v{version:apiVersion}/[controller]/[action]/{paymentCallbackDataId}/{paymentDataId}/{PaymentIntegrationProcessGuiId}")]
        [HttpPost]
        public async Task<IActionResult> PaymentCompleteFromMoka([DefaultValue(0)] long paymentCallbackDataId,
                                                 [DefaultValue(0)] long paymentDataId,
                                                 [DefaultValue("")] string paymentIntegrationProcessGuiId,
                                               [DefaultValue("")][FromForm] string hashValue,
                                               [DefaultValue("")][FromForm] string resultCode,
                                               [DefaultValue("")][FromForm] string resultMessage,
                                               [DefaultValue("")][FromForm] string trxCode,
                                               [DefaultValue("")][FromForm] string OtherTrxCode)
        {
            _logger.LogError("ResultCode: " + resultCode);
            _logger.LogError("ResultMessage: " + resultMessage);
            _logger.LogError("hashValue: " + hashValue);
            _logger.LogError("trxCode: " + trxCode);
            PaymentComplete3DIntegrationRequestDto paymentComplete3DIntegrationRequest = new PaymentComplete3DIntegrationRequestDto();
            paymentComplete3DIntegrationRequest.PaymentIntegrationBankType = PaymentIntegrationBankTypeEnum.MOKA;
            paymentComplete3DIntegrationRequest.PaymentId = paymentDataId;
            paymentComplete3DIntegrationRequest.PaymentCallbackDataId = paymentCallbackDataId;
            paymentComplete3DIntegrationRequest.PaymentIntegrationProcessGuiId = paymentIntegrationProcessGuiId;
            paymentComplete3DIntegrationRequest.PaymentComplete3DRequest = new
            {
                PaymentId = paymentDataId,
                HashValue = hashValue,
                ResultCode = resultCode,
                ResultMessage = resultMessage,
                TrxCode = trxCode,
                OtherTrxCode = OtherTrxCode,
            };
            _logger.LogError("request");
            _logger.LogError(JsonConvert.SerializeObject(paymentComplete3DIntegrationRequest));
            var result = await _paymentIntegrationService.PaymentComplete3DIntegration(paymentComplete3DIntegrationRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
