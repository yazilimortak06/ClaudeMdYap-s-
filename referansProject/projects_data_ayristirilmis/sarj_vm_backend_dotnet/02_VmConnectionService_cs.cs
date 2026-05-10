// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\Vm.Application\Services\ConnectionManagement\VmConnectionService.cs
// VmConnectionService - ANA PARTIAL DOSYA
// Sorumluluk: OCPP WebSocket bağlantısı yönetimi (VM <-> Device <-> CPO <-> Server üçgen mimarisi)

using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.Extentions;
using Shared.Domain.Errors.VmErrors;
using NetTopologySuite.Densify;
using Shared.Domain.Enums.VmEnums;
using Shared.Domain.Dto.VmDto.VmAuthHorizationDtos;
using Shared.Domain.Dto.VmDto.VmTransactionDtos;
using Shared.Domain.Dto.VmDto.VmStatusNotificationDtos;
using Shared.Domain.Dto.VmDto.VmIdTagInfoDtos;
using Shared.Domain.Dto.VmDto.VmHeartBeatDtos;
using Shared.Domain.Dto.VmDto.VmBootNotificationDtos;
using Shared.Domain.Dto.VmDto.VmDataTransferDtos;
using Shared.Domain.Dto.VmDto.VmDiagnosticsStatusNotificationDtos;
using Shared.Domain.Dto.VmDto.VmFirmwareStatusNotificationDtos;
using Shared.Domain.Dto.VmDto.VmConnectionDtos;
using Shared.Domain.RepositoryInterfaces.VmRepositoryInterfaces.VmCpoRepositoryInterfaces;
using Shared.Domain.Entities.VmEntities.VmCpoModule;
using Shared.Domain.Dto.VmDto.VmCpoDtos;
using Shared.Domain.Dto.VmDto.VmEvccIdDtos;
using Shared.Domain.RepositoryInterfaces.VmRepositoryInterfaces.VmEvccIdRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.CpoManagementServiceInterfaces;
using Shared.Domain.Dto.VmDto.VmDeviceDtos;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.DeviceManagementServiceInterfaces;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.ConnectionManagementsServiceInterfaces;
using Shared.Domain.RepositoryInterfaces.VmRepositoryInterfaces.VmParameterRepositoryInterfaces;
using Shared.Domain.Entities.VmEntities.VmParameterModule;
using Shared.Domain.Dto.VmDto.VmParameterDtos;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.VmEntities.VmCommandMessageModule;
using Shared.Domain.RepositoryInterfaces.VmRepositoryInterfaces.VmCommandMessageRepositoryInterfaces;
using Shared.Domain.Dto.VmDto.VmDeviceConnectionDtos;
using Shared.Domain.Entities.VmEntities.VmDeviceConnectionModule;
using Shared.Domain.Dto.VmDto.VmServerConnectionDtos;
using System.Data;
using Shared.Domain.Dto.VmDto.VmCpoConnectionDtos;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.TransactionManagementServiceInterfaces;
using Shared.Domain.Dto.VmDto.VmStartTransactionDtos;
using Shared.Domain.Dto.VmDto.VmOcppMeterValuesDtos;
using Shared.Domain.Dto.VmDto.VmMeterValueDtos;
using Shared.Domain.Dto.VmDto.VmConnectorConnectionDtos;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.VmAuthorizeManagementServiceInterfaces;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Shared.Domain.Dto.VmDto.OcppDeviceResponseDtos;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.BootNotificationManagementServiceInterfaces;
using static System.Collections.Specialized.BitVector32;

namespace Vm.Application.Services.ConnectionManagement
{
    public partial class VmConnectionService : BaseService, IVmConnectionService
    {
        ICustomHttpUtilService _customHttpUtilService;
        ILogger<VmConnectionService> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        // CRITICAL FIX #12: Thread-safe ConcurrentDictionary to prevent server crashes
        public static ConcurrentDictionary<string, VmConnectionSessionDto> _vmSessionStatusDict = new ConcurrentDictionary<string, VmConnectionSessionDto>();
        // Rate limiting for TriggerMessage: Dictionary<"deviceId:messageType", lastTriggerTime>
        private static ConcurrentDictionary<string, DateTime> _triggerMessageRateLimits = new ConcurrentDictionary<string, DateTime>();
        private const int TriggerMessageRateLimitSeconds = 10;
        private const int TriggerMessageBootNotificationRateLimitSeconds = 120;
        private static readonly ThreadLocal<Random> _randomGenerator = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        // NON-BLOCKING RECONNECT FIX: Track in-progress reconnect operations per device
        private readonly ConcurrentDictionary<string, bool> _serverReconnectInProgress = new ConcurrentDictionary<string, bool>();
        private readonly ConcurrentDictionary<string, bool> _cpoReconnectInProgress = new ConcurrentDictionary<string, bool>();
        // Retry/Reconnect configuration
        private const int MaxRetryAttempts = 5;
        private const int InitialRetryDelayMs = 1000;
        private const int MaxRetryDelayMs = 30000;
        private const int ConnectionMonitorIntervalMs = 30000;
        private const string Protocol_OCPP16 = "ocpp1.6";
        private const string Protocol_OCPP16J = "ocpp1.6j";

        public VmConnectionService(IMapper mapper,
                            ICustomHttpUtilService customHttpUtilService,
                             ILogger<VmConnectionService> logger,
                             IServiceProvider services,
                             IConfiguration configuration) : base(mapper)
        {
            _customHttpUtilService = customHttpUtilService;
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }

        /// <summary>
        /// Sends OCPP message to device (public interface implementation)
        /// </summary>
        public async Task SendMessageToDevice(VmConnectionSessionDto vmConnectionSession, string ocppMessage,
            Ocpp16MessageDto ocpp16MessagePayload, VmCommandMessageTopicEnum? vmCommandMessageTopic)
        {
            await SendMessageToDevice(vmConnectionSession, ocppMessage);
        }

        /// <summary>
        /// socket Baglantisi saglanıyor - ana entry point
        /// </summary>
        public async Task ConnectionDevice(string identifier)
        {
            VmConnectionSessionDto vmConnectionSession = null;
            try
            {
                if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.IsWebSocketRequest)
                {
                    VmParameter vmParameter = null;
                    using (var scope = _services.CreateScope())
                    {
                        IVmParameterRepository vmParameterRepository = scope.ServiceProvider.GetRequiredService<IVmParameterRepository>();
                        vmParameter = await vmParameterRepository.GetVmParameter(new VmParameterFilterDto() { }, null).FirstOrDefaultAsync();
                    }
                    if (vmParameter != null)
                    {
                        if (vmParameter.IsVmActive)
                        {
                            GetOrAddVmDeviceConnectionResponseDto? vmDeviceConnection = null;
                            GetOrAddVmDeviceConnectionRequestDto getOrAddVmDeviceConnectionRequest = new GetOrAddVmDeviceConnectionRequestDto();
                            getOrAddVmDeviceConnectionRequest.Identifier = identifier;
                            getOrAddVmDeviceConnectionRequest.Date = DateTime.UtcNow;
                            getOrAddVmDeviceConnectionRequest.ProtocolList = _customHttpUtilService.GetHttpContext().HttpContext.WebSockets.WebSocketRequestedProtocols.ToList();
                            using (var scope = _services.CreateScope())
                            {
                                IVmDeviceConnectionManagementService vmDeviceConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmDeviceConnectionManagementService>();
                                vmDeviceConnection = await vmDeviceConnectionManagementService.GetOrAddVmDeviceConnection(getOrAddVmDeviceConnectionRequest);
                            }
                            if (vmDeviceConnection != null)
                            {
                                await SaveVmCommandMessage(vmDeviceConnection.Id, identifier);
                                string subProtocol = GetSocketSubProtocol();
                                vmConnectionSession = InitVmConnectionSession(identifier, vmDeviceConnection, subProtocol);
                                bool statusSuccess = await UpdateVmDictionaryForDevice(vmConnectionSession, identifier);
                                if (statusSuccess)
                                {
                                    _logger.LogInformation("Device connected successfully: {Identifier}", identifier);
                                    _ = Task.Run(async () => { try { await RunStartupReconciliationAsync(); } catch (Exception rex) { _logger.LogError(rex, "[RECONCILIATION] Startup scan failed"); } });
                                    using (WebSocket webSocket = await _customHttpUtilService.GetHttpContext().HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol))
                                    {
                                        vmConnectionSession.DeviceWebSocket = webSocket;
                                        byte[] bufferDevice = new byte[1024 * 4];
                                        using (MemoryStream memStream = new MemoryStream(bufferDevice.Length))
                                        {
                                            await MainProcess(vmConnectionSession, vmDeviceConnection.VmDevice.VmStation.VmCpo.OcppUrlAddress, vmParameter.ServerUrl, memStream, bufferDevice);
                                        }
                                    }
                                }
                                else
                                {
                                    await UpdateDeviceConnectionDb(vmConnectionSession, false, null);
                                    await DeviceCannotStartConnect(identifier);
                                }
                            }
                        }
                    }
                }
                else
                {
                    await DeviceCannotStartConnect(identifier);
                }
            }
            catch (Exception ex)
            {
                await DeviceCannotStartConnectWithException(identifier, ex);
            }
        }

        private async Task ProcessIncomingDeviceMessage(
            VmConnectionSessionDto vmConnectionSession,
            byte[] buffer,
            MemoryStream memoryStreamDevice,
            Uri uriServer,
            Uri uriCpo)
        {
            byte[] bMessageDevice = ProcessMemoryStream(buffer, ref memoryStreamDevice);
            var deviceMessageStr = UTF8Encoding.UTF8.GetString(bMessageDevice);
            JArray messageList = JArray.Parse(deviceMessageStr);
            string messageType = "";
            string messageId = "";
            JToken payloadToken = null;
            string action = "";

            // HEARTBEAT FIX: Any valid message from device indicates it's alive
            vmConnectionSession.LastHeartbeatTime = DateTime.UtcNow;
            vmConnectionSession.MissedHeartbeatCount = 0;

            if (messageList.Count == 4)
            {
                messageType = messageList[0].ToString();
                messageId = messageList[1].ToString();
                action = messageList[2].ToString();
                payloadToken = messageList[3];
                if (messageType == "2")
                    await ProcessDeviceMessages(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
                else if (messageType == "3" || messageType == "4")
                    await ProcessDeviceResponseMessages(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
                await SaveDeviceCommandMessageDb(vmConnectionSession, messageType, messageId, payloadToken?.ToString() ?? "", action);
            }
            else
            {
                messageType = messageList[0].ToString();
                messageId = messageList[1].ToString();
                payloadToken = messageList[2];
                if (messageType == "2")
                    await ProcessDeviceMessages(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
                else if (messageType == "3" || messageType == "4")
                    await ProcessDeviceResponseMessages(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
                await SaveDeviceCommandMessageDb(vmConnectionSession, messageType, messageId, payloadToken?.ToString() ?? "", action);
            }
        }

        private async Task MainProcess(VmConnectionSessionDto vmConnectionSession,
            string ocppUrlAddress, string serverUrlAddress,
            MemoryStream memoryStreamDevice, byte[] buffer)
        {
            try
            {
                var (uriServer, hasValidServerUrl) = await SetupServerConnection(vmConnectionSession, serverUrlAddress);
                var (uriCpo, hasValidCpoUrl) = await SetupCpoConnection(vmConnectionSession, ocppUrlAddress);
                int firstConnection = 1;
                while (vmConnectionSession.DeviceWebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult resultDevice;
                    try
                    {
                        resultDevice = await vmConnectionSession.DeviceWebSocket.ReceiveAsync(buffer, vmConnectionSession.DeviceCancellationTokenSource.Token);
                    }
                    catch (WebSocketException wsEx)
                    {
                        _logger.LogInformation($"Device WebSocket connection lost for {vmConnectionSession.Identifier}. Reason: {wsEx.Message}");
                        break;
                    }
                    catch (ObjectDisposedException ex) when (ex.Message.Contains("CancellationTokenSource"))
                    {
                        _logger.LogInformation($"CancellationToken disposed during Device ReceiveAsync for {vmConnectionSession.Identifier}");
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation($"Device ReceiveAsync cancelled for {vmConnectionSession.Identifier}");
                        break;
                    }

                    if (resultDevice != null && resultDevice.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogWarning($"Device WebSocket closed for {vmConnectionSession.Identifier}. CloseStatus: {resultDevice.CloseStatus}");
                    }

                    if (resultDevice != null && resultDevice.MessageType != WebSocketMessageType.Close)
                    {
                        memoryStreamDevice.Write(buffer, 0, resultDevice.Count);
                        CheckAndReconnectServer(vmConnectionSession, uriServer, hasValidServerUrl);
                        if (hasValidServerUrl) EnsureServerListenTaskStarted(vmConnectionSession);
                        CheckAndReconnectCpo(vmConnectionSession, uriCpo, hasValidCpoUrl);
                        if (hasValidCpoUrl && uriCpo != null) EnsureCpoListenTaskStarted(vmConnectionSession, uriCpo);
                        await EnsureHeartbeatMonitorStarted(vmConnectionSession);
                        await EnsureConnectionHealthMonitorStarted(vmConnectionSession, serverUrlAddress, ocppUrlAddress);
                        if (resultDevice.EndOfMessage)
                        {
                            try
                            {
                                await ProcessIncomingDeviceMessage(vmConnectionSession, buffer, memoryStreamDevice, uriServer, uriCpo);
                            }
                            catch (Exception msgEx)
                            {
                                _logger.LogError(msgEx, $"[MESSAGE_GUARD] Failed to process device message for {vmConnectionSession.Identifier} — skipping, connection preserved");
                            }
                        }
                        if (Interlocked.CompareExchange(ref firstConnection, 0, 1) == 1)
                        {
                            await UpdateDeviceConnectionDb(vmConnectionSession, true, ChargeDeviceInstantStateEnum.AVAILABLE);
                        }
                    }
                    else
                    {
                        await DeviceDisconnected(vmConnectionSession);
                        break;
                    }
                }
                await DeviceDisconnected(vmConnectionSession);
            }
            catch (Exception exceptionDevice)
            {
                await DeviceDisconnectedWithException(vmConnectionSession, exceptionDevice);
            }
        }

        private async Task ProcessDeviceMessages(VmConnectionSessionDto vmConnectionSession, Uri uriServer, Uri uriCpo, string messageType, string messageId, JToken payloadToken, string action)
        {
            if (action == OcppFromChargePointActionTypeEnum.METER_VALUES.ToDescriptionString())
            {
                await SendMeterValuesResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendMeterValuesToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.STOP_TRANSACTION.ToDescriptionString())
            {
                await SendStopTransactionResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendStopTransactionToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.BOOT_NOTIFICATION.ToDescriptionString())
            {
                await SendBootNotificationResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendBootNotificationToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.HEARTBEAT.ToDescriptionString())
            {
                await SendHeartbeatResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendHeartbeatToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.AUTHORIZE.ToDescriptionString())
            {
                // AUTOCHARGE FIX: Don't send immediate response — wait for CPO/Server responses
                await SendAuthorizeToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.START_TRANSACTION.ToDescriptionString())
            {
                _logger.LogInformation($"StartTransaction received from device {vmConnectionSession.Identifier}, TransactionTarget={vmConnectionSession.TransactionTarget}");
                if (vmConnectionSession.TransactionTarget == TransactionTargetEnum.VM_ONLY)
                {
                    await SendStartTransactionResponseToDevice(vmConnectionSession, messageId, payloadToken);
                }
                else
                {
                    await HandleStartTransactionWithExternalWait(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
                }
            }
            else if (action == OcppFromChargePointActionTypeEnum.STATUS_NOTIFICATION.ToDescriptionString())
            {
                await SendStatusNotificationResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendStatusNotificationToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.DATA_TRANSFER.ToDescriptionString())
            {
                await SendDataTransferResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendDataTransferToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.DIAGNOSTIC_STATUS_NOTIFICATION.ToDescriptionString())
            {
                await SendDiagnosticsStatusNotificationResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendDiagnosticsStatusNotificationToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
            else if (action == OcppFromChargePointActionTypeEnum.FIRMWARE_STATUS_NOTIFICATION.ToDescriptionString())
            {
                await SendFirmwareStatusNotificationResponseToDevice(vmConnectionSession, messageId, payloadToken);
                await SendFirmwareStatusNotificationToCpoAndServer(vmConnectionSession, uriServer, uriCpo, messageType, messageId, payloadToken, action);
            }
        }
    }
}
