// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\ChargeManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ChargeManagmentServiceInterfaces;
using Shared.Domain.Dto.ApiDto.PanelChargeDtos;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class ChargeManagmentController : ControllerBase
    {
        private readonly IChargeManagmentService _chargeManagmentService;
        public ChargeManagmentController(IChargeManagmentService chargeManagmentService)
        {
            _chargeManagmentService = chargeManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelChargeListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDatatable")]
        public async Task<IActionResult> GetChargeDatatable(DataTableFilterModel<GetPanelChargeDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _chargeManagmentService.GetChargeDatatable(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeDetailPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDetail")]
        public async Task<IActionResult> GetChargeDetail(GetChargeDetailPanelRequestDto getChargeDetailPanelRequest)
        {
            var result = await _chargeManagmentService.GetChargeDetail(getChargeDetailPanelRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelChangeChargeStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeChargeState")]
        public async Task<IActionResult> ChangeChargeState(PanelChangeChargeStateRequestDto panelChangeStateChargeRequest)
        {
            var result = await _chargeManagmentService.ChangeChargeState(panelChangeStateChargeRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelExportExcelChargeProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ExportExcelChargeProcess")]
        public async Task<IActionResult> ExportExcelChargeProcess(PanelExportExcelChargeProcessRequestDto panelExportExcelChargeProcessRequest)
        {
            var result = await _chargeManagmentService.ExportExcelChargeProcess(panelExportExcelChargeProcessRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelExportPdfChargeProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ExportPdfChargeProcess")]
        public async Task<IActionResult> ExportPdfChargeProcess(PanelExportPdfChargeProcessRequestDto panelExportPdfChargeProcessRequest)
        {
            var result = await _chargeManagmentService.ExportPdfChargeProcess(panelExportPdfChargeProcessRequest);
            return this.FromResult(result);
        }
    }
}
