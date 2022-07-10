using Marketplace.Domain.UserProfile;
using Marketplace.Framework;

namespace Marketplace.Projections;

public class UserProfileDetailsProjection : IProjection
{
  private readonly List<ReadModels.UserDetails> _items;

  public UserProfileDetailsProjection(List<ReadModels.UserDetails> items)
    => _items = items;

  public Task Project(object @event)
  {
    switch (@event)
    {
      case Events.UserRegistered e:
        _items.Add(new ReadModels.UserDetails(
          e.UserId,
          e.DisplayName,
          PhotoUrl: string.Empty));
        break;

      case Events.UserDisplayNameUpdated e:
        UpdateItem(e.UserId, x => x with { DisplayName = e.DisplayName });
        break;

      case Events.ProfilePhotoUploaded e:
        UpdateItem(e.UserId, x => x with { PhotoUrl = e.PhotoUrl });
        break;

      default:
        return Task.CompletedTask;
    }

    return Task.CompletedTask;
  }

  private void UpdateItem(Guid id,
    Func<ReadModels.UserDetails, ReadModels.UserDetails> update)
  {
    ReadModels.UserDetails? item = _items.Find(x => x.UserId == id);

    if (item is null)
    {
      return;
    }

    ReadModels.UserDetails newItem = update(item);
    ReassignItem(currentItem: item, newItem: newItem);
  }

  private void ReassignItem(ReadModels.UserDetails currentItem,
    ReadModels.UserDetails newItem)
  {
    int itemIndex = _items.IndexOf(currentItem);
    _items[itemIndex] = newItem;
  }
}