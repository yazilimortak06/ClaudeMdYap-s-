// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\CountryCityAndTownController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.CityDtos;
using Shared.Domain.Dto.ApiDto.CountryDtos;
using Shared.Domain.Dto.ApiDto.TownDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.CountryCityAndTownServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class CountryCityAndTownController : ControllerBase
    {
        private readonly ICountryCityAndTownService _countryCityAndTownService;
        private readonly IConfiguration _configuration;

        public CountryCityAndTownController(ICountryCityAndTownService countryCityAndTownService,
            IConfiguration configuration)
        {
            _countryCityAndTownService = countryCityAndTownService;
            _configuration = configuration;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCountryListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCountryList")]
        public async Task<IActionResult> GetCountryList(CountryFilterDto countryFilter)
        {
            var result = await _countryCityAndTownService.GetCountryList(countryFilter);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCityListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCityList")]
        public async Task<IActionResult> GetCityList(CityFilterDto cityFilter)
        {
            var result = await _countryCityAndTownService.GetCityList(cityFilter);
            return this.FromMobilResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetTownListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetTownList")]
        public async Task<IActionResult> GetTownList(TownFilterDto townFilter)
        {
            var result = await _countryCityAndTownService.GetTownList(townFilter);
            return this.FromMobilResult(result);
        }
    }
}
