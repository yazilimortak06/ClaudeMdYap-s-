// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\PanelAdminConnectionController.cs
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Filters;
using Shared.Domain.Dto.NotificationDto.PanelAdminConnectionDtos;
using Shared.Domain.ServiceInterfaces.NotificationServiceInterfaces.PanelAdminConnectionServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class PanelAdminConnectionController : ControllerBase
    {
        private readonly IPanelAdminConnectionService _panelAdminConnectionService;
        public PanelAdminConnectionController(IPanelAdminConnectionService panelAdminConnectionService)
        {
            _panelAdminConnectionService = panelAdminConnectionService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAdminConnectionInsertOrUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "InsertOrUpdatePanelAdminConnection")]
        [ValidateFilter]
        public async Task<IActionResult> InsertOrUpdatePanelAdminConnection(PanelAdminConnectionInsertOrUpdateRequestDto panelAdminConnectionInsertOrUpdateRequest)
        {
            var result = await _panelAdminConnectionService.InsertOrUpdatePanelAdminConnection(panelAdminConnectionInsertOrUpdateRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelAdminConnectionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPanelAdminConnection")]
        [ValidateFilter]
        public async Task<IActionResult> GetPanelAdminConnection(GetPanelAdminConnectionRequestDto getPanelAdminConnectionRequest)
        {
            var result = await _panelAdminConnectionService.GetPanelAdminConnection(getPanelAdminConnectionRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
