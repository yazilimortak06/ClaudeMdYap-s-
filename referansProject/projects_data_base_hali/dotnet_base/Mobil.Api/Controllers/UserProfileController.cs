// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\UserProfileController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.UserProfileServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilUserProfileDtos;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserGeneralInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserGeneralInfo")]
        [ValidateFilter]
        public async Task<IActionResult> GetUserGeneralInfo(GetUserGeneralInfoRequestDto getUserGeneralInfoRequest)
        {
            var result = await _userProfileService.GetUserGeneralInfo(getUserGeneralInfoRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveUserReceiveSmsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveUserReceiveSms")]
        [ValidateFilter]
        public async Task<IActionResult> RemoveUserReceiveSms(RemoveUserReceiveSmsRequestDto removeUserReceiveSmsRequest)
        {
            var result = await _userProfileService.RemoveUserReceiveSms(removeUserReceiveSmsRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveUserResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveUser")]
        [ValidateFilter]
        public async Task<IActionResult> RemoveUser(RemoveUserRequestDto removeUserRequest)
        {
            var result = await _userProfileService.RemoveUser(removeUserRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserProcessListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserProcessList")]
        [ValidateFilter]
        public async Task<IActionResult> GetUserProcessList(GetUserProcessListRequestDto getUserProcessListRequest)
        {
            var result = await _userProfileService.GetUserProcessList(getUserProcessListRequest);
            return this.FromMobilResult(result);
        }
    }
}
