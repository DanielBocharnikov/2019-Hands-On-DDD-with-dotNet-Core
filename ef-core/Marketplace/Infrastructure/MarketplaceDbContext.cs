using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure;

public class MarketplaceDbContext : DbContext
{
  private readonly ILoggerFactory _loggerFactory;

  public MarketplaceDbContext(
      DbContextOptions<MarketplaceDbContext> options,
      ILoggerFactory loggerFactory
  ) : base(options) => _loggerFactory = loggerFactory;

  public DbSet<Domain.ClassifiedAd.ClassifiedAd> ClassifiedAds
    => Set<Domain.ClassifiedAd.ClassifiedAd>();

  public DbSet<Domain.UserProfile.UserProfile> UserProfiles
    => Set<Domain.UserProfile.UserProfile>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseLoggerFactory(_loggerFactory)
      .EnableSensitiveDataLogging();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    _ = modelBuilder.ApplyConfigurationsFromAssembly(
      Assembly.GetExecutingAssembly());

    base.OnModelCreating(modelBuilder);
  }
}

internal class ClassifiedAdEntityTypeConfiguration
  : IEntityTypeConfiguration<Domain.ClassifiedAd.ClassifiedAd>
{
  public void Configure(EntityTypeBuilder<Domain.ClassifiedAd.ClassifiedAd> builder)
  {
    _ = builder.ToTable("ClassifiedAds");
    _ = builder.Property(x => x.ClassifiedAdId).ValueGeneratedNever();
    _ = builder.Property(x => x.Version).IsConcurrencyToken();
    _ = builder.HasKey(x => x.ClassifiedAdId);
    _ = builder.OwnsOne(x => x.Id);
    _ = builder.OwnsOne(x => x.OwnerId);
    _ = builder.OwnsOne(x => x.Title);
    _ = builder.OwnsOne(x => x.Text);
    _ = builder.OwnsOne(x => x.Price, p => p.OwnsOne(c => c.Currency));
    _ = builder.OwnsOne(x => x.ApprovedBy);
    _ = builder.OwnsMany(c => c.Pictures, picture =>
    {
      _ = picture.ToTable("Pictures");
      _ = picture.WithOwner().HasForeignKey(x => x.OwnerClassifiedAdId);
      _ = picture.Property(x => x.PictureId).ValueGeneratedNever();
      _ = picture.HasKey(x => x.PictureId);
      _ = picture.OwnsOne(x => x.Id);
      _ = picture.OwnsOne(x => x.ParentId);
      _ = picture.OwnsOne(x => x.Size);
    });
    _ = builder.Navigation(x => x.Pictures).AutoInclude();
  }
}

internal class UserProfileEntityTypeConfiguration
  : IEntityTypeConfiguration<Domain.UserProfile.UserProfile>
{
  public void Configure(
    EntityTypeBuilder<Domain.UserProfile.UserProfile> builder)
  {
    _ = builder.HasKey(x => x.UserProfileId);
    _ = builder.OwnsOne(x => x.Id);
    _ = builder.OwnsOne(x => x.DisplayName);
    _ = builder.OwnsOne(x => x.FullName);
  }
}
