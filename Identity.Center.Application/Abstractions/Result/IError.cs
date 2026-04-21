namespace Identity.Center.Application.Abstractions.Result;

public interface IError
{
  KeyValuePair<string, string> Seralize();
}
