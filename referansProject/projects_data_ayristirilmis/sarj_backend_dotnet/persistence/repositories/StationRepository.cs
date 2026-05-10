// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Persistences\Api.Persistence\Repositories\StationRepositories\StationRepository.cs
using Api.Persistence.DbContext;
using FrameworkCore.Bases.BaseRepository;
using FrameworkCore.FrameworkCore.UnitOfWorkCore;
using FrameworkCore.Utils.EntityUtils;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Shared.Domain.Dto.ApiDto.StationContentDtos;
using Shared.Domain.Dto.ApiDto.StationDtos;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationRepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Persistence.Repositories.StationRepositories
{
    public class StationRepository : ConnectedRepository<Station>, IStationRepository
    {
        private RotaWattDbContext _appDbContext { get => _dbContext as RotaWattDbContext; }

        private readonly IUtilService _utilService;
        public StationRepository(IUnitOfWork dbContext,
            IUtilService utilService) : base(dbContext)
        {
            _utilService = utilService;
        }
        /// <summary>
        /// istasyonlar çekiliyor
        /// </summary>
        public IQueryable<Station> GetStation(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            var stationQuery = _appDbContext.Station.Where(predicateStation);
            return stationQuery;
        }
        /// <summary>
        /// istasyonlar ve bağlı tablolar çekiliyor
        /// </summary>
        public IQueryable<Station> GetStationWithIncludes(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            Expression<Func<StationFacility, bool>> predicateStationFacility = p => !p.Deleted;
            var stationQuery = _appDbContext.Station.Include(x => x.StationInfo).ThenInclude(x=>x.StationTypesOfUsing.Where(x => !x.Deleted))
                                                                      .Include(x => x.StationInfo).ThenInclude(x => x.StationOpportunities.Where(x => !x.Deleted))
                                                                      .Include(x => x.StationAddress)
                                                                      .Include(x => x.StationFacility.AsQueryable().Where(predicateStationFacility))
                                                                      .Include(x => x.StationPicture.Where(x => !x.Deleted))
                                                                      .Include(x => x.StationPaymentMethod.Where(x => !x.Deleted))
                                                                      .Include(x => x.ReservationSuitableDate)
                                                                      .Where(predicateStation).AsSplitQuery();
            return stationQuery;
        }
        public IQueryable<Station> GetStation(StationFilterDto stationFilter,
         Func<IQueryable<Station>, IIncludableQueryable<Station, object>> include,
         bool disableTracking = true)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            IQueryable<Station> query = _appDbContext.Station;
            if (include != null)
            {
                query = include(query);
            }
            query = query.AsSplitQuery();
            query = query.Where(predicateStation);
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }
        public IQueryable<Station> GetStationWithConnectors(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            var stationQuery = _appDbContext.Station.Include(x => x.ChargeDevice)
                                                                      .Include(x => x.ChargeDevice).ThenInclude(x => x.ChargeDeviceConnector)
                                                                      .Where(predicateStation).AsSplitQuery();
            return stationQuery;
        }
        public IQueryable<Station> GetStationWithDetailAddress(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            var stationQuery = _appDbContext.Station.Include(x => x.StationInfo)
                                                                      .Include(x => x.StationAddress)
                                                                      .Include(x => x.ReservationSuitableDate)
                                                                      .Include(x => x.StationLocationArea)
                                                                      .Where(predicateStation);
            return stationQuery;
        }
        public IQueryable<Station> GetStationWithDetailAddressAsNoTracking(StationFilterDto stationFilter)
        {
            return GetStationWithDetailAddress(stationFilter).AsNoTracking();
        }
        public IQueryable<Station> GetStationAsNoTracking(StationFilterDto stationFilter)
        {
            return GetStation(stationFilter).AsNoTracking();
        }
        public IQueryable<Station> GetStationWithFirmAndAddress(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = PredicateStation(stationFilter);
            var stationQuery = _appDbContext.Station.Include(x => x.Firm).ThenInclude(x=>x.FirmLogo)
                                                                      .Include(x => x.StationAddress)
                                                                      .Where(predicateStation);
            return stationQuery;
        }
        public IQueryable<Station> GetStationWithFirmAndAddressAsNoTracking(StationFilterDto stationFilter)
        {
            return GetStationWithFirmAndAddress(stationFilter).AsNoTracking();
        }
        public IQueryable<Station> GetStationWithIncludesAsNoTracking(StationFilterDto stationFilter)
        {
            return GetStationWithIncludes(stationFilter).AsNoTracking();
        }
        private Expression<Func<Station, bool>> PredicateStation(StationFilterDto stationFilter)
        {
            Expression<Func<Station, bool>> predicateStation = p => !p.Deleted;
            Point location = _utilService.GetPointOfLocationCoordinateCartesian(stationFilter.Latitude, stationFilter.Longtitude);
            if (stationFilter.Id != null)
                predicateStation = predicateStation.And(p => p.Id == stationFilter.Id);
            if (stationFilter.CompanyId != null)
                predicateStation = predicateStation.And(p => p.CompanyId == stationFilter.CompanyId);
            if (stationFilter.FirmId != null)
                predicateStation = predicateStation.And(p => p.FirmId == stationFilter.FirmId);
            if (stationFilter.IsIntegrated != null)
                predicateStation = predicateStation.And(p => p.IsIntegrated == stationFilter.IsIntegrated);
            if (stationFilter.StationOpportunity != null)
                predicateStation = predicateStation.And(p => p.StationInfo.StationOpportunities.Any(x => stationFilter.StationOpportunity.Contains(x.OpportunityId)));
            if (stationFilter.StationRatingPoint != null)
                predicateStation = predicateStation.And(p => (p.StationRating.Sum(x => x.Point) / p.StationRating.Count) >= stationFilter.StationRatingPoint);
            if (stationFilter.FirmIdList != null)
                predicateStation = predicateStation.And(p => stationFilter.FirmIdList.Contains(p.FirmId.GetValueOrDefault()));
            if (stationFilter.MinPrice != null)
                predicateStation = predicateStation.And(pv => pv.ChargeDevice.Any(x => x.ChargeDeviceConnector.Any(x => x.PriceWithKdv >= stationFilter.MinPrice)));
            if (stationFilter.MaxPrice != null)
                predicateStation = predicateStation.And(pv => pv.ChargeDevice.Any(x => x.ChargeDeviceConnector.Any(x => x.PriceWithKdv <= stationFilter.MaxPrice)));
            if (stationFilter.MinKw != null)
                predicateStation = predicateStation.And(p => p.ChargeDevice.Any(x => x.ChargeDeviceConnector.Any(x => x.kW >= stationFilter.MinKw)));
            if (stationFilter.MaxKw != null)
                predicateStation = predicateStation.And(p => p.ChargeDevice.Any(x => x.ChargeDeviceConnector.Any(x => x.kW <= stationFilter.MaxKw)));
            if (stationFilter.MaxDistance != null && stationFilter.Latitude != null && stationFilter.Longtitude != null)
                predicateStation = predicateStation.And(pv => pv.StationAddress.Location.Distance(location) <= stationFilter.MaxDistance);
            if (stationFilter.MinDistance != null && stationFilter.Latitude != null && stationFilter.Longtitude != null)
                predicateStation = predicateStation.And(pv => pv.StationAddress.Location.Distance(location) >= stationFilter.MinDistance);
            if (stationFilter.IsAvailable != null)
            {
                if (stationFilter.IsAvailable.GetValueOrDefault())
                {
                    predicateStation = predicateStation.And(pc => pc.ChargeDevice.Any(x => x.State == ChargeDeviceStateEnum.ACTIVE &&
                                                                                            x.ConnectionState && x.InstantState == ChargeDeviceInstantStateEnum.AVAILABLE &&
                                                                                            x.ChargeDeviceConnector.Any(x => x.State == ChargeDeviceConnectorStateEnum.ACTIVE && x.InstantState == ChargeDeviceConnectorInstantStateEnum.AVAILABLE)));
                }
            }
            return predicateStation;
        }
    }
}
