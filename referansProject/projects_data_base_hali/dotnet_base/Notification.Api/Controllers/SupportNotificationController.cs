// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Controllers\SupportNotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notification.Application.Filters;
using Shared.Domain.Dto.NotificationDto.MobilConnectionDtos;
using Shared.Domain.Dto.NotificationDto.SupportNotificationDtos;
using Shared.Domain.RepositoryInterfaces.NotificationRepositoryInterfaces.MobilConnectionRepositoryInterfaces;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Notification.Application.Hubs.SupportHubs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.RepositoryInterfaces.NotificationRepositoryInterfaces.ConnectionMessagesRepositoryInterfaces;
using Shared.Domain.Enums.NotificationEnums;
using Shared.Domain.RepositoryInterfaces.NotificationRepositoryInterfaces.PanelAdminConnectionRepositoryInterfaces;
using Shared.Domain.Entities.NotificationEntities.PanelAdminConnectionModule;
using Shared.Domain.Dto.NotificationDto.PanelAdminConnectionDtos;
using Shared.Domain.Entities.NotificationEntities.ConnectionModule;

namespace Notification.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(NotificationApiRequestResponseFilterAttribute))]
    public class SupportNotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMobilConnectionRepository _mobilConnectionRepository;
        private readonly IPanelAdminConnectionRepository _panelAdminConnectionRepository;
        private readonly IConnectionMessagesRepository _connectionMessagesRepository;
        private readonly ILogger<SupportNotificationController> _logger;
        private readonly IHubContext<SupportHub> _hub;
        private readonly IHubContext<SupportNotificationHub> _supportNotificationHub;

        public SupportNotificationController(IHubContext<SupportHub> hub, IConfiguration configuration,
                                            ILogger<SupportNotificationController> logger,
                                            IMobilConnectionRepository mobilConnectionRepository,
                                            IPanelAdminConnectionRepository panelAdminConnectionRepository,
                                            IConnectionMessagesRepository connectionMessagesRepository,
                                            IHubContext<SupportNotificationHub> supportNotificationHub)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
            _mobilConnectionRepository = mobilConnectionRepository;
            _panelAdminConnectionRepository = panelAdminConnectionRepository;
            _connectionMessagesRepository = connectionMessagesRepository;
            _supportNotificationHub = supportNotificationHub;
        }

        [HttpPost]
        public async Task<IActionResult> SendOnlyUser(SupportNotificationForMobilDto supportNotification)
        {
            try
            {
                #region kullanicinin connectionid si cekiliyor
                MobilConnectionFilterDto mobilConnectionFilter = new MobilConnectionFilterDto();
                mobilConnectionFilter.MobilUserGuiId = supportNotification.MobilUserGuiId;
                var mobilConnection = await _mobilConnectionRepository.GetMobilConnection(mobilConnectionFilter).FirstOrDefaultAsync();
                #endregion
                if (mobilConnection != null)
                {
                    await _hub.Clients.All.SendAsync(mobilConnection.ConnectionId, supportNotification);
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(ee.Message);
            }
            return Ok("success");
        }

        [HttpPost]
        public async Task<IActionResult> Send(SupportNotificationForMobilDto supportNotification)
        {
            try
            {
                _logger.LogError("sp notify");
                PanelAdminConnection? panelAdminConnection = null;
                #region gonderilecek bildirim icin admin mevcut ise connection verisi cekiliyor ve ona gonderiliyor
                if (supportNotification.AdminGuiId != null)
                {
                    PanelAdminConnectionFilterDto panelAdminConnectionFilter = new PanelAdminConnectionFilterDto();
                    panelAdminConnectionFilter.AdminGuiId = supportNotification.AdminGuiId;
                    panelAdminConnection = await _panelAdminConnectionRepository.GetPanelAdminConnectionAsNoTracking(panelAdminConnectionFilter).FirstOrDefaultAsync();
                    if (panelAdminConnection != null)
                    {
                        await _hub.Clients.Group(panelAdminConnection.ConnectionId).SendAsync("supportNotification", supportNotification);
                    }
                }
                #endregion
                #region butun dinleyicilere gonderiliyor
                else
                {
                    await _hub.Clients.All.SendAsync("supportNotification", supportNotification);
                }
                #endregion
                #region panel destek talep bildirim kismi icin gonderiliyor
                SupportForPanelNotificationDto supportForPanelNotification = new SupportForPanelNotificationDto();
                supportForPanelNotification.SupportId = supportNotification.Id.GetValueOrDefault();
                supportForPanelNotification.LastUpdateDate = supportNotification.LastUpdateDate;
                supportForPanelNotification.UserName = supportNotification.UserName;
                supportForPanelNotification.UserSurname = supportNotification.UserSurname;
                supportForPanelNotification.SupportNotificationType = supportNotification.SupportNotificationType.GetValueOrDefault();
                supportForPanelNotification.SummaryTitle = supportNotification.SummaryTitle;
                supportForPanelNotification.State = supportNotification.State.GetValueOrDefault();
                _logger.LogError("sp notify2");
                _logger.LogError(supportNotification.AdminGuiId);
                _logger.LogError(JsonConvert.SerializeObject(supportNotification));
                await _supportNotificationHub.Clients.All.SendAsync("supportForPanelNotification", supportForPanelNotification);
                #endregion
                #region gonderilecek mesaj, panel admin connection messages tablosuna kayit ediliyor
                ConnectionMessage connectionMessage = new ConnectionMessage();
                connectionMessage.MessageJson = JsonConvert.SerializeObject(supportNotification,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                connectionMessage.ReceivedDate = DateTime.Now;
                connectionMessage.ConnectionSendMessageType = ConnectionSendMessageTypeEnum.PANEL_SUPPORT;
                connectionMessage.PanelAdminConnectionId = panelAdminConnection != null ? panelAdminConnection.Id : null;
                await _connectionMessagesRepository.InsertAsync(connectionMessage);
                await _connectionMessagesRepository.SaveChangesAsync();
                #endregion
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
