// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\SupportManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelSupportDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.SupportManagmentServiceInterfaces;
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
    public class SupportManagmentController : ControllerBase
    {
        private readonly ISupportManagmentService _supportManagmentService;
        private readonly IConfiguration _configuration;

        public SupportManagmentController(IConfiguration configuration, ISupportManagmentService supportManagmentService)
        {
            _configuration = configuration;
            _supportManagmentService = supportManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelSupportListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportDataTablePanel")]
        public async Task<IActionResult> GetSupportDataTablePanel(DataTableFilterModel<GetPanelSupportDataTableRequestModel> dataTableFilterModel)
        {
            var result = await _supportManagmentService.GetSupportDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelSupportListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportList")]
        public async Task<IActionResult> GetSupportList(DataTableFilterModel<GetPanelSupportListRequestDto> getPanelSupportRequest)
        {
            var result = await _supportManagmentService.GetSupportList(getPanelSupportRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetSupportListForNotificationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportListForNotification")]
        public async Task<IActionResult> GetSupportListForNotification(GetPanelSupportListForNotificationRequestDto getPanelSupportListForNotificationRequest)
        {
            var result = await _supportManagmentService.GetSupportListForNotification(getPanelSupportListForNotificationRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelSupportDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportDetail")]
        public async Task<IActionResult> GetSupportDetail(GetPanelSupportDetailRequestDto getPanelSupportDetailRequest)
        {
            var result = await _supportManagmentService.GetSupportDetail(getPanelSupportDetailRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAnswerSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AnswerSupport")]
        public async Task<IActionResult> AnswerSupport(PanelAnswerSupportRequestDto panelAnswerSupportRequest)
        {
            var result = await _supportManagmentService.AnswerSupport(panelAnswerSupportRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelCloseSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CloseSupport")]
        public async Task<IActionResult> CloseSupport(PanelCloseSupportRequestDto panelCloseSupportRequest)
        {
            var result = await _supportManagmentService.CloseSupport(panelCloseSupportRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelAwaitAnswerSupportsCountResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAwaitAnswerSupportsCount")]
        public async Task<IActionResult> GetAwaitAnswerSupportsCount(GetPanelAwaitAnswerSupportsCountRequestDto getPanelAwaitAnswerSupportsCountRequest)
        {
            var result = await _supportManagmentService.GetAwaitAnswerSupportsCount(getPanelAwaitAnswerSupportsCountRequest);
            return this.FromResult(result);
        }
    }
}
