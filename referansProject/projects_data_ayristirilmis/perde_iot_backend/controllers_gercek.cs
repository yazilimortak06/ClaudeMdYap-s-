// ============================================================
// perde_iot_backend — GERCEK CONTROLLER'LAR
// NOT: Proje henuz iskelet asamasinda. Tek gercek controller HomeController'dir.
// Kaynak: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Controllers\
// ============================================================

// ---- HomeController.cs (tek mevcut, gercek kaynak) ----

using Microsoft.AspNetCore.Mvc;
using PixdinnPerdeci.Models;
using System.Diagnostics;

namespace PixdinnPerdeci.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

// ============================================================
// DURUM OZETI:
// Services/Class.cs          -> bos (placeholder)
// ServiceInterfaces/Class.cs -> bos (placeholder)
// Models/ErrorViewModel.cs   -> tek gercek model
//
// Proje IoT perde mantigi icin hazir altyapiya sahip:
// - Autofac convention kayit (Service ile biten -> interface)
// - BaseStartup kalitimi
// - AutoMapper
// - CORS ("_myAllowSpecificOrigins")
// - .NET 8 (en guncel)
//
// Eklenecek controller'lar: DeviceController, CurtainController vb.
// ============================================================
