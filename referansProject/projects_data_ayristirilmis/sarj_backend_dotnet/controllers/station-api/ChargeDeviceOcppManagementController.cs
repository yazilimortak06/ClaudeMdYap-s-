using Api.Application.Filters.StationApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.StationApiDto.ChargeDeviceStationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.StationServiceInterfaces.ChargeDeviceOcppManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Station.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class ChargeDeviceOcppManagementController : ControllerBase
    {
        private readonly IChargeDeviceOcppManagmentService _chargeDeviceOcppManagmentService;
        public ChargeDeviceOcppManagementController(IChargeDeviceOcppManagmentService chargeDeviceOcppManagmentService)
        {
            _chargeDeviceOcppManagmentService = chargeDeviceOcppManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeDeviceForOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDeviceForOcpp")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> GetChargeDeviceForOcpp(GetChargeDeviceForOcppRequestDto getChargeDeviceForOcppRequest)
        {
            var result = await _chargeDeviceOcppManagmentService.GetChargeDeviceForOcpp(getChargeDeviceForOcppRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateChargeDeviceForOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateChargeDeviceForOcpp")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> UpdateChargeDeviceForOcpp(UpdateChargeDeviceForOcppRequestDto updateChargeDeviceForOcppRequest)
        {
            var result = await _chargeDeviceOcppManagmentService.UpdateChargeDeviceForOcpp(updateChargeDeviceForOcppRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
