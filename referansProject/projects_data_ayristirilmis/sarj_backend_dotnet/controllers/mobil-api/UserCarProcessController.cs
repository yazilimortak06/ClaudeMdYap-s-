// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Mobil.Api\Controllers\UserCarProcessController.cs
using Api.Application.Filters.MobilApi;
using FrameworkCore.FrameworkCore.FilterAttributeCore;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.FrameworkCore.WrapperCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dto.ApiDto.MobilUserCarDtos;
using Shared.Domain.ServiceInterfaces.MobilApiServiceInterfaces.UserCarProcessServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Shared.Domain.Dto.ApiDto.MobilCarDtos;

namespace Mobil.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(MobilApiVersionFilterAttribute))]
    [Authorize]
    [ServiceFilter(typeof(MobilApiAuthenticationFilterAttribute))]
    [ServiceFilter(typeof(MobilApiRequestResponseLogFilterAttribute))]
    public class UserCarProcessController : ControllerBase
    {
        private readonly IUserCarProcessService _userCarProcessService;

        public UserCarProcessController(IUserCarProcessService userCarProcessService)
        {
            _userCarProcessService = userCarProcessService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilAddUserCarResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "AddUserCar")]
        [ValidateFilter]
        public async Task<IActionResult> AddUserCar(MobilAddUserCarRequestDto addUserCarRequest)
        {
            var result = await _userCarProcessService.AddUserCar(addUserCarRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilUpdateUserCarResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "UpdateUserCar")]
        [ValidateFilter]
        public async Task<IActionResult> UpdateUserCar(MobilUpdateUserCarRequestDto updateUserCarRequest)
        {
            var result = await _userCarProcessService.UpdateUserCar(updateUserCarRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilPrepareUserCarUpdateFromResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "PrepareUserCarUpdateFrom")]
        [ValidateFilter]
        public async Task<IActionResult> PrepareUserCarUpdateFrom(MobilPrepareUserCarUpdateFromRequestDto prepareUserCarUpdateFromRequest)
        {
            var result = await _userCarProcessService.PrepareUserCarUpdateFrom(prepareUserCarUpdateFromRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetCarBrandForSelectResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarBrands")]
        [ValidateFilter]
        public async Task<IActionResult> GetCarBrands(MobilGetCarBrandForSelectRequestDto getCarBrandForSelectRequest)
        {
            var result = await _userCarProcessService.GetCarBrands(getCarBrandForSelectRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetCarModelForSelectResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarModels")]
        [ValidateFilter]
        public async Task<IActionResult> GetCarModels(MobilGetCarModelForSelectRequestDto getCarModelForSelectRequest)
        {
            var result = await _userCarProcessService.GetCarModels(getCarModelForSelectRequest);
            return this.FromMobilResult(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Result<MobilGetCarTypeForSelectResponseDto>), statusCode: 200)]
        [SwaggerOperation(OperationId = "GetCarTypes")]
        [ValidateFilter]
        public async Task<IActionResult> GetCarTypes(MobilGetCarTypeForSelectRequestDto getCarTypeForSelectRequest)
        {
            var result = await _userCarProcessService.GetCarTypes(getCarTypeForSelectRequest);
            return this.FromMobilResult(result);
        }
    }
}
