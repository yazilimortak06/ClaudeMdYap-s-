using Api.Application.Filters.WebApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.ApiDto.PanelAdminDtos;
using Shared.Domain.ServiceInterfaces.PanelApiServiceInterfaces.PanelAdminServiceInterfaces;
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
    public class PanelAdminController : ControllerBase
    {
        private readonly IPanelAdminService _panelAdminService;
        private readonly IConfiguration _configuration;
        public PanelAdminController(IPanelAdminService panelAdminService,
            IConfiguration configuration
            )
        {
            _panelAdminService = panelAdminService;
            _configuration = configuration;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<PanelAdminDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "List")]
        public async Task<IActionResult> List(DataTableFilterModel<PanelAdminFilterDto> panelAdminFilterDtoFilter)
        {
            var result = await _panelAdminService.GetPanelAdminsDatatablePanel(panelAdminFilterDtoFilter);
            return this.FromResult(result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetPanelAdminById")]
        public async Task<IActionResult> GetPanelAdminById(long id)
        {
            var result = await _panelAdminService.GetPanelAdminForUpdate(id);
            return this.FromResult(result);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Result<bool>), statusCode: 200)]
        [SwaggerOperation(OperationId = "ChangeActiveState")]
        public async Task<IActionResult> ChangeActiveState(PanelAdminIsActiveUpdateDto panelAdminIsActiveUpdate)
        {
            var result = await _panelAdminService.ChangeActiveState(panelAdminIsActiveUpdate.Id);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<AddPanelAdminResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "Add")]
        [ValidateFilter]
        public async Task<IActionResult> Add(AddPanelAdminRequestDto addPanelAdminRequest)
        {

            var result = await _panelAdminService.AddPanelAdmin(addPanelAdminRequest);
            return this.FromResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<RemovePanelAdminResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemovePanelAdmin")]
        [ValidateFilter]
        public async Task<IActionResult> RemovePanelAdmin(RemovePanelAdminRequestDto removePanelAdminRequest)
        {

            var result = await _panelAdminService.RemovePanelAdmin(removePanelAdminRequest);
            return this.FromResult(result);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Result<UpdatePanelAdminResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdatePanelAdmin")]
        //[ValidateFilter]
        public async Task<IActionResult> UpdatePanelAdmin(UpdatePanelAdminRequestDto updatePanelAdminRequest)
        {
            var result = await _panelAdminService.UpdatePanelAdmin(updatePanelAdminRequest);
            return this.FromResult(result);
        }
    }
}
