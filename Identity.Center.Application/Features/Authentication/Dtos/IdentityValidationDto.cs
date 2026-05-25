namespace Identity.Center.Application.Features.Authentication.Dtos;

internal sealed record IdentityValidationDto(
  Guid UserId,
  Guid AppId
);
