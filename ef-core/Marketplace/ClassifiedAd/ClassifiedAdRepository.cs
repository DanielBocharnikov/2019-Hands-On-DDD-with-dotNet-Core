using Marketplace.Domain.ClassifiedAd;
using Marketplace.Infrastructure;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdRepository : IClassifiedAdRepository
{
  private readonly MarketplaceDbContext _db;

  public ClassifiedAdRepository(MarketplaceDbContext db)
    => _db = db;

  public async Task Add(Domain.ClassifiedAd.ClassifiedAd entity)
    => await _db.ClassifiedAds.AddAsync(entity);

  public async Task<bool> Exists(ClassifiedAdId entityId)
    => await _db.ClassifiedAds.FindAsync(
        Guid.Parse(entityId.ToString())) is not null;

  public async Task<Domain.ClassifiedAd.ClassifiedAd> Load(
    ClassifiedAdId entityId)
  {
    Domain.ClassifiedAd.ClassifiedAd? result = await _db.ClassifiedAds
      .FindAsync(Guid.Parse(entityId.ToString()));

    if (result is null)
    {
      throw new ArgumentException($"There is no Classified Ad with such id: {entityId}");
    }

    return result;
  }
}