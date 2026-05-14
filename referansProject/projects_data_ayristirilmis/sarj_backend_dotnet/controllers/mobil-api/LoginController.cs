// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\LoginController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilLoginDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginSessionDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.MobilLoginServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class LoginController : ControllerBase
    {
        private readonly IMobilLoginService _mobilLoginService;

        public LoginController(
            IMobilLoginService mobilLoginService
            )
        {
            _mobilLoginService = mobilLoginService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilLoginFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LoginForm")]
        [ValidateFilter]
        public async Task<IActionResult> LoginForm(MobilLoginFormRequestDto mobilLoginFormRequest)
        {
            var result = await _mobilLoginService.LoginForm(mobilLoginFormRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilLoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Login")]
        [ValidateFilter]
        public async Task<IActionResult> Login(MobilLoginRequestDto mobilLoginRequest)
        {
            var result = await _mobilLoginService.LoginCheck(mobilLoginRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilLogOutResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LogOut")]
        [ValidateFilter]
        public async Task<IActionResult> LogOut()
        {
            var result = await _mobilLoginService.LogOut();
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGuestLoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GuestLogin")]
        [ValidateFilter]
        public async Task<IActionResult> GuestLogin(MobilGuestLoginRequestDto mobilGuestLoginRequest)
        {
            var result = await _mobilLoginService.GuestLogin(mobilGuestLoginRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilCheckAuthorizeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CheckAuthorize")]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public IActionResult CheckAuthorize()
        {
            return this.FromMobilResult(new SuccessResult<MobilCheckAuthorizeResponseDto>(new MobilCheckAuthorizeResponseDto { IsAuthorized = true }));
        }
    }
}
