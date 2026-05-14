using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.OcppDto.Ocpp16Dtos.Ocpp16ChangeAvailabilityDtos;
using Shared.Domain.Dto.OcppDto.Ocpp16StationRemoteTransactionDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ChangeAvailabilityInterfaces;
using Shared.Domain.Services.OcppServices.Ocpp16RemoteTransactionInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ChangeConfigurationInterfaces;
using Shared.Domain.Dto.OcppDto.Ocpp16Dtos.Ocpp16ChangeConfigurationDtos;
using Shared.Domain.Dto.OcppDto.Ocpp16Dtos.Ocpp16ResetDtos;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16RemoteResetInterfaces;
using Shared.Domain.Services.OcppServices.CommandMessageInterfaces;
using Shared.Domain.Dto.OcppDto.OcppCommandMessageDtos;

namespace Ocpp.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class Ocpp16RemoteController : ControllerBase
    {
        private readonly IOcpp16ChangeAvailabilityService _ocpp16ChangeAvailabilityService;
        private readonly IOcpp16ChangeConfigurationService _ocpp16ChangeConfigurationService;
        private readonly IOcpp16RemoteTransactionService _ocpp16RemoteTransactionService;
        private readonly IOcpp16RemoteResetService _ocpp16RemoteResetService;
        private readonly IOcppCommandMessageService _ocppCommandMessageService;
        private readonly ILogger<Ocpp16RemoteController> _logger;

        public Ocpp16RemoteController(ILogger<Ocpp16RemoteController> logger,
            IOcpp16ChangeAvailabilityService ocpp16ChangeAvailabilityService,
            IOcpp16ChangeConfigurationService ocpp16ChangeConfigurationService,
            IOcpp16RemoteTransactionService ocpp16RemoteTransactionService,
            IOcpp16RemoteResetService ocpp16RemoteResetService,
            IOcppCommandMessageService ocppCommandMessageService)
        {
            _logger = logger;
            _ocpp16ChangeAvailabilityService = ocpp16ChangeAvailabilityService;
            _ocpp16ChangeConfigurationService = ocpp16ChangeConfigurationService;
            _ocpp16RemoteTransactionService = ocpp16RemoteTransactionService;
            _ocpp16RemoteResetService = ocpp16RemoteResetService;
            _ocppCommandMessageService = ocppCommandMessageService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<Ocpp16ChangeAvailabilityResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeAvailability")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> ChangeAvailability(Ocpp16ChangeAvailabilityRequestDto ocpp16ChangeAvailabilityRequest)
        {
            var result = await _ocpp16ChangeAvailabilityService.ChangeAvailability(ocpp16ChangeAvailabilityRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<Ocpp16ChangeConfigurationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeConfiguration")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> ChangeConfiguration(Ocpp16ChangeConfigurationRequestDto ocpp16ChangeConfigurationRequest)
        {
            var result = await _ocpp16ChangeConfigurationService.ChangeConfiguration(ocpp16ChangeConfigurationRequest);
            return this.FromHttpClientResult(result);
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
            var result = await _ocpp16RemoteTransactionService.RemoteStopTransactionAsync(ocpp16StationRemoteStopTransactionRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<Ocpp16ResetResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoteReset")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> RemoteReset(Ocpp16ResetRequestDto ocpp16ResetRequest)
        {
            var result = await _ocpp16RemoteResetService.RemoteReset(ocpp16ResetRequest);
            return this.FromHttpClientResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ProcessOcppCommandResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ProcessCommand")]
        public async Task<IActionResult> ProcessCommand(ProcessOcppCommandRequestDto processOcppCommandRequest)
        {
            var result = await _ocppCommandMessageService.ProcessCommand(processOcppCommandRequest);
            return this.FromResult(result);
        }
    }
}
