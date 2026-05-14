// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\WalletInfoManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelWalletDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.WalletInfoManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class WalletInfoManagmentController : ControllerBase
    {
        private readonly IWalletInfoManagmentService _walletInfoManagmentService;
        private readonly IConfiguration _configuration;
        public WalletInfoManagmentController(IConfiguration configuration, IWalletInfoManagmentService walletInfoManagmentService)
        {
            _configuration = configuration;
            _walletInfoManagmentService = walletInfoManagmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelWalletInfoListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetWalletInfoDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetWalletInfoDataTablePanel(DataTableFilterModel<GetPanelWalletInfoDataTableRequestDto> dataTableFilter)
        {
            var result = await _walletInfoManagmentService.GetWalletInfoDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAddBalanceToWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddBalanceToWallet")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> AddBalanceToWallet(PanelAddBalanceToWalletRequestDto addBalanceToWalletRequest)
        {
            var result = await _walletInfoManagmentService.AddBalanceToWallet(addBalanceToWalletRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelRemoveBalanceToWalletResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveBalanceToWallet")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> RemoveBalanceToWallet(PanelRemoveBalanceToWalletRequestDto removeBalanceToWalletRequest)
        {
            var result = await _walletInfoManagmentService.RemoveBalanceToWallet(removeBalanceToWalletRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelWalletProcessListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetWalletProcessDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetWalletProcessDataTablePanel(DataTableFilterModel<GetPanelWalletProcessDataTableRequestDto> dataTableFilter)
        {
            var result = await _walletInfoManagmentService.GetWalletProcessDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelWalletPullMoneyListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetWalletPullMoneyDataTablePanel")]
        [ServiceFilter(typeof(WebApiMainAdminAuthenticationFilterAttribute))]
        [ValidateFilter]
        public async Task<IActionResult> GetWalletPullMoneyDataTablePanel(DataTableFilterModel<GetPanelWalletPullMoneyDataTableRequestDto> dataTableFilter)
        {
            var result = await _walletInfoManagmentService.GetWalletPullMoneyDataTablePanel(dataTableFilter);
            return this.FromResult(result);
        }
    }
}
