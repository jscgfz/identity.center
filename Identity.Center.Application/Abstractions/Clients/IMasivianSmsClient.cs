using Identity.Center.Application.Common.Notifications.Masivian.Sms.Request;
using Refit;

namespace Identity.Center.Application.Abstractions.Clients;

public interface IMasivianSmsClient
{
  [Post("/send-message")]
  Task<HttpResponseMessage> SendMessage([Body] MasivianSmsRequest request, CancellationToken cancellationToken);

  [Post("/send-message-batch")]
  Task<HttpResponseMessage> SendMessage([Body] IEnumerable<MasivianSmsRequest> request, CancellationToken cancellationToken);
}
