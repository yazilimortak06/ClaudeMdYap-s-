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
        /// <param name="checkChargeProcessRequest"></param>
        /// <returns></returns>
        public async Task<Result<CheckChargeProcessResponseDto>> CheckChargeProcess(CheckChargeProcessRequestDto checkChargeProcessRequest)
        {
            #region response dto oluşturuluyor
            CheckChargeProcessResponseDto checkChargeProcessResponse = new CheckChargeProcessResponseDto();
            checkChargeProcessResponse.IsExist = false;
            checkChargeProcessResponse.IsAvailableForStopCharge = false;
            #endregion
            #region kullanıcının son yapılan chargeProcess işlemi getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(checkChargeProcessRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.Finished = false;
                });
            });
            #endregion
            //var charge = await _chargeRepository.GetChargeForCheckProcessAsNoTracking(chargeFilter).OrderBy(x => x.Id).FirstOrDefaultAsync();
            var charge = await _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)).OrderBy(x => x.Id).FirstOrDefaultAsync();
            #endregion
            if (charge != null)
            {
                #region response dto setleniyor
                checkChargeProcessResponse.ChargeGuiId = charge.GuiId;
                checkChargeProcessResponse.IsExist = true;
                checkChargeProcessResponse.CalculatedPrice = charge.CalculatedPrice.GetValueOrDefault();
                checkChargeProcessResponse.LoadedKw = charge.LoadedKw.GetValueOrDefault();
                checkChargeProcessResponse.ChargePercentage = (int?)charge.ChargePercentage;
                checkChargeProcessResponse.State = charge.State;
                checkChargeProcessResponse.StartDate = charge.StartTime != null ? charge.StartTime.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : null;
                checkChargeProcessResponse.EndDate = charge.LastUpdateDate != null ? charge.LastUpdateDate.GetValueOrDefault().ToString("dd-MM-yyyy HH:mm") : null; ;
                checkChargeProcessResponse.StartDateLong = charge.StartTime;
                checkChargeProcessResponse.EndDateLong = charge.LastUpdateDate;
                #endregion  
                #region şarjall bünyesinde şarj işlemi kontrol ediliyor
                if (charge.ChargeFirmType == ChargeFirmTypeEnum.ROTAWATT)
                {
                    #region şarj işleminin soket hareketi ile soketin son hareketi aynı mı kontrol ediliyor
                    string lastSocketMovementGuiId = "";
                    #region işlem henüz sonlandırılmamış veya işlem bitmiş fakat sistem de güncellenmemiş
                    #region ocpp apiye istek atılarak son soket hareket verisi alınıyor
                    #region request dto oluşturuluyor
                    GetLastSocketMovementRequestDto getLastSocketMovementRequest = new GetLastSocketMovementRequestDto();
                    getLastSocketMovementRequest.ConnectorNo = charge.ChargeDeviceConnector.ConnectorNo;
                    getLastSocketMovementRequest.DeviceIdentifier = charge.ChargeDeviceConnector.ChargeDevice.Identifier;
                    #endregion
                    var getLastSocketMovementResponse = await _socketMovementClientService.GetLastSocketMovement(getLastSocketMovementRequest);
                    #endregion
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
                    #endregion
                    #region işlem güncellenmemiş fakat soket hareketi değişmiş. O anki işlem durdurulabilir.
                    else
                    {
                        checkChargeProcessResponse.IsAvailableForStopCharge = true;
                    }
                    #endregion
                    #endregion
                }
                #endregion
                #region entegrasyon firma üzerinden şarj işlemi kontrol ediliyor
                else if (charge.ChargeFirmType == ChargeFirmTypeEnum.FIRM)
                {
                    // firm den durdurulabilir mi şarj işlemi boolean olarak dönülcek
                }
                #endregion
                #region geçersiz firma tipi
                else
                {
                    return new ErrorResult<CheckChargeProcessResponseDto>(checkChargeProcessResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                }
                #endregion
            }
            #region kullanıcının ödeme alınamadığı işlemi varsa getiriliyor
            else
            {
                #region filter dto oluşturuluyor
                ChargeFilterDto unPaidChargeFilter = new ChargeFilterDto();
                unPaidChargeFilter.UserId = _userContextProvider.UserId;
                unPaidChargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL };
                #endregion
                #region ödeme alınamadığı işlem çekiliyor
                var unPaidCharge = await _chargeRepository.GetCharge(unPaidChargeFilter,null).FirstOrDefaultAsync();
                #endregion
                #region ödeme alınamadığı işlem mevcut, response dto setleniyor
                if (unPaidCharge != null)
                {
                    checkChargeProcessResponse.IsExist = true;
                    checkChargeProcessResponse.State = unPaidCharge.State;
                    checkChargeProcessResponse.ChargeGuiId = unPaidCharge.GuiId;
                }
                #endregion
            }
            #endregion
            return new SuccessResult<CheckChargeProcessResponseDto>(checkChargeProcessResponse);
        }
        /// <summary>
        /// Cihaz seçiliyor
        /// </summary>
        /// <param name="selectDeviceRequest"></param>
        /// <returns></returns>
        public async Task<Result<SelectDeviceResponseDto>> SelectDevice(SelectDeviceRequestDto selectDeviceRequest)
        {
            #region response dto oluşturuluyor
            SelectDeviceResponseDto selectDeviceResponse = new SelectDeviceResponseDto();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region ödeme alınamamış şarj işlemi mevcut mu kontrol ediliyor
            #region filter dto oluşturuluyor
            var waitingChargeFilter = new ChargeFilterDto();
            waitingChargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL };
            waitingChargeFilter.UserId = _userContextProvider.UserId;
            #endregion
            var waitingCharge = await _chargeRepository.GetCharge(waitingChargeFilter,null).AnyAsync();
            #region ödenmemiş şarj işlemi mevcut
            if (waitingCharge)
            {
                return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.UNPAID_CHARGE_EXIST);
            }
            #endregion
            #endregion
            #region kayıtlı kredi kartı mevcut mu kontrol ediliyor
            var checkDebitCard = await _debitCardRepository.GetDebitCardAsNoTracking(new DebitCardFilterDto() { UserId = _userContextProvider.UserId }).AnyAsync();
            if (!checkDebitCard)
            {
                return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.FIRST_NEED_TO_ADD_A_CREDIT_CARD);
            }
            #endregion
            #region kayıtlı ve varsayılan fatura adresi mevcut mu kontrol ediliyor
            var checkUserAddress = await _userAddressRepository.GetUserAddressAsNoTracking(new UserAddressFilterDto() { UserId = _userContextProvider.UserId , IsDefault = true }).AnyAsync();
            if (!checkUserAddress)
            {
                return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.FIRST_NEED_TO_ADD_AN_DEFAULT_ADDRESS);
            }
            #endregion
            #region soket için işlem yapılıyor
            #region soket kontrol ediliyor
            #region filter dto oluşturuluyor
            ChargeDeviceConnectorFilterDto chargeDeviceConnectorFilter = _mapper.Map<ChargeDeviceConnectorFilterDto>(selectDeviceRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeDeviceConnectorFilterDto;
                    destData.DeviceState = ChargeDeviceStateEnum.ACTIVE;
                    destData.DeviceInstantState = ChargeDeviceInstantStateEnum.AVAILABLE;
                });
            });
            #endregion
            var checkChargeDeviceConnector = await _chargeDeviceConnectorRepository.GetChargeDeviceConnector(chargeDeviceConnectorFilter,null).AnyAsync();
            #endregion
            if (checkChargeDeviceConnector)
            {
                selectDeviceResponse.HasMultipleSocket = false;
                #region soket seçim işlemi yapılıyor
                #region request dto oluşturuluyor
                SelectSocketRequestDto selectSocketForChargeProcessRequest = _mapper.Map<SelectSocketRequestDto>(selectDeviceRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as SelectSocketRequestDto;
                        destData.ConnectorIdentifier = selectDeviceRequest.Identifier;
                    });
                });
                #endregion
                var selectSocketForChargeProcessResponse = await SelectSocket(selectSocketForChargeProcessRequest);
                if (selectSocketForChargeProcessResponse.ResultType == ResultType.Ok)
                {
                    #region response dto setleniyor
                    selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                    selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                    #endregion
                    return new SuccessResult<SelectDeviceResponseDto>(selectDeviceResponse);
                }
                #region soket seçimi başarısız
                else
                {
                    #region response dto setleniyor
                    selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                    #endregion
                    return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, (ChargeErrorEnum)selectSocketForChargeProcessResponse.ErrorCode);
                }
                #endregion
                #endregion
            }
            #endregion
            #region cihaz için işlem yapılıyor
            else
            {
                #region charge device çekiliyor
                #region filter dto oluşturuluyor
                ChargeDeviceFilterDto chargeDeviceFilter = _mapper.Map<ChargeDeviceFilterDto>(selectDeviceRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as ChargeDeviceFilterDto;
                        destData.DeviceState = ChargeDeviceStateEnum.ACTIVE;
                        destData.InstantState = ChargeDeviceInstantStateEnum.AVAILABLE;
                    });
                });
                #endregion
                //ChargeDevice chargeDevice = await _chargeDeviceRepository.GetChargeDevicesAsNoTracking(chargeDeviceFilter).FirstOrDefaultAsync();
                ChargeDevice chargeDevice = await _chargeDeviceRepository.GetChargeDevices(chargeDeviceFilter, include: source => source
         .Include(a => a.Station)
         .Include(a => a.ChargeDeviceConnector)).FirstOrDefaultAsync();
                #endregion
                if (chargeDevice != null)
                {
                    if (chargeDevice.ConnectionState)
                    {
                        if (chargeDevice.State == ChargeDeviceStateEnum.ACTIVE)
                        {
                            if (chargeDevice.Station.IsActive)
                            {
                                #region istasyon için açık saatler kontrol ediliyor

                                #endregion
                                #region tek soket varsa, soket seçili olup işlem yapılıyor
                                if (chargeDevice.ChargeDeviceConnector.Count == 1)
                                {
                                    selectDeviceResponse.HasMultipleSocket = false;
                                    #region soket seçim işlemi yapılıyor
                                    #region request dto oluşturuluyor

                                    SelectSocketRequestDto selectSocketForChargeProcessRequest = _mapper.Map<SelectSocketRequestDto>(selectDeviceRequest, opt =>
                                    {
                                        opt.AfterMap((src, dest) =>
                                        {
                                            var destData = dest as SelectSocketRequestDto;
                                            destData.ConnectorIdentifier = chargeDevice.ChargeDeviceConnector.FirstOrDefault().Identifier;
                                        });
                                    });
                                    #endregion
                                    var selectSocketForChargeProcessResponse = await SelectSocket(selectSocketForChargeProcessRequest);
                                    if (selectSocketForChargeProcessResponse.ResultType == ResultType.Ok)
                                    {
                                        #region response dto setleniyor
                                        selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                                        selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                                        #endregion
                                        return new SuccessResult<SelectDeviceResponseDto>(selectDeviceResponse);
                                    }
                                    #region soket seçimi başarısız
                                    else
                                    {
                                        #region response dto setleniyor
                                        selectDeviceResponse.ConnectorChargeReadingGuiId = selectSocketForChargeProcessResponse.Data.ConnectorChargeReadingGuiId;
                                        #endregion
                                        return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, (ChargeErrorEnum)selectSocketForChargeProcessResponse.ErrorCode);
                                    }
                                    #endregion
                                    #endregion
                                }
                                #endregion
                                #region birden fazla soket mevcut
                                else if (chargeDevice.ChargeDeviceConnector.Count > 1)
                                {
                                    selectDeviceResponse.HasMultipleSocket = true;
                                }
                                #endregion
                                #region soket bulunamadı
                                else
                                {
                                    return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.CONNECTOR_NOT_FOUNDED);
                                }
                                #endregion
                                return new SuccessResult<SelectDeviceResponseDto>(selectDeviceResponse);
                            }
                            #region istasyon aktif değil
                            else
                            {
                                return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.STATION_NOT_ACTIVE);
                            }
                            #endregion
                        }
                        #region cihaz uygun değil
                        else
                        {
                            return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.DEVICE_IS_NOT_AVAILABLE);
                        }
                        #endregion
                    }
                    #region cihaz bağlantısı yok veya bulunamadı
                    else
                    {
                        return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.DEVICE_IS_NOT_CONNECTED);
                    }
                    #endregion
                }
                #region cihaz bulunamadı
                else
                {
                    return new ErrorResult<SelectDeviceResponseDto>(selectDeviceResponse, ChargeErrorEnum.DEVICE_NOT_FOUND);
                }
                #endregion
            }
            #endregion
        }
        /// <summary>
        /// Şarj işlemi için soketler getiriliyor
        /// </summary>
        /// <param name="getSocketListForChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetSocketListForChargeResponseDto>> GetSocketListForCharge(GetSocketListForChargeRequestDto getSocketListForChargeRequest)
        {
            #region response dto oluşturuluyor
            GetSocketListForChargeResponseDto getSocketListForChargeResponse = new GetSocketListForChargeResponseDto();
            getSocketListForChargeResponse.ConnectorList = new List<ChargeDeviceConnectorMobilDto>();
            #endregion
            #region ödeme alınamamış şarj işlemi mevcut mu kontrol ediliyor
            #region filter dto oluşturuluyor
            var waitingChargeProcessFilter = new ChargeFilterDto();
            waitingChargeProcessFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL };
            waitingChargeProcessFilter.UserId = _userContextProvider.UserId;
            #endregion
            var waitingCharge = await _chargeRepository.GetCharge(waitingChargeProcessFilter,null).AnyAsync();
            #region ödenmemiş şarj işlemi mevcut
            if (waitingCharge)
            {
                return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.UNPAID_CHARGE_EXIST);
            }
            #endregion
            #endregion
            #region charge device çekiliyor
            #region filter dto oluşturuluyor
            ChargeDeviceFilterDto chargeDeviceFilter = _mapper.Map<ChargeDeviceFilterDto>(getSocketListForChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeDeviceFilterDto;
                });
            });
            #endregion
            //ChargeDevice chargeDevice = await _chargeDeviceRepository.GetChargeDevicesAsNoTracking(chargeDeviceFilter).FirstOrDefaultAsync();
            ChargeDevice chargeDevice = await _chargeDeviceRepository.GetChargeDevices(chargeDeviceFilter, include: source => source
     .Include(a => a.Station)
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevicePowerType)
     ).FirstOrDefaultAsync();
            #endregion
            if (chargeDevice != null)
            {
                if (chargeDevice.Station.IsActive)
                {
                    if (chargeDevice.State == ChargeDeviceStateEnum.ACTIVE)
                    {
                        if (chargeDevice.ConnectionState)
                        {
                            if (chargeDevice.InstantState == ChargeDeviceInstantStateEnum.AVAILABLE)
                            {
                                #region response dto setleniyor
                                #region soketler response dto ya setleniyor
                                getSocketListForChargeResponse.ConnectorList = _mapper.Map<List<ChargeDeviceConnectorMobilDto>>(chargeDevice.ChargeDeviceConnector);
                                #endregion
                                getSocketListForChargeResponse.DeviceIdentifier = chargeDevice.Identifier;
                                #endregion
                                return new SuccessResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse);
                            }
                            #region cihaz uygun değil
                            else
                            {
                                return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_AVAILABLE);
                            }
                            #endregion
                        }
                        #region cihaz bağlantısı yok
                        else
                        {
                            return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_CONNECTED);
                        }
                        #endregion
                    }
                    #region cihaz aktif değil
                    else
                    {
                        return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_ACTIVE);
                    }
                    #endregion
                }
                #region istasyon aktif değil
                else
                {
                    return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.STATION_NOT_ACTIVE);
                }
                #endregion
            }
            #region cihaz bulunamadı
            else
            {
                return new ErrorResult<GetSocketListForChargeResponseDto>(getSocketListForChargeResponse, ChargeErrorEnum.DEVICE_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// Soket seçiliyor 
        /// </summary>
        /// <param name="selectSocketRequest"></param>
        /// <returns></returns>
        public async Task<Result<SelectSocketResponseDto>> SelectSocket(SelectSocketRequestDto selectSocketRequest)
        {
            SelectSocketResponseDto selectSocketResponse = new SelectSocketResponseDto();
            #region charge device connector çekiliyor
            #region filter dto oluşturuluyor
            ChargeDeviceConnectorFilterDto chargeDeviceConnectorFilter = new ChargeDeviceConnectorFilterDto();
            chargeDeviceConnectorFilter.Identifier = selectSocketRequest.ConnectorIdentifier;
            #endregion
            //ChargeDeviceConnector chargeDeviceConnector = await _chargeDeviceConnectorRepository.GetChargeDeviceConnectorAsNoTracking(chargeDeviceConnectorFilter).FirstOrDefaultAsync();
            ChargeDeviceConnector chargeDeviceConnector = await _chargeDeviceConnectorRepository.GetChargeDeviceConnector(chargeDeviceConnectorFilter, include: source => source
     .Include(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)).FirstOrDefaultAsync();
            #endregion
            if (chargeDeviceConnector != null)
            {
                if (chargeDeviceConnector.ChargeDevice.Station.IsActive)
                {
                    if (chargeDeviceConnector.ChargeDevice.State == ChargeDeviceStateEnum.ACTIVE)
                    {
                        if (chargeDeviceConnector.State == ChargeDeviceConnectorStateEnum.ACTIVE)
                        {
                            if (chargeDeviceConnector.ChargeDevice.ConnectionState)
                            {
                                #region connector bağlı mı ve müsait mi kontrol ediliyor
                                if (chargeDeviceConnector.ChargeDevice.InstantState == ChargeDeviceInstantStateEnum.AVAILABLE
                                )
                                {
                                    if (chargeDeviceConnector.State == ChargeDeviceConnectorStateEnum.ACTIVE)
                                    {
                                        if ((chargeDeviceConnector.InstantState == null ||
                               chargeDeviceConnector.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE ||
                              chargeDeviceConnector.InstantState == ChargeDeviceConnectorInstantStateEnum.PREPARING))
                                        {
                                            #region şarjall bünyesinde soket seçim işlemi gerçekleşiyor
                                            if (chargeDeviceConnector.ChargeDevice.Station.StationFirmType == StationFirmTypeEnum.ROTAWATT)
                                            {
                                                #region ocpp apiye istek atılarak soket hareket verisi getiriliyor
                                                #region request dto oluşturuluyor
                                                GetLastSocketMovementRequestDto getLastSocketMovementRequest = new GetLastSocketMovementRequestDto();
                                                getLastSocketMovementRequest.ConnectorNo = chargeDeviceConnector.ConnectorNo;
                                                getLastSocketMovementRequest.DeviceIdentifier = chargeDeviceConnector.ChargeDevice.Identifier;
                                                #endregion
                                                var getLastSocketMovementResponse = await _socketMovementClientService.GetLastSocketMovement(getLastSocketMovementRequest);
                                                #endregion
                                                if (getLastSocketMovementResponse.ResultType == ResultType.Ok)
                                                {
                                                    #region istek için request dto oluşturuluyor
                                                    GetConnectorConnectionRequestDto getConnectorConnectionRequest = new GetConnectorConnectionRequestDto();
                                                    getConnectorConnectionRequest.ChargeDeviceConnectorGuiId = chargeDeviceConnector.GuiId;
                                                    #endregion
                                                    #region notification api ye istek atılıyor
                                                    var getConnectorConnectionResponse = await _connectorConnectionClientService.GetConnectorConnection(getConnectorConnectionRequest);
                                                    #endregion
                                                    #region response dto ya veriler setleniyor
                                                    if (getConnectorConnectionResponse.ResultType == ResultType.Ok && getConnectorConnectionResponse.Data != null)
                                                    {
                                                        selectSocketResponse.ConnectionId = getConnectorConnectionResponse.Data.ConnectionId;
                                                    }
                                                    #endregion
                                                    #region soket takılı 
                                                    if (getLastSocketMovementResponse.Data.SocketMovementState == SocketMovementStateEnum.PLUGGED)
                                                    {
                                                        #region şarj işlemi cihaz okuma verisi ekleniyor
                                                        ChargingConnectorReading chargingConnectorReading = new ChargingConnectorReading();
                                                        chargingConnectorReading.ChargeDeviceConnectorId = chargeDeviceConnector.Id;
                                                        chargingConnectorReading.GuiId = Guid.NewGuid() + "";
                                                        chargingConnectorReading.ReadingDate = DateTime.Now;
                                                        chargingConnectorReading.UserId = _userContextProvider.UserId;
                                                        chargingConnectorReading.DeviceSelectType = selectSocketRequest.DeviceSelectType;
                                                        chargingConnectorReading.PluggedSocketMovementGuiId = getLastSocketMovementResponse.Data.GuiId;
                                                        await _chargingConnectorReadingRepository.InsertAsync(chargingConnectorReading);
                                                        await _chargingConnectorReadingRepository.SaveChangesAsync();
                                                        #endregion
                                                        #region response dto setleniyor
                                                        selectSocketResponse.ConnectorChargeReadingGuiId = chargingConnectorReading.GuiId;
                                                        #endregion
                                                        return new SuccessResult<SelectSocketResponseDto>(selectSocketResponse);
                                                    }
                                                    #endregion
                                                    #region soket takılı değil
                                                    else
                                                    {
                                                        return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.SOCKET_REMOVED);
                                                    }
                                                    #endregion
                                                }
                                                #region soket hareketi bulunamadı
                                                else
                                                {
                                                    return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            #region entegrasyon firma üzerinden soket seçim işlemi gerçekleşiyor
                                            else if (chargeDeviceConnector.ChargeDevice.Station.StationFirmType == StationFirmTypeEnum.FIRM)
                                            {
                                                #region şarj işlemi cihaz okuma verisi ekleniyor
                                                ChargingConnectorReading chargingConnectorReading = new ChargingConnectorReading();
                                                chargingConnectorReading.ChargeDeviceConnectorId = chargeDeviceConnector.Id;
                                                chargingConnectorReading.GuiId = Guid.NewGuid() + "";
                                                chargingConnectorReading.ReadingDate = DateTime.Now;
                                                chargingConnectorReading.UserId = _userContextProvider.UserId;
                                                chargingConnectorReading.DeviceSelectType = selectSocketRequest.DeviceSelectType;
                                                await _chargingConnectorReadingRepository.InsertAsync(chargingConnectorReading);
                                                await _chargingConnectorReadingRepository.SaveChangesAsync();
                                                #endregion
                                                #region response dto setleniyor
                                                selectSocketResponse.ConnectorChargeReadingGuiId = chargingConnectorReading.GuiId;
                                                #endregion
                                                return new SuccessResult<SelectSocketResponseDto>(selectSocketResponse);
                                            }
                                            #endregion
                                            #region geçersiz firma tipi
                                            else
                                            {
                                                return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                                            }
                                            #endregion
                                        }
                                        #region soket musait değil
                                        else
                                        {
                                            return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.CONNECTOR_NOT_AVAILABLE);
                                        }
                                        #endregion
                                    }
                                    #region soket aktif değil
                                    else
                                    {
                                        return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.CONNECTOR_IS_NOT_ACTIVE);
                                    }
                                    #endregion
                                }
                                #region cihaz müsait değil
                                else
                                {
                                    return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.DEVICE_IS_NOT_AVAILABLE);
                                }
                                #endregion
                                #endregion
                            }
                            #region cihaz bağlı değil
                            else
                            {
                                return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.DEVICE_IS_NOT_CONNECTED);
                            }
                            #endregion
                        }
                        #region soket aktif değil
                        else
                        {
                            return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.CONNECTOR_IS_NOT_ACTIVE);
                        }
                        #endregion
                    }
                    #region cihaz aktif değil
                    else
                    {
                        return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.DEVICE_IS_NOT_ACTIVE);
                    }
                    #endregion
                }
                #region istasyon uygun değil
                else
                {
                    return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.STATION_NOT_ACTIVE);
                }
                #endregion
            }
            #region charge device connector bulunamadı
            else
            {
                return new ErrorResult<SelectSocketResponseDto>(selectSocketResponse, ChargeErrorEnum.CONNECTOR_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// şarj işlemi başlatılıyor
        /// </summary>
        /// <param name="startChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<StartChargeResponseDto>> StartCharge(StartChargeRequestDto startChargeRequest)
        {
            #region response dto oluşturuluyor
            StartChargeResponseDto startChargeResponse = new StartChargeResponseDto();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region connector charge reading kayıdı getiriliyor
            #region filter dto oluşturuluyor
            ChargingConnectorReadingFilterDto chargingConnectorReadingFilter = _mapper.Map<ChargingConnectorReadingFilterDto>(startChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargingConnectorReadingFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            ChargingConnectorReading chargingConnectorReading = await _chargingConnectorReadingRepository.GetChargingConnectorReadingAsNoTracking(chargingConnectorReadingFilter).FirstOrDefaultAsync();
            #endregion
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
                                        #region şarjall bünyesinde şarj işlemi başlatılıyor
                                        if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Station.StationFirmType == StationFirmTypeEnum.ROTAWATT)
                                        {
                                            #region ocpp apiye istek atılarak son soket hareket verisi alınıyor
                                            #region request dto oluşturuluyor
                                            GetLastSocketMovementRequestDto getLastSocketMovementRequest = new GetLastSocketMovementRequestDto();
                                            getLastSocketMovementRequest.ConnectorNo = chargingConnectorReading.ChargeDeviceConnector.ConnectorNo;
                                            getLastSocketMovementRequest.DeviceIdentifier = chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Identifier;
                                            #endregion
                                            var getLastSocketMovementResponse = await _socketMovementClientService.GetLastSocketMovement(getLastSocketMovementRequest);
                                            #endregion
                                            if (getLastSocketMovementResponse.ResultType == ResultType.Ok)
                                            {
                                                #region soket takılı değil, eğer takılı ise tekrar çıkarıp takın
                                                if (getLastSocketMovementResponse.Data.SocketMovementState != SocketMovementStateEnum.PLUGGED)
                                                {
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
                                                }
                                                #endregion
                                                #region soket hareketi ile şarj işlem verisinde bulunan soket hareketi uyuşmamaktadır
                                                if (getLastSocketMovementResponse.Data.GuiId != chargingConnectorReading.PluggedSocketMovementGuiId)
                                                {
                                                    // error düzelcek
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_MOVEMENT_DOES_NOT_MATCH_WITH_LAST_SOCKET_MOVEMENT);
                                                }
                                                #endregion
                                                Charge charge;
                                                #region soket okuma verisine ait şarj işlemi var ise kontrol ediliyor
                                                if (chargingConnectorReading.Charge != null)
                                                {
                                                    #region şarj işlemi başlamış, başlatılıyor veya bitmiş
                                                    if (chargingConnectorReading.Charge.State == ChargeStateEnum.PAYMENT_BEING_RECEIVED ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.COMPLETED ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.CALCULATING ||
                                                        chargingConnectorReading.Charge.State == ChargeStateEnum.PAYMENT_FAIL)
                                                    {
                                                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
                                                    }
                                                    else if (chargingConnectorReading.Charge.State == ChargeStateEnum.PROCESS_ENDING)
                                                    {
                                                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CURRENT_CHARGE_FINISHING);
                                                    }
                                                    else
                                                    {
                                                        #region işlem başka bir kullanıcı için başlatılmıştır
                                                        if (chargingConnectorReading.Charge.UserId != _userContextProvider.UserId)
                                                        {
                                                            return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CHARGE_STARTING);
                                                        }
                                                        #endregion
                                                    }
                                                    #endregion
                                                    charge = chargingConnectorReading.Charge;
                                                }
                                                #endregion

                                                #region şarj işlem verisi entity oluşturuluyor
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
                                                #endregion
                                                #region ocpp apiye istek atılarak şarj başlatma isteği atılıyor
                                                #region request dto oluşturuluyor
                                                Ocpp16StationRemoteStartTransactionRequestDto remoteStartTransactionRequest = new Ocpp16StationRemoteStartTransactionRequestDto();
                                                remoteStartTransactionRequest.ChargeGuiId = charge.GuiId;
                                                remoteStartTransactionRequest.ConnectorNo = chargingConnectorReading.ChargeDeviceConnector.ConnectorNo;
                                                remoteStartTransactionRequest.DeviceIdentifier = chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Identifier;
                                                #endregion
                                                var ocpp16RemoteStartTransactionResponse = await _remoteTransactionClientService.RemoteStartTransactionAsync(remoteStartTransactionRequest);
                                                #endregion
                                                #region şarj başlatma isteği atıldı, charge process veritabanına kayıt ediliyor
                                                if (ocpp16RemoteStartTransactionResponse.ResultType == ResultType.Ok)
                                                {
                                                    #region veritabanına kayıt ediliyor
                                                    if (charge.Id == 0)
                                                    {
                                                        await _chargeRepository.InsertAsync(charge);
                                                        await _chargeRepository.SaveChangesAsync();
                                                    }
                                                    #endregion
                                                    #region response dto setleniyor
                                                    startChargeResponse.ChargeGuiId = charge.GuiId;
                                                    startChargeResponse.ConnectionId = _userContextProvider.ConnectionId;
                                                    #endregion
                                                    return new SuccessResult<StartChargeResponseDto>(startChargeResponse);
                                                }
                                                #endregion
                                                #region şarj işlemi başlatılamadı
                                                else
                                                {
                                                    charge.State = ChargeStateEnum.FAILED;
                                                    charge.LastUpdateDate = DateTime.Now;
                                                    #region veritabanına kayıt ediliyor
                                                    await _chargeRepository.InsertAsync(charge);
                                                    await _chargeRepository.SaveChangesAsync();
                                                    #endregion
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CHARGE_FAILED_TO_START);
                                                }
                                                #endregion
                                            }
                                            #region soket hareketi getirilemedi
                                            else
                                            {
                                                if (getLastSocketMovementResponse.ErrorCode == (int)SocketMovementErrorEnum.SOCKET_MOVEMENT_CAN_NOT_FOUND)
                                                {
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_MOVEMENT_CAN_NOT_FOUND);
                                                }
                                                else
                                                {
                                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.AN_ERROR_OCCURRED);
                                                }
                                            }
                                            #endregion
                                        }
                                        #endregion
                                        #region entegrasyon firma üzerinden şarj işlemi başlatılıyor
                                        else if (chargingConnectorReading.ChargeDeviceConnector.ChargeDevice.Station.StationFirmType == StationFirmTypeEnum.ROTAWATT)
                                        {
                                            #region firm apiye istek atılarak şarj başlatma işlemi gerçekleşiyor
                                            #region request dto oluşturuluyor

                                            #endregion

                                            #endregion
                                            return new SuccessResult<StartChargeResponseDto>(startChargeResponse);
                                        }
                                        #endregion
                                        #region geçersiz firma tipi
                                        else
                                        {
                                            return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                                        }
                                        #endregion
                                    }
                                    #region soket uygun değil
                                    else
                                    {
                                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.SOCKET_REMOVED);
                                    }
                                    #endregion
                                }
                                #region soket aktif değil
                                else
                                {
                                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.CONNECTOR_IS_NOT_ACTIVE);
                                }
                                #endregion
                            }
                            #region cihaz uygun değil
                            else
                            {
                                return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_AVAILABLE);
                            }
                            #endregion
                        }
                        #region cihaz bağlı değil
                        else
                        {
                            return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_CONNECTED);
                        }
                        #endregion
                    }
                    #region cihaz aktif değil
                    else
                    {
                        return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.DEVICE_IS_NOT_ACTIVE);
                    }
                    #endregion
                }
                #region istasyon aktif değil
                else
                {
                    return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.STATION_NOT_ACTIVE);
                }
                #endregion
            }
            #region soket okuma verisi bulunamadı
            else
            {
                return new ErrorResult<StartChargeResponseDto>(startChargeResponse, ChargeErrorEnum.HAVE_TO_REPLUG_SOCKET);
            }
            #endregion
        }
        /// <summary>
        /// şarj işlemleri getiriliyor
        /// </summary>
        /// <param name="getMobilChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilChargeResponseDto>> GetCharges(GetMobilChargeRequestDto getMobilChargeRequest)
        {
            #region response dto oluşturuluyor
            GetMobilChargeResponseDto getMobilChargeResponse = new GetMobilChargeResponseDto();
            #endregion
            #region charge process çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(getMobilChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.ExceptChargeState = ChargeStateEnum.FAILED;
                });
            });
            #endregion
            //var chargeListQueryable = _chargeRepository.GetChargeAsNoTracking(chargeFilter);
            var chargeListQueryable = _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)
     .ThenInclude(a => a.StationAddress)
       .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)
     .ThenInclude(a => a.StationContent));
            #region şarj işlem ofset ve skip ayarlamaları gerçekleşiyor
            if (getMobilChargeRequest.Ofset != null && getMobilChargeRequest.Count != null)
            {
                var skip = getMobilChargeRequest.Ofset.GetValueOrDefault() * getMobilChargeRequest.Count.GetValueOrDefault();
                chargeListQueryable = chargeListQueryable.Skip(skip).Take(getMobilChargeRequest.Count.GetValueOrDefault());
            }
            #endregion
            #region şarj işlem sayısı setleniyor
            getMobilChargeResponse.TotalRecord = await chargeListQueryable.CountAsync();
            #endregion
            var chargeListEntity = await chargeListQueryable.OrderByDescending(x => x.Id).ToListAsync();
            getMobilChargeResponse.ChargeList = _mapper.Map<List<MobilChargeListItemDto>>(chargeListEntity);
            #endregion
            return new SuccessResult<GetMobilChargeResponseDto>(getMobilChargeResponse);
        }
        /// <summary>
        /// şarj işlem detayı getiriliyor
        /// </summary>
        /// <param name="getMobilChargeDetailRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilChargeDetailResponseDto>> GetChargeDetail(GetMobilChargeDetailRequestDto getMobilChargeDetailRequest)
        {
            #region response dto oluşturuluyor
            GetMobilChargeDetailResponseDto getMobilChargeDetailResponse = new GetMobilChargeDetailResponseDto();
            #endregion
            #region charge çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeProcessFilter = _mapper.Map<ChargeFilterDto>(getMobilChargeDetailRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            //var chargeEntity = await _chargeRepository.GetChargeAsNoTracking(chargeProcessFilter).FirstOrDefaultAsync();
            var chargeEntity = await _chargeRepository.GetCharge(chargeProcessFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)
     .ThenInclude(a => a.StationAddress)
          .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)
     .ThenInclude(a => a.StationContent)
          .Include(a => a.ChargeDeviceConnector)
          .ThenInclude(a => a.ChargeDevicePowerType)
          .Include(a => a.PaymentInfo)
          .ThenInclude(a => a.ProcessingUserAdress)
            .ThenInclude(a => a.Town)
            .ThenInclude(a => a.City)
            .ThenInclude(a => a.Country)
          .Include(a => a.Firm)
          .ThenInclude(a => a.FirmLogo)).FirstOrDefaultAsync();
            #endregion
            #region işlem mevcutsa dto ya dönüştürülüyor
            if (chargeEntity != null)
            {
                #region kullanıcının varsayılan adresi getiriliyor
                #region filter dto oluşturuluyor
                UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                userAddressFilter.UserId = chargeEntity.UserId;
                userAddressFilter.IsDefault = true;
                #endregion
                var defaultUserAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                #endregion
                getMobilChargeDetailResponse = _mapper.Map<GetMobilChargeDetailResponseDto>(chargeEntity, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var srcData = src as Charge;
                        var destData = dest as GetMobilChargeDetailResponseDto;
                        if (srcData.Feedback != null)
                        {
                            destData.FeedbackId = srcData.Feedback.Id;
                        }
                        #region adres setleniyor
                        if (srcData.PaymentInfo != null)
                        {
                            destData.BillingAdress = srcData.PaymentInfo.ProcessingUserAdress.Town.City.CityName +
                                                                    "/" + srcData.PaymentInfo.ProcessingUserAdress.Town.TownName +
                                                                    " " + srcData.PaymentInfo.ProcessingUserAdress.Neighbourhood;
                            destData.AdressType = srcData.PaymentInfo.ProcessingUserAdress.UserAdressType;
                            destData.BillingAdressName = srcData.PaymentInfo.ProcessingUserAdress.Name;
                        }
                        else
                        {
                            if (defaultUserAddress != null)
                            {
                                destData.BillingAdress = defaultUserAddress.Town.City.CityName +
                                                              "/" + defaultUserAddress.Town.TownName +
                                                              " " + defaultUserAddress.Neighbourhood;
                                destData.AdressType = defaultUserAddress.UserAdressType;
                                destData.BillingAdressName = defaultUserAddress.Name;
                            }
                        }
                        #endregion
                    });
                });
                return new SuccessResult<GetMobilChargeDetailResponseDto>(getMobilChargeDetailResponse);
            }
            #endregion
            else
            {
                return new ErrorResult<GetMobilChargeDetailResponseDto>(getMobilChargeDetailResponse, ChargeErrorEnum.PROCESS_CAN_NOT_FOUND);
            }
        }
        /// <summary>
        /// şarj işlem özeti getiriliyor
        /// </summary>
        /// <param name="getChargeCompleteSummaryRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetChargeCompleteSummaryResponseDto>> GetChargeCompleteSummary(GetChargeCompleteSummaryRequestDto getChargeCompleteSummaryRequest)
        {
            #region response dto oluşturuluyor
            GetChargeCompleteSummaryResponseDto getMobilChargeSummaryResponse = new GetChargeCompleteSummaryResponseDto();
            #endregion
            #region charge çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeProcessFilter = _mapper.Map<ChargeFilterDto>(getChargeCompleteSummaryRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            //var chargeEntity = await _chargeRepository.GetChargeAsNoTracking(chargeProcessFilter).FirstOrDefaultAsync();
            var chargeEntity = await _chargeRepository.GetCharge(chargeProcessFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)).FirstOrDefaultAsync();
            #endregion
            #region işlem mevcutsa dto ya dönüştürülüyor
            if (chargeEntity != null)
            {
                #region kullanıcının varsayılan adresi getiriliyor
                #region filter dto oluşturuluyor
                UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                userAddressFilter.UserId = chargeEntity.UserId;
                userAddressFilter.IsDefault = true;
                #endregion
                var defaultUserAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                #endregion
                getMobilChargeSummaryResponse = _mapper.Map<GetChargeCompleteSummaryResponseDto>(chargeEntity, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var srcData = src as Charge;
                        var destData = dest as GetChargeCompleteSummaryResponseDto;
                    });
                });
                return new SuccessResult<GetChargeCompleteSummaryResponseDto>(getMobilChargeSummaryResponse);
            }
            #endregion
            else
            {
                return new ErrorResult<GetChargeCompleteSummaryResponseDto>(getMobilChargeSummaryResponse, ChargeErrorEnum.PROCESS_CAN_NOT_FOUND);
            }
        }
        /// <summary>
        /// şarj işlem ödeme özeti getiriliyor
        /// </summary>
        /// <param name="chargePaymentResultDetailRequest"></param>
        /// <returns></returns>
        public async Task<Result<ChargePaymentResultDetailResponseDto>> ChargePaymentResultDetail(ChargePaymentResultDetailRequestDto chargePaymentResultDetailRequest)
        {
            #region response dto oluşturuluyor
            ChargePaymentResultDetailResponseDto chargePaymentResultDetailResponse = new ChargePaymentResultDetailResponseDto();
            #endregion
            #region charge çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeProcessFilter = _mapper.Map<ChargeFilterDto>(chargePaymentResultDetailRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            //var chargeEntity = await _chargeRepository.GetChargeAsNoTracking(chargeProcessFilter).FirstOrDefaultAsync();
            var chargeEntity = await _chargeRepository.GetCharge(chargeProcessFilter, include: source => source
     .Include(a => a.PaymentInfo)
     .ThenInclude(a => a.ProcessingUserAdress)
     .ThenInclude(a => a.Town)
     .ThenInclude(a => a.City)
     .ThenInclude(a => a.Country)).FirstOrDefaultAsync();
            #endregion
            #region işlem mevcutsa dto ya dönüştürülüyor
            if (chargeEntity != null)
            {
                #region kullanıcının varsayılan adresi getiriliyor
                #region filter dto oluşturuluyor
                UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                userAddressFilter.UserId = chargeEntity.UserId;
                userAddressFilter.IsDefault = true;
                #endregion
                var defaultUserAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                #endregion
                chargePaymentResultDetailResponse = _mapper.Map<ChargePaymentResultDetailResponseDto>(chargeEntity, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var srcData = src as Charge;
                        var destData = dest as ChargePaymentResultDetailResponseDto;
                        #region adres setleniyor
                        if (srcData.PaymentInfo != null)
                        {
                            destData.BillingAdress = srcData.PaymentInfo.ProcessingUserAdress.Town.City.CityName +
                                                                    "/" + srcData.PaymentInfo.ProcessingUserAdress.Town.TownName +
                                                                    " " + srcData.PaymentInfo.ProcessingUserAdress.Neighbourhood;
                            destData.AdressType = srcData.PaymentInfo.ProcessingUserAdress.UserAdressType;
                            destData.BillingAdressName = srcData.PaymentInfo.ProcessingUserAdress.Name;
                        }
                        else
                        {
                            if (defaultUserAddress != null)
                            {
                                destData.BillingAdress = defaultUserAddress.Town.City.CityName +
                                                              "/" + defaultUserAddress.Town.TownName +
                                                              " " + defaultUserAddress.Neighbourhood;
                                destData.AdressType = defaultUserAddress.UserAdressType;
                                destData.BillingAdressName = defaultUserAddress.Name;
                            }
                        }
                        #endregion
                    });
                });
                return new SuccessResult<ChargePaymentResultDetailResponseDto>(chargePaymentResultDetailResponse);
            }
            #endregion
            else
            {
                return new ErrorResult<ChargePaymentResultDetailResponseDto>(chargePaymentResultDetailResponse, ChargeErrorEnum.PROCESS_CAN_NOT_FOUND);
            }
        }
        /// <summary>
        /// şarj işlemi durduruluyor
        /// </summary>
        /// <param name="stopChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<StopChargeResponseDto>> StopCharge(StopChargeRequestDto stopChargeRequest)
        {
            #region response dto oluşturuluyor
            StopChargeResponseDto stopChargeResponse = new StopChargeResponseDto();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region charge getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = new ChargeFilterDto();
            chargeFilter.GuiId = stopChargeRequest.ChargeGuiId;
            chargeFilter.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.FAILED, ChargeStateEnum.PROCESS_ENDING,
                                                                                               ChargeStateEnum.PROCESS_START, ChargeStateEnum.PROCESS_STARTING };
            #endregion
            //var charge = await _chargeRepository.GetChargeWithConnectorAndDevice(chargeFilter).FirstOrDefaultAsync();
            var charge = await _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice),false).FirstOrDefaultAsync();
            #endregion
            if (charge != null)
            {
                #region şarjall bünyesinde şarj işlemi durduruluyor
                if (charge.ChargeFirmType == ChargeFirmTypeEnum.ROTAWATT)
                {
                    #region ocpp apiye istek atılarak uzaktan şarjı durdurma işlemi gerçekleştiriliyor
                    #region request dto oluşturuluyor
                    Ocpp16StationRemoteStopTransactionRequestDto ocpp16StationRemoteStopTransactionRequest = new Ocpp16StationRemoteStopTransactionRequestDto();
                    ocpp16StationRemoteStopTransactionRequest.ChargeGuiId = charge.GuiId;
                    ocpp16StationRemoteStopTransactionRequest.TransactionGuiId = charge.TransactionGuiId;
                    ocpp16StationRemoteStopTransactionRequest.DeviceIdentifier = charge.ChargeDeviceConnector.ChargeDevice.Identifier;
                    ocpp16StationRemoteStopTransactionRequest.ConnectorNo = charge.ChargeDeviceConnector.ConnectorNo;
                    ocpp16StationRemoteStopTransactionRequest.SocketMovementGuiId = charge.PluggedSocketMovementGuiId;
                    #endregion
                    var stationRemoteStopTransactionResponse = await _remoteTransactionClientService.RemoteStopTransactionAsync(ocpp16StationRemoteStopTransactionRequest);
                    #endregion
                    if (stationRemoteStopTransactionResponse.ResultType == ResultType.Ok && stationRemoteStopTransactionResponse.Data != null)
                    {
                        #region charge güncelleniyor
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
                        {
                            charge.TransactionGuiId = stationRemoteStopTransactionResponse.Data.TransactionGuiId;
                        }
                        if (stationRemoteStopTransactionResponse.Data.ChargeState != null)
                        {
                            charge.State = stationRemoteStopTransactionResponse.Data.ChargeState.GetValueOrDefault();
                        }
                        if (stationRemoteStopTransactionResponse.Data.LastUpdateDate != null)
                        {
                            charge.LastUpdateDate = stationRemoteStopTransactionResponse.Data.LastUpdateDate;
                            charge.EndTime = stationRemoteStopTransactionResponse.Data.LastUpdateDate;
                        }
                        if (stationRemoteStopTransactionResponse.Data.LoadedKw != null)
                        {
                            charge.LoadedKw = stationRemoteStopTransactionResponse.Data.LoadedKw;
                            charge.CalculatedPrice = (Math.Round(Convert.ToDecimal(charge.LoadedKw), 2) * charge.KwPrice);
                        }
                        if (stationRemoteStopTransactionResponse.Data.ChargePercentage != null)
                        {
                            charge.ChargePercentage = stationRemoteStopTransactionResponse.Data.ChargePercentage;
                        }
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _chargeRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor
                        stopChargeResponse = _mapper.Map<StopChargeResponseDto>(stationRemoteStopTransactionResponse.Data);
                        return new SuccessResult<StopChargeResponseDto>(stopChargeResponse);
                        #endregion
                    }
                    else
                    {
                        return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.PROCESS_COULD_NOT_STOPPED);
                    }
                }
                #endregion
                #region entegrasyon firma üzerinden şarj işlemi durduruluyor
                else if (charge.ChargeFirmType == ChargeFirmTypeEnum.FIRM)
                {
                    #region firm apiye istek atılarak şarj işlemi durduruluyor
                    #region request dto oluşturuluyor

                    #endregion

                    #endregion
                    return new SuccessResult<StopChargeResponseDto>(stopChargeResponse);
                }
                #endregion
                #region geçersiz firma tipi
                else
                {
                    return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.INVALID_FIRM_TYPE);
                }
                #endregion
            }
            #region şarj işlemi bulunamadı
            else
            {
                return new ErrorResult<StopChargeResponseDto>(stopChargeResponse, ChargeErrorEnum.CHARGE_CAN_NOT_FOUND);
            }
            #endregion
        }
        #region private methods
        #endregion
    }
}
