using System;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
  public static class ApplicationServiceExtensions
  {
    public static async Task HandleUpdate<T, TId>(
      this IApplicationService service,
      IAggregateStore store,
      TId aggregateId,
      Action<T> operation
    ) where T : AggregateRoot<TId> where TId : ValueObject
    {
      if (service == null)
      {
        throw new ArgumentNullException(nameof(service));
      }

      T aggregate = await store.Load<T, TId>(aggregateId);

      if (aggregate is null)
      {
        throw new InvalidOperationException(
          $"Entity with Id {aggregateId} cannot be found.");
      }

      operation(aggregate);

      await store.Save<T, TId>(aggregate);
    }
  }
}