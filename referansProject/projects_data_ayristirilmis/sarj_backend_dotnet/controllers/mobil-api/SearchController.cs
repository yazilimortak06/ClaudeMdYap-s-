// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\SearchController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.RegisterSmsDtos;
using Shared.Domain.Dto.ApiDto.UserRegisterDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.RegisterServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.SearchServiceInterfaces;
using Shared.Domain.Dto.ApiDto.MobilSearchDtos;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetSearchResultResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetSearchResult")]
        [ValidateFilter]
        public async Task<IActionResult> GetSearchResult(MobilGetSearchResultRequestDto mobilSearchRequest)
        {
            var result = await _searchService.GetSearchResult(mobilSearchRequest);
            return this.FromMobilResult(result);
        }
    }
}
