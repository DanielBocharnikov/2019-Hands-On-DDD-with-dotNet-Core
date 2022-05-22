using Marketplace.Framework;

namespace Marketplace.Infrastructure;

public class EfCoreUnitOfWork : IUnitOfWork
{
  private readonly ClassifiedAdDbContext _db;

  public EfCoreUnitOfWork(ClassifiedAdDbContext db) => _db = db;

  public Task Commit() => _db.SaveChangesAsync();
}