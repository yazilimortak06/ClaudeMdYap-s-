// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\CampaignController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilFirmDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.FirmProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.MobilCampaignDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.CampaignProcessServiceInterfaces;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationWithGuestFilterAttribute))]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignProcessService _campaignProcessService;
        public CampaignController(
            ICampaignProcessService campaignProcessService)
        {
            _campaignProcessService = campaignProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilCampaignListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCampaignList")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetCampaignList(GetMobilCampaignListRequestDto getMobilCampaignListRequest)
        {
            var result = await _campaignProcessService.GetCampaignList(getMobilCampaignListRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilCampaignDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetMobilCampaignDetail")]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetMobilCampaignDetail(GetMobilCampaignDetailRequestDto getMobilCampaignDetailRequest)
        {
            var result = await _campaignProcessService.GetMobilCampaignDetail(getMobilCampaignDetailRequest);
            return this.FromMobilResult(result);
        }
    }
}
