using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using MailSms.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.MailSmsDto.RegisterSmsDtos;
using Shared.Domain.ServiceInterfaces.MailSmsApiServiceInterfaces.RegisterSmsServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MailSms.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MailSmsApiRequesResponseLogFilterAttribute))]
    public class RegisterSmsController : ControllerBase
    {
        private readonly IRegisterSmsService _registerSmsService;
        private readonly IConfiguration _configuration;

        public RegisterSmsController(IConfiguration configuration,
            IRegisterSmsService registerSmsService)
        {
            _configuration = configuration;
            _registerSmsService = registerSmsService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RegisterSmsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SendSmsForRegister")]
        [ValidateFilter]
        public async Task<IActionResult> SendSmsForRegister(RegisterSmsRequestDto registerSmsRequest)
        {
            var result = await _registerSmsService.SendSmsForRegister(registerSmsRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RegisterSmsVerifyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "VerifyRegisterSms")]
        [ValidateFilter]
        public async Task<IActionResult> VerifyRegisterSms(RegisterSmsVerifyRequestDto registerSmsVerifyRequest)
        {
            var result = await _registerSmsService.VerifyRegisterSms(registerSmsVerifyRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [SwaggerOperation(OperationId = "Test")]
        [ValidateFilter]
        public IActionResult test()
        {
            _registerSmsService.Test();
            return Ok();
        }
    }
}
