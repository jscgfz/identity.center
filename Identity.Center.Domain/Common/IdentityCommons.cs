using System.Security.Cryptography;
using System.Text;

namespace Identity.Center.Domain.Common;

public static class IdentityCommons
{
  public static Encoding Encoding => Encoding.UTF8;
  public static byte[] NewHashKey => RandomNumberGenerator.GetBytes(32);
}
