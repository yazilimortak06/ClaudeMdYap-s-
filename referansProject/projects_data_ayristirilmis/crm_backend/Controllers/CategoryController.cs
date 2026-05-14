// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\API\Controllers\CategoryController.cs
using FrameworkCore.FileApi.FileApiModel;
using FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.Refit.Service;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.WrapperCore;
using FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PixdinnCrm.Domain.Dto.CategoryDtos;
using PixdinnCrm.Domain.Interfaces.Services;
using PixdinnCrm.Domain.Interfaces.Services.CategoryServiceInterfaces;
using PixdinnCrm.Domain.WrapperModels;
using Refit;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static FrameworkCore.FileApi.FileApiModel.UploadFileRequest;

namespace PixdinnCrm.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;

        public CategoryController(ICategoryService categoryService,
            IConfiguration configuration)
        {
            _categoryService = categoryService;
            _configuration = configuration;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DataTableResponseWrapper<CategoryDataTableDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCategories")]
        public IActionResult GetCategories(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
        {
            var resp = _categoryService.GetCategoryByDatatableDto(dto);
            return this.FromResult(resp);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCategory")]
        public async Task<ActionResult> AddCategory(CategoryAddDto categoryAddDto)
        {
            var resp = await _categoryService.AddNewCategory(categoryAddDto);
            return this.FromResult(resp);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCategoryPicture")]
        public async Task<IActionResult> AddCategoryPicture(IFormFile file)
        {
            var fileApiUrl = _configuration.GetSection("FileApi:Url").Value;
            IRefitService refitFile = RestService.For<IRefitService>(fileApiUrl);

            UploadFileRequest uploadFileRequest = new UploadFileRequest();
            uploadFileRequest.GroupList = JsonConvert.DeserializeObject<List<GroupData>>(Request.Form["fileGroup"]);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                uploadFileRequest.File = memoryStream.ToArray();
            }
            uploadFileRequest.FileName = file.FileName;

            var response = await refitFile.UploadPicture(uploadFileRequest);

            UploadFileResponse uploadFileResponse = new UploadFileResponse();
            uploadFileResponse.FileKey = response.FileKey;
            return this.FromResult(new SuccessResult<UploadFileResponse>(uploadFileResponse));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(int), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCategoryById")]
        public async Task<ActionResult> GetCategoryById(long id)
        {
            var resp = await _categoryService.GetCategoryById(id);
            return this.FromResult(resp);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(int), statusCode: 200)]
        [SwaggerOperation(OperationId = "Delete")]
        public async Task<ActionResult> Delete(long id)
        {
            var resp = await _categoryService.DeleteCategoryById(id);
            return this.FromResult(resp);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Update")]
        public async Task<IActionResult> Update(CategoryUpdateRequestDto dto)
        {
            var result = await _categoryService.UpdateCategory(dto);
            return this.FromResult(result);
        }

        #region mobil api tarafına taşınacak
        [HttpPost]
        [ProducesResponseType(typeof(DataTableResponseWrapper<CategoryDataTableDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCategoriesMobil")]
        public IActionResult GetCategoriesMobil(DataTableFilterModel<CategoryDataTableFilterPanelDto> dto)
        {
            var resp = _categoryService.GetCategoryByDatatableDto(dto);
            return this.FromResult(resp);
        }
        #endregion
    }
}
