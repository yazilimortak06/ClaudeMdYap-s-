// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Applications\Api.Application\Services\MobilApiServices\StationProcess\StationProcessService.cs
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
            var connectorForKw = _chargeDeviceConnectorRepository.GetChargeDeviceConnector(new ChargeDeviceConnectorFilterDto() { },null);
            mobilStationFilterPrepareResponse.MaxKw = await connectorForKw.OrderByDescending(x => x.kW).Select(x => x.kW).FirstOrDefaultAsync();
            mobilStationFilterPrepareResponse.MinKw = await connectorForKw.OrderBy(x => x.kW).Select(x => x.kW).FirstOrDefaultAsync();
            #endregion
            if (mobilStationFilterPrepareRequest.Latitude != null && mobilStationFilterPrepareRequest.Longtitude != null)
            {
                #region max ve min mesafe setleniyor
                Point location = _utilService.GetPointOfLocationCoordinateCartesian(mobilStationFilterPrepareRequest.Latitude, mobilStationFilterPrepareRequest.Longtitude);
                var maxDistance = await _stationRepository.GetStation(new StationFilterDto() { }, include: source => source
     .Include(a => a.StationAddress)).OrderByDescending(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                var minDistance = await _stationRepository.GetStation(new StationFilterDto() { }, include: source => source
     .Include(a => a.StationAddress)).OrderBy(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).Select(x => x.StationAddress.LocationCoordinateCartesian.Distance(location)).FirstOrDefaultAsync();
                mobilStationFilterPrepareResponse.MaxDistance = (int)Math.Ceiling(maxDistance);
                mobilStationFilterPrepareResponse.MinDistance = (int)Math.Floor(minDistance);
                #endregion
            }
            #region max ve min fiyat setleniyor
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
        public async Task<Result<GetMobilStationListResponseDto>> GetStationList(GetMobilStationListRequestDto getMobilStationListRequest)
        {
            GetMobilStationListResponseDto getMobilStationListResponse = new GetMobilStationListResponseDto();
            getMobilStationListResponse.Stations = new List<MobilStationListItemDto>();
            getMobilStationListResponse.TotalRecord = 0;
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
            Point location = _utilService.GetPointOfLocationCoordinateCartesian(stationContentFilter.Latitude, stationContentFilter.Longtitude);
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
            var stationList = new List<StationContent>();
            if (getMobilStationListRequest.Ofset != null && getMobilStationListRequest.Count != null)
            {
                var skip = getMobilStationListRequest.Ofset.GetValueOrDefault() * getMobilStationListRequest.Count.GetValueOrDefault();
                stations = stations.Skip(skip).Take(getMobilStationListRequest.Count.GetValueOrDefault());
            }
            stations = stations.OrderStationContent(getMobilStationListRequest.IsAscending, getMobilStationListRequest.OrderType, location);
            stationList = await stations.ToListAsync();
            getMobilStationListResponse.TotalRecord = await stations.CountAsync();
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
            getMobilStationListResponse.Stations = stationManagmentListDto;
            return new SuccessResult<GetMobilStationListResponseDto>(getMobilStationListResponse);
        }

        /// <summary>
        /// mobil için istasyon filtreleme esnasında istasyon sayısı getiriliyor
        /// </summary>
        public async Task<Result<GetStationFilterCountResponseDto>> GetStationFilterCount(GetStationFilterCountRequestDto getStationFilterCountRequest)
        {
            GetStationFilterCountResponseDto getStationFilterCountResponse = new GetStationFilterCountResponseDto();
            getStationFilterCountResponse.StationCount = 0;
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
            getStationFilterCountResponse.StationCount = await _stationContentRepository.GetStationContent(stationContentFilter,null).CountAsync();
            return new SuccessResult<GetStationFilterCountResponseDto>(getStationFilterCountResponse);
        }

        /// <summary>
        /// istasyon favoriye ekleniyor
        /// </summary>
        public async Task<Result<AddFavoriteStationResponseDto>> AddFavoriteStation(AddFavoriteStationRequestDto addFavoriteStationRequest)
        {
            AddFavoriteStationResponseDto addFavoriteStationResponse = new AddFavoriteStationResponseDto();
            FavoriteStationFilterDto favoriteStationFilter = _mapper.Map<FavoriteStationFilterDto>(addFavoriteStationRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FavoriteStationFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            var checkFavorite = await _favoriteStationRepository.GetFavoriteStationAsNoTracking(favoriteStationFilter).AnyAsync();
            if (!checkFavorite)
            {
                StationFilterDto stationFilter = _mapper.Map<StationFilterDto>(addFavoriteStationRequest);
                var station = await _stationRepository.GetStation(stationFilter,null).FirstOrDefaultAsync();
                if (station != null)
                {
                    FavoriteStation favoriteStation = new FavoriteStation();
                    favoriteStation.StationId = station.Id;
                    favoriteStation.UserId = _userContextProvider.UserId.GetValueOrDefault();
                    await _favoriteStationRepository.InsertAsync(favoriteStation);
                    await _favoriteStationRepository.SaveChangesAsync();
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
        public async Task<Result<RemoveFavoriteStationResponseDto>> RemoveFavoriteStation(RemoveFavoriteStationRequestDto removeFavoriteStationRequest)
        {
            RemoveFavoriteStationResponseDto removeFavoriteStationResponse = new RemoveFavoriteStationResponseDto();
            FavoriteStationFilterDto favoriteStationFilter = _mapper.Map<FavoriteStationFilterDto>(removeFavoriteStationRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FavoriteStationFilterDto;
                    destData.UserId = _userContextProvider.UserId;
                });
            });
            var favoriteStation = await _favoriteStationRepository.GetFavoriteStation(favoriteStationFilter).ToListAsync();
            if (favoriteStation != null)
            {
                _favoriteStationRepository.UpdateWithProperties(favoriteStation.ToArray(), new Expression<Func<FavoriteStation, object>>[] {
                    s => s.Deleted
                });
                favoriteStation.ForEach(favoriteStation =>
                {
                    favoriteStation.Deleted = true;
                });
                await _favoriteStationRepository.SaveChangesAsync();
            }
            return new SuccessResult<RemoveFavoriteStationResponseDto>(removeFavoriteStationResponse);
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
                                });
            return stationContentQuery;
        }
        #endregion
    }
}
