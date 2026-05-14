using Autofac;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocpp.Application.Extentions;
using Ocpp.Persistence.DbContext;
using System;
using System.Collections.Generic;

namespace Ocpp.Api
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
            #region Db configurasyonlar ayarlaniyor
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<OcppDbContext>(dbcontextOptions);
            #endregion
            #region Api configurasyonlar ayarlaniyor
            services.AddRotaWattApiService(Configuration,
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion
            #region Automapper configurasyonlar ayarlaniyor
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            #endregion
            #region filter register
            services.AddFilters();
            #endregion
            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
            #endregion
            #region Temel framework servisleri
            services.AddFrameworkServices();
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
            var webSocketOptions = new WebSocketOptions()
            {
                ReceiveBufferSize = 8 * 1024,
                KeepAliveInterval = TimeSpan.FromMinutes(10)
            };
            app.UseWebSockets(webSocketOptions);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
