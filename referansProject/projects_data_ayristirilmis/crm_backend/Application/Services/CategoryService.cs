// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Application\Services\Category\CategoryService.cs
using AutoMapper;
using FrameworkCore.Bases.BaseService;
using FrameworkCore.FrameworkCore.Auth;
using FrameworkCore.FrameworkCore.Logging;
using FrameworkCore.WrapperCore.Models;
using Microsoft.EntityFrameworkCore;
using PixdinnCrm.Domain.Dto.CategoryDtos;
using PixdinnCrm.Domain.Interfaces.Repositories;
using PixdinnCrm.Domain.Interfaces.Services;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixdinnCrm.Domain.Entities.CategoryEntities;
using System.Linq.Expressions;
using System.Reflection;
using FrameworkCore.Enums;
using PixdinnCrm.Domain.Enums.ErrorEnums;
using PixdinnCrm.Domain.Models.Categories;
using PixdinnCrm.Domain.Enums.DbEnums;
using PixdinnCrm.Domain.Interfaces.Repositories.CategoryRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.CategoryServiceInterfaces;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.Utils.EntityUtils;
using PixdinnCrm.Domain.Interfaces.Repositories.PlaceRepositoryInterfaces;
using PixdinnCrm.Domain.Dto.PlaceDtos;
using PixdinnCrm.Domain.Interfaces.Repositories.ContentLanguageRepositoryInterfaces;
using PixdinnCrm.Domain.Dto.ContentLanguageDtos;
using PixdinnCrm.Domain.Dto.CategoryContentDtos;
using PixdinnCrm.Domain.Interfaces.Repositories.MediaFileRepositoryInterfaces;

namespace PixdinnCrm.Application.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryContentRepository _categoryContentRepository;
        private readonly ICategoryPictureRepository _categoryPictureRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IContentLanguageRepository _contentLanguageRepository;
        private readonly IMediaFileRepository _mediaFileRepository;

        public CategoryService(IMapper mapper,
                           ILoggerService logger,
                           IAuthorizeService authorizeService,
                           IAuthonticationService authonticationService,
                           ICategoryRepository categoryRepository,
                           ICategoryContentRepository categoryContentRepository,
                           ICategoryPictureRepository categoryPictureRepository,
                           IPlaceRepository placeRepository,
                           IContentLanguageRepository contentLanguageRepository,
                           IMediaFileRepository mediaFileRepository) :
            base(mapper, logger, authorizeService, authonticationService)
        {
            _categoryRepository = categoryRepository;
            _categoryContentRepository = categoryContentRepository;
            _categoryPictureRepository = categoryPictureRepository;
            _placeRepository = placeRepository;
            _contentLanguageRepository = contentLanguageRepository;
            _mediaFileRepository = mediaFileRepository;
        }

        public async Task<Result<bool>> AddNewCategory(CategoryAddDto dto)
        {
            var categoryEntity = _mapper.Map<Category>(dto);
            _categoryRepository.Insert(categoryEntity, InsertStrategy.InsertAll);
            var result = await _categoryRepository.SaveChangesAsync();
            if (result < 0)
            {
                return new ErrorResult<bool>(false, CategoryErrorEnum.CATEGORY_INSERT_ERROR);
            }
            return new SuccessResult<bool>(true);
        }

        public Result<DataTableResponseWrapper<CategoryDataTableDto>> GetCategoryByDatatableDto(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
        {
            var caregoryListFilterDto = _mapper.Map<CategoryListPanelFilterDto>(dto.data);
            var queryJoinedTables = GetListCategoryWithContent(caregoryListFilterDto);

            #region pagination işlemi başlıyor
            queryJoinedTables = queryJoinedTables.ApplySorting(dto.orderProperty, dto.orderDirective).AsNoTracking();
            var totalRecordCount = queryJoinedTables.Count();
            var recordPerPage = dto.recordPerPage.GetValueOrDefault();
            var pageNumber = dto.pageNumber.GetValueOrDefault();
            int offset = pageNumber * recordPerPage;
            queryJoinedTables = queryJoinedTables.Skip(offset).Take(recordPerPage);
            #endregion

            var categoryListModelList = queryJoinedTables.ToList();
            var categoryWithContentDtos = _mapper.Map<List<CategoryDataTableDto>>(categoryListModelList);
            var result = new DataTableResponseWrapper<CategoryDataTableDto>(totalRecordCount, categoryWithContentDtos);
            return new SuccessResult<DataTableResponseWrapper<CategoryDataTableDto>>(result);
        }

        public async Task<Result<CategoryUpdateRequestDto>> GetCategoryById(long Id)
        {
            var categoryUpdateModel = new CategoryUpdateModel();
            categoryUpdateModel.CategoryContents = new List<CategoryContent>();
            categoryUpdateModel.CategoryPictures = new List<CategoryPicture>();

            var categoryEntity = await _categoryRepository.FirstOrDefaultAsync(a => a.Id == Id);

            if (categoryEntity != null)
            {
                categoryUpdateModel.CategoryPictures = _categoryPictureRepository.GetCategoryPictures(
                    new CategoryPictureFilterDto { CategoryId = Id }).ToList();
                categoryUpdateModel.CategoryContents = _categoryContentRepository.GetContents(
                    new CategoryContentFilterDto { CategoryId = Id }).ToList();

                var categoryUpdateRequestDto = _mapper.Map<CategoryUpdateRequestDto>(categoryUpdateModel);
                categoryUpdateRequestDto.CategoryPictureUpdateFormDtos = await GetCategoryPictureFormInput(new CategoryPictureFilterDto() { CategoryId = Id });
                categoryUpdateRequestDto.CategoryUpdateFormInputDtos = await GetContentLanguageWithCategoryContent(Id).ToListAsync();

                return new SuccessResult<CategoryUpdateRequestDto>(categoryUpdateRequestDto);
            }
            return new ErrorResult<CategoryUpdateRequestDto>(null, CategoryErrorEnum.CATEGORY_ID_ERROR);
        }

        public async Task<Result<bool>> DeleteCategoryById(long id)
        {
            _categoryRepository.Delete(id);
            var result = await _categoryRepository.SaveChangesAsync();
            if (result == 1)
            {
                return new SuccessResult<bool>(true);
            }
            else
            {
                return new ErrorResult<bool>(false, CategoryErrorEnum.CATEGORY_DELETE_ERROR);
            }
        }

        public async Task<Result<bool>> UpdateCategory(CategoryUpdateRequestDto dto)
        {
            var categoryContentEntitiesToInsert = _mapper.Map<List<CategoryContent>>(dto.CategoryContents.Where(x => x.Id == 0).ToList());
            var categoryContentEntitiesToUpdate = _mapper.Map<List<CategoryContent>>(dto.CategoryContents.Where(x => x.Id != 0)).ToList();
            var categoryPicturesToInsert = _mapper.Map<List<CategoryPicture>>(dto.CategoryPictures);
            var categoryPicturesToDelete = _mapper.Map<List<CategoryPicture>>(dto.CategoryPicturesDeleted);

            _categoryContentRepository.Insert(categoryContentEntitiesToInsert);
            _categoryPictureRepository.Insert(categoryPicturesToInsert);
            _categoryPictureRepository.Delete(categoryPicturesToDelete);
            _categoryContentRepository.UpdateWithProperties(categoryContentEntitiesToUpdate.ToArray(), new Expression<Func<CategoryContent, object>>[] {
                cc => cc.Name
            });

            await _categoryRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        public async Task<List<CategoryDataTableDto>> GetListCategoryForMenu(long placeId)
        {
            var categoryList = await GetListCategoryWithContent(new CategoryListPanelFilterDto() { PlaceId = placeId }).ToListAsync();
            var categoryListDto = _mapper.Map<List<CategoryDataTableDto>>(categoryList);
            return categoryListDto;
        }

        #region private methods
        private IQueryable<CategoryUpdateFormInputDto> GetContentLanguageWithCategoryContent(long categoryId)
        {
            var contentLanguageList = _contentLanguageRepository.GetContentLanguages(new ContentLanguageFilterDto() { });
            var categoryContentList = _categoryContentRepository.GetContents(new CategoryContentFilterDto() { CategoryId = categoryId });
            var categoryContentQuery = (from contentLanguage in contentLanguageList
                                        join categoryContent in categoryContentList
                                        on new { A = contentLanguage.Id, B = categoryId }
                                              equals new { A = categoryContent.LanguageId, B = categoryContent.CategoryId }
                                        into productContents
                                        from categoryContent in productContents.DefaultIfEmpty()
                                        select new CategoryUpdateFormInputDto
                                        {
                                            LanguageId = contentLanguage.Id,
                                            CategoryContentId = categoryContent.Id,
                                            CategoryId = categoryContent.CategoryId,
                                            LanguageName = contentLanguage.Name,
                                            CategoryContentName = categoryContent.Name,
                                            Prefix = contentLanguage.Prefix,
                                        });
            return categoryContentQuery;
        }

        private async Task<List<CategoryPictureUpdateFormDto>> GetCategoryPictureFormInput(CategoryPictureFilterDto categoryPictureFilter)
        {
            var categoryPictureQuery = _categoryPictureRepository.GetCategoryPictures(categoryPictureFilter);
            var categoryPictureListQuery = (from categoryPicture in categoryPictureQuery
                                            select new CategoryPictureUpdateFormDto
                                            {
                                                Id = categoryPicture.Id,
                                                Code = categoryPicture.MediaGuiId,
                                                CategoryId = categoryPicture.CategoryId
                                            });
            var categoryPictureList = await categoryPictureListQuery.ToListAsync();
            return categoryPictureList;
        }

        private IQueryable<CategoryListModel> GetListCategoryWithContent(CategoryListPanelFilterDto categoryListPanelFilterDto)
        {
            var categoryWithContentFilterDto = _mapper.Map<CategoryWithContentFilterDto>(categoryListPanelFilterDto);
            var placeFilterDto = _mapper.Map<PlaceFilterDto>(categoryListPanelFilterDto);
            var categoryPictureFilterDto = _mapper.Map<CategoryPictureFilterDto>(categoryListPanelFilterDto);

            var queryCategoriesWithContent = _categoryContentRepository.GetContentWithCategory(categoryWithContentFilterDto);
            var queryPlace = _placeRepository.GetPlaces(placeFilterDto);
            var queryCategoryPicture = _categoryPictureRepository.GetCategoryPictures(categoryPictureFilterDto);

            var categoryList = (from queryCategoriesWithContentElement in queryCategoriesWithContent
                                join categoryPicture in queryCategoryPicture
                                on queryCategoriesWithContentElement.CategoryId equals categoryPicture.CategoryId
                                into categoryPictures
                                from categoryPicture in categoryPictures.DefaultIfEmpty()
                                join place in queryPlace
                                on queryCategoriesWithContentElement.Category.PlaceId equals place.Id
                                into places
                                from place in places.DefaultIfEmpty()
                                select new CategoryListModel
                                {
                                    Id = queryCategoriesWithContentElement.Category.Id,
                                    CategoryName = queryCategoriesWithContentElement.Name,
                                    PlaceEmail = place.Email,
                                    PlaceName = place.Name,
                                    PlaceId = place.Id,
                                    LanguageId = queryCategoriesWithContentElement.LanguageId
                                });

            return categoryList;
        }
        #endregion
    }
}
