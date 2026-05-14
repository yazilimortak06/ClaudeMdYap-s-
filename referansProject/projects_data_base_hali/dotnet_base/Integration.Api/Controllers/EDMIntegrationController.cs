// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Integration.Api\Controllers\EDMIntegrationController.cs

using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Integration.Application.Filters;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.IntegrationDto.ArchiveAndInvoiceEDMIntegrationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.ArchiveAndInvoiceIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class EDMIntegrationController : ControllerBase
    {
        private readonly IArchiveAndInvoiceEDMIntegrationService _archiveAndInvoiceEDMIntegrationService;

        public EDMIntegrationController(IArchiveAndInvoiceEDMIntegrationService archiveAndInvoiceEDMIntegrationService)
        {
            _archiveAndInvoiceEDMIntegrationService = archiveAndInvoiceEDMIntegrationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ArchiveAndInvoiceEDMIntegrationLoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ArchiveAndInvoiceEDMLogin")]
        [InnerRequestAttribute(new ApiName[] { ApiName.TOKEN })]
        public async Task<IActionResult> ArchiveAndInvoiceEDMLogin(ArchiveAndInvoiceEDMIntegrationLoginRequestDto request)
        {
            var result = await _archiveAndInvoiceEDMIntegrationService.ArchiveAndInvoiceEDMLogin(request);
            return this.FromHttpClientResult(result);
        }
    }
}
