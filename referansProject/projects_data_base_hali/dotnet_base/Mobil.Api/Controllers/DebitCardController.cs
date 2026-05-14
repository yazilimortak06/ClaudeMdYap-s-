// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\DebitCardController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilDebitCardDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.DebitCardProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    public class DebitCardController : ControllerBase
    {
        private readonly IDebitCardProcessService _debitCardProcessService;

        public DebitCardController(IDebitCardProcessService debitCardProcessService)
        {
            _debitCardProcessService = debitCardProcessService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilDebitCardListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDebitCardList")]
        public async Task<IActionResult> GetDebitCardList(GetMobilDebitCardListRequestDto getMobilDebitCardListRequest)
        {
            var result = await _debitCardProcessService.GetDebitCardList(getMobilDebitCardListRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetMobilDebitCardDetailResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDebitCardDetail")]
        public async Task<IActionResult> GetDebitCardDetail(GetMobilDebitCardDetailRequestDto getMobilDebitCardDetailRequest)
        {
            var result = await _debitCardProcessService.GetDebitCardDetail(getMobilDebitCardDetailRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddMobilDebitCardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddDebitCard")]
        [ValidateFilter]
        public async Task<IActionResult> AddDebitCard(AddMobilDebitCardRequestDto addMobilDebitCardRequest)
        {
            var result = await _debitCardProcessService.AddDebitCard(addMobilDebitCardRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<SetDefaultMobilDebitCardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetDefaultDebitCard")]
        public async Task<IActionResult> SetDefaultDebitCard(SetDefaultMobilDebitCardRequestDto setDefaultMobilDebitCardRequest)
        {
            var result = await _debitCardProcessService.SetDefaultDebitCard(setDefaultMobilDebitCardRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveMobilDebitCardResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveDebitCard")]
        public async Task<IActionResult> RemoveDebitCard(RemoveMobilDebitCardRequestDto removeMobilDebitCardRequest)
        {
            var result = await _debitCardProcessService.RemoveDebitCard(removeMobilDebitCardRequest);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetBankInfoFromMobilResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetBankInfoFromMobil")]
        public async Task<IActionResult> GetBankInfoFromMobil(GetBankInfoFromMobilRequestDto getBankInfoFromMobilRequest)
        {
            var result = await _debitCardProcessService.GetBankInfoFromMobil(getBankInfoFromMobilRequest);
            return this.FromMobilResult(result);
        }
    }
}
