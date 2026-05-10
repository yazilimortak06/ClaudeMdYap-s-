// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Startup.cs

using Autofac;
using PixdinnPerdeci.Bases.Models;
using PixdinnPerdeci.Bases.StartupBase;

namespace PixdinnPerdeci
{
    public class Startup : BaseStartup
    {
        public ApiOptions ApiOptions { get; set; }

        public Startup(IConfiguration configuration)
                : base(configuration)
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
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
