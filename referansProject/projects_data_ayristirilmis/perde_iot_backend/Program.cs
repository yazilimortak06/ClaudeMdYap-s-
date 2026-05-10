// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Program.cs

using Autofac.Extensions.DependencyInjection;
using Autofac;
using PixdinnPerdeci;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder.Extensions;
using PixdinnPerdeci.Core.Models;
using PixdinnPerdeci.Bases.StartupBase;


var builder = WebApplication.CreateBuilder(args);

#region autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.ConfigureServices(new ApiOptions()
    {
        ApiClientList = null,
        ApiName = "PixdinnPerdeci",
        RegistrationAssemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory,
                        "*.dll", SearchOption.TopDirectoryOnly)
               .Where(filePath => Path.GetFileName(filePath).StartsWith("PixdinnPerdeci") || Path.GetFileName(filePath).StartsWith("Shared"))
               .Select(Assembly.LoadFrom)
    });
});
#endregion

#region startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); // calling ConfigureServices method
builder.Services.AddControllersWithViews();
var app = builder.Build();
startup.Configure(app, builder.Environment); // calling Configure method
#endregion
