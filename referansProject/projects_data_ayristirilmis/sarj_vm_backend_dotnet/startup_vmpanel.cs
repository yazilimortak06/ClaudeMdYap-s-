// ============================================================
// sarj_vm_backend_dotnet — VmPanel.Api Startup (tam kod)
// Kaynak: src/Presentation/VmPanel.Api/Startup.cs
// ============================================================

using Autofac;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Vm.Persistence.DbContext;
using VmPanel.Application.Extentions;

namespace VmPanel.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
            base.RegistirationPrefixList = base.Configuration
                .GetSection("StartupConfigs:RegistirationPrefixList").Get<string[]>();
            ConfigureApiOptions();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ---- DB ----
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(
                FrameworkCore.Enums.UsingDbType.POSTGRESQL,
                GetAppSettingValue("ConnectionStrings:RotaWattPostgreConnectionString"),
                GetAppSettingValue("StartupConfigs:MigrationAssembly")
            ));
            services.AddRotaWattDbService<VmDbContext>(dbcontextOptions);

            // ---- API temel servisler ----
            services.AddRotaWattApiService(
                Configuration,
                WebHostEnvironment,
                GetAppSettingValue("StartupConfigs:Policy"),
                GetAppSettingValue("StartupConfigs:ApiUrl")
            );

            // ---- Automapper ----
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);

            // ---- Filters ----
            services.AddFilters();

            // ---- Framework core servisleri ----
            services.AddFrameworkServices();

            // ---- JWT/Auth ----
            services.AddWebJwtTocken(Configuration);
            services.AddAuthorization();

            // ---- HttpContext/Context Provider ----
            services.AddContextProvider();

            // ---- MassTransit/RabbitMQ ----
            services.RegisterMasstransit();

            // ---- CORS (SPA icin) ----
            var spaOrigin = GetAppSettingValue("StartupConfigs:SpaOrigin");
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpa", builder =>
                {
                    if (!string.IsNullOrWhiteSpace(spaOrigin))
                        builder.WithOrigins(spaOrigin);
                    else
                        builder.WithOrigins("http://localhost:4200");

                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            // ---- Session ----
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowSpa");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSignalRBuilder(provider, ApiOptions);
        }
    }
}

// ============================================================
// sarj_vm_backend_dotnet — Vm.Api Startup (tam kod)
// Kaynak: src/Presentation/Vm.Api/Startup.cs
// ============================================================

using Autofac;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Vm.Application.Extentions;
using Vm.Persistence.DbContext;

namespace Vm.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.POSTGRESQL,
                GetAppSettingValue("ConnectionStrings:RotaWattPostgreConnectionString"),
                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<VmDbContext>(dbcontextOptions);

            services.AddRotaWattApiService(Configuration,
                WebHostEnvironment,
                GetAppSettingValue("StartupConfigs:Policy"),
                GetAppSettingValue("StartupConfigs:ApiUrl"));

            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFilters();
            services.RegisterMasstransit();
            services.AddFrameworkServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
            builder.RegisterHandlers();  // <-- OCPP mesaj handler'lari
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);

            // WebSocket ayarlari — OCPP icin kritik
            var webSocketOptions = new WebSocketOptions()
            {
                ReceiveBufferSize = 32 * 1024,              // 32KB buffer
                KeepAliveInterval = TimeSpan.FromSeconds(30) // 30 sn ping
            };
            app.UseWebSockets(webSocketOptions);

            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
