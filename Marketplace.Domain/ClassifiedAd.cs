namespace Marketplace.Domain
{
  public class ClassifiedAd
  {
    public Guid Id { get; init; }
    private Guid _ownerId;
    private string _title;
    private string _text;
    private decimal _price;

    public ClassifiedAd(Guid id, Guid ownerId)
    {
      if (id == default)
      {
        throw new ArgumentException(
          "Classified Ad identity must be specified",
          nameof(id)
        );
      }

      if (ownerId == default)
      {
        throw new ArgumentException(
          "Owner identity must be specified",
          nameof(ownerId)
        );
      }

      Id = id;
      _ownerId = ownerId;
    }

    public void SetTitle(string title) => _title = title;
    public void UpdateText(string text) => _text = text;
    public void UpdatePrice(decimal price) => _price = price;
  }
}
