// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Bases\StartupBase\ApiConfigureBaseContainerExtensions.cs

using Autofac;
using PixdinnPerdeci.Bases.Models;

namespace PixdinnPerdeci.Bases.StartupBase
{

    public static class ApiConfigureBaseContainerExtensions
    {
        public static void ConfigureServices(this ContainerBuilder builder, ApiOptions options)
        {
            // Autofac ile tüm assembly'ler taranır, ismi "Service" ile biten sınıflar otomatik kayıt edilir
            builder.RegisterAssemblyTypes(options.RegistrationAssemblies.ToArray()).
                  Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
        }
    }
}
