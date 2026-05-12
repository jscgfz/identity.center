using Identity.Center.Application.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;

namespace Identity.Center.Infrastructure.Configuration.Configuration;

internal sealed class IdentityConfigurationSource(IServiceProvider provider) : IConfigurationSource
{
  private readonly IServiceProvider _provider = provider;
  public IConfigurationChangeDetector? Detector;

  public IdentityConfigurationSource(IServiceProvider provider, IConfigurationChangeDetector detector) : this(provider)
    => Detector = detector;

  public IConfigurationProvider Build(IConfigurationBuilder builder)
    => new IdentityConfigurationProvider(_provider, this);
}
