using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Framework
{
  public abstract class AggregateRoot<TId>
    where TId : ValueObject
  {
    private readonly List<object> _changes;

    public TId Id { get; protected set; } = default!;

    public int Version { get; private set; } = -1;

    protected AggregateRoot() => _changes = new List<object>();

    public void Load(IEnumerable<object> history)
    {
      foreach (object @event in history)
      {
        When(@event);
        Version++;
      }
    }

    public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

    public void ClearChanges() => _changes.Clear();

    protected void Apply(object @event)
    {
      When(@event);
      EnsureValidState();
      _changes.Add(@event);
    }

    protected abstract void When(object @event);

    protected abstract void EnsureValidState();

    protected void ApplyToEntity(IInternalEventHandler entity, object @event) =>
      entity?.Handle(@event);
  }
}