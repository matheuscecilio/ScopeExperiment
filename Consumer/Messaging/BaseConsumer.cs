using Core.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.Messaging;

public abstract class BaseConsumer : IHostedService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    protected readonly ILogger _logger;
    protected string? RoutingKey;
    protected string? QueueName;

    protected BaseConsumer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BaseConsumer>();

        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConnectionValues.RabbitMqHost,
                UserName = ConnectionValues.RabbitMqUsername,
                Password = ConnectionValues.RabbitMqPassword
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        catch (Exception ex)
        {
            _logger.LogError($"RabbitConsumer init error, {ex}");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Register();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection.Close();
        return Task.CompletedTask;
    }

    private void Register()
    {
        _logger.LogInformation($"RabbitListener register,routeKey:{RoutingKey}");

        var dlxQueue = $"{QueueName}-dlx";
        var dlxExchange = $"{QueueName}-dlx-exchange";

        var args = new Dictionary<string, object>()
        {
            { "x-dead-letter-exchange", dlxExchange }
        };

        // Dlq queue
        _channel.ExchangeDeclare(
            exchange: dlxExchange,
            type: ExchangeType.Fanout
        );

        _channel.QueueDeclare(
            queue: dlxQueue,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.QueueBind(
            queue: dlxQueue,
            exchange: dlxExchange,
            routingKey: ""
        );

        // Main queue
        _channel.ExchangeDeclare(
            exchange: RabbitMqConstants.DefaultExchange,
            type: ExchangeType.Topic
        );
        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args
        );

        _channel.QueueBind(
            queue: QueueName,
            exchange: RabbitMqConstants.DefaultExchange,
            routingKey: RoutingKey
        );

        _channel.BasicQos(0, 3, false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            try
            {
                var message = Encoding.UTF8.GetString(body);
                var result = await ProcessAsync(message);

                if (result)
                {
                    _channel.BasicAck(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(
            queue: QueueName,
            consumer: consumer
        );
    }

    public abstract Task<bool> ProcessAsync(string message);
}
