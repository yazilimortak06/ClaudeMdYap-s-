using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.ParameterDtos;
using Shared.Domain.Dto.ApiDto.ParameterValueDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ParameterValueServiceInterfaces;
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
    public class ParameterValueController : ControllerBase
    {
        private readonly IParameterValueService _parameterValueService;
        private readonly IConfiguration _configuration;

        public ParameterValueController(IParameterValueService parameterValueService,
            IConfiguration configuration
            )
        {
            _parameterValueService = parameterValueService;
            _configuration = configuration;

        } 
        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        //[ValidateFilter]
        public async Task<IActionResult> GetParameterValueById(long id)
        {
            var result = await _parameterValueService.GetParameterValueForUpdate(id);
            return this.FromResult(result);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Update")]
        //[ValidateFilter]
        public async Task<IActionResult> Update(List<ParameterValueUpdateDto> dto)
        {
            var result = await _parameterValueService.UpdateParameterValue(dto);
            return this.FromResult(result);
        }

    }
}
