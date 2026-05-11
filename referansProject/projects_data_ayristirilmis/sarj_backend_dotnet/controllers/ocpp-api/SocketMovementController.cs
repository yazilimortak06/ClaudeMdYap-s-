using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.OcppDto.SocketMovementDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.Services.OcppServices.Ocpp16RemoteTransactionInterfaces;
using Shared.Domain.Services.OcppServices.SocketMovementInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocpp.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class SocketMovementController : ControllerBase
    {
        private readonly ISocketMovementService _socketMovementService;
        private readonly IOcpp16RemoteTransactionService _ocpp16RemoteTransactionService;

        public SocketMovementController(ISocketMovementService socketMovementService, IOcpp16RemoteTransactionService ocpp16RemoteTransactionService)
        {
            _socketMovementService = socketMovementService;
            _ocpp16RemoteTransactionService = ocpp16RemoteTransactionService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetLastSocketMovementResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetLastSocketMovement")]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetLastSocketMovement(GetLastSocketMovementRequestDto getLastSocketMovementRequest)
        {
            var result = await _socketMovementService.GetLastSocketMovement(getLastSocketMovementRequest);
            return this.FromHttpClientResult(result);
        }
       
    }
}
