using System.Text.Json;
using Identity.Center.Application.Abstractions.Result;

namespace Identity.Center.Application.Result;

public sealed class JsonError(
  string Name,
  JsonElement JsonError
) : IError
{
  public KeyValuePair<string, object?> Seralize()
    => KeyValuePair.Create<string, object?>(Name, JsonError);
}
