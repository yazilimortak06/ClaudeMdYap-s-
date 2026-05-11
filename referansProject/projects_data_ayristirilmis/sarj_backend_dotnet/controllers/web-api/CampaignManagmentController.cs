using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.CampaignManagmentServiceInterfaces;
using Shared.Domain.Dto.ApiDto.PanelCampaignDtos;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class CampaignManagmentController : ControllerBase
    {
        private readonly ICampaignManagmentService _campaignManagmentService;
        private readonly IConfiguration _configuration;

        public CampaignManagmentController(
            IConfiguration configuration
,
            ICampaignManagmentService campaignManagmentService)
        {
            _configuration = configuration;
            _campaignManagmentService = campaignManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddCampaignResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCampaign")]
        public async Task<IActionResult> AddCampaign(AddCampaignRequestDto addCampaignRequest)
        {
            var result = await _campaignManagmentService.AddCampaign(addCampaignRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateCampaignResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCampaign")]
        public async Task<IActionResult> UpdateCampaign(UpdateCampaignRequestDto updateCampaignRequest)
        {
            var result = await _campaignManagmentService.UpdateCampaign(updateCampaignRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelCampaignForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCampaignForUpdate")]
        public async Task<IActionResult> GetCampaignForUpdate(GetPanelCampaignForUpdateRequestDto getPanelCampaignForUpdateRequest)
        {
            var result = await _campaignManagmentService.GetCampaignForUpdate(getPanelCampaignForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelCampaignListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCampaignDataTablePanel")]
        public async Task<IActionResult> GetCampaignDataTablePanel(DataTableFilterModel<GetPanelCampaignDataTableRequestModel> dataTableFilterModel)
        {
            var result = await _campaignManagmentService.GetCampaignDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCampaignResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCampaign")]
        public async Task<IActionResult> RemoveCampaign(RemoveCampaignRequestDto removeCampaignRequest)
        {
            var result = await _campaignManagmentService.RemoveCampaign(removeCampaignRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeCampaignStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeCampaignState")]
        public async Task<IActionResult> ChangeCampaignState(ChangeCampaignStateRequestDto changeCampaignStateRequest)
        {
            var result = await _campaignManagmentService.ChangeCampaignState(changeCampaignStateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UploadFileResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCampaignPicture")]
        public async Task<IActionResult> AddCampaignPicture(IFormFile file)
        {
            var result = await _campaignManagmentService.AddCampaignPicture(file, Request);
            return this.FromResult(result);
        }
    }
}
