using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Identity.Center.Infrastructure.Hosting.Broker;

internal sealed class BrokerMailingSwitchHandler(IServiceProvider provider, ContactTypes type) : BackgroundService
{
  private readonly IConnection _rabbit = provider.GetRequiredService<IConnection>();
  private readonly IOptionsMonitor<BrokerOptions> _options = provider.GetRequiredService<IOptionsMonitor<BrokerOptions>>();
  private readonly ILogger<BrokerMailingSwitchHandler> _logger = provider.GetRequiredService<ILogger<BrokerMailingSwitchHandler>>();

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("statring {topic} service", type);
    IChannel channel = await _rabbit.CreateChannelAsync(cancellationToken: stoppingToken);
    await channel.ExchangeDeclareAsync(
      exchange: _options.CurrentValue.Exchange,
      type: ExchangeType.Topic,
      durable: true,
      cancellationToken: stoppingToken
    );
    await channel.QueueDeclareAsync(BrokerOptions.Queue(type), true, false, false, cancellationToken: stoppingToken);
    await channel.QueueBindAsync(BrokerOptions.Queue(type), _options.CurrentValue.Exchange, BrokerOptions.Topic(type), cancellationToken: stoppingToken);
    //AsyncRetryPolicy retryPolicy = Policy
    //  .Handle<Exception>()
    //  .WaitAndRetryAsync(5, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
    AsyncEventingBasicConsumer consumer = new(channel);
    consumer.ReceivedAsync += (sender, args) =>
    {
      _logger.LogInformation("{topic} message recived {body}", args.RoutingKey, IdentityCommons.Encoding.GetString(args.Body.ToArray()));
      return Task.CompletedTask;
    };

    await channel.BasicConsumeAsync(BrokerOptions.Queue(type), true, consumer, stoppingToken);
    await Task.Delay(Timeout.Infinite, stoppingToken);
  }
}
