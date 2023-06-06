using Consumer.Messaging;
using Core.Constants;
using Core.Data.Repositories;
using Core.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer;

public class ConsumerWithDI : BaseConsumer
{
    private readonly IPersonRepository _repository;

    public ConsumerWithDI(
            ILoggerFactory loggerFactory,
            IPersonRepository repository
    ) : base(loggerFactory)
    {
        RoutingKey = RabbitMqConstants.DefaultRoutingKey;
        QueueName = RabbitMqConstants.DefaultPersonQueue;
        _repository = repository;
    }

    public override async Task<bool> ProcessAsync(string message)
    {
        var person = JsonConvert.DeserializeObject<Person>(message);

        await _repository.AddAsync(person);

        _logger.LogInformation($"Consumer with DI - {person.Id} - {DateTime.UtcNow}");

        return true;
    }
}
