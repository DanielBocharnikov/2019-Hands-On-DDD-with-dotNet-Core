namespace Marketplace.UserProfile;

public static class Contracts
{
  public static class V1
  {
    public record RegisterUser(
      Guid UserId,
      string FullName,
      string DisplayName);

    public record UpdateUserFullName(Guid UserId, string FullName);

    public record UpdateUserDisplayName(Guid UserId, string DisplayName);

    public record UpdateUserProfilePhoto(Guid UserId, string PhotoUrl);
  }
}