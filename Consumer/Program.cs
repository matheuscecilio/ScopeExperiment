using Consumer;
using Core.Constants;
using Core.Data;
using Core.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var consumerApplication = Host
    .CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        logging.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Warning);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlServer(ConnectionValues.ConnectionStringSqlServer);
        });

        services.AddScoped<IPersonRepository, PersonRepository>();

        //services.AddHostedService<ConsumerWithDI>();
        services.AddHostedService<ConsumerWithScope>();
    })
    .Build();

await consumerApplication.RunAsync();