// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\DeviceSocketNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notification.Application.Filters;
using Notification.Application.Hubs.ChargeHubs;
using Shared.Domain.Dto.NotificationDto.ConnectorConnectionDtos;
using Shared.Domain.Dto.NotificationDto.DeviceSocketNotificationDtos;
using Shared.Domain.ServiceInterfaces.NotificationServiceInterfaces.ConnectorConnectionServiceInterfaces;
using System.Threading.Tasks;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class DeviceSocketNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectorConnectionService _connectorConnectionService;
        private readonly IHubContext<SocketHub> _hub;
        private readonly ILogger<DeviceSocketNotificationController> _logger;

        public DeviceSocketNotificationController(IHubContext<SocketHub> hub,
            IConfiguration configuration,
            IConnectorConnectionService connectorConnectionService,
            ILogger<DeviceSocketNotificationController> logger)
        {
            _hub = hub;
            _configuration = configuration;
            _connectorConnectionService = connectorConnectionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Send(DeviceSocketNotificationDto deviceSocketNotification)
        {
            #region cihaz connection bilgisi aliniyor
            var connectorConnection = await _connectorConnectionService.GetConnectorConnection(new GetConnectorConnectionRequestDto() { ChargeDeviceConnectorGuiId = deviceSocketNotification.ChargeDeviceConnectorGuiId });
            #endregion
            await _hub.Clients.All.SendAsync(connectorConnection.Data.ConnectionId, new DeviceSocketNotificationSocketDto()
            {
                SocketMovementState = deviceSocketNotification.SocketMovementState,
                ConnectorGuiId = deviceSocketNotification.ChargeDeviceConnectorGuiId
            });
            return Ok("success");
        }
    }
}
