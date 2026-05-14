// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\ChargeProcessController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilChargeDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.ChargeProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class ChargeProcessController : ControllerBase
    {
        private readonly IChargeProcessService _chargeProcessService;

        public ChargeProcessController(
            IChargeProcessService chargeProcessService)
        {
            _chargeProcessService = chargeProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<CheckChargeProcessResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CheckChargeProcess")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> CheckChargeProcess(CheckChargeProcessRequestDto checkChargeProcessRequest)
        {
            var result = await _chargeProcessService.CheckChargeProcess(checkChargeProcessRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<SelectDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SelectDevice")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> SelectDevice(SelectDeviceRequestDto selectDeviceRequest)
        {
            var result = await _chargeProcessService.SelectDevice(selectDeviceRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetSocketListForChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSocketListForCharge")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetSocketListForCharge(GetSocketListForChargeRequestDto getSocketListForChargeRequest)
        {
            var result = await _chargeProcessService.GetSocketListForCharge(getSocketListForChargeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<SelectSocketResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SelectSocket")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> SelectSocket(SelectSocketRequestDto selectSocketRequest)
        {
            var result = await _chargeProcessService.SelectSocket(selectSocketRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<StartChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "StartCharge")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> StartCharge(StartChargeRequestDto startChargeRequest)
        {
            var result = await _chargeProcessService.StartCharge(startChargeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<StopChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "StopCharge")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> StopCharge(StopChargeRequestDto stopChargeRequest)
        {
            var result = await _chargeProcessService.StopCharge(stopChargeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCharges")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetCharges(GetMobilChargeRequestDto getMobilChargeRequest)
        {
            var result = await _chargeProcessService.GetCharges(getMobilChargeRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilChargeDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeDetail")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeDetail(GetMobilChargeDetailRequestDto getMobilChargeDetailRequest)
        {
            var result = await _chargeProcessService.GetChargeDetail(getMobilChargeDetailRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetChargeCompleteSummaryResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetChargeCompleteSummary")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetChargeCompleteSummary(GetChargeCompleteSummaryRequestDto getChargeCompleteSummaryRequest)
        {
            var result = await _chargeProcessService.GetChargeCompleteSummary(getChargeCompleteSummaryRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ChargePaymentResultDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChargePaymentResultDetail")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> ChargePaymentResultDetail(ChargePaymentResultDetailRequestDto chargePaymentResultDetailRequest)
        {
            var result = await _chargeProcessService.ChargePaymentResultDetail(chargePaymentResultDetailRequest);
            return this.FromMobilResult(result);
        }
    }
}
