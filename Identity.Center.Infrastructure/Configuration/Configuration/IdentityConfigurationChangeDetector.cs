using Identity.Center.Application.Abstractions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Identity.Center.Infrastructure.Configuration.Configuration;

internal class IdentityConfigurationChangeDetector : IConfigurationChangeDetector
{
  private readonly TimeSpan _interval;
  private readonly Timer _timer;
  private IChangeToken? _changeToken;
  private CancellationTokenSource? _tokenSource;

  public IdentityConfigurationChangeDetector(TimeSpan interval)
  {
    _interval = interval;
    _timer = new(TimerCallBack, null, TimeSpan.Zero, _interval);
  }

  private void TimerCallBack(object? sender)
    => _tokenSource?.Cancel();

  public void Dispose()
  {
    _timer.Dispose();
    _tokenSource?.Dispose();
  }

  public IChangeToken Watch()
  {
    _tokenSource = new();
    _changeToken = new CancellationChangeToken(_tokenSource.Token);
    return _changeToken;
  }
}
