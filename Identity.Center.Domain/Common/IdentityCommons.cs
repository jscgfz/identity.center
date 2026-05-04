using System.Security.Cryptography;
using System.Text;
using Identity.Center.Domain.Common.Models.Cryptography;

namespace Identity.Center.Domain.Common;

public static class IdentityCommons
{
  public static Encoding Encoding => Encoding.UTF8;
  public static byte[] NewHashKey => RandomNumberGenerator.GetBytes(32);
  public static async Task<HashCreationResponse> NewHash(string? value = null, CancellationToken cancellationToken = default)
  {
    byte[] salt = NewHashKey;
    value ??= Encoding.GetString(NewHashKey);
    using HMACSHA256 hmac = new(salt);
    using MemoryStream memo = new(Encoding.GetBytes(value));
    byte[] hash = await hmac.ComputeHashAsync(memo, cancellationToken).ConfigureAwait(false);
    return new(
      salt,
      hash,
      value
    );
  }
  public static async Task<HashValidationResponse> ValidateHash(HashValidationRequest request, CancellationToken cancellationToken = default)
  {
    using HMACSHA256 hmac = new(request.Salt);
    using MemoryStream memo = new(Encoding.GetBytes(request.Value));
    byte[] hashResult = await hmac.ComputeHashAsync(memo, cancellationToken).ConfigureAwait(false);
    return new(
      hashResult.SequenceEqual(request.Hash),
      request.Value,
      request.Hash
    );
  }
}
