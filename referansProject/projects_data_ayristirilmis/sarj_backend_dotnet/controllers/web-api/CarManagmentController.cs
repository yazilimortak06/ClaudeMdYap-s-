using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelCarDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.CarManagmentServiceIntefaces;
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
    public class CarManagmentController : ControllerBase
    {
        private readonly ICarManagmentService _carManagmentService;
        private readonly ICarBrandManagmentService _carBrandManagmentService;
        private readonly ICarModelManagmentService _carModelManagmentService;
        private readonly ICarTypeManagmentService _carTypeManagmentService;
        private readonly IConfiguration _configuration;

        public CarManagmentController(
            IConfiguration configuration
,
            ICarManagmentService carManagmentService,
            ICarBrandManagmentService carBrandManagmentService,
            ICarModelManagmentService carModelManagmentService,
            ICarTypeManagmentService carTypeManagmentService)
        {
            _configuration = configuration;
            _carManagmentService = carManagmentService;
            _carBrandManagmentService = carBrandManagmentService;
            _carModelManagmentService = carModelManagmentService;
            _carTypeManagmentService = carTypeManagmentService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAddCarResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCar")]
        public async Task<IActionResult> AddCar(PanelAddCarRequestDto addCarRequest)
        {
            var result = await _carManagmentService.AddCar(addCarRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelUpdateCarResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCar")]
        public async Task<IActionResult> UpdateCar(PanelUpdateCarRequestDto updateCarRequest)
        {
            var result = await _carManagmentService.UpdateCar(updateCarRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCarForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarForUpdate")]
        public async Task<IActionResult> GetCarForUpdate(GetCarForUpdateRequestDto getCarForUpdateRequest)
        {
            var result = await _carManagmentService.GetCarForUpdate(getCarForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PrepareCarInsertFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareCarInsertForm")]
        public async Task<IActionResult> PrepareCarInsertForm(PrepareCarInsertFormRequestDto prepareCarInsertFormRequest)
        {
            var result = await _carManagmentService.PrepareCarInsertForm(prepareCarInsertFormRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CarDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarDataTablePanel")]
        public async Task<IActionResult> GetCarDataTablePanel(DataTableFilterModel<GetCarDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _carManagmentService.GetCarDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCarResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCar")]
        public async Task<IActionResult> RemoveCar(RemoveCarRequestDto removeCarRequest)
        {
            var result = await _carManagmentService.RemoveCar(removeCarRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAddCarBrandResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCarBrand")]
        public async Task<IActionResult> AddCarBrand(PanelAddCarBrandRequestDto addCarBrandRequest)
        {
            var result = await _carBrandManagmentService.AddCarBrand(addCarBrandRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelUpdateCarBrandResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCarBrand")]
        public async Task<IActionResult> UpdateCarBrand(PanelUpdateCarBrandRequestDto updateCarBrandRequest)
        {
            var result = await _carBrandManagmentService.UpdateCarBrand(updateCarBrandRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CarBrandDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarBrandDataTablePanel")]
        public async Task<IActionResult> GetCarBrandDataTablePanel(DataTableFilterModel<GetCarBrandDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _carBrandManagmentService.GetCarBrandDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCarBrandForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarBrandForSelectList")]
        public async Task<IActionResult> GetCarBrandForSelectList(GetCarBrandForSelectListRequestDto getCarBrandForSelectListRequest)
        {
            var result = await _carBrandManagmentService.GetCarBrandForSelectList(getCarBrandForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCarBrandResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCarBrand")]
        public async Task<IActionResult> RemoveCarBrand(RemoveCarBrandRequestDto removeCarBrandRequest)
        {
            var result = await _carBrandManagmentService.RemoveCarBrand(removeCarBrandRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAddCarModelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCarModel")]
        public async Task<IActionResult> AddCarModel(PanelAddCarModelRequestDto addCarModelRequest)
        {
            var result = await _carModelManagmentService.AddCarModel(addCarModelRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelUpdateCarModelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCarModel")]
        public async Task<IActionResult> UpdateCarModel(PanelUpdateCarModelRequestDto updateCarModelRequest)
        {
            var result = await _carModelManagmentService.UpdateCarModel(updateCarModelRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CarModelDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarModelDataTablePanel")]
        public async Task<IActionResult> GetCarModelDataTablePanel(DataTableFilterModel<GetCarModelDataTablePanelRequestDto> dataTableFilterModel)
        {
            var result = await _carModelManagmentService.GetCarModelDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCarModelForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarModelForSelectList")]
        public async Task<IActionResult> GetCarModelForSelectList(GetCarModelForSelectListRequestDto getCarModelForSelectListRequest)
        {
            var result = await _carModelManagmentService.GetCarModelForSelectList(getCarModelForSelectListRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCarModelResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCarModel")]
        public async Task<IActionResult> RemoveCarModel(RemoveCarModelRequestDto removeCarModelRequest)
        {
            var result = await _carModelManagmentService.RemoveCarModel(removeCarModelRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAddCarTypeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCarType")]
        public async Task<IActionResult> AddCarType(PanelAddCarTypeRequestDto addCarTypeRequest)
        {
            var result = await _carTypeManagmentService.AddCarType(addCarTypeRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelUpdateCarTypeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCarType")]
        public async Task<IActionResult> UpdateCarType(PanelUpdateCarTypeRequestDto updateCarTypeRequest)
        {
            var result = await _carTypeManagmentService.UpdateCarType(updateCarTypeRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CarTypeDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarTypeDataTablePanel")]
        public async Task<IActionResult> GetCarTypeDataTablePanel(DataTableFilterModel<GetCarTypeDataTablePanelRequestDto> dataTableFilterType)
        {
            var result = await _carTypeManagmentService.GetCarTypeDataTablePanel(dataTableFilterType);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCarTypeForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarTypeForSelectList")]
        public async Task<IActionResult> GetCarTypeForSelectList(GetCarTypeForSelectListRequestDto getCarTypeForSelectListRequestDto)
        {
            var result = await _carTypeManagmentService.GetCarTypeForSelectList(getCarTypeForSelectListRequestDto);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCarTypeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCarType")]
        public async Task<IActionResult> RemoveCarType(RemoveCarTypeRequestDto removeCarTypeRequest)
        {
            var result = await _carTypeManagmentService.RemoveCarType(removeCarTypeRequest);
            return this.FromResult(result);
        }
    }
}
