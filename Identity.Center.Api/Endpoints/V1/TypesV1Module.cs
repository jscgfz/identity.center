using Identity.Center.Api.Configuration;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Features.Types.Queries.GetContactTypes;
using Identity.Center.Application.Features.Types.Queries.GetCredentialTypes;
using Identity.Center.Application.Result;
using Identity.Center.Infrastructure.Common;
using MediatR;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class TypesV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/types")
      .WithTags("types_def")
      .RequireAuthorization(
        IdentityPolicies.Jwt,
        IdentityPolicies.ApiKey
      );

    group
      .MapGet("/contact", async ([AsParameters] GetContactTypesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.FromClaim("contacttypes:view")
      )
      .BuildRequirementsDoc("Obtiene los tipos de contacto");

    group
      .MapGet("/credentials", async ([AsParameters] GetCredentialTypesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.FromClaim("credentialtypes:view")
      )
      .BuildRequirementsDoc("Obtiene los tipos de credenciales");
  }
}
