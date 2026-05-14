// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\WorkerService\Workers\TaskControlWorker.cs
using FrameworkCore.Utils.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Domain.Dto.WorkerServiceDto.GeneralTaskDtos;
using Shared.Domain.ServiceInterfaces.WorkerServiceServiceInterfaces.GeneralTaskServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService.Workers
{
    public class TaskControlWorker : BackgroundService
    {
        // Tum worker servislerin kullandigi statik task listesi
        public static List<GeneralTaskDto> _generaTasks;
        private Task _executingTask;
        private CancellationTokenSource _cts;
        IHostApplicationLifetime _lifetime;

        private readonly ILogger<TaskControlWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUtilService _utilService;
        private readonly IGeneralTaskService _generalTaskService;

        public TaskControlWorker(ILogger<TaskControlWorker> logger, IConfiguration configuration, IUtilService utilService, IHostApplicationLifetime lifetime, IGeneralTaskService generalTaskService)
        {
            _logger = logger;
            _configuration = configuration;
            _utilService = utilService;
            _lifetime = lifetime;
            _generalTaskService = generalTaskService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    #region request dto olusturuluyor
                    GetGeneralTaskRequestDto getGeneralTaskRequest = new GetGeneralTaskRequestDto();
                    #endregion
                    var taskProcess = await _generalTaskService.GetGeneralTask(getGeneralTaskRequest).ConfigureAwait(true);
                    _generaTasks = taskProcess.Data.GeneralTasks;
                    _logger.LogError("tasklar");
                    _logger.LogError(JsonConvert.SerializeObject(_generaTasks));
                    await Task.Delay(3000);
                }
                catch (Exception exception)
                {
                    _logger.LogInformation("Sistem Hatasi : " + exception.Message);
                    await Task.Delay(3000, stoppingToken);
                    await StopAsync(stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_lifetime.ApplicationStopping.IsCancellationRequested)
            {
                _logger.LogWarning("Worker service stopping.");
            }
            else
            {
                // hata gelirse 10 dk sonra tekrar baslatiliyor
                await Task.Delay(600000);
                await ExecuteAsync(cancellationToken);
            }
        }
    }
}
