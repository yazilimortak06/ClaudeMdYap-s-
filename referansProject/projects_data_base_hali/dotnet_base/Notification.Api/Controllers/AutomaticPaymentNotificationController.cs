// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\AutomaticPaymentNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notification.Application.Filters;
using Notification.Application.Hubs.PaymentHubs;
using Shared.Domain.Dto.NotificationDto.PaymentNotificationDtos;
using System;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class AutomaticPaymentNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutomaticPaymentNotificationController> _logger;
        private readonly IHubContext<PaymentHub> _hub;

        public AutomaticPaymentNotificationController(IHubContext<PaymentHub> hub, IConfiguration configuration,
                                            ILogger<AutomaticPaymentNotificationController> logger)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SendOnlyUser(AutomaticPaymentNotificationDto automaticPaymentNotification)
        {
            try
            {
                _logger.LogError("veri gitti");
                _logger.LogError(JsonConvert.SerializeObject(automaticPaymentNotification));
                _hub.Clients.All.SendAsync(automaticPaymentNotification.ConnectionId, automaticPaymentNotification);
            }
            catch (Exception ee)
            {
                _logger.LogError("error line 50");
                _logger.LogError(ee.Message);
            }

            return Ok("success");
        }
    }
}
