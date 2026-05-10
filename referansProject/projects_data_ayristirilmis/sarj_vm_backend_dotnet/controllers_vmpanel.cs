// ============================================================
// sarj_vm_backend_dotnet — VmPanel.Api TUM CONTROLLER'LAR
// Kaynak: src/Presentation/VmPanel.Api/Controllers/
// ============================================================

// ---- AuthenticationController.cs ----

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

// ---- CpoManagementController.cs ----

using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.ServiceInterfaces.VmPanelServiceInterfaces.CpoManagementServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Dto.VmPanelDto.CpoManagementDtos;
using VmPanel.Application.Filters;

namespace VmPanel.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    public class CpoManagementController : ControllerBase
    {
        private readonly ICpoManagementService _cpoManagementService;
        private readonly IConfiguration _configuration;

        public CpoManagementController(IConfiguration configuration, ICpoManagementService cpoManagementService)
        {
            _configuration = configuration;
            _cpoManagementService = cpoManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddCpoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddCpo")]
        public async Task<IActionResult> AddCpo(AddCpoRequestDto addCpoRequest)
        {
            var result = await _cpoManagementService.AddCpo(addCpoRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateCpoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateCpo")]
        public async Task<IActionResult> UpdateCpo(UpdateCpoRequestDto updateCpoRequest)
        {
            var result = await _cpoManagementService.UpdateCpo(updateCpoRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<CpoDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCpoDataTablePanel")]
        public async Task<IActionResult> GetCpoDataTablePanel(DataTableFilterModel<GetCpoDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _cpoManagementService.GetCpoDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCpoForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCpoForUpdate")]
        public async Task<IActionResult> GetCpoForUpdate(GetCpoForUpdateRequestDto getCpoForUpdateRequest)
        {
            var result = await _cpoManagementService.GetCpoForUpdate(getCpoForUpdateRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetCpoForSelectListResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCpoForSelectList")]
        public async Task<IActionResult> GetCpoForSelectList(GetCpoForSelectListRequestDto getCpoForSelectListRequest)
        {
            var result = await _cpoManagementService.GetCpoForSelectList(getCpoForSelectListRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<RemoveCpoResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "RemoveCpo")]
        public async Task<IActionResult> RemoveCpo(RemoveCpoRequestDto removeCpoRequest)
        {
            var result = await _cpoManagementService.RemoveCpo(removeCpoRequest);
            return this.FromResult(result);
        }
    }
}

// ---- VmDeviceManagementController.cs ----

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
        private readonly IConfiguration _configuration;

        public VmDeviceManagementController(IConfiguration configuration, IVmDeviceManagementService vmDeviceManagementService)
        {
            _configuration = configuration;
            _vmDeviceManagementService = vmDeviceManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<AddVmDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddVmDevice")]
        public async Task<IActionResult> AddVmDevice(AddVmDeviceRequestDto addVmDeviceRequest)
        {
            var result = await _vmDeviceManagementService.AddVmDevice(addVmDeviceRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<UpdateVmDeviceResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateVmDevice")]
        public async Task<IActionResult> UpdateVmDevice(UpdateVmDeviceRequestDto updateVmDeviceRequest)
        {
            var result = await _vmDeviceManagementService.UpdateVmDevice(updateVmDeviceRequest);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<VmDeviceDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmDeviceDataTablePanel")]
        public async Task<IActionResult> GetVmDeviceDataTablePanel(DataTableFilterModel<GetVmDeviceDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmDeviceManagementService.GetVmDeviceDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetVmDeviceForUpdateResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmDeviceForUpdate")]
        public async Task<IActionResult> GetVmDeviceForUpdate(GetVmDeviceForUpdateRequestDto getVmDeviceForUpdateRequest)
        {
            var result = await _vmDeviceManagementService.GetVmDeviceForUpdate(getVmDeviceForUpdateRequest);
            return this.FromResult(result);
        }
    }
}

// ---- VmStationManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmStationManagementController : ControllerBase
    {
        private readonly IVmStationManagementService _vmStationManagementService;
        private readonly IConfiguration _configuration;

        public VmStationManagementController(IConfiguration configuration, IVmStationManagementService vmStationManagementService)
        {
            _configuration = configuration;
            _vmStationManagementService = vmStationManagementService;
        }

        [HttpPost] public async Task<IActionResult> AddVmStation(AddVmStationRequestDto addVmStationRequest)
        {
            var result = await _vmStationManagementService.AddVmStation(addVmStationRequest);
            return this.FromResult(result);
        }
        [HttpPost] public async Task<IActionResult> UpdateVmStation(UpdateVmStationRequestDto updateVmStationRequest)
        {
            var result = await _vmStationManagementService.UpdateVmStation(updateVmStationRequest);
            return this.FromResult(result);
        }
        [HttpPost] public async Task<IActionResult> GetVmStationDataTablePanel(DataTableFilterModel<GetVmStationDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmStationManagementService.GetVmStationDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
        [HttpPost] public async Task<IActionResult> GetVmStationForUpdate(GetVmStationForUpdateRequestDto getVmStationForUpdateRequest)
        {
            var result = await _vmStationManagementService.GetVmStationForUpdate(getVmStationForUpdateRequest);
            return this.FromResult(result);
        }
        [HttpPost] public async Task<IActionResult> RemoveVmStation(RemoveVmStationRequestDto removeVmStationRequest)
        {
            var result = await _vmStationManagementService.RemoveVmStation(removeVmStationRequest);
            return this.FromResult(result);
        }
        [HttpPost] public async Task<IActionResult> GetVmStationsForSelectList(GetVmStationsForSelectListRequestDto getVmStationsForSelectListRequest)
        {
            var result = await _vmStationManagementService.GetVmStationsForSelectList(getVmStationsForSelectListRequest);
            return this.FromResult(result);
        }
    }
}

// ---- VmPanelAdminManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmPanelAdminManagementController : ControllerBase
    {
        private readonly IVmPanelAdminManagementService _vmPanelAdminManagementService;
        private readonly IConfiguration _configuration;

        public VmPanelAdminManagementController(IConfiguration configuration, IVmPanelAdminManagementService vmPanelAdminManagementService)
        {
            _configuration = configuration;
            _vmPanelAdminManagementService = vmPanelAdminManagementService;
        }

        [HttpPost] public async Task<IActionResult> AddVmPanelAdmin(AddVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.AddVmPanelAdmin(r));
        [HttpPost] public async Task<IActionResult> UpdateVmPanelAdmin(UpdateVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.UpdateVmPanelAdmin(r));
        [HttpPost] public async Task<IActionResult> GetVmPanelAdminForUpdate(GetVmPanelAdminForUpdateRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.GetVmPanelAdminForUpdate(r));
        [HttpPost] public async Task<IActionResult> GetVmPanelAdminsDatatablePanel(DataTableFilterModel<GetVmPanelAdminDataTableRequestDto> r) => this.FromResult(await _vmPanelAdminManagementService.GetVmPanelAdminsDatatablePanel(r));
        [HttpPost] public async Task<IActionResult> ChangeVmPanelAdminActiveState(ChangeVmPanelAdminActiveStateRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.ChangeVmPanelAdminActiveState(r));
        [HttpPost] public async Task<IActionResult> RemoveVmPanelAdmin(RemoveVmPanelAdminRequestDto r) => this.FromResult(await _vmPanelAdminManagementService.RemoveVmPanelAdmin(r));
    }
}

// ---- VmPanelHomeManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(VmPanelAuthenticationFilterAttribute))]  // <-- MainAdmin degil
    public class VmPanelHomeManagementController : ControllerBase
    {
        private readonly IVmPanelHomeMangementService _vmPanelHomeMangementService;
        private readonly IConfiguration _configuration;

        public VmPanelHomeManagementController(IConfiguration configuration, IVmPanelHomeMangementService vmPanelHomeMangementService)
        {
            _configuration = configuration;
            _vmPanelHomeMangementService = vmPanelHomeMangementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<PrepareVmPanelHomeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareVmPanelHome")]
        public async Task<IActionResult> PrepareVmPanelHome(PrepareVmPanelHomeRequestDto prepareVmPanelHomeRequest)
        {
            var result = await _vmPanelHomeMangementService.PrepareVmPanelHome(prepareVmPanelHomeRequest);
            return this.FromResult(result);
        }
    }
}

// ---- VmTransactionManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmTransactionManagementController : ControllerBase
    {
        private readonly IVmTransactionManagementService _vmTransactionManagementService;
        private readonly IConfiguration _configuration;

        public VmTransactionManagementController(IConfiguration configuration, IVmTransactionManagementService vmTransactionManagementService)
        {
            _configuration = configuration;
            _vmTransactionManagementService = vmTransactionManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<VmTransactionDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmTransactionDataTablePanel")]
        public async Task<IActionResult> GetVmTransactionDataTablePanel(DataTableFilterModel<GetVmTransactionDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmTransactionManagementService.GetVmTransactionDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
    }
}

// ---- VmConnectorManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmConnectorManagementController : ControllerBase
    {
        private readonly IVmConnectorManagementService _vmConnectorManagementService;
        private readonly IConfiguration _configuration;

        public VmConnectorManagementController(IConfiguration configuration, IVmConnectorManagementService vmConnectorManagementService)
        {
            _configuration = configuration;
            _vmConnectorManagementService = vmConnectorManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<GetVmPanelConnectorPowerTypeResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmPanelConnectorPowerType")]
        public async Task<IActionResult> GetVmPanelConnectorPowerType(GetVmPanelConnectorPowerTypeRequestDto getVmPanelConnectorPowerTypeRequest)
        {
            var result = await _vmConnectorManagementService.GetVmPanelConnectorPowerType(getVmPanelConnectorPowerTypeRequest);
            return this.FromResult(result);
        }
    }
}

// ---- VmDeviceConnectionManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmDeviceConnectionManagementController : ControllerBase
    {
        private readonly IVmDeviceConnectionManagementService _vmDeviceConnectionManagementService;
        private readonly IConfiguration _configuration;

        public VmDeviceConnectionManagementController(IConfiguration configuration, IVmDeviceConnectionManagementService vmDeviceConnectionManagementService)
        {
            _configuration = configuration;
            _vmDeviceConnectionManagementService = vmDeviceConnectionManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<VmDeviceConnectionDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmDeviceConnectionDataTablePanel")]
        public async Task<IActionResult> GetVmDeviceConnectionDataTablePanel(DataTableFilterModel<GetVmDeviceConnectionDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmDeviceConnectionManagementService.GetVmDeviceConnectionDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<DeviceConnectionCommandMessageDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmCommandMessageDataTablePanel")]
        public async Task<IActionResult> GetVmCommandMessageDataTablePanel(DataTableFilterModel<GetVmCommandMessageDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmDeviceConnectionManagementService.GetVmCommandMessageDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
    }
}

// ---- VmConnectorConnectionManagementController.cs ----

namespace VmPanel.Api.Controllers
{
    [ApiController][Authorize]
    [ServiceFilter(typeof(VmPanelMainAdminAuthenticationFilterAttribute))]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(VmPanelRequestResponseLogFilterAttribute))]
    public class VmConnectorConnectionManagementController : ControllerBase
    {
        private readonly IVmConnectorConnectionManagementService _vmConnectorConnectionManagementService;
        private readonly IConfiguration _configuration;

        public VmConnectorConnectionManagementController(IConfiguration configuration, IVmConnectorConnectionManagementService vmConnectorConnectionManagementService)
        {
            _configuration = configuration;
            _vmConnectorConnectionManagementService = vmConnectorConnectionManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<DataTableResponseWrapper<VmConnectorConnectionDataTableItemDto>>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetVmConnectorConnectionDataTablePanel")]
        public async Task<IActionResult> GetVmConnectorConnectionDataTablePanel(DataTableFilterModel<GetVmConnectorConnectionDataTableRequestDto> dataTableFilterModel)
        {
            var result = await _vmConnectorConnectionManagementService.GetVmConnectorConnectionDataTablePanel(dataTableFilterModel);
            return this.FromResult(result);
        }
    }
}

// ---- Vm.Api/Controllers/MocppController.cs ----

namespace Vm.Api.Controllers
{
    [ServiceFilter(typeof(VmRequestResponseLogFilterAttribute))]
    public class MocppController : Controller
    {
        private readonly IVmConnectionService _vmConnectionService;

        public MocppController(IVmConnectionService vmConnectionService)
        {
            _vmConnectionService = vmConnectionService;
        }

        [Route("[controller]/{Identifier}")]
        public async Task ConnectionDevice(string Identifier)
        {
            await _vmConnectionService.ConnectionDevice(Identifier);
        }
    }
}
