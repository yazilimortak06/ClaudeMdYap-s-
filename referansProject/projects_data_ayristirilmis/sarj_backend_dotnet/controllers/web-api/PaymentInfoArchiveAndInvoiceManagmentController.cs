using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.PaymentInfoArchiveAndInvoiceDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PaymentInfoManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.MobilPaymentInfoDtos;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
 
    public class PaymentInfoArchiveAndInvoiceManagmentController : ControllerBase
    {
        private readonly IPaymentInfoArchiveAndInvoiceManagmentService _paymentInfoArchiveAndInvoiceManagmentService;

        public PaymentInfoArchiveAndInvoiceManagmentController( IPaymentInfoArchiveAndInvoiceManagmentService paymentInfoArchiveAndInvoiceManagmentService)
        {
            _paymentInfoArchiveAndInvoiceManagmentService = paymentInfoArchiveAndInvoiceManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<CreatePaymentInfoArchiveAndInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreatePaymentInfoArchiveAndInvoice")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> CreatePaymentInfoArchiveAndInvoice(CreatePaymentInfoArchiveAndInvoiceRequestDto createPaymentInfoArchiveAndInvoiceRequest)
        {
            var result = await _paymentInfoArchiveAndInvoiceManagmentService.CreatePaymentInfoArchiveAndInvoice(createPaymentInfoArchiveAndInvoiceRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPaymentInfoArchiveAndInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPaymentInfoArchiveAndInvoice")]
        [Authorize]
        [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
        public async Task<IActionResult> GetPaymentInfoArchiveAndInvoice(GetPaymentInfoArchiveAndInvoiceRequestDto getPaymentInfoArchiveAndInvoiceRequest)
        {
            var result = await _paymentInfoArchiveAndInvoiceManagmentService.GetPaymentInfoArchiveAndInvoice(getPaymentInfoArchiveAndInvoiceRequest);
            return this.FromResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<PanelGetDocumentPaymentInfoArchiveInvoiceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDocumentPaymentInfoArchiveInvoice")]
        public async Task<IActionResult> GetDocumentPaymentInfoArchiveInvoice(string guiId, string securityKey, bool? isCanceled)
        {
            var getDocumentArchiveForViewResponse = await _paymentInfoArchiveAndInvoiceManagmentService.GetDocumentPaymentInfoArchiveInvoice(new PanelGetDocumentPaymentInfoArchiveInvoiceRequestDto()
            {
                GuiId = guiId,
                SecurityKey = securityKey,
                IsCanceled = isCanceled
            });
            return File(getDocumentArchiveForViewResponse.Data, getDocumentArchiveForViewResponse.ContentType);
        }
    }
}
