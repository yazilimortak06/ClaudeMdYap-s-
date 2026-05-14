// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\OpportunityController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilFirmDtos;
using Shared.Domain.Dto.ApiDto.MobilOpportunityDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.FirmProcessServiceInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.OpportunityProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class OpportunityController : ControllerBase
    {
        private readonly IOpportunityProcessService _opportunityProcessService;
        public OpportunityController(
            IOpportunityProcessService opportunityProcessService)
        {
            _opportunityProcessService = opportunityProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilOpportunityListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetOpportunityList")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetOpportunityList(GetMobilOpportunityListRequestDto getMobilOpportunityListRequest)
        {
            var result = await _opportunityProcessService.GetOpportunityList(getMobilOpportunityListRequest);
            return this.FromMobilResult(result);
        }
    }
}
