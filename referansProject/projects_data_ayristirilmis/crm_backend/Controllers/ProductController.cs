// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\API\Controllers\ProductController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Domain.Interfaces.Services;
using Domain.Interfaces.Services.ProductServiceInterfaces;
using FluentValidation.AspNetCore;
using FrameworkCore.FileApi.FileApiEnum;
using FrameworkCore.FileApi.FileApiModel;
using FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.FileApi.FileApiInterface;
using FrameworkCore.FrameworkCore.FileApi.FileApiModel;
using FrameworkCore.FrameworkCore.Refit.Service;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.WrapperCore;
using FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PixdinnCrm.Domain.Dto;
using PixdinnCrm.Domain.Dto.ProductDtos;
using PixdinnCrm.Domain.Enums.ErrorEnums;
using PixdinnCrm.Domain.Interfaces.Services;
using PixdinnCrm.Domain.Interfaces.Services.MediaFileServiceInterfaces;
using PixdinnCrm.Domain.WrapperModels;
using Refit;
using Swashbuckle.AspNetCore.Annotations;
using static FrameworkCore.FileApi.FileApiModel.UploadFileRequest;

namespace API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IFileApiWebRequest _fileApiWebRequest;
        private readonly IMediaFileService _mediaFileService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService,
            IFileApiWebRequest fileApiWebRequest,
            IMediaFileService mediaFileService,
            IConfiguration configuration)
        {
            _productService = productService;
            _fileApiWebRequest = fileApiWebRequest;
            _mediaFileService = mediaFileService;
            _configuration = configuration;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Add")]
        public async Task<IActionResult> Add(ProductInsertDto dto)
        {
            var result = await _productService.CreateNewProduct(dto);
            return this.FromResult(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Update")]
        [ValidateFilter]
        public async Task<IActionResult> Update(ProductUpdateRequestDto dto)
        {
            var result = await _productService.UpdateProduct(dto);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddProductPicture")]
        public async Task<IActionResult> AddProductPicture(IFormFile file)
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

        [HttpPost]
        [ProducesResponseType(typeof(Result<ProductDataTableDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public IActionResult List(DataTableFilterModel<ProductDataTableFilterPanelDto> dataTableFilterModel)
        {
            var result = _productService.GetProductDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetProductById")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var result = await _productService.GetProductForUpdate(id);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ProductExcelInsert")]
        public async Task<IActionResult> ProductExcelInsert(ExcelProductParseRequestModel excelProduct)
        {
            var result = await _productService.CreateNewProductExcel(excelProduct);
            return this.FromResult(new SuccessResult<ExcelProductParseResponseModel>(result.Data));
        }
    }
}
