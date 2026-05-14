using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelAnnouncementDtos;
using Shared.Domain.Dto.ApiDto.PanelCampaignDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.AnnouncementManagmentServiceInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.CampaignManagmentServiceInterfaces;
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
    public class AnnouncementManagmentController : ControllerBase
    {
        private readonly IAnnouncementManagmentService _announcementManagmentService;
        private readonly IConfiguration _configuration;

        public AnnouncementManagmentController(
            IConfiguration configuration,
            IAnnouncementManagmentService announcementManagmentService)
        {
            _configuration = configuration;
            _announcementManagmentService = announcementManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelAnnouncementForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAnnouncementForUpdate")]
        public async Task<IActionResult> GetAnnouncementForUpdate(GetPanelAnnouncementForUpdateRequestDto getPanelAnnouncementForUpdateRequest)
        {
            var result = await _announcementManagmentService.GetAnnouncementForUpdate(getPanelAnnouncementForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateAnnouncementResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateAnnouncement")]
        public async Task<IActionResult> UpdateAnnouncement(UpdateAnnouncementRequestDto updateAnnouncementRequest)
        {
            var result = await _announcementManagmentService.UpdateAnnouncement(updateAnnouncementRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddAnnouncementResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddAnnouncement")]
        public async Task<IActionResult> AddAnnouncement(AddAnnouncementRequestDto addAnnouncementRequest)
        {
            var result = await _announcementManagmentService.AddAnnouncement(addAnnouncementRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelAnnouncementListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAnnouncementDataTablePanel")]
        public async Task<IActionResult> GetAnnouncementDataTablePanel(DataTableFilterModel<GetPanelAnnouncementDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _announcementManagmentService.GetAnnouncementDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeAnnouncementStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeAnnouncementState")]
        public async Task<IActionResult> ChangeAnnouncementState(ChangeAnnouncementStateRequestDto changeAnnouncementStateRequest)
        {
            var result = await _announcementManagmentService.ChangeAnnouncementState(changeAnnouncementStateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveAnnouncementResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveAnnouncement")]
        public async Task<IActionResult> RemoveAnnouncement(RemoveAnnouncementRequestDto removeAnnouncementRequest)
        {
            var result = await _announcementManagmentService.RemoveAnnouncement(removeAnnouncementRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<SendAnnouncementGeneralMessageResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SendAnnouncementGeneralMessage")]
        public async Task<IActionResult> SendAnnouncementGeneralMessage(SendAnnouncementGeneralMessageRequestDto sendAnnouncementGeneralMessageRequest)
        {
            var result = await _announcementManagmentService.SendAnnouncementGeneralMessage(sendAnnouncementGeneralMessageRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UploadFileResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddAnnouncementPicture")]
        public async Task<IActionResult> AddAnnouncementPicture(IFormFile file)
        {
            var result = await _announcementManagmentService.AddAnnouncementPicture(file, Request);
            return this.FromResult(result);
        }
    }
}
