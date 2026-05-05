// startup_pattern.cs
// crm_backend — Startup.cs Yapısal Pattern
// Ana CRM API ve tüm microservislerde bu pattern uygulanır.
// Infrastructure katmanı eklenmesi sarj_backend_dotnet'ten farklıdır.

using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FrameworkCore.Bases.StartupBase;

namespace PixdinnCrm.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        // ---------------------------------------------------------------
        // 1. IServiceCollection — Framework servislerini kaydet
        // ---------------------------------------------------------------
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // Veritabanı context ve connection string
            services.AddProjectDbService(Configuration);

            // ASP.NET Core MVC, routing, CORS, versioning
            services.AddProjectApiService(Configuration);

            // AutoMapper profil kayıtları
            services.AddProjectAutoMapperService();

            // Action filter'lar
            services.AddFilters();

            // Framework genelinde servisler (ProblemDetails vb.)
            services.AddFrameworkServices(Configuration);

            // MassTransit + RabbitMQ consumer kayıtları
            services.RegisterMasstransit(Configuration);

            // JWT Authentication — TokenService ile entegrasyon
            services.AddJwtAuthentication(Configuration);
        }

        // ---------------------------------------------------------------
        // 2. Autofac ContainerBuilder — Repository, Service, Infrastructure
        // ---------------------------------------------------------------
        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // Repository interface/implementation eşleştirmeleri
            builder.ConfigureRepositories();

            // Application service kayıtları (interceptor zincirleri dahil)
            builder.ConfigureServices();

            // Infrastructure servis kayıtları
            // (Email, SMS, 3. parti entegrasyonlar)
            builder.ConfigureInfrastructure();
        }

        // ---------------------------------------------------------------
        // 3. IApplicationBuilder — Middleware pipeline
        // ---------------------------------------------------------------
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            app.ConfigureBuilderInit(env);

            // JWT middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Swagger UI
            app.UseSwaggerBuilder(Configuration);

            // Global hata yakalama (ProblemDetails formatı)
            app.UseErrorBuilder(env);
        }
    }
}

// ---------------------------------------------------------------
// Infrastructure Kayıt Extension Metodu
// (PixdinnCrm.Infrastructure katmanında tanımlanır)
// ---------------------------------------------------------------
/*
public static class InfrastructureRegistration
{
    public static void ConfigureInfrastructure(this ContainerBuilder builder)
    {
        // E-posta servisi
        builder.RegisterType<SmtpEmailService>()
               .As<IEmailService>()
               .InstancePerLifetimeScope();

        // SMS servisi
        builder.RegisterType<NetgsmSmsService>()
               .As<ISmsService>()
               .InstancePerLifetimeScope();

        // Dosya servisi HTTP client
        builder.RegisterType<FileServiceClient>()
               .As<IFileServiceClient>()
               .InstancePerLifetimeScope();

        // Token servisi HTTP client
        builder.RegisterType<TokenServiceClient>()
               .As<ITokenServiceClient>()
               .InstancePerLifetimeScope();
    }
}
*/

// ---------------------------------------------------------------
// Log Servisi Startup (sadece RabbitMQ consumer — HTTP endpoint yok)
// ---------------------------------------------------------------
/*
namespace PixdinnCrmLogService
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // MongoDB bağlantısı
            services.AddMongoDb(Configuration);

            // RabbitMQ consumer kayıtları
            services.RegisterMasstransit(Configuration);

            // Log servisinin API endpoint'i yok — MVC ekleme
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.RegisterType<MongoLogRepository>().As<ILogRepository>().SingleInstance();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            // Sadece health check endpoint (opsiyonel)
            app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/health"));
        }
    }
}
*/

// ---------------------------------------------------------------
// Program.cs — Her microservice için aynı yapı
// ---------------------------------------------------------------
/*
public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
*/
