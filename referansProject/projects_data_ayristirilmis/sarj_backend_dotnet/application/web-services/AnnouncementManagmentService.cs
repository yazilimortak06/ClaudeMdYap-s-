using AutoMapper;
using FrameworkCore.Bases.BaseServices;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Domain.Dto.ApiDto.AnnouncementDtos;
using Shared.Domain.Dto.ApiDto.CampaignContentDtos;
using Shared.Domain.Dto.ApiDto.CampaignDtos;
using Shared.Domain.Dto.ApiDto.CampaignPictureDtos;
using Shared.Domain.Dto.ApiDto.ContentLanguageDtos;
using Shared.Domain.Dto.ApiDto.PanelAnnouncementDtos;
using Shared.Domain.Dto.ApiDto.PanelCampaignDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.Entities.ApiEntities.AnnouncementModule;
using Shared.Domain.Entities.ApiEntities.CampaignModule;
using Shared.Domain.Entities.ApiEntities.MessageModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Errors.WebPanelErrors;
using Shared.Domain.HttpClients.HttpClientInterfaces.FileApiInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.AnnouncementRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.ContentLanguageRepositoryInterfaces;
using Shared.Domain.RepositoryInterfaces.ApiRepositoryInterfaces.MessageRepositoryInterfaces;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.AnnouncementManagmentServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.Services.PanelServices.AnnouncementManagment
{
    public class AnnouncementManagmentService : BaseService, IAnnouncementManagmentService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IAnnouncementPictureRepository _announcementPictureRepository;
        private readonly IAnnouncementContentRepository _announcementContentRepository;
        private readonly IContentLanguageRepository _contentLanguageContentRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IFileClientService _fileClientService;
        //private readonly IAnnouncementNotificationClientService _announcementNotificationClientService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnnouncementManagmentService> _logger;

        public AnnouncementManagmentService(IMapper mapper,
                           IAnnouncementContentRepository announcementContentRepository,
                           IAnnouncementPictureRepository announcementPictureRepository,
                           IConfiguration configuration,
                           IContentLanguageRepository contentLanguageContentRepository,
                           IAnnouncementRepository announcementRepository,
                           ILogger<AnnouncementManagmentService> logger,
                           IFileClientService fileClientService,
                           //IAnnouncementNotificationClientService announcementNotificationClientService,
                           IMessageRepository messageRepository) : base(
                           mapper
                               )
        {
            _announcementRepository = announcementRepository;
            _announcementContentRepository = announcementContentRepository;
            _announcementPictureRepository = announcementPictureRepository;
            _contentLanguageContentRepository = contentLanguageContentRepository;
            _configuration = configuration;
            _logger = logger;
            _fileClientService = fileClientService;
            //_announcementNotificationClientService = announcementNotificationClientService;
            _messageRepository = messageRepository;
        }

        #region ekleme,güncelleme,silme
        /// <summary>
        /// Duyuru Güncelleme Formu İçin kampanyalar Çekilir
        /// </summary>
        /// <param name="getPanelAnnouncementForUpdateRequest"></param>
        /// <returns></returns>
        public async Task<Result<GetPanelAnnouncementForUpdateResponseDto>> GetAnnouncementForUpdate(GetPanelAnnouncementForUpdateRequestDto getPanelAnnouncementForUpdateRequest)
        {
            #region response dto oluşturuluyor
            GetPanelAnnouncementForUpdateResponseDto getPanelAnnouncementForUpdateResponse = new GetPanelAnnouncementForUpdateResponseDto();
            getPanelAnnouncementForUpdateResponse.AnnouncementContentFormInput = new List<PanelAnnouncementContentFormInputDto>();
            getPanelAnnouncementForUpdateResponse.AnnouncementContent = new List<AnnouncementContentDto>();
            #endregion
            #region kampanya getiriliyor ve response dto ya mapleniyor
            #region filter dto oluşturuluyor
            AnnouncementFilterDto announcementFilter = _mapper.Map<AnnouncementFilterDto>(getPanelAnnouncementForUpdateRequest);
            #endregion
            var announcement = await _announcementRepository.GetAnnouncementsAsNoTracking(announcementFilter).FirstOrDefaultAsync();
            #endregion
            if (announcement != null)
            {
                #region duyuru response dto ya mapleniyor
                getPanelAnnouncementForUpdateResponse.Announcement = _mapper.Map<AnnouncementDto>(announcement);
                #endregion
                #region duyuru content response dto ya mapleniyor
                #region duyuru content getiriliyor
                #region filter dto oluşturuluyor
                AnnouncementContentFilterDto announcementContentFilter = _mapper.Map<AnnouncementContentFilterDto>(getPanelAnnouncementForUpdateRequest);
                #endregion
                var announcementContentEntity = await _announcementContentRepository.GetAnnouncementContentForPanelForm(announcementContentFilter).ToListAsync();
                #endregion
                getPanelAnnouncementForUpdateResponse.AnnouncementContent = _mapper.Map<List<AnnouncementContentDto>>(announcementContentEntity);
                getPanelAnnouncementForUpdateResponse.AnnouncementContentFormInput = await GetContentLanguageWithAnnouncementContent(getPanelAnnouncementForUpdateRequest.AnnouncementId).ToListAsync();
                #endregion
                return new SuccessResult<GetPanelAnnouncementForUpdateResponseDto>(getPanelAnnouncementForUpdateResponse);
            }
            else
            {
                return new ErrorResult<GetPanelAnnouncementForUpdateResponseDto>(getPanelAnnouncementForUpdateResponse, AnnouncementManagmentErrorEnum.ANNOUNCEMENT_NOT_FOUND_ERROR);
            }
        }
        /// <summary>
        /// duyuru güncelleniyor
        /// </summary>
        /// <param name="updateAnnouncementRequest"></param>
        /// <returns></returns>
        public async Task<Result<UpdateAnnouncementResponseDto>> UpdateAnnouncement(UpdateAnnouncementRequestDto updateAnnouncementRequest)
        {
            #region response dto oluşturuluyor
            UpdateAnnouncementResponseDto updateAnnouncementResponse = new UpdateAnnouncementResponseDto();
            #endregion
            #region duyuru kontrol ediliyor
            #region filter dto oluşturuluyor
            AnnouncementFilterDto announcementFilter = new AnnouncementFilterDto();
            announcementFilter.Id = updateAnnouncementRequest.Announcement.Id;
            #endregion
            var announcementCheck = await _announcementRepository.GetAnnouncementsAsNoTracking(announcementFilter).AnyAsync();
            #endregion
            if (announcementCheck)
            {
                #region duyuru mapleniyor
                var announcementEntity = _mapper.Map<Announcement>(updateAnnouncementRequest.Announcement, opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        var destData = dest as Announcement;
                        if (destData.ValidityDate != null)
                        {
                        }
                    });
                });
                #endregion
                #region güncellenen ve eklenen duyuru contentleri mapleniyor
                var announcementContentEntityUpdate = _mapper.Map<List<AnnouncementContent>>(updateAnnouncementRequest.AnnouncementContent.Where(x => x.Id != 0).ToList());
                var announcementContentEntityInsert = _mapper.Map<List<AnnouncementContent>>(updateAnnouncementRequest.AnnouncementContent.Where(x => x.Id == 0).ToList());
                #endregion
                #region eklenen ve silinen duyuru resimleri mapleniyor
                var announcementPictureEntityInsert = _mapper.Map<List<AnnouncementPicture>>(updateAnnouncementRequest.AnnouncementPicturesInserted);
                var announcementPictureEntityDelete = _mapper.Map<List<AnnouncementPicture>>(updateAnnouncementRequest.AnnouncementPicturesDeleted);
                #endregion
                #region duyuru güncelleniyor
                _announcementRepository.UpdateWithProperties(announcementEntity, new Expression<Func<Announcement, object>>[] {
                    s => s.ValidityDate,
                });
                #endregion
                #region duyuru contenti güncelleniyor
                _announcementContentRepository.UpdateWithProperties(announcementContentEntityUpdate.ToArray(), new Expression<Func<AnnouncementContent, object>>[] {
                    s => s.Title,
                    s => s.Content,
                    s => s.HtmlContent,
                });
                #endregion
                #region veri tabanına kayıt ediliyor
                _announcementContentRepository.Insert(announcementContentEntityInsert);
                _announcementPictureRepository.Insert(announcementPictureEntityInsert);
                _announcementPictureRepository.DeleteWithStateRange(announcementPictureEntityDelete);
                await _announcementRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<UpdateAnnouncementResponseDto>(updateAnnouncementResponse);
            }
            #region duyuru bulunamadı
            else
            {
                return new ErrorResult<UpdateAnnouncementResponseDto>(updateAnnouncementResponse, AnnouncementManagmentErrorEnum.ANNOUNCEMENT_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// duyuru ekleniyor
        /// </summary>
        /// <param name="addAnnouncementRequest"></param>
        /// <returns></returns>
        public async Task<Result<AddAnnouncementResponseDto>> AddAnnouncement(AddAnnouncementRequestDto addAnnouncementRequest)
        {
            #region response dto oluşturuluyor
            AddAnnouncementResponseDto addAnnouncementResponse = new AddAnnouncementResponseDto();
            #endregion
            #region kampanya mapleniyor
            var announcement = _mapper.Map<Announcement>(addAnnouncementRequest, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    var destData = dest as Announcement;
                    destData.CreatedDate = DateTime.Now;
                    destData.State = true;
                    destData.Type = AnnouncementTypeEnum.GENERAL;
                });
            });
            #endregion
            #region veritabanına ekleniyor
            await _announcementRepository.InsertAsync(announcement);
            await _announcementRepository.SaveChangesAsync();
            #endregion
            //#region notification apiye istek atılarak yeni eklenen duyuru signalr ile iletiliyor
            //#region request dto oluşturuluyor
            //#region uygulama için varsayılan dil getiriliyor
            //#region filter dto oluşturuluyor
            //ContentLanguageFilterDto contentLanguageFilter = new ContentLanguageFilterDto();
            //contentLanguageFilter.IsDefault = true;
            //#endregion
            //var contentLanguage = await _contentLanguageContentRepository.GetContentLanguagesAsNoTracking(contentLanguageFilter).FirstOrDefaultAsync();
            //#endregion
            //AnnouncementNotificationDto announcementNotification = new AnnouncementNotificationDto();
            //announcementNotification.CreatedDate = announcementEntity.CreatedDate.ToClientDateAndHourString();
            //announcementNotification.Id = insertedAnnouncement.Id;
            //if (contentLanguage != null)
            //{
            //    var insertedAnnouncementContent = insertedAnnouncement.AnnouncementContent.Where(x => x.ContentLanguageId == contentLanguage.Id).FirstOrDefault();
            //    var insertedAnnouncementPicture = insertedAnnouncement.AnnouncementPicture.OrderBy(x => x.Id).ToList().FirstOrDefault();
            //    if (insertedAnnouncementContent != null)
            //    {
            //        announcementNotification.Title = insertedAnnouncementContent.Title;
            //        announcementNotification.Content = insertedAnnouncementContent.Content;
            //    }
            //    if (insertedAnnouncementPicture != null)
            //    {
            //        announcementNotification.MediaGuiId = insertedAnnouncementPicture.MediaGuiId;
            //    }
            //}
            //#endregion
            //await _announcementNotificationClientService.SendAll(announcementNotification);
            //#endregion
            return new SuccessResult<AddAnnouncementResponseDto>(addAnnouncementResponse);
        }
        #endregion
        /// <summary>
        /// panel için duyuru verileri çekiliyor
        /// </summary>
        /// <param name="dataTableFilterModel"></param>
        /// <returns></returns>
        public async Task<Result<DataTableResponseWrapper<PanelAnnouncementListItemDto>>> GetAnnouncementDataTablePanel(DataTableFilterModel<GetPanelAnnouncementDataTableRequestDto> dataTableFilterModel)
        {
            #region duyuru sayısını tutan değişken tanımlanıyor
            int toplamKayit = 0;
            #endregion
            #region duyurular getiriliyor
            #region paging verileri setleniyor
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;
            #endregion
            var announcements = GetAnnouncementListForPanel(dataTableFilterModel.data);
            var resultAnnouncementn = announcements.ApplySortingDto(dataTableFilterModel.orderProperty, dataTableFilterModel.orderDirective).AsNoTracking();
            #endregion
            #region response dto setleniyor
            toplamKayit = resultAnnouncementn.Count();
            var announcementList = await resultAnnouncementn.Skip(ofset).Take(recordPerPage).ToListAsync();
            var result = new DataTableResponseWrapper<PanelAnnouncementListItemDto>(toplamKayit, announcementList);
            #endregion
            return new SuccessResult<DataTableResponseWrapper<PanelAnnouncementListItemDto>>(result);
        }
        /// <summary>
        /// duyuru durumu güncelleniyor
        /// </summary>
        /// <param name="changeAnnouncementStateRequest"></param>
        /// <returns></returns>
        public async Task<Result<ChangeAnnouncementStateResponseDto>> ChangeAnnouncementState(ChangeAnnouncementStateRequestDto changeAnnouncementStateRequest)
        {
            #region response dto oluşturuluyor
            ChangeAnnouncementStateResponseDto changeAnnouncementStateResponse = new ChangeAnnouncementStateResponseDto();
            #endregion
            #region duyuru getiriliyor
            #region filter dto oluşturuluyor
            AnnouncementFilterDto announcementFilter = _mapper.Map<AnnouncementFilterDto>(changeAnnouncementStateRequest);
            #endregion
            var announcementEntity = await _announcementRepository.GetAnnouncements(announcementFilter).FirstOrDefaultAsync();
            #endregion
            if (announcementEntity != null)
            {
                #region kampanya güncelleniyor
                _announcementRepository.UpdateWithProperties(announcementEntity, new Expression<Func<Announcement, object>>[] {
                    s => s.State
                });
                announcementEntity.State = !announcementEntity.State;
                await _announcementRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<ChangeAnnouncementStateResponseDto>(changeAnnouncementStateResponse);
            }
            #region kampanya bulunamadı
            else
            {
                return new ErrorResult<ChangeAnnouncementStateResponseDto>(changeAnnouncementStateResponse, AnnouncementManagmentErrorEnum.ANNOUNCEMENT_NOT_FOUND_ERROR);
            }
            #endregion
        }
        /// <summary>
        /// duyuru kaldırılıyor
        /// </summary>
        /// <param name="removeAnnouncementRequest"></param>
        /// <returns></returns>
        public async Task<Result<RemoveAnnouncementResponseDto>> RemoveAnnouncement(RemoveAnnouncementRequestDto removeAnnouncementRequest)
        {
            #region response dto oluşturuluyor
            RemoveAnnouncementResponseDto removeAnnouncementResponse = new RemoveAnnouncementResponseDto();
            #endregion
            #region duyuru getiriliyor
            #region filter dto oluşturuluyor
            AnnouncementFilterDto announcementFilter = new AnnouncementFilterDto();
            announcementFilter.Id = removeAnnouncementRequest.Id;
            #endregion
            var announcementEntity = await _announcementRepository.GetAnnouncements(announcementFilter).FirstOrDefaultAsync();
            #endregion
            if (announcementEntity != null)
            {
                #region duyuru kaldırılıyor
                _announcementRepository.UpdateWithProperties(announcementEntity, new Expression<Func<Announcement, object>>[] {
                    s => s.Deleted
                });
                announcementEntity.Deleted = true;
                await _announcementRepository.SaveChangesAsync();
                #endregion
                return new SuccessResult<RemoveAnnouncementResponseDto>(removeAnnouncementResponse);
            }
            #region duyuru bulunamadı
            else
            {
                return new ErrorResult<RemoveAnnouncementResponseDto>(removeAnnouncementResponse, AnnouncementManagmentErrorEnum.ANNOUNCEMENT_NOT_FOUND_ERROR);
            }
            #endregion
        }
        public async Task<Result<UploadFileResponseDto>> AddAnnouncementPicture(IFormFile file, HttpRequest request)
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
        /// genel duyuru gönderiliyor 
        /// </summary>
        /// <param name="sendAnnouncementGeneralMessageRequest"></param> 
        /// <returns></returns>
        public async Task<Result<SendAnnouncementGeneralMessageResponseDto>> SendAnnouncementGeneralMessage(SendAnnouncementGeneralMessageRequestDto sendAnnouncementGeneralMessageRequest)
        {
            #region response dto oluşturuluyor
            SendAnnouncementGeneralMessageResponseDto sendAnnouncementGeneralMessageResponse = new SendAnnouncementGeneralMessageResponseDto();
            #endregion
            #region message entity oluşturuluyor
            Message message = _mapper.Map<Message>(sendAnnouncementGeneralMessageRequest);
            #endregion
            #region duyuru seçili değil, announcement oluşturuluyor
            if (sendAnnouncementGeneralMessageRequest.AnnouncementId == null)
            {
                Announcement announcement = new Announcement();
                announcement.CreatedDate = DateTime.Now;
                announcement.State = true;
                announcement.Type = AnnouncementTypeEnum.GENERAL;
                announcement.ValidityDate = sendAnnouncementGeneralMessageRequest.ValidityDate;
                #region announcement, message tablosuna ekleniyor
                message.Announcement = announcement;
                #endregion
            }
            #endregion
            #region veri tabanına kayıt ediliyor
            await _messageRepository.InsertAsync(message);
            #endregion
            return new SuccessResult<SendAnnouncementGeneralMessageResponseDto>(sendAnnouncementGeneralMessageResponse);
        }

        #region private methods
        private IQueryable<PanelAnnouncementListItemDto> GetAnnouncementListForPanel(GetPanelAnnouncementDataTableRequestDto getPanelAnnouncementDataTableRequest)
        {
            var announcementContentFilter = _mapper.Map<AnnouncementContentFilterDto>(getPanelAnnouncementDataTableRequest);
            var announcementResult = _announcementContentRepository.GetAnnouncementWithContent(announcementContentFilter);
            var pictureResult = _announcementPictureRepository.GetAnnouncementPictures(new AnnouncementPictureFilterDto { });
            var announcementQuery = (from announcement in announcementResult
                                     from pictures in pictureResult.Where(x => x.AnnouncementId == announcement.AnnouncementId).OrderBy(x => x.Id).DefaultIfEmpty().Take(1)
                                     select new PanelAnnouncementListItemDto
                                     {
                                         Id = announcement.AnnouncementId,
                                         Title = announcement.Title,
                                         Content = announcement.Content.Length > 150 ? announcement.Content.Substring(0, 150) + "..." : announcement.Content,
                                         State = announcement.Announcement.State,
                                         MediaGuiId = pictures.MediaGuiId
                                     });
            return announcementQuery;
        }
        /// <summary>
        /// duyuru güncelleme formu için duyuru content ve  content language çekiliyor
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private IQueryable<PanelAnnouncementContentFormInputDto> GetContentLanguageWithAnnouncementContent(long announcementId)
        {
            var contentLanguageList = _contentLanguageContentRepository.GetContentLanguages();
            var announcementContentList = _announcementContentRepository.GetAnnouncementContentForPanelForm(new AnnouncementContentFilterDto() { AnnouncementId = announcementId });
            var query = (from contentLanguage in contentLanguageList
                         join announcementContent in announcementContentList
                          on new { A = contentLanguage.Id, B = announcementId }
                                equals new { A = announcementContent.ContentLanguageId, B = announcementContent.AnnouncementId }
                          into announcementContents
                         from announcementContent in announcementContents.DefaultIfEmpty()
                         select new PanelAnnouncementContentFormInputDto
                         {
                             ContentLanguageId = contentLanguage.Id,
                             Description = contentLanguage.Description,
                             Content = contentLanguage.Content,
                             AnnouncementId = announcementContent.AnnouncementId,
                             AnnouncementContentId = announcementContent.Id,
                             AnnouncementContent = announcementContent.Content,
                             AnnouncementTitle = announcementContent.Title,
                             AnnouncementHtmlContent = announcementContent.HtmlContent,
                             IsDefault = contentLanguage.IsDefault
                         });
            return query;
        }
        #endregion
    }

}
