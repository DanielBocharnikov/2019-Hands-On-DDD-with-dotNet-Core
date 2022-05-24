using Marketplace.Domain.SharedCore;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public class UserProfile : AggregateRoot<UserId>
{
  public FullName FullName { get; private set; } = new(string.Empty);

  public DisplayName DisplayName { get; private set; } = new(string.Empty);

  public string PhotoUrl { get; private set; } = string.Empty;

  private string DbId
  {
    get => $"UserProfile/{Id!.Value}";
    set { }
  }

  public UserProfile(UserId id, FullName fullName, DisplayName displayName)
    => Apply(new Events.UserRegistered(id, fullName, displayName));

  public void UpdateFullName(FullName newFullName)
    => Apply(new Events.UserFullNameUpdated(Id!, newFullName));

  public void UpdateDisplayName(DisplayName newDisplayName)
    => Apply(new Events.UserDisplayNameUpdated(Id!, newDisplayName));

  public void UpdateProfilePhoto(Uri newPhotoUrl)
    => Apply(new Events.ProfilePhotoUploaded(Id!, newPhotoUrl.ToString()));

  protected override void EnsureValidState()
  {
  }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.UserRegistered e:
        Id = new UserId(e.UserId);
        FullName = new FullName(e.FullName);
        DisplayName = new DisplayName(e.DisplayName);
        break;
      case Events.UserFullNameUpdated e:
        FullName = new FullName(e.FullName);
        break;
      case Events.UserDisplayNameUpdated e:
        DisplayName = new DisplayName(e.DisplayName);
        break;
      case Events.ProfilePhotoUploaded e:
        PhotoUrl = e.PhotoUrl;
        break;
      default:
        return;
    }
  }
}