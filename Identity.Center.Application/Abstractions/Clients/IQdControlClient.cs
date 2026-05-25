using Identity.Center.Application.Common.Authentication.Models;
using Refit;

namespace Identity.Center.Application.Abstractions.Clients;

public interface IQdControlClient
{
  [Post("/ValidaUsuario")]
  Task<HttpResponseMessage> Validate(LdapAuthenticationRequest request, CancellationToken cancellationToken);
}
