// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\File.Api\Controllers\FileController.cs

using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.FileDto.ExportExcelDtos;
using Shared.Domain.Dto.FileDto.ExportPdfDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.ServiceInterfaces.FileServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File.Api.Controllers
{
    /// <summary>
    /// Dosya yükleme, Excel ve PDF oluşturma işlemleri için controller.
    /// Route pattern: v{version}/File/{action}
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Base64 veya binary formatında resim yükler.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UploadPicture")]
        [ValidateFilter]
        public async Task<IActionResult> UploadPicture([FromBody] UploadFileRequestDto uploadFileRequest)
        {
            var response = await _fileService.UploadPicture(uploadFileRequest);
            return this.FromResult(response);
        }

        /// <summary>
        /// Şarj işlemleri için Excel raporu oluşturur.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateExcelChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateExcelCharge")]
        [ValidateFilter]
        public IActionResult CreateExcelCharge(CreateExcelChargeRequestDto createExcelChargeRequest)
        {
            var response = _fileService.CreateExcelCharge(createExcelChargeRequest);
            return this.FromHttpClientResult(response);
        }

        /// <summary>
        /// Ödeme bilgileri için Excel raporu oluşturur.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateExcelPaymentInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateExcelPaymentInfo")]
        [ValidateFilter]
        public IActionResult CreateExcelPaymentInfo(CreateExcelPaymentInfoRequestDto createExcelPaymentInfoRequest)
        {
            var response = _fileService.CreateExcelPaymentInfo(createExcelPaymentInfoRequest);
            return this.FromHttpClientResult(response);
        }

        /// <summary>
        /// Ödeme bilgileri için PDF oluşturur.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreatePdfPaymentInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreatePdfPaymentInfo")]
        [ValidateFilter]
        public IActionResult CreatePdfPaymentInfo(CreatePdfPaymentInfoRequestDto createPdfPaymentInfoRequest)
        {
            var response = _fileService.CreatePdfPaymentInfo(createPdfPaymentInfoRequest);
            return this.FromHttpClientResult(response);
        }

        /// <summary>
        /// Şarj işlemleri için PDF oluşturur.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreatePdfChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreatePdfCharge")]
        [ValidateFilter]
        public IActionResult CreatePdfCharge(CreatePdfChargeRequestDto createPdfChargeRequest)
        {
            var response = _fileService.CreatePdfCharge(createPdfChargeRequest);
            return this.FromHttpClientResult(response);
        }
    }
}
