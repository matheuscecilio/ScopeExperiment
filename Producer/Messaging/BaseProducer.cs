using Core.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Producer.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Messaging;

public class BaseProducer : IBaseProducer
{
    private readonly IModel _channel;
    private readonly ILogger _logger;

    public BaseProducer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BaseProducer>();
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConnectionValues.RabbitMqHost,
                UserName = ConnectionValues.RabbitMqUsername,
                Password = ConnectionValues.RabbitMqPassword
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(-1, ex, "RabbitMQClient init fail");
        }
    }

    public void PublishMessage(string routingKey, object message)
    {
        _logger.LogInformation($"PublishMessage, routingKey: {routingKey}");

        _channel.ExchangeDeclare(
            exchange: RabbitMqConstants.DefaultExchange,
            type: ExchangeType.Topic
        );

        string msgJson = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(msgJson);

        _channel.BasicPublish(
            exchange: RabbitMqConstants.DefaultExchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );
    }
}