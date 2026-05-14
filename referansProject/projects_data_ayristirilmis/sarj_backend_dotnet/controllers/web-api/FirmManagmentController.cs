using Api.Application.Filters.WebApi;
using Api.Application.Services.PanelServices.FirmManagment;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelFirmDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.FirmManagmentServiceInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.StationManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.FilterAttributeCore;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class FirmManagmentController : ControllerBase
    {
        private readonly IFirmManagmentService _firmManagmentService;
        private readonly IConfiguration _configuration;
        public FirmManagmentController( IConfiguration configuration,
            IFirmManagmentService firmManagmentService)
        {
            _configuration = configuration;
            _firmManagmentService = firmManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddFirmResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddFirm")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddFirm(AddFirmRequestDto addFirmRequest)
        {
            var result = await _firmManagmentService.AddFirm(addFirmRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddOrUpdateFirmSettingFromPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddOrUpdateFirmSettingFromPanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> AddOrUpdateFirmSettingFromPanel(AddOrUpdateFirmSettingFromPanelRequestDto addOrUpdateFirmSettingFromPanelRequest)
        {
            var result = await _firmManagmentService.AddOrUpdateFirmSettingFromPanel(addOrUpdateFirmSettingFromPanelRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateFirmResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateFirm")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> UpdateFirm(UpdateFirmRequestDto updateFirmRequest)
        {
            var result = await _firmManagmentService.UpdateFirm(updateFirmRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<FirmDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetFirmDataTablePanel(DataTableFilterModel<GetFirmDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _firmManagmentService.GetFirmDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<FirmPriceDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmPriceDatatablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetFirmPriceDatatablePanel(DataTableFilterModel<GetFirmPriceDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _firmManagmentService.GetFirmPriceDatatablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetFirmForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmForSelectList")]
        [ServiceFilter(typeof(WebApiFirmAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetFirmForSelectList(GetFirmForSelectListRequestDto getFirmForSelectListRequest)
        {
            var result = await _firmManagmentService.GetFirmForSelectList(getFirmForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetFirmForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmForUpdate")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetFirmForUpdate(GetFirmForUpdateRequestDto getFirmForUpdateRequest)
        {
            var result = await _firmManagmentService.GetFirmForUpdate(getFirmForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelFirmSettingPrepareFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "FirmSettingPrepareForm")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> FirmSettingPrepareForm(PanelFirmSettingPrepareFormRequestDto panelFirmSettingPrepareFormRequest)
        {
            var result = await _firmManagmentService.FirmSettingPrepareForm(panelFirmSettingPrepareFormRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeActiveStateFirmResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeActiveStateFirm")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> ChangeActiveStateFirm(ChangeActiveStateFirmRequestDto changeActiveStateFirmRequest)
        {
            var result = await _firmManagmentService.ChangeActiveStateFirm(changeActiveStateFirmRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveFirmResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveFirm")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> RemoveFirm(RemoveFirmRequestDto removeFirmRequest)
        {
            var result = await _firmManagmentService.RemoveFirm(removeFirmRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddOrUpdateFirmPriceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddOrUpdateFirmPrice")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddOrUpdateFirmPrice(AddOrUpdateFirmPriceRequestDto addFirmPriceRequest)
        {
            var result = await _firmManagmentService.AddOrUpdateFirmPrice(addFirmPriceRequest);
            return this.FromResult(result); 
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveFirmPriceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveFirmPrice")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> RemoveFirmPrice(RemoveFirmPriceRequestDto removeFirmPriceRequest)
        {
            var result = await _firmManagmentService.RemoveFirmPrice(removeFirmPriceRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UploadFileResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddFirmLogo")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        public async Task<IActionResult> AddFirmLogo(IFormFile file)
        {
            var uploadFileResponse = await _firmManagmentService.AddFirmLogo(file, Request);
            return this.FromResult(uploadFileResponse);
        }
    }
}
