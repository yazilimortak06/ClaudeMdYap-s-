using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using MailSms.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.MailSmsDto.UserRemoveSmsDtos;
using Shared.Domain.ServiceInterfaces.MailSmsApiServiceInterfaces.UserRemoveSmsServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MailSms.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MailSmsApiRequesResponseLogFilterAttribute))]
    public class UserRemoveSmsController : ControllerBase
    {
        private readonly IUserRemoveSmsService _userRemoveSmsService;
        private readonly IConfiguration _configuration;

        public UserRemoveSmsController(IConfiguration configuration,
            IUserRemoveSmsService userRemoveSmsService)
        {
            _configuration = configuration;
            _userRemoveSmsService = userRemoveSmsService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UserRemoveSendSmsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SendSmsForUserRemove")]
        [ValidateFilter]
        public async Task<IActionResult> SendSmsForUserRemove(UserRemoveSendSmsRequestDto userRemoveSendSmsRequest)
        {
            var result = await _userRemoveSmsService.SendSmsForUserRemove(userRemoveSendSmsRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UserRemoveSmsVerifyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "VerifyUserRemoveSms")]
        [ValidateFilter]
        public async Task<IActionResult> VerifyUserRemoveSms(UserRemoveSmsVerifyRequestDto userRemoveSmsVerifyRequest)
        {
            var result = await _userRemoveSmsService.VerifyUserRemoveSms(userRemoveSmsVerifyRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
