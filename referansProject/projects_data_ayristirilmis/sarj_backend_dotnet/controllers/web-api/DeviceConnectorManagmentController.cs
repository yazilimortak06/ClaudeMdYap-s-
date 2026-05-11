using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelChargeDeviceConnectorDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ChargeDeviceManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    public class DeviceConnectorManagmentController : ControllerBase
    {
        private readonly IChargeDeviceConnectorManagmentService _chargeDeviceConnectorManagmentService;
        private readonly IConfiguration _configuration;
        public DeviceConnectorManagmentController(IChargeDeviceConnectorManagmentService chargeDeviceConnectorManagmentService,
            IConfiguration configuration
            )
        {
            _chargeDeviceConnectorManagmentService = chargeDeviceConnectorManagmentService;
            _configuration = configuration;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelChargeDeviceConnectorListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnectorDataTablePanel")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDeviceConnectorDataTablePanel(DataTableFilterModel<GetPanelChargeDeviceConnectorDataTableRequestDto> dataTableFilter)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetChargeDeviceConnectorDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelChargeDeviceConnectorListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnector")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDeviceConnector(GetPanelChargeDeviceConnectorListRequestDto getPanelChargeDeviceConnectorListRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetChargeDeviceConnector(getPanelChargeDeviceConnectorListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelChargeDeviceConnectorsKwResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnectorsKw")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDeviceConnectorsKw(GetPanelChargeDeviceConnectorsKwRequestDto getPanelChargeDeviceConnectorsKwRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetChargeDeviceConnectorsKw(getPanelChargeDeviceConnectorsKwRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelChargeDeviceConnectorsPowerTypeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnectorsPowerType")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDeviceConnectorsPowerType(GetPanelChargeDeviceConnectorsPowerTypeRequestDto getPanelChargeDeviceConnectorsPowerTypeRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetChargeDeviceConnectorsPowerType(getPanelChargeDeviceConnectorsPowerTypeRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeDeviceConnectorForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnectorForSelectList")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDeviceConnectorForSelectList(GetChargeDeviceConnectorForSelectListRequestDto getChargeDeviceConnectorForSelectListRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetChargeDeviceConnectorForSelectList(getChargeDeviceConnectorForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeChargeDeviceConnectorStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeChargeDeviceConnectorState")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> ChangeChargeDeviceConnectorState(ChangeChargeDeviceConnectorStateRequestDto changeChargeDeviceConnectorStateRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.ChangeChargeDeviceConnectorState(changeChargeDeviceConnectorStateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeChargeDeviceConnectorPriceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeConnectorPrice")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> ChangeConnectorPrice(ChangeChargeDeviceConnectorPriceRequestDto changeChargeDeviceConnectorPriceRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.ChangeConnectorPrice(changeChargeDeviceConnectorPriceRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeMultipleChargeDeviceConnectorPriceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeMultipleConnectorPrice")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> ChangeMultipleConnectorPrice(ChangeMultipleChargeDeviceConnectorPriceRequestDto changeMultipleChargeDeviceConnectorPriceRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.ChangeMultipleConnectorPrice(changeMultipleChargeDeviceConnectorPriceRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelChargeDeviceConnectorPriceManagmentListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDeviceConnectorPriceManagmentDataTablePanel")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetDeviceConnectorPriceManagmentDataTablePanel(DataTableFilterModel<GetDeviceConnectorPriceManagmentDataTablePanelRequestDto> dataTableFilter)
        {
            var result = await _chargeDeviceConnectorManagmentService.GetDeviceConnectorPriceManagmentDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelConfirmChangeConnectorPriceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ConfirmChangeConnectorPrice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE })]
        public async Task<IActionResult> ConfirmChangeConnectorPrice(PanelConfirmChangeConnectorPriceRequestDto confirmChangeConnectorPriceRequest)
        {
            var result = await _chargeDeviceConnectorManagmentService.ConfirmChangeConnectorPrice(confirmChangeConnectorPriceRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
