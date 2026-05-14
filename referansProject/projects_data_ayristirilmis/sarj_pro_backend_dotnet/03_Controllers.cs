// KAYNAK: E:\Projeler\Backend\SarjAllPro\src\Presentation\
// SarjAllPro Controller'ları

// ============================================================
// FILE: Ocpp.Api\Controllers\OCPP16Controller.cs
// ============================================================
// FARK: OcppDeviceTypeEnum.CIRCONTROL sabit geçiyor (VM'de dinamik)
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Enums.OcppEnums;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ConnectionInterfaces;

namespace Ocpp.Api.Controllers
{
    public class OCPP16Controller : Controller
    {
        private readonly IOcpp16ConnectionService _ocpp16ConnectionService;
        public OCPP16Controller(IOcpp16ConnectionService ocpp16ConnectionService)
        {
            _ocpp16ConnectionService = ocpp16ConnectionService;
        }
        [Route("[controller]/[action]/{Identifier}")]
        public async Task Connection(string Identifier)
        {
            await _ocpp16ConnectionService.Connection(Identifier, OcppDeviceTypeEnum.CIRCONTROL);
        }
    }
}

// ============================================================
// FILE: SarjAllMobil.Api\Controllers\ChargeController.cs
// ============================================================
// Mobil uygulama şarj işlemleri
// Endpoints: PrepareChargeProcess, GetSocketListForChargeProcess, SelectSocketForPrepareChargeProcess,
//            StartCharge, GetChargeStatus, StopCharge, GetPayment, GetChargeProcess

using Api.Application.Filters.SarjAllMobilApi;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.SarjAllMobilDto.ChargeService;
using Shared.Domain.Services.SarjAllServices.ChargeInterfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SarjAllMobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    [ServiceFilter(typeof(SarjAllMobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(SarjAllMobilApiRequesResponsetLogFilterAttribute))]
    public class ChargeController : ControllerBase
    {
        private readonly IChargeService _chargeService;
        public ChargeController(IChargeService chargeService) { _chargeService = chargeService; }

        [HttpPost] public async Task<IActionResult> PrepareChargeProcess(PrepareChargeProcessRequestDto req) => this.FromHttpClientResult(await _chargeService.PrepareChargeProcess(req));
        [HttpPost] public async Task<IActionResult> GetSocketListForChargeProcess(GetSocketListRequestDto req) => this.FromHttpClientResult(await _chargeService.GetSocketListForChargeProcess(req));
        [HttpPost] public async Task<IActionResult> SelectSocketForPrepareChargeProcess(SelectSocketForPrepareChargeProcessRequestDto req) => this.FromHttpClientResult(await _chargeService.SelectSocketForPrepareChargeProcess(req));
        [HttpPost] public async Task<IActionResult> StartCharge(StartChargeRequestDto req) => this.FromHttpClientResult(await _chargeService.StartCharge(req));
        [HttpPost] public async Task<IActionResult> GetChargeStatus(GetChargeStatusRequestDto req) => this.FromHttpClientResult(await _chargeService.GetChargeStatus(req));
        [HttpPost] public async Task<IActionResult> StopCharge(StopChargeRequestDto req) => this.FromHttpClientResult(await _chargeService.StopCharge(req));
        [HttpPost] public async Task<IActionResult> GetPayment(GetPaymentRequestDto req) => this.FromHttpClientResult(await _chargeService.GetPayment(req));
        [HttpPost] public async Task<IActionResult> GetChargeProcess(GetChargeProcessSarjAllRequestDto req) => this.FromHttpClientResult(await _chargeService.GetChargeProcess(req));
    }
}

// ============================================================
// FILE: Station.Api\Controllers\ChargeProcessManagmentController.cs
// ============================================================
// NOT: Bazı endpoint'ler henüz implement edilmemiş (stub return)

using Api.Application.Filters.StationApi;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Shared.Domain.Dto.StationApiDto.ConnectorSessionDtos;
using Shared.Domain.Dto.StationApiDto.DeviceSessionDtos;
using Shared.Domain.Errors.StationApiErrors.ChargeProcessManagment;
using Shared.Domain.GeneralAttribute;
using Shared.Domain.GeneralEnums;

namespace Station.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(StationApiRequestInfoFilterAttribute))]
    public class ChargeProcessManagmentController : ControllerBase
    {
        public ChargeProcessManagmentController() { }

        [HttpPost]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetConnectorSession(GetConnectorSessionRequestDto req)
        {
            // STUB - henüz implement edilmemiş
            return this.FromHttpClientResult(new SuccessResult<GetConnectorSessionResponseDto>(new GetConnectorSessionResponseDto() {
                ConnectorSession = new ConnectorSessionDto() { },
                IsExist = false
            }));
        }

        [HttpPost]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetDeviceSession(GetDeviceSessionRequestDto req)
        {
            // STUB - henüz implement edilmemiş — hata döndürüyor
            return this.FromHttpClientResult(new ErrorResult<GetDeviceSessionResponseDto>(
                new GetDeviceSessionResponseDto() { },
                ChargeProcessStationErrorEnum.CHARGE_DEVICE_IS_NOT_CONNECTED));
        }
    }
}

// ============================================================
// FILE: Log.Api\Controllers\RequestResponseController.cs
// ============================================================
// (Bu controller VM projesindeki ile benzer log endpoint)

// ============================================================
// DIĞER CONTROLLER'LAR (SarjAllPro'ya özgü):
// ============================================================
// Ocpp.Api/OcppCommandMessageController        — OCPP komut mesajı yönetimi
// Ocpp.Api/RemoteTransactionController         — Remote Start/Stop Transaction
// Ocpp.Api/SocketMovementController            — Soket hareket takibi
// Ocpp.Api/OcppTriggerMessageManagmentController — TriggerMessage yönetimi
// Station.Api/ChargeDeviceConnectorOcppManagmentController
// Station.Api/ChargeDeviceOcppManagementController
// Station.Api/ChargeProcessOcppManagmentController
// Station.Api/CommandManagmentController
// Station.Api/OCPP16RemoteController
// Web.Api/AuthenticationController, PanelAdminController, PolicyManagmentController vb.
// Tocken.Api/AuthController, PanelUserController, AdminUserTypeController vb.
// SarjAllMobil.Api/StationController, SupportController, TokenController
