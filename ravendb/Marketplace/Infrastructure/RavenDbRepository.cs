using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure;

public class RavenDbRepository<T, TId>
  where T : AggregateRoot<TId>
  where TId : ValueObject
{
  private readonly IAsyncDocumentSession _session;
  private readonly Func<TId, string> _entityId;

  public RavenDbRepository(
    IAsyncDocumentSession session,
    Func<TId, string> entityId)
  {
    _session = session;
    _entityId = entityId;
  }

  public async Task Add(T entity)
    => await _session.StoreAsync(entity, _entityId(entity.Id!));

  public async Task<bool> Exists(TId id)
    => await _session.Advanced.ExistsAsync(_entityId(id));

  public async Task<T> Load(TId id)
    => await _session.LoadAsync<T>(_entityId(id));
}