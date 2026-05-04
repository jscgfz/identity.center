using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Entities.Core.Authentication;

namespace Identity.Center.Domain.Common;

public static class IdentityCommons
{
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
  public static bool IsValidClaim(string claim)
    => Regex.IsMatch(claim, @"^[a-z]*\:[a-z]*$") || claim.Equals(nameof(ApiKey.Root).ToLower());
  public static Regex LowerRegex => new(@"^[a-z]*$");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
  public static KeyValuePair<string, string> Deserialize(string claim)
  {
    IEnumerable<string> parts = claim.Split(':');
    return KeyValuePair.Create(parts.ElementAt(0), parts.ElementAt(1));
  }
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
