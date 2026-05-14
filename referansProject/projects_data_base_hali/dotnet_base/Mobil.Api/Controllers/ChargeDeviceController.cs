// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\ChargeDeviceController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.MobilChargeDeviceDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.ChargeDeviceProcessServiceInterfaces;
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
    [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
    public class ChargeDeviceController : ControllerBase
    {
        private readonly IChargeDeviceProcessService _chargeDeviceProcessService;
        private readonly IUserContextProvider _userContextProvider;
        public ChargeDeviceController(
            IUserContextProvider userContextProvider,
            IChargeDeviceProcessService chargeDeviceProcessService)
        {
            _userContextProvider = userContextProvider;
            _chargeDeviceProcessService = chargeDeviceProcessService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetDeviceListForStationDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDeviceListForStationDetail")]
        public async Task<IActionResult> GetDeviceListForStationDetail(GetDeviceListForStationDetailRequestDto getDeviceListForStationDetailRequest)
        {
            var result = await _chargeDeviceProcessService.GetDeviceListForStationDetail(getDeviceListForStationDetailRequest);
            return this.FromMobilResult(result);
        }
    }
}
