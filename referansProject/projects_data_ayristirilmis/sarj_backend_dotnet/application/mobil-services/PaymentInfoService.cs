using Api.Persistence.Repositories.WalletRepositories;
using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.DebitCardDtos;
using Shared.Domain.Dto.ApiDto.MobilDebitCardDtos;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.MobilUserAddressDtos;
using Shared.Domain.Dto.ApiDto.MobilWalletDtos;
using Shared.Domain.Dto.ApiDto.PanelPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.PaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.UserAddressDtos;
using Shared.Domain.Dto.ApiDto.UserDtos;
using Shared.Domain.Dto.ApiDto.WalletDtos;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using Shared.Domain.Dto.BankDto.WalletDtos;
using Shared.Domain.Dto.TockenDto.DebitCardIdentityDtos;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.DebitCardModule;
using Shared.Domain.Entities.ApiEntities.PaymentInfoModule;
using Shared.Domain.Entities.ApiEntities.UserAdressModule;
using Shared.Domain.Entities.ApiEntities.WalletInfoModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.WorkerServiceEnums;
using Shared.Domain.Errors.BankErrors;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.Errors.TockenErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.BankApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces;
using Shared.Domain.HttpClients.HttpClientServices.BankApiServices;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.DebitCardRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ParameterRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.PaymentInfoRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserAddressRepositories;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserAddressRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.WalletRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.PaymentInfoServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Application.Services.MobilApiServices.PaymentInfoService
{
    public class PaymentInfoService : BaseService, IPaymentInfoService
    {
        private readonly IPaymentInfoRepository _paymentInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IProcessingUserAdressRepository _processingUserAdressRepository;
        private readonly IWalletFormRepository _walletFormRepository;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IDebitCardRepository _debitCardRepository;
        private readonly IWalletInfoRepository _walletInfoRepository;
        private readonly IPaymentClientService _paymentClientService;
        private readonly IWalletClientService _walletClientService;
        private readonly IDebitCardIdentityClientService _debitCardIdentityClientService;
        private readonly IWalletProcessRepository _walletProcessRepository;
        private readonly ILogger<PaymentInfoService> _logger;
        public PaymentInfoService(IMapper mapper,
                          IConfiguration configuration,
                          IUtilService utilService,
                          IUserRepository userRepository,
                          IUserContextProvider userContextProvider,
                          IUserAddressRepository userAddressRepository,
                          ILogger<PaymentInfoService> logger,
                          IPaymentClientService paymentClientService,
                          IParameterRepository parameterRepository,
                          IProcessingUserAdressRepository processingUserAdressRepository,
                          IPaymentInfoRepository paymentInfoRepository,
                          IChargeRepository chargeRepository,
                          IDebitCardRepository debitCardRepository,
                          IDebitCardIdentityClientService debitCardIdentityClientService,
                          IWalletInfoRepository walletInfoRepository,
                          IWalletProcessRepository walletProcessRepository,
                          IWalletFormRepository walletFormRepository,
                          IWalletClientService walletClientService) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _utilService = utilService;
            _userContextProvider = userContextProvider;
            _userRepository = userRepository;
            _userAddressRepository = userAddressRepository;
            _logger = logger;
            _paymentClientService = paymentClientService;
            _parameterRepository = parameterRepository;
            _processingUserAdressRepository = processingUserAdressRepository;
            _paymentInfoRepository = paymentInfoRepository;
            _chargeRepository = chargeRepository;
            _debitCardRepository = debitCardRepository;
            _debitCardIdentityClientService = debitCardIdentityClientService;
            _walletInfoRepository = walletInfoRepository;
            _walletProcessRepository = walletProcessRepository;
            _walletFormRepository = walletFormRepository;
            _walletClientService = walletClientService;
        }
        #region ekleme,güncelleme,silme
        #endregion
        /// <summary>
        /// ödeme işlemi formu hazırlanıyor
        /// </summary>
        /// <param name="preparePaymentFormRequest"></param>
        /// <returns></returns>
        public async Task<Result<PreparePaymentFormResponseDto>> PreparePaymentForm(PreparePaymentFormRequestDto preparePaymentFormRequest)
        {
            #region response dto oluşturuluyor
            PreparePaymentFormResponseDto preparePaymentFormResponse = new PreparePaymentFormResponseDto();
            #endregion
            #region adres getiriliyor ve response dto ya setleniyor
            #region filter dto oluşturuluyor
            UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
            userAddressFilter.UserId = _userContextProvider.UserId;
            if (preparePaymentFormRequest.UserAddressId != null)
            {
                userAddressFilter.Id = preparePaymentFormRequest.UserAddressId;
            }
            else
            {
                userAddressFilter.IsDefault = true;
            }
            #endregion
            var selectedUserAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
            #region response dto ya setleniyor
            if (selectedUserAddress != null)
            {
                preparePaymentFormResponse.SelectedUserAddress = _mapper.Map<MobilUserAddressPaymentListItemDto>(selectedUserAddress);
            }
            #endregion
            #endregion
            #region kart getiriliyor ve response dto ya setleniyor
            #region filter dto oluşturuluyor
            DebitCardFilterDto debitCardFilter = new DebitCardFilterDto();
            debitCardFilter.UserId = _userContextProvider.UserId;
            if (preparePaymentFormRequest.DebitCardId != null)
            {
                debitCardFilter.Id = preparePaymentFormRequest.DebitCardId;
            }
            else
            {
                debitCardFilter.IsDefault = true;
            }
            #endregion
            var selectedDebitCard = await _debitCardRepository.GetDebitCardAsNoTracking(debitCardFilter).FirstOrDefaultAsync();
            #region token apiye istek atılarak kart bilgileri getiriliyor
            if (selectedDebitCard != null)
            {
                #region request dto oluşturuluyor
                GetDebitCardIdentityRequestDto getDebitCardIdentityRequest = new GetDebitCardIdentityRequestDto();
                getDebitCardIdentityRequest.GuidList = new List<string>() { selectedDebitCard.DebitCardIdentityGuiId };
                #endregion
                var response = await _debitCardIdentityClientService.GetDebitCardIdentity(getDebitCardIdentityRequest);
                #region response dto ya setleniyor
                if (response.ResultType == ResultType.Ok)
                {
                    if (response.Data.DebitCardIdentityList.FirstOrDefault() != null)
                    {
                        #region response dto setleniyor
                        var selectedDebitCardIdentity = response.Data.DebitCardIdentityList.Where(x => x.GuiId == selectedDebitCard.DebitCardIdentityGuiId).FirstOrDefault();
                        if (selectedDebitCardIdentity != null)
                        {
                            preparePaymentFormResponse.SelectedDebitCard = _mapper.Map<MobilPaymentDebitCardItemDto>(selectedDebitCardIdentity, opt =>
                            {
                                opt.AfterMap((src, dest) =>
                                {
                                    var destData = dest as MobilPaymentDebitCardItemDto;
                                    destData.Id = selectedDebitCard.Id;
                                    destData.CardName = selectedDebitCard.CardName;
                                });
                            });
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
            #endregion
            #region cüzdan getiriliyor ve response dto ya setleniyor
            #region security key oluşturuluyor
            var securityKey = _utilService.GetRandomString(128);
            #endregion
            #region cüzdan getiriliyor
            #region filter dto oluşturuluyor
            WalletInfoFilterDto walletInfoFilter = new WalletInfoFilterDto();
            walletInfoFilter.UserId = _userContextProvider.UserId;
            #endregion
            var walletInfo = await _walletInfoRepository.GetWalletInfo(walletInfoFilter, include: source => source
             .Include(a => a.WalletForm), false).FirstOrDefaultAsync();
            #endregion
            #region cüzdan ekleniyor
            if(walletInfo == null)
            {
                #region bank apiye istek atılarak cüzdan verisi oluşturuluyor
                #region request dto oluşturuluyor
                CreateWalletRequestDto createWalletRequest = new CreateWalletRequestDto();
                createWalletRequest.MobilUserGuiId = _userContextProvider.MobilUserGuiId;
                #endregion
                var createWalletResponse = await _walletClientService.CreateWallet(createWalletRequest);
                #endregion
                if (createWalletResponse.ResultType == ResultType.Ok)
                {
                    #region wallet form oluşturuluyor
                    WalletForm walletForm = new WalletForm();
                    walletForm.CreatedDate = DateTime.Now;
                    walletForm.SecurityKey = securityKey;
                    #endregion
                    #region wallet info oluşturuluyor
                    walletInfo = _mapper.Map<WalletInfo>(createWalletResponse.Data, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            var destData = dest as WalletInfo;
                            destData.UserId = _userContextProvider.UserId.GetValueOrDefault();
                            destData.CreatedDate = DateTime.Now;
                            destData.WalletAmount = 0;
                            destData.WalletForm = walletForm;
                        });
                    });
                    await _walletInfoRepository.InsertAsync(walletInfo);
                    #endregion
                }
            }
            #endregion
            if (walletInfo != null)
            {
                #region wallet form güncelleniyor
                if (walletInfo.Id != 0)
                {
                    _walletFormRepository.UpdateWithProperties(walletInfo.WalletForm, new Expression<Func<WalletForm, object>>[] {
                        s => s.SecurityKey,
                        s => s.UpdatedDate
                    });
                    walletInfo.WalletForm.SecurityKey = securityKey;
                    walletInfo.WalletForm.UpdatedDate = DateTime.Now;
                }
                #endregion
                #region response dto ya setleniyor
                preparePaymentFormResponse.WalletInfo = _mapper.Map<MobilWalletInfoDto>(walletInfo);
                #endregion
            }
            #region veritabanına kayıt ediliyor
            await _walletInfoRepository.SaveChangesAsync();
            #endregion
            #endregion
            #region şarj işlemi getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = new ChargeFilterDto();
            chargeFilter.UserId = _userContextProvider.UserId;
            chargeFilter.Id = preparePaymentFormRequest.ChargeId;
            chargeFilter.GuiId = preparePaymentFormRequest.ChargeGuiId;
            #endregion
            var charge = await _chargeRepository.GetCharge(chargeFilter,null).FirstOrDefaultAsync();
            if (charge != null)
            {
                #region response dto setleniyor
                preparePaymentFormResponse.Price = charge.CalculatedPrice.GetValueOrDefault();
                preparePaymentFormResponse.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                preparePaymentFormResponse.Discount = charge.Discount;
                preparePaymentFormResponse.ChargeState = charge.State;
                #endregion
            }
            #region şarj işlemi bulunamadı
            else
            {
                return new ErrorResult<PreparePaymentFormResponseDto>(preparePaymentFormResponse, MobilPaymentInfoErrorEnum.CHARGE_PROCESS_NOT_FOUND);
            }
            #endregion
            #endregion
            return new SuccessResult<PreparePaymentFormResponseDto>(preparePaymentFormResponse);
        }
        /// <summary>
        /// şarj işlemi için ödeme alınıyor
        /// </summary>
        /// <param name="getMobilPaymentChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilPaymentChargeResponseDto>> GetPaymentCharge(GetMobilPaymentChargeRequestDto getMobilPaymentChargeRequest)
        {
            #region response dto oluşturuluyor
            GetMobilPaymentChargeResponseDto getMobilPaymentChargeResponse = new GetMobilPaymentChargeResponseDto();
            getMobilPaymentChargeResponse.Is3DSecurity = getMobilPaymentChargeRequest.Is3DSecurity.GetValueOrDefault();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region charge çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(getMobilPaymentChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.UserId = _userContextProvider.UserId; 
                    destData.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_FAIL };
                });
            });
            #endregion
            var charge = await _chargeRepository.GetCharge(chargeFilter, include: source => source
           .Include(x => x.PaymentInfo)
           .Include(x => x.Firm)
            .Include(x => x.ChargeDeviceConnector).ThenInclude(x => x.ChargeDevicePowerType)
           .Include(x => x.ChargeDeviceConnector).ThenInclude(x => x.ChargeDevice).ThenInclude(x => x.Station).ThenInclude(x => x.StationPaymentMethod),false).FirstOrDefaultAsync();
            #endregion
            if (charge != null)
            {
                #region ödeme işlemi bulunmaktadır
                if (charge.PaymentInfo != null)
                {
                    #region ödeme işlemi başarısız değil
                    if (charge.PaymentInfo.PaymentStatus != PaymentStatusEnum.FAILURE)
                    {
                        #region şarj işlemi güncelleniyor
                        charge.State = ChargeStateEnum.COMPLETED;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _chargeRepository.SaveChangesAsync();
                        #endregion
                        return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_COMPLETED_ALREADY);
                    }
                    #endregion
                    else
                    {
                        #region şarj işlemi güncelleniyor
                        charge.State = ChargeStateEnum.PAYMENT_FAIL;
                        #endregion
                        #region payment info güncelleniyor
                        #region mevcut işlem çıkarılıyor
                        _paymentInfoRepository.UpdateWithProperties(charge.PaymentInfo, new Expression<Func<PaymentInfo, object>>[] {
                                s => s.ChargeId,
                            });
                        charge.PaymentInfo.ChargeId = null;
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region  kullanıcı için adres ve kullanıcı bilgisi alınıyor
                #region kullanıcı çekiliyor
                #region filter dto oluşturuluyor
                UserMobilFilterDto userFilter = new UserMobilFilterDto();
                userFilter.Id = _userContextProvider.UserId;
                #endregion
                var user = await _userRepository.GetUserAsNoTracking(userFilter).FirstOrDefaultAsync();
                #endregion
                #region kullanıcı adresi çekiliyor
                #region filter dto oluşturuluyor
                UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                if (getMobilPaymentChargeRequest.BillingAddressId == null)
                {
                    userAddressFilter.IsDefault = true;
                }
                else
                {
                    userAddressFilter.Id = getMobilPaymentChargeRequest.BillingAddressId;
                }
                userAddressFilter.UserId = _userContextProvider.UserId;
                #endregion
                var userAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                #endregion
                #region kullanıcı bulunamadı
                if (user == null)
                {
                    return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.USER_CAN_NOT_FOUND);
                }
                #endregion
                #region kullanıcı adresi bulunamadı
                if (userAddress == null)
                {
                    return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.BILLING_ADDRESS_NOT_FOUND);
                }
                #endregion
                #endregion
                #region istasyon için ödeme metodu geçerli mi kontrol ediliyor
                var stationPaymentMethods = charge.ChargeDeviceConnector.ChargeDevice.Station.StationPaymentMethod.ToList();
                #region istasyon için ödeme metodu geçersiz
                if (stationPaymentMethods.Count > 0 && !stationPaymentMethods.Any(x => x.PaymentMethodId == (long)getMobilPaymentChargeRequest.PaymentMethod))
                {
                    return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED);
                }
                #endregion
                #endregion
                #region tocken a istek atılarak kayıtlı kart bilgileri getiriliyor
                GetDebitCardIdentityForPaymentResponseDto getDebitCardIdentityForPaymentResponseData = null;
                DebitCard debitCard = null;
                #region filter dto oluşturuluyor
                DebitCardFilterDto debitCardFilter = new DebitCardFilterDto();
                debitCardFilter.GuiId = getMobilPaymentChargeRequest.RegisteredCardGuiId;
                debitCardFilter.UserId = _userContextProvider.UserId;
                #endregion
                debitCard = await _debitCardRepository.GetDebitCardAsNoTracking(debitCardFilter).FirstOrDefaultAsync();
                if (debitCard != null)
                {
                    Result<GetDebitCardIdentityForPaymentResponseDto> getDebitCardIdentityForPaymentResponse = await GetDebitCardIdentityForPayment(debitCard);
                    if (getDebitCardIdentityForPaymentResponse.ResultType != ResultType.Error)
                    {
                        getDebitCardIdentityForPaymentResponseData = getDebitCardIdentityForPaymentResponse.Data;
                    }
                    else
                    {
                        if (getDebitCardIdentityForPaymentResponse.ErrorCode == (int)DebitCardIdentityErrorEnum.DEBIT_CARD_IS_NOT_FOUND)
                        {
                            return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.DEBIT_CARD_IDENTITY_IS_NOT_FOUND);
                        }
                        else
                        {
                            return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED_WHILE_GET_DEBIT_CARD_IDENTITY);
                        }
                    }
                }
                else
                {
                    return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND);
                }
                #endregion
                #region processing user address ekleniyor
                ProcessingUserAdress insertedProcessingAddress = await CreateProcessingUserAdress(userAddress);
                #endregion
                #region response dto setleniyor
                getMobilPaymentChargeResponse.Is3DSecurity = getMobilPaymentChargeRequest.Is3DSecurity.GetValueOrDefault();
                #endregion
                #region kredi kartı ile ödeme yapılıyor
                if (getMobilPaymentChargeRequest.PaymentMethod == PaymentMethodEnum.DEBIT_CARD)
                {
                    #region 3d ile ödeme yapılıyor
                    if (getMobilPaymentChargeRequest.Is3DSecurity.GetValueOrDefault())
                    {
                        #region request dto oluşturuluyor
                        PaymentStartDebitCard3DRequestDto paymentStartDebitCard3DRequest = _mapper.Map<PaymentStartDebitCard3DRequestDto>(getMobilPaymentChargeRequest, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as PaymentStartDebitCard3DRequestDto;
                                destData.FirmGuiId = charge.Firm.GuiId;
                                destData.PaymentReason = PaymentReasonEnum.CHARGE;
                                destData.Price = charge.CalculatedPrice.GetValueOrDefault();
                                destData.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                                destData.UserAdressGuiId = userAddress.GuiId;
                                destData.UserAdressId = userAddress.Id;
                                destData.UserGuiId = user.MobilUserGuiId;
                                destData.ProcessingUserAdressId = insertedProcessingAddress.Id;
                                destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                                destData.ConnectionId = _userContextProvider.ConnectionId;
                                destData.Kdv = charge.Kdv;
                                destData.ChargeGuiId = charge.GuiId;
                                destData.ChargeId = charge.Id;
                                destData.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                                destData.DebitCardId = debitCard.Id;
                                destData.PaymentChargeInfoJson = JsonSerializer.Serialize(new PaymentChargeInfoDto()
                                {
                                    ChargeDevicePowerType = charge.ChargeDeviceConnector.ChargeDevicePowerType.Text,
                                    ChargeStartTime = charge.StartTime.GetValueOrDefault(),
                                    LoadedKw = charge.LoadedKw.GetValueOrDefault(),
                                    KwPrice = charge.KwPrice,
                                    kWh = charge.ChargeDeviceConnector.kW,
                                    ChargeEndTime = charge.EndTime,
                                });
                                #region adres dto ya dönüştürülüyor
                                UserAdressDto userAdressDto = _mapper.Map<UserAdressDto>(userAddress);
                                #endregion
                                destData.UserAdressJson = JsonSerializer.Serialize(userAdressDto);
                                if (debitCard != null && getDebitCardIdentityForPaymentResponseData != null)
                                {
                                    destData.CardNumber = getDebitCardIdentityForPaymentResponseData.CardNumber;
                                    destData.ExpireYear = getDebitCardIdentityForPaymentResponseData.ExpireYear;
                                    destData.ExpireMonth = getDebitCardIdentityForPaymentResponseData.ExpireMonth;
                                    destData.Cvc = getDebitCardIdentityForPaymentResponseData.Cvc;
                                    destData.CardHolderName = getDebitCardIdentityForPaymentResponseData.CardHolderName;
                                }
                            });
                        });
                        #endregion
                        #region banka kartı ile 3D ödeme isteği atılıyor
                        var paymentStartDebitCard3DResponse = await _paymentClientService.PaymentStartDebitCard3D(paymentStartDebitCard3DRequest);
                        #endregion
                        #region 3D ödeme isteği başarılı
                        if (paymentStartDebitCard3DResponse.ResultType == ResultType.Ok)
                        {
                            #region response dto setleniyor
                            getMobilPaymentChargeResponse = _mapper.Map<GetMobilPaymentChargeResponseDto>(paymentStartDebitCard3DResponse.Data, opt =>
                            {
                                opt.AfterMap((src, dest) =>
                                {
                                    var destData = dest as GetMobilPaymentChargeResponseDto;
                                    destData.PaymentMethod = PaymentMethodEnum.DEBIT_CARD;
                                    destData.ChargeGuiId = charge.GuiId;
                                    destData.Is3DSecurity = true;
                                });
                            });
                            #endregion
                            return new SuccessResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse);
                        }
                        #endregion
                        #region 3D ödeme isteği başarısız
                        else
                        {
                            if (paymentStartDebitCard3DResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_WAS_COMPLETED)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_WAS_COMPLETED);
                            }
                            else
                            {
                                #region charge güncelleniyor
                                _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                                    s => s.State
                                });
                                charge.State = ChargeStateEnum.PAYMENT_FAIL;
                                await _chargeRepository.SaveChangesAsync();
                                #endregion
                                return GetPaymentChargeDebitCardErrorHandle<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, paymentStartDebitCard3DResponse.ErrorCode);
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region direk ödeme gerçekleşiyor
                    else
                    {
                        #region request dto oluşturuluyor
                        PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest = _mapper.Map<PaymentDirectDebitCardRequestDto>(getMobilPaymentChargeRequest, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as PaymentDirectDebitCardRequestDto;
                                destData.FirmGuiId = charge.Firm.GuiId;
                                destData.PaymentReason = PaymentReasonEnum.CHARGE;
                                destData.Price = charge.CalculatedPrice.GetValueOrDefault();
                                destData.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                                destData.UserAdressGuiId = userAddress.GuiId;
                                destData.UserGuiId = user.MobilUserGuiId;
                                destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                                destData.Kdv = charge.Kdv;
                                destData.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                                destData.PaymentChargeInfoJson = JsonSerializer.Serialize(new PaymentChargeInfoDto()
                                {
                                    ChargeDevicePowerType = charge.ChargeDeviceConnector.ChargeDevicePowerType.Text,
                                    ChargeStartTime = charge.StartTime.GetValueOrDefault(),
                                    LoadedKw = charge.LoadedKw.GetValueOrDefault(),
                                    KwPrice = charge.KwPrice,
                                    kWh = charge.ChargeDeviceConnector.kW,
                                    ChargeEndTime = charge.EndTime,
                                });
                                #region adres dto ya dönüştürülüyor
                                UserAdressDto userAdressDto = _mapper.Map<UserAdressDto>(userAddress);
                                #endregion
                                destData.UserAdressJson = JsonSerializer.Serialize(userAdressDto);
                                if (debitCard != null && getDebitCardIdentityForPaymentResponseData != null)
                                {
                                    destData.CardNumber = getDebitCardIdentityForPaymentResponseData.CardNumber;
                                    destData.ExpireYear = getDebitCardIdentityForPaymentResponseData.ExpireYear;
                                    destData.ExpireMonth = getDebitCardIdentityForPaymentResponseData.ExpireMonth;
                                    destData.Cvc = getDebitCardIdentityForPaymentResponseData.Cvc;
                                    destData.CardHolderName = getDebitCardIdentityForPaymentResponseData.CardHolderName;
                                }
                            });
                        });
                        #endregion
                        #region banka kartı ile direk ödeme isteği atılıyor
                        var paymentDirectDebitCardResponse = await _paymentClientService.PaymentDirectDebitCard(paymentDirectDebitCardRequest);
                        #endregion
                        #region şarj işlemi için ödeme tamamlama başarılı
                        if (paymentDirectDebitCardResponse.ResultType == ResultType.Ok)
                        {
                            #region payment info oluşturuluyor
                            PaymentInfo paymentInfo = new PaymentInfo();
                            paymentInfo.Price = charge.CalculatedPrice.GetValueOrDefault();
                            paymentInfo.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                            paymentInfo.PaymentGuiId = paymentDirectDebitCardResponse.Data.PaymentGuiId;
                            paymentInfo.Discount = charge.Discount;
                            paymentInfo.PaymentReason = PaymentReasonEnum.CHARGE;
                            paymentInfo.PaymentMethod = PaymentMethodEnum.DEBIT_CARD;
                            paymentInfo.PaymentBankType = paymentDirectDebitCardResponse.Data.PaymentBankType;
                            paymentInfo.CompletedDate = paymentDirectDebitCardResponse.Data.CompletedDate;
                            paymentInfo.Kdv = charge.Kdv;
                            paymentInfo.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                            paymentInfo.UserId = _userContextProvider.UserId;
                            paymentInfo.UserAdressId = userAddress.Id;
                            paymentInfo.ChargeId = charge.Id;
                            paymentInfo.ProcessingUserAdressId = insertedProcessingAddress.Id;
                            paymentInfo.DebitCardId = debitCard.Id;
                            await _paymentInfoRepository.InsertAsync(paymentInfo);
                            #endregion
                            #region charge güncelleniyor
                            _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                                s => s.State,
                            });
                            charge.State = ChargeStateEnum.COMPLETED;
                            #endregion
                            #region veritabanına kayıt ediliyor
                            await _paymentInfoRepository.SaveChangesAsync();
                            #endregion
                            #region response dto setleniyor
                            getMobilPaymentChargeResponse.Is3DSecurity = false;
                            #endregion
                            return new SuccessResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse);
                        }
                        #endregion
                        #region şarj işlemi için ödeme tamamlama başarısız
                        else
                        {
                            #region charge process güncelleniyor
                            _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                                s => s.State,
                            });
                            charge.State = ChargeStateEnum.PAYMENT_FAIL;
                            #endregion
                            #region veritabanına kayıt ediliyor
                            await _paymentInfoRepository.SaveChangesAsync();
                            #endregion
                            return GetPaymentChargeDebitCardErrorHandle<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, paymentDirectDebitCardResponse.ErrorCode);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region cüzdan ile ödeme yapılıyor
                else if (getMobilPaymentChargeRequest.PaymentMethod == PaymentMethodEnum.WALLET)
                {
                    #region cüzdan getiriliyor
                    var walletInfo = await _walletInfoRepository.GetWalletInfo(new WalletInfoFilterDto() { UserId = _userContextProvider.UserId, SecurityKey = getMobilPaymentChargeRequest.WalletSecurityKey }, include: source => source
             .Include(a => a.WalletForm), false).FirstOrDefaultAsync();
                    #endregion
                    if (walletInfo != null)
                    {
                        #region ödenecek tutar belirleniyor
                        var paidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                        #endregion
                        #region cüzdan bakiyesi kontrol ediliyor
                        #region cüzdan bakiyesi yetersiz
                        if (walletInfo == null || walletInfo.WalletAmount < paidPrice)
                        {
                            return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.WALLET_AMOUNT_INSUFFICIENT);
                        }
                        #endregion
                        #endregion
                        #region wallet process verisi oluşturuluyor ve kayıt ediliyor
                        WalletProcess walletProcess = new WalletProcess();
                        walletProcess.WalletInfoId = walletInfo.Id;
                        walletProcess.ProcessType = WalletProcessTypeEnum.SPEND;
                        walletProcess.CreatedDate = datetimeNow;
                        walletProcess.State = WalletProcessStateEnum.WAITING;
                        walletProcess.GuiId = Guid.NewGuid() + "";
                        walletProcess.WalletGuiId = walletInfo.WalletGuiId;
                        walletProcess.Amount = paidPrice;
                        walletProcess.SecurityKey = _utilService.GetRandomString(128);
                        walletProcess.WalletTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + walletInfo.User.MobilUserGuiId + "/" + _configuration.GetSection("Wallet:TockenKey").Value);
                        walletProcess.AmountTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + paidPrice + "/" + walletProcess.WalletTockenGuiId);
                        var insertedWalletProcess = await _walletProcessRepository.InsertAsync(walletProcess);
                        await _walletProcessRepository.SaveChangesAsync();
                        #endregion
                        #region cüzdan ile ödeme gerçekleştiriliyor
                        #region request dto oluşturuluyor
                        PaymentWalletRequestDto chargePaymentWalletRequest = _mapper.Map<PaymentWalletRequestDto>(getMobilPaymentChargeRequest, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as PaymentWalletRequestDto;
                                destData.FirmGuiId = charge.Firm.GuiId;
                                destData.UserGuiId = user.MobilUserGuiId;
                                destData.UserAdressGuiId = userAddress.GuiId;
                                destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                                destData.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                                destData.Discount = charge.Discount;
                                destData.Price = charge.CalculatedPrice.GetValueOrDefault();
                                destData.WalletGuiId = walletInfo.WalletGuiId;
                                destData.Kdv = charge.Kdv;
                                destData.WalletAmountTockenGuiId = walletProcess.AmountTockenGuiId;
                                destData.WalletTockenGuiId = walletProcess.WalletTockenGuiId;
                                destData.WalletProcessGuiId = walletProcess.GuiId;
                                destData.PaymentReason = PaymentReasonEnum.CHARGE;
                                destData.PaymentChargeInfoJson = JsonSerializer.Serialize(new PaymentChargeInfoDto()
                                {
                                    ChargeDevicePowerType = charge.ChargeDeviceConnector.ChargeDevicePowerType.Text,
                                    ChargeStartTime = charge.StartTime.GetValueOrDefault(),
                                    LoadedKw = charge.LoadedKw.GetValueOrDefault(),
                                    KwPrice = charge.KwPrice,
                                    kWh = charge.ChargeDeviceConnector.kW,
                                    ChargeEndTime = charge.EndTime,
                                });
                                #region adres dto ya dönüştürülüyor
                                UserAdressDto userAdressDto = _mapper.Map<UserAdressDto>(userAddress);
                                #endregion
                                destData.UserAdressJson = JsonSerializer.Serialize(userAdressDto);
                            });
                        });
                        #endregion
                        var paymentWalletResponse = await _paymentClientService.PaymentWallet(chargePaymentWalletRequest);
                        #endregion
                        #region ödeme işlemi başarılı
                        if (paymentWalletResponse.ResultType == ResultType.Ok)
                        {
                            #region cüzdan verileri oluşturuluyor / güncelleniyor
                            #region wallet spend money ekleniyor
                            WalletSpendMoneyInfo walletSpendMoneyInfo = new WalletSpendMoneyInfo();
                            walletSpendMoneyInfo.Date = datetimeNow;
                            walletSpendMoneyInfo.SpendAmount = paidPrice;
                            walletSpendMoneyInfo.WalletProcessId = insertedWalletProcess.Id;
                            walletSpendMoneyInfo.WalletSpendMoneyGuiId = paymentWalletResponse.Data.WalletSpendMoneyGuiId;
                            #endregion
                            #region security key oluşturuluyor
                            var securityKey = _utilService.GetRandomString(128);
                            #endregion
                            #region wallet form güncelleniyor
                            var walletForm = walletInfo.WalletForm;
                            _walletFormRepository.UpdateWithProperties(walletInfo.WalletForm, new Expression<Func<WalletForm, object>>[] {
                                s => s.SecurityKey,
                                s => s.UpdatedDate
                            });
                            walletInfo.WalletForm.SecurityKey = securityKey;
                            walletInfo.WalletForm.UpdatedDate = datetimeNow;
                            #endregion
                            #region wallet amount güncelleniyor
                            _walletInfoRepository.UpdateWithProperties(walletInfo, new Expression<Func<WalletInfo, object>>[] {
                                s => s.WalletAmount,
                            });
                            walletInfo.WalletAmount = walletInfo.WalletAmount - paidPrice;
                            #endregion
                            #endregion
                            #region payment info oluşturuluyor
                            PaymentInfo paymentInfo = new PaymentInfo();
                            paymentInfo.Price = charge.CalculatedPrice.GetValueOrDefault();
                            paymentInfo.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                            paymentInfo.PaymentGuiId = paymentWalletResponse.Data.PaymentGuiId;
                            paymentInfo.Discount = charge.Discount;
                            paymentInfo.PaymentReason = PaymentReasonEnum.CHARGE;
                            paymentInfo.PaymentMethod = PaymentMethodEnum.WALLET;
                            paymentInfo.CompletedDate = paymentWalletResponse.Data.CompletedDate;
                            paymentInfo.Kdv = charge.Kdv;
                            paymentInfo.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                            paymentInfo.UserId = _userContextProvider.UserId;
                            paymentInfo.UserAdressId = userAddress.Id;
                            paymentInfo.ChargeId = charge.Id;
                            paymentInfo.ProcessingUserAdressId = insertedProcessingAddress.Id;
                            paymentInfo.WalletSpendMoneyInfo = walletSpendMoneyInfo;

                            await _paymentInfoRepository.InsertAsync(paymentInfo);
                            #endregion
                            #region charge process güncelleniyor
                            _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                                s => s.State,
                            });
                            charge.State = ChargeStateEnum.COMPLETED;
                            #endregion
                            #region wallet process güncelleniyor
                            _walletProcessRepository.UpdateWithProperties(insertedWalletProcess, new Expression<Func<WalletProcess, object>>[] {
                                s => s.State,
                                s => s.CompletedDate
                            });
                            insertedWalletProcess.State = WalletProcessStateEnum.COMPLETED;
                            insertedWalletProcess.CompletedDate = paymentWalletResponse.Data.CompletedDate;
                            #endregion
                            #region veritabanına kayıt ediliyor
                            await _paymentInfoRepository.SaveChangesAsync();
                            #endregion
                            #region response dto setleniyor
                            getMobilPaymentChargeResponse.PaymentMethod = PaymentMethodEnum.WALLET;
                            getMobilPaymentChargeResponse.ChargeGuiId = charge.GuiId;
                            #endregion
                            return new SuccessResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse);
                        }
                        #endregion
                        #region ödeme işlemi başarısız
                        else
                        {
                            #region charge ve wallet process güncelleniyor
                            if (paymentWalletResponse.ErrorCode != (int)PaymentErrorEnum.PAYMENT_WAS_COMPLETED)
                            {
                                _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                                    s => s.State,
                                });
                                charge.State = ChargeStateEnum.PAYMENT_FAIL;
                                #region wallet process güncelleniyor
                                _walletProcessRepository.UpdateWithProperties(insertedWalletProcess, new Expression<Func<WalletProcess, object>>[] {
                                    s => s.State,
                                    s => s.CompletedDate
                                });
                                insertedWalletProcess.State = WalletProcessStateEnum.FAILED;
                                insertedWalletProcess.CompletedDate = datetimeNow;
                                #endregion
                                await _chargeRepository.SaveChangesAsync();
                            }
                            #endregion
                            if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.WALLET_NOT_FOUND)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.WALLET_NOT_FOUND);
                            }
                            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND);
                            }
                            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.INVALID_PAYMENT_REASON)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.INVALID_PAYMENT_REASON);
                            }
                            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_WAS_COMPLETED)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_WAS_COMPLETED);
                            }
                            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.WALLET_AMOUNT_INSUFFICIENT)
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.WALLET_AMOUNT_INSUFFICIENT);
                            }
                            else
                            {
                                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.WALLET_INFO_NOT_FOUND);
                    }
                }
                #endregion
                #region ödeme yöntemi seçilmemiş
                else
                {
                    return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SELECTED);
                }
                #endregion
            }
            #region şarj işlemi bulunamadı
            else
            {
                return new ErrorResult<GetMobilPaymentChargeResponseDto>(getMobilPaymentChargeResponse, MobilPaymentInfoErrorEnum.CHARGE_INFO_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// cüzdana para ekleme için ödeme alınıyor
        /// </summary>
        /// <param name="getMobilPaymentAddBalanceWalletRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilPaymentAddBalanceWalletResponseDto>> GetPaymentAddBalanceWallet(GetMobilPaymentAddBalanceWalletRequestDto getMobilPaymentAddBalanceWalletRequest)
        {
            #region response dto oluşturuluyor
            GetMobilPaymentAddBalanceWalletResponseDto getMobilPaymentAddBalanceWalletResponse = new GetMobilPaymentAddBalanceWalletResponseDto();
            getMobilPaymentAddBalanceWalletResponse.Is3DSecurity = getMobilPaymentAddBalanceWalletRequest.Is3DSecurity;
            #endregion
            #region şimdiki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region cüzdan getiriliyor
            #region filter dto oluşturuluyor
            WalletInfoFilterDto walletFilter = _mapper.Map<WalletInfoFilterDto>(getMobilPaymentAddBalanceWalletRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as WalletInfoFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            var walletInfo = await _walletInfoRepository.GetWalletInfo(walletFilter, include: source => source
             .Include(a => a.WalletForm)).FirstOrDefaultAsync();
            #endregion
            if (walletInfo != null)
            {
                #region response dto setleniyor
                getMobilPaymentAddBalanceWalletResponse.Is3DSecurity = getMobilPaymentAddBalanceWalletRequest.Is3DSecurity;
                #endregion
                #region  kullanıcı için adres ve kullanıcı bilgisi alınıyor
                #region kullanıcı çekiliyor
                #region filter dto oluşturuluyor
                UserMobilFilterDto userFilter = new UserMobilFilterDto();
                userFilter.Id = _userContextProvider.UserId;
                #endregion
                var user = await _userRepository.GetUserAsNoTracking(userFilter).FirstOrDefaultAsync();
                #endregion
                #region kullanıcı adresi çekiliyor
                #region filter dto oluşturuluyor
                UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                userAddressFilter.UserId = _userContextProvider.UserId;
                userAddressFilter.IsDefault = true;
                #endregion
                var userAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                #endregion
                #region kullanıcı bulunamadı
                if (user == null)
                {
                    return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.USER_CAN_NOT_FOUND);
                }
                #endregion
                #region kullanıcı adresi bulunamadı
                if (userAddress == null)
                {
                    return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.BILLING_ADDRESS_NOT_FOUND);
                }
                #endregion
                #endregion
                #region kayıtlı kart seçili ise tocken a istek atılarak kayıtlı kart bilgileri getiriliyor
                #region tocken a istek atılarak kayıtlı kart bilgileri getiriliyor
                GetDebitCardIdentityForPaymentResponseDto getDebitCardIdentityForPaymentResponseData = null;
                DebitCard? debitCard = null;
                if (getMobilPaymentAddBalanceWalletRequest.IsRegisteredCard)
                {
                    #region filter dto oluşturuluyor
                    DebitCardFilterDto debitCardFilter = new DebitCardFilterDto();
                    debitCardFilter.GuiId = getMobilPaymentAddBalanceWalletRequest.RegisteredCardGuiId;
                    debitCardFilter.UserId = _userContextProvider.UserId;
                    #endregion
                    debitCard = await _debitCardRepository.GetDebitCardAsNoTracking(debitCardFilter).FirstOrDefaultAsync();
                    if (debitCard != null)
                    {
                        Result<GetDebitCardIdentityForPaymentResponseDto> getDebitCardIdentityForPaymentResponse = await GetDebitCardIdentityForPayment(debitCard);
                        if (getDebitCardIdentityForPaymentResponse.ResultType != ResultType.Error)
                        {
                            getDebitCardIdentityForPaymentResponseData = getDebitCardIdentityForPaymentResponse.Data;
                        }
                        else
                        {
                            if (getDebitCardIdentityForPaymentResponse.ErrorCode == (int)DebitCardIdentityErrorEnum.DEBIT_CARD_IS_NOT_FOUND)
                            {
                                return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.DEBIT_CARD_IDENTITY_IS_NOT_FOUND);
                            }
                            else
                            {
                                return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED_WHILE_GET_DEBIT_CARD_IDENTITY);
                            }
                        }
                    }
                    else
                    {
                        return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND);
                    }
                    #endregion
                }
                #endregion
                #region processing user address ekleniyor
                ProcessingUserAdress insertedProcessingAddress = await CreateProcessingUserAdress(userAddress);
                #endregion
                #region wallet process verisi oluşturuluyor ve kayıt ediliyor
                WalletProcess walletProcess = new WalletProcess();
                walletProcess.WalletInfoId = walletInfo.Id;
                walletProcess.CreatedDate = datetimeNow;
                walletProcess.State = WalletProcessStateEnum.WAITING;
                walletProcess.GuiId = Guid.NewGuid() + "";
                walletProcess.WalletGuiId = walletInfo.WalletGuiId;
                walletProcess.Amount = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                walletProcess.SecurityKey = getMobilPaymentAddBalanceWalletRequest.SecurityKey;
                walletProcess.WalletTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + _userContextProvider.MobilUserGuiId + "/" + _configuration.GetSection("Wallet:TockenKey").Value);
                walletProcess.AmountTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + getMobilPaymentAddBalanceWalletRequest.PushAmount + "/" + walletProcess.WalletTockenGuiId);
                WalletProcess walletProcessEntity = _mapper.Map<WalletProcess>(walletProcess);
                var insertedWalletProcessEntity = await _walletProcessRepository.InsertAsync(walletProcessEntity);
                await _walletProcessRepository.SaveChangesAsync();
                #endregion
                #region 3d ile ödeme yapılıyor
                if (getMobilPaymentAddBalanceWalletRequest.Is3DSecurity)
                {
                    #region request dto oluşturuluyor
                    PaymentStartDebitCard3DRequestDto paymentStartDebitCard3DRequest = _mapper.Map<PaymentStartDebitCard3DRequestDto>(getMobilPaymentAddBalanceWalletRequest, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            var destData = dest as PaymentStartDebitCard3DRequestDto;
                            destData.PaymentReason = PaymentReasonEnum.WALLET;
                            destData.Price = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                            destData.PaidPrice = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                            destData.UserAdressGuiId = userAddress.GuiId;
                            destData.UserAdressId = userAddress.Id;
                            destData.UserGuiId = user.MobilUserGuiId;
                            destData.ProcessingUserAdressId = insertedProcessingAddress.Id;
                            destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                            destData.ConnectionId = _userContextProvider.ConnectionId;
                            destData.WalletTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + _userContextProvider.MobilUserGuiId + "/" + _configuration.GetSection("Wallet:TockenKey").Value);
                            destData.WalletAmountTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + getMobilPaymentAddBalanceWalletRequest.PushAmount + "/" + walletProcess.WalletTockenGuiId);
                            destData.WalletGuiId = walletInfo.WalletGuiId;
                            destData.WalletProcessGuiId = walletProcess.GuiId;
                            if (debitCard != null && getDebitCardIdentityForPaymentResponseData != null)
                            {
                                destData.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                                destData.DebitCardId = debitCard.Id;
                                destData.CardNumber = getDebitCardIdentityForPaymentResponseData.CardNumber;
                                destData.ExpireYear = getDebitCardIdentityForPaymentResponseData.ExpireYear;
                                destData.ExpireMonth = getDebitCardIdentityForPaymentResponseData.ExpireMonth;
                                destData.Cvc = getDebitCardIdentityForPaymentResponseData.Cvc;
                                destData.CardHolderName = getDebitCardIdentityForPaymentResponseData.CardHolderName;
                            }
                        });
                    });
                    #endregion
                    #region banka kartı ile 3D ödeme isteği atılıyor
                    var paymentStartDebitCard3DResponse = await _paymentClientService.PaymentStartDebitCard3D(paymentStartDebitCard3DRequest);
                    #endregion
                    #region 3D ödeme isteği başarılı
                    if (paymentStartDebitCard3DResponse.ResultType == ResultType.Ok)
                    {
                        #region response dto setleniyor
                        getMobilPaymentAddBalanceWalletResponse = _mapper.Map<GetMobilPaymentAddBalanceWalletResponseDto>(paymentStartDebitCard3DResponse.Data, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as GetMobilPaymentAddBalanceWalletResponseDto;
                                destData.Is3DSecurity = true;
                            });
                        });
                        #endregion
                    }
                    #endregion
                    #region 3D ödeme isteği başarısız
                    else
                    {
                        if (paymentStartDebitCard3DResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_WAS_COMPLETED)
                        {
                            return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.PAYMENT_WAS_COMPLETED);
                        }
                        else
                        {
                            #region wallet process güncelleniyor
                            _walletProcessRepository.UpdateWithProperties(insertedWalletProcessEntity, new Expression<Func<WalletProcess, object>>[] {
                                s => s.State,
                                s => s.CompletedDate
                            });
                            insertedWalletProcessEntity.State = WalletProcessStateEnum.FAILED;
                            insertedWalletProcessEntity.CompletedDate = datetimeNow;
                            await _walletProcessRepository.SaveChangesAsync();
                            #endregion
                            return GetPaymentChargeDebitCardErrorHandle<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, paymentStartDebitCard3DResponse.ErrorCode);
                        }
                    }
                    #endregion
                }
                #endregion
                #region direk ödeme gerçekleşiyor
                else
                {
                    _logger.LogError("hata1");
                    #region request dto oluşturuluyor
                    PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest = _mapper.Map<PaymentDirectDebitCardRequestDto>(getMobilPaymentAddBalanceWalletRequest, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            var destData = dest as PaymentDirectDebitCardRequestDto;
                            destData.PaymentReason = PaymentReasonEnum.WALLET;
                            destData.Price = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                            destData.PaidPrice = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                            destData.UserAdressGuiId = userAddress.GuiId;
                            destData.UserGuiId = user.MobilUserGuiId;
                            destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                            destData.WalletTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + _userContextProvider.MobilUserGuiId + "/" + _configuration.GetSection("Wallet:TockenKey").Value);
                            destData.WalletAmountTockenGuiId = _utilService.Base64Encode(walletInfo.WalletGuiId + "/" + getMobilPaymentAddBalanceWalletRequest.PushAmount + "/" + walletProcess.WalletTockenGuiId);
                            destData.WalletGuiId = walletInfo.WalletGuiId;
                            destData.WalletProcessGuiId = walletProcess.GuiId;
                            if (debitCard != null && getDebitCardIdentityForPaymentResponseData != null)
                            {
                                destData.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                                destData.CardNumber = getDebitCardIdentityForPaymentResponseData.CardNumber;
                                destData.ExpireYear = getDebitCardIdentityForPaymentResponseData.ExpireYear;
                                destData.ExpireMonth = getDebitCardIdentityForPaymentResponseData.ExpireMonth;
                                destData.Cvc = getDebitCardIdentityForPaymentResponseData.Cvc;
                                destData.CardHolderName = getDebitCardIdentityForPaymentResponseData.CardHolderName;
                            }
                        });
                    });
                    #endregion
                    _logger.LogError("hata2");
                    #region banka kartı ile direk ödeme isteği atılıyor
                    var paymentDirectDebitCardResponse = await _paymentClientService.PaymentDirectDebitCard(paymentDirectDebitCardRequest);
                    #endregion
                    #region ödeme işlemi başarılı
                    if (paymentDirectDebitCardResponse.ResultType == ResultType.Ok)
                    {
                        _logger.LogError("hata3");
                        #region payment info oluşturuluyor
                        PaymentInfo paymentInfo = new PaymentInfo();
                        paymentInfo.Price = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                        paymentInfo.PaidPrice = getMobilPaymentAddBalanceWalletRequest.PushAmount;
                        paymentInfo.PaymentGuiId = paymentDirectDebitCardResponse.Data.PaymentGuiId;
                        paymentInfo.Discount = 0;
                        paymentInfo.PaymentReason = PaymentReasonEnum.WALLET;
                        paymentInfo.PaymentMethod = PaymentMethodEnum.DEBIT_CARD;
                        paymentInfo.PaymentBankType = paymentDirectDebitCardResponse.Data.PaymentBankType;
                        paymentInfo.CompletedDate = paymentDirectDebitCardResponse.Data.CompletedDate;
                        paymentInfo.Kdv = 0;
                        paymentInfo.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                        paymentInfo.UserId = _userContextProvider.UserId;
                        paymentInfo.UserAdressId = userAddress.Id;
                        paymentInfo.ProcessingUserAdressId = insertedProcessingAddress.Id;
                        paymentInfo.DebitCardId = debitCard != null ? debitCard.Id : null;
                        #endregion
                        _logger.LogError("hata4");
                        #region walllet push money verisi ekleniyor
                        WalletPushMoneyInfo walletPushMoneyInfo = new WalletPushMoneyInfo();
                        walletPushMoneyInfo.GuiId = Guid.NewGuid() + "";
                        walletPushMoneyInfo.WalletPushMoneyGuiId = paymentDirectDebitCardResponse.Data.WalletPushMoneyGuiId;
                        walletPushMoneyInfo.Date = paymentDirectDebitCardResponse.Data.CompletedDate;
                        walletPushMoneyInfo.WalletProcessId = walletProcessEntity.Id;
                        paymentInfo.WalletPushMoneyInfo = walletPushMoneyInfo;
                        #endregion
                        #region wallet process güncelleniyor
                        _walletProcessRepository.UpdateWithProperties(walletProcessEntity, new Expression<Func<WalletProcess, object>>[] {
                            s => s.State,
                            s => s.CompletedDate,
                        });
                        _logger.LogError("hata5");
                        walletProcessEntity.State = WalletProcessStateEnum.COMPLETED;
                        walletProcessEntity.CompletedDate = datetimeNow;
                        #endregion
                        #region wallet amount güncelleniyor
                        _walletInfoRepository.UpdateWithProperties(walletInfo, new Expression<Func<WalletInfo, object>>[] {
                            s => s.WalletAmount
                        });
                        walletInfo.WalletAmount = walletInfo.WalletAmount + walletProcess.Amount;
                        #endregion
                        _logger.LogError("hata6");
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.InsertAsync(paymentInfo);
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        _logger.LogError("hata7");
                        #region response dto setleniyor
                        getMobilPaymentAddBalanceWalletResponse = _mapper.Map<GetMobilPaymentAddBalanceWalletResponseDto>(paymentDirectDebitCardResponse.Data, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as GetMobilPaymentAddBalanceWalletResponseDto;
                                destData.Is3DSecurity = false;
                            });
                        });
                        #endregion
                        _logger.LogError("hata8");
                    }
                    #endregion
                    #region ödeme işlemi başarısız
                    else
                    {
                        #region wallet process güncelleniyor
                        _walletProcessRepository.UpdateWithProperties(insertedWalletProcessEntity, new Expression<Func<WalletProcess, object>>[] {
                            s => s.State,
                            s => s.CompletedDate
                        });
                        insertedWalletProcessEntity.State = WalletProcessStateEnum.FAILED;
                        insertedWalletProcessEntity.CompletedDate = datetimeNow;
                        await _walletProcessRepository.SaveChangesAsync();
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        return GetPaymentChargeDebitCardErrorHandle<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, paymentDirectDebitCardResponse.ErrorCode);
                    }
                    #endregion
                }
                #endregion
                #region kart kaydetme seçildiyse kart kayıt ediliyor
                if (getMobilPaymentAddBalanceWalletRequest.SaveDebitCard.GetValueOrDefault())
                {
                    #region kullanıcıya ait kayıtlı kart sayısı getiriliyor
                    #region filter dto oluşturuluyor
                    DebitCardFilterDto allDebitCardFilter = new DebitCardFilterDto();
                    allDebitCardFilter.UserId = _userContextProvider.UserId;
                    #endregion
                    var debitCardCount = await _debitCardRepository.GetDebitCardAsNoTracking(allDebitCardFilter).CountAsync();
                    #endregion
                    #region tocken apiye istek atılarak kart bilgileri ekleniyor
                    #region request dto oluşturuluyor
                    AddDebitCardIdentityRequestDto addDebitCardIdentityRequest = new AddDebitCardIdentityRequestDto();
                    addDebitCardIdentityRequest.DebitCardIdentityInfo = _mapper.Map<DebitCardIdentityInfoDto>(getMobilPaymentAddBalanceWalletRequest);
                    #endregion
                    var response = await _debitCardIdentityClientService.AddDebitCardIdentity(addDebitCardIdentityRequest);
                    #endregion
                    if (response.ResultType == ResultType.Ok)
                    {
                        #region debit card entity oluşturuluyor
                        DebitCard insertDebitCard = new DebitCard();
                        insertDebitCard.UserId = _userContextProvider.UserId.GetValueOrDefault();
                        insertDebitCard.CardName = "Kartım - " + debitCardCount +1;
                        insertDebitCard.DebitCardIdentityGuiId = response.Data.GuiId;
                        insertDebitCard.PaymentDebitCardVerificationGuiId = "";
                        #region eğer eklenen ilk kart ise default otomatik true setleniyor
                        #region filter dto oluşturuluyor
                        DebitCardFilterDto checkDebitCardFilter = new DebitCardFilterDto();
                        checkDebitCardFilter.UserId = _userContextProvider.UserId;
                        checkDebitCardFilter.IsDefault = true;
                        #endregion
                        var checkDebitCard = await _debitCardRepository.GetDebitCardAsNoTracking(checkDebitCardFilter).AnyAsync();
                        if (!checkDebitCard)
                        {
                            insertDebitCard.IsDefault = true;
                        }
                        #endregion
                        //debitCard.PaymentDebitCardVerificationGuiId = getPaymentDebitCardVerificationResponse.Data.PaymentDebitCardVerificationGuiId;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _debitCardRepository.InsertAsync(insertDebitCard);
                        await _debitCardRepository.SaveChangesAsync();
                        #endregion
                    }
                }
                #endregion
                return new SuccessResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse);
            }
            #region cüzdan bulunamadı
            else
            {
                return new ErrorResult<GetMobilPaymentAddBalanceWalletResponseDto>(getMobilPaymentAddBalanceWalletResponse, MobilPaymentInfoErrorEnum.WALLET_INFO_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// kredi kartı 3D ödeme işlemi tamamlanıyor
        /// </summary>
        /// <param name="completeMobilPaymentInfoDebitCard3DRequest"></param>
        /// <returns></returns>
        public async Task<Result<CompleteMobilPaymentInfoDebitCard3DResponseDto>> CompletePaymentInfoDebitCard3D(CompleteMobilPaymentInfoDebitCard3DRequestDto completeMobilPaymentInfoDebitCard3DRequest)
        {
            #region response dto oluşturuluyor
            CompleteMobilPaymentInfoDebitCard3DResponseDto completeMobilPaymentInfoDebitCard3DResponse = new CompleteMobilPaymentInfoDebitCard3DResponseDto();
            #endregion
            #region şarj işlemi ödemesi
            if (completeMobilPaymentInfoDebitCard3DRequest.PaymentReason == PaymentReasonEnum.CHARGE)
            {
                #region şarj bilgisi bulunamadı
                if (completeMobilPaymentInfoDebitCard3DRequest.ChargeId == null)
                {
                    return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.CHARGE_INFO_NOT_FOUND);
                }
                #endregion
                #region şarj işlemi getiriliyor
                #region filter dto oluşturuluyor
                ChargeFilterDto chargeFilter = new ChargeFilterDto();
                chargeFilter.Id = completeMobilPaymentInfoDebitCard3DRequest.ChargeId;
                #endregion
                var charge = await _chargeRepository.GetCharge(chargeFilter,null,false).FirstOrDefaultAsync();
                #endregion
                #region charge güncelleniyor
                if (charge != null)
                {
                    #region şarj işlemi için ödeme tamamlama başarılı
                    if (completeMobilPaymentInfoDebitCard3DRequest.PaymentStatus == PaymentStatusEnum.SUCCESSFUL)
                    {
                        #region payment info oluşturuluyor
                        PaymentInfo paymentInfo = _mapper.Map<PaymentInfo>(completeMobilPaymentInfoDebitCard3DRequest, opt =>
                        {
                            opt.AfterMap((src, dest) =>
                            {
                                var destData = dest as PaymentInfo;
                                destData.UserId = charge.UserId;
                            });
                        });
                        await _paymentInfoRepository.InsertAsync(paymentInfo);
                        #endregion
                        #region charge güncelleniyor
                        _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                            s => s.State,
                        });
                        charge.State = ChargeStateEnum.COMPLETED;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion

                    }
                    #endregion
                    #region şarj işlemi için ödeme tamamlama başarısız
                    else
                    {
                        #region charge process güncelleniyor
                        _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                            s => s.State,
                        });
                        charge.State = ChargeStateEnum.PAYMENT_FAIL;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.PAYMENT_FAILED);
                    }
                    #endregion
                }
                #endregion
                #region şarj işlemi bulunamadı
                else
                {
                    return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.CHARGE_PROCESS_NOT_FOUND);
                }
                #endregion
            }
            #endregion
            #region cüzdana para aktarma ödemesi
            else if (completeMobilPaymentInfoDebitCard3DRequest.PaymentReason == PaymentReasonEnum.WALLET)
            {
                #region wallet process getiriliyor
                #region filter dto oluşturuluyor
                WalletProcessFilterDto walletProcessFilter = new WalletProcessFilterDto();
                walletProcessFilter.GuiId = completeMobilPaymentInfoDebitCard3DRequest.WalletProcessGuiId;
                #endregion
                var walletProcess = await _walletProcessRepository.GetWalletProcess(walletProcessFilter).FirstOrDefaultAsync();
                #endregion
                #region wallet process bulunamadı
                if (walletProcess == null)
                {
                    return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.WALLET_PROCESS_NOT_FOUND);
                }
                #endregion
                #region ödeme işlemi başarılı
                if (completeMobilPaymentInfoDebitCard3DRequest.PaymentStatus == PaymentStatusEnum.SUCCESSFUL)
                {
                    #region payment info oluşturuluyor
                    PaymentInfo paymentInfo = _mapper.Map<PaymentInfo>(completeMobilPaymentInfoDebitCard3DRequest);
                    #endregion
                    #region walllet push money verisi ekleniyor
                    WalletPushMoneyInfo walletPushMoneyInfo = new WalletPushMoneyInfo();
                    walletPushMoneyInfo.GuiId = Guid.NewGuid() + "";
                    walletPushMoneyInfo.WalletPushMoneyGuiId = completeMobilPaymentInfoDebitCard3DRequest.WalletPushMoneyGuiId;
                    walletPushMoneyInfo.Date = completeMobilPaymentInfoDebitCard3DRequest.CompletedDate;
                    walletPushMoneyInfo.WalletProcessId = walletProcess.Id;
                    paymentInfo.WalletPushMoneyInfo = walletPushMoneyInfo;
                    #endregion
                    #region wallet process güncelleniyor
                    _walletProcessRepository.UpdateWithProperties(walletProcess, new Expression<Func<WalletProcess, object>>[] {
                        s => s.State,
                        s => s.CompletedDate,
                    });
                    walletProcess.State = WalletProcessStateEnum.COMPLETED;
                    walletProcess.CompletedDate = completeMobilPaymentInfoDebitCard3DRequest.CompletedDate;
                    #endregion
                    #region wallet amount güncelleniyor
                    _walletInfoRepository.UpdateWithProperties(walletProcess.WalletInfo, new Expression<Func<WalletInfo, object>>[] {
                        s => s.WalletAmount
                    });
                    walletProcess.WalletInfo.WalletAmount = walletProcess.WalletInfo.WalletAmount + walletProcess.Amount;
                    #endregion
                    #region veritabanına kayıt ediliyor
                    await _paymentInfoRepository.InsertAsync(paymentInfo);
                    await _paymentInfoRepository.SaveChangesAsync();
                    #endregion
                }
                #endregion
                #region ödeme işlemi başarısız
                else
                {
                    #region wallet process güncelleniyor
                    _walletProcessRepository.UpdateWithProperties(walletProcess, new Expression<Func<WalletProcess, object>>[] {
                        s => s.State,
                        s => s.CompletedDate
                    });
                    walletProcess.State = WalletProcessStateEnum.FAILED;
                    walletProcess.CompletedDate = DateTime.Now;
                    await _walletProcessRepository.SaveChangesAsync();
                    #endregion
                    return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.PAYMENT_FAILED);
                }
                #endregion
            }
            #endregion
            #region tanımlı olmayan ödeme sebebi
            else
            {
                return new ErrorResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse, MobilPaymentInfoErrorEnum.INVALID_PAYMENT_REASON);
            }
            #endregion
            return new SuccessResult<CompleteMobilPaymentInfoDebitCard3DResponseDto>(completeMobilPaymentInfoDebitCard3DResponse);
        }
        /// <summary>
        /// ödeme durumu getiriliyor
        /// </summary>
        /// <param name="getMobilPaymentInfoStatusRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilPaymentInfoStatusResponseDto>> GetPaymentInfoStatus(GetMobilPaymentInfoStatusRequestDto getMobilPaymentInfoStatusRequest)
        {
            #region response dto oluşturuluyor
            GetMobilPaymentInfoStatusResponseDto getMobilPaymentInfoStatusResponse = new GetMobilPaymentInfoStatusResponseDto();
            #endregion
            #region bank apiye istek atılarak ödeme durumu getiriliyor
            #region request dto oluşturuluyor
            GetPaymentStatusRequestDto getPaymentStatusRequest = new GetPaymentStatusRequestDto();
            getPaymentStatusRequest.SecurityKey = getMobilPaymentInfoStatusRequest.SecurityKey;
            #endregion
            var getPaymentStatusResponse = await _paymentClientService.GetPaymentStatus(getPaymentStatusRequest);
            #endregion
            if (getPaymentStatusResponse.ResultType == ResultType.Ok)
            {
                getMobilPaymentInfoStatusResponse.PaymentStatus = getPaymentStatusResponse.Data.PaymentStatus;
                return new SuccessResult<GetMobilPaymentInfoStatusResponseDto>(getMobilPaymentInfoStatusResponse);
            }
            else
            {
                if (getPaymentStatusResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_NOT_FOUND)
                {
                    return new ErrorResult<GetMobilPaymentInfoStatusResponseDto>(getMobilPaymentInfoStatusResponse, MobilPaymentInfoErrorEnum.PAYMENT_NOT_FOUND);
                }
                else
                {
                    return new ErrorResult<GetMobilPaymentInfoStatusResponseDto>(getMobilPaymentInfoStatusResponse, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED);
                }
            }
        }

        private async Task<ProcessingUserAdress> CreateProcessingUserAdress(UserAdress userAdress)
        {
#nullable enable
            ProcessingUserAdress? insertedProcessingAddress = null;
#nullable disable
            #region processing user adres entity oluşturuluyor
            ProcessingUserAdress processingUserAdress = new ProcessingUserAdress();
            processingUserAdress.GuiId = Guid.NewGuid().ToString();
            processingUserAdress.TownId = userAdress.TownId;
            processingUserAdress.Name = userAdress.Name;
            processingUserAdress.PostCode = userAdress.PostCode;
            processingUserAdress.Neighbourhood = userAdress.Neighbourhood;
            processingUserAdress.Description = userAdress.Description;
            processingUserAdress.Email = userAdress.Email;
            processingUserAdress.TcNumber = userAdress.TcNumber;
            processingUserAdress.UserName = userAdress.UserName;
            processingUserAdress.UserSurname = userAdress.UserSurname;
            processingUserAdress.PlateNumber = userAdress.PlateNumber;
            processingUserAdress.TaxAdministration = userAdress.TaxAdministration;
            processingUserAdress.TaxNumber = userAdress.TaxNumber;
            processingUserAdress.CompanyName = userAdress.CompanyName;
            processingUserAdress.UserAdressType = userAdress.UserAdressType;
            processingUserAdress.CompanyType = userAdress.CompanyType;
            processingUserAdress.UserId = userAdress.UserId;
            processingUserAdress.UserAdressId = userAdress.Id;
            #endregion
            #region veritabanına kayıt ediliyor
            insertedProcessingAddress = await _processingUserAdressRepository.InsertAsync(processingUserAdress);
            await _processingUserAdressRepository.SaveChangesAsync();
            #endregion
            return insertedProcessingAddress;
        }
        private Result<T> GetPaymentChargeDebitCardErrorHandle<T>(T response, int errorCode)
        {
            if (errorCode == (int)PaymentErrorEnum.AN_ERROR_OCCURRED)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED);
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INFORMATION_NOT_FOUND_ERROR)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.PAYMENT_INFORMATION_NOT_FOUND_ERROR);
            }
            else if (errorCode == (int)PaymentErrorEnum.CARD_INFORMATION_INCORRECT)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.CARD_INFORMATION_INCORRECT);
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND);
            }
            else if (errorCode == (int)PaymentErrorEnum.FAILED_TO_STARTING_3D_VERIFICATION)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.FAILED_TO_STARTING_3D_VERIFICATION);
            }
            else if (errorCode == (int)PaymentErrorEnum.VERIFICATION_3D_FAILED)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.VERIFICATION_3D_FAILED);
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_NOT_FOUND ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_REASON_NOT_FOUND ||
                errorCode == (int)PaymentErrorEnum.INVALID_AMOUNT ||
                errorCode == (int)PaymentErrorEnum.FAILED_TO_DO_DIRECT_PAYMENT ||
                errorCode == (int)PaymentErrorEnum.SERVICE_NOT_EXIST_FOR_SELECTED_PAYMENT_INTEGRATION ||
                errorCode == (int)PaymentErrorEnum.FIRM_PAYMENT_SETTING_NOT_FOUND ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_DEALER_ERROR ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_LIMITS_DAILY_CARD_LIMIT_EXCEEDED ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_DEALER_PROCESS_ERROR ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_MOKA_INTEGRATION_GENERAL_ERROR ||
                errorCode == (int)PaymentErrorEnum.PAYMENT_CALLBACK_DATA_NOT_FOUND)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.PAYMENT_FAILED);
            }
            else if (errorCode == (int)PaymentErrorEnum.INVALID_CARD_INFO)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.CARD_INFORMATION_INCORRECT);
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION);
            }
            else if (errorCode == (int)PaymentErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE);
            }
            else if (errorCode == (int)PaymentErrorEnum.CARD_STORAGE_NOT_AVAILABLE_FOR_DEALER)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.CARD_STORAGE_NOT_AVAILABLE_FOR_DEALER);
            }
            else if (errorCode == (int)PaymentErrorEnum.VIRTUAL_POS_NOT_AVAILABLE_FOR_DEBIT_CARD)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.VIRTUAL_POS_NOT_AVAILABLE_FOR_DEBIT_CARD);
            }
            else if (errorCode == (int)PaymentErrorEnum.FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER);
            }
            else if (errorCode == (int)PaymentErrorEnum.INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY)
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY);
            }
            else
            {
                return new ErrorResult<T>(response, MobilPaymentInfoErrorEnum.AN_ERROR_OCCURRED);
            }
        }
        private async Task<Result<GetDebitCardIdentityForPaymentResponseDto>> GetDebitCardIdentityForPayment(DebitCard debitCard)
        {
            #region request dto oluşturuluyor
            GetDebitCardIdentityForPaymentRequestDto getDebitCardIdentityForPaymentRequest = new GetDebitCardIdentityForPaymentRequestDto();
            getDebitCardIdentityForPaymentRequest.GuiId = debitCard.DebitCardIdentityGuiId;
            #endregion
            #region bank apiye istek atılıyor
            var getDebitCardIdentityForPaymentResponse = await _debitCardIdentityClientService.GetDebitCardIdentityForPayment(getDebitCardIdentityForPaymentRequest);
            #endregion
            return getDebitCardIdentityForPaymentResponse;
        }
    }

}
