// controller_pattern.cs
// crm_backend — Controller Yapısal Pattern
// sarj_backend_dotnet ile aynı temel pattern, CRM'e özgü farklılıklar belirtilmiştir.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FrameworkCore.Bases.BaseAttributes;
using FrameworkCore.FilterAttributeCore;
using FrameworkCore.WrapperCore;
using PixdinnCrm.Application.Services;
using PixdinnCrm.Domain.DTOs.Customer;

namespace PixdinnCrm.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Authorize]  // CRM — tüm controller'lar kimlik doğrulama gerektirir
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        // ---------------------------------------------------------------
        // POST — Yeni müşteri oluştur
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<CustomerResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> Create(CreateCustomerRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // POST — Filtre ile müşteri listesi getir
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<PagedResult<CustomerResponseDto>>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> GetList(CustomerFilterDto filter)
        {
            var result = await _service.GetListAsync(filter);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // POST — ID ile tek müşteri getir
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<CustomerDetailDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> GetById(IdRequestDto request)
        {
            var result = await _service.GetByIdAsync(request.Id);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // POST — Müşteri güncelle
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<CustomerResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> Update(UpdateCustomerRequestDto request)
        {
            var result = await _service.UpdateAsync(request);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // POST — Soft delete
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> Delete(IdRequestDto request)
        {
            var result = await _service.DeleteAsync(request.Id);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // POST — Müşteriye aktivite ekle (CRM'e özgü)
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<ActivityResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        public async Task<IActionResult> AddActivity(AddActivityRequestDto request)
        {
            var result = await _service.AddActivityAsync(request);
            return this.FromHttpClientResult(result);
        }
    }
}

// ---------------------------------------------------------------
// Notlar:
//
// [Authorize]
//   → CRM uygulamasında tüm endpoint'ler kimlik doğrulama gerektirir.
//   → sarj_backend_dotnet'te InnerRequestAttribute vardı; burada JWT [Authorize].
//
// PagedResult<T>
//   → CRM liste sorguları sayfalı döner: { Data: [], TotalCount: N, Page: X }
//
// Soft Delete
//   → CRM varlıkları fiziksel olarak silinmez; IsDeleted = true yapılır.
//   → GetList sorguları IsDeleted = false filtresi uygular.
//
// Token Validation
//   → JWT token TokenService.API tarafından üretilmiş.
//   → Bu API, token'ı JWT Bearer middleware ile doğrular.
// ---------------------------------------------------------------
