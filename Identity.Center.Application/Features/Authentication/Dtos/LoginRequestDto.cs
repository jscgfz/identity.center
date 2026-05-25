namespace Identity.Center.Application.Features.Authentication.Dtos;

public record LoginRequestDto(
  string Username,
  string Password
);
