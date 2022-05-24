using Marketplace.Domain.ClassifiedAd;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdRepository : IClassifiedAdRepository
{
  private readonly IAsyncDocumentSession _session;

  public ClassifiedAdRepository(IAsyncDocumentSession session) =>
    _session = session;

  public Task Add(Domain.ClassifiedAd.ClassifiedAd entity) => _session
    .StoreAsync(entity, EntityId(entity.Id!));
  public Task<bool> Exists(ClassifiedAdId entityId) => _session
    .Advanced.ExistsAsync(EntityId(entityId));
  public Task<Domain.ClassifiedAd.ClassifiedAd> Load(ClassifiedAdId entityId) => _session
    .LoadAsync<Domain.ClassifiedAd.ClassifiedAd>(EntityId(entityId));

  private static string EntityId(ClassifiedAdId id) =>
    $"ClassifiedAd/{id.Value}";
}