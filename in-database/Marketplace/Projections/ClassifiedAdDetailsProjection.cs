using Marketplace.Framework;
using static Marketplace.Domain.ClassifiedAd.Events;
using static Marketplace.Domain.UserProfile.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;

namespace Marketplace.Projections;

public class ClassifiedAdDetailsProjection : IProjection
{
  private readonly List<ReadModels.ClassifiedAdDetails> _items;
  private readonly Func<Guid, string> _getUserDisplayName;

  public ClassifiedAdDetailsProjection(
    List<ReadModels.ClassifiedAdDetails> items,
    Func<Guid, string> getUserDisplayName)
  {
    _items = items;
    _getUserDisplayName = getUserDisplayName;
  }

  public Task Project(object @event)
  {
    switch (@event)
    {
      case ClassifiedAdCreated e:
        _items.Add(new ReadModels.ClassifiedAdDetails(
          ClassifiedAdId: e.Id,
          SellersId: e.OwnerId,
          SellersDisplayName: _getUserDisplayName(e.OwnerId),
          SellersPhotoUrl: string.Empty,
          Title: string.Empty,
          Price: decimal.Zero,
          CurrencyCode: string.Empty,
          Description: string.Empty,
          PhotoUrls: Array.Empty<string>()
        ));
        break;

      case ClassifiedAdTitleChanged e:
        UpdateItem(e.Id, ad => ad with
        {
          Title = e.Title
        });
        break;

      case ClassifiedAdTextUpdated e:
        UpdateItem(e.Id, ad => ad with { Description = e.Text });
        break;

      case ClassifiedAdPriceUpdated e:
        UpdateItem(e.Id, ad => ad with
        {
          Price = e.Price,
          CurrencyCode = e.CurrencyCode
        });
        break;

      case PictureAddedToClassifiedAd e:
        UpdateItem(e.ClassifiedAdId, ad => ad with
        {
          PhotoUrls = ad.PhotoUrls.Append(e.Url).ToArray()
        });
        break;

      case UserDisplayNameUpdated e:
        UpdateMultipleItems(
          x => x.SellersId == e.UserId,
          x => x with { SellersDisplayName = e.DisplayName });
        break;

      case V1.ClassifiedAdPublished e:
        UpdateItem(e.Id, ad => ad with { SellersPhotoUrl = e.SellersPhotoUrl });
        break;

      default:
        return Task.CompletedTask;
    }

    return Task.CompletedTask;
  }

  private void UpdateItem(Guid id,
    Func<ReadModels.ClassifiedAdDetails, ReadModels.ClassifiedAdDetails> update)
  {
    ReadModels.ClassifiedAdDetails? item = _items.Find(
      x => x.ClassifiedAdId == id);

    if (item is null)
    {
      return;
    }

    ReadModels.ClassifiedAdDetails newItem = update(item);
    ReassignItem(currentItem: item, newItem: newItem);
  }

  private void UpdateMultipleItems(
    Func<ReadModels.ClassifiedAdDetails, bool> query,
    Func<ReadModels.ClassifiedAdDetails, ReadModels.ClassifiedAdDetails> update)
  {
    var classifiedAdDetailsFilteredByOwnerId = _items.Where(query).ToList();

    for (int i = 0; i < classifiedAdDetailsFilteredByOwnerId.Count; i++)
    {
      ReadModels.ClassifiedAdDetails newItem = update(_items[i]);
      ReassignItem(_items[i], newItem);
    }
  }

  private void ReassignItem(ReadModels.ClassifiedAdDetails currentItem,
    ReadModels.ClassifiedAdDetails newItem)
  {
    int itemIndex = _items.IndexOf(currentItem);
    _items[itemIndex] = newItem;
  }
}