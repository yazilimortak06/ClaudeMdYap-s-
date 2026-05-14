using Api.Application.Filters.StationApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.StationApiDto.ChargeProcessStationDtos;
using FrameworkCore.FrameworkCore.WrapperCore;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.StationServiceInterfaces.ChargeOcppManagmentInterfaces;

namespace Station.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class ChargeOcppManagmentController : ControllerBase
    {
        private readonly IChargeOcppManagmentService _chargeOcppManagmentService;
        public ChargeOcppManagmentController(IChargeOcppManagmentService chargeOcppManagmentService)
        {
            _chargeOcppManagmentService = chargeOcppManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateChargeOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCharge")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> UpdateCharge(UpdateChargeOcppRequestDto updateChargeOcppRequest)
        {
            var result = await _chargeOcppManagmentService.UpdateCharge(updateChargeOcppRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
