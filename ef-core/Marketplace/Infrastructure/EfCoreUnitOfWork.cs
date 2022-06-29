using Marketplace.Framework;

namespace Marketplace.Infrastructure;

public class EfCoreUnitOfWork : IUnitOfWork
{
  private readonly MarketplaceDbContext _db;

  public EfCoreUnitOfWork(MarketplaceDbContext db) => _db = db;

  public Task Commit() => _db.SaveChangesAsync();
}