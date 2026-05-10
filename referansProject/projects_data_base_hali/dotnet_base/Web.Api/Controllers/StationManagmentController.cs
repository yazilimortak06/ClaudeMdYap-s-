// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Controllers\StationManagmentController.cs
using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelStationDtos;
using Shared.Domain.Dto.FileDto.FileUploadDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.StationManagmentServiceInterfaces;
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
    public class StationManagmentController : ControllerBase
    {
        private readonly IStationManagmentService _stationManagmentService;
        private readonly IConfiguration _configuration;
        public StationManagmentController(IStationManagmentService stationManagmentService, IConfiguration configuration)
        {
            _stationManagmentService = stationManagmentService;
            _configuration = configuration;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetStationsForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationsForSelectList")]
        public async Task<IActionResult> GetStationsForSelectList(GetStationsForSelectListRequestDto getStationsForSelectListRequest)
        {
            var result = await _stationManagmentService.GetStationsForSelectList(getStationsForSelectListRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<PanelStationListItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationDataTablePanel")]
        public async Task<IActionResult> GetStationDataTablePanel(DataTableFilterModel<GetPanelStationDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _stationManagmentService.GetStationDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelStationListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationList")]
        public async Task<IActionResult> GetStationList(GetPanelStationListRequestDto getPanelStationListRequest)
        {
            var result = await _stationManagmentService.GetStationList(getPanelStationListRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetPanelStationForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetStationForUpdate")]
        public async Task<IActionResult> GetStationForUpdate(GetPanelStationForUpdateRequestDto getPanelStationForUpdateRequest)
        {
            var result = await _stationManagmentService.GetStationForUpdate(getPanelStationForUpdateRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddPanelStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddStation")]
        public async Task<IActionResult> AddStation(AddPanelStationRequestDto addPanelStationRequest)
        {
            var result = await _stationManagmentService.AddStation(addPanelStationRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ChangeStationStateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeStationState")]
        public async Task<IActionResult> ChangeStationState(ChangeStationStateRequestDto changeStationStateRequest)
        {
            var result = await _stationManagmentService.ChangeStationState(changeStationStateRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveStation")]
        public async Task<IActionResult> RemoveStation(RemoveStationRequestDto removeStationRequest)
        {
            var result = await _stationManagmentService.RemoveStation(removeStationRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdatePanelStationResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateStation")]
        public async Task<IActionResult> UpdateStation(UpdatePanelStationRequestDto updatePanelStationRequest)
        {
            var result = await _stationManagmentService.UpdateStation(updatePanelStationRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UploadFileResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddStationPicture")]
        public async Task<IActionResult> AddStationPicture(IFormFile file)
        {
            var uploadFileResponse = await _stationManagmentService.AddStationPicture(file, Request);
            return this.FromResult(uploadFileResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelStationPrepareInsertFormResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "StationPrepareInsertForm")]
        public async Task<IActionResult> StationPrepareInsertForm(PanelStationPrepareInsertFormRequestDto panelStationPrepareInsertFormRequest)
        {
            var result = await _stationManagmentService.StationPrepareInsertForm(panelStationPrepareInsertFormRequest);
            return this.FromResult(result);
        }
    }
}
