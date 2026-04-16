namespace Identity.Center.Application.Abstractions.Data;

public interface IGlobalAtomicValues<TUser>
  where TUser : struct, IEquatable<TUser>
{
  TUser UserId { get; }
  DateTime UtcDatime { get; }
}
