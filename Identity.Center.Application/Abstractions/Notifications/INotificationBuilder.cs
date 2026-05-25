using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Application.Abstractions.Notifications;

public interface INotificationBuilder
{
  Task<Result<Unit>> DispachAsync(IServiceProvider provider, CancellationToken cancellationToken);
  Task<Result<Unit>> EnQueueAsync(IServiceProvider provider, CancellationToken cancellationToken);
}
