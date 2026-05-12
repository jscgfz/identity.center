using Identity.Center.Api.Common;
using Identity.Center.Api.Configuration;
using Identity.Center.Api.Configuration.Authorization;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Common.Response;
using Identity.Center.Application.Features.Types.Queries.GetContactTypes;
using Identity.Center.Application.Features.Types.Queries.GetCredentialTypes;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Enums;
using MediatR;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class TypesV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/types")
      .WithTags("types_def")
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Merged(
            BaseIdentityPolicies.Jwt,
            BaseIdentityPolicies.ApiKey
          )
      )
      .AddRequirement("autorization", "apikey")
      .AddRequirement("autorization", "jwt");

    group
      .MapGet("/contact", async ([AsParameters] GetContactTypesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
        .WithIdentityAuthorization(
          IdentityPolicyBuilder
            .Empty
            .RequireClaims("contacttypes:view")
        )
        .Produces<IEnumerable<MasterOption<ContactTypes>>>()
        .AddRequirement("claims", "contacttypes:view")
        .BuildRequirements("Obtiene los tipos de contacto configurados en el sistema");

    group
      .MapGet("/credentials", async ([AsParameters] GetCredentialTypesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
        .WithIdentityAuthorization(
          IdentityPolicyBuilder
            .Empty
            .RequireClaims("credentialtypes:view")
        )
        .Produces<IEnumerable<MasterOption<int>>>()
        .AddRequirement("claims", "credentialtypes:view")
        .BuildRequirements("Obtiene los tipos de credenciales configuradas en el sistema");
  }
}
