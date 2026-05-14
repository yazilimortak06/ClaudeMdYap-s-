// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\RegisterController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.RegisterSmsDtos;
using Shared.Domain.Dto.ApiDto.UserRegisterDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.RegisterServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RegisterValidatePersonalInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RegisterValidatePersonalInfo")]
        [ValidateFilter]
        public async Task<IActionResult> RegisterValidatePersonalInfo(RegisterValidatePersonalInfoRequestDto userRegister)
        {
            var result = await _registerService.RegisterValidatePersonalInfo(userRegister);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilRegisterSmsVerifyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "VerifyRegisterSms")]
        [ValidateFilter]
        public async Task<IActionResult> VerifyRegisterSms(MobilRegisterSmsVerifyRequestDto mobilRegisterSmsVerifyRequest)
        {
            var result = await _registerService.VerifyRegisterSms(mobilRegisterSmsVerifyRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UserRegisterCompleteResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RegisterComplete")]
        [ValidateFilter]
        public async Task<IActionResult> RegisterComplete(UserRegisterCompleteRequestDto userRegister)
        {
            var result = await _registerService.RegisterComplete(userRegister);
            return this.FromMobilResult(result);
        }
    }
}
