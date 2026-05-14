// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Application\Services\Product\ProductService.cs
using AutoMapper;
using Domain.Entities;
using Domain.Entities.ProductEntities;
using Domain.Interfaces.Services;
using Domain.Interfaces.Services.ProductServiceInterfaces;
using FrameworkCore.Bases.BaseService;
using FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.Auth;
using FrameworkCore.FrameworkCore.Extentions;
using FrameworkCore.FrameworkCore.FileApi.FileApiModel;
using FrameworkCore.FrameworkCore.Logging;
using FrameworkCore.FrameworkCore.Refit.Service;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.EntityUtils;
using FrameworkCore.Utils.Interface;
using FrameworkCore.WrapperCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PixdinnCrm.Domain.Dto;
using PixdinnCrm.Domain.Dto.ContentLanguageDtos;
using PixdinnCrm.Domain.Dto.PlaceDtos;
using PixdinnCrm.Domain.Dto.ProductDtos;
using PixdinnCrm.Domain.Entities.FileEntities;
using PixdinnCrm.Domain.Entities.ProductEntities;
using PixdinnCrm.Domain.Enums.DbEnums;
using PixdinnCrm.Domain.Enums.ErrorEnums;
using PixdinnCrm.Domain.Interfaces.Repositories;
using PixdinnCrm.Domain.Interfaces.Repositories.ContentLanguageRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.MediaFileRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.PlaceRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.ProductRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.MediaFileServiceInterfaces;
using PixdinnCrm.Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductContentRepository _productContentRepository;
        private readonly IProductPictureRepository _productPictureRepository;
        private readonly IContentLanguageRepository _contentLanguageRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IUtilService _utilService;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IMediaFileService _mediaFileService;
        private readonly IConfiguration _configuration;

        public ProductService(IMapper mapper,
                           ILoggerService logger,
                           IAuthorizeService authorizeService,
                           IAuthonticationService authonticationService,
                           IProductRepository productRepository,
                           IProductContentRepository productContentRepository,
                           IContentLanguageRepository contentLanguageRepository,
                           IProductPictureRepository productPictureRepository,
                           IPlaceRepository placeRepository,
                           IUtilService utilService,
                           IMediaFileRepository mediaFileRepository,
                           IMediaFileService mediaFileService,
                           IConfiguration configuration) : base(mapper, logger, authorizeService, authonticationService)
        {
            _productRepository = productRepository;
            _productContentRepository = productContentRepository;
            _productPictureRepository = productPictureRepository;
            _contentLanguageRepository = contentLanguageRepository;
            _placeRepository = placeRepository;
            _utilService = utilService;
            _mediaFileRepository = mediaFileRepository;
            _mediaFileService = mediaFileService;
            _configuration = configuration;
        }

        #region ekleme, güncelleme, silme
        /// <summary>
        /// Yeni Ürün Oluşturur
        /// </summary>
        public async Task<Result<bool>> CreateNewProduct(ProductInsertDto productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            var insertedProduct = _productRepository.Insert(productEntity, InsertStrategy.InsertAll);
            await _productRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        /// <summary>
        /// Excel İle Yeni Ürün Oluşturur
        /// </summary>
        public async Task<Result<ExcelProductParseResponseModel>> CreateNewProductExcel(ExcelProductParseRequestModel excelProduct)
        {
            ExcelProductParseResponseModel excelProductParseResponseModel = new ExcelProductParseResponseModel();
            excelProductParseResponseModel.ProductIdList = new List<long>();

            var productDto = _mapper.Map<List<ProductInsertDto>>(excelProduct.ExcelProducts);
            var productEntity = _mapper.Map<List<Product>>(productDto);
            await _productRepository.InsertAsync(productEntity);
            await _productRepository.SaveChangesAsync();
            excelProductParseResponseModel.State = true;

            return new SuccessResult<ExcelProductParseResponseModel>(excelProductParseResponseModel);
        }

        /// <summary>
        /// Ürün Günceller
        /// </summary>
        public async Task<Result<bool>> UpdateProduct(ProductUpdateRequestDto productDto)
        {
            try
            {
                var productEntity = _mapper.Map<Product>(productDto.ProductUpdate);
                _productRepository.Update(productEntity, UpdateStrategy.OnlyMain);

                var productContentEntityUpdate = _mapper.Map<List<ProductContent>>(productDto.ProductContents.Where(x => x.Id != 0).ToList());
                var productContentEntityInsert = _mapper.Map<List<ProductContent>>(productDto.ProductContents.Where(x => x.Id == 0).ToList());
                var productPictureEntityInsert = _mapper.Map<List<ProductPicture>>(productDto.ProductPictures);
                var productPictureEntityDelete = _mapper.Map<List<ProductPicture>>(productDto.ProductPicturesDeleted);

                _productContentRepository.UpdateWithProperties(productContentEntityUpdate.ToArray(), new Expression<Func<ProductContent, object>>[] {
                    s => s.Detail,
                    s => s.Name
                });
                _productContentRepository.Insert(productContentEntityInsert);
                _productPictureRepository.Insert(productPictureEntityInsert);
                _productPictureRepository.DeleteWithStateRange(productPictureEntityDelete);

                await _productRepository.SaveChangesAsync();
            }
            catch (Exception ee) { }

            return new SuccessResult<bool>(true);
        }
        #endregion

        /// <summary>
        /// Ürün Güncelleme Formu İçin Ürünler Çekilir
        /// </summary>
        public async Task<Result<ProductUpdateRequestDto>> GetProductForUpdate(long id)
        {
            var productUpdateModel = new ProductUpdateModel();
            var productEntity = await _productRepository.GetProductById(id).AsNoTracking().FirstOrDefaultAsync();
            if (productEntity != null)
            {
                productUpdateModel.ProductUpdate = productEntity;
                productUpdateModel.ProductPictures = await _productPictureRepository.GetProductPictures(new ProductPictureFilterDto() { ProductId = id }).ToListAsync();
                productUpdateModel.ProductContents = await _productContentRepository.GetContents(new ProductContentFilterDto() { ProductId = id }).ToListAsync();
            }
            var productDto = _mapper.Map<ProductUpdateRequestDto>(productUpdateModel);
            productDto.ProductContentFormInput = await GetContentLanguageWithProductContent(id).ToListAsync();
            productDto.ProductMainPictureFormInput = await GetProductPictureFormInput(new ProductPictureFilterDto() { ProductId = id });

            return new SuccessResult<ProductUpdateRequestDto>(productDto);
        }

        /// <summary>
        /// Panel Ürün Tablosu İçin Ürünler Çekilir
        /// </summary>
        public Result<DataTableResponseWrapper<ProductDataTableDto>> GetProductDataTablePanel(DataTableFilterModel<ProductDataTableFilterPanelDto> dto)
        {
            var productFilter = _mapper.Map<ProductFilterDto>(dto.data);
            var placeFilter = _mapper.Map<PlaceFilterDto>(dto.data);

            var products = _productRepository.GetProducts(productFilter);
            var places = _placeRepository.GetPlaces(placeFilter);

            var productQuery = (from product in products
                                join place in places on product.PlaceId equals place.Id
                                into placeItems
                                from place in placeItems.DefaultIfEmpty()
                                select new ProductDataTableDto
                                {
                                    Id = product.Id,
                                    PlaceName = place != null ? place.Name : "",
                                    PlaceId = place != null ? place.Id : 0,
                                    Price = product.Price
                                });

            var totalRecordCount = productQuery.Count();
            var recordPerPage = dto.recordPerPage.GetValueOrDefault();
            var pageNumber = dto.pageNumber.GetValueOrDefault();
            int offset = pageNumber * recordPerPage;
            var productList = productQuery.Skip(offset).Take(recordPerPage).ToList();

            var result = new DataTableResponseWrapper<ProductDataTableDto>(totalRecordCount, productList);
            return new SuccessResult<DataTableResponseWrapper<ProductDataTableDto>>(result);
        }

        #region private methods
        private IQueryable<ProductUpdateFormInputDto> GetContentLanguageWithProductContent(long productId)
        {
            var contentLanguageList = _contentLanguageRepository.GetContentLanguages(new ContentLanguageFilterDto() { });
            var productContentList = _productContentRepository.GetContents(new ProductContentFilterDto() { ProductId = productId });

            var productContentQuery = (from contentLanguage in contentLanguageList
                                       join productContent in productContentList
                                       on new { A = contentLanguage.Id, B = productId }
                                             equals new { A = productContent.LanguageId, B = productContent.ProductId }
                                       into productContents
                                       from productContent in productContents.DefaultIfEmpty()
                                       select new ProductUpdateFormInputDto
                                       {
                                           LanguageId = contentLanguage.Id,
                                           ProductContentId = productContent != null ? productContent.Id : 0,
                                           ProductId = productId,
                                           LanguageName = contentLanguage.Name,
                                           ProductContentName = productContent != null ? productContent.Name : "",
                                           Prefix = contentLanguage.Prefix,
                                       });
            return productContentQuery;
        }

        private async Task<List<ProductPictureFormInputDto>> GetProductPictureFormInput(ProductPictureFilterDto productPictureFilter)
        {
            var productPictureQuery = _productPictureRepository.GetProductPictures(productPictureFilter);
            var productPictureListQuery = (from productPicture in productPictureQuery
                                           select new ProductPictureFormInputDto
                                           {
                                               Id = productPicture.Id,
                                               Code = productPicture.MediaGuiId,
                                               ProductId = productPicture.ProductId
                                           });
            var productPictureList = await productPictureListQuery.ToListAsync();
            return productPictureList;
        }
        #endregion
    }
}
