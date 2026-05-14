using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PolicyDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PolicyManagmentServiceInterfaces;
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
    public class PolicyManagmentController : ControllerBase
    {
        private readonly IPolicyManagmentService _policyManagmentService;
        private readonly IConfiguration _configuration;

        public PolicyManagmentController(IConfiguration configuration, IPolicyManagmentService policyManagmentService)
        {
            _configuration = configuration;
            _policyManagmentService = policyManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdatePolicyResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdatePolicy")]
        public async Task<IActionResult> UpdatePolicy(UpdatePolicyRequestDto updatePolicyRequest)
        {
            var result = await _policyManagmentService.UpdatePolicy(updatePolicyRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPolicyNamesResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPolicyNames")]
        public IActionResult GetPolicyNames(GetPolicyNamesRequestDto getPolicyNamesRequest)
        {
            var result =  _policyManagmentService.GetPolicyNames(getPolicyNamesRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPolicyDetailPanelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPolicyDetail")]
        public async Task<IActionResult> GetPolicyDetail(GetPolicyDetailPanelRequestDto getPolicyDetailPanelRequest)
        {
            var result = await _policyManagmentService.GetPolicyDetail(getPolicyDetailPanelRequest);
            return this.FromResult(result);
        }
    }
}
