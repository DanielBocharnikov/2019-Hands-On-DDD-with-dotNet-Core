using Marketplace.Domain;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure;

public class ClassifiedAdRepository : IClassifiedAdRepository
{
  private readonly IAsyncDocumentSession _session;

  public ClassifiedAdRepository(IAsyncDocumentSession session) =>
    _session = session;

  public Task Add(ClassifiedAd entity) => _session
    .StoreAsync(entity, EntityId(entity.Id!));
  public Task<bool> Exists(ClassifiedAdId entityId) => _session
    .Advanced.ExistsAsync(EntityId(entityId));
  public Task<ClassifiedAd> Load(ClassifiedAdId entityId) => _session
    .LoadAsync<ClassifiedAd>(EntityId(entityId));

  private static string EntityId(ClassifiedAdId id) =>
    $"ClassifiedAd/{id.Value}";
}