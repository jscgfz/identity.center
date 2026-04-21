namespace Identity.Center.Domain.Common.Models.Cryptography;

public sealed record HashValidationRequest(
  string Value,
  byte[] Hash,
  byte[] Salt
);
