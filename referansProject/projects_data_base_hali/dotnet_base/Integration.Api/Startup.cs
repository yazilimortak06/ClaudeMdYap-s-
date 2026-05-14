// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Integration.Api\Startup.cs

using Autofac;
using FrameworkCore.Bases.StartupBase;
using Integration.Application.Extentions;
using Integration.Persistence.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Integration.Api
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
            #region Db konfigürasyonları ayarlanıyor
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<IntegrationDbContext>(dbcontextOptions);
            #endregion

            #region Api konfigürasyonları ayarlanıyor
            services.AddRotaWattApiService(Configuration,
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion

            #region Automapper konfigürasyonları ayarlanıyor
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            #endregion

            #region filter attr register
            services.AddFilters();
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
            #endregion

            #region context provider
            services.AddContextProvider();
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
