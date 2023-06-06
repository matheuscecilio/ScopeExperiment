using Consumer.Messaging;
using Core.Constants;
using Core.Data.Repositories;
using Core.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer;

public class ConsumerWithScope : BaseConsumer
{
    private readonly IServiceProvider _serviceProvider;
    public ConsumerWithScope(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IServiceProvider serviceProvider
    ) : base(loggerFactory, configuration)
    {
        RoutingKey = RabbitMqConstants.DefaultRoutingKey;
        QueueName = RabbitMqConstants.ScopePersonQueue;
        _serviceProvider = serviceProvider;
    }

    public override async Task<bool> ProcessAsync(string message)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();

        var person = JsonConvert.DeserializeObject<Person>(message);

        _logger.LogInformation($"Consumer with Scope - {person.Id} - {DateTime.UtcNow}");

        await repository.AddAsync(person);

        return true;
    }
}
