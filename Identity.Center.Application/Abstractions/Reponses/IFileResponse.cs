using Identity.Center.Application.Common;

namespace Identity.Center.Application.Abstractions.Reponses;

public interface IFileResponse
{
  FileContentResponse Render();
}
