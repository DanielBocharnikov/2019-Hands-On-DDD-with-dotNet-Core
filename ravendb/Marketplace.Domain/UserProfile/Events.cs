namespace Marketplace.Domain.UserProfile;

public static class Events
{
  public record UserRegistered(
    Guid UserId,
    string FullName,
    string DisplayName);

  public record ProfilePhotoUploaded(Guid UserId, string PhotoUrl);

  public record UserFullNameUpdated(Guid UserId, string FullName);

  public record UserDisplayNameUpdated(Guid UserId, string DisplayName);
}