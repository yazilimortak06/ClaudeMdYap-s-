// ============================================================
// perde_iot_backend — GERCEK Startup.cs ve Program.cs
// NOT: startup_pattern.cs = tahmini/orneksel, bu dosya = gercek kaynak kodu
// Kaynak: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\
// ============================================================

// ---- Program.cs (gercek) ----

using Autofac.Extensions.DependencyInjection;
using Autofac;
using PixdinnPerdeci;
using System.Reflection;
using PixdinnPerdeci.Bases.StartupBase;

var builder = WebApplication.CreateBuilder(args);

#region autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.ConfigureServices(new ApiOptions()
    {
        ApiClientList = null,
        ApiName = "PixdinnPerdeci",
        // "PixdinnPerdeci" veya "Shared" ile baslayan DLL'leri yükle
        RegistrationAssemblies = Directory.EnumerateFiles(
                AppDomain.CurrentDomain.BaseDirectory,
                "*.dll",
                SearchOption.TopDirectoryOnly)
           .Where(filePath =>
               Path.GetFileName(filePath).StartsWith("PixdinnPerdeci") ||
               Path.GetFileName(filePath).StartsWith("Shared"))
           .Select(Assembly.LoadFrom)
    });
});
#endregion

#region startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); // IServiceCollection kayitlari
builder.Services.AddControllersWithViews();  // MVC controller + view destegi
var app = builder.Build();
startup.Configure(app, builder.Environment); // Middleware pipeline
#endregion

// ---- Startup.cs (gercek) ----

using Autofac;
using PixdinnPerdeci.Bases.Models;
using PixdinnPerdeci.Bases.StartupBase;

namespace PixdinnPerdeci
{
    public class Startup : BaseStartup
    {
        public ApiOptions ApiOptions { get; set; }

        public Startup(IConfiguration configuration) : base(configuration)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddRazorPages();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            ConfigureBuilderInit(app, env);
            // app.UseHttpsRedirection();  // Devre disi
            // app.UseStaticFiles();        // Devre disi (yorum satiri)
            // app.UseRouting();            // ConfigureBuilderInit icinde yapiliyor
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}

// ---- BaseStartup.cs (gercek) ----

using PixdinnPerdeci.Bases.Models;
using System.Reflection;

namespace PixdinnPerdeci.Bases.StartupBase
{
    public class BaseStartup
    {
        public IConfiguration Configuration { get; }
        public ApiOptions ApiOptions { get; set; }
        public string ProjectPrefix;

        public BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            // Not: ProjectPrefix bu noktada bos olabilir (kalitimdan once set edilmeli)
            ApiOptions = new ApiOptions()
            {
                ApiClientList = null,
                ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
                RegistrationAssemblies = Directory.EnumerateFiles(
                        AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                   .Where(filePath =>
                       Path.GetFileName(filePath).StartsWith(ProjectPrefix) ||
                       Path.GetFileName(filePath).StartsWith("Shared"))
                   .Select(Assembly.LoadFrom)
            };
        }

        public string GetAppSettingValue(string name)
        {
            if (Configuration.GetSection(name) == null) return "";
            return Configuration.GetSection(name).Value;
        }

        public void ConfigureBuilderInit(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("_myAllowSpecificOrigins");
            app.UseRouting();
        }
    }
}

// ---- ApiConfigureBaseContainerExtensions.cs (gercek) ----

using Autofac;
using PixdinnPerdeci.Bases.Models;

namespace PixdinnPerdeci.Bases.StartupBase
{
    public static class ApiConfigureBaseContainerExtensions
    {
        public static void ConfigureServices(this ContainerBuilder builder, ApiOptions options)
        {
            // Convention: "Service" ile biten siniflar -> implement ettigi interface'lerle kayit
            builder.RegisterAssemblyTypes(options.RegistrationAssemblies.ToArray())
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();
        }
    }
}

// ---- ApiOptions.cs (gercek) ----

using System.Reflection;

namespace PixdinnPerdeci.Bases.Models
{
    public class ApiOptions
    {
        public IEnumerable<Assembly> RegistrationAssemblies { get; set; }
        public IEnumerable<Type> HubListSignalR { get; set; }
        public IEnumerable<Type> ApiClientList { get; set; }
        public string ApiName { get; set; }
    }
}

// ---- HomeController.cs (gercek, tek mevcut controller) ----

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

// ---- ErrorViewModel.cs (tek model) ----

namespace PixdinnPerdeci.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
