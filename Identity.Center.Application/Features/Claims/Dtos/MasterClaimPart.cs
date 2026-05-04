namespace Identity.Center.Application.Features.Claims.Dtos;

public sealed record MasterClaimPart(
  Guid Id,
  string Name,
  string? Description,
  DateTimeOffset CreatedAtUtc
);
