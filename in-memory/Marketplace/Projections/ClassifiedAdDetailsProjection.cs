using Marketplace.Domain.ClassifiedAd;
using Marketplace.Framework;

namespace Marketplace.Projections;

public class ClassifiedAdDetailsProjection : IProjection
{
  private readonly List<ReadModels.ClassifiedAdDetails> _items;

  public ClassifiedAdDetailsProjection(
    List<ReadModels.ClassifiedAdDetails> items) => _items = items;

  public Task Project(object @event)
  {
    switch (@event)
    {
      case Events.ClassifiedAdCreated e:
        _items.Add(new ReadModels.ClassifiedAdDetails(
          ClassifiedAdId: e.Id,
          SellerId: e.OwnerId,
          Title: string.Empty,
          Price: decimal.Zero,
          CurrencyCode: string.Empty,
          Description: string.Empty,
          SellerDisplayName: string.Empty,
          Array.Empty<string>()
        ));
        break;

      case Events.ClassifiedAdTitleChanged e:
        UpdateItem(e.Id, ad => ad with
        {
          Title = e.Title
        });
        break;

      case Events.ClassifiedAdTextUpdated e:
        UpdateItem(e.Id, ad => ad with { Description = e.Text });
        break;

      case Events.ClassifiedAdPriceUpdated e:
        UpdateItem(e.Id, ad => ad with
        {
          Price = e.Price,
          CurrencyCode = e.CurrencyCode
        });
        break;

      case Events.PictureAddedToClassifiedAd e:
        UpdateItem(e.ClassifiedAdId, ad => ad with
        {
          PhotoUrls = ad.PhotoUrls.Append(e.Url).ToArray()
        });
        break;

      case Domain.UserProfile.Events.UserDisplayNameUpdated e:
        UpdateMultipleItems(x => x.SellerId == e.UserId,
          x => x with { SellerDisplayName = e.DisplayName });
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