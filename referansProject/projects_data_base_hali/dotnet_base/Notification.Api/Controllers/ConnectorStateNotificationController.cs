// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\ConnectorStateNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Notification.Application.Filters;
using Notification.Application.Hubs.StationHubs;
using Shared.Domain.Dto.NotificationDto.ConnectorSocketNotificationDtos;
using Shared.Domain.Dto.NotificationDto.StationNotificationDtos;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class ConnectorStateNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ConnectorStateHub> _hub;

        public ConnectorStateNotificationController(IHubContext<ConnectorStateHub> hub, IConfiguration configuration)
        {
            _hub = hub;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult SendAll(ConnectorStateNotificationDto connectorStateNotification)
        {
            _hub.Clients.All.SendAsync("connectorStateNotification", new ConnectorSocketNotificationSocketDto()
            {
                GuiId = connectorStateNotification.GuiId,
                State = connectorStateNotification.State,
            });
            return Ok("success");
        }
    }
}
