using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelFirmDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.FirmManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.PanelCompanyDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.CompanyManagmentServiceInterfaces;
using FrameworkCore.FrameworkCore.FilterAttributeCore;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class CompanyManagmentController : ControllerBase
    {
        private readonly ICompanyManagmentService _companyManagmentService;
        private readonly IConfiguration _configuration;
        public CompanyManagmentController(IConfiguration configuration,
            ICompanyManagmentService companyManagmentService)
        {
            _configuration = configuration;
            _companyManagmentService = companyManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddCompanyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCompany")]
        [ValidateFilter]
        public async Task<IActionResult> AddCompany(AddCompanyRequestDto addCompanyRequest)
        {
            var result = await _companyManagmentService.AddCompany(addCompanyRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateCompanyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCompany")]
        [ValidateFilter]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyRequestDto updateCompanyRequest)
        {
            var result = await _companyManagmentService.UpdateCompany(updateCompanyRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCompanyForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCompanyForUpdate")]
        public async Task<IActionResult> GetCompanyForUpdate(GetCompanyForUpdateRequestDto getCompanyForUpdateRequest)
        {
            var result = await _companyManagmentService.GetCompanyForUpdate(getCompanyForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CompanyDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCompanyDataTablePanel")]
        public async Task<IActionResult> GetCompanyDataTablePanel(DataTableFilterModel<GetCompanyDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _companyManagmentService.GetCompanyDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCompanyForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCompanyForSelectList")]
        public async Task<IActionResult> GetCompanyForSelectList(GetCompanyForSelectListRequestDto getCompanyForSelectListRequest)
        {
            var result = await _companyManagmentService.GetCompanyForSelectList(getCompanyForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCompanyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCompany")]
        public async Task<IActionResult> RemoveCompany(RemoveCompanyRequestDto removeCompanyRequest)
        {
            var result = await _companyManagmentService.RemoveCompany(removeCompanyRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeActiveStateCompanyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeActiveStateCompany")]
        public async Task<IActionResult> ChangeActiveStateCompany(ChangeActiveStateCompanyRequestDto changeActiveStateCompanyRequest)
        {
            var result = await _companyManagmentService.ChangeActiveStateCompany(changeActiveStateCompanyRequest);
            return this.FromResult(result);
        }
    }
}
