// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\WorkerService\Workers\AutomaticPaymentWorker.cs
using AutoMapper;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using FrameworkCore.Utils.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.ApiDto.PanelPaymentInfoDtos;
using Shared.Domain.Entities.WorkerServiceEntities.AutomaticPaymentModule;
using Shared.Domain.Enums.WorkerServiceEnums;
using Shared.Domain.HttpClients.HttpClientInterfaces.WebApiInterfaces;
using Shared.Domain.RabbitMq.ProducersInterfaces;
using Shared.Domain.RepositoryInterfaces.WorkerServiceRepositories.AutomaticPaymentProcessRepositoryInterfaces;
using Shared.Domain.UtilServices.WorkerServiceUtilServices;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService.Workers
{
    public class AutomaticPaymentWorker : BackgroundService
    {
        IHostApplicationLifetime _lifetime;
        private int? delaySecond;
        AutomaticPaymentRequestDto automaticPaymentRequest;
        Result<AutomaticPaymentResponseDto> automaticPaymentResponse;

        private readonly ILogger<AutomaticPaymentWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IServiceProvider _services;
        private readonly ICustomHttpUtilService _customHttpUtilService;
        private readonly IWorkerServiceExceptionProducer _workerServiceExceptionProducer;
        private readonly IWorkerServiceUtilService _workerServiceUtilService;
        protected readonly IMapper _mapper;

        public AutomaticPaymentWorker(ILogger<AutomaticPaymentWorker> logger,
            IConfiguration configuration,
            IUtilService utilService,
            IHostApplicationLifetime lifetime,
            IServiceProvider services,
            ICustomHttpUtilService customHttpUtilService,
            IWorkerServiceExceptionProducer workerServiceExceptionProducer,
            IWorkerServiceUtilService workerServiceUtilService,
            IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _utilService = utilService;
            _lifetime = lifetime;
            _services = services;
            _customHttpUtilService = customHttpUtilService;
            _workerServiceExceptionProducer = workerServiceExceptionProducer;
            _workerServiceUtilService = workerServiceUtilService;
            _mapper = mapper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                automaticPaymentRequest = new AutomaticPaymentRequestDto();
                automaticPaymentResponse = new Result<AutomaticPaymentResponseDto>();
                return ExecuteAsync(cancellationToken).IsCompleted ? ExecuteAsync(cancellationToken) : Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _workerServiceUtilService.SaveWorkerServiceException(exception, GeneralTaskIdEnum.AUTOMATIC_PAYMENT, _workerServiceExceptionProducer, _customHttpUtilService);
                return Task.CompletedTask;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (TaskControlWorker._generaTasks != null && TaskControlWorker._generaTasks.Where(x => x.Id == (int)GeneralTaskIdEnum.AUTOMATIC_PAYMENT && x.Active) != null)
                    {
                        #region ilgili task in bekleme suresi aliniyor
                        if (delaySecond == null)
                        {
                            delaySecond = TaskControlWorker._generaTasks.Where(x => x.Id == (int)GeneralTaskIdEnum.AUTOMATIC_PAYMENT && x.Active).FirstOrDefault().TaskScheduleSecond;
                        }
                        #endregion
                        using (var scope = _services.CreateScope())
                        {
                            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                            _logger.LogError("environment");
                            _logger.LogError(environment);
                            var automaticPaymentProcessRepository = scope.ServiceProvider.GetRequiredService<IAutomaticPaymentProcessRepository>();
                            var paymentInfoManagmentClientService = scope.ServiceProvider.GetRequiredService<IPaymentInfoManagmentClientService>();
                            #region web apiye istek atilarak odenmemis sarj islemi odeniyor
                            DateTime requestDate = DateTime.Now;
                            automaticPaymentResponse = await paymentInfoManagmentClientService.AutomaticPayment(automaticPaymentRequest);
                            #endregion
                            #region automatic payment process verisi ekleniyor
                            AutomaticPaymentProcess automaticPaymentProcess = _mapper.Map<AutomaticPaymentProcess>(automaticPaymentResponse.Data, opt =>
                            {
                                opt.AfterMap((src, dest) =>
                                {
                                    var destData = dest as AutomaticPaymentProcess;
                                    destData.RequestDate = requestDate;
                                });
                            });
                            await automaticPaymentProcessRepository.InsertAsync(automaticPaymentProcess);
                            await automaticPaymentProcessRepository.SaveChangesAsync();
                            #endregion
                        }
                        await Task.Delay(delaySecond.GetValueOrDefault() * 1000, stoppingToken);
                    }
                    else
                    {
                        _logger.LogInformation("Task Bulunamadi");
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }
            catch (Exception exception)
            {
                _workerServiceUtilService.SaveWorkerServiceException(exception, GeneralTaskIdEnum.AUTOMATIC_PAYMENT, _workerServiceExceptionProducer, _customHttpUtilService);
                _logger.LogInformation("Sistem Hatasi : " + exception.Message);
                await Task.Delay(5000, stoppingToken);
                await StopAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Worker service stopping: cancellationToken.IsCancellationRequested");
                }
                if (_lifetime.ApplicationStopping.IsCancellationRequested)
                {
                    _logger.LogWarning("Worker service stopping: _lifetime.ApplicationStopping.IsCancellationRequested");
                }
                else
                {
                    await Task.Delay(5000);
                    await ExecuteAsync(cancellationToken);
                }
            }
            catch (Exception exception)
            {
                _workerServiceUtilService.SaveWorkerServiceException(exception, GeneralTaskIdEnum.AUTOMATIC_PAYMENT, _workerServiceExceptionProducer, _customHttpUtilService);
                await Task.Delay(90000);
                await ExecuteAsync(cancellationToken);
            }
        }
    }
}
