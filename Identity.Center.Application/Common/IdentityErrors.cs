using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Result;

namespace Identity.Center.Application.Common;

public static class IdentityErrors
{
  public static IError NotFound => new BaseError("Object.NotFound", "No se encontró información");
}
