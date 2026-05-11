using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.OcppDto.OcppTriggerMessageManagmentDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.Services.OcppServices.OcppTriggerMessageManagmentInterfaces;
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
    //[ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class OcppTriggerMessageManagmentController : ControllerBase
    {
        private readonly IOcppTriggerMessageManagmentService _ocppTriggerMessageManagmentService;
        private readonly IConfiguration _configuration;

        public OcppTriggerMessageManagmentController(
            IConfiguration configuration,
            IOcppTriggerMessageManagmentService ocppTriggerMessageManagmentService)
        {
            _configuration = configuration;
            _ocppTriggerMessageManagmentService = ocppTriggerMessageManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetDevicesToTriggerResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetDevicesToTrigger")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WORKER_SERVICE })]
        public async Task<IActionResult> GetDevicesToTrigger(GetDevicesToTriggerRequestDto getDevicesToTriggerRequest)
        {
            var result = await _ocppTriggerMessageManagmentService.GetDevicesToTrigger(getDevicesToTriggerRequest);
            return this.FromHttpClientResult(result);
        }
    }
}
