using Integration.Application.Filters;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.GibIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.IntegrationDto.GibIntegrationDtos;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class GibIntegrationController : ControllerBase
    {
        private readonly IGibIntegrationService _gibIntegrationService;
        public GibIntegrationController(IGibIntegrationService gibIntegrationService)
        {
            _gibIntegrationService = gibIntegrationService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddEsuGibIntegrationTaxPayerResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddEsuGibIntegrationTaxPayer")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> AddEsuGibIntegrationTaxPayer(AddEsuGibIntegrationTaxPayerRequestDto addEsuGibIntegrationTaxPayerRequest)
        {
            var result = await _gibIntegrationService.AddEsuGibIntegrationTaxPayer(addEsuGibIntegrationTaxPayerRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddEsuGibIntegrationChargeDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddEsuGibIntegrationChargeDevice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> AddEsuGibIntegrationChargeDevice(AddEsuGibIntegrationChargeDeviceRequestDto addEsuGibIntegrationChargeDeviceRequest)
        {
            var result = await _gibIntegrationService.AddEsuGibIntegrationChargeDevice(addEsuGibIntegrationChargeDeviceRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
