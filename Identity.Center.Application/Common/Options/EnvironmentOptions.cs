using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Center.Application.Common.Options;

public sealed class EnvironmentOptions
{
  public required IEnumerable<string> DevEnvironments { get; set; }
  public bool IsDevEnvironment(string environmentName)
    => DevEnvironments.Contains(environmentName);

  public bool IsDevEnvironment(ref IServiceProvider provider)
    => IsDevEnvironment(provider.GetRequiredService<IWebHostEnvironment>());

  public bool IsDevEnvironment<THost>(THost host) where THost : IHostEnvironment
    => IsDevEnvironment(host.EnvironmentName);
}
