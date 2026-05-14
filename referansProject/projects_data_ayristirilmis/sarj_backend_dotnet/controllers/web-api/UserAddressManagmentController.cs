using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.PanelUserAddressDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.UserAddressManagmentServiceInterfaces;
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
    public class UserAddressManagmentController : ControllerBase
    {
        private readonly IUserAddressManagmentService _userAddressManagmentService;

        public UserAddressManagmentController(
            IUserAddressManagmentService userAddressManagmentService)
        {
            _userAddressManagmentService = userAddressManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelUserAddressListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserAddressDataTable")]
        public async Task<IActionResult> GetUserAddressDataTable(DataTableFilterModel<GetPanelUserAddressDataTableRequestDto> dataTableFilter)
        {
            var result = await _userAddressManagmentService.GetUserAddressDataTable(dataTableFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetProcessingUserAddressDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetProcessingUserAddressDetail")]
        public async Task<IActionResult> GetProcessingUserAddressDetail(GetProcessingUserAddressDetailRequestDto getProcessingUserAddressDetailRequest)
        {
            var result = await _userAddressManagmentService.GetProcessingUserAddressDetail(getProcessingUserAddressDetailRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserAddressDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserAddressDetail")]
        public async Task<IActionResult> GetUserAddressDetail(GetUserAddressDetailRequestDto getUserAddressDetailRequest)
        {
            var result = await _userAddressManagmentService.GetUserAddressDetail(getUserAddressDetailRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserAddressForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserAddressForSelectList")]
        public async Task<IActionResult> GetUserAddressForSelectList(GetUserAddressForSelectListRequestDto getUserAddressForSelectListRequest)
        {
            var result = await _userAddressManagmentService.GetUserAddressForSelectList(getUserAddressForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateUserProcessAddressResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateUserProcessAddress")]
        public async Task<IActionResult> CreateUserProcessAddress(CreateUserProcessAddressRequestDto createUserProcessAddressRequest)
        {
            var result = await _userAddressManagmentService.CreateUserProcessAddress(createUserProcessAddressRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<SetUserProcessAddressToPaymentResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetUserProcessAddressToPayment")]
        public async Task<IActionResult> SetUserProcessAddressToPayment(SetUserProcessAddressToPaymentRequestDto setUserAddressToPaymentRequest)
        {
            var result = await _userAddressManagmentService.SetUserProcessAddressToPayment(setUserAddressToPaymentRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetProcessingUserAddressDetailForIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetProcessingUserAddressDetailForIntegration")]
        public async Task<IActionResult> GetProcessingUserAddressDetailForIntegration(GetProcessingUserAddressDetailForIntegrationRequestDto getProcessingUserAddressDetailForIntegrationRequest)
        {
            var result = await _userAddressManagmentService.GetProcessingUserAddressDetailForIntegration(getProcessingUserAddressDetailForIntegrationRequest);
            return this.FromResult(result);
        }
    }
}
