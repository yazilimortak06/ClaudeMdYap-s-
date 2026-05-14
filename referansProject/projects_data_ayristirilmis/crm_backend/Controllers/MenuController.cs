// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\API\Controllers\MenuController.cs
using Domain.Interfaces.Services.ProductServiceInterfaces;
using FrameworkCore.FrameworkCore.FileApi.FileApiInterface;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.WrapperCore;
using FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PixdinnCrm.Domain.Dto.MenuDtos;
using PixdinnCrm.Domain.Interfaces.Services.MediaFileServiceInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.MenuServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixdinnCrm.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IConfiguration _configuration;
        private readonly IFileApiWebRequest _fileApiWebRequest;
        private readonly IMediaFileService _mediaFileService;
        private readonly IProductService _productService;

        public MenuController(IMenuService menuService,
            IFileApiWebRequest fileApiWebRequest,
            IMediaFileService mediaFileService,
            IConfiguration configuration,
            IProductService productService)
        {
            _menuService = menuService;
            _configuration = configuration;
            _mediaFileService = mediaFileService;
            _fileApiWebRequest = fileApiWebRequest;
            _productService = productService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public IActionResult List(DataTableFilterModel<MenuDataTableFilterDto> menuFilterDto)
        {
            var result = _menuService.GetMenuForDataTablePanel(menuFilterDto);
            return this.FromResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MenuUpdateForm")]
        public async Task<IActionResult> MenuUpdateForm(long placeId, long menuId)
        {
            var result = await _menuService.GetMenuUpdateFormData(menuId);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddMenu")]
        public async Task<IActionResult> AddMenu(MenuDto dto)
        {
            var result = await _menuService.CreateNewMenu(dto);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddMenuProduct")]
        public async Task<IActionResult> AddMenuProduct(MenuProductInsertDto dto)
        {
            var result = await _menuService.CreateNewMenuProduct(dto);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetListMenuProduct")]
        public IActionResult GetListMenuProduct(MenuProductFilterDto menuProductFilterDto)
        {
            var result = _menuService.GetListMenuProduct(menuProductFilterDto);
            return this.FromResult(result);
        }
    }
}
