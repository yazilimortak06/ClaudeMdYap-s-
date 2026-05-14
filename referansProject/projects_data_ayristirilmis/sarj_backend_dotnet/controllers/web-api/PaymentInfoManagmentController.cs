using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.PanelPaymentInfoDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PaymentInfoManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    //[ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
 
    public class PaymentInfoManagmentController : ControllerBase
    {
        private readonly IPaymentInfoManagmentService _paymentInfoManagmentService;

        public PaymentInfoManagmentController(IPaymentInfoManagmentService paymentInfoManagmentService)
        {
            _paymentInfoManagmentService = paymentInfoManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelPaymentInfoDashboardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPanelPaymentInfoDashboard")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetPanelPaymentInfoDashboard(GetPanelPaymentInfoDashboardRequestDto getPanelPaymentInfoDashboardRequest)
        {
            var result = await _paymentInfoManagmentService.GetPanelPaymentInfoDashboard(getPanelPaymentInfoDashboardRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentInfoDetailPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentInfoDetailPanel")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetPaymentInfoDetailPanel(GetPaymentInfoDetailPanelRequestDto getPaymentInfoDetailPanelRequest)
        {
            var result = await _paymentInfoManagmentService.GetPaymentInfoDetailPanel(getPaymentInfoDetailPanelRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelPaymentInfoListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentInfoDataTablePanel")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetPaymentInfoDataTablePanel(DataTableFilterModel<GetPanelPaymentInfoDatatableRequestDto> dataTableFilter)
        {
            var result = await _paymentInfoManagmentService.GetPaymentInfoDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AutomaticPaymentResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AutomaticPayment")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, })]
        public async Task<IActionResult> AutomaticPayment(AutomaticPaymentRequestDto automaticPaymentResquest)
        {
            var result = await _paymentInfoManagmentService.AutomaticPayment(automaticPaymentResquest);
            return this.FromHttpClientDataResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelExportExcelPaymentInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ExportExcelPaymentInfo")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> ExportExcelPaymentInfo(PanelExportExcelPaymentInfoRequestDto panelExportExcelPaymentInfoRequest)
        {
            var result = await _paymentInfoManagmentService.ExportExcelPaymentInfo(panelExportExcelPaymentInfoRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelExportPdfPaymentInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ExportPdfPaymentInfo")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> ExportPdfPaymentInfo(PanelExportPdfPaymentInfoRequestDto panelExportPdfPaymentInfoRequest)
        {
            var result = await _paymentInfoManagmentService.ExportPdfPaymentInfo(panelExportPdfPaymentInfoRequest);
            return this.FromResult(result);
        }
    }
}
