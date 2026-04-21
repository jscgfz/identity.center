using Identity.Center.Application.Abstractions.Result;

namespace Identity.Center.Application.Result;

public record BaseError(
  string Name,
  string Description
) : IError
{
  public KeyValuePair<string, string> Seralize()
    => KeyValuePair.Create(Name, Description);
} 
