// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\StationRatingProcessController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.Dto.ApiDto.MobilStationRatingDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.StationRatingProcessServiceInterfaces;
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
    public class StationRatingProcessController : ControllerBase
    {
        private readonly IStationRatingProcessService _stationRatingProcessService;
        private readonly IUserContextProvider _userContextProvider;
        public StationRatingProcessController(
            IUserContextProvider userContextProvider,
            IStationRatingProcessService stationRatingProcessService)
        {
            _userContextProvider = userContextProvider;
            _stationRatingProcessService = stationRatingProcessService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PreparingStationRatingFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PreparingStationRatingForm")]
        public async Task<IActionResult> PreparingStationRatingForm(PreparingStationRatingFormRequestDto preparingStationRatingFormRequest)
        {
            var result = await _stationRatingProcessService.PreparingStationRatingForm(preparingStationRatingFormRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MakeStationRatingResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "MakeStationRating")]
        public async Task<IActionResult> MakeStationRating(MakeStationRatingRequestDto makeStationRatingRequest)
        {
            var result = await _stationRatingProcessService.MakeStationRating(makeStationRatingRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetStationRatingForStationDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationRatingForStationDetail")]
        public async Task<IActionResult> GetStationRatingForStationDetail(GetStationRatingForStationDetailRequestDto getStationRatingForStationDetailRequest)
        {
            var result = await _stationRatingProcessService.GetStationRatingForStationDetail(getStationRatingForStationDetailRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetStationRatingPointsForStationDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationRatingPointsForStationDetail")]
        public async Task<IActionResult> GetStationRatingPointsForStationDetail(GetStationRatingPointsForStationDetailRequestDto getStationRatingPointsForStationDetailRequest)
        {
            var result = await _stationRatingProcessService.GetStationRatingPointsForStationDetail(getStationRatingPointsForStationDetailRequest);
            return this.FromMobilResult(result);
        }
    }
}
