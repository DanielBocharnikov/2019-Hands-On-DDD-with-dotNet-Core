using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure;

public static class AppBuilderDatabaseExtensions
{
  // public static IApplicationBuilder EnsureDatabase(
  //   this IApplicationBuilder builder)
  // {
  //   EnsureContextIsMigrated(builder.ApplicationServices
  //     .GetService<MarketplaceDbContext>());

  //   return builder;
  // }

  // private static void EnsureContextIsMigrated(
  //   DbContext? context)
  // {
  //   if (context is not null && !context.Database.EnsureCreated())
  //   {
  //     context.Database.Migrate();
  //   }
  // }

  public static IApplicationBuilder EnsureDatabase(
    this IApplicationBuilder builder)
      => builder.UseMiddleware<EnsureDatabaseMiddleware>();

  public static IServiceCollection AddPostgresDbContext<T>(
    this IServiceCollection services,
    string connectionString) where T : DbContext
  {
    _ = services.AddDbContext<T>(options =>
      options.UseNpgsql(connectionString));

    return services;
  }
}
