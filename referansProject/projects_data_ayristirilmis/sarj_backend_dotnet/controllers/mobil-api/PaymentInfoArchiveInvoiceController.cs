// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\PaymentInfoArchiveInvoiceController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.PaymentInfoServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.IntegrationDto.ArchiveAndInvoiceIntegrationDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.Dto.ApiDto.PaymentInfoArchiveAndInvoiceDtos;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class PaymentInfoArchiveInvoiceController : ControllerBase
    {
        private readonly IPaymentInfoArchiveInvoiceService _paymentInfoArchiveInvoiceService;
        private readonly ILogger<PaymentInfoArchiveInvoiceController> _logger;

        public PaymentInfoArchiveInvoiceController(
            ILogger<PaymentInfoArchiveInvoiceController> logger,
            IPaymentInfoArchiveInvoiceService paymentInfoArchiveInvoiceService)
        {
            _logger = logger;
            _paymentInfoArchiveInvoiceService = paymentInfoArchiveInvoiceService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilPaymentInfoArchiveInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentInfoArchiveInvoice")]
        public async Task<IActionResult> GetPaymentInfoArchiveInvoice(GetMobilPaymentInfoArchiveInvoiceRequestDto getMobilPaymentInfoArchiveInvoiceRequest)
        {
            var result = await _paymentInfoArchiveInvoiceService.GetPaymentInfoArchiveInvoice(getMobilPaymentInfoArchiveInvoiceRequest);
            return this.FromMobilResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<GetDocumentPaymentInfoArchiveInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDocumentPaymentInfoArchiveInvoice")]
        public async Task<IActionResult> GetDocumentPaymentInfoArchiveInvoice(string guiId, string securityKey, bool? isCanceled)
        {
            var getDocumentArchiveForViewResponse = await _paymentInfoArchiveInvoiceService.GetDocumentPaymentInfoArchiveInvoice(new GetDocumentPaymentInfoArchiveInvoiceRequestDto()
            {
                GuiId = guiId,
                SecurityKey = securityKey,
                IsCanceled = isCanceled
            });
            return File(getDocumentArchiveForViewResponse.Data, getDocumentArchiveForViewResponse.ContentType);
        }
    }
}
