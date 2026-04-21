namespace Identity.Center.Domain.Primitives.Abstractions;

public interface IAuditEntityFields<TUser> : ICreatedEntityFields<TUser>, ILastModifiedEntityFields<TUser>, ISoftDeletedEntityFields<TUser>
  where TUser : struct, IEquatable<TUser>
{ }
