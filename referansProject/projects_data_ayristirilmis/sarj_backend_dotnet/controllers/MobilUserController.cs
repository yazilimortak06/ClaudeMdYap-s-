// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Tocken.Api\Controllers\MobilUserController.cs
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.CityDtos;
using Shared.Domain.Dto.TockenDto.MobilUserDtos;
using Shared.Domain.ServiceIntefaces.TockenServiceInterfaces.MobilUserServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Tocken.Application.Filters;
using Shared.Domain.Dto.TockenDto.ChangePasswordDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginFormDtos;
using Shared.Domain.Dto.TockenDto.MobilUserLoginSessionDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestDtos;
using Shared.Domain.Dto.TockenDto.MobilGuestLoginSessionDtos;

namespace Tocken.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(TokenRequestResponseInfoFilterAttribute))]
    public class MobilUserController : ControllerBase
    {
        private readonly IMobilUserService _mobilUserService;
        private readonly IMobilUserLoginService _mobilUserLoginService;

        public MobilUserController(
            IMobilUserService mobilUserService,
            IMobilUserLoginService mobilUserLoginService
            )
        {
            _mobilUserService = mobilUserService;
            _mobilUserLoginService = mobilUserLoginService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserInsertResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddMobilUser")]
        public async Task<IActionResult> AddMobilUser(MobilUserInsertRequestDto mobilUserInsertRequest)
        {
            var result = await _mobilUserService.AddMobilUser(mobilUserInsertRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<CheckMobilUserResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CheckMobilUser")]
        public async Task<IActionResult> CheckMobilUser(CheckMobilUserRequestDto checkMobilUserRequest)
        {
            var result = await _mobilUserService.CheckMobilUser(checkMobilUserRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserChangePasswordVerifyInsertResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangePasswordVerifyInsert")]
        public async Task<IActionResult> ChangePasswordVerifyInsert(MobilUserChangePasswordVerifyInsertRequestDto mobilUserChangePasswordInsertRequest)
        {
            var result = await _mobilUserService.ChangePasswordVerifyInsert(mobilUserChangePasswordInsertRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserChangePasswordResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangePassword")]
        public async Task<IActionResult> ChangePassword(MobilUserChangePasswordRequestDto mobilUserChangePasswordRequest)
        {
            var result = await _mobilUserService.ChangePasswordMobilUser(mobilUserChangePasswordRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserLoginFormPrepareResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilUserLoginFormPrepare")]
        public async Task<IActionResult> MobilUserLoginFormPrepare(MobilUserLoginFormPrepareRequestDto mobilUserLoginFormPrepareRequest)
        {
            var result = await _mobilUserLoginService.MobilUserLoginFormPrepare(mobilUserLoginFormPrepareRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginCheckMobilUserResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilUserLoginCheck")]
        public async Task<IActionResult> MobilUserLoginCheck(LoginCheckMobilUserRequestDto loginCheckMobilUserRequest)
        {
            var result = await _mobilUserLoginService.MobilUserLoginCheck(loginCheckMobilUserRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserLoginSessionCheckResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilUserLoginSessionCheck")]
        public async Task<IActionResult> MobilUserLoginSessionCheck(MobilUserLoginSessionCheckRequestDto mobilUserLoginSessionCheckRequest)
        {
            var result = await _mobilUserLoginService.MobilUserLoginSessionCheck(mobilUserLoginSessionCheckRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserLoginSessionUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilUserLoginSessionUpdate")]
        public async Task<IActionResult> MobilUserLoginSessionUpdate(MobilUserLoginSessionUpdateRequestDto mobilUserLoginSessionUpdateRequest)
        {
            var result = await _mobilUserLoginService.MobilUserLoginSessionUpdate(mobilUserLoginSessionUpdateRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilLogOutResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilUserLogOut")]
        public async Task<IActionResult> MobilUserLogOut(MobilUserLogOutRequestDto mobilUserLogOutRequest)
        {
            var result = await _mobilUserLoginService.MobilUserLogOut(mobilUserLogOutRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGuestLoginProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilGuestLoginProcess")]
        public async Task<IActionResult> MobilGuestLoginProcess(MobilGuestLoginProcessRequestDto mobilGuestLoginProcessRequest)
        {
            var result = await _mobilUserLoginService.MobilGuestLoginProcess(mobilGuestLoginProcessRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGuestLoginSessionCheckResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MobilGuestLoginSessionCheck")]
        public async Task<IActionResult> MobilGuestLoginSessionCheck(MobilGuestLoginSessionCheckRequestDto mobilGuestLoginSessionCheckRequest)
        {
            var result = await _mobilUserLoginService.MobilGuestLoginSessionCheck(mobilGuestLoginSessionCheckRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
