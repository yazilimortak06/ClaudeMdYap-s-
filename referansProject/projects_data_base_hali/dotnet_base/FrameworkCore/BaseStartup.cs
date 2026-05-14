// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\StartupBase\BaseStartup.cs

using FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.Bases.StartupBase
{
    public class BaseStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public ApiOptions ApiOptions { get; set; }
        public string ProjectPrefix;

        public BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
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

        /// <summary>
        /// DbContext için options builder döndürür.
        /// </summary>
        public Action<DbContextOptionsBuilder> GetDbContextOption(UsingDbType dbType, string connection, string migrationassembly)
        {
            DbContextOptionsBuilder dbBuilder = new DbContextOptionsBuilder();
            if (dbType == UsingDbType.MSSQL)
            {
                return dbBuilder => dbBuilder.UseSqlServer(connection, b => b.MigrationsAssembly(migrationassembly).UseNetTopologySuite());
            }
            else if (dbType == UsingDbType.POSTGRESQL)
            {
                // return dbBuilder => dbBuilder.UseNpgsql(connection, b => b.MigrationsAssembly(migrationassembly));
            }
            return null;
        }

        /// <summary>
        /// appsettings'den değer okur.
        /// </summary>
        public string GetAppSettingValue(string name)
        {
            if (Configuration.GetSection(name) == null)
            {
                return "";
            }
            return Configuration.GetSection(name).Value;
        }

        /// <summary>
        /// Temel middleware pipeline kurulumunu yapar (CORS, Routing).
        /// </summary>
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
