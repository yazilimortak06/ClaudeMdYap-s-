// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\WorkerService\Program.cs
using Autofac.Extensions.DependencyInjection;
using FrameworkCore.Utils.Interface;
using FrameworkCore.Utils.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Shared.Domain.RabbitMq.Producers;
using Shared.Domain.RabbitMq.ProducersInterfaces;
using Shared.Domain.UtilServices.WorkerServiceUtilServices;
using WorkerService.Api;
using WorkerService.Application.UtilServices;
using WorkerService.Workers;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .ConfigureWebHostDefaults(webBuilder =>
               {
                  webBuilder.UseStartup<Startup>();
               }).ConfigureServices((hostContext, services) =>
               {
                   IConfiguration configuration = hostContext.Configuration;
                   var refitSetting = new RefitSettings { CollectionFormat = CollectionFormat.Csv };
                   services.AddHttpClient();
                   services.AddSingleton<IUtilService, UtilService>();
                   services.AddSingleton<IWorkerServiceUtilService, WorkerServiceUtilService>();
                   services.AddSingleton<IGenericHttpClientService, GenericHttpClientService>();
                   // Worker servisler kayit ediliyor
                   services.AddHostedService<TaskControlWorker>();
                   services.AddHostedService<AutomaticPaymentWorker>();
                   services.AddHostedService<EpdkCheckChargeWorker>();
                   services.AddHostedService<EpdkPriceInfoWorker>();
                   services.AddHostedService<RefundDebitCardVerificationWorker>();
                   services.AddHostedService<ArchiveAndInvoiceCreateRequestWorker>();
                   services.AddHostedService<ArchiveAndInvoiceGetStatusWorker>();
                   services.AddHostedService<ArchiveAndInvoiceSetDocumentDataWorker>();
                   services.AddHostedService<ArchiveSetDocumentCanceledDataWorker>();
                   services.AddSingleton<IWorkerServiceExceptionProducer, WorkerServiceExceptionProducer>();
               });
    }
}
