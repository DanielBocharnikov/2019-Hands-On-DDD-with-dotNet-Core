using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;

namespace Marketplace.UserProfile;

public class UserProfileRepository : IUserProfileRepository
{
  private readonly MarketplaceDbContext _dbContext;

  public UserProfileRepository(MarketplaceDbContext dbContext)
    => _dbContext = dbContext;

  public async Task Add(Domain.UserProfile.UserProfile entity)
    => await _dbContext.UserProfiles.AddAsync(entity);

  public async Task<bool> Exists(UserId id)
    => await _dbContext.UserProfiles.FindAsync(id.Value) is not null;

  public async Task<Domain.UserProfile.UserProfile?> Load(UserId id)
    => await _dbContext.UserProfiles.FindAsync(id.Value);
}
