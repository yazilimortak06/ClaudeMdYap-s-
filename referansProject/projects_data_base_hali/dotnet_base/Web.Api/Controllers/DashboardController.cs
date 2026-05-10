// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\DashboardController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.DashboardServiceInterfaces;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PanelDashboardDtos;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IConfiguration _configuration;

        public DashboardController(IDashboardService dashboardService, IConfiguration configuration)
        {
            _dashboardService = dashboardService;
            _configuration = configuration;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelStationForDashboardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStations")]
        public async Task<IActionResult> GetStations(GetPanelStationForDashboardRequestDto getPanelStationForDashboardRequest)
        {
            var result = await _dashboardService.GetStations(getPanelStationForDashboardRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelMonthlyChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetMonthlyCharge")]
        public async Task<IActionResult> GetMonthlyCharge(GetPanelMonthlyChargeRequestDto getPanelMonthlyChargeRequest)
        {
            var result = await _dashboardService.GetMonthlyCharge(getPanelMonthlyChargeRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelSumOfProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSumOfProcess")]
        public async Task<IActionResult> GetSumOfProcess(GetPanelSumOfProcessRequestDto getPanelSumOfProcessRequest)
        {
            var result = await _dashboardService.GetSumOfProcess(getPanelSumOfProcessRequest);
            return this.FromResult(result);
        }
    }
}
