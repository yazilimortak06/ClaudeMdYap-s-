using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Integration.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.IntegrationDto.SmsIntegrationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.SmsIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class SmsIntegrationController : ControllerBase
    {
        private readonly ISmsIntegrationService _smsIntegrationService;
        private readonly ILogger<SmsIntegrationController> _logger;
        public SmsIntegrationController(
            ILogger<SmsIntegrationController> logger,
            ISmsIntegrationService smsIntegrationService)
        {
            _logger = logger;
            _smsIntegrationService = smsIntegrationService;
        }
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<SendSmsIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SendSmsIntegration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MAIL_SMS })]
        public async Task<IActionResult> SendSmsIntegration(SendSmsIntegrationRequestDto sendSmsIntegrationRequest)
        {
            var result = await _smsIntegrationService.SendSmsIntegration(sendSmsIntegrationRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
