namespace Identity.Center.Domain.Common.Models.Cryptography;

public sealed record HashValidationResponse(
  bool Success,
  string Value,
  byte[] Hash
);
