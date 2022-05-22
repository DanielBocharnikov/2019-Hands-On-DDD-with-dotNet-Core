using Marketplace.Domain;

namespace Marketplace.Infrastructure;

public class ClassifiedAdRepository : IClassifiedAdRepository
{
  private readonly ClassifiedAdDbContext _db;

  public ClassifiedAdRepository(ClassifiedAdDbContext db) =>
    _db = db;

  public async Task Add(ClassifiedAd entity) =>
    await _db.ClassifiedAds!.AddAsync(entity);

  public async Task<bool> Exists(ClassifiedAdId entityId) => await _db
    .ClassifiedAds!
    .FindAsync(Guid.Parse(entityId.ToString())) is not null;

  public async Task<ClassifiedAd?> Load(ClassifiedAdId entityId) =>
    await _db.ClassifiedAds!.FindAsync(Guid.Parse(entityId.ToString()));
}