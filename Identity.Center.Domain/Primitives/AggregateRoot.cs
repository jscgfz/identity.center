using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Primitives;

public abstract class AggregateRoot : IAggregateRoot
{
  private List<IDomainEvent> _domainEvents = [];
  public virtual IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public bool HasEvents => _domainEvents.Count > 0;

  public virtual void AddDomainEvent<TEvent>(TEvent @event) where TEvent : IDomainEvent
    => _domainEvents.Add(@event);

  public virtual void ClearDomainEvents()
    => _domainEvents.Clear();
}
