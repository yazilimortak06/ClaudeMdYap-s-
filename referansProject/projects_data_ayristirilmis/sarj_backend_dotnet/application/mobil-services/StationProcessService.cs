using AutoMapper;
using Castle.Core.Resource;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using GeoAPI.Geometries;
using Geolocation;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.ChargeDeviceConnectorDtos;
using Shared.Domain.Dto.ApiDto.ChargeDeviceDtos;
using Shared.Domain.Dto.ApiDto.FirmDtos;
using Shared.Domain.Dto.ApiDto.LocationAreaDtos;
using Shared.Domain.Dto.ApiDto.MobilChargeDeviceDtos;
using Shared.Domain.Dto.ApiDto.MobilFirmDtos;
using Shared.Domain.Dto.ApiDto.MobilStationDtos;
using Shared.Domain.Dto.ApiDto.OpportunityDtos;
using Shared.Domain.Dto.ApiDto.PaymentMethodDtos;
using Shared.Domain.Dto.ApiDto.StationContentDtos;
using Shared.Domain.Dto.ApiDto.StationDtos;
using Shared.Domain.Dto.ApiDto.StationLocationAreaDtos;
using Shared.Domain.Dto.ApiDto.StationOpportunityDtos;
using Shared.Domain.Dto.ApiDto.StationPictureDtos;
using Shared.Domain.Dto.ApiDto.StationTypeOfUsingDtos;
using Shared.Domain.Entities.ApiEntities.LocationAreaModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Errors.MobilApiErrors;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceConnectorRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.FirmRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.LocationAreaRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationAddressRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationContentRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationDetailRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.StationProcessServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationPictureRepositoryInterfaces;
using Shared.Domain.Dto.ApiDto.StationRatingDtos;
using Shared.Domain.Dto.ApiDto.StationFacilityDtos;
using Api.Application.Extentions;
using Shared.Domain.Enums.TockenEnums;
using Shared.Domain.Dto.ApiDto.MobilStationRatingDtos;

namespace Api.Application.Services.MobilApiServices.StationProcess
{
    public class StationProcessService : BaseService, IStationProcessService
    {

        private readonly IStationRepository _stationRepository;
        private readonly IStationContentRepository _stationContentRepository;
        private readonly IStationAddressRepository _stationAddressRepository;
        private readonly IStationInfoRepository _stationDetailRepository;
        private readonly IStationLocationAreaRepository _stationLocationAreaRepository;
        private readonly IStationPictureRepository _stationPictureRepository;
        private readonly IStationRatingRepository _stationRatingRepository;
        private readonly ILocationAreaRepository _locationAreaRepository;
        private readonly IFavoriteStationRepository _favoriteStationRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IFacilityRepository _facilityRepository;
        private readonly IFirmRepository _firmRepository;
        private readonly IChargeDeviceRepository _chargeDeviceRepository;
        private readonly IChargeDevicePowerTypeRepository _chargeDevicePowerTypeRepository;
        private readonly IChargeDeviceConnectorRepository _chargeDeviceConnectorRepository;
        private readonly IChargeDeviceSocketTypeRepository _chargeDeviceSocketTypeRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUtilService _utilService;
        private readonly IConfiguration _configuration;
        private readonly IUserContextProvider _userContextProvider;
        private readonly ILogger<StationProcessService> _logger;

        public StationProcessService(IMapper mapper,
                           IChargeDeviceRepository chargeDeviceRepository,
                           IConfiguration configuration,
                           IChargeDevicePowerTypeRepository chargeDevicePowerTypeRepository,
                           IChargeDeviceSocketTypeRepository chargeDeviceSocketTypeRepository,
                           IPaymentMethodRepository paymentMethodRepository,
                           IUserContextProvider userContextProvider,
                           ILogger<StationProcessService> logger,
                           IStationRepository stationRepository,
                           IStationContentRepository stationContentRepository,
                           IStationAddressRepository stationAddressRepository,
                           IStationInfoRepository stationDetailRepository,
                           IOpportunityRepository opportunityRepository,
                           IFavoriteStationRepository favoriteStationRepository,
                           IStationLocationAreaRepository stationLocationAreaRepository,
                           ILocationAreaRepository locationAreaRepository,
                           IUtilService utilService,
                           IFirmRepository firmRepository,
                           IChargeDeviceConnectorRepository chargeDeviceConnectorRepository,
                           IStationPictureRepository stationPictureRepository,
                           IStationRatingRepository stationRatingRepository,
                           IFacilityRepository facilityRepository) : base(
                           mapper
                               )
        {
            _chargeDeviceRepository = chargeDeviceRepository;
            _configuration = configuration;
            _chargeDevicePowerTypeRepository = chargeDevicePowerTypeRepository;
            _chargeDeviceSocketTypeRepository = chargeDeviceSocketTypeRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _userContextProvider = userContextProvider;
            _logger = logger;
            _stationRepository = stationRepository;
            _stationContentRepository = stationContentRepository;
            _stationAddressRepository = stationAddressRepository;
            _stationDetailRepository = stationDetailRepository;
            _opportunityRepository = opportunityRepository;
            _favoriteStationRepository = favoriteStationRepository;
            _stationLocationAreaRepository = stationLocationAreaRepository;
            _locationAreaRepository = locationAreaRepository;
            _utilService = utilService;
            _firmRepository = firmRepository;
            _chargeDeviceConnectorRepository = chargeDeviceConnectorRepository;
            _stationPictureRepository = stationPictureRepository;
            _stationRatingRepository = stationRatingRepository;
            _facilityRepository = facilityRepository;
        }
        /// <summary>
        /// mobil istasyon filtreleme ekranı için veriler çekiliyor
        /// </summary>
        /// <param name="mobilStationFilterPrepareRequest"></param>
        /// <returns></returns>
        public async Task<Result<MobilStationFilterPrepareResponseDto>> PrepareStationFilter(MobilStationFilterPrepareRequestDto mobilStationFilterPrepareRequest)
        {
            MobilStationFilterPrepareResponseDto mobilStationFilterPrepareResponse = new MobilStationFilterPrepareResponseDto();
            #region cihaz güç tipleri çekiliyor
            var chargeDevicePowerTypesEntity = await _chargeDevicePowerTypeRepository.GetChargeDevicePowerTypes().ToListAsync();
            mobilStationFilterPrepareResponse.ChargeDevicePowerTypes = _mapper.Map<List<ChargeDevicePowerTypeDto>>(chargeDevicePowerTypesEntity);
            #endregion
            #region cihaz soket tipleri çekiliyor
            var chargeDeviceSocketTypesEntity = await _chargeDeviceSocketTypeRepository.GetChargeDeviceSocketTypes().ToListAsync();
            mobilStationFilterPrepareResponse.ChargeDeviceSocketTypes = _mapper.Map<List<ChargeDeviceSocketTypeDto>>(chargeDeviceSocketTypesEntity);
            #endregion
            #region istasyon ödeme metodları çekiliyor
            var paymentMethods = await _paymentMethodRepository.GetPaymentMethod().ToListAsync();
            mobilStationFilterPrepareResponse.PaymentMethods = _mapper.Map<List<PaymentMethodDto>>(paymentMethods);
            #endregion
            #region olanaklar çekiliyor
            var opportunities = await _opportunityRepository.GetOpportunitiesAsNoTracking(new OpportunityFilterDto() { }).ToListAsync();
            mobilStationFilterPrepareResponse.Opportunities = _mapper.Map<List<OpportunityDto>>(opportunities);
            #endregion
            #region olanaklar çekiliyor
            var facilities = await _facilityRepository.GetFacilityAsNoTracking().ToListAsync();
            mobilStationFilterPrepareResponse.Facilities = _mapper.Map<List<FacilityDto>>(facilities);
            #endregion
            #region max ve min kw değerleri setleniyor
            //var connectorForKw = _chargeDeviceConnectorRepository.GetOnlyChargeDeviceConnectorAsNoTracking(new ChargeDeviceConnectorFilterDto() { });
            var connectorForKw = _chargeDeviceConnectorRepository.GetChargeDeviceConnector(new ChargeDeviceConnectorFilterDto() { },null);
            mobilStationFilterPrepareResponse.MaxKw = await connectorForKw.OrderByDescending(x => x.kW).Select(x => x.kW).FirstOrDefaultAsync();
            mobilStationFilterPrepareResponse.MinKw = await connectorForKw.OrderBy(x => x.kW).Select(x => x.kW).FirstOrDefaultAsync();
            #endregion
            if (mobilStationFilterPrepareRequest.Latitude != null && mobilStationFilterPrepareRequest.Longtitude != null)
            {
                #region max ve min mesafe setleniyor
                Point location = _utilService.GetPointOfLocationCoordinateCartesian(mobilStationFilterPrepareRequest.Latitude, mobilStationFilterPrepareRequest.Longtitude);
                //var maxDistance = await _stationRepository.GetStationAsNoTracking(new StationFilterDto() { }).OrderByDescending(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                var maxDistance = await _stationRepository.GetStation(new StationFilterDto() { }, include: source => source
     .Include(a => a.StationAddress)).OrderByDescending(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                //var minDistance = await _stationRepository.GetStationAsNoTracking(new StationFilterDto() { }).OrderBy(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                var minDistance = await _stationRepository.GetStation(new StationFilterDto() { }, include: source => source
     .Include(a => a.StationAddress)).OrderBy(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                mobilStationFilterPrepareResponse.MaxDistance = (int)Math.Ceiling(maxDistance);
                mobilStationFilterPrepareResponse.MinDistance = (int)Math.Floor(minDistance);
                #endregion
            }
            #region max ve min fiyat setleniyor
            //var connectorForPrice = _chargeDeviceConnectorRepository.GetOnlyChargeDeviceConnectorAsNoTracking(new ChargeDeviceConnectorFilterDto() { });
            var connectorForPrice = _chargeDeviceConnectorRepository.GetChargeDeviceConnector(new ChargeDeviceConnectorFilterDto() { }, null);
            var maxPrice = await connectorForKw.OrderByDescending(x => x.PriceWithKdv).Select(x => x.PriceWithKdv).FirstOrDefaultAsync();
            var minPirce = await connectorForKw.OrderBy(x => x.PriceWithKdv).Select(x => x.PriceWithKdv).FirstOrDefaultAsync();
            mobilStationFilterPrepareResponse.MaxPrice = (int)Math.Ceiling(maxPrice);
            mobilStationFilterPrepareResponse.MinPrice = (int)Math.Floor(minPirce);
            #endregion
            return new SuccessResult<MobilStationFilterPrepareResponseDto>(mobilStationFilterPrepareResponse);
        }
        /// <summary>
        /// mobil için istasyon verileri çekiliyor
        /// </summary>
        /// <param name="getMobilStationListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationListResponseDto>> GetStationList(GetMobilStationListRequestDto getMobilStationListRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationListResponseDto getMobilStationListResponse = new GetMobilStationListResponseDto();
            getMobilStationListResponse.Stations = new List<MobilStationListItemDto>();
            getMobilStationListResponse.TotalRecord = 0;
            #endregion
            #region istasyonlar getiriliyor
            #region filter dto oluşturuluyor
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getMobilStationListRequest, opt =>
            {
                opt.BeforeMap((src, dest) =>
                {
                    var srcData = src as GetMobilStationListRequestDto;
                    if (srcData.Filter.Latitude != null && srcData.Filter.Longtitude != null)
                    {
                        srcData.Filter.Latitude = srcData.Filter.Latitude.Replace(",", ".");
                        srcData.Filter.Longtitude = srcData.Filter.Longtitude.Replace(",", ".");
                    }
                });
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.IsActive = true;
                    destData.IsTestStation = false;
                });
            });
            #endregion
            #region lokasyon verisi oluşturuluyor
            Point location = _utilService.GetPointOfLocationCoordinateCartesian(stationContentFilter.Latitude, stationContentFilter.Longtitude);
            #endregion
            #region istasyon contentler getiriliyor
            //var stations = _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationContentFilter);
            var stations = _stationContentRepository.GetStationContent(stationContentFilter, include: source => source
     .Include(a => a.Station)
     .ThenInclude(a => a.ChargeDevice.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceConnector.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDevicePowerType)
        .Include(a => a.Station).ThenInclude(a => a.ChargeDevice.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceConnector.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceSocketType)
     .Include(a => a.Station)
          .ThenInclude(a => a.FavoriteStation.Where(x => !x.Deleted && stationContentFilter.UserId != null ? x.UserId == stationContentFilter.UserId : true))
      .Include(pc => pc.Station.StationPaymentMethod.Where(x => !x.Deleted && !x.PaymentMethod.Deleted)).ThenInclude(pc => pc.PaymentMethod)
            .Include(a => a.Station).ThenInclude(a => a.StationRating)
           .Include(pc => pc.Station).ThenInclude(pc => pc.StationAddress)
        .Include(pc => pc.Station).ThenInclude(pc => pc.StationFacility).ThenInclude(pc => pc.Facility)
          .Include(pc => pc.Station).ThenInclude(pc => pc.StationInfo).ThenInclude(pc => pc.StationOpportunities.Where(x => !x.Deleted && !x.Opportunity.Deleted)).ThenInclude(pc => pc.Opportunity)
               .Include(pc => pc.Station).ThenInclude(pc => pc.StationInfo).ThenInclude(pc => pc.StationTypesOfUsing.Where(x => !x.Deleted && !x.TypesOfUsing.Deleted)).ThenInclude(pc => pc.TypesOfUsing)
         .Include(pc => pc.Station).ThenInclude(pc => pc.Company)
          .Include(pc => pc.Station).ThenInclude(pc => pc.StationPicture.Where(x => !x.Deleted))
             .Include(pc => pc.Station).ThenInclude(pc => pc.Firm).ThenInclude(x => x.FirmLogo));
            #endregion
            #endregion
            #region istasyon ofset ve skip ayarlamaları gerçekleşiyor
            var stationList = new List<StationContent>();
            if (getMobilStationListRequest.Ofset != null && getMobilStationListRequest.Count != null)
            {
                var skip = getMobilStationListRequest.Ofset.GetValueOrDefault() * getMobilStationListRequest.Count.GetValueOrDefault();
                stations = stations.Skip(skip).Take(getMobilStationListRequest.Count.GetValueOrDefault());
            }
            #endregion
            #region istasyon sıralaması ayarlanıyor
            stations = stations.OrderStationContent(getMobilStationListRequest.IsAscending,  getMobilStationListRequest.OrderType, location);
            #endregion
            stationList = await stations.ToListAsync();
            #region istasyon sayısı setleniyor
            getMobilStationListResponse.TotalRecord = await stations.CountAsync();
            #endregion
            #region istasyon entity mapleniyor
            var stationManagmentListDto = _mapper.Map<List<MobilStationListItemDto>>(stationList, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as List<MobilStationListItemDto>;
                    var srcData = src as List<StationContent>;
                    destData.ForEach(item =>
                    {
                        var stationContent = srcData.Where(x => x.Station.Id == item.Id).FirstOrDefault();
                        if (stationContent != null)
                        {
                            item.Distance = _utilService.ParseInt(stationContent.Station.StationAddress.LocationCoordinateCartesian.Distance(location));
                            item.IsFavorite = _userContextProvider.SessionType == MobilSessionTypeEnum.USER ?
                          stationContent.Station.FavoriteStation.Where(x => x.UserId == _userContextProvider.UserId && !x.Deleted).Any() :
                          false;
                        }
                       
                    });
                });
            });
            #endregion
            #region response dto setleniyor
            getMobilStationListResponse.Stations = stationManagmentListDto;
            #endregion
            return new SuccessResult<GetMobilStationListResponseDto>(getMobilStationListResponse);
        }
 
        /// <summary>
        /// mobil için istasyon filtreleme esnasında istasyon sayısı getiriliyor
        /// </summary>
        /// <param name="getStationFilterCountRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetStationFilterCountResponseDto>> GetStationFilterCount(GetStationFilterCountRequestDto getStationFilterCountRequest)
        {
            #region response dto oluşturuluyor
            GetStationFilterCountResponseDto getStationFilterCountResponse = new GetStationFilterCountResponseDto();
            getStationFilterCountResponse.StationCount = 0;
            #endregion
            #region istasyonlar getiriliyor
            #region filter dto oluşturuluyor
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getStationFilterCountRequest, opt =>
            {
                opt.BeforeMap((src, dest) =>
                {
                    var srcData = src as GetMobilStationListRequestDto;
                    if (srcData.Filter.Latitude != null && srcData.Filter.Longtitude != null)
                    {
                        srcData.Filter.Latitude = srcData.Filter.Latitude.Replace(",", ".");
                        srcData.Filter.Longtitude = srcData.Filter.Longtitude.Replace(",", ".");
                    }
                });
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.IsTestStation = false;
                });
            });
            #endregion
            #endregion
            #region istasyon sayısı setleniyor
            //getStationFilterCountResponse.StationCount = await _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationContentFilter).CountAsync();
            getStationFilterCountResponse.StationCount = await _stationContentRepository.GetStationContent(stationContentFilter,null).CountAsync();
            #endregion
            return new SuccessResult<GetStationFilterCountResponseDto>(getStationFilterCountResponse);
        }
        /// <summary>
        /// mobil harita için istasyon verileri çekiliyor
        /// </summary>
        /// <param name="getMobilStationMapListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationMapListResponseDto>> GetStationMapList(GetMobilStationMapListRequestDto getMobilStationMapListRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationMapListResponseDto getMobilStationMapListResponse = new GetMobilStationMapListResponseDto();
            getMobilStationMapListResponse.Stations = new List<MobilStationMapListItemDto>();
            getMobilStationMapListResponse.StationLocations = new List<MobilLocationAreaItemDto>();
            #endregion
            #region level 1 ise bütün location area lar getiriliyor
            if (getMobilStationMapListRequest.Level == 1)
            {
                #region filter dto oluşturuluyor
                LocationAreaFilterDto locationAreaFilter = _mapper.Map<LocationAreaFilterDto>(getMobilStationMapListRequest, opt =>
                {
                    opt.BeforeMap((src, dest) =>
                    {
                        var srcData = src as GetMobilStationMapListRequestDto;
                    });
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as LocationAreaFilterDto;
                        destData.Level = 1;
                        destData.IsStationActive = true;
                        destData.CheckStationCount = true;
                    });
                });
                #endregion
                var locationAreaList = await _locationAreaRepository.GetLocationAreaWithStationLocationAreaAsNoTracking(locationAreaFilter).ToListAsync();
                #region response dto setleniyor
                getMobilStationMapListResponse.StationLocations = _mapper.Map<List<MobilLocationAreaItemDto>>(locationAreaList);
                #endregion
            }
            #endregion
            else if (getMobilStationMapListRequest.Level == 2)
            {
                #region filter dto oluşturuluyor
                LocationAreaFilterDto locationAreaFilter = _mapper.Map<LocationAreaFilterDto>(getMobilStationMapListRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as LocationAreaFilterDto;
                        destData.Level = 2;
                        destData.CheckStationCount = true;
                        destData.IsStationActive = true;
                    });
                });
                #endregion
                var locationAreaList = await _locationAreaRepository.GetLocationAreaWithStationLocationAreaAsNoTracking(locationAreaFilter).ToListAsync();
                #region response dto setleniyor
                getMobilStationMapListResponse.StationLocations = _mapper.Map<List<MobilLocationAreaItemDto>>(locationAreaList);
                #endregion
            }
            else if (getMobilStationMapListRequest.Level == 3)
            {
                #region filter dto oluşturuluyor
                LocationAreaFilterDto locationAreaFilter = _mapper.Map<LocationAreaFilterDto>(getMobilStationMapListRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as LocationAreaFilterDto;
                        destData.Level = 3;
                        destData.CheckStationCount = true;
                        destData.IsStationActive = true;
                    });
                });
                #endregion
                #region station location area predicate oluşturuluyor
                var predicateStationLocationArea = _locationAreaRepository.PredicateStationLocationArea(locationAreaFilter);
                #endregion
                //var locationAreaList = await _locationAreaRepository.GetLocationAreaWithAllIncludesAsNoTracking(locationAreaFilter).ToListAsync();
                var locationAreaList = await _locationAreaRepository.GetLocationArea(locationAreaFilter, include: source => source
     .Include(x => x.StationLocationArea.AsQueryable().Where(predicateStationLocationArea)).ThenInclude(x => x.Station).ThenInclude(x => x.StationContent)
     .Include(x => x.StationLocationArea).ThenInclude(x => x.Station).ThenInclude(x => x.StationAddress)
       .Include(x => x.StationLocationArea).ThenInclude(x => x.Station).ThenInclude(x => x.Firm).ThenInclude(x => x.FirmLogo)
       .Include(x => x.LocationAreaConfig)).ToListAsync();
                #region response dto setleniyor
                getMobilStationMapListResponse.Stations = _mapper.Map<List<MobilStationMapListItemDto>>(locationAreaList.SelectMany(x => x.StationLocationArea).Select(x => x.Station).ToList());
                #region istasyon sayısı setleniyor
                getMobilStationMapListResponse.TotalRecord = getMobilStationMapListResponse.Stations.Count();
                #endregion
                #endregion
            }
            return new SuccessResult<GetMobilStationMapListResponseDto>(getMobilStationMapListResponse);
        }

        /// <summary>
        /// mobil  için istasyon liste item verisi çekiliyor
        /// </summary>
        /// <param name="getMobilStationListDetailRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationListDetailResponseDto>> GetStationListDetail(GetMobilStationListDetailRequestDto getMobilStationListDetailRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationListDetailResponseDto getMobilStationListDetailResponse = new GetMobilStationListDetailResponseDto();
            getMobilStationListDetailResponse.Station = new MobilStationListItemDto();
            #endregion
            #region istasyon getiriliyor
            #region filter dto oluşturuluyor
            var stationManagmentWithContentFilter = _mapper.Map<StationContentFilterDto>(getMobilStationListDetailRequest, opt =>
            {
                opt.BeforeMap((src, dest) =>
                {
                    var srcData = src as GetMobilStationListDetailRequestDto;
                    if (srcData.Latitude != null && srcData.Longtitude != null)
                    {
                        srcData.Latitude = srcData.Latitude.Replace(",", ".");
                        srcData.Longtitude = srcData.Longtitude.Replace(",", ".");
                    }
                });
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.IsTestStation = false;
                });
            });
            #endregion
            //var stationManagment = await _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationManagmentWithContentFilter).FirstOrDefaultAsync();
            #region istasyon contentler getiriliyor
            var stationManagment = await _stationContentRepository.GetStationContent(stationManagmentWithContentFilter, include: source => source
     .Include(a => a.Station).ThenInclude(a => a.ChargeDevice.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceConnector.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDevicePowerType)
        .Include(a => a.Station).ThenInclude(a => a.ChargeDevice.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceConnector.Where(x => !x.Deleted)).ThenInclude(a => a.ChargeDeviceSocketType)
     .Include(a => a.Station).ThenInclude(a => a.FavoriteStation.Where(x => !x.Deleted && stationManagmentWithContentFilter.UserId != null ? x.UserId == stationManagmentWithContentFilter.UserId : true))
      .Include(pc => pc.Station.StationPaymentMethod.Where(x => !x.Deleted && !x.PaymentMethod.Deleted)).ThenInclude(pc => pc.PaymentMethod)
            .Include(a => a.Station).ThenInclude(a => a.StationRating)
           .Include(pc => pc.Station).ThenInclude(pc => pc.StationAddress)
        .Include(pc => pc.Station).ThenInclude(pc => pc.StationFacility).ThenInclude(pc => pc.Facility)
          .Include(pc => pc.Station).ThenInclude(pc => pc.StationInfo).ThenInclude(pc => pc.StationOpportunities.Where(x => !x.Deleted && !x.Opportunity.Deleted)).ThenInclude(pc => pc.Opportunity)
               .Include(pc => pc.Station).ThenInclude(pc => pc.StationInfo).ThenInclude(pc => pc.StationTypesOfUsing.Where(x => !x.Deleted && !x.TypesOfUsing.Deleted)).ThenInclude(pc => pc.TypesOfUsing)
         .Include(pc => pc.Station).ThenInclude(pc => pc.Company)
          .Include(pc => pc.Station).ThenInclude(pc => pc.StationPicture.Where(x => !x.Deleted))
             .Include(pc => pc.Station).ThenInclude(pc => pc.Firm).ThenInclude(x => x.FirmLogo)).FirstOrDefaultAsync();
            #endregion
            #endregion
            if (stationManagment != null)
            {
                Point location = _utilService.GetPointOfLocationCoordinateCartesian(getMobilStationListDetailRequest.Latitude, getMobilStationListDetailRequest.Longtitude);
                #region istasyon entitiy response dto ya mapleniyor
                getMobilStationListDetailResponse.Station = _mapper.Map<MobilStationListItemDto>(stationManagment, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as MobilStationListItemDto;
                        var srcData = src as StationContent;
                        destData.DeviceCount = srcData.Station.ChargeDevice.Where(x => !x.Deleted).Count();
                        if (srcData.Station.IsActive)
                        {
                            destData.AvailableDeviceCount = srcData.Station.ChargeDevice.Where(x => !x.Deleted && x.State == ChargeDeviceStateEnum.ACTIVE &&
                                                                            (x.ChargeDeviceConnector.Any(x => x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE && x.State == ChargeDeviceConnectorStateEnum.ACTIVE)))
                                                                            .Count();
                        }
                        else
                        {
                            destData.AvailableDeviceCount = 0;
                        }
                        destData.IsFavorite = _userContextProvider.SessionType == MobilSessionTypeEnum.USER ?
                        srcData.Station.FavoriteStation.Where(x => x.UserId == _userContextProvider.UserId && !x.Deleted).Any() :
                        false;
                        #region mesafe verisi setleniyor
                        destData.Distance = _utilService.ParseInt(srcData.Station.StationAddress.LocationCoordinateCartesian.Distance(location));
                        #endregion
                    });
                });
                #endregion
                #region istasyonlar ve kullanıcı mesafe bilgileri için googleservice api ye istek atılıyor
                //if (getMobilStationListDetailRequest.Latitude != null && getMobilStationListDetailRequest.Longtitude != null)
                //{
                //    #region request dto oluşturuluyor
                //    GetStationUserDistanceRequestDto getStationUserDistanceRequest = _mapper.Map<GetStationUserDistanceRequestDto>(getStationListDetailRequest);
                //    getStationUserDistanceRequest.StationLocationGuiIdList = new List<string>() { stationManagment.StationManagment.StationLocationGuiId };
                //    #endregion
                //    var getStationUserDistanceResponse = await _stationLocationClientService.GetStationUserDistance(getStationUserDistanceRequest);
                //    #region mesafe bilgileri dto ya setleniyor
                //    if (getStationUserDistanceResponse.ResultType == ResultType.Ok && getStationUserDistanceResponse.Data.DistanceStationFromUserLocations != null)
                //    {
                //        var distanceResponse = getStationUserDistanceResponse.Data.DistanceStationFromUserLocations.FirstOrDefault();
                //        if (distanceResponse != null)
                //        {
                //            getStationListDetailResponse.Station.Distance = distanceResponse.DistanceText;
                //        }
                //    }
                //    #endregion
                //}
                #endregion
                return new SuccessResult<GetMobilStationListDetailResponseDto>(getMobilStationListDetailResponse);
            }
            #region istasyon bulunamadı
            else
            {
                return new ErrorResult<GetMobilStationListDetailResponseDto>(getMobilStationListDetailResponse , StationProcessErrorEnum.STATION_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// mobil için istasyon detayı çekiliyor
        /// </summary>
        /// <param name="getMobilStationDetailRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationDetailResponseDto>> GetStationDetail(GetMobilStationDetailRequestDto getMobilStationDetailRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationDetailResponseDto getMobilStationDetailResponse = new GetMobilStationDetailResponseDto();
            getMobilStationDetailResponse.StationFacility = new List<MobilStationFacilityDto>();
            #endregion
            #region istasyon getiriliyor
            #region filter dto oluşturuluyor
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getMobilStationDetailRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                    destData.IsTestStation = false;
                });
            });
            #endregion
            var station = await _stationContentRepository.GetStationContentWithAllRelations(stationContentFilter).FirstOrDefaultAsync();
            #endregion
            if (station != null)
            {
                Point location = _utilService.GetPointOfLocationCoordinateCartesian(getMobilStationDetailRequest.Latitude, getMobilStationDetailRequest.Longtitude);
                #region istasyon entity response dto ya mapleniyor
                getMobilStationDetailResponse = _mapper.Map<GetMobilStationDetailResponseDto>(station, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as GetMobilStationDetailResponseDto;
                        var srcData = src as StationContent;
                        if (srcData.Station.StationPaymentMethod.Count == 0)
                        {
                            destData.StationPaymentMethod = new List<MobilStationPaymentMethodDto>();
                        }
                        destData.IsFavorite = _userContextProvider.SessionType == MobilSessionTypeEnum.USER ?
                        srcData.Station.FavoriteStation.Where(x => x.UserId == _userContextProvider.UserId && !x.Deleted).Any() :
                        false;
                        #region mesafe verisi setleniyor
                        destData.Distance = _utilService.ParseInt(srcData.Station.StationAddress.LocationCoordinateCartesian.Distance(location));
                        #endregion
                    });
                });
                #endregion
                return new SuccessResult<GetMobilStationDetailResponseDto>(getMobilStationDetailResponse);
            }
            #region istasyon bulunamadı
            else
            {
                return new ErrorResult<GetMobilStationDetailResponseDto>(getMobilStationDetailResponse, StationProcessErrorEnum.STATION_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// mobil için istasyon resimleri çekiliyor
        /// </summary>
        /// <param name="getMobilStationPictureRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationPictureResponseDto>> GetStationPicture(GetMobilStationPictureRequestDto getMobilStationPictureRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationPictureResponseDto getMobilStationPictureResponse = new GetMobilStationPictureResponseDto();
            #endregion
            #region istasyon resimleri getiriliyor
            #region filter dto oluşturuluyor
            var stationPictureFilter = _mapper.Map<StationPictureFilterDto>(getMobilStationPictureRequest);
            #endregion
            var stationPictures = await _stationPictureRepository.GetStationPictures(stationPictureFilter).ToListAsync();
            #endregion
            #region response dto setleniyor
            getMobilStationPictureResponse.StationPicture = _mapper.Map<List<StationPictureDto>>(stationPictures);
            #endregion
            return new SuccessResult<GetMobilStationPictureResponseDto>(getMobilStationPictureResponse);
        }
        /// <summary>
        /// favori istasyonlar çekiliyor
        /// </summary>
        /// <param name="getMobilFavoriteStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilFavoriteStationResponseDto>> GetFavoriteStation(GetMobilFavoriteStationRequestDto getMobilFavoriteStationRequest)
        {
            #region response dto oluşturuluyor
            GetMobilFavoriteStationResponseDto getMobilFavoriteStationResponse = new GetMobilFavoriteStationResponseDto();
            getMobilFavoriteStationResponse.StationList = new List<MobilFavoriteStationListItemDto>();
            #endregion
            #region favori istasyonlar çekiliyor
            #region filter dto oluşturuluyor
            FavoriteStationFilterDto favoriteStationFilter = _mapper.Map<FavoriteStationFilterDto>(getMobilFavoriteStationRequest, opt =>
            {
                opt.BeforeMap((src, dest) =>
                {
                    var srcData = src as GetMobilFavoriteStationRequestDto;
                    if (srcData.Filter.Latitude != null && srcData.Filter.Longtitude != null)
                    {
                        srcData.Filter.Latitude = srcData.Filter.Latitude.Replace(",", ".");
                        srcData.Filter.Longtitude = srcData.Filter.Longtitude.Replace(",", ".");
                    }
                });
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FavoriteStationFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            var favoriteStationsEntity =  _favoriteStationRepository.GetFavoriteStationAsNoTracking(favoriteStationFilter);
            #endregion
            #region lokasyon verisi oluşturuluyor
            Point location = _utilService.GetPointOfLocationCoordinateCartesian(favoriteStationFilter.Latitude, favoriteStationFilter.Longtitude);
            #endregion
            #region istasyon ofset ve skip ayarlamaları gerçekleşiyor
            var stationList = new List<FavoriteStation>();
            if (getMobilFavoriteStationRequest.Ofset != null && getMobilFavoriteStationRequest.Count != null)
            {
                var skip = getMobilFavoriteStationRequest.Ofset.GetValueOrDefault() * getMobilFavoriteStationRequest.Count.GetValueOrDefault();
                favoriteStationsEntity = favoriteStationsEntity.Skip(skip).Take(getMobilFavoriteStationRequest.Count.GetValueOrDefault());
            }
            #endregion
            #region istasyon sıralaması ayarlanıyor
            favoriteStationsEntity = favoriteStationsEntity.OrderFavoriteStation(getMobilFavoriteStationRequest.IsAscending, getMobilFavoriteStationRequest.OrderType, location);
            #endregion
            stationList = await favoriteStationsEntity.ToListAsync();
            #region entity response dto ya mapleniyor
            getMobilFavoriteStationResponse.StationList = _mapper.Map<List<MobilFavoriteStationListItemDto>>(stationList, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as List<MobilFavoriteStationListItemDto>;
                    var srcData = src as List<FavoriteStation>;
                    destData.ForEach(item =>
                    {
                        var favoriteStation = srcData.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (favoriteStation != null)
                        {
                            item.Station.Distance = _utilService.ParseInt(favoriteStation.Station.StationAddress.LocationCoordinateCartesian.Distance(location));
                            item.Station.DeviceCount = favoriteStation.Station.ChargeDevice.Where(x => !x.Deleted).Count();
                            if (favoriteStation.Station.IsActive)
                            {
                                item.Station.AvailableDeviceCount = favoriteStation.Station.ChargeDevice.Where(x => !x.Deleted && x.State == ChargeDeviceStateEnum.ACTIVE &&
                                                                                    (x.ChargeDeviceConnector.Any(x => x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE && x.State == ChargeDeviceConnectorStateEnum.ACTIVE)))
                                                                                    .Count();
                            }
                            else
                            {
                                item.Station.AvailableDeviceCount = 0;
                            }
                            item.Station.IsFavorite = true;
                        }
                    });
                });
            });
            #endregion
            return new SuccessResult<GetMobilFavoriteStationResponseDto>(getMobilFavoriteStationResponse);
        }
        /// <summary>
        /// istasyon favoriye ekleniyor
        /// </summary>
        /// <param name="addFavoriteStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddFavoriteStationResponseDto>> AddFavoriteStation(AddFavoriteStationRequestDto addFavoriteStationRequest)
        {
            #region response dto oluşturuluyor
            AddFavoriteStationResponseDto addFavoriteStationResponse = new AddFavoriteStationResponseDto();
            #endregion
            #region favorilerde mevcut mu kontrol ediliyor
            #region filter dto oluşturuluyor
            FavoriteStationFilterDto favoriteStationFilter = _mapper.Map<FavoriteStationFilterDto>(addFavoriteStationRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FavoriteStationFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            var checkFavorite = await _favoriteStationRepository.GetFavoriteStationAsNoTracking(favoriteStationFilter).AnyAsync();
            #endregion
            if (!checkFavorite)
            {
                #region istasyon getiriliyor
                #region filter dto oluşturuluyor
                StationFilterDto stationFilter = _mapper.Map<StationFilterDto>(addFavoriteStationRequest);
                #endregion
                //var station = await _stationRepository.GetStationWithIncludesAsNoTracking(stationFilter).FirstOrDefaultAsync();
                var station = await _stationRepository.GetStation(stationFilter,null).FirstOrDefaultAsync();
                #endregion
                if (station != null)
                {
                    #region istasyon favorilere ekleniyor
                    FavoriteStation favoriteStation = new FavoriteStation();
                    favoriteStation.StationId = station.Id;
                    favoriteStation.UserId = _userContextProvider.UserId.GetValueOrDefault();
                    await _favoriteStationRepository.InsertAsync(favoriteStation);
                    await _favoriteStationRepository.SaveChangesAsync();
                    #endregion
                }
                else
                {
                    return new ErrorResult<AddFavoriteStationResponseDto>(addFavoriteStationResponse, StationProcessErrorEnum.STATION_NOT_FOUND_ERROR);
                }
            }
            return new SuccessResult<AddFavoriteStationResponseDto>(addFavoriteStationResponse);
        }
        /// <summary>
        /// istasyon favorilerden kaldırılıyor
        /// </summary>
        /// <param name="removeFavoriteStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<RemoveFavoriteStationResponseDto>> RemoveFavoriteStation(RemoveFavoriteStationRequestDto removeFavoriteStationRequest)
        {
            #region response dto oluşturuluyor
            RemoveFavoriteStationResponseDto removeFavoriteStationResponse = new RemoveFavoriteStationResponseDto();
            #endregion
            #region favori istasyon getiriliyor
            #region filter dto oluşturuluyor
            FavoriteStationFilterDto favoriteStationFilter = _mapper.Map<FavoriteStationFilterDto>(removeFavoriteStationRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FavoriteStationFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            #endregion
            var favoriteStation = await _favoriteStationRepository.GetFavoriteStation(favoriteStationFilter).ToListAsync();
            #endregion
            if (favoriteStation != null)
            {
                #region istasyon favorilerden kaldırılıyor
                _favoriteStationRepository.UpdateWithProperties(favoriteStation.ToArray(), new Expression<Func<FavoriteStation, object>>[] {
                    s => s.Deleted
                });
                favoriteStation.ForEach(favoriteStation =>
                {
                    favoriteStation.Deleted = true;
                });
                #endregion
                await _favoriteStationRepository.SaveChangesAsync();
            }
            return new SuccessResult<RemoveFavoriteStationResponseDto>(removeFavoriteStationResponse);
        }
        /// <summary>  
        /// istasyon fiyatları getiriliyor
        /// </summary>
        /// <param name="getMobilStationPricesRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetMobilStationPricesResponseDto>> GetStationPrices(GetMobilStationPricesRequestDto getMobilStationPricesRequest)
        {
            #region response dto oluşturuluyor
            GetMobilStationPricesResponseDto getMobilStationPricesResponse = new GetMobilStationPricesResponseDto();
            getMobilStationPricesResponse.StationList = new List<MobilStationPriceItemDto>();
            #endregion
            #region istasyonlar çekiliyor
            #region filter dto oluşturuluyor
            StationContentFilterDto stationFilter = _mapper.Map<StationContentFilterDto>(getMobilStationPricesRequest, opt =>
            {
              
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationContentFilterDto;
                    destData.IsTestStation = false;
                });
            });
            #endregion
            var stationsEntity = _stationContentRepository.GetStationContentForPrices(stationFilter);
            #endregion
            #region istasyon entity response dto suna mapleniyor
            var stationList = new List<StationContent>();
            if (getMobilStationPricesRequest.Ofset != null && getMobilStationPricesRequest.Count != null)
            {
                var skip = getMobilStationPricesRequest.Ofset.GetValueOrDefault() * getMobilStationPricesRequest.Count.GetValueOrDefault();
                stationsEntity =  stationsEntity.Skip(skip).Take(getMobilStationPricesRequest.Count.GetValueOrDefault());
            }
            stationList = await stationsEntity.OrderByDescending(x => x.Id).ToListAsync();
            getMobilStationPricesResponse.StationList = _mapper.Map<List<MobilStationPriceItemDto>>(stationList);
            #endregion
            #region istasyon sayısı setleniyor
            getMobilStationPricesResponse.TotalRecord = await stationsEntity.CountAsync();
            #endregion
            return new SuccessResult<GetMobilStationPricesResponseDto>(getMobilStationPricesResponse);
        }
        #region private methods
        private IQueryable<MobilStationListItemDto> GetStationListLinqQuery(StationContentFilterDto stationContentFilter)
        {
            Point location = new Point(0, 0);
            if (stationContentFilter.Latitude != null && stationContentFilter.Longtitude != null)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                location = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(_utilService.ParseDouble(stationContentFilter.Longtitude), _utilService.ParseDouble(stationContentFilter.Latitude)));
            }
            else
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                location = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(0, 0));
            }
            var stationContentResult = _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationContentFilter);
            var stationContentQuery = (from stationContent in stationContentResult
                                select new MobilStationListItemDto
                                {
                                    Id = stationContent.Station.Id,
                                    Name = stationContent.Name,
                                    Distance = _utilService.ParseInt(stationContent.Station.StationAddress.LocationCoordinateCartesian.Distance(location)),
                                    DeviceCount = stationContent.Station.ChargeDevice.AsQueryable().Where(x => !x.Deleted).Count(),
                                    AvailableDeviceCount = stationContent.Station.IsActive == true ? stationContent.Station.ChargeDevice.AsQueryable().Where(x => !x.Deleted && x.State == ChargeDeviceStateEnum.ACTIVE &&
                                                                                (x.ChargeDeviceConnector.AsQueryable().Any(x => x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE && x.State == ChargeDeviceConnectorStateEnum.ACTIVE)))
                                                                                .Count() : 0,
                                    IsActive = stationContent.Station.IsActive,
                                    IsFavorite = _userContextProvider.SessionType == MobilSessionTypeEnum.USER ? 
                                    stationContent.Station.FavoriteStation.AsQueryable().Where(x => x.UserId == _userContextProvider.UserId && !x.Deleted).Any() :
                                    false,
                                    HasAcPowerType = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.AC)).Any(),
                                    HasDcPowerType = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.DC)).Any(),
                                    AcPowerTypeCount = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.AC)).Count(),
                                    DcPowerTypeCount = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.DC)).Count(),
                                    AvailableAcPowerTypeCount = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ConnectionState && x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.AC && x.State == ChargeDeviceConnectorStateEnum.ACTIVE && x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE)).Count(),
                                    AvailableDcPowerTypeCount = stationContent.Station.ChargeDevice.AsQueryable().Where(x => x.ConnectionState &&  x.ChargeDeviceConnector.AsQueryable().Any(x => x.ChargeDevicePowerTypeId == (int)ChargeDevicePowerTypeEnum.DC && x.State == ChargeDeviceConnectorStateEnum.ACTIVE && x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE)).Count(),
                                    StationRatingCount = stationContent.Station.StationRating.Count(),
                                    StationRatingPoint = stationContent.Station.StationRating.Count > 0 ? (stationContent.Station.StationRating.Sum(x => x.Point) / stationContent.Station.StationRating.Count) : 0,
                                });
            return stationContentQuery;
        }
        #endregion
    }
}
