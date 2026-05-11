using Api.Application.Filters.StationApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.StationApiDto.ChargeProcessStationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.StationServiceInterfaces.ChargeOcppManagmentInterfaces;
using Shared.Domain.ServiceInterfaces.StationServiceInterfaces.OcppFirmManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.StationApiDto.OcppFirmManagmentDtos;

namespace Station.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class OcppFirmManagmentController : ControllerBase
    {
        private readonly IOcppFirmManagmentService _ocppFirmManagmentService;
        public OcppFirmManagmentController(IOcppFirmManagmentService ocppFirmManagmentService)
        {
            _ocppFirmManagmentService = ocppFirmManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetFirmForOcppResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmForOcpp")]
        [InnerRequestAttribute(new ApiName[] { ApiName.VM })]
        public async Task<IActionResult> GetFirmForOcpp(GetFirmForOcppRequestDto getFirmForOcppRequest)
        {
            var result = await _ocppFirmManagmentService.GetFirmForOcpp(getFirmForOcppRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
