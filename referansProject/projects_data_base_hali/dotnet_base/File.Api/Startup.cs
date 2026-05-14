// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\File.Api\Startup.cs

using Autofac;
using DinkToPdf.Contracts;
using DinkToPdf;
using File.Application.Extentions;
using File.Persistence.DbContext;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace File.Api
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
            services.AddRotaWattDbService<FileDbContext>(dbcontextOptions);
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

            #region Temel framework servisleri
            services.AddFrameworkServices();
            #endregion

            #region Fluent Validation
            services.AddFluentValidators();
            #endregion

            #region Filter dependency injections
            services.AddFilters();
            #endregion

            // PDF dönüşümü için DinkToPdf singleton olarak register edilir
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureRepositories(ApiOptions);
            builder.ConfigureServices(ApiOptions);
        }

        // HTTP request pipeline konfigürasyonu.
        // Uploaded_Files klasörü static file serving ile dışarıya açılır.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            ConfigureBuilderInit(app, env);

            app.UseSwaggerBuilder(provider, ApiOptions);
            app.UseSignalRBuilder(provider, ApiOptions);
            app.UseErrorBuilder(provider, ApiOptions);

            // Yüklenen dosyaları /Uploaded_Files path'i üzerinden sunmak için
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Uploaded_Files")),
                RequestPath = "/Uploaded_Files"
            });
        }
    }
}
