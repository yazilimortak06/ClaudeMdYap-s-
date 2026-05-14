// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Web.Api\Startup.cs
using Api.Persistence.DbContext;
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
using Api.Application.Extentions;

namespace Web.Api
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
            services.AddRotaWattDbService<RotaWattDbContext>(dbcontextOptions);
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

            #region Validator
            services.AddFluentValidators();
            #endregion

            #region fitlers dependency injections
            services.AddFilters();
            #endregion

            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
            #endregion

            #region jwt register
            services.AddWebJwtTocken(Configuration);
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
