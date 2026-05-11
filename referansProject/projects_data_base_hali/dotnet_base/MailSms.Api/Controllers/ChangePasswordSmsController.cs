using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using MailSms.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.MailSmsDto.ChangePasswordSmsDtos;
using Shared.Domain.ServiceInterfaces.MailSmsApiServiceInterfaces.ChangePasswordSmsServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MailSms.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MailSmsApiRequesResponseLogFilterAttribute))]
    public class ChangePasswordSmsController : ControllerBase
    {
        private readonly IChangePasswordSmsService _changePasswordSmsService;
        private readonly IConfiguration _configuration;

        public ChangePasswordSmsController(IConfiguration configuration,
            IChangePasswordSmsService changePasswordSmsService)
        {
            _configuration = configuration;
            _changePasswordSmsService = changePasswordSmsService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangePasswordSmsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SendSmsForChangePassword")]
        [ValidateFilter]
        public async Task<IActionResult> SendSmsForChangePassword(ChangePasswordSmsRequestDto changePasswordSmsRequest)
        {
            var result = await _changePasswordSmsService.SendSmsForChangePassword(changePasswordSmsRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangePasswordSmsVerifyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "VerifyChangePasswordSms")]
        [ValidateFilter]
        public async Task<IActionResult> VerifyChangePasswordSms(ChangePasswordSmsVerifyRequestDto changePasswordSmsVerifyRequest)
        {
            var result = await _changePasswordSmsService.VerifyChangePasswordSms(changePasswordSmsVerifyRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
