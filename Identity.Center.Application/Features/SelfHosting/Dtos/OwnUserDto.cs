using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.SelfHosting.Dtos;

public sealed record OwnUserDto(
  Guid Id,
  string DocumentType,
  string DocumentNumber,
  string FirstName,
  string? SecondName,
  string FirstLastName,
  string? SecondLastName,
  DateTimeOffset CratedAtUtc,
  IEnumerable<OwnContactInfoDto> ContactInfo,
  IEnumerable<Guid> Roles
);


public sealed record OwnContactInfoDto(
  ContactTypes Type,
  string Value,
  bool Confirmed,
  DateTimeOffset CratedAtUtc
);