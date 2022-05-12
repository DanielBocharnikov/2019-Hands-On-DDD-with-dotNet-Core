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

    protected void Raise(object @event) => _events.Add(@event);

    public IEnumerable<object> GetChanges() => _events.AsEnumerable();

    public void ClearChanges() => _events.Clear();
  }
}