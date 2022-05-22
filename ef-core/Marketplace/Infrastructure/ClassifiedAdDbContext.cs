using Marketplace.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure;

public class ClassifiedAdDbContext : DbContext
{
  private readonly ILoggerFactory _loggerFactory;

  public ClassifiedAdDbContext(DbContextOptions<ClassifiedAdDbContext> options, ILoggerFactory loggerFactory) : base(options) =>
    _loggerFactory = loggerFactory;

  public DbSet<ClassifiedAd>? ClassifiedAds { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
      .UseLoggerFactory(_loggerFactory)
      .EnableSensitiveDataLogging();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    _ = modelBuilder.ApplyConfiguration(
      new ClassifiedAdEntityTypeConfiguration());

    _ = modelBuilder.ApplyConfiguration(
      new PictureEntityTypeConfiguration());
  }
}

internal class ClassifiedAdEntityTypeConfiguration
  : IEntityTypeConfiguration<ClassifiedAd>
{
  public void Configure(EntityTypeBuilder<ClassifiedAd> builder)
  {
    _ = builder.HasKey(x => x.ClassifiedAdId);
    _ = builder.OwnsOne(x => x.Id);
    _ = builder.OwnsOne(x => x.OwnerId);
    _ = builder.OwnsOne(x => x.Title);
    _ = builder.OwnsOne(x => x.Text);
    _ = builder.OwnsOne(x => x.Price, p => p.OwnsOne(c => c.Currency));
    _ = builder.OwnsOne(x => x.ApprovedBy);
  }
}

internal class PictureEntityTypeConfiguration
  : IEntityTypeConfiguration<Picture>
{
  public void Configure(EntityTypeBuilder<Picture> builder)
  {
    _ = builder.HasKey(x => x.PictureId);
    _ = builder.OwnsOne(x => x.Id);
    _ = builder.OwnsOne(x => x.ParentId);
    _ = builder.OwnsOne(x => x.Size);
  }
}

public static class AppBuilderDatabaseExtensions
{
  public static void EnsureDatabase(
    this IApplicationBuilder app)
  {
    using IServiceScope? scope = app.ApplicationServices.CreateScope();

    ClassifiedAdDbContext? context = scope
      .ServiceProvider
      .GetRequiredService<ClassifiedAdDbContext>();

    if (!(context?.Database.EnsureCreated() ?? false))
    {
      context?.Database.Migrate();
    }
  }
}