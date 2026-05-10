// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\Application\Services\Menu\MenuService.cs
using AutoMapper;
using Domain.Interfaces.Services.ProductServiceInterfaces;
using FrameworkCore.Bases.BaseService;
using FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.Auth;
using FrameworkCore.FrameworkCore.Logging;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.WrapperCore.Models;
using Microsoft.EntityFrameworkCore;
using PixdinnCrm.Domain.Dto.CategoryDtos;
using PixdinnCrm.Domain.Dto.MenuDtos;
using PixdinnCrm.Domain.Dto.PlaceDtos;
using PixdinnCrm.Domain.Dto.ProductDtos;
using PixdinnCrm.Domain.Entities.MenuEntities;
using PixdinnCrm.Domain.Enums.DbEnums;
using PixdinnCrm.Domain.Enums.ErrorEnums;
using PixdinnCrm.Domain.Interfaces.Repositories.CategoryRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.MenuProductRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.MenuRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.PlaceRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Repositories.ProductRepositoryInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.CategoryServiceInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.MenuServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixdinnCrm.Application.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuProductRepository _menuProductRepository;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductContentRepository _productContentRepository;
        private readonly ICategoryContentRepository _categoryContentRepository;
        private readonly IProductPictureRepository _productPictureRepository;
        private readonly ICategoryPictureRepository _categoryPictureRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public MenuService(IMapper mapper,
                           ILoggerService logger,
                           IAuthorizeService authorizeService,
                           IAuthonticationService authonticationService,
                           IMenuRepository menuRepository,
                           IProductService productService,
                           ICategoryService categoryService,
                           IMenuProductRepository menuProductRepository,
                           IProductContentRepository productContentRepository,
                           ICategoryContentRepository categoryContentRepository,
                           IProductPictureRepository productPictureRepository,
                           ICategoryPictureRepository categoryPictureRepository,
                           IPlaceRepository placeRepository,
                           IProductRepository productRepository,
                           ICategoryRepository categoryRepository) : base(
                           mapper, logger, authorizeService, authonticationService)
        {
            _menuRepository = menuRepository;
            _productService = productService;
            _categoryService = categoryService;
            _menuProductRepository = menuProductRepository;
            _productContentRepository = productContentRepository;
            _categoryContentRepository = categoryContentRepository;
            _productPictureRepository = productPictureRepository;
            _categoryPictureRepository = categoryPictureRepository;
            _placeRepository = placeRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// panel menü tablosu için menüler çekilir
        /// </summary>
        public Result<DataTableResponseWrapper<MenuDataTableDto>> GetMenuForDataTablePanel(DataTableFilterModel<MenuDataTableFilterDto> dataTableFilterModel)
        {
            int toplamKayit = 0;
            var recordPerPage = dataTableFilterModel.recordPerPage.GetValueOrDefault();
            var pageNumber = dataTableFilterModel.pageNumber.GetValueOrDefault();
            int ofset = pageNumber * recordPerPage;

            var menuFilter = _mapper.Map<MenuFilterDto>(dataTableFilterModel.data);
            var placeFilter = _mapper.Map<PlaceFilterDto>(dataTableFilterModel.data);

            var menu = _menuRepository.GetMenus(menuFilter);
            var placeList = _placeRepository.GetPlaces(placeFilter);

            var menuQuery = (from m in menu
                             join place in placeList on m.PlaceId equals place.Id
                             into places
                             from place in places.DefaultIfEmpty()
                             select new MenuDataTableDto
                             {
                                 Id = m.Id,
                                 MenuName = m.MenuName,
                                 State = m.State,
                                 StateText = m.State == ActivePassive.ACTIVE ? "Aktif" : "Pasif",
                                 PlaceName = place.Name,
                                 PlaceId = place.Id
                             });

            toplamKayit = menuQuery.Count();
            var menuList = menuQuery.Skip(ofset).Take(recordPerPage).ToList();

            if (menuList != null)
            {
                var result = new DataTableResponseWrapper<MenuDataTableDto>(toplamKayit, menuList);
                return new SuccessResult<DataTableResponseWrapper<MenuDataTableDto>>(result);
            }
            else
            {
                return new SuccessResult<DataTableResponseWrapper<MenuDataTableDto>>(null);
            }
        }

        public async Task<Result<MenuUpdateFormDto>> GetMenuUpdateFormData(long menuId)
        {
            var menuUpdateFormDto = new MenuUpdateFormDto();
            var menuEntity = await _menuRepository.GetMenuById(menuId).FirstOrDefaultAsync();
            menuUpdateFormDto.Menu = _mapper.Map<MenuDto>(menuEntity);
            return new SuccessResult<MenuUpdateFormDto>(menuUpdateFormDto);
        }

        public async Task<Result<bool>> CreateNewMenu(MenuDto dto)
        {
            var menuEntity = _mapper.Map<Menu>(dto);
            _menuRepository.Insert(menuEntity, InsertStrategy.InsertAll);
            await _menuRepository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        public async Task<Result<bool>> CreateNewMenuProduct(MenuProductInsertDto menuProductInsert)
        {
            if (menuProductInsert.Type == MenuProductTypeEnum.CATEGORY)
            {
                var menuKontrol = _menuProductRepository.Where(x => x.MenuId == menuProductInsert.MenuId && x.Level == menuProductInsert.Level && x.Type == MenuProductTypeEnum.PRODUCT && !x.Deleted).FirstOrDefault();
                if (menuKontrol == null)
                {
                    var menuProductEntity = _mapper.Map<MenuProduct>(menuProductInsert);
                    var insertedMenuProduct = _menuProductRepository.Insert(menuProductEntity, InsertStrategy.OnlytMain);
                    var result = await _menuProductRepository.SaveChangesAsync();
                    return new SuccessResult<bool>(true);
                }
                return new ErrorResult<bool>(false, MenuErrorEnum.MENU_CATEGORY_INSERT_ERROR);
            }
            else if (menuProductInsert.Type == MenuProductTypeEnum.PRODUCT)
            {
                var menuKontrol = _menuProductRepository.Where(x => x.MenuId == menuProductInsert.MenuId && x.Level == menuProductInsert.Level && x.Type == MenuProductTypeEnum.CATEGORY && !x.Deleted).FirstOrDefault();
                if (menuKontrol == null)
                {
                    var menuProductEntity = _mapper.Map<MenuProduct>(menuProductInsert);
                    _menuProductRepository.Insert(menuProductEntity, InsertStrategy.OnlytMain);
                    await _menuProductRepository.SaveChangesAsync();
                    return new SuccessResult<bool>(true);
                }
                return new ErrorResult<bool>(false, MenuErrorEnum.MENU_PRODUCT_INSERT_ERROR);
            }
            return new ErrorResult<bool>(false, MenuErrorEnum.MENU_TYPE_ERROR);
        }

        public Result<DataTableResponseWrapper<MenuProductDataTableDto>> GetListMenuProduct(MenuProductFilterDto menuProductFilterDto)
        {
            var menuProductList = _menuProductRepository.Where(x => x.MenuId == menuProductFilterDto.MenuId && !x.Deleted).ToList();
            var menuProductListDto = _mapper.Map<List<MenuProductDataTableDto>>(menuProductList);
            var result = new DataTableResponseWrapper<MenuProductDataTableDto>(menuProductListDto.Count, menuProductListDto);
            return new SuccessResult<DataTableResponseWrapper<MenuProductDataTableDto>>(result);
        }
    }
}
