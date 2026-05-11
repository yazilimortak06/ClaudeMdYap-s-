using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.ParameterGroupDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ParameterGroupServiceInterfaces;
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
    public class ParameterGroupController : ControllerBase
    {
        private readonly IParameterGroupService _parameterGroupService;
        private readonly IConfiguration _configuration;

        public ParameterGroupController(IParameterGroupService parameterGroupService,
            IConfiguration configuration
            )
        {
            _parameterGroupService = parameterGroupService;
            _configuration = configuration;

        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ParameterGroupDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> List()
        {
            var result = await _parameterGroupService.GetParameterGroups();
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ParameterGroupDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> ListParameterGroupWithParameter()
        {
            var result = await _parameterGroupService.GetParameterGroupsWithParameter();
            return this.FromResult(result);
        }
    }

}
