// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\WalletInfoController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilWalletDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.WalletInfoServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class WalletInfoController : ControllerBase
    {
        private readonly IWalletInfoService _walletInfoService;
        public WalletInfoController(IWalletInfoService walletInfoService)
        {
            _walletInfoService = walletInfoService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetWalletFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetWalletForm")]
        public async Task<IActionResult> GetWalletForm(MobilGetWalletFormRequestDto mobilGetWalletFormRequest)
        {
            var result = await _walletInfoService.GetWalletForm(mobilGetWalletFormRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilCreateWalletInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateWalletInfo")]
        public async Task<IActionResult> CreateWalletInfo(MobilCreateWalletInfoRequestDto mobilCreateWalletInfoRequest)
        {
            var result = await _walletInfoService.CreateWalletInfo(mobilCreateWalletInfoRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetWalletAmountResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetWalletAmount")]
        public async Task<IActionResult> GetWalletAmount(MobilGetWalletAmountRequestDto mobilGetWalletAmountRequest)
        {
            var result = await _walletInfoService.GetWalletAmount(mobilGetWalletAmountRequest);
            return this.FromMobilResult(result);
        }
    }
}
