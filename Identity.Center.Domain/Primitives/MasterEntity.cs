using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Primitives;

public abstract class MasterEntity<TKey, TUser> : Entity<TKey, TUser>, IMasterFields
  where TKey : struct, IEquatable<TKey>
  where TUser : struct, IEquatable<TUser>
{
  public required string Name { get; set; }
  public string? Description { get; set; }
}

public abstract class MasterEntity<TKey> : MasterEntity<TKey, Guid>
  where TKey : struct, IEquatable<TKey>
{ }
