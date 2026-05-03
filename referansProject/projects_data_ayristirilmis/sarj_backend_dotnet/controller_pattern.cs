// controller_pattern.cs
// sarj_backend_dotnet — Controller Yapısal Pattern
// Tüm API projelerindeki controller'lar bu pattern'ı izler.

using Microsoft.AspNetCore.Mvc;
using FrameworkCore.Bases.BaseAttributes;
using FrameworkCore.FilterAttributeCore;
using FrameworkCore.WrapperCore;
using [ServiceName].Application.Services;
using [ServiceName].Domain.Dtos;

namespace [ServiceName].Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class [ResourceName]Controller : ControllerBase
    {
        private readonly I[ResourceName]Service _service;

        public [ResourceName]Controller(I[ResourceName]Service service)
        {
            _service = service;
        }

        // ---------------------------------------------------------------
        // POST Action — Standart pattern
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<[ResourceName]ResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
        public async Task<IActionResult> Create([ResourceName]RequestDto request)
        {
            var result = await _service.Create(request);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // GET Action — Liste sorgulama
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<[ResourceName]ResponseDto>>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB })]
        public async Task<IActionResult> GetList([ResourceName]FilterDto filter)
        {
            var result = await _service.GetList(filter);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // GET by ID — Tekil sorgulama
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<[ResourceName]ResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL, ApiName.WEB, ApiName.STATION })]
        public async Task<IActionResult> GetById(IdRequestDto request)
        {
            var result = await _service.GetById(request.Id);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // PUT Action — Güncelleme
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<[ResourceName]ResponseDto>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> Update([ResourceName]UpdateDto request)
        {
            var result = await _service.Update(request);
            return this.FromHttpClientResult(result);
        }

        // ---------------------------------------------------------------
        // DELETE Action
        // ---------------------------------------------------------------
        [Route("v{version:apiVersion}/[controller]/[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), 200)]
        [ServiceFilter(typeof(RequestResponseFilterAttribute))]
        [InnerRequestAttribute(new ApiName[] { ApiName.WEB })]
        public async Task<IActionResult> Delete(IdRequestDto request)
        {
            var result = await _service.Delete(request.Id);
            return this.FromHttpClientResult(result);
        }
    }
}

// ---------------------------------------------------------------
// Notlar:
//
// [Route("v{version:apiVersion}/[controller]/[action]")]
//   → API versioning desteği: /v1/Payment/Create, /v2/Payment/Create
//
// [ServiceFilter(typeof(RequestResponseFilterAttribute))]
//   → Her request/response çifti loglanır (action filter üzerinden)
//
// [InnerRequestAttribute(new ApiName[] { ApiName.MOBIL })]
//   → Sadece belirtilen iç servislerden gelen istekler kabul edilir
//   → Servisler arası yetkilendirme mekanizması
//
// this.FromHttpClientResult(result)
//   → WrapperCore extension metodu
//   → Result<T> nesnesini uygun HTTP status code'a çevirir
//   → Başarılı: 200 OK + Result<T>
//   → Başarısız: Result içindeki hata kodu/mesajı ile yanıt
// ---------------------------------------------------------------
