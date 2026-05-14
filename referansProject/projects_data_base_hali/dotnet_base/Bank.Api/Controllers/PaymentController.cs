// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Bank.Api\Controllers\PaymentController.cs
using Bank.Application.Filters;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.BankDto.PaymentCallbackDataDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.BankServiceInterfaces.PaymentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.BankDto.PaymentDtos;

namespace Bank.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentWallet")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> PaymentWallet(PaymentWalletRequestDto paymentWalletRequest)
        {
            var result = await _paymentService.PaymentWallet(paymentWalletRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentDirectDebitCardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentDirectDebitCard")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> PaymentDirectDebitCard(PaymentDirectDebitCardRequestDto paymentDirectDebitCardRequest)
        {
            var result = await _paymentService.PaymentDirectDebitCard(paymentDirectDebitCardRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentStartDebitCard3DResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentStartDebitCard3D")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> PaymentStartDebitCard3D(PaymentStartDebitCard3DRequestDto paymentStartDebitCard3DRequest)
        {
            var result = await _paymentService.PaymentStartDebitCard3D(paymentStartDebitCard3DRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PaymentCompleteDebitCard3DResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PaymentCompleteDebitCard3D")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.INTEGRATION })]
        public async Task<IActionResult> PaymentCompleteDebitCard3D(PaymentCompleteDebitCard3DRequestDto paymentCompleteDebitCard3DRequest)
        {
            var result = await _paymentService.PaymentCompleteDebitCard3D(paymentCompleteDebitCard3DRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreatePaymentCallbackDataResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreatePaymentCallbackData")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> CreatePaymentCallbackData(CreatePaymentCallbackDataRequestDto createPaymentCallbackDataRequest)
        {
            var result = await _paymentService.CreatePaymentCallback(createPaymentCallbackDataRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentStatusResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentStatus")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> GetPaymentStatus(GetPaymentStatusRequestDto getPaymentStatusRequest)
        {
            var result = await _paymentService.GetPaymentStatus(getPaymentStatusRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentDataResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPayment")]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB, })]
        public async Task<IActionResult> GetPayment(GetPaymentDataRequestDto getPaymentRequest)
        {
            var result = await _paymentService.GetPayment(getPaymentRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentDataListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentList")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB, })]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        public async Task<IActionResult> GetPaymentList(GetPaymentDataListRequestDto getPaymentListRequest)
        {
            var result = await _paymentService.GetPaymentList(getPaymentListRequest);
            return this.FromHttpClientResult(result);
        }

        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentDetailForArchiveAndInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentDetailForArchiveAndInvoice")]
        [InnerRequestAttribute(new ApiName[] { ApiName.INTEGRATION, })]
        [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
        public async Task<IActionResult> GetPaymentDetailForArchiveAndInvoice(GetPaymentDetailForArchiveAndInvoiceRequestDto getPaymentDetailForArchiveAndInvoiceRequest)
        {
            var result = await _paymentService.GetPaymentDetailForArchiveAndInvoice(getPaymentDetailForArchiveAndInvoiceRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
