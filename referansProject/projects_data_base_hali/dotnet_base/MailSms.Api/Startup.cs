using Autofac;
using FrameworkCore.Bases.StartupBase;
using MailSms.Application.Extentions;
using MailSms.Persistence.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MailSms.Api
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
            services.AddRotaWattDbService<MailSmsDbContext>(dbcontextOptions);
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
            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion
            #region Validator
            services.AddFluentValidators();
            #endregion
            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
            #endregion
            #region Filters
            services.AddFilters();
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
