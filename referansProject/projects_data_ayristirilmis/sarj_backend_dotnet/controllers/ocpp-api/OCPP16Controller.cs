using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Enums.OcppEnums;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ConnectionInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocpp.Api.Controllers
{
    public class OCPP16Controller : Controller
    {
        private readonly IOcpp16ConnectionService _ocpp16ConnectionService;
        public OCPP16Controller(
            IOcpp16ConnectionService ocpp16ConnectionService
            )
        {
            _ocpp16ConnectionService = ocpp16ConnectionService;
        }
        [Route("[controller]/[action]/{Identifier}")]
        public async Task Connection(string Identifier)
        {
            await _ocpp16ConnectionService.Connection(Identifier);
        }
    }
}
