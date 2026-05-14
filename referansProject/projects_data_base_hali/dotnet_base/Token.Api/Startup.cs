// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Tocken.Api\Startup.cs
// NOT: Projede "Tocken" yazılmış (typo), referans için orijinal hali korundu.

using Autofac;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tocken.Application.Extentions;
using Tocken.Persistence.DbContext;

namespace Tocken.Api
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
            #region Db konfigürasyonları
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<TockenDbContext>(dbcontextOptions);
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

            #region Filter dependency injections
            services.AddFilters();
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            #region Context provider (current user bilgisi için)
            services.AddContextProvider();
            #endregion

            #region MassTransit-RabbitMQ konfigürasyonu
            services.RegisterRabbitmq();
            #endregion
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
