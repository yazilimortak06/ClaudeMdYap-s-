// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\UserManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.PanelUserDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.UserManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class UserManagmentController : ControllerBase
    {
        private readonly IUserManagmentService _userManagmentService;

        public UserManagmentController(IUserManagmentService userManagmentService)
        {
            _userManagmentService = userManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelUserListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserDataTablePanel")]
        public async Task<IActionResult> GetUserDataTablePanel(DataTableFilterModel<GetPanelUserDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _userManagmentService.GetUserDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelUserResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUser")]
        public async Task<IActionResult> GetUser(GetPanelUserRequestDto getPanelUserRequest)
        {
            var result = await _userManagmentService.GetUser(getPanelUserRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUsersForSelectList")]
        public async Task<IActionResult> GetUsersForSelectList(GetUserForSelectListRequestDto getUserForSelectListRequest)
        {
            var result = await _userManagmentService.GetUsersForSelectList(getUserForSelectListRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelChangeUserStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeUserState")]
        public async Task<IActionResult> ChangeUserState(PanelChangeUserStateRequestDto panelChangeUserStateRequest)
        {
            var result = await _userManagmentService.ChangeUserState(panelChangeUserStateRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelRemoveUserResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveUser")]
        public async Task<IActionResult> RemoveUser(PanelRemoveUserRequestDto panelRemoveUserRequest)
        {
            var result = await _userManagmentService.RemoveUser(panelRemoveUserRequest);
            return this.FromResult(result);
        }
    }
}
