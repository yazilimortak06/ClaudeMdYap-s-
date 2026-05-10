// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\DeviceRemoteManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelChargeDeviceRemoteManagmentDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ChargeDeviceManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using Shared.Domain.Dto.ApiDto.PanelOcppCommandMessageDtos;
using Shared.Domain.Dto.ApiDto.PanelChargeDeviceDtos;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class DeviceRemoteManagmentController : ControllerBase
    {
        private readonly IChargeDeviceRemoteManagmentService _chargeDeviceRemoteManagmentService;
        private readonly IConfiguration _configuration;

        public DeviceRemoteManagmentController(IConfiguration configuration, IChargeDeviceRemoteManagmentService chargeDeviceRemoteManagmentService)
        {
            _configuration = configuration;
            _chargeDeviceRemoteManagmentService = chargeDeviceRemoteManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelRemoteManagmentDeviceListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDeviceForRemoteManagmentDataTablePanel")]
        public async Task<IActionResult> GetDeviceForRemoteManagmentDataTablePanel(DataTableFilterModel<GetDeviceForRemoteManagmentDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _chargeDeviceRemoteManagmentService.GetDeviceForRemoteManagmentDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelOcppCommandMessageListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetOcppCommandMessageDataTablePanel")]
        [ValidateFilter]
        public async Task<IActionResult> GetOcppCommandMessageDataTablePanel(DataTableFilterModel<GetPanelOcppCommandDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _chargeDeviceRemoteManagmentService.GetOcppCommandMessageDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DeviceRemoteStartTransactionFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "DeviceRemoteStartTransactionFromPanel")]
        public async Task<IActionResult> DeviceRemoteStartTransactionFromPanel(DeviceRemoteStartTransactionFromPanelRequestDto deviceRemoteStartTransactionFromPanelRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.DeviceRemoteStartTransactionFromPanel(deviceRemoteStartTransactionFromPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DeviceRemoteStopTransactionFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "DeviceRemoteStopTransactionFromPanel")]
        public async Task<IActionResult> DeviceRemoteStopTransactionFromPanel(DeviceRemoteStopTransactionFromPanelRequestDto deviceRemoteStopTransactionFromPanelRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.DeviceRemoteStopTransactionFromPanel(deviceRemoteStopTransactionFromPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeDeviceAvailabilityFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeDeviceAvailabilityFromPanel")]
        public async Task<IActionResult> ChangeDeviceAvailabilityFromPanel(ChangeDeviceAvailabilityFromPanelRequestDto changeDeviceAvailabilityFromPanelRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.ChangeDeviceAvailabilityFromPanel(changeDeviceAvailabilityFromPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DeviceRemoteResetFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "DeviceRemoteResetFromPanel")]
        public async Task<IActionResult> DeviceRemoteResetFromPanel(DeviceRemoteResetFromPanelRequestDto deviceRemoteResetFromPanelRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.DeviceRemoteResetFromPanel(deviceRemoteResetFromPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PrepareChangeConfigurationDeviceFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareChangeConfigurationDeviceForm")]
        public async Task<IActionResult> PrepareChangeConfigurationDeviceForm(PrepareChangeConfigurationDeviceFormRequestDto prepareChangeConfigurationDeviceFormRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.PrepareChangeConfigurationDeviceForm(prepareChangeConfigurationDeviceFormRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DeviceRemoteChangeConfigurationFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "DeviceRemoteChangeConfigurationFromPanel")]
        public async Task<IActionResult> DeviceRemoteChangeConfigurationFromPanel(DeviceRemoteChangeConfigurationFromPanelRequestDto deviceRemoteChangeConfigurationFromPanelRequest)
        {
            var result = await _chargeDeviceRemoteManagmentService.DeviceRemoteChangeConfigurationFromPanel(deviceRemoteChangeConfigurationFromPanelRequest);
            return this.FromResult(result);
        }
    }
}
