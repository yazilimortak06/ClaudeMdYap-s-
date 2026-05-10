// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\UserForgetPasswordController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.UserForgetPasswordServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilUserForgetPasswordDtos;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class UserForgetPasswordController : ControllerBase
    {
        private readonly IUserForgetPasswordService _userForgetPasswordService;
        public UserForgetPasswordController(IUserForgetPasswordService userForgetPasswordService)
        {
            _userForgetPasswordService = userForgetPasswordService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserForgetPasswordSendSmsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UserForgetPasswordSendSms")]
        [ValidateFilter]
        public async Task<IActionResult> UserForgetPasswordSendSms(MobilUserForgetPasswordSendSmsRequestDto mobilUserForgetPasswordSendSmsRequest)
        {
            var result = await _userForgetPasswordService.UserForgetPasswordSendSms(mobilUserForgetPasswordSendSmsRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserForgetPasswordSmsVerifyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UserForgetPasswordSmsVerify")]
        [ValidateFilter]
        public async Task<IActionResult> UserForgetPasswordSmsVerify(MobilUserForgetPasswordSmsVerifyRequestDto mobilUserForgetPasswordSmsVerifyRequest)
        {
            var result = await _userForgetPasswordService.UserForgetPasswordSmsVerify(mobilUserForgetPasswordSmsVerifyRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserForgetPasswordCompleteResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UserForgetPasswordComplete")]
        [ValidateFilter]
        public async Task<IActionResult> UserForgetPasswordComplete(MobilUserForgetPasswordCompleteRequestDto mobilUserForgetPasswordCompleteRequest)
        {
            var result = await _userForgetPasswordService.UserForgetPasswordComplete(mobilUserForgetPasswordCompleteRequest);
            return this.FromMobilResult(result);
        }
    }
}
