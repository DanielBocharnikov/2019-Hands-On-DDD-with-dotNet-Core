namespace Marketplace.UserProfile;

public static class Commands
{
  public static class V1
  {
    public record class RegisterUser(Guid UserId, string FullName,
      string DisplayName);

    public record class UpdateUserFullName(Guid UserId, string FullName);

    public record class UpdateUserDisplayName(Guid UserId, string DisplayName);

    public record class UpdateUserProfilePhoto(Guid UserId, string PhotoUrl);
  }
}