using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.ChargeDtos;
using Shared.Domain.Dto.ApiDto.PanelChargeDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.StationContentDtos;
using Shared.Domain.Dto.FileDto.ExportExcelDtos;
using Shared.Domain.Dto.FileDto.ExportPdfDtos;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.FileApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ChargeRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ContentLanguageRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.StationContentRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ChargeManagmentServiceInterfaces;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static MassTransit.ValidationResultExtensions;

namespace Api.Application.Services.PanelServices.ChargeManagment
{
    public class ChargeManagmentService : BaseService, IChargeManagmentService
    {
        private readonly IChargeRepository _chargeRepository;
        private readonly IStationContentRepository _stationContentRepository;
        private readonly IAdminContextProvider _adminContextProvider;
        private readonly IFileClientService _fileClientService;
        private readonly IUtilService _utilService;
        private readonly IConfiguration _configuration;
        public ChargeManagmentService(IMapper mapper,
                           IConfiguration configuration,
                           IAdminContextProvider adminContextProvider,
                           IUtilService utilService,
                           IChargeRepository chargeRepository,
                           IStationContentRepository stationContentRepository,
                           IFileClientService fileClientService) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _adminContextProvider = adminContextProvider;
            _utilService = utilService;
            _chargeRepository = chargeRepository;
            _stationContentRepository = stationContentRepository;
            _fileClientService = fileClientService;
        }
        /// <summary>
        /// şarj işlemi detayı getiriliyor
        /// </summary>
        /// <param name="GetChargeDetailPanelRequestDto"></param>
        /// <returns></returns>
        public async Task<Result<GetChargeDetailPanelResponseDto>> GetChargeDetail(GetChargeDetailPanelRequestDto getChargeDetailPanelRequest)
        {
            #region response dto oluşturuluyor
            GetChargeDetailPanelResponseDto chargeDetailPanelResponse = new GetChargeDetailPanelResponseDto();
            chargeDetailPanelResponse.ChargeDetail = new ChargeDetailPanelDto();
            #endregion
            chargeDetailPanelResponse.ChargeDetail = await GetChargeDetailPanel(getChargeDetailPanelRequest).FirstOrDefaultAsync();
            return new SuccessResult<GetChargeDetailPanelResponseDto>(chargeDetailPanelResponse);
        }
        /// <summary>
        /// panel datatable için şarj işlemleri getiriliyor
        /// </summary>
        /// <param name="dataTableFilterModel"></param>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<PanelChargeListItemDto>>> GetChargeDatatable(DataTableFilterModel<GetPanelChargeDataTableRequestDto> dataTableFilterModel)
        {
            #region şarj sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region şarjlar getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            var charges = GetCharge(dataTableFilterModel.data);
            var resultCharge = charges.ApplySortingDto(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultCharge.Count();
            var chargeList = await resultCharge.Skip(ofset).Take(recordPerPage).ToListAsync();
            var chargeListDto = _mapper.Map<List<PanelChargeListItemDto>>(chargeList);
            var result = new DataTableResponseWrapper<PanelChargeListItemDto>(toplamKayit, chargeListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<PanelChargeListItemDto>>(result);
        }
        /// <summary>
        ///  sarj işleminin durumu güncelleniyor
        /// </summary>
        /// <param name="panelChangeStateChargeRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelChangeChargeStateResponseDto>> ChangeChargeState(PanelChangeChargeStateRequestDto panelChangeStateChargeRequest)
        {
            #region response dto oluşturuluyor
            PanelChangeChargeStateResponseDto panelChangeStateChargeResponse = new PanelChangeChargeStateResponseDto();
            #endregion
            #region şimdi ki zaman değişkeni oluşturuluyor
            var datetimeNow = DateTime.Now;
            #endregion
            #region şarj işlemi getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(panelChangeStateChargeRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as ChargeFilterDto;
                    destData.CompanyId = _adminContextProvider.CompanyId;
                    destData.FirmId = _adminContextProvider.FirmId;
                });
            });
            #endregion
            var charge = await _chargeRepository.GetCharge(chargeFilter,null,false).FirstOrDefaultAsync();
            #endregion
            if (charge != null)
            {
                #region durumu güncelleniyor
                _chargeRepository.UpdateWithProperties(charge, new Expression<Func<Charge, object>>[] {
                    s => s.State,
                });
                charge.State = panelChangeStateChargeRequest.ChargeState;
                #endregion
                #region kaydediliyor
                await _chargeRepository.SaveChangesAsync();
                #endregion
            }
            return new SuccessResult<PanelChangeChargeStateResponseDto>(panelChangeStateChargeResponse);
        }
        /// <summary>
        /// şarj işlemleri exceli oluşturuluyor
        /// </summary>
        /// <param name="panelExportExcelChargeProcessRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelExportExcelChargeProcessResponseDto>> ExportExcelChargeProcess(PanelExportExcelChargeProcessRequestDto panelExportExcelChargeProcessRequest)
        {
            #region response dto oluşturuluyor
            PanelExportExcelChargeProcessResponseDto panelExportExcelChargeProcessResponse = new PanelExportExcelChargeProcessResponseDto();
            #endregion
            #region işlemler getiriliyor
            #region filter dto oluşturuluyor
            GetPanelChargeDataTableRequestDto getPanelChargeDataTableRequest = _mapper.Map<GetPanelChargeDataTableRequestDto>(panelExportExcelChargeProcessRequest);
            #endregion
            var chargeList = await GetCharge(getPanelChargeDataTableRequest).ToListAsync();
            #endregion
            #region file service istek atılarak excel oluşturuluyor
            #region request dto oluşturuluyor
            CreateExcelChargeRequestDto exportExcelRequest = new CreateExcelChargeRequestDto();
            exportExcelRequest.ChargeList = chargeList;
            #endregion
            var exportExcelResponse = await _fileClientService.CreateExcelCharge(exportExcelRequest);
            #endregion
            if (exportExcelResponse.ResultType == ResultType.Ok)
            {
                #region response  dto setleniyor
                panelExportExcelChargeProcessResponse.FileKey = exportExcelResponse.Data.FileKey;
                panelExportExcelChargeProcessResponse.FileUrl = exportExcelResponse.Data.FileUrl;
                #endregion
                return new SuccessResult<PanelExportExcelChargeProcessResponseDto>(panelExportExcelChargeProcessResponse);
            }
            #region excel oluşturma işlemi başarısız
            else
            {
                return new ErrorResult<PanelExportExcelChargeProcessResponseDto>(panelExportExcelChargeProcessResponse, ChargeManagmentErrorEnum.EXPORT_EXCEL_FAILED);
            }
            #endregion
        }
        /// <summary>
        /// şarj işlemleri pdf i oluşturuluyor
        /// </summary>
        /// <param name="panelExportPdfChargeProcessRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelExportPdfChargeProcessResponseDto>> ExportPdfChargeProcess(PanelExportPdfChargeProcessRequestDto panelExportPdfChargeProcessRequest)
        {
            #region response dto oluşturuluyor
            PanelExportPdfChargeProcessResponseDto panelExportPdfChargeProcessResponse = new PanelExportPdfChargeProcessResponseDto();
            #endregion
            #region işlemler getiriliyor
            #region filter dto oluşturuluyor
            GetPanelChargeDataTableRequestDto getPanelChargeDataTableRequest = _mapper.Map<GetPanelChargeDataTableRequestDto>(panelExportPdfChargeProcessRequest);
            #endregion
            var chargeList = await GetCharge(getPanelChargeDataTableRequest).ToListAsync();
            #endregion
            #region file service istek atılarak excel oluşturuluyor
            #region request dto oluşturuluyor
            CreatePdfChargeRequestDto createPdfChargeRequest = new CreatePdfChargeRequestDto();
            createPdfChargeRequest.ChargeList = chargeList;
            #endregion
            var createPdfChargeResponse = await _fileClientService.CreatePdfCharge(createPdfChargeRequest);
            #endregion
            if (createPdfChargeResponse.ResultType == ResultType.Ok)
            {
                #region response  dto setleniyor
                panelExportPdfChargeProcessResponse.FileKey = createPdfChargeResponse.Data.FileKey;
                panelExportPdfChargeProcessResponse.FileUrl = createPdfChargeResponse.Data.FileUrl;
                #endregion
                return new SuccessResult<PanelExportPdfChargeProcessResponseDto>(panelExportPdfChargeProcessResponse);
            }
            #region excel oluşturma işlemi başarısız
            else
            {
                return new ErrorResult<PanelExportPdfChargeProcessResponseDto>(panelExportPdfChargeProcessResponse, ChargeManagmentErrorEnum.EXPORT_PDF_FAILED);
            }
            #endregion
        }
        #region private methods
        /// <summary>
        /// şarj detayı getiriliyor
        /// </summary>
        /// <param name="getChargeDetailPanelRequest"></param>
        /// <returns></returns>
        private IQueryable<ChargeDetailPanelDto> GetChargeDetailPanel(GetChargeDetailPanelRequestDto getChargeDetailPanelRequest)
        {
            #region şarj işlemleri getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(getChargeDetailPanelRequest);
            #endregion
            //var chargeQuery = _chargeRepository.GetChargeAsNoTracking(chargeFilter);
            var chargeQuery = _chargeRepository.GetCharge(chargeFilter, include: source => source
     .Include(a => a.ChargeDeviceConnector)
     .ThenInclude(a => a.ChargeDevice)
     .ThenInclude(a => a.Station)
     .ThenInclude(a => a.StationAddress)
     .Include(a => a.User));
            #endregion
            //var stationManagmentContentList = _stationContentRepository.GetStationContent(new StationContentFilterDto() { StationId = getChargeDetailPanelRequest.StationId });
            var stationManagmentContentList = _stationContentRepository.GetStationContent(new StationContentFilterDto() { StationId = getChargeDetailPanelRequest.StationId },
 include: source => source
.Include(a => a.ContentLanguage));
            var query = (from charge in chargeQuery
                         join stationContent in stationManagmentContentList
                         on charge.ChargeDeviceConnector.ChargeDevice.StationId
                         equals stationContent.StationId
                         into stationContents
                         from stationContent in stationContents.DefaultIfEmpty()
                         where (chargeFilter.ContentLanguageId != null && stationContent.Id == chargeFilter.ContentLanguageId) || stationContent.ContentLanguage.IsDefault
                         select new ChargeDetailPanelDto
                         {
                             Id = charge.Id,
                             GuiId = charge.GuiId,
                             LoadedKw = Math.Round(charge.LoadedKw.GetValueOrDefault(), 2),
                             CalculatedPrice = charge.CalculatedPrice.GetValueOrDefault(),
                             LastUpdateDate = charge.LastUpdateDate,
                             StartDate = charge.StartTime,
                             ProcessEndDate = charge.EndTime,
                             ProcessingTime = charge.StartTime != null && charge.EndTime != null ?
                             (int)(charge.EndTime.GetValueOrDefault() - charge.StartTime.GetValueOrDefault()).TotalSeconds / 60 + " dk," +
                             Math.Round((charge.EndTime.GetValueOrDefault() - charge.StartTime.GetValueOrDefault()).TotalSeconds % 60, 0) + " sn"
                             : charge.StartTime != null && charge.LastUpdateDate != null ?
                             (int)(charge.LastUpdateDate.GetValueOrDefault() - charge.StartTime.GetValueOrDefault()).TotalSeconds / 60 + " dk," +
                             Math.Round((charge.LastUpdateDate.GetValueOrDefault() - charge.StartTime.GetValueOrDefault()).TotalSeconds % 60, 0) + " sn"
                             : "-"
                             ,
                             //PaymentMethod = charge.UserPayment != null ? charge.UserPayment.PaymentMethod : null,
                             State = charge.State,
                             UserName = charge.User != null ? charge.User.Name : "",
                             UserSurname = charge.User != null ? charge.User.Surname : "",
                             Phone = charge.User != null ? "+" + charge.User.PhoneCountryCode + charge.User.Phone : "",
                             StationName = stationContent.Name,
                             DeviceName = charge.ChargeDeviceConnector.ChargeDevice.Name,
                             ConnectorName = charge.ChargeDeviceConnector.Name,
                             ConnectorNumber = charge.ChargeDeviceConnector.ConnectorNo,
                             StationAddress = charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.CountryName + "/" + charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.CityName +
                                               ", İlçe: " + charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.TownName +
                                               ", Mahalle: " + charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.Neighbourhood +
                                               ", Cadde/Sokak: " + charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.Street +
                                               ", Adres Açıklaması: " + charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.Description
                         });
            return query;
        }
        /// <summary>
        /// şarj işlemi getiriliyor
        /// </summary>
        /// <param name="getChargeProcessDatatablePanelRequest"></param>
        /// <returns></returns>
        private IQueryable<PanelChargeListItemDto> GetCharge(GetPanelChargeDataTableRequestDto getChargeProcessDatatablePanelRequest)
        {
            #region şarj işlemleri getiriliyor
            #region filter dto oluşturuluyor
            ChargeFilterDto chargeFilter = _mapper.Map<ChargeFilterDto>(getChargeProcessDatatablePanelRequest, opt =>
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
                });
            });
            #endregion
            //var chargeProcess = _chargeRepository.GetChargeAsNoTracking(chargeFilter);
            var chargeProcess = _chargeRepository.GetCharge(chargeFilter, include: source => source
   .Include(a => a.User)
   .Include(a => a.PaymentInfo)
   .Include(a => a.ChargeDeviceConnector)
   .ThenInclude(a => a.ChargeDevice)
   .ThenInclude(a => a.Station)
   .ThenInclude(a => a.StationAddress));
            #endregion
            //var stationManagmentContentList = _stationContentRepository.GetStationContent(new StationContentFilterDto() { StationId = getChargeProcessDatatablePanelRequest.StationId });
            var stationManagmentContentList = _stationContentRepository.GetStationContent(new StationContentFilterDto() { StationId = getChargeProcessDatatablePanelRequest.StationId },
   include: source => source
.Include(a => a.ContentLanguage));
            var query = (from charge in chargeProcess
                         join stationContent in stationManagmentContentList
                         on charge.ChargeDeviceConnector.ChargeDevice.StationId
                         equals stationContent.StationId
                         into stationContents
                         from stationContent in stationContents.DefaultIfEmpty()
                         where (chargeFilter.ContentLanguageId != null && stationContent.Id == chargeFilter.ContentLanguageId) || stationContent.ContentLanguage.IsDefault
                         select new PanelChargeListItemDto
                         {
                             Id = charge.Id,
                             GuiId = charge.GuiId,
                             CalculatedPrice = charge.CalculatedPrice != null ? Math.Round(charge.CalculatedPrice.GetValueOrDefault() - charge.Discount, 2) : 0,
                             StationId = charge.ChargeDeviceConnector.ChargeDevice.StationId.GetValueOrDefault(),
                             LoadedKw = Math.Round(charge.LoadedKw.GetValueOrDefault(), 2),
                             RefundedPrice = charge.PaymentInfo != null ? Math.Round(charge.PaymentInfo.RefundedPrice.GetValueOrDefault(), 2) : 0,
                             CompletedPrice = charge.PaymentInfo != null ? Math.Round(charge.PaymentInfo.PaidPrice - charge.PaymentInfo.RefundedPrice.GetValueOrDefault(), 2) : 0,
                             PaidPrice = charge.PaymentInfo != null ? Math.Round(charge.CalculatedPrice.GetValueOrDefault() - charge.Discount, 2) : 0,
                             LastUpdateDate = charge.LastUpdateDate,
                             StartingDate = charge.StartTime,
                             State = charge.State,
                             ChargeFirmType = charge.ChargeFirmType,
                             StationName = stationContent.Name,
                             Identifier = charge.ChargeDeviceConnector.ChargeDevice.Identifier,
                             ConnectorName = charge.ChargeDeviceConnector.Name,
                             StationTown = charge.ChargeDeviceConnector.ChargeDevice.Station.StationAddress.TownName,
                             PaymentGuiId = charge.PaymentInfo != null ? charge.PaymentInfo.PaymentGuiId : "",
                             PaymentInfoId = charge.PaymentInfo != null ? charge.PaymentInfo.Id : 0,
                             UserNameSurname = charge.User.Name + " " + charge.User.Surname
                         });
            return query;
        }
        #endregion
    }

}
