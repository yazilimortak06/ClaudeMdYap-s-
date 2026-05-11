// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\UserAddressController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilUserAddressDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.UserAddressProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressProcessService _userAddressProcessService;

        public UserAddressController(
            IUserAddressProcessService userAddressProcessService
            )
        {
            _userAddressProcessService = userAddressProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilUserAddressListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserAddressList")]
        [ValidateFilter]
        public async Task<IActionResult> GetUserAddressList(GetMobilUserAddressListRequestDto getMobilUserAddressListRequest)
        {
            var result = await _userAddressProcessService.GetUserAddressList(getMobilUserAddressListRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateMobilUserAddressResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateUserAddress")]
        [ValidateFilter]
        public async Task<IActionResult> UpdateUserAddress(UpdateMobilUserAddressRequestDto updateMobilUserAddressRequest)
        {
            var result = await _userAddressProcessService.UpdateUserAddress(updateMobilUserAddressRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserAddressPrepareInsertFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UserAddressPrepareInsertForm")]
        [ValidateFilter]
        public async Task<IActionResult> UserAddressPrepareInsertForm(MobilUserAddressPrepareInsertFormRequestDto userAddressMobilPrepareInsertFormRequest)
        {
            var result = await _userAddressProcessService.UserAddressPrepareInsertForm(userAddressMobilPrepareInsertFormRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUserAddressPrepareUpdateFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UserAddressPrepareUpdateForm")]
        [ValidateFilter]
        public async Task<IActionResult> UserAddressPrepareUpdateForm(MobilUserAddressPrepareUpdateFormRequestDto mobilUserAddressPrepareUpdateFormRequest)
        {
            var result = await _userAddressProcessService.UserAddressPrepareUpdateForm(mobilUserAddressPrepareUpdateFormRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddMobilUserAddressResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddUserAddress")]
        [ValidateFilter]
        public async Task<IActionResult> AddUserAddress(AddMobilUserAddressRequestDto addMobilUserAddressRequest)
        {
            var result = await _userAddressProcessService.AddUserAddress(addMobilUserAddressRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveMobilUserAddressResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveUserAddress")]
        [ValidateFilter]
        public async Task<IActionResult> RemoveUserAddress(RemoveMobilUserAddressRequestDto removeMobilUserAddressRequest)
        {
            var result = await _userAddressProcessService.RemoveUserAddress(removeMobilUserAddressRequest);
            return this.FromMobilResult(result);
        }
    }
}
