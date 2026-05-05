// startup_pattern.cs
// perde_iot_backend — Startup.cs ve Program.cs Yapısal Pattern
// .NET 8 MVC + Autofac + BaseStartup kalıtımı
// sarj_backend_dotnet'ten fark: MVC (view destekli), API-only değil

using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FrameworkCore.Bases.StartupBase;

namespace PixdinnPerdeci
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        // ---------------------------------------------------------------
        // 1. IServiceCollection — MVC ve uygulama servislerini kaydet
        // ---------------------------------------------------------------
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // ASP.NET Core MVC (View desteği dahil)
            services.AddControllersWithViews();

            // AutoMapper — Assembly tarama ile profil kayıtları
            services.AddAutoMapper(typeof(Startup));

            // Session desteği (controller'larda oturum yönetimi)
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // HTTP Context erişimi (controller dışında kullanım için)
            services.AddHttpContextAccessor();

            // SignalR — Real-time cihaz durumu güncellemeleri
            services.AddSignalR();
        }

        // ---------------------------------------------------------------
        // 2. Autofac ContainerBuilder — Dinamik DLL yükleme ile kayıt
        // ---------------------------------------------------------------
        public override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // Dinamik assembly tarama — kendi namespace'inden DLL'leri yükle
            // NOT: "*.*" yerine proje-spesifik pattern kullan
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("PixdinnPerdeci"))
                .ToArray();

            // ServiceInterfaces'i implement eden tipleri otomatik kaydet
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }

        // ---------------------------------------------------------------
        // 3. IApplicationBuilder — MVC middleware pipeline
        // ---------------------------------------------------------------
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Static dosyalar (CSS, JS, images — wwwroot/)
            app.UseStaticFiles();

            // Routing
            app.UseRouting();

            // Kimlik doğrulama ve yetkilendirme
            app.UseAuthentication();
            app.UseAuthorization();

            // Session middleware
            app.UseSession();

            // Endpoint yönlendirme
            app.UseEndpoints(endpoints =>
            {
                // MVC varsayılan route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // SignalR hub (real-time cihaz güncellemeleri)
                endpoints.MapHub<DeviceHub>("/deviceHub");
            });
        }
    }
}

// ---------------------------------------------------------------
// Program.cs — .NET 8 ile Minimal API + Autofac
// ---------------------------------------------------------------
/*
var builder = WebApplication.CreateBuilder(args);

// Autofac'ı DI container olarak kullan
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Autofac modüllerini yükle (Startup.ConfigureContainer çağrılır)
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Dinamik DLL yükleme — proje başlarken assembly'ler taranır
    var dllPath = AppDomain.CurrentDomain.BaseDirectory;
    var assemblies = Directory.GetFiles(dllPath, "PixdinnPerdeci*.dll")
        .Select(f => Assembly.LoadFrom(f))
        .ToArray();

    containerBuilder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
});

var app = builder.Build();

// Startup.Configure'daki middleware ayarları burada da yapılabilir
// .NET 8 Minimal API yaklaşımında Startup sınıfı opsiyonel

app.Run();
*/

// ---------------------------------------------------------------
// DeviceHub — SignalR Hub (real-time cihaz durumu)
// ---------------------------------------------------------------
/*
public class DeviceHub : Hub
{
    // Cihaz durumu değiştiğinde tüm bağlı client'lara bildir
    public async Task SendDeviceStatus(string deviceId, string status)
    {
        await Clients.All.SendAsync("DeviceStatusUpdated", deviceId, status);
    }

    // Perde durumu güncelleme
    public async Task SendCurtainPosition(string curtainId, int position)
    {
        await Clients.All.SendAsync("CurtainPositionUpdated", curtainId, position);
    }
}
*/
