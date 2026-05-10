// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\DeviceStateNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Notification.Application.Hubs.StationHubs;
using Shared.Domain.Dto.NotificationDto.StationNotificationDtos;

namespace Notification.Api.Controllers
{
    public class DeviceStateNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<DeviceStateHub> _hub;

        public DeviceStateNotificationController(IHubContext<DeviceStateHub> hub, IConfiguration configuration)
        {
            _hub = hub;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult SendAll(DeviceStateNotificationDto deviceStateNotification)
        {
            _hub.Clients.All.SendAsync("deviceStateNotification", new
            {
                GuiId = deviceStateNotification.GuiId,
                LastUpdateDate = deviceStateNotification.LastUpdateDate,
                State = deviceStateNotification.State,
            });
            return Ok("success");
        }
    }
}
