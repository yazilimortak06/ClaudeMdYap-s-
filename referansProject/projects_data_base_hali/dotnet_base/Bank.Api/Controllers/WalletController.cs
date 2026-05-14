// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Bank.Api\Controllers\WalletController.cs
using Bank.Application.Filters;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.BankDto.WalletDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.BankServiceInterfaces.WalletServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Bank.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Produces("application/json")]
    [ServiceFilter(typeof(BankApiRequestResponseFilterAttribute))]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<CreateWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "CreateWallet")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> CreateWallet(CreateWalletRequestDto createWalletRequest)
        {
            var result = await _walletService.CreateWallet(createWalletRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<IncreaseAmountofWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "IncreaseAmountofWallet")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> IncreaseAmountofWallet(IncreaseAmountofWalletRequestDto increaseAmountofWalletRequest)
        {
            var result = await _walletService.IncreaseAmountofWallet(increaseAmountofWalletRequest);
            return this.FromHttpClientResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DecreaseAmountofWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "DecreaseAmountofWallet")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> DecreaseAmountofWallet(DecreaseAmountofWalletRequestDto decreaseAmountofWalletRequest)
        {
            var result = await _walletService.DecreaseAmountofWallet(decreaseAmountofWalletRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
