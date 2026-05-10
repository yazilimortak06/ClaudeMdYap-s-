// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Applications\Api.Application\Services\MobilApiServices\ChargeProcess\ChargeProcessService.cs
using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.ChargeDeviceConnectorDtos;
using Shared.Domain.Dto.ApiDto.ChargeDeviceDtos;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.ChargingConnectorReadingDtos;
using Shared.Domain.Dto.ApiDto.DebitCardDtos;
using Shared.Domain.Dto.ApiDto.MobilChargeDeviceConnectorDtos;
using Shared.Domain.Dto.ApiDto.MobilChargeDtos;
using Shared.Domain.Dto.ApiDto.UserAddressDtos;
using Shared.Domain.Dto.NotificationDto.ConnectorConnectionDtos;
using Shared.Domain.Dto.OcppDto.Ocpp16StationRemoteTransactionDtos;
using Shared.Domain.Dto.OcppDto.SocketMovementDtos;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceModule;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.OcppEnums;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.Errors.OcppErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.NotificationApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.OcppApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceConnectorRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.DebitCardRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserAddressRepositories;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.ChargeProcessServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Application.Services.MobilApiServices.ChargeProcess
{
    public class ChargeProcessService : BaseService, IChargeProcessService
    {
        private readonly IChargeDeviceConnectorRepository _chargeDeviceConnectorRepository;
        private readonly IChargeDeviceRepository _chargeDeviceRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IDebitCardRepository _debitCardRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IChargingConnectorReadingRepository _chargingConnectorReadingRepository;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IRemoteTransactionClientService _remoteTransactionClientService;
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IConnectorConnectionClientService _connectorConnectionClientService;
        private readonly IDeviceSocketNotificationClientService _deviceSocketNotificationClientService;
        private readonly ISocketMovementClientService _socketMovementClientService;
        private readonly ILogger<ChargeProcessService> _logger;

        public ChargeProcessService(IMapper mapper,
                            IConfiguration configuration,
                            IUtilService utilService,
                            IUserContextProvider userContextProvider,
                           ILogger<ChargeProcessService> logger,
                           ICustomHttpUtilService customHttpUtilService,
                           IChargeDeviceConnectorRepository chargeDeviceConnectorRepository,
                           IConnectorConnectionClientService connectorConnectionClientService,
                           IDeviceSocketNotificationClientService deviceSocketNotificationClientService,
                           IChargeDeviceRepository chargeDeviceRepository,
                           ISocketMovementClientService socketMovementClientService,
                           IRemoteTransactionClientService remoteTransactionClientService,
                           IUserAddressRepository userAddressRepository,
                           IChargeRepository chargeRepository,
                           IDebitCardRepository debitCardRepository,
                           IChargingConnectorReadingRepository chargingConnectorReadingRepository) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _utilService = utilService;
            _userContextProvider = userContextProvider;
            _logger = logger;
            _customHttpUtilService = customHttpUtilService;
            _chargeDeviceConnectorRepository = chargeDeviceConnectorRepository;
            _connectorConnectionClientService = connectorConnectionClientService;
            _deviceSocketNotificationClientService = deviceSocketNotificationClientService;
            _chargeDeviceRepository = chargeDeviceRepository;
            _socketMovementClientService = socketMovementClientService;
            _remoteTransactionClientService = remoteTransactionClientService;
            _userAddressRepository = userAddressRepository;
            _chargeRepository = chargeRepository;
            _debitCardRepository = debitCardRepository;
            _chargingConnectorReadingRepository = chargingConnectorReadingRepository;
        }
        /// <summary>
        /// kullanıcının devam eden sarj işlemi mevcut mu kontrol ediliyor
        /// </summary>
        public async Task<Result<CheckChargeProcessResponseDto>> CheckChargeProcess(CheckChargeProcessRequestDto checkChargeProcessRequest)
        {
            CheckChargeProcessResponseDto checkChargeProcessResponse = new CheckChargeProcessResponseDto();
            checkChargeProcessResponse.IsExist = false;
            checkChargeProcessResponse.IsAvailableForStopCharge = false;
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(checkChargeProcessRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.Finished = false;
                });
            });
            var charge = await _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)).OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (charge != null)
            {
                checkChargeProcessResponse.ChargeGuiId = charge.GuiId;
                checkChargeProcessResponse.IsExist = true;
                checkChargeProcessResponse.CalculatedPrice = charge.CalculatedPrice.GetValueOrDefault();
                checkChargeProcessResponse.LoadedKw = charge.LoadedKw.GetValueOrDefault();
                checkChargeProcessResponse.ChargePercentage = (int?)charge.ChargePercentage;
                checkChargeProcessResponse.State = charge.State;
                checkChargeProcessResponse.StartDate = charge.StartTime != null ? charge.StartTime.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : null;
                checkChargeProcessResponse.EndDate = charge.LastUpdateDate != null ? charge.LastUpdateDate.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : null;
                checkChargeProcessResponse.StartDateLong = charge.StartTime;
                checkChargeProcessResponse.EndDateLong = charge.LastUpdateDate;
                if (charge.ChargeFirmType == ChargeFirmTypeEnum.ROTAWATT)
                {
                    string lastSocketMovementGuiId = "";
                    GetLastSocketMovementRequestDto getLastSocketMovementRequest = new GetLastSocketMovementRequestDto();
                    getLastSocketMovementRequest.ConnectorNo = charge.ChargeDeviceConnector.ConnectorNo;
                    getLastSocketMovementRequest.DeviceIdentifier = charge.ChargeDeviceConnector.ChargeDevice.Identifier;
                    var getLastSocketMovementResponse = await _socketMovementClientService.GetLastSocketMovement(getLastSocketMovementRequest);
                    if (getLastSocketMovementResponse.ResultType == ResultType.Ok)
                    {
                        lastSocketMovementGuiId = getLastSocketMovementResponse.Data.GuiId;
                    }
                    if (charge.PluggedSocketMovementGuiId == lastSocketMovementGuiId)
                    {
                        checkChargeProcessResponse.ConnectionId = _userContextProvider.ConnectionId;
                        if (charge.State == ChargeStateEnum.PROCESS_START)
                        {
                            checkChargeProcessResponse.IsAvailableForStopCharge = true;
                        }
                    }
                    else
                    {
                        checkChargeProcessResponse.IsAvailableForStopCharge = true;
                    }
                }
                else if (charge.ChargeFirmType == ChargeFirmTypeEnum.FIRM)
                {
                    // firm den durdurulabilir mi şarj işlemi boolean olarak dönülcek
                }
                else
                {
                    return new ErrorResult<CheckChargeProcessResponseDto>(checkChargeProcessResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                }
            }
            else
            {
                ChargeFilterDto unPaidChargeFilter = new ChargeFilterDto();
                unPaidChargeFilter.UserId = _userContextProvider.UserId;
                unPaidChargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL };
                var unPaidCharge = await _chargeRepository.GetCharge(unPaidChargeFilter,null).FirstOrDefaultAsync();
                if (unPaidCharge != null)
                {
                    checkChargeProcessResponse.IsExist = true;
                    checkChargeProcessResponse.State = unPaidCharge.State;
                    checkChargeProcessResponse.ChargeGuiId = unPaidCharge.GuiId;
                }
            }
            return new SuccessResult<CheckChargeProcessResponseDto>(checkChargeProcessResponse);
        }
        /// <summary>
        /// şarj işlemi başlatılıyor
        /// </summary>
        public async Task<Result<StartChargeResponseDto>> StartCharge(StartChargeRequestDto startChargeRequest)
        {
            StartChargeResponseDto startChargeResponse = new StartChargeResponseDto();
            var datetimeNow = DateTime.Now;
            ChargingConnectorReadingFilterDto chargingConnectorReadingFilter = _mapper.Map<ChargingConnectorReadingFilterDto>(startChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargingConnectorReadingFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            ChargingConnectorReading chargingConnectorReading = await _chargingConnectorReadingRepository.GetChargingConnectorReadingAsNoTracking(chargingConnectorReadingFilter).FirstOrDefaultAsync();
            if (chargingConnectorReading != null)
            {
                if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Station.IsActive)
                {
                    if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.State == ChargeDeviceStateEnum.ACTIVE)
                    {
                        if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.ConnectionState)
                        {
                            if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.InstantState == ChargeDeviceInstantStateEnum.AVAILABLE)
                            {
                                if (chargingConnectorReading.ChargeDeviceConnector.State == ChargeDeviceConnectorStateEnum.ACTIVE)
                                {
                                    if (chargingConnectorReading.ChargeDeviceConnector.InstantState == ChargeDeviceConnectorInstantStateEnum.PREPARING)
                                    {
                                        if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Station.StationFirmType == StationFirmTypeEnum.ROTAWATT)
                                        {
                                            GetLastSocketMovementRequestDto getLastSocketMovementRequest = new GetLastSocketMovementRequestDto();
                                            getLastSocketMovementRequest.ConnectorNo = chargingConnectorReading.ChargeDeviceConnector.ConnectorNo;
                                            getLastSocketMovementRequest.DeviceIdentifier = chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Identifier;
                                            var getLastSocketMovementResponse = await _socketMovementClientService.GetLastSocketMovement(getLastSocketMovementRequest);
                                            if (getLastSocketMovementResponse.ResultType == ResultType.Ok)
                                            {
                                                if (getLastSocketMovementResponse.Data.SocketMovementState != SocketMovementStateEnum.PLUGGED)
                                                {
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
                                                }
                                                if (getLastSocketMovementResponse.Data.GuiId != chargingConnectorReading.PluggedSocketMovementGuiId)
                                                {
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_MOVEMENT_DOES_NOT_MATCH_WITH_LAST_SOCKET_MOVEMENT);
                                                }
                                                Charge charge;
                                                if (chargingConnectorReading.Charge != null)
                                                {
                                                    if (chargingConnectorReading.Charge.State == ChargeStateEnum.PAYMENT_BEING_RECEIVED ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.COMPLETED ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.CALCULATING ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.PAYMENT_FAIL)
                                                    {
                                                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
                                                    }
                                                    charge = chargingConnectorReading.Charge;
                                                }
                                                else
                                                {
                                                    charge = new Charge();
                                                    charge.GuiId = Guid.NewGuid() + "";
                                                    charge.ConnectionId = _userContextProvider.ConnectionId;
                                                    charge.CalculatedPrice = 0;
                                                    charge.ChargeDeviceConnectorId = chargingConnectorReading.ChargeDeviceConnectorId;
                                                    charge.Discount = 0;
                                                    charge.Kdv = chargingConnectorReading.ChargeDeviceConnector.Kdv;
                                                    charge.PaidPrice = 0;
                                                    charge.Price = 0;
                                                    charge.KwPrice = chargingConnectorReading.ChargeDeviceConnector.PriceWithKdv;
                                                    charge.CreatedDate = datetimeNow;
                                                    charge.LastUpdateDate = datetimeNow;
                                                    charge.UserId = _userContextProvider.UserId;
                                                    charge.ChargingConnectorReadingId = chargingConnectorReading.Id;
                                                    charge.State = ChargeStateEnum.PROCESS_STARTING;
                                                    charge.ChargeFirmType = ChargeFirmTypeEnum.ROTAWATT;
                                                    charge.PluggedSocketMovementGuiId = chargingConnectorReading.PluggedSocketMovementGuiId;
                                                    charge.FirmId = chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Station.FirmId;
                                                }
                                                Ocpp16StationRemoteStartTransactionRequestDto remoteStartTransactionRequest = new Ocpp16StationRemoteStartTransactionRequestDto();
                                                remoteStartTransactionRequest.ChargeGuiId = charge.GuiId;
                                                remoteStartTransactionRequest.ConnectorNo = chargingConnectorReading.ChargeDeviceConnector.ConnectorNo;
                                                remoteStartTransactionRequest.DeviceIdentifier = chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Identifier;
                                                var ocpp16RemoteStartTransactionResponse = await _remoteTransactionClientService.RemoteStartTransactionAsync(remoteStartTransactionRequest);
                                                if (ocpp16RemoteStartTransactionResponse.ResultType == ResultType.Ok)
                                                {
                                                    if (charge.Id == 0)
                                                    {
                                                        await _chargeRepository.InsertAsync(charge);
                                                        await _chargeRepository.SaveChangesAsync();
                                                    }
                                                    startChargeResponse.ChargeGuiId = charge.GuiId;
                                                    startChargeResponse.ConnectionId = _userContextProvider.ConnectionId;
                                                    return new SuccessResult<StartChargeResponseDto>(startChargeResponse);
                                                }
                                                else
                                                {
                                                    charge.State = ChargeStateEnum.FAILED;
                                                    charge.LastUpdateDate = DateTime.Now;
                                                    await _chargeRepository.InsertAsync(charge);
                                                    await _chargeRepository.SaveChangesAsync();
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CHARGE_FAILED_TO_START);
                                                }
                                            }
                                            else
                                            {
                                                return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_MOVEMENT_CAN_NOT_FOUND);
                                            }
                                        }
                                        else
                                        {
                                            return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                                        }
                                    }
                                    else
                                    {
                                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_REMOVED);
                                    }
                                }
                                else
                                {
                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CONNECTOR_IS_NOT_ACTIVE);
                                }
                            }
                            else
                            {
                                return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_AVAILABLE);
                            }
                        }
                        else
                        {
                            return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_CONNECTED);
                        }
                    }
                    else
                    {
                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_ACTIVE);
                    }
                }
                else
                {
                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.STATION_NOT_ACTIVE);
                }
            }
            else
            {
                return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
            }
        }
        /// <summary>
        /// şarj işlemi durduruluyor
        /// </summary>
        public async Task<Result<StopChargeResponseDto>> StopCharge(StopChargeRequestDto stopChargeRequest)
        {
            StopChargeResponseDto stopChargeResponse = new StopChargeResponseDto();
            var datetimeNow = DateTime.Now;
            ChargeFilterDto chargeFilter = new ChargeFilterDto();
            chargeFilter.GuiId = stopChargeRequest.ChargeGuiId;
            chargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.FAILED, ChargeStateEnum.PROCESS_ENDING,
                                                                                               ChargeStateEnum.PROCESS_START, ChargeStateEnum.PROCESS_STARTING };
            var charge = await _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice),false).FirstOrDefaultAsync();
            if (charge != null)
            {
                if (charge.ChargeFirmType == ChargeFirmTypeEnum.ROTAWATT)
                {
                    Ocpp16StationRemoteStopTransactionRequestDto ocpp16StationRemoteStopTransactionRequest = new Ocpp16StationRemoteStopTransactionRequestDto();
                    ocpp16StationRemoteStopTransactionRequest.ChargeGuiId = charge.GuiId;
                    ocpp16StationRemoteStopTransactionRequest.TransactionGuiId = charge.TransactionGuiId;
                    ocpp16StationRemoteStopTransactionRequest.DeviceIdentifier = charge.ChargeDeviceConnector.ChargeDevice.Identifier;
                    ocpp16StationRemoteStopTransactionRequest.ConnectorNo = charge.ChargeDeviceConnector.ConnectorNo;
                    ocpp16StationRemoteStopTransactionRequest.SocketMovementGuiId = charge.PluggedSocketMovementGuiId;
                    var stationRemoteStopTransactionResponse = await _remoteTransactionClientService.RemoteStopTransactionAsync(ocpp16StationRemoteStopTransactionRequest);
                    if (stationRemoteStopTransactionResponse.ResultType == ResultType.Ok && stationRemoteStopTransactionResponse.Data != null)
                    {
                        _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                            s => s.TransactionGuiId,
                            s => s.State,
                            s => s.LastUpdateDate,
                            s => s.CalculatedPrice,
                            s => s.LoadedKw,
                            s => s.ChargePercentage,
                            s => s.EndTime,
                        });
                        if (stationRemoteStopTransactionResponse.Data.TransactionGuiId != null)
                            charge.TransactionGuiId = stationRemoteStopTransactionResponse.Data.TransactionGuiId;
                        if (stationRemoteStopTransactionResponse.Data.ChargeState != null)
                            charge.State = stationRemoteStopTransactionResponse.Data.ChargeState.GetValueOrDefault();
                        if (stationRemoteStopTransactionResponse.Data.LoadedKw != null)
                        {
                            charge.LoadedKw = stationRemoteStopTransactionResponse.Data.LoadedKw;
                            charge.CalculatedPrice = (Math.Round(Convert.ToDecimal(charge.LoadedKw), 2) * charge.KwPrice);
                        }
                        await _chargeRepository.SaveChangesAsync();
                        stopChargeResponse = _mapper.Map<StopChargeResponseDto>(stationRemoteStopTransactionResponse.Data);
                        return new SuccessResult<StopChargeResponseDto>(stopChargeResponse);
                    }
                    else
                    {
                        return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.PROCESS_COULD_NOT_STOPPED);
                    }
                }
                else if (charge.ChargeFirmType == ChargeFirmTypeEnum.FIRM)
                {
                    return new SuccessResult<StopChargeResponseDto>(stopChargeResponse);
                }
                else
                {
                    return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                }
            }
            else
            {
                return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.CHARGE_CAN_NOT_FOUND);
            }
        }
    }
}
