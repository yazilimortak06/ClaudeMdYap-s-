// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Integration.Api\Controllers\ArchiveAndInvoiceIntegrationController.cs

using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Integration.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.ArchiveIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.IntegrationDto.ArchiveIntegrationDtos;
using Shared.Domain.Dto.IntegrationDto.ArchiveAndInvoiceEDMIntegrationDtos;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.ArchiveAndInvoiceIntegrationServiceInterfaces;
using Shared.Domain.Dto.IntegrationDto.ArchiveAndInvoiceIntegrationDtos;
using Integration.Application.Services.ArchiveIntegrationService;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class ArchiveAndInvoiceIntegrationController : ControllerBase
    {
        private readonly IArchiveAndInvoiceIntegrationService _archiveAndInvoiceIntegrationService;
        private readonly IArchiveAndInvoiceEDMIntegrationService _archiveEDMIntegrationService;

        public ArchiveAndInvoiceIntegrationController(
            IArchiveAndInvoiceEDMIntegrationService archiveEDMIntegrationService,
            IArchiveAndInvoiceIntegrationService archiveAndInvoiceIntegrationService)
        {
            _archiveEDMIntegrationService = archiveEDMIntegrationService;
            _archiveAndInvoiceIntegrationService = archiveAndInvoiceIntegrationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ArchiveAndInvoiceIntegrationLoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ArchiveAndInvoiceIntegrationLogin")]
        [InnerRequestAttribute(new ApiName[] { ApiName.TOKEN })]
        public async Task<IActionResult> ArchiveAndInvoiceIntegrationLogin(ArchiveAndInvoiceIntegrationLoginRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.ArchiveAndInvoiceIntegrationLogin(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateArchiveAndInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> CreateArchiveAndInvoice(CreateArchiveAndInvoiceRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.CreateArchiveAndInvoice(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateRequestArchiveAndInvoiceResponseDto), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateRequestArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
        public async Task<IActionResult> CreateRequestArchiveAndInvoice(CreateRequestArchiveAndInvoiceRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.CreateRequestArchiveAndInvoice(request);
            return this.FromHttpClientDataResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(GetStatusArchiveAndInvoiceResponseDto), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStatusArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
        public async Task<IActionResult> GetStatusArchiveAndInvoice(GetStatusArchiveAndInvoiceRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.GetStatusArchiveAndInvoice(request);
            return this.FromHttpClientDataResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetStatusArchiveAndInvoiceListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStatusArchiveAndInvoiceList")]
        [InnerRequestAttribute(new ApiName[] { ApiName.BANK })]
        public async Task<IActionResult> GetStatusArchiveAndInvoiceList(GetStatusArchiveAndInvoiceListRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.GetStatusArchiveAndInvoiceList(request);
            return this.FromHttpClientDataResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SetDocumentDataArchiveAndInvoiceResponseDto), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetDocumentDataArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE, ApiName.BANK })]
        public async Task<IActionResult> SetDocumentDataArchiveAndInvoice(SetDocumentDataArchiveAndInvoiceRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.SetDocumentDataArchiveAndInvoice(request);
            return this.FromHttpClientDataResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetDocumentArchiveAndInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDocumentArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetDocumentArchiveAndInvoice(GetDocumentArchiveAndInvoiceRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.GetDocumentArchiveAndInvoice(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetDocumentArchiveAndInvoiceListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDocumentArchiveAndInvoiceList")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> GetDocumentArchiveAndInvoiceList(GetDocumentArchiveAndInvoiceListRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.GetDocumentArchiveAndInvoiceList(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetDocumentArchiveAndInvoiceForViewResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDocumentArchiveAndInvoiceForView")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetDocumentArchiveAndInvoiceForView(GetDocumentArchiveAndInvoiceForViewRequestDto request)
        {
            var result = await _archiveAndInvoiceIntegrationService.GetDocumentArchiveAndInvoiceForView(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ArchiveAndInvoiceEDMIntegrationLoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ArchiveEDMLogin")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> ArchiveEDMLogin(ArchiveAndInvoiceEDMIntegrationLoginRequestDto request)
        {
            var result = await _archiveEDMIntegrationService.ArchiveAndInvoiceEDMLogin(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ArchiveAndInvoiceEDMntegrationSendInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ArchiveEDMSendInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> ArchiveEDMSendInvoice(ArchiveAndInvoiceEDMntegrationSendInvoiceRequestDto request)
        {
            var result = await _archiveEDMIntegrationService.CreateEDMArchiveAndInvoice(request);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ArchiveAndInvoiceEDMIntegrationGetStatusResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStatusEDMArchive")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetStatusEDMArchive(ArchiveAndInvoiceEDMIntegrationGetStatusRequestDto request)
        {
            var result = await _archiveEDMIntegrationService.GetStatusEDMArchiveAndInvoice(request);
            return Ok();
        }
    }
}
