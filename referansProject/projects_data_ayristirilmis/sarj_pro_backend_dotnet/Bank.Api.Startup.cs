// ORIJINAL DOSYA: src/Presentation/Bank.Api/Startup.cs (SarjAllPro)
// Credential degerler [MASKED] yapildi
// FARK: AddRotaWatt* -> AddPixdinn* olarak degistirilmis, ConnectionString key farklı

using Autofac;
using Bank.Application.Extentions;
using Bank.Persistence.DbContext;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Bank.Api
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
                                GetAppSettingValue("ConnectionStrings:PixdinnConnectionString"),  // <-- FARK: RotaWattConnectionString degil
                                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddPixdinnDbService<PaymentDbContext>(dbcontextOptions);  // <-- FARK: AddRotaWattDbService degil
            #endregion

            #region Api configurasyonlari ayarlaniyor
            services.AddPixdinnApiService(Configuration,    // <-- FARK
                                    WebHostEnvironment,
                                    GetAppSettingValue("StartupConfigs:Policy"),
                                    GetAppSettingValue("StartupConfigs:ApiUrl"));
            #endregion

            #region Automapper configurasyonlari ayarlaniyor
            services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);  // <-- FARK
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
