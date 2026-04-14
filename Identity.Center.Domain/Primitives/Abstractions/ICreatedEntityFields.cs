namespace Identity.Center.Domain.Primitives.Abstractions;

public interface ICreatedEntityFields<TUser>
  where TUser : struct, IEquatable<TUser>
{
  DateTimeOffset CreatedAtUtc { get; }
  TUser CreatedBy { get; }
}
