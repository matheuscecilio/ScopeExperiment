using Core.Constants;
using Core.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Interfaces;

namespace Producer;
public class DefaultProducer : BackgroundService
{
    private readonly IBaseProducer _producer;
    private readonly ILogger _logger;

    public DefaultProducer(
        IBaseProducer producer,
        ILoggerFactory loggerFactory
    )
    {
        _producer = producer;
        _logger = loggerFactory.CreateLogger<DefaultProducer>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var waitTimeInSeconds = 2;

        for(var i = 0; i < 10; i++)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            _producer.PublishMessage(
                RabbitMqConstants.DefaultRoutingKey,
                person
            );

            Thread.Sleep(waitTimeInSeconds * 1000);
        }

        return Task.CompletedTask;
    }
}
