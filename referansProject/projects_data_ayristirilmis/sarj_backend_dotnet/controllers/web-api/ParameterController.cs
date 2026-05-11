using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.ParameterDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ParameterServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{

    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class ParameterController : ControllerBase
    {
        private readonly IParameterService _parameterService;
        private readonly IConfiguration _configuration;

        public ParameterController(IParameterService parameterService,
            IConfiguration configuration
            )
        {
            _parameterService = parameterService;
            _configuration = configuration;

        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<ParameterListDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        //[ValidateFilter]
        public async Task<IActionResult> GetParameters(ParameterFilterDto parameterFilter)
        {
            var result = await _parameterService.GetParameterList(parameterFilter);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Add")]
        //[ValidateFilter]
        public async Task<IActionResult> UpdateParameterValue(List<ParameterDto> parameters)
        {
            var result = await _parameterService.ChangeParameterValue(parameters);
            return this.FromResult(result);
        }
    }
}
