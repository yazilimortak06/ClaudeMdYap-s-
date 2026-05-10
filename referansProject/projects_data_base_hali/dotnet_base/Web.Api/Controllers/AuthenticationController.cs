// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\AuthenticationController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.AuthenticationDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.AuthenticationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LoginForm")]
        public async Task<IActionResult> LoginForm(LoginFormRequestDto loginFormRequest)
        {
            var result = await _authenticationService.LoginForm(loginFormRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var result = await _authenticationService.Login(loginRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<LogoutResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var result = await _authenticationService.LogOut();
            return this.FromResult(result);
        }
    }
}
