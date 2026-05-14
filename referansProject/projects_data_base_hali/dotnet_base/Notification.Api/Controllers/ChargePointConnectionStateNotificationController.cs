// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\ChargePointConnectionStateNotification.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Notification.Application.Filters;
using Notification.Application.Hubs.StationHubs;
using Shared.Domain.Dto.NotificationDto.StationNotificationDtos;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class ChargePointConnectionStateNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChargePointConnectionStateHub> _hub;

        public ChargePointConnectionStateNotificationController(IHubContext<ChargePointConnectionStateHub> hub, IConfiguration configuration)
        {
            _hub = hub;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult SendAll(ChargePointConnectionStateNotificationDto chargePointConnectionStateNotification)
        {
            _hub.Clients.All.SendAsync("deviceConnectionStateNotification", new
            {
                GuiId = chargePointConnectionStateNotification.GuiId,
                ConnectState = chargePointConnectionStateNotification.ConnectState,
                Name = chargePointConnectionStateNotification.Name,
                LastUpdateDate = chargePointConnectionStateNotification.LastInstantStateUpdatedDate,
                ConnectionDate = chargePointConnectionStateNotification.ConnectionDate,
                DisconnectionDate = chargePointConnectionStateNotification.DisconnectionDate,
                State = chargePointConnectionStateNotification.InstantState,
            });
            return Ok("success");
        }
    }
}
