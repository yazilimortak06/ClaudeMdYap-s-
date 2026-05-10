// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Presentation\VmPanel.Api\Controllers\
// VmPanel API Controller'ları — Tümü Authorize + Admin Authentication filter gerektiriyor

// ============================================================
// FILE: AuthenticationController.cs
// ============================================================
// KAYNAK: ..\VmPanel.Api\Controllers\AuthenticationController.cs

using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.VmPanelDto.AuthenticationDtos;
using Shared.Domain.ServiceInterfaces.VmPanelServiceInterfaces.AuthenticationServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using VmPanel.Application.Filters;

namespace VmPanel.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
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
        [ValidateFilter]
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

// ============================================================
// FILE: VmDeviceManagementController.cs
// ============================================================
// KAYNAK: ..\VmPanel.Api\Controllers\VmDeviceManagementController.cs

namespace VmPanel.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    public class VmDeviceManagementController : ControllerBase
    {
        private readonly IVmDeviceManagementService _vmDeviceManagementService;
        public VmDeviceManagementController(IConfiguration configuration, IVmDeviceManagementService vmDeviceManagementService)
        {
            _vmDeviceManagementService = vmDeviceManagementService;
        }
        [HttpPost] public async Task<IActionResult> AddVmDevice(AddVmDeviceRequestDto request) => this.FromResult(await _vmDeviceManagementService.AddVmDevice(request));
        [HttpPost] public async Task<IActionResult> UpdateVmDevice(UpdateVmDeviceRequestDto request) => this.FromResult(await _vmDeviceManagementService.UpdateVmDevice(request));
        [HttpPost] public async Task<IActionResult> GetVmDeviceDataTablePanel(DataTableFilterModel<GetVmDeviceDataTableRequestDto> model) => this.FromResult(await _vmDeviceManagementService.GetVmDeviceDataTablePanel(model));
        [HttpPost] public async Task<IActionResult> GetVmDeviceForUpdate(GetVmDeviceForUpdateRequestDto request) => this.FromResult(await _vmDeviceManagementService.GetVmDeviceForUpdate(request));
    }
}

// ============================================================
// FILE: VmStationManagementController.cs
// ============================================================
// KAYNAK: ..\VmPanel.Api\Controllers\VmStationManagementController.cs

namespace VmPanel.Api.Controllers
{
    [ApiController][Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    [Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    public class VmStationManagementController : ControllerBase
    {
        private readonly IVmStationManagementService _vmStationManagementService;
        public VmStationManagementController(IConfiguration configuration, IVmStationManagementService vmStationManagementService)
        {
            _vmStationManagementService = vmStationManagementService;
        }
        [HttpPost] public async Task<IActionResult> AddVmStation(AddVmStationRequestDto request) => this.FromResult(await _vmStationManagementService.AddVmStation(request));
        [HttpPost] public async Task<IActionResult> UpdateVmStation(UpdateVmStationRequestDto request) => this.FromResult(await _vmStationManagementService.UpdateVmStation(request));
        [HttpPost] public async Task<IActionResult> GetVmStationDataTablePanel(DataTableFilterModel<GetVmStationDataTableRequestDto> model) => this.FromResult(await _vmStationManagementService.GetVmStationDataTablePanel(model));
        [HttpPost] public async Task<IActionResult> GetVmStationForUpdate(GetVmStationForUpdateRequestDto request) => this.FromResult(await _vmStationManagementService.GetVmStationForUpdate(request));
        [HttpPost] public async Task<IActionResult> RemoveVmStation(RemoveVmStationRequestDto request) => this.FromResult(await _vmStationManagementService.RemoveVmStation(request));
        [HttpPost] public async Task<IActionResult> GetVmStationsForSelectList(GetVmStationsForSelectListRequestDto request) => this.FromResult(await _vmStationManagementService.GetVmStationsForSelectList(request));
    }
}

// ============================================================
// FILE: VmTransactionManagementController.cs
// ============================================================
// KAYNAK: ..\VmPanel.Api\Controllers\VmTransactionManagementController.cs

namespace VmPanel.Api.Controllers
{
    [ApiController][Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    [Authorize][ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    public class VmTransactionManagementController : ControllerBase
    {
        private readonly IVmTransactionManagementService _vmTransactionManagementService;
        public VmTransactionManagementController(IConfiguration configuration, IVmTransactionManagementService vmTransactionManagementService)
        {
            _vmTransactionManagementService = vmTransactionManagementService;
        }
        [HttpPost]
        public async Task<IActionResult> GetVmTransactionDataTablePanel(DataTableFilterModel<GetVmTransactionDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmTransactionManagementService.GetVmTransactionDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
    }
}

// ============================================================
// DİGER CONTROLLER'LAR (sadece imzalar — implementasyon yukaridaki pattern ile aynı)
// ============================================================
// CpoManagementController           — CPO CRUD (Add/Update/GetDataTable/GetForUpdate/Remove/GetSelectList)
// VmConnectorConnectionManagementController — Connector bağlantı yönetimi
// VmConnectorManagementController   — Connector CRUD
// VmDeviceConnectionManagementController  — Cihaz bağlantı durumu
// VmPanelAdminManagementController  — Panel admin yönetimi
// VmPanelHomeManagementController   — Ana sayfa dashboard verileri
