using Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;
using Refit;

namespace Identity.Center.Application.Abstractions.Clients;

public interface IMasivianMailClient
{
  [Post("v1/delivery")]
  Task<HttpResponseMessage> DeliveryV1([Body] MasivianEmailRequest request, CancellationToken cancellationToken);
}
