using System.Collections;
using Identity.Center.Application.Common.Alfresco.Response;
using Identity.Center.Application.Common.Response;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.SelfHosting.Dtos;

public sealed record RolePictureDto(
  string? Name,
  string? Description,
  string? DomainName,
  bool? ActiveDiretoryMandatory,
  bool? Root,
  IEnumerable<string>? Claims
);

public sealed record RolePictureComparisonDto(
  Guid Id,
  Guid RoleId,
  AuthorizationFileNode AuthorizationFile,
  ValueComparison<string> Name,
  ValueComparison<string?> Description,
  ValueComparison<string> DomainName,
  ValueComparison<bool?> ActiveDiretoryMandatory,
  ValueComparison<bool?> Root,
  ValueComparison<IEnumerable<string>> Claims,
  MasterOption<Guid> RequestBy,
  DateTimeOffset RequestAtUtc,
  ChangeControlStates Status
);

public sealed record AuthorizationFileNode(
  string Id,
  string Name,
  AlfrescoContentInfoResponse? Info
);

public sealed record ValueComparison<TValue>(
  TValue Current,
  TValue? Request
)
{
  public bool HasChange =>
    Request switch
    {
      null => Current != null,
      IEnumerable requestColl when Current is IEnumerable currentColl && typeof(TValue) != typeof(string) =>
        CollectionChanges(currentColl, requestColl),
      _ => !(Current?.Equals(Request) ?? false)
    };

  private static bool CollectionChanges(IEnumerable current, IEnumerable request)
  {
    List<object> currentList = [.. current.Cast<object>()];
    List<object> requestList = [.. request.Cast<object>()];
    return currentList.Count != requestList.Count || currentList.Except(requestList).Any();
  }
}
