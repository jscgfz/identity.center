using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Primitives;

public abstract class Entity<TKey, TUser> : AggregateRoot, IKeyedEntity<TKey>, IAuditEntityFields<TUser>
  where TKey : struct, IEquatable<TKey>
  where TUser : struct, IEquatable<TUser>
{
  public virtual TKey Id { get; set; }
  public virtual DateTimeOffset CreatedAtUtc { get; set; }
  public virtual TUser CreatedBy { get; set; }
  public virtual DateTimeOffset? LastModifiedAtUtc { get; set; }
  public virtual TUser? LastModifiedBy { get; set; }
  public virtual bool IsDeleted { get; set; }
  public virtual DateTimeOffset? DeletedAtUtc { get; set; }
  public virtual TUser? DeletedBy { get; set; }
}

public abstract class Entity<TKey> : Entity<TKey, Guid>
  where TKey : struct, IEquatable<TKey>
{ }
