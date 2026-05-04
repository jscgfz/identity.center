using Identity.Center.Api.Configuration;
using Identity.Center.Application.Common.Response;
using Identity.Center.Application.Features.Types.Queries.GetContactTypes;
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
      .WithDescription("Definición de los tipos de datos del sistema");

    group
      .MapGet("/contact", async ([AsParameters] GetContactTypesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
        .Produces<IEnumerable<MasterOption<ContactTypes>>>()
        .WithDescription("Obtiene los tipos de contacto configurados en el sistema");
  }
}
