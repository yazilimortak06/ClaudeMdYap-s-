using Api.Application.Filters.MobilApi;
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.AuthenticationDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.AuthenticationServiceInterfaces;
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
    //[Authorize]
    //[ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(
            IAuthenticationService authenticationService
            )
        {
            _authenticationService = authenticationService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LoginForm")]
        //[ValidateFilter]
        public async Task<IActionResult> LoginForm(LoginFormRequestDto loginFormRequest)
        {
            var result = await _authenticationService.LoginForm(loginFormRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Login")]
        //[ValidateFilter]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var result = await _authenticationService.Login(loginRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<LogoutResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LogOut")]
        //[ValidateFilter]
        public async Task<IActionResult> LogOut()
        {
            var result = await _authenticationService.LogOut();
            return this.FromResult(result);
        }
     
    }
}
