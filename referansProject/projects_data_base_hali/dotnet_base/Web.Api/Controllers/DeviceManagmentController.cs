// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\DeviceManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelChargeDeviceDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ChargeDeviceManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class DeviceManagmentController : ControllerBase
    {
        private readonly IChargeDeviceManagmentService _chargeDeviceManagmentService;
        private readonly IConfiguration _configuration;
        public DeviceManagmentController(IConfiguration configuration, IChargeDeviceManagmentService chargeDeviceManagmentService)
        {
            _configuration = configuration;
            _chargeDeviceManagmentService = chargeDeviceManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddPanelChargeDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddChargeDevice")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddChargeDevice(AddPanelChargeDeviceRequestDto addPanelChargeDeviceRequest)
        {
            var result = await _chargeDeviceManagmentService.AddChargeDevice(addPanelChargeDeviceRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdatePanelChargeDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateChargeDevice")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> UpdateChargeDevice(UpdatePanelChargeDeviceRequestDto updatePanelChargeDeviceRequest)
        {
            var result = await _chargeDeviceManagmentService.UpdateChargeDevice(updatePanelChargeDeviceRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelChargeDevicePrepareInsertFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChargeDevicePrepareInsertForm")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> ChargeDevicePrepareInsertForm(PanelChargeDevicePrepareInsertFormRequestDto panelChargeDevicePrepareInsertFormRequest)
        {
            var result = await _chargeDeviceManagmentService.ChargeDevicePrepareInsertForm(panelChargeDevicePrepareInsertFormRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelChargeDeviceForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceForUpdate")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetChargeDeviceForUpdate(GetPanelChargeDeviceForUpdateRequestDto getPanelChargeDeviceForUpdateRequest)
        {
            var result = await _chargeDeviceManagmentService.GetChargeDeviceForUpdate(getPanelChargeDeviceForUpdateRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeDeviceListPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceListPanel")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetChargeDeviceListPanel(GetChargeDeviceListPanelRequestDto getChargeDeviceListPanelRequest)
        {
            var result = await _chargeDeviceManagmentService.GetChargeDeviceListPanel(getChargeDeviceListPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelChargeDeviceListItemDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceDataTablePanel")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetChargeDeviceDataTablePanel(DataTableFilterModel<GetPanelChargeDeviceDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _chargeDeviceManagmentService.GetChargeDeviceDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelChargeDevicePowerTypesResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDevicePowerTypes")]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetDevicePowerTypes(GetPanelChargeDevicePowerTypesRequestDto getPanelChargeDevicePowerTypesRequest)
        {
            var result = await _chargeDeviceManagmentService.GetDevicePowerTypes(getPanelChargeDevicePowerTypesRequest);
            return this.FromResult(result);
        }
    }
}
