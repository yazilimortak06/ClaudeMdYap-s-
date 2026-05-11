using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.PanelDashboardDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.StationContentDtos;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.PaymentInfoRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationContentRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.DashboardServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Services.PanelServices.Dashboard
{
    public class DashboardService : BaseService, IDashboardService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IStationRepository _stationRepository;
        private readonly IStationContentRepository _stationContentRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IPaymentInfoRepository _paymentInfoRepository;
        private readonly IAdminContextProvider _adminContextProvider;

        public DashboardService(IMapper mapper,
                            IConfiguration configuration,
                            IUtilService utilService,
                            IAdminContextProvider adminContextProvider,
                            IStationRepository stationRepository,
                            IStationContentRepository stationContentRepository,
                            IChargeRepository chargeRepository,
                            IPaymentInfoRepository paymentInfoRepository) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _utilService = utilService;
            _adminContextProvider = adminContextProvider;
            _stationRepository = stationRepository;
            _stationContentRepository = stationContentRepository;
            _chargeRepository = chargeRepository;
            _paymentInfoRepository = paymentInfoRepository;
        }
        /// <summary>
        /// İstasyonlar getiriliyor
        /// </summary>
        /// <param name="getPanelStationForDashboardRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelStationForDashboardResponseDto>> GetStations(GetPanelStationForDashboardRequestDto getPanelStationForDashboardRequest)
        {
            #region response dto oluşturuluyor
            GetPanelStationForDashboardResponseDto getPanelStationForDashboardResponse = new GetPanelStationForDashboardResponseDto();
            getPanelStationForDashboardResponse.Stations = new List<PanelStationDashboardItemDto>();
            #endregion
            #region istasyonlar çekiliyor
            #region filter dto oluşturuluyor
            StationContentFilterDto stationContentFilter = _mapper.Map<StationContentFilterDto>(getPanelStationForDashboardRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
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
            var stationList = await _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationContentFilter).ToListAsync();
            #endregion
            #region istasyonlar dto ya mapleniyor
            getPanelStationForDashboardResponse.Stations = _mapper.Map<List<PanelStationDashboardItemDto>>(stationList, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as List<PanelStationDashboardItemDto>;
                    var srcData = src as List<StationContent>;
                    destData.ForEach(item =>
                    {
                        if (srcData.Where(x => x.Station.Id == item.Id).FirstOrDefault() != null)
                        {
                            item.DeviceCount = srcData.Where(x => x.Station.Id == item.Id).FirstOrDefault().Station.ChargeDevice.Where(x => !x.Deleted).Count();
                            item.AvailableDeviceCount = srcData.Where(x => x.Station.Id == item.Id).FirstOrDefault().Station.ChargeDevice.Where(x => !x.Deleted && x.State == ChargeDeviceStateEnum.ACTIVE &&
                                                                                      (x.ChargeDeviceConnector.Any(x => x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE && x.State == ChargeDeviceConnectorStateEnum.ACTIVE)))
                                                                                      .Count();
                        }
                    });
                });
            });
            #endregion
            return new SuccessResult<GetPanelStationForDashboardResponseDto>(getPanelStationForDashboardResponse);
        }
        /// <summary>
        /// aylık şarj işlem bilgileri getiriliyor
        /// </summary>
        /// <param name="getPanelMonthlyChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelMonthlyChargeResponseDto>> GetMonthlyCharge(GetPanelMonthlyChargeRequestDto getPanelMonthlyChargeRequest)
        {
            try
            {
                #region response dto oluşturuluyor
                GetPanelMonthlyChargeResponseDto getPanelMonthlyChargeResponse = new GetPanelMonthlyChargeResponseDto();
                getPanelMonthlyChargeResponse.ChargeWithAc = new List<PanelMonthlyChargeDashboardItemDto>();
                getPanelMonthlyChargeResponse.ChargeWithDc = new List<PanelMonthlyChargeDashboardItemDto>();
                #endregion
                #region Ac cihaz için aylık şarj bilgileri çekiliyor
                #region filter dto oluşturuluyor
                ChargeFilterDto acChargeFilter = _mapper.Map<ChargeFilterDto>(getPanelMonthlyChargeRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as ChargeFilterDto;
                        if (_adminContextProvider.CompanyId != null)
                        {
                            destData.CompanyId = _adminContextProvider.CompanyId;
                        }
                        if (_adminContextProvider.FirmId != null)
                        {
                            destData.FirmId = _adminContextProvider.FirmId;
                        }
                        destData.ChargeDevicePowerTypeId = (long)ChargeDevicePowerTypeEnum.AC;
                        destData.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.COMPLETED, ChargeStateEnum.PAYMENT_FAIL, ChargeStateEnum.PAYMENT_BEING_RECEIVED };
                    });
                });
                #endregion
                var acCharge = _chargeRepository.GetCharge(acChargeFilter,null);
                #region şarj bilgileri aylara göre gruplandırılıyor
                getPanelMonthlyChargeResponse.ChargeWithAc = await (from process in acCharge
                                                                    select new
                                                                    {
                                                                        date = process.StartTime.Value.Month.ToString() + "-" + process.StartTime.Value.Year.ToString(),
                                                                        loadedKw = Math.Round(process.LoadedKw != null ? process.LoadedKw.Value : 0, 2)
                                                                    }).GroupBy(cc => cc.date).
                                     Select(p => new PanelMonthlyChargeDashboardItemDto()
                                     {
                                         DateText = p.Key,
                                         TotalLoadedKw = p.Select(p => p.loadedKw).Sum()
                                     }).ToListAsync();
                #endregion
                #endregion
                #region Dc cihaz için aylık şarj bilgileri çekiliyor
                #region filter dto oluşturuluyor
                ChargeFilterDto dcChargeFilter = _mapper.Map<ChargeFilterDto>(getPanelMonthlyChargeRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as ChargeFilterDto;
                        if (_adminContextProvider.CompanyId != null)
                        {
                            destData.CompanyId = _adminContextProvider.CompanyId;
                        }
                        if (_adminContextProvider.FirmId != null)
                        {
                            destData.FirmId = _adminContextProvider.FirmId;
                        }
                        destData.ChargeDevicePowerTypeId = (long)ChargeDevicePowerTypeEnum.DC;
                        destData.ChargeStateList = new List<ChargeStateEnum>() { ChargeStateEnum.COMPLETED, ChargeStateEnum.PAYMENT_FAIL, ChargeStateEnum.PAYMENT_BEING_RECEIVED };
                    });
                });
                #endregion
                var dcChargeProcess = _chargeRepository.GetCharge(dcChargeFilter, null);
                #region şarj bilgileri aylara göre gruplandırılıyor
                getPanelMonthlyChargeResponse.ChargeWithDc = await (from process in dcChargeProcess
                                                                    select new
                                                                    {
                                                                        date = process.StartTime.Value.Month.ToString() + "-" + process.StartTime.Value.Year.ToString(),
                                                                        loadedKw = Math.Round(process.LoadedKw != null ? process.LoadedKw.Value : 0, 2)
                                                                    }).GroupBy(cc => cc.date).
                                     Select(p => new PanelMonthlyChargeDashboardItemDto()
                                     {
                                         DateText = p.Key,
                                         TotalLoadedKw = p.Select(p => p.loadedKw).Sum()
                                     }).ToListAsync();
                #endregion
                #endregion
                return new SuccessResult<GetPanelMonthlyChargeResponseDto>(getPanelMonthlyChargeResponse);
            }
            catch (Exception ee)
            {

                throw;
            }
      
        }
        /// <summary>
        /// şarj ve rezervasyonların toplam işlem sayıları getiriliyor
        /// </summary>
        /// <returns></returns>
        public async Task<Result<GetPanelSumOfProcessResponseDto>> GetSumOfProcess(GetPanelSumOfProcessRequestDto getPanelSumOfProcessRequest)
        {
            #region response dto oluşturuluyor
            GetPanelSumOfProcessResponseDto getPanelSumOfProcessResponse = new GetPanelSumOfProcessResponseDto();
            getPanelSumOfProcessResponse.SumOfProcess = new PanelSumOfProcessDto(); 
            #endregion
            #region ödemeler çekiliyor
            #region filter dto oluşturuluyor
            PaymentInfoFilterDto paymentInfoFilter = new PaymentInfoFilterDto();
            paymentInfoFilter.Year = getPanelSumOfProcessRequest.Year;
            paymentInfoFilter.PaymentReason = PaymentReasonEnum.CHARGE;
            if (_adminContextProvider.CompanyId != null)
            {
                paymentInfoFilter.CompanyId = _adminContextProvider.CompanyId;
            }
            if (_adminContextProvider.FirmId != null)
            {
                paymentInfoFilter.FirmId = _adminContextProvider.FirmId;
            }
            #endregion
            var payments = _paymentInfoRepository.GetPaymentInfoAsNoTracking(paymentInfoFilter);
            var sumOfProcessList = await (from payment in payments
                                          select new PanelSumOfProcessDto()
                                          {
                                              TotalPaidPrice = payment.PaidPrice - payment.RefundedPrice.GetValueOrDefault(),
                                              TotalLoadedKw = payment.Charge != null ? payment.Charge.LoadedKw.GetValueOrDefault() : 0,
                                              TotalReservationCount = 0,
                                              TotalSuccessfulyReservationCount = 0
                                          }).ToListAsync();
            #region response dto ya toplam fiyat setleniyor
            getPanelSumOfProcessResponse.SumOfProcess.TotalPaidPrice = Math.Round(sumOfProcessList.Select(x => x.TotalPaidPrice).Sum(), 2);
            #endregion
            #endregion
            #region şarj işlemleri çekiliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = new ChargeFilterDto();
            chargeFilter.Year = getPanelSumOfProcessRequest.Year;
            chargeFilter.ChargeStateList = new List<ChargeStateEnum>() {
                 ChargeStateEnum.COMPLETED,
                 ChargeStateEnum.PAYMENT_FAIL,
                 ChargeStateEnum.PAYMENT_BEING_RECEIVED
            };
            if (_adminContextProvider.CompanyId != null)
            {
                chargeFilter.CompanyId = _adminContextProvider.CompanyId;
            }
            if (_adminContextProvider.FirmId != null)
            {
                chargeFilter.FirmId = _adminContextProvider.FirmId;
            }
            #endregion
            var chargeProcess = await _chargeRepository.GetCharge(chargeFilter, null).ToListAsync();
            var sumOfLoadedKw = chargeProcess.Select(x => x.LoadedKw).Sum();
            #endregion
            #region response dto ya toplam kw setleniyor
            getPanelSumOfProcessResponse.SumOfProcess.TotalLoadedKw = Math.Round(sumOfLoadedKw.GetValueOrDefault(), 2);
            #endregion
            //#region rezervasyonlar çekiliyor
            //#region filter dto oluşturuluyor
            //ChargeDeviceReservationFilterDto chargeDeviceReservationFilter = new ChargeDeviceReservationFilterDto();
            //if (_adminContextProvider.CompanyId != null)
            //{
            //    chargeDeviceReservationFilter.CompanyId = _adminContextProvider.CompanyId;
            //}
            //else
            //{
            //    chargeDeviceReservationFilter.CompanyId = getSumOfProcessRequest.CompanyId;
            //}
            //#endregion
            //var reservations = await _chargeDeviceReservationRepository.GetChargeDeviceReservationAsNoTracking(chargeDeviceReservationFilter).ToListAsync();
            //#endregion
            //#region rezervasyon bilgileri response dto ya setleniyor
            //sumOfProcess.TotalReservationCount = reservations.Count();
            //sumOfProcess.TotalSuccessfulyReservationCount = reservations.Where(x => x.ChargeDeviceReservationState == ChargeDeviceReservationStateEnum.COMPLETED).Count();
            //#endregion
            return new SuccessResult<GetPanelSumOfProcessResponseDto>(getPanelSumOfProcessResponse);
        }
    }
}
