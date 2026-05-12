using Identity.Center.Application.Features.Credentials.Dtos;
using Identity.Center.Application.Features.Roles.Dtos;

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
  IEnumerable<ContactInfoDto> ContactInfo,
  IEnumerable<CreatedCredentialDto>? DomainCredentials,
  IEnumerable<CreatedCredentialDto>? SingleCredentials,
  IEnumerable<RoleDto>? Roles
)
{
  private readonly IEnumerable<CreatedCredentialDto>? DomainCredentials = DomainCredentials;
  private readonly IEnumerable<CreatedCredentialDto>? SingleCredentials = SingleCredentials;

  public IEnumerable<CreatedCredentialDto>? Credentials => (DomainCredentials is null || !DomainCredentials.Any()) && (SingleCredentials is null || !SingleCredentials.Any()) ? null : [
    .. (DomainCredentials ?? Enumerable.Empty<CreatedCredentialDto>()),
    .. (SingleCredentials ?? Enumerable.Empty<CreatedCredentialDto>())
  ];
}
