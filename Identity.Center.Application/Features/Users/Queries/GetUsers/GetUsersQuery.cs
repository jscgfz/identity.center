using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Users.Dtos;

namespace Identity.Center.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
  int? PageIndex,
  int? PageSize,
  bool? FullSet,
  string? DocumentType,
  string? DocumentNumber,
  string? FirstName,
  string? SecondName,
  string? FirstLastName,
  string? SecondLastName,
  string? ContactValue
) : IPaginationParams, IQuery<IPaginatedResult<BasicUserInfoDto>>;
