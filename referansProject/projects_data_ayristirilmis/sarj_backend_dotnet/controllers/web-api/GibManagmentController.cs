using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.Extensions.Configuration;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.GibManagmentServiceInterfaces;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.ApiDto.GibDtos;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using Api.Application.Filters.WebApi;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class GibManagmentController : ControllerBase
    {
        private readonly IGibManagmentService _gibManagmentService;
        private readonly IGibReportingService _gibReportingService;
        private readonly IConfiguration _configuration;
        public GibManagmentController(IConfiguration configuration,
            IGibManagmentService gibManagmentService,
            IGibReportingService gibReportingService)
        {
            _configuration = configuration;
            _gibManagmentService = gibManagmentService;
            _gibReportingService = gibReportingService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<EsuGibChargeDeviceListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetEsuGibChargeDeviceDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetEsuGibChargeDeviceDataTablePanel(DataTableFilterModel<GetPanelEsuGibChargeDeviceDatatableRequestDto> dataTableFilter)
        {
            var result = await _gibManagmentService.GetEsuGibChargeDeviceDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<InstantGibReportingDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetInstantGibReportDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetInstantGibReportDataTablePanel(DataTableFilterModel<GetInstantGibReportDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _gibReportingService.GetInstantGibReportDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddEsuGibTaxPayerResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddEsuGibTaxPayer")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddEsuGibTaxPayer(AddEsuGibTaxPayerRequestDto addEsuGibTaxPayerRequest)
        {
            var result = await _gibManagmentService.AddEsuGibTaxPayer(addEsuGibTaxPayerRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddEsuGibChargeDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddEsuGibChargeDevice")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddEsuGibChargeDevice(AddEsuGibChargeDeviceRequestDto addEsuGibChargeDeviceRequest)
        {
            var result = await _gibManagmentService.AddEsuGibChargeDevice(addEsuGibChargeDeviceRequest);
            return this.FromResult(result);
        }
    }
}
