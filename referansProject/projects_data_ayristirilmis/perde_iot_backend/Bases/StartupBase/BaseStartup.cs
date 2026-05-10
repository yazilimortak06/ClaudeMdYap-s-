// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Bases\StartupBase\BaseStartup.cs

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
            ApiOptions = new ApiOptions()
            {
                ApiClientList = null,
                ApiName = GetAppSettingValue("StartupConfigs:ApiName"),
                RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory,
                        "*.dll", SearchOption.TopDirectoryOnly)
               .Where(filePath => Path.GetFileName(filePath).StartsWith(ProjectPrefix) || Path.GetFileName(filePath).StartsWith("Shared"))
               .Select(Assembly.LoadFrom)
            };
        }

        public string GetAppSettingValue(string name)
        {
            if (Configuration.GetSection(name) == null)
            {
                return "";
            }
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
