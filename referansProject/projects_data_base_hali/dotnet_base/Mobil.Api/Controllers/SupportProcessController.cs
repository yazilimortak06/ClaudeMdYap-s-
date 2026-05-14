// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\SupportProcessController.cs
using Api.Application.Filters.MobilApi;
using Microsoft.AspNetCore.Authorization;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.SupportProcessServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilSupportDtos;
using FrameworkCore.FrameworkCore.FilterAttributeCore;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class SupportProcessController : ControllerBase
    {
        private readonly ISupportProcessService _supportProcessService;
        public SupportProcessController(ISupportProcessService supportProcessService)
        {
            _supportProcessService = supportProcessService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilSupportListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportList")]
        public async Task<IActionResult> GetSupportList(GetMobilSupportListRequestDto getMobilSupportListRequest)
        {
            var result = await _supportProcessService.GetSupportList(getMobilSupportListRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportDetail")]
        public async Task<IActionResult> GetSupportDetail(GetMobilSupportRequestDto getMobilSupportRequest)
        {
            var result = await _supportProcessService.GetSupport(getMobilSupportRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetSupportTitleTypeListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportTitleTypeList")]
        [ValidateFilter]
        public async Task<IActionResult> GetSupportTitleTypeList(MobilGetSupportTitleTypeListRequestDto mobilGetSupportTitleTypeListRequest)
        {
            var result = await _supportProcessService.GetSupportTitleTypeList(mobilGetSupportTitleTypeListRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilCreateSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateSupport")]
        [ValidateFilter]
        public async Task<IActionResult> CreateSupport(MobilCreateSupportRequestDto createSupportRequest)
        {
            var result = await _supportProcessService.CreateSupport(createSupportRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilAnswerSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AnswerSupport")]
        [ValidateFilter]
        public async Task<IActionResult> AnswerSupport(MobilAnswerSupportRequestDto supportAnswerFromMobilRequest)
        {
            var result = await _supportProcessService.AnswerSupport(supportAnswerFromMobilRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilRatingSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RatingSupport")]
        public async Task<IActionResult> RatingSupport(MobilRatingSupportRequestDto ratingSupportRequest)
        {
            var result = await _supportProcessService.RatingSupport(ratingSupportRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilCloseSupportResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CloseSupport")]
        public async Task<IActionResult> CloseSupport(MobilCloseSupportRequestDto supportCloseMobilRequest)
        {
            var result = await _supportProcessService.CloseSupport(supportCloseMobilRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilSetMessageSupportSeenResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetMessageSupportSeen")]
        public async Task<IActionResult> SetMessageSupportSeen(MobilSetMessageSupportSeenRequestDto setMessageSupportSeenRequest)
        {
            var result = await _supportProcessService.SetMessageSupportSeen(setMessageSupportSeenRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilSetMessageSupportReadResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetMessageSupportRead")]
        public async Task<IActionResult> SetMessageSupportRead(MobilSetMessageSupportReadRequestDto setMessageSupportReadRequest)
        {
            var result = await _supportProcessService.SetMessageSupportRead(setMessageSupportReadRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetUnSeenMessageCountResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetUnSeenMessageCount")]
        public async Task<IActionResult> GetUnSeenMessageCount(MobilGetUnSeenMessageCountRequestDto getUnSeenMessageCountRequest)
        {
            var result = await _supportProcessService.GetUnSeenMessageCount(getUnSeenMessageCountRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetSupportMessageResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSupportMessage")]
        public async Task<IActionResult> GetSupportMessage(MobilGetSupportMessageRequestDto getSupportMessageRequest)
        {
            var result = await _supportProcessService.GetSupportMessage(getSupportMessageRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilSetAllMessagesSupportSeenResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetAllMessagesSupportSeen")]
        public async Task<IActionResult> SetAllMessagesSupportSeen(MobilSetAllMessagesSupportSeenRequestDto setAllMessagesSupportSeenRequest)
        {
            var result = await _supportProcessService.SetAllMessagesSupportSeen(setAllMessagesSupportSeenRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilSetSupportAllMessagesReadResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetSupportAllMessagesRead")]
        public async Task<IActionResult> SetSupportAllMessagesRead(MobilSetSupportAllMessagesReadRequestDto setSupportAllMessagesReadRequest)
        {
            var result = await _supportProcessService.SetSupportAllMessagesRead(setSupportAllMessagesReadRequest);
            return this.FromMobilResult(result);
        }
    }
}
