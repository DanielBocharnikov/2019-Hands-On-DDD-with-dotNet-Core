using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Framework
{
  public abstract class Entity
  {
    private readonly List<object> _events = new List<object>();

    protected Entity()
    {
    }

    protected void Apply(object @event)
    {
      When(@event);
      EnsureValidState();
      _events.Add(@event);
    }

    protected abstract void When(object @event);

    protected abstract void EnsureValidState();

    public IEnumerable<object> GetChanges() => _events.AsEnumerable();

    public void ClearChanges() => _events.Clear();
  }
}