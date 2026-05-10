// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\StationController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.StationProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.MobilStationDtos;
using FrameworkCore.FrameworkCore.FilterAttributeCore;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class StationController : ControllerBase
    {
        private readonly IStationProcessService _stationProcessService;
        private readonly IUserContextProvider _userContextProvider;
        public StationController(
            IStationProcessService stationProcessService,
            IUserContextProvider userContextProvider
            )
        {
            _stationProcessService = stationProcessService;
            _userContextProvider = userContextProvider;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilStationFilterPrepareResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareStationFilter")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> PrepareStationFilter(MobilStationFilterPrepareRequestDto mobilStationFilterPrepareRequest)
        {
            var result = await _stationProcessService.PrepareStationFilter(mobilStationFilterPrepareRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationList")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> GetStationList(GetMobilStationListRequestDto getMobilStationListRequest)
        {
            var result = await _stationProcessService.GetStationList(getMobilStationListRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetStationFilterCountResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationFilterCount")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> GetStationFilterCount(GetStationFilterCountRequestDto getStationFilterCountRequest)
        {
            var result = await _stationProcessService.GetStationFilterCount(getStationFilterCountRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationMapListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationMapList")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetStationMapList(GetMobilStationMapListRequestDto getMobilStationMapListRequest)
        {
            var result = await _stationProcessService.GetStationMapList(getMobilStationMapListRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationListDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationListDetail")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetStationListDetail(GetMobilStationListDetailRequestDto getMobilStationListDetailRequest)
        {
            var result = await _stationProcessService.GetStationListDetail(getMobilStationListDetailRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationDetail")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetStationDetail(GetMobilStationDetailRequestDto getMobilStationDetailRequest)
        {
            var result = await _stationProcessService.GetStationDetail(getMobilStationDetailRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationPictureResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationPicture")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetStationPicture(GetMobilStationPictureRequestDto getMobilStationPictureRequest)
        {
            var result = await _stationProcessService.GetStationPicture(getMobilStationPictureRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilFavoriteStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetFavoriteStation")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> GetFavoriteStation(GetMobilFavoriteStationRequestDto getMobilFavoriteStationRequest)
        {
            var result = await _stationProcessService.GetFavoriteStation(getMobilFavoriteStationRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddFavoriteStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddFavoriteStation")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> AddFavoriteStation(AddFavoriteStationRequestDto addFavoriteStationRequest)
        {
            var result = await _stationProcessService.AddFavoriteStation(addFavoriteStationRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveFavoriteStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveFavoriteStation")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> RemoveFavoriteStation(RemoveFavoriteStationRequestDto removeFavoriteStationRequest)
        {
            var result = await _stationProcessService.RemoveFavoriteStation(removeFavoriteStationRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilStationPricesResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationPrices")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        //[ValidateFilter]
        public async Task<IActionResult> GetStationPrices(GetMobilStationPricesRequestDto getMobilStationPricesRequest)
        {
            var result = await _stationProcessService.GetStationPrices(getMobilStationPricesRequest);
            return this.FromMobilResult(result);
        }
    }
}
