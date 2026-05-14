// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\PaymentInfoController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.ContextProviders.Interfaces.MobilApi;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.PaymentInfoServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;
using Shared.Domain.Dto.ApiDto.MobilChargeDtos;
using Microsoft.AspNetCore.Authorization;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class PaymentInfoController : ControllerBase
    {
        private readonly IPaymentInfoService _paymentInfoService;
        private readonly IUserContextProvider _userContextProvider;
        private readonly ILogger<PaymentInfoController> _logger;

        public PaymentInfoController(
            IUserContextProvider userContextProvider,
            ILogger<PaymentInfoController> logger,
            IPaymentInfoService paymentInfoService)
        {
            _userContextProvider = userContextProvider;
            _logger = logger;
            _paymentInfoService = paymentInfoService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilPaymentInfoStatusResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentInfoStatus")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetPaymentInfoStatus(GetMobilPaymentInfoStatusRequestDto getMobilPaymentInfoStatusRequest)
        {
            var result = await _paymentInfoService.GetPaymentInfoStatus(getMobilPaymentInfoStatusRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PreparePaymentFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PreparePaymentForm")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> PreparePaymentForm(PreparePaymentFormRequestDto preparePaymentFormRequest)
        {
            var result = await _paymentInfoService.PreparePaymentForm(preparePaymentFormRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilPaymentChargeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentCharge")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetPaymentCharge(GetMobilPaymentChargeRequestDto getMobilPaymentChargeRequest)
        {
            var result = await _paymentInfoService.GetPaymentCharge(getMobilPaymentChargeRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilPaymentAddBalanceWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentAddBalanceWallet")]
        [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
        [Authorize]
        [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetPaymentAddBalanceWallet(GetMobilPaymentAddBalanceWalletRequestDto getMobilPaymentAddBalanceWalletRequest)
        {
            var result = await _paymentInfoService.GetPaymentAddBalanceWallet(getMobilPaymentAddBalanceWalletRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<CompleteMobilPaymentInfoDebitCard3DResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CompletePaymentInfoDebitCard3D")]
        [InnerRequestAttribute(new ApiName[] { ApiName.BANK })]
        public async Task<IActionResult> CompletePaymentInfoDebitCard3D(CompleteMobilPaymentInfoDebitCard3DRequestDto completeMobilPaymentInfoDebitCard3DRequest)
        {
            var result = await _paymentInfoService.CompletePaymentInfoDebitCard3D(completeMobilPaymentInfoDebitCard3DRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
