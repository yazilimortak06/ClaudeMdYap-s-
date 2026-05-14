// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Log.Api\Startup.cs

using Autofac;
using FrameworkCore.Bases.StartupBase;
using Log.Application.Extensions;
using Log.Persistence.DbContext;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Log.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        // Bu method runtime tarafından çağrılır. Servisleri DI container'a eklemek için kullanılır.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Db konfigürasyonları
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<LogDbContext>(dbcontextOptions);
            #endregion

            #region Api konfigürasyonları
            services.AddRotaWattApiService(Configuration,
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion

            #region Automapper konfigürasyonları
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            #region MassTransit-RabbitMQ konfigürasyonu
            services.RegisterMasstransit(Configuration);
            #endregion
        }

        // Autofac container konfigürasyonu: Repository ve service'leri kayıt eder.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        // Bu method runtime tarafından çağrılır. HTTP request pipeline'ı konfigüre eder.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
