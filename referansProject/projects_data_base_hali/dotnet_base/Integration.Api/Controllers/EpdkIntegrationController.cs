// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Integration.Api\Controllers\EpdkIntegrationController.cs

using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Integration.Application.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.IntegrationDto.EpdkIntegrationDtos.EpdkCheckChargeProcessDtos;
using Shared.Domain.Dto.IntegrationDto.EpdkIntegrationDtos.EpdkTokenDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.IntegrationServiceInterfaces.EpdkIntegrationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.IntegrationDto.EpdkIntegrationDtos.EpdkStartInUseSocketDtos;
using Shared.Domain.Dto.IntegrationDto.EpdkIntegrationDtos.EpdkEndInUseSocketDtos;
using Shared.Domain.Dto.IntegrationDto.EpdkIntegrationDtos.EpdkAddPriceInfoDtos;

namespace Integration.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(IntegrationApiRequestResponseLogFilterAttribute))]
    public class EpdkIntegrationController : ControllerBase
    {
        private readonly IEpdkIntegrationService _epdkIntegrationService;

        public EpdkIntegrationController(IEpdkIntegrationService epdkIntegrationService)
        {
            _epdkIntegrationService = epdkIntegrationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkLoginIntegrationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LoginIntegration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.TOKEN })]
        public async Task<IActionResult> EpdkLoginIntegration(EpdkLoginIntegrationRequestDto request)
        {
            var result = await _epdkIntegrationService.EpdkLoginIntegration(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkStartInUseSocketResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "StartInUseSocket")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP })]
        public async Task<IActionResult> StartInUseSocket(EpdkStartInUseSocketRequestDto request)
        {
            var result = await _epdkIntegrationService.StartInUseSocket(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkCheckChargeProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CheckChargeProcess")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE })]
        public async Task<IActionResult> CheckChargeProcess(EpdkCheckChargeProcessRequestDto request)
        {
            var result = await _epdkIntegrationService.CheckChargeProcess(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkEndInUseSocketResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "EndInUseSocket")]
        [InnerRequestAttribute(new ApiName[] { ApiName.OCPP, ApiName.WORKER_SERVICE })]
        public async Task<IActionResult> EndInUseSocket(EpdkEndInUseSocketRequestDto request)
        {
            var result = await _epdkIntegrationService.EndInUseSocket(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkAddMultiplePriceInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddMultiplePriceInfo")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> AddMultiplePriceInfo(EpdkAddMultiplePriceInfoRequestDto request)
        {
            var result = await _epdkIntegrationService.AddMultiplePriceInfo(request);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<EpdkAddPriceInfoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddPriceInfo")]
        [InnerRequestAttribute(new ApiName[] { ApiName.STATION, ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> AddPriceInfo(EpdkAddPriceInfoRequestDto request)
        {
            var result = await _epdkIntegrationService.AddPriceInfo(request);
            return this.FromHttpClientResult(result);
        }
    }
}
