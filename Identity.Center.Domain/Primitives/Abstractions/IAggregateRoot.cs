namespace Identity.Center.Domain.Primitives.Abstractions;

public interface IAggregateRoot
{
  void AddDomainEvent<TEvent>(TEvent @event) where TEvent : IDomainEvent;
  IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
  void ClearDomainEvents();
  bool HasEvents { get; }
}
