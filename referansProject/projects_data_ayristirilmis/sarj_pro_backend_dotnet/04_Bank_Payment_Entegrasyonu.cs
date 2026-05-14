// KAYNAK: E:\Projeler\Backend\SarjAllPro\src\Core\Applications\Bank.Application\
//         E:\Projeler\Backend\SarjAllPro\src\Core\Persistences\Bank.Persistence\
//         E:\Projeler\Backend\SarjAllPro\src\Presentation\Bank.Api\
// Ödeme entegrasyonu — Bank.Api servisi (Moka / banka ödeme gateway)

// ============================================================
// FILE: Bank.Api\Startup.cs
// ============================================================
// Yapılandırma: PaymentDbContext (MSSQL), Autofac DI, Refit HTTP client
using Autofac;
using Autofac.Extras.DynamicProxy;
using Bank.Application.Extentions;
using Bank.Persistence.DbContext;
using FrameworkCore.Bases.StartupBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            // DB konfigürasyonu
            var dbcontextOptions = new List<Action<DbContextOptionsBuilder>>();
            dbcontextOptions.Add(GetDbContextOption(
                FrameworkCore.Enums.UsingDbType.MSSQL,
                GetAppSettingValue("ConnectionStrings:PixdinnConnectionString"),  // connection string gizlendi
                GetAppSettingValue("StartupConfigs:MigrationAssembly")));
            services.AddPixdinnDbService<PaymentDbContext>(dbcontextOptions);

            services.AddPixdinnApiService(Configuration, WebHostEnvironment,
                GetAppSettingValue("StartupConfigs:Policy"),
                GetAppSettingValue("StartupConfigs:ApiUrl"));

            services.AddPixdinnAutoMapperService(ApiOptions.RegistrationAssemblies);
            services.AddFilters();       // BankApiRequestResponseFilterAttribute
            services.AddFrameworkServices();
            services.RegisterMasstransit();   // ILogProducer kaydı
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

// ============================================================
// FILE: Bank.Persistence\DbContext\PaymentDbContext.cs
// ============================================================
// NOT: Entity Fluent ve repository'ler henüz boş — altyapı kurulmuş ama impl yok
using FrameworkCore.Bases.BaseUnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Bank.Persistence.DbContext
{
    public class PaymentDbContext : UnitOfWork
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Entity Fluent konfigürasyonları — henüz boş
            // Data seeding — henüz boş
        }
    }
}

// ============================================================
// ÖDEME ENTEGRASYONUNDAKİ DURUM ANALİZİ:
// ============================================================
/*
Bank.Application:
  - Filters/BankApiRequestResponseFilterAttribute.cs — request/response log filter
  - Extentions/FilterRegister.cs — DI kaydı
  - Extentions/RabbitMqRegister.cs — ILogProducer kaydı
  - Services/ — BOŞ (implementasyon yok)
  - Interceptors/ — BOŞ
  - RabbitMq/Consumers/ — BOŞ

Bank.Persistence:
  - DbContext/PaymentDbContext.cs — sadece boş context
  - EntityFluent/ — BOŞ
  - Repositories/ — BOŞ
  - RefitStubs.g.cs mevcut (Refit HTTP client stubs oluşturulmuş ama interface yok)

Bank.Api:
  - Startup.cs — PaymentDbContext bağlı
  - Controllers yok (API endpoint yok henüz)

SONUÇ: Bank/Ödeme altyapısı kurulmuş ama Moka veya başka bir ödeme gateway implementasyonu
yok. Refit stubs oluşturulmuş olması HttpClient tabanlı bir ödeme API'ye bağlanılacağını
(muhtemelen Moka Ödeme API) gösteriyor ancak interface tanımları kaynak kodda bulunamadı.

Benzer şekilde Integration.Persistence de Refit stubs içeriyor — harici servis entegrasyonu planlandı.
*/

// ============================================================
// Integration.Application\Extentions\RabbitMqRegister.cs
// ============================================================
// KAYNAK: E:\Projeler\Backend\SarjAllPro\src\Core\Applications\Integration.Application\Extentions\RabbitMqRegister.cs
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.RabbitMq.Producers;
using Shared.Domain.RabbitMq.ProducersInterfaces;

namespace Integration.Application.Extentions
{
    public static class RabbitMqRegister
    {
        public static IServiceCollection RegisterMasstransit(this IServiceCollection services)
        {
            services.AddSingleton<ILogProducer, LogProducer>();
            return services;
        }
    }
}
