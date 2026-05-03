// startup_pattern.cs
// sarj_backend_dotnet — Startup.cs Yapısal Pattern
// Her microservice API projesi bu pattern'ı BaseStartup üzerinden uygular.
// Gerçek servis adları yerine açıklayıcı generic isimler kullanılmıştır.

using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FrameworkCore.Bases.StartupBase;

namespace [ServiceName].Api
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

            // Veritabanı context ve connection string kaydı
            services.AddProjectDbService(Configuration);

            // ASP.NET Core MVC, routing, CORS, versioning kayıtları
            services.AddProjectApiService(Configuration);

            // AutoMapper profil kayıtları
            services.AddProjectAutoMapperService();

            // Action filter'lar (RequestResponseFilter, vb.)
            services.AddFilters();

            // Framework genelinde kullanılan servisler (ProblemDetails, vb.)
            services.AddFrameworkServices(Configuration);

            // MassTransit + RabbitMQ consumer kayıtları
            services.RegisterMasstransit(Configuration);
        }

        // ---------------------------------------------------------------
        // 2. Autofac ContainerBuilder — Repository ve Service kayıtları
        // ---------------------------------------------------------------
        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // Repository interface/implementation eşleştirmeleri
            builder.ConfigureRepositories();

            // Application service interface/implementation eşleştirmeleri
            // Interceptor zincirleri burada kurulur (logging, transaction)
            builder.ConfigureServices();
        }

        // ---------------------------------------------------------------
        // 3. IApplicationBuilder — Middleware pipeline
        // ---------------------------------------------------------------
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            // Temel middleware başlatma (routing, auth, vb.)
            app.ConfigureBuilderInit(env);

            // Swagger UI ve endpoint
            app.UseSwaggerBuilder(Configuration);

            // SignalR hub mapping (gerekli servislerde)
            app.UseSignalRBuilder();

            // Global hata yakalama (ProblemDetails formatı)
            app.UseErrorBuilder(env);
        }
    }
}

// ---------------------------------------------------------------
// Program.cs — Generic Host yapılandırması
// ---------------------------------------------------------------
// using Autofac.Extensions.DependencyInjection;

/*
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            // Autofac'ı DI container olarak kullan
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
*/
