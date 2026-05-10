// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Notification.Api\Startup.cs
using Autofac;
using FrameworkCore.Bases.StartupBase;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Application.Extensions;
using Notification.Application.Hubs.ChargeHubs;
using Notification.Application.Hubs.PaymentHubs;
using Notification.Application.Hubs.StationHubs;
using Notification.Application.Hubs.SupportHubs;
using Notification.Persistence.DbContext;
using System;
using System.Collections.Generic;

namespace Notification.Api
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
            #region Db configurasyonlari ayarlaniyor
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<NotificationDbContext>(dbcontextOptions);
            #endregion

            #region Api configurasyonlari ayarlaniyor
            services.AddRotaWattApiService(Configuration,
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion

            #region Automapper configurasyonlari ayarlaniyor
            services.AddRotaWattAutoMapperService(ApiOptions.RegistrationAssemblies);
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            services.AddFilters();

            #region MassTransit-RabbitMQ configuration
            services.RegisterRabbitMq();
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy(name: GetAppSettingValue("StartupConfigs:Policy"),
                                  builder =>
                                  {
                                      builder.WithOrigins(GetAppSettingValue("SignalR:AllowLocalUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                                      builder.WithOrigins(GetAppSettingValue("SignalR:AllowApiUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                                      builder.WithOrigins(GetAppSettingValue("SignalR:AllowDomainUrl")).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                                  });
            });

            services.AddSignalR();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
             );
            app.UseCors(builder => builder.WithOrigins(GetAppSettingValue("SignalR:AllowLocalUrl"), GetAppSettingValue("SignalR:AllowApiUrl"), GetAppSettingValue("SignalR:AllowDomainUrl"), "*").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChargePointConnectionStateHub>("/deviceConnectionStateNotification");
                endpoints.MapHub<ConnectorStateHub>("/connectorStateNotification");
                endpoints.MapHub<SocketHub>("/deviceSocketNotification");
                endpoints.MapHub<ChargeHub>("/chargeNotification");
                endpoints.MapHub<DeviceStateHub>("/deviceStateNotification");
                endpoints.MapHub<PaymentHub>("/paymentNotification");
                endpoints.MapHub<PaymentHub>("/automaticPaymentNotification");
                endpoints.MapHub<SupportHub>("/supportNotification");
                endpoints.MapHub<SupportNotificationHub>("/supportForPanelNotification");
            });
        }
    }
}
