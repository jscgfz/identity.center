namespace Identity.Center.Application.Features.Users.Dtos;

public record BasicUserInfoDto(
  Guid Id,
  string DocumentType,
  string DocumentNumber,
  string FirstName,
  string? SecondName,
  string FirstLastName,
  string? SecondLastName,
  DateTimeOffset CreatedAtUtc,
  IEnumerable<ContactInfoDto> ContactInfo
);
