// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\ReportingController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelDeviceReportingDtos;
using Shared.Domain.Dto.ApiDto.PanelPaymentReportingDtos;
using Shared.Domain.Dto.ApiDto.PanelStationReportingDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ReportingServiceInterfaces;
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
    public class ReportingController : ControllerBase
    {
        private readonly IStationReportingService _stationReportingService;
        private readonly IPaymentReportingService _paymentReportingService;
        private readonly IDeviceReportingService _deviceReportingService;
        private readonly IConfiguration _configuration;

        public ReportingController(IConfiguration configuration, IStationReportingService stationReportingService, IPaymentReportingService paymentReportingService, IDeviceReportingService deviceReportingService)
        {
            _configuration = configuration;
            _stationReportingService = stationReportingService;
            _paymentReportingService = paymentReportingService;
            _deviceReportingService = deviceReportingService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelSumOfStationProcessReportingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSumOfStationProcessReporting")]
        public async Task<IActionResult> GetSumOfStationProcessReporting(GetPanelSumOfStationProcessReportingRequestDto getPanelSumOfStationProcessReportingRequest)
        {
            var result = await _stationReportingService.GetSumOfStationProcessReporting(getPanelSumOfStationProcessReportingRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelStationProcessReportingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationProcessReporting")]
        public async Task<IActionResult> GetStationProcessReporting(GetPanelStationProcessReportingRequestDto getPanelStationProcessReportingRequest)
        {
            var result = await _stationReportingService.GetStationProcessReporting(getPanelStationProcessReportingRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelSumOfStationProcessByPowerTypeReportingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSumOfStationProcessByPowerTypeReporting")]
        public async Task<IActionResult> GetSumOfStationProcessByPowerTypeReporting(GetPanelSumOfStationProcessByPowerTypeReportingRequestDto getPanelSumOfStationProcessByPowerTypeReportingRequest)
        {
            var result = await _stationReportingService.GetSumOfStationProcessByPowerTypeReporting(getPanelSumOfStationProcessByPowerTypeReportingRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<StationProcessReportingDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationProcessReportingDataTablePanel")]
        public async Task<IActionResult> GetStationProcessReportingDataTablePanel(DataTableFilterModel<GetPanelStationProcessReportingDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _stationReportingService.GetStationProcessReportingDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<DeviceProcessReportingDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDeviceProcessReportingDataTablePanel")]
        public async Task<IActionResult> GetDeviceProcessReportingDataTablePanel(DataTableFilterModel<GetPanelDeviceProcessReportingDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _stationReportingService.GetDeviceProcessReportingDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<UserPaymentReportingDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserPaymentReportingDataTablePanel")]
        public async Task<IActionResult> GetUserPaymentReportingDataTablePanel(DataTableFilterModel<GetPanelUserPaymentReportingDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _paymentReportingService.GetUserPaymentReportingDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PrepareMonthlyDeviceProcessByPowerTypeReportingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareMonthlyDeviceProcessByPowerTypeReporting")]
        public async Task<IActionResult> PrepareMonthlyDeviceProcessByPowerTypeReporting(PrepareMonthlyDeviceProcessByPowerTypeReportingRequestDto prepareMonthlyDeviceProcessByPowerTypeReportingRequest)
        {
            var result = await _deviceReportingService.PrepareMonthlyDeviceProcessByPowerTypeReporting(prepareMonthlyDeviceProcessByPowerTypeReportingRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelMonthlyDeviceProcessByPowerTypeReportingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetMonthlyDeviceProcessByPowerTypeReporting")]
        public async Task<IActionResult> GetMonthlyDeviceProcessByPowerTypeReporting(GetPanelMonthlyDeviceProcessByPowerTypeReportingRequestDto getPanelMonthlyDeviceProcessByPowerTypeReportingRequest)
        {
            var result = await _deviceReportingService.GetMonthlyDeviceProcessByPowerTypeReporting(getPanelMonthlyDeviceProcessByPowerTypeReportingRequest);
            return this.FromResult(result);
        }
    }
}
