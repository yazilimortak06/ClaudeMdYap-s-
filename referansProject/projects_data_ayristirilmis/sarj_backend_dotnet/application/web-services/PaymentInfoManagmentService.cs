using Api.Persistence.Repositories.WalletRepositories;
using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.DebitCardDtos;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.PanelChargeDtos;
using Shared.Domain.Dto.ApiDto.PanelPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.ParameterDtos;
using Shared.Domain.Dto.ApiDto.PaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.UserAddressDtos;
using Shared.Domain.Dto.ApiDto.UserDtos;
using Shared.Domain.Dto.ApiDto.WalletDtos;
using Shared.Domain.Dto.BankDto.PaymentDtos;
using Shared.Domain.Dto.FileDto.ExportExcelDtos;
using Shared.Domain.Dto.FileDto.ExportPdfDtos;
using Shared.Domain.Dto.NotificationDto.PaymentNotificationDtos;
using Shared.Domain.Dto.TockenDto.DebitCardIdentityDtos;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.DebitCardModule;
using Shared.Domain.Entities.ApiEntities.PaymentInfoModule;
using Shared.Domain.Entities.ApiEntities.UserAdressModule;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.Entities.ApiEntities.WalletInfoModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.WorkerServiceEnums;
using Shared.Domain.Errors.BankErrors;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.Errors.TockenErrors;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.BankApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.FileApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.NotificationApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.TockenApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.DebitCardRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ParameterRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.PaymentInfoRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserAddressRepositories;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserAddressRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.UserRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.WalletRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PaymentInfoManagmentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Application.Services.PanelServices.PaymentInfoManagment
{
    public class PaymentInfoManagmentService : BaseService, IPaymentInfoManagmentService
    {
        private readonly IPaymentInfoRepository _paymentInfoRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWalletProcessRepository _walletProcessRepository;
        private readonly IWalletInfoRepository _walletInfoRepository;
        private readonly IWalletFormRepository _walletFormRepository;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly IDebitCardRepository _debitCardRepository;
        private readonly IProcessingUserAdressRepository _processingUserAdressRepository;
        private readonly IAdminContextProvider _adminContextProvider;
        private readonly IPaymentClientService _paymentClientService;
        private readonly IFileClientService _fileClientService;
        private readonly IAutomaticPaymentNotificationClientService _automaticPaymentNotificationClientService;
        private readonly IDebitCardIdentityClientService _debitCardIdentityClientService;
        private readonly ILogger<PaymentInfoManagmentService> _logger;

        public PaymentInfoManagmentService(IMapper mapper,
                            IConfiguration configuration,
                            IUtilService utilService,
                            IUserRepository userRepository,
                            IUserAddressRepository userAddressRepository,
                           IAdminContextProvider adminContextProvider,
                           IPaymentClientService paymentClientService,
                           IPaymentInfoRepository paymentInfoRepository,
                           IChargeRepository chargeRepository,
                           IParameterRepository parameterRepository,
                           IDebitCardRepository debitCardRepository,
                           IProcessingUserAdressRepository processingUserAdressRepository,
                           IDebitCardIdentityClientService debitCardIdentityClientService,
                           IAutomaticPaymentNotificationClientService automaticPaymentNotificationClientService,
                           IFileClientService fileClientService,
                           ILogger<PaymentInfoManagmentService> logger,
                           IWalletProcessRepository walletProcessRepository,
                           IWalletInfoRepository walletInfoRepository,
                           IWalletFormRepository walletFormRepository) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _utilService = utilService;
            _userRepository = userRepository;
            _userAddressRepository = userAddressRepository;
            _adminContextProvider = adminContextProvider;
            _paymentClientService = paymentClientService;
            _paymentInfoRepository = paymentInfoRepository;
            _chargeRepository = chargeRepository;
            _parameterRepository = parameterRepository;
            _debitCardRepository = debitCardRepository;
            _processingUserAdressRepository = processingUserAdressRepository;
            _debitCardIdentityClientService = debitCardIdentityClientService;
            _automaticPaymentNotificationClientService = automaticPaymentNotificationClientService;
            _fileClientService = fileClientService;
            _logger = logger;
            _walletProcessRepository = walletProcessRepository;
            _walletInfoRepository = walletInfoRepository;
            _walletFormRepository = walletFormRepository;
        }
        #region ekleme,güncelleme,silme

        #endregion
        /// <summary>
        /// panel için kullanıcı ödeme verileri çekiliyor
        /// </summary>
        /// <param name="getPanelPaymentInfoDashboardRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelPaymentInfoDashboardResponseDto>> GetPanelPaymentInfoDashboard(GetPanelPaymentInfoDashboardRequestDto getPanelPaymentInfoDashboardRequest)
        {
            #region response dto oluşturuluyor
            GetPanelPaymentInfoDashboardResponseDto getPanelPaymentInfoDashboardResponse = new GetPanelPaymentInfoDashboardResponseDto();
            getPanelPaymentInfoDashboardResponse.Payments = new List<PanelPaymentInfoDashboardItemDto>();
            #endregion
            #region toplam ödeme ve iadeler için ödeme verileri getiriliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilterForSumPrice = _mapper.Map<PaymentInfoFilterDto>(getPanelPaymentInfoDashboardRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentInfoFilterDto;
                    destData.PaymentStatus = PaymentStatusEnum.SUCCESSFUL;
                    if (_adminContextProvider.CompanyId != null)
                    {
                        destData.CompanyId = _adminContextProvider.CompanyId;
                    }
                    if (_adminContextProvider.FirmId != null)
                    {
                        destData.FirmId = _adminContextProvider.FirmId;
                    }
                });
            });
            #endregion
            getPanelPaymentInfoDashboardResponse.TotalPaidPrice = await _paymentInfoRepository.GetOnlyPaymentInfoAsNoTracking(paymentInfoFilterForSumPrice).Select(x => x.PaidPrice).SumAsync();
            getPanelPaymentInfoDashboardResponse.TotalRefundedPrice = await _paymentInfoRepository.GetOnlyPaymentInfoAsNoTracking(paymentInfoFilterForSumPrice).Select(x => x.RefundedPrice.GetValueOrDefault()).SumAsync();
            #endregion
            #region ödeme işlemleri getiriliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = _mapper.Map<PaymentInfoFilterDto>(getPanelPaymentInfoDashboardRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentInfoFilterDto;
                    destData.Year = DateTime.Now.Year;
                    if (_adminContextProvider.CompanyId != null)
                    {
                        destData.CompanyId = _adminContextProvider.CompanyId;
                    }
                    if (_adminContextProvider.FirmId != null)
                    {
                        destData.FirmId = _adminContextProvider.FirmId;
                    }
                });
            });
            #endregion
            var paymentInfo = await _paymentInfoRepository.GetPaymentInfotWithUserAsNoTracking(paymentInfoFilter).ToListAsync();
            #endregion
            #region response dto setleniyor
            getPanelPaymentInfoDashboardResponse.Payments = _mapper.Map<List<PanelPaymentInfoDashboardItemDto>>(paymentInfo, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as List<PanelPaymentInfoDashboardItemDto>;
                    destData.ForEach(item =>
                    {
                        item.Month = item.CompletedDate.ToString("MMMM");
                        item.MonthNumber = item.CompletedDate.Month;
                        item.Price = Math.Round(item.Price, 2);
                        item.PaidPrice = Math.Round(item.PaidPrice, 2);
                    });
                });
            });
            #endregion
            return new SuccessResult<GetPanelPaymentInfoDashboardResponseDto>(getPanelPaymentInfoDashboardResponse);
        }
        /// <summary>
        /// panel için kullanıcı ödeme detayı çekiliyor
        /// </summary>
        /// <param name="getPaymentInfoDetailPanelRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPaymentInfoDetailPanelResponseDto>> GetPaymentInfoDetailPanel(GetPaymentInfoDetailPanelRequestDto getPaymentInfoDetailPanelRequest)
        {
            #region response dto oluşturuluyor
            GetPaymentInfoDetailPanelResponseDto getPaymentInfoDetailPanelResponse = new GetPaymentInfoDetailPanelResponseDto();
            //userPaymentDetailPanel.OtherPaymentList = new List<UserPaymentDto>();
            #endregion
            #region ödeme verisi çekiliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = _mapper.Map<PaymentInfoFilterDto>(getPaymentInfoDetailPanelRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentInfoFilterDto;
                    if (_adminContextProvider.CompanyId != null)
                    {
                        destData.CompanyId = _adminContextProvider.CompanyId;
                    }
                    if (_adminContextProvider.FirmId != null)
                    {
                        destData.FirmId = _adminContextProvider.FirmId;
                    }
                });
            });
            #endregion
            var paymentInfo = await _paymentInfoRepository.GetPaymentInfo(paymentInfoFilter, include: source => source
             .Include(x => x.User)
             .Include(x => x.Charge)).FirstOrDefaultAsync();
            #endregion
            if (paymentInfo != null)
            {
                getPaymentInfoDetailPanelResponse.PaymentInfoDetail = _mapper.Map<PaymentInfoDetailPanelDto>(paymentInfo);
                //#region ödeme işlemi şarj işlemi içinse, işleme bağlı diğer ödeme işlemleri de getiriliyor
                //if (userPayment.PaymentReason == UserPaymentReasonEnum.CHARGE)
                //{
                //    #region payment apiye istek atılarak işleme bağlı olan diğer ödeme verileri getiriliyor
                //    #region request dto oluşturuluyor
                //    GetPaymentDataListRequestDto getPaymentListRequest = new GetPaymentDataListRequestDto();
                //    getPaymentListRequest.ChargeProcessGuiId = userPayment.ChargeProcess.GuiId;
                //    #endregion
                //    var getPaymentListResponse = await _paymentClientService.GetPaymentList(getPaymentListRequest);
                //    #endregion
                //    if (getPaymentListResponse.ResultType == ResultType.Ok)
                //    {
                //        userPaymentDetailPanel.OtherPaymentList = _mapper.Map<List<UserPaymentDto>>(getPaymentListResponse.Data.PaymentList.Where(x => x.GuiId != userPayment.PaymentGuiId).ToList());
                //    }
                //}
                //#endregion
                return new SuccessResult<GetPaymentInfoDetailPanelResponseDto>(getPaymentInfoDetailPanelResponse);
            }
            #region ödeme bulunamadı
            else
            {
                //#region ödeme işlemi şarj işlemi içinse, işleme bağlı diğer ödeme işlemleri de getiriliyor
                //if (getUserPaymentDetailPanelRequest != null)
                //{
                //    #region şarj işlemi çekiliyor
                //    #region filter dto oluşturuluyor
                //    ChargeProcessFilterDto chargeProcessFilter = new ChargeProcessFilterDto();
                //    chargeProcessFilter.GuiId = getUserPaymentDetailPanelRequest.ChargeProcessGuiId;
                //    #endregion
                //    var chargeProcess = await _chargeProcessRepository.GetChargeProcessAsNoTracking(chargeProcessFilter).FirstOrDefaultAsync();
                //    #endregion
                //    if (chargeProcess != null)
                //    {
                //        #region payment apiye istek atılarak işleme bağlı olan diğer ödeme verileri getiriliyor
                //        #region request dto oluşturuluyor
                //        GetPaymentDataListRequestDto getPaymentListRequest = new GetPaymentDataListRequestDto();
                //        getPaymentListRequest.ChargeProcessGuiId = chargeProcess.GuiId;
                //        #endregion
                //        var getPaymentListResponse = await _paymentClientService.GetPaymentList(getPaymentListRequest);
                //        #endregion
                //        if (getPaymentListResponse.ResultType == ResultType.Ok)
                //        {
                //            userPaymentDetailPanel.OtherPaymentList = _mapper.Map<List<UserPaymentDto>>(getPaymentListResponse.Data.PaymentList);
                //        }
                //        return new SuccessResult<UserPaymentDetailPanelDto>(userPaymentDetailPanel);
                //    }
                //    else
                //    {
                //        return new ErrorResult<UserPaymentDetailPanelDto>(userPaymentDetailPanel, UserPaymentManagmentErrorEnum.CHARGE_PROCESS_NOT_FOUND_ERROR);
                //    }
                //}
                //#endregion
                //else
                //{
                //    return new ErrorResult<UserPaymentDetailPanelDto>(userPaymentDetailPanel, UserPaymentManagmentErrorEnum.USER_PAYMENT_NOT_FOUND_ERROR);
                //}
                return new ErrorResult<GetPaymentInfoDetailPanelResponseDto>(getPaymentInfoDetailPanelResponse, PanelPaymentInfoErrorEnum.PAYMENT_NOT_FOUND);
            }
            #endregion
        }
        /// <summary>
        /// panel datatable için kullanıcı ödeme verileri çekiliyor
        /// </summary>
        /// <param name="dataTableFilter"></param>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<PanelPaymentInfoListItemDto>>> GetPaymentInfoDataTablePanel(DataTableFilterModel<GetPanelPaymentInfoDatatableRequestDto> dataTableFilter)
        {
            #region ödeme sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region ödemeler getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilter.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilter.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = _mapper.Map<PaymentInfoFilterDto>(dataTableFilter.data, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as PaymentInfoFilterDto;
                    if (_adminContextProvider.CompanyId != null)
                    {
                        destData.CompanyId = _adminContextProvider.CompanyId;
                    }
                    if (_adminContextProvider.FirmId != null)
                    {
                        destData.FirmId = _adminContextProvider.FirmId;
                    }
                });
            });
            #endregion
            var paymentInfos = _paymentInfoRepository.GetPaymentInfo(paymentInfoFilter, include: source => source
             .Include(x => x.User)
             .Include(x => x.DebitCard)
             .Include(x => x.Charge)
             .Include(x => x.ProcessingUserAdress));
            var resultPaymentInfo = paymentInfos.ApplySorting(dataTableFilter.orderProperty, dataTableFilter.orderDirective);
            #endregion
            #region response dto setleniyor
            toplamKayit = resultPaymentInfo.Count();
            var paymentInfoList = await resultPaymentInfo.Skip(ofset).Take(recordPerPage).ToListAsync();
            var paymentInfoListDto = _mapper.Map<List<PanelPaymentInfoListItemDto>>(paymentInfoList);
            var result = new DataTableResponseWrapper<PanelPaymentInfoListItemDto>(toplamKayit, paymentInfoListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<PanelPaymentInfoListItemDto>>(result);
        }
        /// <summary>
        /// otomatik ödeme işlemi yapılıyor
        /// </summary>
        /// <param name="automaticPaymentResquest"></param>
        /// <returns></returns>
        public async Task<Result<AutomaticPaymentResponseDto>> AutomaticPayment(AutomaticPaymentRequestDto automaticPaymentResquest)
        {
            #region response dto oluşturuluyor
            AutomaticPaymentResponseDto automaticPaymentResponse = new AutomaticPaymentResponseDto();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region parametrelerden otomatik ödeme deneme zaman aralığı alınıyor
            var automaticPaymentTrialTimeRange = await _parameterRepository.GetParametersAsNoTracking(new ParameterFilterDto() { Id = (long)ParameterId.AUTOMATIC_PAYMENT_TRIAL_TIME_RANGE_MINUTE }).FirstOrDefaultAsync();
            #region parametrelerden otomatik ödeme için ilk deneme zaman aralığı alınıyor
            var automaticPaymentFirstTrialTimeRange = await _parameterRepository.GetParametersAsNoTracking(new ParameterFilterDto() { Id = (long)ParameterId.AUTOMATIC_PAYMENT_FIRST_TRIAL_TIME_RANGE_MINUTE }).FirstOrDefaultAsync();
            #endregion
            #endregion
            if (automaticPaymentTrialTimeRange != null && automaticPaymentTrialTimeRange.ParameterValue != null &&
               automaticPaymentFirstTrialTimeRange != null && automaticPaymentFirstTrialTimeRange.ParameterValue != null)
            {
                var automaticPaymentTrialTimeRangeParameterValue = _utilService.ParseInt(automaticPaymentTrialTimeRange.ParameterValue.Value);
                var automaticPaymentFirstTrialTimeRangeParameterValue = _utilService.ParseInt(automaticPaymentFirstTrialTimeRange.ParameterValue.Value);
                #region şarj işlemi çekiliyor
                var charge = await _chargeRepository.GetCharge(new ChargeFilterDto()
                {
                    AutomaticPaymentTrialTimeRange = automaticPaymentTrialTimeRangeParameterValue,
                    AutomaticPaymentFirstTrialTimeRange = automaticPaymentFirstTrialTimeRangeParameterValue,
                    ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.PAYMENT_BEING_RECEIVED, ChargeStateEnum.PAYMENT_FAIL }
                }, include: source => source
           .Include(x => x.Firm)
           .Include(x => x.PaymentInfo)
            .Include(x => x.ChargeDeviceConnector).ThenInclude(x => x.ChargeDevicePowerType)
           .Include(x => x.ChargeDeviceConnector).ThenInclude(x => x.ChargeDevice).ThenInclude(x => x.Station).ThenInclude(x => x.StationPaymentMethod), false).OrderBy(x => x.AutomaticPaymentTrialDate).FirstOrDefaultAsync();
                #endregion

                if (charge != null)
                {
                    _logger.LogError(charge.Id  + "");
                    #region charge güncelleniyor
                    _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                        s => s.State,
                        s => s.AutomaticPaymentTrialDate,
                    });
                    charge.AutomaticPaymentTrialDate = datetimeNow;
                    #endregion
                    #region işlem ücreti bulunmamaktadır
                    if (charge.CalculatedPrice == 0 || charge.CalculatedPrice == null)
                    {
                        #region şarj işlemi güncelleniyor
                        charge.State = ChargeStateEnum.COMPLETED;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _chargeRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.SUCCESSFUL;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.NONE;
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        #endregion
                        #region signalr ile mobil apiye bildiriliyor
                        await _automaticPaymentNotificationClientService.SendOnlyUser(new AutomaticPaymentNotificationDto()
                        {
                            ChargeGuiId = charge.GuiId,
                            ChargeState = charge.State,
                            ConnectionId = charge.ConnectionId,
                            IsSuccessful = true,
                        });
                        #endregion
                        return new SuccessResult<AutomaticPaymentResponseDto>(automaticPaymentResponse);
                    }
                    #endregion
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
                            #region response dto setleniyor
                            automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.SUCCESSFUL;
                            automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.NONE;
                            automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                            #endregion
                            return new SuccessResult<AutomaticPaymentResponseDto>(automaticPaymentResponse);
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
                    #region kullanıcı için adres ve kullanıcı bilgisi alınıyor
                    #region kullanıcı çekiliyor
                    #region filter dto oluşturuluyor
                    UserMobilFilterDto userFilter = new UserMobilFilterDto();
                    userFilter.Id = charge.UserId;
                    #endregion
                    var user = await _userRepository.GetUserAsNoTracking(userFilter).FirstOrDefaultAsync();
                    #endregion
                    #region kullanıcı adresi çekiliyor
                    #region filter dto oluşturuluyor
                    UserAddressFilterDto userAddressFilter = new UserAddressFilterDto();
                    userAddressFilter.IsDefault = true;
                    userAddressFilter.UserId = charge.UserId;
                    #endregion
                    var userAddress = await _userAddressRepository.GetUserAddressAsNoTracking(userAddressFilter).FirstOrDefaultAsync();
                    #endregion
                    #region kullanıcı bulunamadı
                    if (user == null)
                    {
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.NONE;
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.GeneralErrorDescription = PanelPaymentInfoErrorEnum.USER_CAN_NOT_FOUND.ToDescriptionString();
                        automaticPaymentResponse.GeneralErrorCode = (int)PanelPaymentInfoErrorEnum.USER_CAN_NOT_FOUND;
                        #endregion
                        return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.USER_CAN_NOT_FOUND);
                    }
                    #endregion
                    #region kullanıcı adresi bulunamadı
                    if (userAddress == null)
                    {
                        _logger.LogError("adres bulunamadı");
                        _logger.LogError(charge.UserId.ToString());
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        #region response dto setleniyor++
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.NONE;
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.GeneralErrorDescription = PanelPaymentInfoErrorEnum.BILLING_ADDRESS_NOT_FOUND.ToDescriptionString();
                        automaticPaymentResponse.GeneralErrorCode = (int)PanelPaymentInfoErrorEnum.BILLING_ADDRESS_NOT_FOUND;
                        #endregion
                        return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.BILLING_ADDRESS_NOT_FOUND);
                    }
                    #endregion
                    #region processing user address ekleniyor
                    ProcessingUserAdress insertedProcessingAddress = await CreateProcessingUserAdress(userAddress);
                    #endregion
                    #region istasyon ödeme yöntemleri alınıyor
                    var stationPaymentMethods = charge.ChargeDeviceConnector.ChargeDevice.Station.StationPaymentMethod.ToList();
                    #endregion
                    #endregion
                    #region cüzdan ile ödeme yapılıyor
                    #region istasyon için ödeme metodu geçerli mi kontrol ediliyor
                    #region istasyon için ödeme metodu geçersiz
                    if (stationPaymentMethods.Count > 0 && !stationPaymentMethods.Any(x => x.PaymentMethodId == (long)PaymentMethodEnum.WALLET))
                    {
                        #region response dto setleniyor
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.WALLET;
                        automaticPaymentResponse.WalletProcessErrorDescription = PanelPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED.ToDescriptionString();
                        automaticPaymentResponse.WalletProcessErrorCode = (int)PanelPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED;
                        #endregion
                        goto paymentWithDebitCard;
                    }
                    #endregion
                    #endregion
                    #region cüzdan getiriliyor
                    var walletInfo = await _walletInfoRepository.GetWalletInfo(new WalletInfoFilterDto() { UserId = charge.UserId }, include: source => source
                    .Include(a => a.User)
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
                            #region veritabanına kayıt ediliyor
                            await _chargeRepository.SaveChangesAsync();
                            #endregion
                            #region response dto setleniyor
                            automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                            automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.WALLET;
                            automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                            automaticPaymentResponse.WalletProcessErrorDescription = PanelPaymentInfoErrorEnum.WALLET_AMOUNT_INSUFFICIENT.ToDescriptionString();
                            automaticPaymentResponse.WalletProcessErrorCode = (int)PanelPaymentInfoErrorEnum.WALLET_AMOUNT_INSUFFICIENT;
                            #endregion
                            //return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, UserPaymentProcessErrorEnum.WALLET_AMOUNT_INSUFFICIENT);
                            goto paymentWithDebitCard;
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
                        PaymentWalletRequestDto chargePaymentWalletRequest = CreateChargePaymentWalletRequestDto(charge, walletProcess,
                        userAddress, walletInfo, insertedProcessingAddress.GuiId);
                        #endregion
                        //#region request dto oluşturuluyor
                        //PaymentWalletRequestDto chargePaymentWalletRequest = _mapper.Map<PaymentWalletRequestDto>(getMobilPaymentChargeRequest, opt =>
                        //{
                        //    opt.AfterMap((src, dest) =>
                        //    {
                        //        var destData = dest as PaymentWalletRequestDto;
                        //        destData.FirmGuiId = charge.Firm.GuiId;
                        //        destData.UserGuiId = user.MobilUserGuiId;
                        //        destData.UserAdressGuiId = userAddress.GuiId;
                        //        destData.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                        //        destData.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                        //        destData.Discount = charge.Discount;
                        //        destData.Price = charge.CalculatedPrice.GetValueOrDefault();
                        //        destData.WalletGuiId = walletInfo.WalletGuiId;
                        //        destData.Kdv = charge.Kdv;
                        //        destData.WalletAmountTockenGuiId = walletProcess.AmountTockenGuiId;
                        //        destData.WalletTockenGuiId = walletProcess.WalletTockenGuiId;
                        //        destData.WalletProcessGuiId = walletProcess.GuiId;
                        //        destData.PaymentReason = PaymentReasonEnum.CHARGE;
                        //    });
                        //});
                        //#endregion
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
                            paymentInfo.UserId = charge.UserId;
                            paymentInfo.UserAdressId = userAddress.Id;
                            paymentInfo.ChargeId = charge.Id;
                            paymentInfo.ProcessingUserAdressId = insertedProcessingAddress.Id;
                            paymentInfo.WalletSpendMoneyInfo = walletSpendMoneyInfo;
                            await _paymentInfoRepository.InsertAsync(paymentInfo);
                            #endregion
                            #region charge güncelleniyor
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
                            automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.SUCCESSFUL;
                            automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.WALLET;
                            automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                            #endregion
                            #region signalr ile mobil apiye bildiriliyor
                            await _automaticPaymentNotificationClientService.SendOnlyUser(new AutomaticPaymentNotificationDto()
                            {
                                ChargeGuiId = charge.GuiId,
                                ChargeState = charge.State,
                                ConnectionId = charge.ConnectionId,
                                IsSuccessful = true,
                            });
                            #endregion
                            return new SuccessResult<AutomaticPaymentResponseDto>(automaticPaymentResponse);
                        }
                        #endregion
                        #region ödeme işlemi başarısız
                        else
                        {
                            var error = HandlePaymentErrorFromBankApi(paymentWalletResponse);
                            #region response dto setleniyor
                            if (error == PanelPaymentInfoErrorEnum.PAYMENT_WAS_COMPLETED)
                            {
                                #region şarj işlemi güncelleniyor
                                charge.State = ChargeStateEnum.COMPLETED;
                                #endregion
                                #region veritabanına kayıt ediliyor
                                await _chargeRepository.SaveChangesAsync();
                                #endregion
                                #region signalr ile mobil apiye bildiriliyor
                                await _automaticPaymentNotificationClientService.SendOnlyUser(new AutomaticPaymentNotificationDto()
                                {
                                    ChargeGuiId = charge.GuiId,
                                    ChargeState = charge.State,
                                    ConnectionId = charge.ConnectionId,
                                    IsSuccessful = true,
                                });
                                #endregion
                                automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.SUCCESSFUL;
                                automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.NONE;
                                return new SuccessResult<AutomaticPaymentResponseDto>(automaticPaymentResponse);
                            }
                            else
                            {
                                automaticPaymentResponse.WalletProcessErrorDescription = error.ToDescriptionString();
                                automaticPaymentResponse.WalletProcessErrorCode = (int)error;
                                automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                                automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.WALLET;
                                //return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse,error);
                                goto paymentWithDebitCard;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region response dto setleniyor
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.WalletProcessErrorDescription = PanelPaymentInfoErrorEnum.WALLET_INFO_NOT_FOUND.ToDescriptionString();
                        automaticPaymentResponse.WalletProcessErrorCode = (int)PanelPaymentInfoErrorEnum.WALLET_INFO_NOT_FOUND;
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.WALLET;
                        #endregion
                        goto paymentWithDebitCard;
                    }
                #endregion
                    #region kredi kartı ile ödeme yapılıyor
                paymentWithDebitCard:
                    #region istasyon için ödeme metodu geçerli mi kontrol ediliyor
                    #region istasyon için ödeme metodu geçersiz
                    if (stationPaymentMethods.Count > 0 && !stationPaymentMethods.Any(x => x.PaymentMethodId == (long)PaymentMethodEnum.DEBIT_CARD))
                    {
                        #region response dto setleniyor
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.DEBIT_CARD;
                        automaticPaymentResponse.DebitCardProcessErrorDescription = PanelPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED.ToDescriptionString();
                        automaticPaymentResponse.DebitCardProcessErrorCode = (int)PanelPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED;
                        #endregion
                        return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.PAYMENT_METHOD_NOT_SUPPORTED);
                    }
                    #endregion
                    #endregion
                    #region tocken a istek atılarak kayıtlı kart bilgileri getiriliyor
                    GetDebitCardIdentityForPaymentResponseDto getDebitCardIdentityForPaymentResponseData = null;
                    DebitCard debitCard = null;
                    #region filter dto oluşturuluyor
                    DebitCardFilterDto debitCardFilter = new DebitCardFilterDto();
                    debitCardFilter.IsDefault = true;
                    debitCardFilter.UserId = charge.UserId;
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
                            #region response dto setleniyor ve return işlemi gerçekleşiyor
                            automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                            automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                            automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.DEBIT_CARD;
                            if (getDebitCardIdentityForPaymentResponse.ErrorCode == (int)DebitCardIdentityErrorEnum.DEBIT_CARD_IS_NOT_FOUND)
                            {
                                automaticPaymentResponse.DebitCardProcessErrorDescription = PanelPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND.ToDescriptionString();
                                automaticPaymentResponse.DebitCardProcessErrorCode = (int)PanelPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND;
                                return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND);
                            }
                            else
                            {
                                automaticPaymentResponse.DebitCardProcessErrorDescription = PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED.ToDescriptionString();
                                automaticPaymentResponse.DebitCardProcessErrorCode = (int)PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED;
                                return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.DEBIT_CARD_IS_NOT_FOUND);
                    }
                    #endregion
                    #region request dto oluşturuluyor
                    PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest = new PaymentDirectDebitCardRequestDto();
                    paymentDirectDebitCardRequest.FirmGuiId = charge.Firm.GuiId;
                    paymentDirectDebitCardRequest.PaymentReason = PaymentReasonEnum.CHARGE;
                    paymentDirectDebitCardRequest.Price = charge.CalculatedPrice.GetValueOrDefault();
                    paymentDirectDebitCardRequest.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
                    paymentDirectDebitCardRequest.UserAdressGuiId = userAddress.GuiId;
                    paymentDirectDebitCardRequest.UserGuiId = user.MobilUserGuiId;
                    paymentDirectDebitCardRequest.ChargeGuiId = charge.GuiId;
                    paymentDirectDebitCardRequest.ProcessingUserAdressGuiId = insertedProcessingAddress.GuiId;
                    paymentDirectDebitCardRequest.Kdv = charge.Kdv;
                    paymentDirectDebitCardRequest.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                    paymentDirectDebitCardRequest.PaymentChargeInfoJson = JsonSerializer.Serialize(new PaymentChargeInfoDto()
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
                    paymentDirectDebitCardRequest.UserAdressJson = JsonSerializer.Serialize(userAdressDto);
                    if (debitCard != null && getDebitCardIdentityForPaymentResponseData != null)
                    {
                        paymentDirectDebitCardRequest.CardNumber = getDebitCardIdentityForPaymentResponseData.CardNumber;
                        paymentDirectDebitCardRequest.ExpireYear = getDebitCardIdentityForPaymentResponseData.ExpireYear;
                        paymentDirectDebitCardRequest.ExpireMonth = getDebitCardIdentityForPaymentResponseData.ExpireMonth;
                        paymentDirectDebitCardRequest.Cvc = getDebitCardIdentityForPaymentResponseData.Cvc;
                        paymentDirectDebitCardRequest.CardHolderName = getDebitCardIdentityForPaymentResponseData.CardHolderName;
                    }
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
                        paymentInfo.UserId = charge.UserId;
                        paymentInfo.UserAdressId = userAddress.Id;
                        paymentInfo.ChargeId = charge.Id;
                        paymentInfo.ProcessingUserAdressId = insertedProcessingAddress.Id;
                        paymentInfo.DebitCardId = debitCard.Id;
                        await _paymentInfoRepository.InsertAsync(paymentInfo);
                        #endregion
                        #region charge güncelleniyor
                        charge.State = ChargeStateEnum.COMPLETED;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        #region signalr ile mobil apiye bildiriliyor
                        await _automaticPaymentNotificationClientService.SendOnlyUser(new AutomaticPaymentNotificationDto()
                        {
                            ChargeGuiId = charge.GuiId,
                            ChargeState = charge.State,
                            ConnectionId = charge.ConnectionId,
                            IsSuccessful = true,
                        });
                        #endregion
                        #region response dto setleniyor
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.SUCCESSFUL;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.DEBIT_CARD;
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.DebitCardGuiId = debitCard.DebitCardIdentityGuiId;
                        automaticPaymentResponse.PaymentGuiId = paymentInfo.PaymentGuiId;
                        #endregion
                        return new SuccessResult<AutomaticPaymentResponseDto>(automaticPaymentResponse);
                    }
                    #endregion
                    #region şarj işlemi için ödeme tamamlama başarısız
                    else
                    {
                        var error = GetPaymentChargeDebitCardErrorHandle(paymentDirectDebitCardResponse.ErrorCode);
                        #region charge güncelleniyor
                        charge.State = ChargeStateEnum.PAYMENT_FAIL;
                        #endregion
                        #region veritabanına kayıt ediliyor
                        await _paymentInfoRepository.SaveChangesAsync();
                        #endregion
                        #region signalr ile mobil apiye bildiriliyor
                        await _automaticPaymentNotificationClientService.SendOnlyUser(new AutomaticPaymentNotificationDto()
                        {
                            ChargeGuiId = charge.GuiId,
                            ChargeState = charge.State,
                            ConnectionId = charge.ConnectionId,
                            IsSuccessful = false,
                        });
                        #endregion
                        #region response dto setleniyor
                        automaticPaymentResponse.ChargeGuiId = charge.GuiId;
                        automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                        automaticPaymentResponse.AutomaticPaymentType = AutomaticPaymentTypeEnum.DEBIT_CARD;
                        automaticPaymentResponse.DebitCardProcessErrorDescription = error.ToDescriptionString();
                        automaticPaymentResponse.DebitCardProcessErrorCode = (int)error;
                        #endregion
                        return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, error);
                    }
                    #endregion
                    #endregion
                }
                #region işlem bulunamadı veya ödemesi gerçekleşmiş
                else
                {
                    #region response dto setleniyor
                    automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                    automaticPaymentResponse.GeneralErrorDescription = PanelPaymentInfoErrorEnum.CHARGE_PROCESS_NOT_FOUND_FOR_AUTOMATIC_PAYMENT.ToDescriptionString();
                    automaticPaymentResponse.GeneralErrorCode = (int)PanelPaymentInfoErrorEnum.CHARGE_PROCESS_NOT_FOUND_FOR_AUTOMATIC_PAYMENT;
                    #endregion
                    return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.CHARGE_PROCESS_NOT_FOUND_FOR_AUTOMATIC_PAYMENT);
                }
                #endregion
            }
            #region parametre değeri bulunamadı
            else
            {
                #region response dto setleniyor
                automaticPaymentResponse.AutomaticPaymentResultState = AutomaticPaymentResultStateEnum.FAILED;
                automaticPaymentResponse.GeneralErrorDescription = PanelPaymentInfoErrorEnum.PARAMETER_VALUE_CAN_NOT_FOUND.ToDescriptionString();
                automaticPaymentResponse.GeneralErrorCode = (int)PanelPaymentInfoErrorEnum.PARAMETER_VALUE_CAN_NOT_FOUND;
                #endregion
                return new ErrorResult<AutomaticPaymentResponseDto>(automaticPaymentResponse, PanelPaymentInfoErrorEnum.PARAMETER_VALUE_CAN_NOT_FOUND);
            }
            #endregion
        }
        private PaymentWalletRequestDto CreateChargePaymentWalletRequestDto(Charge charge, WalletProcess walletProcess,
            UserAdress userAddress, WalletInfo walletInfo, string processingUserAddresGuiId)
        {
            PaymentWalletRequestDto chargePaymentWalletRequest = new PaymentWalletRequestDto();
            chargePaymentWalletRequest.ChargeGuiId = charge.GuiId;
            chargePaymentWalletRequest.FirmGuiId = charge.GuiId;
            chargePaymentWalletRequest.ProcessingUserAdressGuiId = processingUserAddresGuiId;
            chargePaymentWalletRequest.PaidPrice = charge.CalculatedPrice.GetValueOrDefault() - charge.Discount;
            chargePaymentWalletRequest.Discount = charge.Discount;
            chargePaymentWalletRequest.Price = charge.CalculatedPrice.GetValueOrDefault();
            chargePaymentWalletRequest.WalletGuiId = walletInfo.WalletGuiId;
            chargePaymentWalletRequest.UserGuiId = charge.User.MobilUserGuiId;
            chargePaymentWalletRequest.UserAdressGuiId = userAddress.GuiId;
            chargePaymentWalletRequest.Kdv = charge.Kdv;
            chargePaymentWalletRequest.WalletAmountTockenGuiId = walletProcess.AmountTockenGuiId;
            chargePaymentWalletRequest.WalletTockenGuiId = walletProcess.WalletTockenGuiId;
            chargePaymentWalletRequest.WalletProcessGuiId = walletProcess.GuiId;
            chargePaymentWalletRequest.PaymentReason = PaymentReasonEnum.CHARGE;
            chargePaymentWalletRequest.PaymentChargeInfoJson = JsonSerializer.Serialize(new PaymentChargeInfoDto()
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
            chargePaymentWalletRequest.UserAdressJson = JsonSerializer.Serialize(userAdressDto);
            return chargePaymentWalletRequest;
        }
        private PanelPaymentInfoErrorEnum HandlePaymentErrorFromBankApi(Result<PaymentWalletResponseDto> paymentWalletResponse)
        {
            if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.WALLET_NOT_FOUND)
            {
                return PanelPaymentInfoErrorEnum.WALLET_INFO_NOT_FOUND;
            }
            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND)
            {
                return PanelPaymentInfoErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND;
            }
            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.INVALID_PAYMENT_REASON)
            {
                return  PanelPaymentInfoErrorEnum.INVALID_PAYMENT_REASON;
            }
            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.PAYMENT_WAS_COMPLETED)
            {
                return  PanelPaymentInfoErrorEnum.PAYMENT_WAS_COMPLETED;
            }
            else if (paymentWalletResponse.ErrorCode == (int)PaymentErrorEnum.WALLET_AMOUNT_INSUFFICIENT)
            {
                return PanelPaymentInfoErrorEnum.WALLET_AMOUNT_INSUFFICIENT;
            }
            else
            {
                return PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED;
            }
        }

        /// <summary>
        /// ödeme işlemleri exceli oluşturuluyor
        /// </summary>
        /// <param name="panelExportExcelPaymentInfoRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelExportExcelPaymentInfoResponseDto>> ExportExcelPaymentInfo(PanelExportExcelPaymentInfoRequestDto panelExportExcelPaymentInfoRequest)
        {
            #region response dto oluşturuluyor
            PanelExportExcelPaymentInfoResponseDto panelExportExcelPaymentInfoResponse = new PanelExportExcelPaymentInfoResponseDto();
            #endregion
            #region ödeme işlemleri getiriliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = _mapper.Map<PaymentInfoFilterDto>(panelExportExcelPaymentInfoRequest);
            #endregion
            var paymentInfos = _paymentInfoRepository.GetPaymentInfoAsNoTracking(paymentInfoFilter);
            var paymentInfoListDto = _mapper.Map<List<PanelPaymentInfoListItemDto>>(paymentInfos);
            #endregion
            #region file service istek atılarak excel oluşturuluyor
            #region request dto oluşturuluyor
            CreateExcelPaymentInfoRequestDto createExcelPaymentInfoRequest = new CreateExcelPaymentInfoRequestDto();
            createExcelPaymentInfoRequest.PaymentInfoList = paymentInfoListDto;
            #endregion
            var exportExcelResponse = await _fileClientService.CreateExcelPaymentInfo(createExcelPaymentInfoRequest);
            #endregion
            if (exportExcelResponse.ResultType == ResultType.Ok)
            {
                #region response  dto setleniyor
                panelExportExcelPaymentInfoResponse.FileKey = exportExcelResponse.Data.FileKey;
                panelExportExcelPaymentInfoResponse.FileUrl = exportExcelResponse.Data.FileUrl;
                #endregion
                return new SuccessResult<PanelExportExcelPaymentInfoResponseDto>(panelExportExcelPaymentInfoResponse);
            }
            #region excel oluşturma işlemi başarısız
            else
            {
                return new ErrorResult<PanelExportExcelPaymentInfoResponseDto>(panelExportExcelPaymentInfoResponse, PanelPaymentInfoErrorEnum.EXPORT_EXCEL_FAILED);
            }
            #endregion
        }
        /// <summary>
        /// ödeme işlemleri pdf oluşturuluyor
        /// </summary>
        /// <param name="panelExportPdfPaymentInfoRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelExportPdfPaymentInfoResponseDto>> ExportPdfPaymentInfo(PanelExportPdfPaymentInfoRequestDto panelExportPdfPaymentInfoRequest)
        {
            #region response dto oluşturuluyor
            PanelExportPdfPaymentInfoResponseDto panelExportPdfPaymentInfoResponse = new PanelExportPdfPaymentInfoResponseDto();
            #endregion
            #region ödeme işlemleri getiriliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = _mapper.Map<PaymentInfoFilterDto>(panelExportPdfPaymentInfoRequest);
            #endregion
            var paymentInfos = _paymentInfoRepository.GetPaymentInfoAsNoTracking(paymentInfoFilter);
            var paymentInfoListDto = _mapper.Map<List<PanelPaymentInfoListItemDto>>(paymentInfos);
            #endregion
            #region file service istek atılarak excel oluşturuluyor
            #region request dto oluşturuluyor
            CreatePdfPaymentInfoRequestDto createPdfPaymentInfoRequest = new CreatePdfPaymentInfoRequestDto();
            createPdfPaymentInfoRequest.PaymentInfoList = paymentInfoListDto;
            #endregion
            var createPdfPaymentInfoResponse = await _fileClientService.CreatePdfPaymentInfo(createPdfPaymentInfoRequest);
            #endregion
            if (createPdfPaymentInfoResponse.ResultType == ResultType.Ok)
            {
                #region response  dto setleniyor
                panelExportPdfPaymentInfoResponse.FileKey = createPdfPaymentInfoResponse.Data.FileKey;
                panelExportPdfPaymentInfoResponse.FileUrl = createPdfPaymentInfoResponse.Data.FileUrl;
                #endregion
                return new SuccessResult<PanelExportPdfPaymentInfoResponseDto>(panelExportPdfPaymentInfoResponse);
            }
            #region excel oluşturma işlemi başarısız
            else
            {
                return new ErrorResult<PanelExportPdfPaymentInfoResponseDto>(panelExportPdfPaymentInfoResponse, PanelPaymentInfoErrorEnum.EXPORT_PDF_FAILED);
            }
            #endregion
        }
        #region private methods
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
        private PanelPaymentInfoErrorEnum GetPaymentChargeDebitCardErrorHandle( int errorCode)
        {
            if (errorCode == (int)PaymentErrorEnum.AN_ERROR_OCCURRED)
            {
                return PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED;
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INFORMATION_NOT_FOUND_ERROR)
            {
                return PanelPaymentInfoErrorEnum.PAYMENT_INFORMATION_NOT_FOUND_ERROR;
            }
            else if (errorCode == (int)PaymentErrorEnum.CARD_INFORMATION_INCORRECT)
            {
                return PanelPaymentInfoErrorEnum.CARD_INFORMATION_INCORRECT;
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND)
            {
                return PanelPaymentInfoErrorEnum.PAYMENT_INTEGRATION_NOT_FOUND;
            }
            else if (errorCode == (int)PaymentErrorEnum.FAILED_TO_STARTING_3D_VERIFICATION)
            {
                return PanelPaymentInfoErrorEnum.FAILED_TO_STARTING_3D_VERIFICATION;
            }
            else if (errorCode == (int)PaymentErrorEnum.VERIFICATION_3D_FAILED)
            {
                return PanelPaymentInfoErrorEnum.VERIFICATION_3D_FAILED;
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
                return PanelPaymentInfoErrorEnum.PAYMENT_FAILED;
            }
            else if (errorCode == (int)PaymentErrorEnum.INVALID_CARD_INFO)
            {
                return PanelPaymentInfoErrorEnum.CARD_INFORMATION_INCORRECT;
            }
            else if (errorCode == (int)PaymentErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION)
            {
                return PanelPaymentInfoErrorEnum.PAYMENT_INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY_TRANSACTION;
            }
            else if (errorCode == (int)PaymentErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE)
            {
                return PanelPaymentInfoErrorEnum.THIS_INSTALLMENT_NUMBER_NOT_AVAILABLE;
            }
            else if (errorCode == (int)PaymentErrorEnum.CARD_STORAGE_NOT_AVAILABLE_FOR_DEALER)
            {
                return PanelPaymentInfoErrorEnum.CARD_STORAGE_NOT_AVAILABLE_FOR_DEALER;
            }
            else if (errorCode == (int)PaymentErrorEnum.VIRTUAL_POS_NOT_AVAILABLE_FOR_DEBIT_CARD)
            {
                return PanelPaymentInfoErrorEnum.VIRTUAL_POS_NOT_AVAILABLE_FOR_DEBIT_CARD;
            }
            else if (errorCode == (int)PaymentErrorEnum.FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER)
            {
                return PanelPaymentInfoErrorEnum.FOREIGN_CURRENCY_NOT_AVAILABLE_FOR_THIS_DEALER;
            }
            else if (errorCode == (int)PaymentErrorEnum.INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY)
            {
                return PanelPaymentInfoErrorEnum.INSTALLMENT_NOT_AVAILABLE_FOR_FOREIGN_CURRENCY;
            }
            else
            {
                return PanelPaymentInfoErrorEnum.AN_ERROR_OCCURRED;
            }
        }


        #endregion
    }

}
