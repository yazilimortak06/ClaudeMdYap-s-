// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Presentation\Vm.Api\Controllers\MocppController.cs
// WebSocket entry point controller — tek endpoint: /Mocpp/{Identifier}
// NOT: OCPP cihazları bu URL ile VM'e bağlanır

using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.ConnectionManagementsServiceInterfaces;
using System.Threading.Tasks;
using Vm.Application.Filters;

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
