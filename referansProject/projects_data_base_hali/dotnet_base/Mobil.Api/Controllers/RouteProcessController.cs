// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\RouteProcessController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.RouteProcessServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilRouteProcessDtos;
using Microsoft.AspNetCore.Authorization;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.SearchServiceInterfaces;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class RouteProcessController : ControllerBase
    {
        private readonly IRouteProcessService _routeProcessService;
        public RouteProcessController(ISearchService searchService, IRouteProcessService routeProcessService)
        {
            _routeProcessService = routeProcessService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateRouteProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateRouteProcess")]
        [ValidateFilter]
        public async Task<IActionResult> CreateRouteProcess(CreateRouteProcessRequestDto createRouteProcessRequest)
        {
            var result = await _routeProcessService.CreateRouteProcess(createRouteProcessRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilRouteStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetRouteStation")]
        [ValidateFilter]
        public async Task<IActionResult> GetRouteStation(GetMobilRouteStationRequestDto getMobilRouteStationRequest)
        {
            var result = await _routeProcessService.GetRouteStation(getMobilRouteStationRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilRouteDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetRouteDetail")]
        [ValidateFilter]
        public async Task<IActionResult> GetRouteDetail(GetMobilRouteDetailRequestDto getMobilRouteDetailRequest)
        {
            var result = await _routeProcessService.GetRouteDetail(getMobilRouteDetailRequest);
            return this.FromMobilResult(result);
        }
    }
}
