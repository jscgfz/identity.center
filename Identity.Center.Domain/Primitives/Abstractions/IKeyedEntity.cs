namespace Identity.Center.Domain.Primitives.Abstractions;

public interface IKeyedEntity<TKey>
  where TKey : struct, IEquatable<TKey>
{
  TKey Id { get; }
}
