using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
  public static Task<HashCreationResponse> NewHash(string? value = null)
  {
    byte[] salt = NewHashKey;
    value ??= Encoding.GetString(NewHashKey);
    byte[] hash = KeyDerivation.Pbkdf2(
      password: value,
      salt: salt,
      prf: KeyDerivationPrf.HMACSHA256,
      iterationCount: 100_000,
      numBytesRequested: salt.Length
    );

    return Task.FromResult(
      new HashCreationResponse(
        salt,
        hash,
        value
      )
    );
  }
  public static Task<HashValidationResponse> ValidateHash(HashValidationRequest request)
  {
    byte[] hashResult = KeyDerivation.Pbkdf2(
      password: request.Value,
      salt: request.Salt,
      prf: KeyDerivationPrf.HMACSHA256,
      iterationCount: 100_000,
      request.Salt.Length
    );

    return Task.FromResult(
      new HashValidationResponse(
        hashResult.SequenceEqual(request.Hash),
        request.Value,
        request.Hash
      )
    );
  }

  public static string Serialize<TEnum>(TEnum @enum)
    where TEnum : struct
  {
    Type type = typeof(TEnum);
    MemberInfo? memberInfo = type.GetMember(@enum.ToString()!).FirstOrDefault();
    return memberInfo is null
      ? throw new InvalidOperationException()
      : memberInfo.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name ?? throw new InvalidOperationException();
  }

  public static TEnum Deserialize<TEnum>(string value)
    where TEnum : struct
  {
    Type type = typeof(TEnum);
    MemberInfo? memberInfo = type.GetMembers()
      .FirstOrDefault(mi => mi.GetCustomAttribute<JsonStringEnumMemberNameAttribute>() is JsonStringEnumMemberNameAttribute attr && attr.Name.Equals(value));
    return memberInfo is null
      ? throw new InvalidOperationException()
      : Enum.Parse<TEnum>(memberInfo.Name);
  }

  private const string ClaimPart = "cprt-";
  public static string FromClaim([RegularExpression(@"^[a-z]*\:[a-z]*$")] string claim)
    => $"{ClaimPart}{claim}";
  public static bool ValidatePolicyFromClaim(string policyName, [NotNullWhen(true)] out string? claim)
  {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    if (Regex.IsMatch(policyName, @$"^{ClaimPart}[a-z]*\:[a-z]*$") || policyName == $"{ClaimPart}root")
    {
      claim = policyName.Replace(ClaimPart, string.Empty);
      return true;
    }
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    claim = null;
    return false;
  }
}
