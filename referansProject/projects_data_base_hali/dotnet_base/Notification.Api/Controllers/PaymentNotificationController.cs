// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\PaymentNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notification.Application.Filters;
using Notification.Application.Hubs.PaymentHubs;
using Shared.Domain.Dto.NotificationDto.PaymentNotificationDtos;
using System;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class PaymentNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentNotificationController> _logger;
        private readonly IHubContext<PaymentHub> _hub;

        public PaymentNotificationController(IHubContext<PaymentHub> hub, IConfiguration configuration,
                                            ILogger<PaymentNotificationController> logger)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SendOnlyUser(PaymentNotificationDto paymentNotification)
        {
            try
            {
                _logger.LogError(paymentNotification.ConnectionId);
                _logger.LogError(paymentNotification.MdStatus);
                _logger.LogError(paymentNotification.Status);
                _logger.LogError("isSuccess");
                _logger.LogError(paymentNotification.IsSuccessful + "");
                _hub.Clients.All.SendAsync(paymentNotification.ConnectionId, new PaymentNotificationSocketDto()
                {
                    MdStatus = paymentNotification.MdStatus,
                    Status = paymentNotification.Status,
                    IsSuccessful = paymentNotification.IsSuccessful
                });
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
