using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Identity.Center.Infrastructure.Hosting.Broker;

internal sealed class BrokerInitializer(IServiceProvider provider) : IHostedService
{
  private readonly IConnection _rabbit = provider.GetRequiredService<IConnection>();
  private readonly IOptionsMonitor<BrokerOptions> _options = provider.GetRequiredService<IOptionsMonitor<BrokerOptions>>();
  private readonly ILogger<BrokerInitializer> _logger = provider.GetRequiredService<ILogger<BrokerInitializer>>();
  
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Initializing");
    IChannel channel = await _rabbit.CreateChannelAsync(cancellationToken: cancellationToken);
    await channel.ExchangeDeclareAsync(
      exchange: _options.CurrentValue.Exchange,
      type: ExchangeType.Topic,
      durable: true,
      cancellationToken: cancellationToken
    );
    foreach(ContactTypes contactType in Enum.GetValues<ContactTypes>())
    {
      await channel.QueueDeclareAsync(BrokerOptions.Queue(contactType), true, false, false, cancellationToken: cancellationToken);
      await channel.QueueBindAsync(BrokerOptions.Queue(contactType), _options.CurrentValue.Exchange, BrokerOptions.Topic(contactType), cancellationToken: cancellationToken);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
