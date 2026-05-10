// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\WorkerService.Api\Startup.cs
using Autofac;
using FrameworkCore.Bases.BaseRepository;
using FrameworkCore.Bases.StartupBase;
using FrameworkCore.FrameworkCore.Enums;
using FrameworkCore.FrameworkCore.Repository;
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
using WorkerService.Application.Extentions;
using WorkerService.Persistence.DbContext;

namespace WorkerService.Api
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
              : base(configuration, env)
        {
            base.ProjectPrefix = GetAppSettingValue("StartupConfigs:ProjectPrefix");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Db configurasyonlari ayarlaniyor
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:RotaWattConnectionString"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddRotaWattDbService<WorkerServiceDbContext>(DepencyInjectionType.TRANSIENT, dbcontextOptions);
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

            #region Filters
            services.AddFilters();
            #endregion

            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
            #endregion
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ConnectedRepository<>)).As(typeof(IRepository<>))
           .InstancePerDependency();
            builder.RegisterAssemblyTypes(ApiOptions.RegistrationAssemblies.ToArray()).
                Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.ConfigureServices(ApiOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
