namespace Identity.Center.Domain.Primitives.Abstractions;

public interface ILastModifiedEntityFields<TUser>
  where TUser : struct, IEquatable<TUser>
{
  DateTimeOffset? LastModifiedAtUtc { get; }
  TUser? LastModifiedBy { get; }
}
