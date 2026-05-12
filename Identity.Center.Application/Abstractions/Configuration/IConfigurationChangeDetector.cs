using Microsoft.Extensions.Primitives;

namespace Identity.Center.Application.Abstractions.Configuration;

public interface IConfigurationChangeDetector : IDisposable
{
  IChangeToken Watch();
}
