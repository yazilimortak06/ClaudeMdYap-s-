// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Bank.Api\Startup.cs
using Autofac;
using Autofac.Extras.DynamicProxy;
using Bank.Application.Extentions;
using Bank.Persistence.DbContext;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Api
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
            services.AddRotaWattDbService<PaymentDbContext>(dbcontextOptions);
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
            #region bank api icin filter di
            services.AddFilters();
            #endregion

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion
            #region MassTransit-RabbitMQ configuration
            services.RegisterMasstransit();
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
