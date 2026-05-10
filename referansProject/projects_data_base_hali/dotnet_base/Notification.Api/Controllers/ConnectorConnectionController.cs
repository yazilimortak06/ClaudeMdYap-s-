// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\ConnectorConnectionController.cs
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Filters;
using Shared.Domain.Dto.NotificationDto.ConnectorConnectionDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.NotificationServiceInterfaces.ConnectorConnectionServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class ConnectorConnectionController : ControllerBase
    {
        private readonly IConnectorConnectionService _connectorConnectionService;
        public ConnectorConnectionController(IConnectorConnectionService connectorConnectionService)
        {
            _connectorConnectionService = connectorConnectionService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddMultipleConnectorConnectionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddMultipleConnectorConnection")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> AddMultipleConnectorConnection(AddMultipleConnectorConnectionRequestDto addMultipleConnectorConnectionRequest)
        {
            var result = await _connectorConnectionService.AddMultipleConnectorConnection(addMultipleConnectorConnectionRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetConnectorConnectionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetConnectorConnection")]
        [InnerRequestAttribute(new ApiName[] { ApiName.STATION, ApiName.MOBIL })]
        public async Task<IActionResult> GetConnectorConnection(GetConnectorConnectionRequestDto getConnectorConnectionRequest)
        {
            var result = await _connectorConnectionService.GetConnectorConnection(getConnectorConnectionRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
