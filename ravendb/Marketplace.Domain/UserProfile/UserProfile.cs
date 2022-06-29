using Marketplace.Domain.SharedCore;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public class UserProfile : AggregateRoot<UserId>
{
  public FullName FullName { get; private set; } = FullName.None;

  public DisplayName DisplayName { get; private set; } = DisplayName.None;

  public string PhotoUrl { get; private set; } = string.Empty;

  /// <summary>
  /// Used by RavenDb
  /// </summary>
  /// <value></value>
  private string DbId
  {
    get => $"UserProfile/{Id.Value}";
    set { }
  }

  public UserProfile(UserId id, FullName fullName, DisplayName displayName)
    => Apply(new Events.UserRegistered(id, fullName, displayName));

  public void UpdateFullName(FullName newFullName)
    => Apply(new Events.UserFullNameUpdated(Id, newFullName));

  public void UpdateDisplayName(DisplayName newDisplayName)
    => Apply(new Events.UserDisplayNameUpdated(Id, newDisplayName));

  public void UpdateProfilePhoto(Uri newPhotoUrl)
    => Apply(new Events.ProfilePhotoUploaded(Id, newPhotoUrl.ToString()));

  protected override void EnsureValidState()
  {
    bool valid = Id != UserId.None
      && FullName != FullName.None
      && DisplayName != DisplayName.None;

    if (!valid)
    {
      throw new DomainExceptions.InvalidEntityStateException(
        this, "Post-checks failed");
    }
  }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.UserRegistered e:
        Id = new UserId { Value = e.UserId };
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