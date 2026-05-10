// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\MobilConnectionController.cs
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Filters;
using Shared.Domain.Dto.NotificationDto.MobilConnectionDtos;
using Shared.Domain.ServiceIntefaces.NotificationServiceInterfaces.MobilConnectionServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class MobilConnectionController : ControllerBase
    {
        private readonly IMobilConnectionService _mobilConnectionService;
        public MobilConnectionController(IMobilConnectionService mobilConnectionService)
        {
            _mobilConnectionService = mobilConnectionService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilConnectionInsertOrUpdateRequestDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "InsertOrUpdateMobilConnection")]
        [ValidateFilter]
        public async Task<IActionResult> InsertOrUpdateMobilConnection(MobilConnectionInsertOrUpdateRequestDto mobilConnectionInsertOrUpdateRequest)
        {
            var result = await _mobilConnectionService.InsertOrUpdateMobilConnection(mobilConnectionInsertOrUpdateRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilConnectionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetMobilConnection")]
        [ValidateFilter]
        public async Task<IActionResult> GetMobilConnection(GetMobilConnectionRequestDto getMobilConnectionRequestDto)
        {
            var result = await _mobilConnectionService.GetMobilConnection(getMobilConnectionRequestDto);
            return this.FromHttpClientResult(result);
        }
    }
}
