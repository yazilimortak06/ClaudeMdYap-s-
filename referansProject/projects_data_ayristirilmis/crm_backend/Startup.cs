// KAYNAK: E:\Projeler\Backend\PixdinnCrm\PixdinnCrmProjectBackEnd\API\Startup.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using PixdinnCrm.Application.Extentions;
using MassTransit;
using FrameworkCore.FrameworkCore.RabbitMq.Models;
using Refit;
using FrameworkCore.FrameworkCore.Refit.Service;
using System.Text.Json.Serialization;
using FrameworkCore.RabbitMq.Common;

namespace API
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
            #region Db configurasyonları ayarlanıyor
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(FrameworkCore.Enums.UsingDbType.MSSQL,
                                GetAppSettingValue("ConnectionStrings:PixdinnConnectionStringTest"),
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddPixdinnDbService<PixdinnCrmDbContext>(dbcontextOptions);
            #endregion

            #region Api configurasyonları ayarlanıyor
            services.AddPixdinnApiService(Configuration,
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion

            #region Automapper configurasyonları ayarlanıyor
            services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            #region Validator
            services.AddFluentValidators();
            #endregion

            #region MassTransit-RabbitMQ configuration
            services.AddRabbitMqServices(new RabbitMqConfigModel()
            {
                HostAddress = GetAppSettingValue("EventBusSettings:HostAddress")
            });
            #endregion

            #region refit
            services.AddRefitClient<IRefitService>(new RefitSettings { CollectionFormat = CollectionFormat.Csv })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(GetAppSettingValue("FileApi:Url")));

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
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
            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);
        }
    }
}
