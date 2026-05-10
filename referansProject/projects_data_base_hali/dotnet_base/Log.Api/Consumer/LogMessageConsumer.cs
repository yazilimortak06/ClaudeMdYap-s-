// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Applications\Log.Application\RabbitMq\Consumers\MiddleWareLog\RequestResponseLogInfoConsumer.cs
// Ek: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Applications\Log.Application\RabbitMq\Consumers\ExceptionLog\ApiExceptionConsumer.cs
//
// NOT: Sistem iki consumer içerir:
//   1. RequestResponseLogInfoConsumer - IHostedService olarak çalışır, raw RabbitMQ.Client kullanır,
//      hem RequestResponse hem ApiException kuyruğunu aynı anda dinler.
//   2. ApiExceptionConsumer - MassTransit IConsumer<T> pattern ile çalışır.

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Domain.RabbitMq;
using Shared.Domain.RabbitMq.Command;
using Shared.Domain.RabbitMq.Command.LogProcess;
using Shared.Domain.ServiceInterfaces.LogServiceInterfaces.ExceptionsServiceInterfaces;
using Shared.Domain.ServiceInterfaces.LogServiceInterfaces.RequestResponsesServiceInterfaces;

namespace Log.Application.RabbitMq.Consumers.MiddleWareLog
{
    /// <summary>
    /// IHostedService olarak register edilir.
    /// Uygulama başladığında RabbitMQ'ya bağlanır ve iki kuyruğu dinler:
    ///   - generalReqResLogQueue: HTTP request/response logları
    ///   - generalApiExceptionQueue: API exception logları
    /// </summary>
    public class RequestResponseLogInfoConsumer : IHostedService
    {
        private readonly ILogger<RequestResponseLogInfoConsumer> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public RequestResponseLogInfoConsumer(ILogger<RequestResponseLogInfoConsumer> logger,
         IConfiguration configuration, IServiceProvider services)
        {
            _logger = logger;
            _configuration = configuration;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _configuration.GetSection("RabbitMqSettings:HostName").Value,
                Port = int.Parse(_configuration.GetSection("RabbitMqSettings:PortNumber").Value)
            };
            factory.UserName = _configuration.GetSection("RabbitMqSettings:UserName").Value;
            factory.Password = _configuration.GetSection("RabbitMqSettings:Password").Value;

            IConnection conn = factory.CreateConnection();
            IModel _channel = conn.CreateModel();

            #region Exchange yoksa oluşturulacak
            _channel.ExchangeDeclare(RabbitmqConstants.rotaWattDirectExchangeName, ExchangeType.Direct);
            #endregion

            #region Request/Response log kuyruğu
            _channel.QueueDeclare(queue: RabbitmqConstants.generalReqResLogQueueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            _channel.QueueBind(
                   queue: RabbitmqConstants.generalReqResLogQueueName,
                   routingKey: RabbitmqConstants.generalReqResLogQueueKey,
                   exchange: RabbitmqConstants.rotaWattDirectExchangeName);
            #endregion

            #region API Exception log kuyruğu
            _channel.QueueDeclare(queue: RabbitmqConstants.generalApiExceptionQueueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            _channel.QueueBind(
                   queue: RabbitmqConstants.generalApiExceptionQueueName,
                   routingKey: RabbitmqConstants.generalApiExceptionQueueKey,
                   exchange: RabbitmqConstants.rotaWattDirectExchangeName);
            #endregion

            #region Request/Response consumer
            var consumerRequestResponse = new EventingBasicConsumer(_channel);
            consumerRequestResponse.Received += async (model, ea) =>
            {
                using (var scope = _services.CreateScope())
                {
                    var requestResponseService = scope.ServiceProvider.GetRequiredService<IRequestResponseService>();
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    SaveRequestResponseLog saveLogRequestInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveRequestResponseLog>(message);
                    await requestResponseService.SaveRequestResponse(saveLogRequestInfo);
                }
            };
            _channel.BasicConsume(queue: RabbitmqConstants.generalReqResLogQueueName,
                                   autoAck: true,
                                   consumer: consumerRequestResponse);
            #endregion

            #region API Exception consumer
            var consumerApiException = new EventingBasicConsumer(_channel);
            consumerApiException.Received += (model, ea) =>
            {
                using (var scope = _services.CreateScope())
                {
                    var apiExceptionService = scope.ServiceProvider.GetRequiredService<IApiExceptionService>();
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    SaveApiException saveApiException = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveApiException>(message);
                    apiExceptionService.AddNewExceptionAsync(saveApiException);
                }
            };
            _channel.BasicConsume(queue: RabbitmqConstants.generalApiExceptionQueueName,
                                   autoAck: true,
                                   consumer: consumerApiException);
            #endregion

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

namespace Log.Application.RabbitMq.Consumers.ExceptionLog
{
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Shared.Domain.RabbitMq.Command.LogProcess;
    using Shared.Domain.ServiceInterfaces.LogServiceInterfaces.ExceptionsServiceInterfaces;

    /// <summary>
    /// MassTransit IConsumer pattern ile ApiException mesajlarını tüketir.
    /// RegisterMasstransit extension'ı ile kayıt edilir.
    /// </summary>
    public class ApiExceptionConsumer : IConsumer<SaveApiException>
    {
        private readonly IApiExceptionService _apiExceptionService;
        private readonly ILogger<ApiExceptionConsumer> _logger;

        public ApiExceptionConsumer(IApiExceptionService apiExceptionService, ILogger<ApiExceptionConsumer> logger)
        {
            _apiExceptionService = apiExceptionService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SaveApiException> context)
        {
            _apiExceptionService.AddNewExceptionAsync(context.Message);
        }
    }
}
