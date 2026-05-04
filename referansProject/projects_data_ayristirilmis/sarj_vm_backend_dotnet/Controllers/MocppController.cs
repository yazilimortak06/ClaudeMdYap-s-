// ORIJINAL DOSYA: src/Presentation/Vm.Api/Controllers/MocppController.cs

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
