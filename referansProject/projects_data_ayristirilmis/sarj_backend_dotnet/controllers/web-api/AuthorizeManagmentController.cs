using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ContextProviders.Interfaces.WebApi;
using Shared.Domain.Dto.ApiDto.PanelAdminDtos;
using Shared.Domain.Dto.TockenDto.AdminUserTypeAuthDtos;
using Shared.Domain.Dto.TockenDto.AuthGroupDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.AuthorizeManagmentServiceInterfaces; 
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
    //[ServiceFilter(typeof(MobilApiRequesResponsetLogFilterAttribute))]
    //[ServiceFilter(typeof(MobilApiExceptionFilterAttribute))]
    public class AuthorizeManagmentController : ControllerBase
    {
        private readonly IAuthorizeManagmentService _authorizeManagmentService;

        public AuthorizeManagmentController(
            IAuthorizeManagmentService authorizeManagmentService)
        {
            _authorizeManagmentService = authorizeManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<AuthGroupDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAuthGroupWithAuth")]
        //[ValidateFilter]
        public async Task<IActionResult> GetAuthGroupWithAuth()
        {
            var result = await _authorizeManagmentService.GetAuthGroupWithAuth();
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddAdminUserTypeAuthResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddAdminUserTypeAuth")]
        //[ValidateFilter]
        public async Task<IActionResult> AddAdminUserTypeAuth(List<AdminUserTypeAuthDto> adminUserTypeAuth)
        {
            var result = await _authorizeManagmentService.AddAdminUserTypeAuth(adminUserTypeAuth);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetAdminUserAuthsResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAdminUserAuths")]
        //[ValidateFilter]
        public async Task<IActionResult> GetAdminUserAuths(GetAdminUserAuthsRequestDto getAdminUserAuthsRequest)
        {
            var result = await _authorizeManagmentService.GetAdminUserAuths(getAdminUserAuthsRequest);
            return this.FromResult(result);
        }
    }
}
