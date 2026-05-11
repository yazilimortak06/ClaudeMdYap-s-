using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelLogDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.LogManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class LogManagmentController : ControllerBase
    {
        private readonly ILogManagmentService _logManagmentService;
        private readonly IConfiguration _configuration;
        public LogManagmentController(IConfiguration configuration,
            ILogManagmentService logManagmentService)
        {
            _configuration = configuration;
            _logManagmentService = logManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelRequestResponseDataTableDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetRequestResponseDatatablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetRequestResponseDatatablePanel(DataTableFilterModel<PanelGetRequestResponseDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _logManagmentService.GetRequestResponseDatatablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelExceptionDataTableDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetExceptionDatatablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetExceptionDatatablePanel(DataTableFilterModel<PanelGetExceptionDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _logManagmentService.GetExceptionDatatablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
    }
}
