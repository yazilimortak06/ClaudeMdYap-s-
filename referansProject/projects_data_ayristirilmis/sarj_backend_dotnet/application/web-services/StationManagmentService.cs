using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using GeoAPI.Geometries;
using Geolocation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.LocationAreaDtos;
using Shared.Domain.Dto.ApiDto.OpportunityDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PaymentMethodDtos;
using Shared.Domain.Dto.ApiDto.StationContentDtos;
using Shared.Domain.Dto.ApiDto.StationDtos;
using Shared.Domain.Dto.ApiDto.StationFacilityDtos;
using Shared.Domain.Dto.ApiDto.StationOpportunityDtos;
using Shared.Domain.Dto.ApiDto.StationTypeOfUsingDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceModule;
using Shared.Domain.Entities.ApiEntities.FacilityModule;
using Shared.Domain.Entities.ApiEntities.LocationAreaModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.TockenEnums;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.FileApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceConnectorRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeDeviceRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ContentLanguageRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.LocationAreaRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationAddressRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationContentRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationDetailRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationPaymentMethodRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationPictureRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.StationManagmentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GeoAPI;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Xml.Linq;
using NetTopologySuite.Operation.Distance;
using ProjNet.CoordinateSystems;
using ProjNet;
using ProjNet.CoordinateSystems.Transformations;
using NetTopologySuite.Index.HPRtree;
using GeoAPI.CoordinateSystems.Transformations;
using Shared.Domain.HttpClients.HttpClientInterfaces.FirmIntegrationApiInterfaces;

namespace Api.Application.Services.PanelServices.StationManagment
{
    public class StationManagmentService : BaseService, IStationManagmentService
    {
        private readonly IStationRepository _stationRepository;
        private readonly IStationContentRepository _stationContentRepository;
        private readonly IStationAddressRepository _stationAddressRepository;
        private readonly IStationInfoRepository _stationDetailRepository;
        private readonly ILocationAreaConfigsRepository _locationAreaConfigRepository;
        private readonly ILocationAreaRepository _locationAreaRepository;
        private readonly IStationLocationAreaRepository _stationLocationAreaRepository;
        private readonly IContentLanguageRepository _contentLanguageContentRepository;
        private readonly IStationPictureRepository _stationPictureRepository;
        private readonly IChargeDeviceRepository _chargeDeviceRepository;
        private readonly IChargeDeviceConnectorRepository _chargeDeviceConnectorRepository;
        private readonly IStationPaymentMethodRepository _stationPaymentMethodRepository;
        private readonly IStationTypesOfUsingRepository _stationTypesOfUsingRepository;
        private readonly IStationOpportunitiesRepository _stationOpportunitiesRepository;
        private readonly IStationFacilityRepository _stationFacilityRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IFacilityRepository _facilityRepository;
        private readonly ITypesOfUsingRepository _typesOfUsingRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IFileClientService _fileClientService;
        private readonly IFirmSettingManagementClientService _firmSettingManagementClientService;
        private readonly IConfiguration _configuration;
        private readonly IAdminContextProvider _adminContextProvider;
        private readonly IUtilService _utilService;

        public StationManagmentService(IMapper mapper,
                           IStationRepository stationRepository,
                           IContentLanguageRepository contentLanguageContentRepository,
                           IStationAddressRepository stationAddressRepository,
                           IStationInfoRepository stationDetailRepository,
                           IStationPaymentMethodRepository stationPaymentMethodRepository,
                           IPaymentMethodRepository paymentMethodRepository,
                           IStationPictureRepository stationPictureRepository,
                           IConfiguration configuration,
                           IStationContentRepository stationContentRepository,
                           IUtilService utilService,
                           IStationTypesOfUsingRepository stationTypesOfUsingRepository,
                           ITypesOfUsingRepository typesOfUsingRepository,
                           IAdminContextProvider adminContextProvider,
                           IFileClientService fileClientService,
                           IStationOpportunitiesRepository stationOpportunitiesRepository,
                           IOpportunityRepository opportunityRepository,
                           IChargeDeviceRepository chargeDeviceRepository,
                           IChargeDeviceConnectorRepository chargeDeviceConnectorRepository,
                           ILocationAreaRepository locationAreaRepository,
                           IStationLocationAreaRepository stationLocationAreaRepository,
                           ILocationAreaConfigsRepository locationAreaConfigRepository,
                           IFacilityRepository facilityRepository,
                           IFirmSettingManagementClientService firmSettingManagementClientService
,
                           IStationFacilityRepository stationFacilityRepository) : base(
                           mapper
                               )
        {
            _stationRepository = stationRepository;
            _stationContentRepository = stationContentRepository;
            _contentLanguageContentRepository = contentLanguageContentRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _stationAddressRepository = stationAddressRepository;
            _stationPictureRepository = stationPictureRepository;
            _stationDetailRepository = stationDetailRepository;
            _stationPaymentMethodRepository = stationPaymentMethodRepository;
            _configuration = configuration;
            _utilService = utilService;
            _stationTypesOfUsingRepository = stationTypesOfUsingRepository;
            _typesOfUsingRepository = typesOfUsingRepository;
            _adminContextProvider = adminContextProvider;
            _fileClientService = fileClientService;
            _stationOpportunitiesRepository = stationOpportunitiesRepository;
            _opportunityRepository = opportunityRepository;
            _chargeDeviceRepository = chargeDeviceRepository;
            _chargeDeviceConnectorRepository = chargeDeviceConnectorRepository;
            _locationAreaRepository = locationAreaRepository;
            _stationLocationAreaRepository = stationLocationAreaRepository;
            _locationAreaConfigRepository = locationAreaConfigRepository;
            _facilityRepository = facilityRepository;
            _firmSettingManagementClientService = firmSettingManagementClientService;
            _stationFacilityRepository = stationFacilityRepository;
        }
        #region ekleme,güncelleme,silme
        /// <summary>
        /// istasyon ekleniyor
        /// </summary>
        /// <param name="addPanelStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddPanelStationResponseDto>> AddStation(AddPanelStationRequestDto addPanelStationRequest)
        {
            #region response dto oluşturuluyor
            AddPanelStationResponseDto addPanelStationResponse = new AddPanelStationResponseDto();
            #endregion
            #region google service apiye istek atılarak station location ekleniyor
            //#region request dto oluşturuluyor
            //AddStationLocationRequestDto addStationLocationRequest = _mapper.Map<AddStationLocationRequestDto>(stationManagmentInsert.StationManagmentAddress);
            //#endregion
            //var addStationLocationResponse = await _stationLocationClientService.AddStationLocation(addStationLocationRequest);
            #endregion
            var stationEntity = _mapper.Map<Station>(addPanelStationRequest, opt =>
            {
                opt.BeforeMap((src, dest) =>
                {
                    var srcData = src as AddPanelStationRequestDto;
                    srcData.StationAddress.Latitude = srcData.StationAddress.Latitude.Replace(",", ".");
                    srcData.StationAddress.Longtitude = srcData.StationAddress.Longtitude.Replace(",", ".");
                });
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Station;
                    //destData.StationLocationGuiId = addStationLocationResponse.Data.StationLocationGuiId;
                    destData.CreatedDate = DateTime.Now;
                    destData.LastUpdateDate = DateTime.Now;
                    destData.StationFirmType = StationFirmTypeEnum.ROTAWATT;
                    #region şirket admini ise companyId admine göre setleniyor
                    if (_adminContextProvider.AdminManagmentType == AdminManagmentTypeEnum.COMPANY_ADMIN)
                    {
                        destData.CompanyId = _adminContextProvider.CompanyId;
                    }
                    #endregion
                    #region firma admini ise firmId admine göre setleniyor
                    else if (_adminContextProvider.AdminManagmentType == AdminManagmentTypeEnum.FIRM_ADMIN)
                    {
                        destData.FirmId = _adminContextProvider.FirmId;
                    }
                    #endregion
                    destData.StationAddress.Location = _utilService.GetPointOfLocation(addPanelStationRequest.StationAddress.Latitude, addPanelStationRequest.StationAddress.Longtitude);
                    destData.StationAddress.LocationCoordinateCartesian = _utilService.GetPointOfLocationCoordinateCartesian(addPanelStationRequest.StationAddress.Latitude, addPanelStationRequest.StationAddress.Longtitude);
                });
            });
            #region station location area verisi oluşturuluyor
            await CreateStationLocationArea(addPanelStationRequest, stationEntity);
            #endregion
            #region veritabanına kayıt ediliyor
            await _stationRepository.InsertAsync(stationEntity);
            await _stationRepository.SaveChangesAsync();
            #endregion
            return new SuccessResult<AddPanelStationResponseDto>(addPanelStationResponse);
        }
        /// <summary>
        /// istasyon güncelleniyor
        /// </summary>
        /// <param name="updatePanelStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<UpdatePanelStationResponseDto>> UpdateStation(UpdatePanelStationRequestDto updatePanelStationRequest)
        {
            #region response dto oluşturuluyor
            UpdatePanelStationResponseDto updatePanelStationResponse = new UpdatePanelStationResponseDto();
            #endregion
            #region istasyon kontrolü sağlanıyor
            #region filter dto oluşturuluyor
            StationFilterDto stationFilter = new StationFilterDto();
            stationFilter.CompanyId = _adminContextProvider.CompanyId;
            stationFilter.IsIntegrated = false;
            stationFilter.FirmId = _adminContextProvider.FirmId;
            stationFilter.Id = updatePanelStationRequest.Station.Id;
            #endregion
            var station = await _stationRepository.GetStationWithDetailAddress(stationFilter).FirstOrDefaultAsync();
            #endregion
            if (station != null)
            {
                #region istasyon güncelleniyor
                #region eğer şirket admini ise veya firma admini ise companyIdi firmId güncellenmiyor
                if (_adminContextProvider.AdminManagmentType == AdminManagmentTypeEnum.COMPANY_ADMIN ||
                    _adminContextProvider.AdminManagmentType == AdminManagmentTypeEnum.FIRM_ADMIN)
                {
                    if (_adminContextProvider.AdminManagmentType == AdminManagmentTypeEnum.FIRM_ADMIN)
                    {
                        _stationRepository.UpdateWithProperties(station, new Expression<Func<Station, object>>[] {
                            s=>s.LastUpdateDate,
                            s=>s.CompanyId,
                        });
                    }
                    else
                    {
                        _stationRepository.UpdateWithProperties(station, new Expression<Func<Station, object>>[] {
                            s=>s.LastUpdateDate,
                        });
                    }
                }
                #endregion
                else
                {
                    _stationRepository.UpdateWithProperties(station, new Expression<Func<Station, object>>[] {
                        s=>s.CompanyId,
                        s=>s.FirmId,
                        s=>s.LastUpdateDate,
                        s=>s.IsTestStation,
                    });
                }
                #endregion
                #region istasyon adresi güncelleniyor
                _stationAddressRepository.UpdateWithProperties(station.StationAddress, new Expression<Func<StationAddress, object>>[] {
                    s=>s.Phone,
                    s => s.Neighbourhood,
                    s => s.Street,
                    s => s.Description,
                    s => s.CountryId,
                    s => s.CityId,
                    s => s.TownId,
                    s => s.CountryName,
                    s => s.CityName,
                    s => s.TownName,
                    s => s.Location,
                    s => s.LocationCoordinateCartesian,
                });
                #endregion
                #region istasyon detayı güncelleniyor
                _stationDetailRepository.UpdateWithProperties(station.StationInfo, new Expression<Func<StationInfo, object>>[] {
                    s=>s.StartTime,
                    s=>s.StartTimeTotalMinute,
                    s=>s.EndTime,
                    s=>s.EndTimeTotalMinute,
                    s=>s.PriceOfParkArea,
                    s=>s.CarCountOfParkArea,
                    s=>s.ReservationPrice,
                    s=>s.ReservationTimeInterval,
                    s=>s.ReservationMinuteFlexibility,
                    s=>s.ReservationTempTimeOutMinute,
                    s=>s.AvailableReservationAfterChargeMinute,
                    s=>s.LockingForReservationMinute,
                    s=>s.ChargeProcessTempTimeOutMinute,
                    s=>s.ReservationAdditionalMinuteForCharge,
                });
                #endregion
                #region yeni lokasyon oluşturuluyor
                updatePanelStationRequest.Station.StationAddress.Latitude = updatePanelStationRequest.Station.StationAddress.Latitude.Replace(",", ".");
                updatePanelStationRequest.Station.StationAddress.Longtitude = updatePanelStationRequest.Station.StationAddress.Longtitude.Replace(",", ".");
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                //var newlocation = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(_utilService.ParseDouble(updatePanelStationRequest.Station.StationAddress.Latitude),
                //    _utilService.ParseDouble(updatePanelStationRequest.Station.StationAddress.Longtitude)));
                var newlocation = _utilService.GetPointOfLocation(updatePanelStationRequest.Station.StationAddress.Latitude, updatePanelStationRequest.Station.StationAddress.Longtitude);
                var newlocationCartesian = _utilService.GetPointOfLocationCoordinateCartesian(updatePanelStationRequest.Station.StationAddress.Latitude,
                    updatePanelStationRequest.Station.StationAddress.Longtitude);
                #endregion
                #region eğer istasyon enlem ve boylamı değiştirildiyse, mevcut station location verisi kaldırılıyor ve yeni station location ekleniyor

                if (station.StationAddress.Location.X.ToString() != newlocation.X.ToString() ||
              station.StationAddress.Location.Y.ToString() != newlocation.Y.ToString())
                {
                    #region mevcut station location verisi kaldırılıyor
                    _stationLocationAreaRepository.UpdateWithProperties(station.StationLocationArea.ToArray(), new Expression<Func<StationLocationArea, object>>[] {
                        s => s.Deleted,
                    });
                    station.StationLocationArea.ToList().ForEach(x =>
                    {
                        x.Deleted = true;
                    });
                    #endregion
                    await UpdateStationLocationArea(updatePanelStationRequest, station);
                }
                #endregion
                #region map işlemleri gerçekleşiyor
                #region istasyon mapleniyor
                station = _mapper.Map(updatePanelStationRequest.Station, station, opt =>
                {

                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as Station;
                        var srcData = src as PanelStationUpdateDto;
                        destData.StationAddress.Location = newlocation;
                        destData.StationAddress.LocationCoordinateCartesian = newlocationCartesian;
                        destData.LastUpdateDate = DateTime.Now;
                    });
                });
                #endregion
                #region eklenen ve silinen istasyon resimleri mapleniyor
                var stationPictureEntityInsert = _mapper.Map<List<StationPicture>>(updatePanelStationRequest.StationPicturesInserted);
                var stationPictureEntityDelete = _mapper.Map<List<StationPicture>>(updatePanelStationRequest.StationPicturesDeleted);
                #endregion
                #region eklenen ve silinen ödeme yöntemleri mapleniyor
                var stationPaymentMethodEntityInsert = _mapper.Map<List<StationPaymentMethod>>(updatePanelStationRequest.StationPaymentMethodInsert);
                var stationPaymentMethodEntityDelete = _mapper.Map<List<StationPaymentMethod>>(updatePanelStationRequest.StationPaymentMethodDeleted);
                #endregion
                #region eklenen ve silinen kullanım tipleri mapleniyor
                var stationTypesOfUsingEntityInsert = _mapper.Map<List<StationTypesOfUsing>>(updatePanelStationRequest.StationTypesOfUsingInsert);
                var stationTypesOfUsingEntityDelete = _mapper.Map<List<StationTypesOfUsing>>(updatePanelStationRequest.StationTypesOfUsingDeleted);
                #endregion
                #region eklenen ve silinen olanaklar mapleniyor
                var stationOpportunitiesEntityInsert = _mapper.Map<List<StationOpportunities>>(updatePanelStationRequest.StationOpportunityInsert);
                var stationOpportunitiesEntityDelete = _mapper.Map<List<StationOpportunities>>(updatePanelStationRequest.StationOpportunityDeleted);
                #endregion
                #region eklenen ve silinen hizmetler mapleniyor
                var stationFacilitiesEntityInsert = _mapper.Map<List<StationFacility>>(updatePanelStationRequest.StationFacilityInsert);
                var stationFacilitiesEntityDelete = _mapper.Map<List<StationFacility>>(updatePanelStationRequest.StationFacilityDeleted);
                #endregion
                #region güncellenen ve eklenen istasyon contentleri mapleniyor
                var stationContentEntityUpdate = _mapper.Map<List<StationContent>>(updatePanelStationRequest.StationContent.Where(x => x.Id != 0).ToList());
                var stationContentEntityInsert = _mapper.Map<List<StationContent>>(updatePanelStationRequest.StationContent.Where(x => x.Id == 0).ToList());
                #endregion
                #endregion
                #region güncelleme işlemleri gerçekleşiyor
                #region istasyon contenti güncelleniyor
                _stationContentRepository.UpdateWithProperties(stationContentEntityUpdate.ToArray(), new Expression<Func<StationContent, object>>[] {
                    s => s.Name,
                    s => s.Description
                });
                #endregion
                #endregion
                #region veri tabanına kayıt ediliyor
                _stationPictureRepository.Insert(stationPictureEntityInsert);
                _stationTypesOfUsingRepository.Insert(stationTypesOfUsingEntityInsert);
                _stationOpportunitiesRepository.Insert(stationOpportunitiesEntityInsert);
                _stationPictureRepository.DeleteWithStateRange(stationPictureEntityDelete);
                _stationPaymentMethodRepository.Insert(stationPaymentMethodEntityInsert);
                _stationPaymentMethodRepository.DeleteWithStateRange(stationPaymentMethodEntityDelete);
                _stationTypesOfUsingRepository.DeleteWithStateRange(stationTypesOfUsingEntityDelete);
                _stationOpportunitiesRepository.DeleteWithStateRange(stationOpportunitiesEntityDelete);
                _stationFacilityRepository.Insert(stationFacilitiesEntityInsert);
                _stationFacilityRepository.DeleteWithStateRange(stationFacilitiesEntityDelete);
                _stationContentRepository.Insert(stationContentEntityInsert);
                await _stationRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<UpdatePanelStationResponseDto>(updatePanelStationResponse);
            }
            else
            {
                return new ErrorResult<UpdatePanelStationResponseDto>(updatePanelStationResponse, StationManagmentErrorEnum.STATION_NOT_FOUND_ERROR);
            }
        }
        #endregion
        /// <summary>
        /// İstasyon Güncelleme Formu İçin İstasyon Çekilir
        /// </summary>
        /// <param name="getPanelStationForUpdateRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelStationForUpdateResponseDto>> GetStationForUpdate(GetPanelStationForUpdateRequestDto getPanelStationForUpdateRequest)
        {
            #region response dto oluşturuluyor
            GetPanelStationForUpdateResponseDto getPanelStationForUpdateResponse = new GetPanelStationForUpdateResponseDto();
            getPanelStationForUpdateResponse.Station = new StationDto();
            #endregion
            #region istasyon kontrol ediliyor
            #region filter dto oluşturuluyor
            StationFilterDto stationFilter = _mapper.Map<StationFilterDto>(getPanelStationForUpdateRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as StationFilterDto;
                    destData.CompanyId = _adminContextProvider.CompanyId;
                    destData.FirmId = _adminContextProvider.FirmId;
                    destData.IsIntegrated = false;
                });
            });
            #endregion
            //var station = await _stationRepository.GetStationWithIncludesAsNoTracking(stationFilter).FirstOrDefaultAsync();
            var station = await _stationRepository.GetStation(stationFilter, include: source => source
     .Include(a => a.StationContent)
     .Include(a => a.StationAddress)
     .Include(a => a.StationPicture.Where(p => !p.Deleted))
     .Include(a => a.StationPaymentMethod.Where(p => !p.Deleted))
     .Include(a => a.StationInfo)
     .ThenInclude(a => a.StationTypesOfUsing.Where(p => !p.Deleted))
     .Include(a => a.StationInfo)
     .ThenInclude(a => a.StationOpportunities.Where(p => !p.Deleted))
     .Include(a => a.ReservationSuitableDate)
     .Include(a => a.StationFacility.Where(p => !p.Deleted))
     .Include(a => a.ChargeDevice)
     .ThenInclude(a => a.ChargeDeviceMark)
     ).FirstOrDefaultAsync();
            #endregion
            if (station != null)
            {
                #region google service api ye istek atılarak station location getiriliyor
                //#region request dto oluşturuluyor
                //GetStationLocationRequestDto getStationLocationRequest = new GetStationLocationRequestDto();
                //getStationLocationRequest.StationLocationGuiId = station.StationLocationGuiId;
                //#endregion
                //var getStationLocationResponse = await _stationLocationClientService.GetStationLocation(getStationLocationRequest);
                #endregion
                #region istasyon ve detayı,adresi,resimleri, kullanım alanları ve ödeme yöntemleri response dto ya mapleniyor
                getPanelStationForUpdateResponse.Station = _mapper.Map<StationDto>(station, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as StationDto;
                        destData.StationAddress.Latitude = destData.StationAddress.Latitude.Replace(',', '.');
                        destData.StationAddress.Longtitude = destData.StationAddress.Longtitude.Replace(',', '.');
                    });
                });
                #endregion
                #region istasyon content response dto ya mapleniyor
                #region station content getiriliyor
                #region filter dto oluşturuluyor
                StationContentFilterDto stationContentFilter = _mapper.Map<StationContentFilterDto>(getPanelStationForUpdateRequest);
                #endregion
                var stationContentEntity = await _stationContentRepository.GetStationContentForPanelForm(stationContentFilter).ToListAsync();
                #endregion
                getPanelStationForUpdateResponse.StationContent = _mapper.Map<List<StationContentDto>>(stationContentEntity);
                getPanelStationForUpdateResponse.StationContentFormInput = await GetContentLanguageWithStationContent(getPanelStationForUpdateRequest.StationId).ToListAsync();
                #endregion
                #region kullanım alanları, olanaklar, hizmetler ve ödeme tipleri çekiliyor
                var paymentMethods = await _paymentMethodRepository.GetPaymentMethodAsNoTracking().ToListAsync();
                var typesOfUsing = await _typesOfUsingRepository.GetTypesOfUsingAsNoTracking().ToListAsync();
                var opportunities = await _opportunityRepository.GetOpportunitiesAsNoTracking(new OpportunityFilterDto() { }).ToListAsync();
                var facilities = await _facilityRepository.GetFacilityAsNoTracking().ToListAsync();
                #region response dto ya mapleniyor
                getPanelStationForUpdateResponse.PaymentMethod = _mapper.Map<List<PaymentMethodDto>>(paymentMethods);
                getPanelStationForUpdateResponse.TypesOfUsing = _mapper.Map<List<TypesOfUsingDto>>(typesOfUsing);
                getPanelStationForUpdateResponse.Opportunities = _mapper.Map<List<OpportunityDto>>(opportunities);
                getPanelStationForUpdateResponse.Facilities = _mapper.Map<List<FacilityDto>>(facilities);
                #endregion
                #endregion
                return new SuccessResult<GetPanelStationForUpdateResponseDto>(getPanelStationForUpdateResponse);
            }
            else
            {
                return new ErrorResult<GetPanelStationForUpdateResponseDto>(getPanelStationForUpdateResponse, StationManagmentErrorEnum.STATION_NOT_FOUND_ERROR);
            }

        }
        /// <summary>
        /// istasyon Ekleme Formu İçin Bilgiler Çekilir
        /// </summary>
        /// <param name="panelStationPrepareInsertFormRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelStationPrepareInsertFormResponseDto>> StationPrepareInsertForm(PanelStationPrepareInsertFormRequestDto panelStationPrepareInsertFormRequest)
        {
            #region response dto oluşturuluyor
            PanelStationPrepareInsertFormResponseDto panelStationPrepareInsertFormResponse = new PanelStationPrepareInsertFormResponseDto();
            #endregion
            #region ödeme türleri getiriliyor ve response dto ya mapleniyor
            var paymentMethods = await _paymentMethodRepository.GetPaymentMethodAsNoTracking().ToListAsync();
            panelStationPrepareInsertFormResponse.PaymentMethod = _mapper.Map<List<PaymentMethodDto>>(paymentMethods);
            #endregion
            #region kullanım alanları getiriliyor ve response dto ya mapleniyor
            var typeOfUsing = await _typesOfUsingRepository.GetTypesOfUsingAsNoTracking().ToListAsync();
            panelStationPrepareInsertFormResponse.TypesOfUsing = _mapper.Map<List<TypesOfUsingDto>>(typeOfUsing);
            #endregion
            #region olanaklar getiriliyor ve response dto ya mapleniyor
            var opportunities = await _opportunityRepository.GetOpportunitiesAsNoTracking(new OpportunityFilterDto() { }).ToListAsync();
            panelStationPrepareInsertFormResponse.Opportunities = _mapper.Map<List<OpportunityDto>>(opportunities);
            #endregion
            #region imkanlar getiriliyor ve response dto ya mapleniyor
            var facilities = await _facilityRepository.GetFacilityAsNoTracking().ToListAsync();
            panelStationPrepareInsertFormResponse.Facilities = _mapper.Map<List<FacilityDto>>(facilities);
            #endregion
            //#region firm integration apiye istek atılarak integration process verileri getiriliyor ve response dto ya mapleniyor
            //#region request dto oluşturuluyor
            //GetIntegrationProcessRequestDto getIntegrationProcessRequest = new GetIntegrationProcessRequestDto();
            //#endregion
            //var getIntegrationProcessResponse = await _firmSettingManagementClientService.GetIntegrationProcess(getIntegrationProcessRequest);
            //panelStationPrepareInsertFormResponse.IntegrationProcessList = getIntegrationProcessResponse.Data.IntegrationProcessList;
            //#endregion
            return new SuccessResult<PanelStationPrepareInsertFormResponseDto>(panelStationPrepareInsertFormResponse);
        }
        public async Task<Result<UploadFileResponseDto>> AddStationPicture(IFormFile file, HttpRequest request)
        {
            UploadFileRequestDto uploadFileRequest = new UploadFileRequestDto();
            uploadFileRequest.GroupList = JsonConvert.DeserializeObject<List<UploadFileRequestDto.GroupData>>(request.Form["fileGroup"]);
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                uploadFileRequest.File = memoryStream.ToArray();
            }
            uploadFileRequest.FileName = file.FileName;
            var response = await _fileClientService.UploadPicture(uploadFileRequest);

            UploadFileResponseDto uploadFileResponse = new UploadFileResponseDto();
            uploadFileResponse.FileKey = response.FileKey;
            return new SuccessResult<UploadFileResponseDto>(uploadFileResponse);
        }
        /// <summary>
        /// istasyon verileri çekiliyor
        /// </summary>
        /// <param name="getPanelStationListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelStationListResponseDto>> GetStationList(GetPanelStationListRequestDto getPanelStationListRequest)
        {
            #region response dto oluşturuluyor
            GetPanelStationListResponseDto getPanelStationListResponse = new GetPanelStationListResponseDto();
            #endregion
            #region istasyonlar getiriliyor
            #region filter dto oluşturuluyor
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getPanelStationListRequest, opt =>
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
            #region response dto setleniyor
            getPanelStationListResponse.StationList = _mapper.Map<List<PanelStationListItemDto>>(stationList);
            #endregion
            return new SuccessResult<GetPanelStationListResponseDto>(getPanelStationListResponse);
        }
        /// <summary>
        /// istasyon seçimi için istasyonlar getiriliyor
        /// </summary>
        /// <param name="getStationsForSelectListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetStationsForSelectListResponseDto>> GetStationsForSelectList(GetStationsForSelectListRequestDto getStationsForSelectListRequest)
        {
            #region response dto oluşturuluyor
            GetStationsForSelectListResponseDto getStationsForSelectListResponse = new GetStationsForSelectListResponseDto();
            getStationsForSelectListResponse.StationList = new List<PanelStationSelectListItemDto>();
            #endregion
            #region istasyonlar getiriliyor
            #region filter dto oluşturuluyor
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getStationsForSelectListRequest, opt =>
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
            //var stations = await _stationContentRepository.GetStationWithContent(stationContentFilter).ToListAsync();
            var stations = await _stationContentRepository.GetStationContent(stationContentFilter, null).ToListAsync();
            #endregion
            #region response dto setleniyor
            getStationsForSelectListResponse.StationList = _mapper.Map<List<PanelStationSelectListItemDto>>(stations);
            #endregion
            return new SuccessResult<GetStationsForSelectListResponseDto>(getStationsForSelectListResponse);
        }
        /// <summary>
        /// panel için istasyon verileri çekiliyor
        /// </summary>
        /// <param name="dataTableFilterModel"></param>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<PanelStationListItemDto>>> GetStationDataTablePanel(DataTableFilterModel<GetPanelStationDataTableRequestDto> dataTableFilterModel)
        {
            #region istasyon sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region istasyonlar getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            var stations = GetStationForPanel(dataTableFilterModel.data);
            var resultStation = stations.ApplySortingDto(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultStation.Count();
            var stationList = await resultStation.Skip(ofset).Take(recordPerPage).ToListAsync();
            var stationListDto = _mapper.Map<List<PanelStationListItemDto>>(stationList);
            var result = new DataTableResponseWrapper<PanelStationListItemDto>(toplamKayit, stationListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<PanelStationListItemDto>>(result);
        }
        /// <summary>
        /// istasyon aktifliği güncelleniyor
        /// </summary>
        /// <param name="changeStationStateRequest"></param>
        /// <returns></returns>
        public async Task<Result<ChangeStationStateResponseDto>> ChangeStationState(ChangeStationStateRequestDto changeStationStateRequest)
        {
            #region response dto oluşturuluyor
            ChangeStationStateResponseDto changeStationStateResponse = new ChangeStationStateResponseDto();
            #endregion
            #region istasyon getiriliyor
            #region filter dto oluşturuluyor
            StationFilterDto stationFilter = _mapper.Map<StationFilterDto>(changeStationStateRequest);
            #endregion
            var station = await _stationRepository.GetStation(stationFilter,null,false).FirstOrDefaultAsync();
            #endregion
            if (station != null)
            {
                #region istasyon durumu güncelleniyor
                _stationRepository.UpdateWithProperties(station, new Expression<Func<Station, object>>[] {
                    s => s.IsActive,
                    s => s.LastUpdateDate,
                });
                station.LastUpdateDate = DateTime.Now;
                station.IsActive = !station.IsActive;
                #endregion
                #region veritabanına kayıt ediliyor
                await _stationRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<ChangeStationStateResponseDto>(changeStationStateResponse);
            }
            else
            {
                return new ErrorResult<ChangeStationStateResponseDto>(changeStationStateResponse, StationManagmentErrorEnum.STATION_NOT_FOUND_ERROR);
            }
        }
        /// <summary>
        /// istasyon kaldırılıyor
        /// </summary>
        /// <param name="removeStationRequest"></param>
        /// <returns></returns>
        public async Task<Result<RemoveStationResponseDto>> RemoveStation(RemoveStationRequestDto removeStationRequest)
        {
            #region response dto oluşturuluyor
            RemoveStationResponseDto removeStationResponse = new RemoveStationResponseDto();
            #endregion
            #region istaston getiriliyor
            #region filter dto oluşturuluyor
            StationFilterDto stationFilter = _mapper.Map<StationFilterDto>(removeStationRequest);
            #endregion
            //var station = await _stationRepository.GetStationWithConnectors(stationFilter).FirstOrDefaultAsync();
            var station = await _stationRepository.GetStation(stationFilter, include: source => source
     .Include(a => a.ChargeDevice)
     .ThenInclude(a => a.ChargeDeviceConnector),false).FirstOrDefaultAsync();
            #endregion
            if (station != null)
            {
                #region istasyon,cihazlar,cihaz bağlantıları, soketler ve soket bağlantıları siliniyor
                #region istasyon kaldırılıyor
                _stationRepository.UpdateWithProperties(station, new Expression<Func<Station, object>>[] {
                    s => s.Deleted,
                });
                station.Deleted = true;
                #endregion
                #region cihazlar kaldırılıyor
                _chargeDeviceRepository.UpdateWithProperties(station.ChargeDevice.ToArray(), new Expression<Func<ChargeDevice, object>>[] {
                    s => s.Deleted,
                });
                foreach (var device in station.ChargeDevice)
                {
                    #region soketler kaldırılıyor
                    _chargeDeviceConnectorRepository.UpdateWithProperties(device.ChargeDeviceConnector.ToArray(), new Expression<Func<ChargeDeviceConnector, object>>[] {
                        s => s.Deleted,
                    });
                    foreach (var conenctor in device.ChargeDeviceConnector)
                    {
                        conenctor.Deleted = true;
                    }
                    #endregion
                    device.Deleted = true;
                }
                #endregion
                #endregion
                await _stationRepository.SaveChangesAsync();
                return new SuccessResult<RemoveStationResponseDto>(removeStationResponse);
            }
            #region istasyon bulunamadı
            else
            {
                return new ErrorResult<RemoveStationResponseDto>(removeStationResponse, StationManagmentErrorEnum.STATION_NOT_FOUND_ERROR);
            }
            #endregion
        }
        #region private methods
        private async Task CreateStationLocationArea(AddPanelStationRequestDto addPanelStationRequest, Station stationEntity)
        {
            #region eklenecek station location areaları tutan liste tanımlanıyor
            List<StationLocationArea> stationLocationareas = new List<StationLocationArea>();
            #endregion
            #region station location area verisi oluşturuluyor
            #region location area config çekiliyor
            var locationAreaConfigs = await _locationAreaConfigRepository.GetLocationAreaConfigsAsNoTracking().ToListAsync();
            #endregion
            Point location = _utilService.GetPointOfLocation(addPanelStationRequest.StationAddress.Latitude, addPanelStationRequest.StationAddress.Longtitude);
            Point locationCoordinateCartesian = _utilService.GetPointOfLocationCoordinateCartesian(addPanelStationRequest.StationAddress.Latitude, addPanelStationRequest.StationAddress.Longtitude);
            #region location area config verileri için location area mevcutluğu kontrol edilip, yoksa oluşturuluyor ve station location area verisi oluşturuluyor
            foreach (var locationAreaConfig in locationAreaConfigs)
            {
                #region location arealar alınıyor
                #region filter dto oluşturuluyor
                LocationAreaFilterDto locationAreaFilter = new LocationAreaFilterDto();
                locationAreaFilter.LocationAreaConfigId = locationAreaConfig.Id;
                locationAreaFilter.Latitude = addPanelStationRequest.StationAddress.Latitude;
                locationAreaFilter.Longtitude = addPanelStationRequest.StationAddress.Longtitude;
                locationAreaFilter.CheckGroupRadius = true;
                #endregion
                #region eklenecek olan istasyon için, mesafesi en yakın olan location area alınıyor
                var selectedLocationArea = await GetLocationArea(locationAreaFilter, locationCoordinateCartesian).OrderBy(x => x.Distance).FirstOrDefaultAsync();
                #endregion
                #endregion
                #region eklenecek istasyon için uygun location area yok ise yeni location area ve station location area oluşturuluyor
                if (selectedLocationArea == null)
                {
                    #region location area oluşturuluyor
                    LocationArea locationArea = new LocationArea();
                    locationArea.LocationAreaConfigId = locationAreaConfig.Id;
                    locationArea.Location = location;
                    locationArea.LocationCoordinateCartesian = locationCoordinateCartesian;
                    #endregion
                    #region station location area oluşturuluyor
                    StationLocationArea stationLocationArea = new StationLocationArea();
                    stationLocationArea.LocationArea = locationArea;
                    stationLocationArea.Station = stationEntity;
                    #endregion
                    #region station entity sine dahil ediliyor oluşturulan station location area
                    stationLocationareas.Add(stationLocationArea);
                    #endregion
                }
                #endregion
                #region eklenecek istasyon için uygun location area mevcut
                else
                {
                    #region station location area oluşturuluyor
                    StationLocationArea stationLocationArea = new StationLocationArea();
                    stationLocationArea.LocationAreaId = selectedLocationArea.Id;
                    stationLocationArea.Station = stationEntity;
                    #endregion
                    #region station entity sine dahil ediliyor oluşturulan station location area
                    stationLocationareas.Add(stationLocationArea);
                    //stationEntity.StationLocationArea.Add(stationLocationArea);
                    #endregion
                }
                #endregion
            }
            #endregion
            await _stationLocationAreaRepository.InsertAsync(stationLocationareas);
            #endregion
        }
        private async Task UpdateStationLocationArea(UpdatePanelStationRequestDto updatePanelStationRequest, Station station)
        {
            #region station location area verisi oluşturuluyor
            #region location area config çekiliyor
            var locationAreaConfigs = await _locationAreaConfigRepository.GetLocationAreaConfigsAsNoTracking().ToListAsync();
            #endregion
            #region enlem ve boylam verileri düzenleniyor
            updatePanelStationRequest.Station.StationAddress.Latitude = updatePanelStationRequest.Station.StationAddress.Latitude.Replace(",", ".");
            updatePanelStationRequest.Station.StationAddress.Longtitude = updatePanelStationRequest.Station.StationAddress.Longtitude.Replace(",", ".");
            #endregion
            Point location = _utilService.GetPointOfLocation(updatePanelStationRequest.Station.StationAddress.Latitude, updatePanelStationRequest.Station.StationAddress.Longtitude);
            Point locationCoordinateCartesian = _utilService.GetPointOfLocationCoordinateCartesian(updatePanelStationRequest.Station.StationAddress.Latitude,
                updatePanelStationRequest.Station.StationAddress.Longtitude);
            #region location area config verileri için location area mevcutluğu kontrol edilip, yoksa oluşturuluyor ve station location area verisi oluşturuluyor
            foreach (var locationAreaConfig in locationAreaConfigs)
            {
                //#region location arealar alınıyor
                //#region filter dto oluşturuluyor
                //LocationAreaFilterDto locationAreaFilter = new LocationAreaFilterDto();
                //locationAreaFilter.LocationAreaConfigId = locationAreaConfig.Id;
                //locationAreaFilter.CheckGroupRadius = true;
                //#endregion
                //var locationAreas = await _locationAreaRepository.GetLocationAreaWithLocationConfigAreaAsNoTracking(locationAreaFilter).ToListAsync();
                //#endregion
                //#region alınan station locationlar ile eklenecek istasyonun lokasyonuna olan uzaklıklarını tutan dto oluşturuluyor
                //List<LocationAreaDistanceDto> locationAreaDistances = _mapper.Map<List<LocationAreaDistanceDto>>(locationAreas, opt =>
                //{
                //    opt.AfterMap((src, dest) =>
                //    {
                //        var destData = dest as List<LocationAreaDistanceDto>;
                //        #region distance hesaplanıp setleniyor
                //        foreach (var locationAreaDistance in destData)
                //        {
                //            locationAreaDistance.Distance = GeoCalculator.GetDistance(_utilService.ParseDouble(locationAreaDistance.CenterLatitude),
                //                _utilService.ParseDouble(locationAreaDistance.CenterLongtitude),
                //                _utilService.ParseDouble(updatePanelStationRequest.Station.StationAddress.Latitude), _utilService.ParseDouble(updatePanelStationRequest.Station.StationAddress.Longtitude), 1, DistanceUnit.Kilometers);
                //        }
                //        #endregion
                //    });
                //});
                //#endregion
                #region eklenecek olan istasyon için, mesafesi en yakın olan location area alınıyor
                #region filter dto oluşturuluyor
                LocationAreaFilterDto locationAreaFilter = new LocationAreaFilterDto();
                locationAreaFilter.LocationAreaConfigId = locationAreaConfig.Id;
                locationAreaFilter.CheckGroupRadius = true;
                #endregion
                var selectedLocationArea = await GetLocationArea(locationAreaFilter, locationCoordinateCartesian).OrderBy(x => x.Distance).FirstOrDefaultAsync();
                #endregion
                #region eklenecek istasyon için uygun location area yok ise yeni location area ve station location area oluşturuluyor
                if (selectedLocationArea == null)
                {
                    #region location area oluşturuluyor
                    LocationArea locationArea = new LocationArea();
                    locationArea.LocationAreaConfigId = locationAreaConfig.Id;
                    locationArea.Location = location;
                    locationArea.LocationCoordinateCartesian = locationCoordinateCartesian;
                    #endregion
                    #region station location area oluşturuluyor
                    StationLocationArea stationLocationArea = new StationLocationArea();
                    stationLocationArea.LocationArea = locationArea;
                    stationLocationArea.StationId = station.Id;
                    #endregion
                    #region station location area veritabanına kayıt ediliyor
                    await _stationLocationAreaRepository.InsertAsync(stationLocationArea);
                    #endregion
                }
                #endregion
                #region eklenecek istasyon için uygun location area mevcut
                else
                {
                    #region station location area oluşturuluyor
                    StationLocationArea stationLocationArea = new StationLocationArea();
                    stationLocationArea.LocationAreaId = selectedLocationArea.Id;
                    stationLocationArea.StationId = station.Id;
                    #endregion
                    #region station location area veritabanına kayıt ediliyor
                    await _stationLocationAreaRepository.InsertAsync(stationLocationArea);
                    #endregion
                }
                #endregion
            }
            #endregion
            #endregion
        }
        private IQueryable<PanelStationListItemDto> GetStationForPanel(GetPanelStationDataTableRequestDto getPanelStationDataTableRequest)
        {
            var stationContentFilter = _mapper.Map<StationContentFilterDto>(getPanelStationDataTableRequest, opt =>
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
            //var stationResult = _stationContentRepository.GetStationContentWithAllRelationsAsNoTracking(stationContentFilter);
            var stationResult = _stationContentRepository.GetStationContent(stationContentFilter, include: source => source
             .Include(a => a.Station)
             .ThenInclude(a => a.Firm)
             .ThenInclude(a => a.FirmLogo)
             .Include(a => a.Station)
             .ThenInclude(a => a.Company));
            var stationAddressResult = _stationAddressRepository.GetStationAddressWithNoTracking();
            var stationQuery = (from station in stationResult
                                join stationAddress in stationAddressResult
                            on new { A = station.Station.Id }
                                  equals new { A = stationAddress.StationId }
                            into stationAddressList
                                from stationAddress in stationAddressList.DefaultIfEmpty()
                                select new PanelStationListItemDto
                                {
                                    Id = station.Station.Id,
                                    Description = station.Description,
                                    Name = station.Name,
                                    IsActive = station.Station.IsActive,
                                    Latitude = stationAddress != null ? stationAddress.Location.X.ToString().Replace(",", ".") : "",
                                    Longtitude = stationAddress != null ? stationAddress.Location.Y.ToString().Replace(",", ".") : "",
                                    FirmName = station.Station.Firm.Name,
                                    CompanyName = station.Station.Company.Name,
                                    FirmLogoMediaGuiId = station.Station.Firm.FirmLogo.MediaGuiId
                                });
            return stationQuery;
        }
        /// <summary>
        /// istasyon güncelleme formu için istasyon content ve  content language çekiliyor
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        private IQueryable<PanelStationContentFormInputDto> GetContentLanguageWithStationContent(long stationId)
        {
            var contentLanguageList = _contentLanguageContentRepository.GetContentLanguages();
            //var stationContentList = _stationContentRepository.GetStationContentForPanelForm(new StationContentFilterDto() { StationId = stationId });
            var stationContentList = _stationContentRepository.GetStationContent(new StationContentFilterDto() { StationId = stationId }, null);
            var query = (from contentLanguage in contentLanguageList
                         join stationContent in stationContentList
                          on new { A = contentLanguage.Id, B = stationId }
                                equals new { A = stationContent.ContentLanguageId, B = stationContent.StationId }
                          into stationContents
                         from stationContent in stationContents.DefaultIfEmpty()
                         select new PanelStationContentFormInputDto
                         {
                             ContentLanguageId = contentLanguage.Id,
                             Description = contentLanguage.Description,
                             Content = contentLanguage.Content,
                             StationId = stationContent.StationId,
                             StationContentId = stationContent.Id,
                             StationDescription = stationContent.Description,
                             StationName = stationContent.Name,
                             IsDefault = contentLanguage.IsDefault
                         });
            return query;
        }
        private IQueryable<LocationAreaDistanceDto> GetLocationArea(LocationAreaFilterDto locationAreaFilter, Point location)
        {
            var locationAreas = _locationAreaRepository.GetLocationAreaWithLocationConfigAreaAsNoTracking(locationAreaFilter);
            var query = (from locationArea in locationAreas
                         select new LocationAreaDistanceDto
                         {
                             Id = locationArea.Id,
                             LocationAreaConfigId = locationArea.LocationAreaConfigId,
                             Distance = (locationArea.LocationCoordinateCartesian.Distance(location)),
                         });
            return query;
        }
        #endregion
    }
}
