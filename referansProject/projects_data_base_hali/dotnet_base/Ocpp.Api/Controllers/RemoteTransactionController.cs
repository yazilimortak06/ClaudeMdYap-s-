using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.OcppDto.Ocpp16StationRemoteTransactionDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.Services.OcppServices.Ocpp16RemoteTransactionInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Ocpp.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class RemoteTransactionController : ControllerBase
    {
        private readonly IOcpp16RemoteTransactionService _ocpp16RemoteTransactionService;
        private readonly ILogger<RemoteTransactionController> _logger;
        public RemoteTransactionController(IOcpp16RemoteTransactionService ocpp16RemoteTransactionService, ILogger<RemoteTransactionController> logger)
        {
            _ocpp16RemoteTransactionService = ocpp16RemoteTransactionService;
            _logger = logger;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<Ocpp16StationRemoteStartTransactionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoteStartTransaction")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> RemoteStartTransaction(Ocpp16StationRemoteStartTransactionRequestDto ocpp16StationRemoteStartTransactionRequest)
        {
            var result = await _ocpp16RemoteTransactionService.RemoteStartTransactionAsync(ocpp16StationRemoteStartTransactionRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<Ocpp16StationRemoteStopTransactionResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoteStopTransaction")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> RemoteStopTransaction(Ocpp16StationRemoteStopTransactionRequestDto ocpp16StationRemoteStopTransactionRequest)
        {
            _logger.LogError("controller-geldi");
            var result = await _ocpp16RemoteTransactionService.RemoteStopTransactionAsync(ocpp16StationRemoteStopTransactionRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
