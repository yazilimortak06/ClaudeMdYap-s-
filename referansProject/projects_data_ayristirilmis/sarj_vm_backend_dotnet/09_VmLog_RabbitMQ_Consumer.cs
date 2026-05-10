// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\VmLog.Application\RabbitMq\Consumers\MiddleWareLog\RequestResponseLogInfoConsumer.cs
// RabbitMQ Consumer: İki kuyrugu dinler — RequestResponse log + ApiException log
// IHostedService olarak çalışır, uygulama başladığında otomatik başlar

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Shared.Domain.RabbitMq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shared.Domain.RabbitMq.Command.LogProcess;
using Shared.Domain.ServiceInterfaces.VmLogServiceInterfaces.RequestResponseLogServiceInterfaces;
using Shared.Domain.ServiceInterfaces.VmLogServiceInterfaces.ExceptionLogServiceInterfaces;

namespace VmLog.Application.RabbitMq.Consumers.MiddleWareLog
{
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
            _channel.BasicQos(0, 10, false); // 10 mesajlık paralel işleme sınırı

            // Exchange ve kuyruk bildirimleri
            _channel.ExchangeDeclare(RabbitmqConstants.vmDirectExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queue: RabbitmqConstants.vmReqResLogQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: RabbitmqConstants.vmReqResLogQueueName, routingKey: RabbitmqConstants.vmReqResLogQueueKey, exchange: RabbitmqConstants.vmDirectExchangeName);
            _channel.QueueDeclare(queue: RabbitmqConstants.vmApiExceptionQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: RabbitmqConstants.vmApiExceptionQueueName, routingKey: RabbitmqConstants.vmApiExceptionQueueKey, exchange: RabbitmqConstants.vmDirectExchangeName);

            // Request/Response log consumer
            var consumerRequestResponse = new EventingBasicConsumer(_channel);
            _logger.LogError("request geldi");
            consumerRequestResponse.Received += async (model, ea) =>
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var requestResponseService = scope.ServiceProvider.GetRequiredService<IRequestResponseLogService>();
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var saveLogRequestInfo = JsonConvert.DeserializeObject<SaveRequestResponseLog>(message);
                    await requestResponseService.SaveRequestResponse(saveLogRequestInfo);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "request response consumer hatası");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };
            _channel.BasicConsume(queue: RabbitmqConstants.vmReqResLogQueueName, autoAck: false, consumer: consumerRequestResponse);

            // API Exception consumer
            var consumerApiException = new EventingBasicConsumer(_channel);
            _logger.LogError("exception geldi");
            consumerApiException.Received += async (model, ea) =>
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var apiExceptionService = scope.ServiceProvider.GetRequiredService<IExceptionLogService>();
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var saveExceptionLog = JsonConvert.DeserializeObject<SaveExceptionLog>(message);
                    await apiExceptionService.AddNewExceptionAsync(saveExceptionLog);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "api exception consumer hatası");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };
            _channel.BasicConsume(queue: RabbitmqConstants.vmApiExceptionQueueName, autoAck: false, consumer: consumerApiException);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
