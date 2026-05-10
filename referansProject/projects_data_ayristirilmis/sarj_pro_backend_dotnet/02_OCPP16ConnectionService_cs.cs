// KAYNAK: E:\Projeler\Backend\SarjAllPro\src\Core\Applications\Ocpp.Application\Services\Ocpp16\Ocpp16Connection\Ocpp16ConnectionService.cs
// SarjAllPro - OCPP 1.6 bağlantı servisi
// FARK VM'den: Tek bağlantı (device->VM), VM gibi üçgen mimari YOK (CPO+Server yok)
// Dictionary<long, DeviceSessionStatusDto> — ConcurrentDictionary değil (basit lock ile)

using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.Utils.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Domain.Dto.OcppDto.DeviceConnectionDtos;
using Shared.Domain.Dto.OcppDto.Ocpp16Dtos.Ocpp16MessageDtos;
using Shared.Domain.Dto.OcppDto.Ocpp16Dtos.Ocpp16RemoteTriggerMessageDtos;
using Shared.Domain.Dto.OcppDto.OcppCommandMessageDtos;
using Shared.Domain.Dto.OcppDto.OcppRequestDtos;
using Shared.Domain.Entities.OcppEntities.CommandMessageModule;
using Shared.Domain.Entities.OcppEntities.DeviceConnectionModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.OcppEnums;
using Shared.Domain.Errors.StationApiErrors.Occp16;
using Shared.Domain.Repositories.OcppRepositories.CommandMessageRepositoryInterfaces;
using Shared.Domain.Repositories.OcppRepositories.DeviceConnectionRepositoryInterfaces;
using Shared.Domain.Services.OcppServices.CommandMessageInterfaces;
using Shared.Domain.Services.OcppServices.DeviceConnectionInterfaces;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ConnectionInterfaces;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16HandleInterfaces;
using Shared.Domain.Services.OcppServices.OcppRequestServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ocpp.Application.Services.Ocpp16.Ocpp16Connection
{
    public class Ocpp16ConnectionService : BaseService, IOcpp16ConnectionService
    {
        // Dictionary ile status takibi (VM'den farklı: ConcurrentDictionary değil, lock ile yönetilen Dictionary)
        public static Dictionary<long, DeviceSessionStatusDto> _deviceSessionStatusDict = new Dictionary<long, DeviceSessionStatusDto>();
        private const string Protocol_OCPP16 = "ocpp1.6";
        private const string Protocol_OCPP16J = "ocpp1.6j";
        private static string MessageRegExp = "^\\[\\s*(\\d)\\s*,\\s*\"([^\"]*)\"\\s*,(?:\\s*\"(\\w*)\"\\s*,)?\\s*(.*)\\s*\\]$";
        // Asenkron API çağrıları için request kuyruğu
        private Dictionary<string, Ocpp16MessageDto> _requestQueue = new Dictionary<string, Ocpp16MessageDto>();
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IServiceProvider _services;
        private readonly ILogger<Ocpp16ConnectionService> _logger;

        public Ocpp16ConnectionService(IMapper mapper, ICustomHttpUtilService customHttpUtilService,
                             ILogger<Ocpp16ConnectionService> logger, IServiceProvider services) : base(mapper)
        {
            _customHttpUtilService = customHttpUtilService;
            _logger = logger;
            _services = services;
        }

        /// <summary>
        /// socket Baglantisi saglanıyor
        /// </summary>
        public async Task Connection(string identifier, OcppDeviceTypeEnum occpDeviceType)
        {
            var date = DateTime.Now;
            _logger.LogError("connection: " + identifier);
            DeviceSessionStatusDto deviceSessionStatus = null;
            if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.IsWebSocketRequest)
            {
                DeviceConnectionDto deviceConnection = null;
                try
                {
                    await SaveCommandMessage(identifier, occpDeviceType, OcppMessageTypeEnum.RECEIVED, date);
                    deviceConnection = await GetDeviceconnection(identifier, deviceConnection, date);
                    if (deviceConnection != null)
                    {
                        string subProtocol = GetSocketSubProtocol();
                        deviceSessionStatus = SetDeviceSessionStatus(occpDeviceType, deviceConnection, subProtocol);
                        bool statusSuccess = await UpdateChargePointDictionary(deviceSessionStatus, deviceConnection);
                        if (statusSuccess)
                        {
                            using (WebSocket webSocket = await _customHttpUtilService.GetHttpContext().HttpContext.WebSockets.AcceptWebSocketAsync(subProtocol))
                            {
                                deviceSessionStatus.WebSocket = webSocket;
                                await Receive(deviceSessionStatus, occpDeviceType);
                            }
                        }
                        else
                        {
                            await InvalidWebSocketRequest(identifier, occpDeviceType, date);
                        }
                    }
                    else
                    {
                        await OcppError(Occp16ConnectionErrorEnum.CHARGE_POINT_NOT_FOUND, StatusCodes.Status500InternalServerError);
                    }
                }
                catch (Exception ex)
                {
                    if (deviceSessionStatus != null && deviceSessionStatus.WebSocket != null)
                        _logger.LogError("hata: " + ex.Message + " , " + deviceSessionStatus.WebSocket.State);
                }
            }
            else
            {
                await InvalidWebSocketRequest(identifier, occpDeviceType, date);
            }
        }

        private async Task<DeviceConnectionDto> GetDeviceconnection(string identifier, DeviceConnectionDto deviceConnection, DateTime date)
        {
            using (var scope = _services.CreateScope())
            {
                var deviceConnectionService = scope.ServiceProvider.GetRequiredService<IDeviceConnectionService>();
                var addDeviceConnectionResponse = await deviceConnectionService.GetOrAddDeviceConnection(
                    new GetOrAddDeviceConnectionRequestDto() { Identifier = identifier, Date = date });
                if (addDeviceConnectionResponse.ResultType == ResultType.Ok)
                    deviceConnection = addDeviceConnectionResponse.Data.DeviceConnection;
            }
            return deviceConnection;
        }

        private async Task Receive(DeviceSessionStatusDto deviceSessionStatus, OcppDeviceTypeEnum occpDeviceType)
        {
            byte[] buffer = new byte[1024 * 4];
            MemoryStream memStream = new MemoryStream(buffer.Length);
            try
            {
                var datetimeNow = DateTime.Now;
                var firstConnection = true;
                while (deviceSessionStatus.WebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await deviceSessionStatus.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    datetimeNow = DateTime.Now;
                    if (result != null && result.MessageType != WebSocketMessageType.Close)
                    {
                        memStream.Write(buffer, 0, result.Count);
                        if (result.EndOfMessage)
                        {
                            byte[] bMessage = processMemoryStream(buffer, ref memStream);
                            string ocppMessage;
                            List<object> data;
                            getDataFromMessage(bMessage, out ocppMessage, out data);
                            if (data.Count >= 4)
                            {
                                string messageTypeId = data[0] + "";
                                string uniqueId = data[1] + "";
                                string action = data[2] + "";
                                string jsonPaylod = data[3] + "";
                                Ocpp16MessageDto msgIn = new Ocpp16MessageDto(messageTypeId, uniqueId, action, jsonPaylod);
                                await ProcessOcppCommand(deviceSessionStatus, occpDeviceType, datetimeNow, firstConnection, ocppMessage, uniqueId, msgIn, datetimeNow);
                                if (msgIn.MessageType == "2")
                                {
                                    Ocpp16MessageDto msgOut = new Ocpp16MessageDto();
                                    using (var scope = _services.CreateScope())
                                    {
                                        var processRequestAndResponseService = scope.ServiceProvider.GetRequiredService<IOcpp16ProcessRequestAndResponseService>();
                                        msgOut = await processRequestAndResponseService.ProcessRequest(msgIn, deviceSessionStatus, datetimeNow);
                                    }
                                    await SendOcppMessage(msgOut, deviceSessionStatus, null, datetimeNow);
                                }
                                else if (msgIn.MessageType == "3" || msgIn.MessageType == "4")
                                {
                                    // Cihaza gönderilen komutlara gelen cevaplar karşılanıyor
                                    if (_requestQueue.ContainsKey(msgIn.UniqueId))
                                    {
                                        ProcessAnswer(msgIn, _requestQueue[msgIn.UniqueId]);
                                        _requestQueue.Remove(msgIn.UniqueId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        await ClosingSocketConnection(deviceSessionStatus);
                        await UpdateDeviceConnection(deviceSessionStatus, null, datetimeNow, datetimeNow, null, null, false, true);
                    }
                    firstConnection = false;
                }
            }
            catch (Exception exception)
            {
                var datetimeNow = DateTime.Now;
                await CloseSocketConnectionException(deviceSessionStatus, exception);
                await UpdateDeviceConnection(deviceSessionStatus, null, datetimeNow, datetimeNow, null, null, false, true);
            }
        }

        public void ProcessAnswer(Ocpp16MessageDto msgIn, Ocpp16MessageDto msgOut)
        {
            using (var scope = _services.CreateScope())
            {
                var handleService = scope.ServiceProvider.GetRequiredService<IOcpp16HandleService>();
                switch (msgOut.Action)
                {
                    case "Reset": handleService.HandleReset(msgIn, msgOut); break;
                    case "UnlockConnector": handleService.HandleUnlockConnector(msgIn, msgOut); break;
                    case "RemoteStartTransaction": handleService.HandleRemoteStartTransaction(msgIn, msgOut); break;
                    default: break;
                }
            }
        }

        public async Task SendOcppMessage(Ocpp16MessageDto msg, DeviceSessionStatusDto deviceSessionStatus, OcppRequestTypeEnum? ocppRequestTypeEnum, DateTime date)
        {
            string ocppTextMessage = null;
            if (string.IsNullOrEmpty(msg.ErrorCode))
            {
                if (msg.MessageType == "2")
                {
                    if (ocppRequestTypeEnum != null) await SaveOcppRequest(msg, deviceSessionStatus, ocppRequestTypeEnum);
                    msg.TaskCompletionSource = new TaskCompletionSource<string>();
                    _requestQueue.Add(msg.UniqueId, msg);
                    ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]", msg.MessageType, msg.UniqueId, msg.Action, msg.JsonPayload);
                }
                else
                {
                    ocppTextMessage = string.Format("[{0},\"{1}\",{2}]", msg.MessageType, msg.UniqueId, msg.JsonPayload);
                }
            }
            else
            {
                ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]", msg.MessageType, msg.UniqueId, msg.ErrorCode, msg.ErrorDescription, "{}");
            }
            if (string.IsNullOrEmpty(ocppTextMessage))
            {
                ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]", "4", string.Empty, Ocpp16ErrorCodes.ProtocolError, string.Empty, "{}");
            }
            try { await SaveCommandMessage(deviceSessionStatus.Id, deviceSessionStatus.Identifier, deviceSessionStatus.DeviceType, OcppMessageTypeEnum.SENDER, msg.JsonPayload, null, date); }
            catch (Exception ee) { }
            byte[] binaryMessage = UTF8Encoding.UTF8.GetBytes(ocppTextMessage);
            if (deviceSessionStatus.WebSocket.State == WebSocketState.Open || deviceSessionStatus.WebSocket.State == WebSocketState.Connecting)
            {
                await deviceSessionStatus.WebSocket.SendAsync(new ArraySegment<byte>(binaryMessage, 0, binaryMessage.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task PushTriggerMessage(long deviceConnectionId, DateTime date)
        {
            DeviceSessionStatusDto? deviceSessionStatus;
            lock (_deviceSessionStatusDict)
            {
                deviceSessionStatus = _deviceSessionStatusDict.ContainsKey(deviceConnectionId)
                    ? _deviceSessionStatusDict[deviceConnectionId] : null;
            };
            if (deviceSessionStatus != null)
            {
                Ocpp16MessageDto msg = new Ocpp16MessageDto();
                Ocpp16TriggerMessageDto ocpp16TriggerMessage = new Ocpp16TriggerMessageDto();
                ocpp16TriggerMessage.requestedMessage = MessageTriggerTypeEnum.HEARTBEAT.ToDescriptionString();
                ocpp16TriggerMessage.connectorId = 1;
                msg.UniqueId = Guid.NewGuid() + "";
                msg.Action = "TriggerMessage";
                msg.MessageType = "2";
                msg.JsonPayload = JsonConvert.SerializeObject(ocpp16TriggerMessage);
                await SendOcppMessageTrigger(msg, deviceSessionStatus, date);
            }
        }

        // Yardımcı metodlar...
        private string GetSocketSubProtocol()
        {
            string subProtocol = null;
            if (!_customHttpUtilService.GetHttpContext().HttpContext.Response.HasStarted)
            {
                if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.WebSocketRequestedProtocols.Contains(Protocol_OCPP16))
                    subProtocol = Protocol_OCPP16;
                else if (_customHttpUtilService.GetHttpContext().HttpContext.WebSockets.WebSocketRequestedProtocols.Contains(Protocol_OCPP16J))
                    subProtocol = Protocol_OCPP16J;
            }
            return subProtocol;
        }

        private void getDataFromMessage(byte[] bMessage, out string ocppMessage, out List<object> data)
        {
            ocppMessage = UTF8Encoding.UTF8.GetString(bMessage);
            data = JsonConvert.DeserializeObject<List<object>>(ocppMessage);
        }

        private byte[] processMemoryStream(byte[] buffer, ref MemoryStream memStream)
        {
            byte[] bMessage = memStream.ToArray();
            memStream = new MemoryStream(buffer.Length);
            return bMessage;
        }

        private async Task<bool> UpdateChargePointDictionary(DeviceSessionStatusDto deviceSessionStatus, DeviceConnectionDto deviceConnection)
        {
            bool statusSuccess = false;
            DeviceSessionStatusDto? dummydeviceSessionStatus = null;
            try
            {
                statusSuccess = UpdateStatusDict(deviceSessionStatus, deviceConnection, ref dummydeviceSessionStatus);
                await CloseOldConnection(dummydeviceSessionStatus);
            }
            catch (Exception exp)
            {
                _logger.LogError("error: " + exp.Message);
                await OcppError(Occp16ConnectionErrorEnum.REFUSE_CONNECTİON, StatusCodes.Status500InternalServerError);
            }
            return statusSuccess;
        }

        private bool UpdateStatusDict(DeviceSessionStatusDto deviceSessionStatus, DeviceConnectionDto deviceConnection, ref DeviceSessionStatusDto dummydeviceSessionStatus)
        {
            bool statusSuccess;
            lock (_deviceSessionStatusDict)
            {
                _logger.LogError("count : " + _deviceSessionStatusDict.Count);
                if (_deviceSessionStatusDict.ContainsKey(deviceConnection.Id))
                    _deviceSessionStatusDict.Remove(deviceConnection.Id, out dummydeviceSessionStatus);
                _deviceSessionStatusDict.Add(deviceConnection.Id, deviceSessionStatus);
                statusSuccess = true;
            }
            return statusSuccess;
        }

        private async Task CloseOldConnection(DeviceSessionStatusDto dummydeviceSessionStatus)
        {
            if (dummydeviceSessionStatus?.WebSocket != null &&
                (dummydeviceSessionStatus.WebSocket.State == WebSocketState.Open || dummydeviceSessionStatus.WebSocket.State == WebSocketState.Connecting ||
                 dummydeviceSessionStatus.WebSocket.State == WebSocketState.CloseSent || dummydeviceSessionStatus.WebSocket.State == WebSocketState.CloseReceived))
            {
                if (dummydeviceSessionStatus.WebSocket.State == WebSocketState.Open || dummydeviceSessionStatus.WebSocket.State == WebSocketState.CloseReceived)
                    await dummydeviceSessionStatus.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                else if (dummydeviceSessionStatus.WebSocket.State == WebSocketState.Connecting)
                    dummydeviceSessionStatus.WebSocket.Abort();
            }
        }

        private async Task OcppError(Occp16ConnectionErrorEnum error, int statusCode)
        {
            var response = new ErrorResultWithoutData(error.ToDescriptionString());
            _customHttpUtilService.GetHttpContext().HttpContext.Response.ContentType = "application/json";
            _customHttpUtilService.GetHttpContext().HttpContext.Response.StatusCode = statusCode;
            await _customHttpUtilService.GetHttpContext().HttpContext.Response.WriteAsync(
                JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
}
