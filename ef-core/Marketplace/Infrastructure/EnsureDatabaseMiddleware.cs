using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure;

public class EnsureDatabaseMiddleware
{
  private readonly RequestDelegate _next;

  public EnsureDatabaseMiddleware(RequestDelegate next) => _next = next;

  public async Task InvokeAsync(HttpContext context, MarketplaceDbContext db)
  {
    EnsureContextIsMigrated(db);
    await _next(context);
  }

  private static void EnsureContextIsMigrated(DbContext? context)
  {
    if (context is not null && !context.Database.EnsureCreated())
    {
      context.Database.Migrate();
    }
  }
}