using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Primitives.Abstractions;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Identity.Center.Infrastructure.Configuration.Configuration;

internal sealed class IdentityConfigurationProvider : ConfigurationProvider, IDisposable
{
  private readonly IDisposable? _registration;
  private readonly IConfiguration _configuration;
  private readonly IdentityContext _context;
  private readonly IdentityConfigurationSource _source;

  public IdentityConfigurationProvider(IServiceProvider provider, IdentityConfigurationSource source)
  {
    _context = provider.GetRequiredService<IDbContextFactory<IdentityContext>>().CreateDbContext();
    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    _configuration = provider.GetRequiredService<IConfiguration>();
    _source = source;
    if (_source.Detector != null)
      _registration = ChangeToken.OnChange(
        _source.Detector.Watch,
        Load
      );
  }

  public void Dispose()
    => _registration?.Dispose();

  public override void Load()
  {
    Guid appId = _configuration.GetSection($"{nameof(IdentityConfigurationProvider)}:{nameof(IKeyedEntity<Guid>.Id)}").Get<Guid>();
    Data = _context
      .Set<AppConfigurationSection>()
      .Where(row => row.AppId == appId)
      .ToDictionary(
        static row => row.Key,
        static row => row.Value,
        StringComparer.OrdinalIgnoreCase
      )!;
  }
}
