// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\ChargeNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notification.Application.Filters;
using Notification.Application.Hubs.ChargeHubs;
using Shared.Domain.Dto.NotificationDto.ChargeNotificationDtos;
using System;
using System.Globalization;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class ChargeNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChargeNotificationController> _logger;
        private readonly IHubContext<ChargeHub> _hub;

        public ChargeNotificationController(IHubContext<ChargeHub> hub, IConfiguration configuration,
                                            ILogger<ChargeNotificationController> logger)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SendOnlyUser(ChargeNotificationDto chargeNotification)
        {
            try
            {
                _logger.LogError(JsonConvert.SerializeObject(chargeNotification));
                _logger.LogError(chargeNotification.ConnectionId);
                _logger.LogError(chargeNotification.TotalMeter + "");
                _logger.LogError(chargeNotification.Percent + "");
                if (chargeNotification.Percent > 100)
                {
                    chargeNotification.Percent = 100;
                }
                _hub.Clients.All.SendAsync(chargeNotification.ConnectionId, new ChargeNotificationDto()
                {
                    TotalMeter = Math.Round(chargeNotification.TotalMeter, 2),
                    Percent = chargeNotification.Percent != null ? chargeNotification.Percent.GetValueOrDefault() : null,
                    CalculatedPrice = Math.Round(chargeNotification.CalculatedPrice, 2),
                    ChargeState = chargeNotification.ChargeState,
                    StartDate = chargeNotification.StartDateLong != null ? chargeNotification.StartDateLong.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : "",
                    EndDate = chargeNotification.EndDateLong != null ? chargeNotification.EndDateLong.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : "",
                    StartDateLong = chargeNotification.StartDateLong != null ? DateTime.Parse(chargeNotification.StartDateLong.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None) : null,
                    EndDateLong = chargeNotification.EndDateLong != null ? DateTime.Parse(chargeNotification.EndDateLong.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None) : null,
                    ChargeVelocity = chargeNotification.ChargeVelocity,
                    ChargeVelocityStr = chargeNotification.ChargeVelocityStr,
                    ChargeGuiId = chargeNotification.ChargeGuiId,
                });
            }
            catch (Exception ee)
            {
                _logger.LogError("error line 50");
                _logger.LogError(ee.Message);
            }
            return Ok("success");
        }

        [HttpPost]
        public IActionResult SendChargeProcessInformationToPanel(ChargeNotificationDto chargeNotification)
        {
            try
            {
                _logger.LogError(chargeNotification.ConnectionId);
                _logger.LogError(chargeNotification.TotalMeter + "");
                _logger.LogError(chargeNotification.Percent + "");
                if (chargeNotification.Percent > 100)
                {
                    chargeNotification.Percent = 100;
                }
                _hub.Clients.All.SendAsync(chargeNotification.ChargeGuiId, new ChargeNotificationDto()
                {
                    TotalMeter = Math.Round(chargeNotification.TotalMeter, 2),
                    Percent = chargeNotification.Percent != null ? chargeNotification.Percent.GetValueOrDefault() : null,
                    CalculatedPrice = Math.Round(chargeNotification.CalculatedPrice, 2),
                    ChargeState = chargeNotification.ChargeState,
                    StartDate = chargeNotification.StartDate,
                    EndDate = chargeNotification.EndDate,
                    ProcessingTimeTotalMiliseconds = chargeNotification.ProcessingTimeTotalMiliseconds,
                    ChargeGuiId = chargeNotification.ChargeGuiId,
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
