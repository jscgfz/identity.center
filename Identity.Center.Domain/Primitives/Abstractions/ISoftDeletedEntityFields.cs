namespace Identity.Center.Domain.Primitives.Abstractions;

public interface ISoftDeletedEntityFields<TUser>
  where TUser : struct, IEquatable<TUser>
{
  bool IsDeleted { get; }
  DateTimeOffset? DeletedAtUtc { get; }
  TUser? DeletedBy { get; }
}
