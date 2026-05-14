// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Core\Applications\Ocpp.Application\RabbitMq\Consumers\OcppTriggerMessage\OcppTriggerMessageConsumer.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Domain.RabbitMq;
using Shared.Domain.RabbitMq.Command.OcppTriggerMessage;
using Shared.Domain.Services.OcppServices.Ocpp16.Ocpp16ConnectionInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ocpp.Application.RabbitMq.Consumers.OcppTriggerMessage
{
    public class OcppTriggerMessageConsumer : IHostedService
    {
        private readonly ILogger<OcppTriggerMessageConsumer> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public OcppTriggerMessageConsumer(ILogger<OcppTriggerMessageConsumer> logger,
         IConfiguration configuration, IServiceProvider services)
        {
            _logger = logger;
            _configuration = configuration;
            _services = services;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new ConnectionFactory() { DispatchConsumersAsync = true, HostName = _configuration.GetSection("RabbitMqSettings:HostName").Value, Port = int.Parse(_configuration.GetSection("RabbitMqSettings:PortNumber").Value) };
            factory.UserName = _configuration.GetSection("RabbitMqSettings:UserName").Value;
            factory.Password = _configuration.GetSection("RabbitMqSettings:Password").Value;
            IConnection conn = factory.CreateConnection();
            IModel _channel = conn.CreateModel();
            #region exchange yoksa olusturulacak
            _channel.ExchangeDeclare(RabbitmqConstants.rotaWattDirectExchangeName, ExchangeType.Direct);
            #endregion
            #region kuyruk yoksa olusturulacak
            _channel.QueueDeclare(queue: RabbitmqConstants.ocppTriggerMessageQueueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            #endregion
            #region kuyruk direct exchange'e baglanıyor
            _channel.QueueBind(
                   queue: RabbitmqConstants.ocppTriggerMessageQueueName,
                   routingKey: RabbitmqConstants.ocppTriggerMessageQueueKey,
                   exchange: RabbitmqConstants.rotaWattDirectExchangeName
               );
            #endregion
            #region consumer
            var consumerOcppTriggerMessage = new AsyncEventingBasicConsumer(_channel);
            consumerOcppTriggerMessage.Received += async (model, ea) =>
            {
                using (var scope = _services.CreateScope())
                {
                    DateTime date = DateTime.Now;
                    var ocpp16ConnectionService = scope.ServiceProvider.GetRequiredService<IOcpp16ConnectionService>();
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    OcppTriggerMessageCommand ocppTriggerMessageCommand = Newtonsoft.Json.JsonConvert.DeserializeObject<OcppTriggerMessageCommand>(message);
                    await ocpp16ConnectionService.PushTriggerMessage(ocppTriggerMessageCommand.DeviceConnectionId, date);
                }
            };
            _channel.BasicConsume(queue: RabbitmqConstants.ocppTriggerMessageQueueName,
                                   autoAck: true,
                                   consumer: consumerOcppTriggerMessage);
            #endregion
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
