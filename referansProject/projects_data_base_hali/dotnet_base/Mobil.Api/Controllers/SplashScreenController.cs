// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\SplashScreenController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.SplashScreenDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.SplashScreenServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class SplashScreenController : ControllerBase
    {
        private readonly ISplashScreenService _splashScreenService;

        public SplashScreenController(ISplashScreenService splashScreenService)
        {
            _splashScreenService = splashScreenService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetSplashScreenResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSplashScreen")]
        [ValidateFilter]
        public async Task<IActionResult> GetSplashScreen(GetSplashScreenRequestDto getSplashScreenRequest)
        {
            var result = await _splashScreenService.GetSplashScreen(getSplashScreenRequest);
            return this.FromMobilResult(result);
        }
    }
}
