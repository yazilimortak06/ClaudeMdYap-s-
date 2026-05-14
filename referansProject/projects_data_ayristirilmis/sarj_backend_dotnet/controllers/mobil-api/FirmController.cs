// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\FirmController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.MobilFirmDtos;
using Shared.Domain.Dto.ApiDto.MobilStationDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.FirmProcessServiceInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.StationProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.MobilFirmPriceDtos;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class FirmController : ControllerBase
    {
        private readonly IFirmProcessService _firmProcessService;
        public FirmController(
            IFirmProcessService firmProcessService)
        {
            _firmProcessService = firmProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilFirmListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmList")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetFirmList(GetMobilFirmListRequestDto getMobilFirmListRequest)
        {
            var result = await _firmProcessService.GetFirmList(getMobilFirmListRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilFirmPricesResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFirmPriceList")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetFirmPriceList(GetMobilFirmPricesRequestDto getMobilFirmPricesRequest)
        {
            var result = await _firmProcessService.GetFirmPriceList(getMobilFirmPricesRequest);
            return this.FromMobilResult(result);
        }
    }
}
