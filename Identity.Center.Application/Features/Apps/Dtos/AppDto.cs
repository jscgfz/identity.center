namespace Identity.Center.Application.Features.Apps.Dtos;

public sealed record AppDto(
  Guid Id,
  long Index,
  string Name,
  string? DomainName,
  string? Description,
  DateTimeOffset CreatedAtUtc,
  string Prefix
);
