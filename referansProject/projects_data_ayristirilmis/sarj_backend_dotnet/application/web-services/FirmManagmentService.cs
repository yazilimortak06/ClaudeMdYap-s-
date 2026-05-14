using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.CompanyDtos;
using Shared.Domain.Dto.ApiDto.FirmDtos;
using Shared.Domain.Dto.ApiDto.FirmPriceDtos;
using Shared.Domain.Dto.ApiDto.OpportunityDtos;
using Shared.Domain.Dto.ApiDto.PanelAdminDtos;
using Shared.Domain.Dto.ApiDto.PanelCompanyDtos;
using Shared.Domain.Dto.ApiDto.PanelFirmDtos;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.ApiDto.PaymentMethodDtos;
using Shared.Domain.Dto.ApiDto.PolicyDtos;
using Shared.Domain.Dto.ApiDto.StationDtos;
using Shared.Domain.Dto.ApiDto.StationFacilityDtos;
using Shared.Domain.Dto.ApiDto.StationOpportunityDtos;
using Shared.Domain.Dto.ApiDto.StationTypeOfUsingDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.Dto.FirmIntegrationDto.FirmSettingDtos;
using Shared.Domain.Entities.ApiEntities.AdminModule;
using Shared.Domain.Entities.ApiEntities.FirmModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Errors.FirmIntegrationErrors;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.FileApiInterfaces;
using Shared.Domain.HttpClients.HttpClientInterfaces.FirmIntegrationApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.FirmRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.PolicyRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.FirmManagmentServiceInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PolicyManagmentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Services.PanelServices.FirmManagment
{
    public class FirmManagmentService : BaseService, IFirmManagmentService
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IFirmPriceRepository _firmPriceRepository;
        private readonly IFirmLogoRepository _firmLogoRepository;
        private readonly IConfiguration _configuration;
        private readonly IFileClientService _fileClientService;
        private readonly IAdminContextProvider _adminContextProvider;
        private readonly IUtilService _utilService;
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IFirmSettingManagementClientService _firmSettingManagementClientService;

        public FirmManagmentService(IMapper mapper,
                            IConfiguration configuration,
                           ICustomHttpUtilService customHttpUtilService,
                            IUtilService utilService,
                            IPolicyRepository policyRepository,
                            IFirmRepository firmRepository,
                            IFirmLogoRepository firmLogoRepository,
                            IFileClientService fileClientService,
                            IAdminContextProvider adminContextProvider,
                            IFirmSettingManagementClientService firmSettingManagementClientService,
                            IFirmPriceRepository firmPriceRepository) : base(
                           mapper
                               )
        {
            _configuration = configuration;
            _utilService = utilService;
            _customHttpUtilService = customHttpUtilService;
            _firmRepository = firmRepository;
            _firmLogoRepository = firmLogoRepository;
            _fileClientService = fileClientService;
            _adminContextProvider = adminContextProvider;
            _firmSettingManagementClientService = firmSettingManagementClientService;
            _firmPriceRepository = firmPriceRepository;
        }
        #region ekleme,güncelleme,silme
        /// <summary>
        /// firma ekleniyor
        /// </summary>
        /// <param name="addFirmRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddFirmResponseDto>> AddFirm(AddFirmRequestDto addFirmRequest)
        {
            #region response dto oluşturuluyor
            AddFirmResponseDto addFirmResponse = new AddFirmResponseDto();
            #endregion
            #region firm entity oluşturuluyor
            var firm = _mapper.Map<Firm>(addFirmRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Firm;
                    destData.CreatedDate = DateTime.Now;
                    destData.GuiId = Guid.NewGuid().ToString();
                    destData.IsActive = true;
                });
            });
            #endregion
            #region veritabanına kayıt ediliyor
            await _firmRepository.InsertAsync(firm);
            await _firmRepository.SaveChangesAsync();
            #endregion
            return new SuccessResult<AddFirmResponseDto>(addFirmResponse);
        }
        /// <summary>
        /// firma ayarı ekleniyor
        /// </summary>
        /// <param name="addOrUpdateFirmSettingFromPanelRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddOrUpdateFirmSettingFromPanelResponseDto>> AddOrUpdateFirmSettingFromPanel(AddOrUpdateFirmSettingFromPanelRequestDto addOrUpdateFirmSettingFromPanelRequest)
        {
            #region response dto oluşturuluyor
            AddOrUpdateFirmSettingFromPanelResponseDto addOrUpdateFirmSettingFromPanelResponse = new AddOrUpdateFirmSettingFromPanelResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(addOrUpdateFirmSettingFromPanelRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : addOrUpdateFirmSettingFromPanelRequest.Id;
                });
            });
            #endregion
            var firm = await _firmRepository.GetFirmAsNoTracking(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region firm integration apiye istek atılarak firm integration settings verisi ekleniyor
                #region request dto oluşturuluyor
                AddOrUpdateFirmSettingRequestDto addOrUpdateFirmSettingRequest = _mapper.Map<AddOrUpdateFirmSettingRequestDto>(addOrUpdateFirmSettingFromPanelRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as AddOrUpdateFirmSettingRequestDto;
                        destData.FirmGuiId = firm.GuiId;
                    });
                });
                #endregion
                var addOrUpdateFirmSettingResponse = await _firmSettingManagementClientService.AddOrUpdateFirmSetting(addOrUpdateFirmSettingRequest);
                #endregion
                #region firm integration setting verisi ekleme başarılı
                if (addOrUpdateFirmSettingResponse.ResultType == ResultType.Ok)
                {
                    return new SuccessResult<AddOrUpdateFirmSettingFromPanelResponseDto>(addOrUpdateFirmSettingFromPanelResponse);
                }
                #endregion
                #region
                else
                {
                    if (addOrUpdateFirmSettingResponse.ErrorCode == (int)FirmSettingErrorEnum.FIRM_SETTING_EXIST_WITH_SAME_NAME)
                    {
                        return new ErrorResult<AddOrUpdateFirmSettingFromPanelResponseDto>(addOrUpdateFirmSettingFromPanelResponse, FirmManagmentErrorEnum.FIRM_SETTING_EXIST_WITH_SAME_NAME);
                    }
                    else
                    {
                        return new ErrorResult<AddOrUpdateFirmSettingFromPanelResponseDto>(addOrUpdateFirmSettingFromPanelResponse, FirmManagmentErrorEnum.AN_ERROR_OCCURRED_ADD_FIRM_SETTING);
                    }
                }
                #endregion
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<AddOrUpdateFirmSettingFromPanelResponseDto>(addOrUpdateFirmSettingFromPanelResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// firma güncelleniyor
        /// </summary>
        /// <param name="updateFirmRequest"></param>
        /// <returns></returns>
        public async Task<Result<UpdateFirmResponseDto>> UpdateFirm(UpdateFirmRequestDto updateFirmRequest)
        {
            #region response dto oluşturuluyor
            UpdateFirmResponseDto updateFirmResponse = new UpdateFirmResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = new FirmFilterDto();
            firmFilter.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : updateFirmRequest.Id;
            #endregion
            var firm = await _firmRepository.GetFirm(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region firma güncelleniyor
                _firmRepository.UpdateWithProperties(firm, new Expression<Func<Firm, object>>[] {
                    s => s.UpdatedDate,
                    s => s.DividendPercantage,
                    s => s.Mail,
                    s => s.Name,
                    s => s.Phone,
                    s => s.IntegrationType,
                    s => s.OcppUrlAddress,
                });
                firm.DividendPercantage = updateFirmRequest.DividendPercantage;
                firm.Mail = updateFirmRequest.Mail;
                firm.Name = updateFirmRequest.Name;
                firm.Phone = updateFirmRequest.Phone;
                firm.IntegrationType = updateFirmRequest.IntegrationType;
                firm.OcppUrlAddress = updateFirmRequest.OcppUrlAddress;
                firm.UpdatedDate = DateTime.Now;
                #endregion
                #region logo mevcut değilse ekleniyor
                if (firm.FirmLogo == null)
                {
                    var firmLogo = _mapper.Map<FirmLogo>(updateFirmRequest.FirmLogo);
                    await _firmLogoRepository.InsertAsync(firmLogo);
                }
                #endregion
                #region eğer logo kaldırıldıysa siliniyor veya değiştiyse eski logo verisi kaldırılıyor, yenisi ekleniyor
                else
                {
                    if (updateFirmRequest.FirmLogo == null || updateFirmRequest.FirmLogo.MediaGuiId != firm.FirmLogo.MediaGuiId)
                    {
                        #region eski logo verisi kaldırılıyor
                        _firmLogoRepository.UpdateWithProperties(firm.FirmLogo, new Expression<Func<FirmLogo, object>>[] {
                            s => s.Deleted,
                            s => s.FirmId,
                        });
                        firm.FirmLogo.Deleted = true;
                        firm.FirmLogo.FirmId = null;
                        #endregion
                        #region yeni logo verisi ekleniyor
                        if (updateFirmRequest.FirmLogo != null)
                        {
                            var firmLogo = _mapper.Map<FirmLogo>(updateFirmRequest.FirmLogo);
                            await _firmLogoRepository.InsertAsync(firmLogo);
                        }
                        #endregion
                    }
                }
                #endregion
                #region veritabanına kayıt ediliyor
                await _firmRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<UpdateFirmResponseDto>(updateFirmResponse);
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<UpdateFirmResponseDto>(updateFirmResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        #endregion
        /// <summary>
        /// panel için firmalar verileri çekiliyor
        /// </summary>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<FirmDataTableItemDto>>> GetFirmDataTablePanel(DataTableFilterModel<GetFirmDataTableRequestDto> dataTableFilterModel)
        {
            #region firma sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region firmalar getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(dataTableFilterModel.data, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : null;
                });
            });
            #endregion
            var firms = _firmRepository.GetFirmAsNoTracking(firmFilter);
            var resultFirm = firms.ApplySorting(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultFirm.Count();
            var firmList = await resultFirm.Skip(ofset).Take(recordPerPage).ToListAsync();
            var firmListDto = _mapper.Map<List<FirmDataTableItemDto>>(firmList);
            var result = new DataTableResponseWrapper<FirmDataTableItemDto>(toplamKayit, firmListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<FirmDataTableItemDto>>(result);
        }
        /// <summary>
        /// panel için firma fiyat verileri çekiliyor
        /// </summary>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<FirmPriceDataTableItemDto>>> GetFirmPriceDatatablePanel(DataTableFilterModel<GetFirmPriceDataTableRequestDto> dataTableFilterModel)
        {
            #region firma fiyat sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region firma fiyatları getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            #region filter dto oluşturuluyor
            FirmPriceFilterDto firmPriceFilter = _mapper.Map<FirmPriceFilterDto>(dataTableFilterModel.data, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmPriceFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : null;
                });
            });
            #endregion
            var firmPrices = _firmPriceRepository.GetFirmPrice(firmPriceFilter, include: source => source
             .Include(a => a.Firm)
             .Include(a => a.ChargeDevicePowerType));
            var resultFirmPrice = firmPrices.ApplySorting(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultFirmPrice.Count();
            var firmPriceList = await resultFirmPrice.Skip(ofset).Take(recordPerPage).ToListAsync();
            var firmPriceListDto = _mapper.Map<List<FirmPriceDataTableItemDto>>(firmPriceList);
            var result = new DataTableResponseWrapper<FirmPriceDataTableItemDto>(toplamKayit, firmPriceListDto);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<FirmPriceDataTableItemDto>>(result);
        }
        /// <summary>
        /// seçim listesi için firmalar getiriliyor
        /// </summary>
        /// <param name="getFirmForSelectListRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetFirmForSelectListResponseDto>> GetFirmForSelectList(GetFirmForSelectListRequestDto getFirmForSelectListRequest)
        {
            #region response dto oluşturuluyor
            GetFirmForSelectListResponseDto getFirmForSelectListResponse = new GetFirmForSelectListResponseDto();
            getFirmForSelectListResponse.Firms = new List<PanelFirmSelectListItemDto>();
            #endregion
            #region firmalar getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(getFirmForSelectListRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : null;
                });
            });
            #endregion
            var firms = await _firmRepository.GetFirmAsNoTracking(firmFilter).ToListAsync();
            #endregion
            #region response dto setleniyor
            getFirmForSelectListResponse.Firms = _mapper.Map<List<PanelFirmSelectListItemDto>>(firms);
            #endregion
            return new SuccessResult<GetFirmForSelectListResponseDto>(getFirmForSelectListResponse);
        }
        /// <summary>
        /// güncelleme işlemi için firma getiriliyor
        /// </summary>
        /// <param name="getFirmForUpdateRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetFirmForUpdateResponseDto>> GetFirmForUpdate(GetFirmForUpdateRequestDto getFirmForUpdateRequest)
        {
            #region response dto oluşturuluyor
            GetFirmForUpdateResponseDto getFirmForUpdateResponse = new GetFirmForUpdateResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(getFirmForUpdateRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : getFirmForUpdateRequest.Id;
                });
            });
            #endregion
            var firm = await _firmRepository.GetFirmAsNoTracking(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region response dto setleniyor
                getFirmForUpdateResponse = _mapper.Map<GetFirmForUpdateResponseDto>(firm);
                #endregion
                return new SuccessResult<GetFirmForUpdateResponseDto>(getFirmForUpdateResponse);
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<GetFirmForUpdateResponseDto>(getFirmForUpdateResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// Firma ayarı Formu İçin Bilgiler Çekilir
        /// </summary>
        /// <param name="panelFirmSettingPrepareFormRequest"></param>
        /// <returns></returns>
        public async Task<Result<PanelFirmSettingPrepareFormResponseDto>> FirmSettingPrepareForm(PanelFirmSettingPrepareFormRequestDto panelFirmSettingPrepareFormRequest)
        {
            #region response dto oluşturuluyor
            PanelFirmSettingPrepareFormResponseDto panelFirmSettingPrepareFormResponse = new PanelFirmSettingPrepareFormResponseDto();
            #endregion
            #region firma kontrol ediliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(panelFirmSettingPrepareFormRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : panelFirmSettingPrepareFormRequest.FirmId;
                });
            });
            #endregion
            var firm = await _firmRepository.GetFirmAsNoTracking(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region firm integration apiye istek atılarak integration process verileri getiriliyor ve response dto ya mapleniyor
                #region request dto oluşturuluyor
                GetIntegrationProcessRequestDto getIntegrationProcessRequest = new GetIntegrationProcessRequestDto();
                #endregion
                var getIntegrationProcessResponse = await _firmSettingManagementClientService.GetIntegrationProcess(getIntegrationProcessRequest);
                panelFirmSettingPrepareFormResponse.IntegrationProcessList = _mapper.Map<List<IntegrationProcessFormInputDto>>(getIntegrationProcessResponse.Data.IntegrationProcessList);
                #endregion
                #region firm integration apiye istek atılarak firm setting verileri getiriliyor ve response dto ya mapleniyor
                #region request dto oluşturuluyor
                GetFirmSettingRequestDto getFirmSettingRequest = new GetFirmSettingRequestDto();
                getFirmSettingRequest.FirmGuiId = firm.GuiId;
                #endregion
                var getFirmSettingResponse = await _firmSettingManagementClientService.GetFirmSetting(getFirmSettingRequest);
                #region response dto ya setleniyor
                if (getFirmSettingResponse.ResultType == ResultType.Ok)
                {
                    panelFirmSettingPrepareFormResponse.FirmSetting = getFirmSettingResponse.Data.FirmSetting;
                }
                #endregion
                #endregion
                return new SuccessResult<PanelFirmSettingPrepareFormResponseDto>(panelFirmSettingPrepareFormResponse);
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<PanelFirmSettingPrepareFormResponseDto>(panelFirmSettingPrepareFormResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// firma aktifliği güncelleniyor
        /// </summary>
        /// <param name="changeActiveStateFirmRequest"></param>
        /// <returns></returns>
        public async Task<Result<ChangeActiveStateFirmResponseDto>> ChangeActiveStateFirm(ChangeActiveStateFirmRequestDto changeActiveStateFirmRequest)
        {
            #region response dto oluşturuluyor
            ChangeActiveStateFirmResponseDto changeActiveStateFirmResponse = new ChangeActiveStateFirmResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(changeActiveStateFirmRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : changeActiveStateFirmRequest.Id;
                });
            });
            #endregion
            var firm = await _firmRepository.GetFirm(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region firma güncelleniyor
                _firmRepository.UpdateWithProperties(firm, new Expression<Func<Firm, object>>[] {
                    s => s.IsActive,
                    s => s.UpdatedDate,
                });
                firm.IsActive = !firm.IsActive;
                firm.UpdatedDate = DateTime.Now;
                #endregion
                #region veritabanına kayıt ediliyor
                await _firmRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<ChangeActiveStateFirmResponseDto>(changeActiveStateFirmResponse);
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<ChangeActiveStateFirmResponseDto>(changeActiveStateFirmResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// firma kaldırılıyor
        /// </summary>
        /// <param name="removeFirmRequest"></param>
        /// <returns></returns>
        public async Task<Result<RemoveFirmResponseDto>> RemoveFirm(RemoveFirmRequestDto removeFirmRequest)
        {
            #region response dto oluşturuluyor
            RemoveFirmResponseDto removeFirmResponse = new RemoveFirmResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(removeFirmRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : removeFirmRequest.Id;
                });
            });
            #endregion
            var firm = await _firmRepository.GetFirm(firmFilter).FirstOrDefaultAsync();
            #endregion
            if (firm != null)
            {
                #region firma kaldırılıyor
                _firmRepository.UpdateWithProperties(firm, new Expression<Func<Firm, object>>[] {
                    s => s.Deleted,
                    s => s.UpdatedDate,
                });
                firm.UpdatedDate = DateTime.Now;
                firm.Deleted = true;
                #endregion
                #region veritabanına kayıt ediliyor
                await _firmRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<RemoveFirmResponseDto>(removeFirmResponse);
            }
            else
            {
                return new ErrorResult<RemoveFirmResponseDto>(removeFirmResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
        }
        /// <summary>
        /// firma fiyat bilgisi ekleniyor / güncelleniyor
        /// </summary>
        /// <param name="addOrUpdateFirmPriceRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddOrUpdateFirmPriceResponseDto>> AddOrUpdateFirmPrice(AddOrUpdateFirmPriceRequestDto addOrUpdateFirmPriceRequest)
        {
            #region response dto oluşturuluyor
            AddOrUpdateFirmPriceResponseDto addOrUpdateFirmPriceResponse = new AddOrUpdateFirmPriceResponseDto();
            #endregion
            #region firma getiriliyor
            #region filter dto oluşturuluyor
            FirmFilterDto firmFilter = _mapper.Map<FirmFilterDto>(addOrUpdateFirmPriceRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmFilterDto;
                    destData.Id = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : addOrUpdateFirmPriceRequest.FirmId;
                });
            });
            #endregion
            var firmCheck = await _firmRepository.GetFirm(firmFilter,null).AnyAsync();
            #endregion
            if (firmCheck)
            {
                #region firmaya ait ilgili fiyat bilgisi varsa getiriliyor
                #region filter dto oluştuurluyor
                FirmPriceFilterDto firmPriceFilter = _mapper.Map<FirmPriceFilterDto>(addOrUpdateFirmPriceRequest, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as FirmPriceFilterDto;
                        destData.FirmId = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : addOrUpdateFirmPriceRequest.FirmId;
                    });
                });
                #endregion
                FirmPrice firmPrice = await _firmPriceRepository.GetFirmPrice(firmPriceFilter, null,false).FirstOrDefaultAsync();
                #endregion
                #region firma fiyat bilgisi ekleniyor
                if (firmPrice == null)
                {
                    firmPrice = _mapper.Map<FirmPrice>(addOrUpdateFirmPriceRequest);
                    await _firmPriceRepository.InsertAsync(firmPrice);
                }
                #endregion
                #region firma fiyat bilgisi güncelleniyor
                else
                {
                    _firmPriceRepository.UpdateWithProperties(firmPrice, new Expression<Func<FirmPrice, object>>[] {
                        s => s.Price,
                    });
                    firmPrice.Price = addOrUpdateFirmPriceRequest.Price;
                }
                #endregion
                #region veritabanına kayıt ediliyor
                await _firmPriceRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<AddOrUpdateFirmPriceResponseDto>(addOrUpdateFirmPriceResponse);
            }
            #region firma bulunamadı
            else
            {
                return new ErrorResult<AddOrUpdateFirmPriceResponseDto>(addOrUpdateFirmPriceResponse, FirmManagmentErrorEnum.FIRM_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// firma fiyat bilgisi kaldırılıyor
        /// </summary>
        /// <param name="removeFirmPriceRequest"></param>
        /// <returns></returns>
        public async Task<Result<RemoveFirmPriceResponseDto>> RemoveFirmPrice(RemoveFirmPriceRequestDto removeFirmPriceRequest)
        {
            #region response dto oluşturuluyor
            RemoveFirmPriceResponseDto removeFirmPriceResponse = new RemoveFirmPriceResponseDto();
            #endregion
            #region firma fiyatı getiriliyor
            #region filter dto oluşturuluyor
            FirmPriceFilterDto firmPriceFilter = _mapper.Map<FirmPriceFilterDto>(removeFirmPriceRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as FirmPriceFilterDto;
                    destData.FirmId = _adminContextProvider.FirmId != null ? _adminContextProvider.FirmId : null;
                });
            });
            #endregion
            var firmPrice = await _firmPriceRepository.GetFirmPrice(firmPriceFilter, null,false).FirstOrDefaultAsync();
            #endregion
            if (firmPrice != null)
            {
                #region firma fiyat bilgisi kaldırılıyor
                _firmPriceRepository.UpdateWithProperties(firmPrice, new Expression<Func<FirmPrice, object>>[] {
                        s => s.Deleted,
                    });
                firmPrice.Deleted = true;
                #endregion
                #region veritabanına kayıt ediliyor
                await _firmPriceRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<RemoveFirmPriceResponseDto>(removeFirmPriceResponse);
            }
            #region firma fiyatı bulunamadı
            else
            {
                return new ErrorResult<RemoveFirmPriceResponseDto>(removeFirmPriceResponse, FirmManagmentErrorEnum.FIRM_PRICE_NOT_FOUND_ERROR);
            }
            #endregion
        }
        public async Task<Result<UploadFileResponseDto>> AddFirmLogo(IFormFile file, HttpRequest request)
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
    }
}
