using Api.Application.Filters.StationApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.StationApiDto.ChargeDeviceConnectorStationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.StationServiceInterfaces.ChargeDeviceOcppManagmentServiceInterfaces;

namespace Station.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class ChargeDeviceConnectorOcppManagmentController : ControllerBase
    {
        private readonly IChargeDeviceConnectorOcppManagmentService _chargeDeviceConnectorOcppManagmentService;
        public ChargeDeviceConnectorOcppManagmentController(IChargeDeviceConnectorOcppManagmentService chargeDeviceConnectorOcppManagmentService)
        {
            _chargeDeviceConnectorOcppManagmentService = chargeDeviceConnectorOcppManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeDeviceConnectorForOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceConnectorForOcpp")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> GetChargeDeviceConnectorForOcpp(GetChargeDeviceConnectorForOcppRequestDto getChargeDeviceConnectorForOcppRequest)
        {
            var result = await _chargeDeviceConnectorOcppManagmentService.GetChargeDeviceConnectorForOcpp(getChargeDeviceConnectorForOcppRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateChargeDeviceConnectorForOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateChargeDeviceConnectorForOcpp")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> UpdateChargeDeviceConnectorForOcpp(UpdateChargeDeviceConnectorForOcppRequestDto updateChargeDeviceConnectorForOcppRequest)
        {
            var result = await _chargeDeviceConnectorOcppManagmentService.UpdateChargeDeviceConnectorForOcpp(updateChargeDeviceConnectorForOcppRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
