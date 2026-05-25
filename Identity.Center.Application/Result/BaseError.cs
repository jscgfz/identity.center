using Identity.Center.Application.Abstractions.Result;

namespace Identity.Center.Application.Result;

public sealed record BaseError(
  string Name,
  string Description
) : IError
{
  public KeyValuePair<string, object?> Seralize()
    => KeyValuePair.Create<string, object?>(Name, Description);
}
