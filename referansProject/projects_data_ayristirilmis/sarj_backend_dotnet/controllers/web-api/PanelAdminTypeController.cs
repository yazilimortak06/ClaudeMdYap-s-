using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Domain.Dto.ApiDto.PanelAdminTypeDtos;
using Shared.Domain.Dto.TockenDto.AdminUserTypeAuthDtos; 
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PanelAdminTypeServiceInterfaces;
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
    [ServiceFilter(typeof(WebApiRootAdminAuthenticationFilterAttribute))]
    public class PanelAdminTypeController : ControllerBase
    {
        private readonly IPanelAdminTypeService _panelAdminTypeService;

        public PanelAdminTypeController(IPanelAdminTypeService panelAdminTypeService
            )
        {
            _panelAdminTypeService = panelAdminTypeService;

        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAdminTypeDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> List(DataTableFilterModel<PanelAdminTypeFilterDto> panelAdminTypeFilterDtoFilter)
        {
            var result = await _panelAdminTypeService.GetPanelAdminTypesDatatablePanel(panelAdminTypeFilterDtoFilter);
            return this.FromResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPanelAdminTypeById")]
        //[ValidateFilter]
        public async Task<IActionResult> GetPanelAdminTypeById(long id)
        {
            var result = await _panelAdminTypeService.GetPanelAdminTypeForUpdate(id);
            return this.FromResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPanelAdminTypes")]
        //[ValidateFilter]
        public async Task<IActionResult> GetPanelAdminTypes()
        {
            var result = await _panelAdminTypeService.GetPanelAdminTypes();
            return this.FromResult(result);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeActiveState")]
        //[ValidateFilter]
        public async Task<IActionResult> ChangeActiveState(PanelAdminTypeIsActiveUpdateDto panelAdminTypeIsActiveUpdate)
        {
            var result = await _panelAdminTypeService.ChangeActiveState(panelAdminTypeIsActiveUpdate.Id);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Add")]
        //[ValidateFilter]
        public async Task<IActionResult> Add(PanelAdminTypeDto dto)
        {
  
            var result = await _panelAdminTypeService.AddPanelAdminType(dto);
            return this.FromResult(result);
        }
 
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Update")]
        //[ValidateFilter]
        public async Task<IActionResult> Update(PanelAdminTypeDto dto)
        {
            var result = await _panelAdminTypeService.UpdatePanelAdminType(dto);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<AdminUserTypeAuthDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAdminUserAuthType")]
        //[ValidateFilter]
        public async Task<IActionResult> GetAdminUserAuthType(PanelAdminTypeAuthFilterDto dto)
        {
            var result = await _panelAdminTypeService.GetAdminUserAuthType(dto.Id);
            return this.FromResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<AdminUserTypeAuthDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetAdminUserType")]
        //[ValidateFilter]
        public async Task<IActionResult> GetAdminUserType(long id)
        {
            var result = await _panelAdminTypeService.GetAdminUserType(id);
            return this.FromResult(result);
        }
    }
}
