namespace Identity.Center.Domain.Common.Models.Cryptography;

public sealed record HashCreationResponse(
  byte[] Salt,
  byte[] Hash,
  string Value
);
