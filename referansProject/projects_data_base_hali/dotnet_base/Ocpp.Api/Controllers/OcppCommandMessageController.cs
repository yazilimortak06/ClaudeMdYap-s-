using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.OcppDto.OcppCommandMessageDtos;
using Shared.Domain.Services.OcppServices.CommandMessageInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Ocpp.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    public class OcppCommandMessageController : ControllerBase
    {
        private readonly IOcppCommandMessageService _ocppCommandMessageService;
        public OcppCommandMessageController(IOcppCommandMessageService ocppCommandMessageService)
        {
            _ocppCommandMessageService = ocppCommandMessageService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<OcppCommandMessageDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetOcppCommandMessage")]
        public async Task<IActionResult> GetOcppCommandMessage(DataTableFilterModel<OcppCommandMessageFilterDto> dataTableFilterModel)
        {
            var result = await _ocppCommandMessageService.GetOcppCommandMessage(dataTableFilterModel);
            return this.FromHttpClientResult(result);
        }
    }
}
