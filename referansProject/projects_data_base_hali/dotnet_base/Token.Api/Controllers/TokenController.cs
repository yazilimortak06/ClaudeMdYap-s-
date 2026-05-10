// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Tocken.Api\Controllers\AuthController.cs
// NOT: Token projesi birden fazla controller içerir (AuthController, MobilUserController, PanelUserController vb.)
//      Bu dosya temel AuthController'ı içerir.

using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.TockenDto.AuthGroupDtos;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.TockenServiceInterfaces.AuthGroupServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tocken.Application.Filters;

namespace Tocken.Api.Controllers
{
    /// <summary>
    /// JWT token ve yetkilendirme grubu (AuthGroup) yönetimi için controller.
    /// Route: v{version}/Auth/{action}
    ///
    /// TokenRequestResponseInfoFilter ile her istek/yanıt loglanır.
    /// InnerRequestAttribute ile sadece belirli API'lardan gelen istekler kabul edilir.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(TokenRequestResponseInfoFilterAttribute))]
    public class AuthController : ControllerBase
    {
        private readonly IAuthGroupService _authGroupService;

        public AuthController(IAuthGroupService authGroupService)
        {
            _authGroupService = authGroupService;
        }

        /// <summary>
        /// Yetkilendirme gruplarını auth listesiyle birlikte getirir.
        /// Sadece WEB API'sından çağrılabilir (InnerRequestAttribute).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<AuthGroupDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "TockenGetAuthGroupWithAuthList")]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> TockenGetAuthGroupWithAuthList()
        {
            var result = await _authGroupService.TockenGetAuthGroupWithAuthList();
            return this.FromHttpClientResult(result);
        }

        /// <summary>
        /// Yetkilendirme listesini getirir.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<AuthGroupDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "TockenGetAuthList")]
        public async Task<IActionResult> TockenGetAuthList()
        {
            var result = await _authGroupService.TockenGetAuthGroupWithAuthList();
            return this.FromResult(result);
        }
    }
}
