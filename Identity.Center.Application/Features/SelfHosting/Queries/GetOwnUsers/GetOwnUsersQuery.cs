using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetOwnUsers;

public sealed record GetOwnUsersQuery(
  int? PageIndex,
  int? PageSize,
  bool? FullSet,
  string? DocumentType,
  string? DocumentNumber,
  string? FirstName,
  string? SecondName,
  string? FirstLastName,
  string? SecondLastName,
  string? ContactInfo,
  string? Role
) : IPaginationParams, IQuery<IPaginatedResult<OwnUserDto>>;
