// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Application\Services\Places\PlaceService.cs
using AutoMapper;
using FrameworkCore.Bases.BaseService;
using FrameworkCore.FrameworkCore.Auth;
using FrameworkCore.FrameworkCore.Logging;
using FrameworkCore.WrapperCore.Models;
using PixdinnCrm.Domain.Dto.PlaceDtos;
using PixdinnCrm.Domain.Interfaces.Repositories;
using PixdinnCrm.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixdinnCrm.Domain.Entities.PlaceEntities;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using PixdinnCrm.Domain.Enums.ErrorEnums;
using FrameworkCore.Enums;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PixdinnCrm.Domain.Dto;
using PixdinnCrm.Domain.Interfaces.Repositories.PlaceLanguageRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.PlaceRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.ContentLanguageRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.PlaceServiceInterfaces;
using FrameworkCore.FrameworkCore.Extentions;

namespace PixdinnCrm.Application.Services
{
    public class PlaceService : BaseService, IPlaceService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IPlaceLanguageRepository _placeLanguageRepository;
        private readonly IContentLanguageRepository _contentLanguageRepository;
        private readonly IPlacePictureRepository _placePictureRepository;

        public PlaceService(
            IMapper mapper,
            ILoggerService logger,
            IAuthorizeService authorizeService,
            IAuthonticationService authonticationService,
            IPlaceRepository placeRepository,
            IPlaceLanguageRepository placeLanguageRepository,
            IContentLanguageRepository contentLanguageRepository,
            IPlacePictureRepository placePictureRepository
            )
            : base(mapper, logger, authorizeService, authonticationService)
        {
            _placeRepository = placeRepository;
            _placeLanguageRepository = placeLanguageRepository;
            _contentLanguageRepository = contentLanguageRepository;
            _placePictureRepository = placePictureRepository;
        }

        /// <summary>
        /// yeni place oluşturur
        /// </summary>
        public async Task<Result<bool>> CreateNewPlace(PlaceInsertDto placeUpdateRequestDto)
        {
            var placeToBeAdded = _mapper.Map<Place>(placeUpdateRequestDto);
            var insertedPlace = await _placeRepository.InsertAsync(placeToBeAdded);
            var effectedRowCount = await _placeRepository.SaveChangesAsync();
            if (effectedRowCount > 0)
            {
                return new SuccessResult<bool>(true);
            }
            else
            {
                return new ErrorResult<bool>(false, PlaceErrorEnum.PLACE_INSERT_ERROR);
            }
        }

        /// <summary>
        /// place günceller
        /// </summary>
        public async Task<Result<bool>> UpdatePlace(PlaceUpdateRequestDto placeUpdateRequestDto)
        {
            var placeEntity = _mapper.Map<Place>(placeUpdateRequestDto);
            _placeRepository.Update(placeEntity, UpdateStrategy.OnlyMain);
            await _placeRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        /// <summary>
        /// place panel tablosunu getirir
        /// </summary>
        public Result<DataTableResponseWrapper<PlaceDataTableDto>> GetPlacesForDataTable(DataTableFilterModel<PlaceFilterDto> placeFilterDto)
        {
            int toplamKayit = 0;
            var recordPerPage = placeFilterDto.recordPerPage.GetValueOrDefault();
            var pageNumber = placeFilterDto.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;

            var placesEntityFilter = _placeRepository.GetPlaces(placeFilterDto.data);
            var productResult = placesEntityFilter.ApplySorting(placeFilterDto.orderProperty, placeFilterDto.orderDirective).AsNoTracking();
            toplamKayit = placesEntityFilter.Count();

            var placesEntity = placesEntityFilter.Skip(ofset).Take(recordPerPage).ToList();

            if (placesEntity != null)
            {
                var placesDto = _mapper.Map<List<PlaceDataTableDto>>(placesEntity);
                var result = new DataTableResponseWrapper<PlaceDataTableDto>(toplamKayit, placesDto);
                return new SuccessResult<DataTableResponseWrapper<PlaceDataTableDto>>(result);
            }
            else
            {
                return new SuccessResult<DataTableResponseWrapper<PlaceDataTableDto>>(null);
            }
        }

        /// <summary>
        /// id'ye göre place getirir
        /// </summary>
        public async Task<Result<PlaceDto>> GetPlaceById(long id)
        {
            var placesEntity = await _placeRepository.GetPlaceById(id).FirstOrDefaultAsync();
            var placeDto = _mapper.Map<PlaceDto>(placesEntity);
            return new SuccessResult<PlaceDto>(placeDto);
        }

        /// <summary>
        /// place güncellemesi için place verilerini getirir
        /// </summary>
        public async Task<Result<PlaceUpdateRequestDto>> GetPlaceByIdForUpdateRequest(long id)
        {
            var placeEntity = await _placeRepository.GetPlaceById(id).FirstOrDefaultAsync();
            var placePictures = _placePictureRepository.GetPlacePictures(new PlacePictureFilterDto() { PlaceId = id });
            var placeDto = _mapper.Map<PlaceUpdateRequestDto>(placeEntity);
            placeDto.PlacePictureFormInput = await GetPlacePictureFormInput(new PlacePictureFilterDto() { PlaceId = id });
            return new SuccessResult<PlaceUpdateRequestDto>(placeDto);
        }

        #region private methods
        /// <summary>
        /// Mekan güncellemesi formu için init olacak resim verilerini döndürür
        /// </summary>
        private async Task<List<PlacePictureFormInputDto>> GetPlacePictureFormInput(PlacePictureFilterDto placePictureFilter)
        {
            var placePictureQuery = _placePictureRepository.GetPlacePictures(placePictureFilter);

            var placePictureQueryList = (from placePicture in placePictureQuery
                                         select new PlacePictureFormInputDto
                                         {
                                             Id = placePicture.Id,
                                             code = placePicture.MediaGuiId,
                                             PlaceId = placePicture.PlaceId
                                         });

            var placePictureList = await placePictureQueryList.ToListAsync();
            return placePictureList;
        }
        #endregion
    }
}
