using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.ContentLanguageDtos;
using Shared.Domain.Enums.TockenEnums;
using Shared.Domain.GeneralEnums;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.ContentLanguageManagmentServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Web.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(WebApiRequestInfoFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(WebApiAuthenticationFilterAttribute))]
    public class ContentLanguageManagmentController : ControllerBase
    {
        private readonly IContentLanguageManagmentService _contentLanguageManagmentService;
        private readonly IConfiguration _configuration;

        public ContentLanguageManagmentController(IContentLanguageManagmentService contentLanguageManagmentService,
            IConfiguration configuration
            )
        {
            _contentLanguageManagmentService = contentLanguageManagmentService;
            _configuration = configuration;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ContentLanguageDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> List()
        {
            var result = await _contentLanguageManagmentService.GetContentLanguagesForPanel();
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<ContentLanguageDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> ListForPanel(DataTableFilterModel<ContentLanguageFilterDto> filterDto)
        {
            var result = await _contentLanguageManagmentService.GetContentLanguagesForDataTable(filterDto);
            return this.FromResult(result);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "SetDefault")]
        [TypeFilter(typeof(WebApiAuthorizationFilterAttribute), Arguments = new object[] { new long[] { (long)AuthId.CHANGE_DEFAULT_LANGUAGE }, AuthorizationByAdminType.ALL })]
        //[ValidateFilter]
        public async Task<IActionResult> SetDefault(ContentLanguageSetDefaultDto contentLanguageSetDefault)
        {
            var result = await _contentLanguageManagmentService.SetDefault(contentLanguageSetDefault.Id);
            return this.FromResult(result);
        }
    }
}
