// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\HomeController.cs
using Api.Application.Filters.MobilApi;
using Api.Application.Services.MobilApiServices.MobilHome;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilAnnouncementDtos;
using Shared.Domain.Dto.ApiDto.MobilFirmDtos;
using Shared.Domain.Dto.ApiDto.MobilHomeDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.FirmProcessServiceInterfaces;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.MobilHomeServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly IMobilHomeService _mobilHomeService;
        public HomeController( IMobilHomeService mobilHomeService)
        {
            _mobilHomeService = mobilHomeService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCampaignMobilHomeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCampaignHome")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetCampaignHome(GetCampaignMobilHomeRequestDto getCampaignMobilHomeRequest)
        {
            var result = await _mobilHomeService.GetCampaignMobilHome(getCampaignMobilHomeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUnreadedAnnouncementCountResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUnreadedAnnouncementCount")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetUnreadedAnnouncementCount(GetUnreadedAnnouncementCountRequestDto getUnreadedAnnouncementCountRequest)
        {
            var result = await _mobilHomeService.GetUnreadedAnnouncementCountMobilHome(getUnreadedAnnouncementCountRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetUserProcessMobilHomeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUserProcessMobilHome")]
        [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
        public async Task<IActionResult> GetUserProcessMobilHome(GetUserProcessMobilHomeRequestDto getUserProcessMobilHomeRequest)
        {
            var result = await _mobilHomeService.GetUserProcessMobilHome(getUserProcessMobilHomeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetHomeMapInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetHomeMapInfo")]
        public async Task<IActionResult> GetHomeMapInfo(GetHomeMapInfoRequestDto getHomeMapInfoRequest)
        {
            var result = await _mobilHomeService.GetHomeMapInfo(getHomeMapInfoRequest);
            return this.FromMobilResult(result);
        }
    }
}
