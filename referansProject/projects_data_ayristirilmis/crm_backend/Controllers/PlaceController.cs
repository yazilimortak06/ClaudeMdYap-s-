// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\API\Controllers\PlaceController.cs
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
using PixdinnCrm.Domain.Dto.PlaceDtos;
using PixdinnCrm.Domain.Interfaces.Services;
using PixdinnCrm.Domain.Interfaces.Services.MediaFileServiceInterfaces;
using PixdinnCrm.Domain.Interfaces.Services.PlaceServiceInterfaces;
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
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly IConfiguration _configuration;
        private readonly IFileApiWebRequest _fileApiWebRequest;
        private readonly IMediaFileService _mediaFileService;

        public PlaceController(IPlaceService placeService,
            IFileApiWebRequest fileApiWebRequest,
            IMediaFileService mediaFileService,
            IConfiguration configuration)
        {
            _placeService = placeService;
            _configuration = configuration;
            _mediaFileService = mediaFileService;
            _fileApiWebRequest = fileApiWebRequest;
        }

        /// <summary>
        /// @ Yeni mekan ekler.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Add")]
        [ValidateFilter]
        public async Task<IActionResult> Add(PlaceInsertDto placeDto)
        {
            var result = await _placeService.CreateNewPlace(placeDto);
            return this.FromResult(result);
        }

        /// <summary>
        /// @ Mekan günceller.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Update")]
        [ValidateFilter]
        public async Task<IActionResult> Update(PlaceUpdateRequestDto dto)
        {
            var result = await _placeService.UpdatePlace(dto);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public IActionResult List(DataTableFilterModel<PlaceFilterDto> placeFilterDto)
        {
            var result = _placeService.GetPlacesForDataTable(placeFilterDto);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddPlacePicture")]
        public async Task<IActionResult> AddPlacePicture([FromForm] IFormFile file)
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

        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPlaceById")]
        public async Task<IActionResult> GetPlaceById(long id)
        {
            var result = await _placeService.GetPlaceByIdForUpdateRequest(id);
            return this.FromResult(result);
        }
    }
}
