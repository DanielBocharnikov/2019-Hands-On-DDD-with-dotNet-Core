namespace Marketplace.Domain
{
  public class ClassifiedAd
  {
    public Guid Id { get; init; }
    private Guid _ownerId;
    private string _title;
    private string _text;
    private decimal _price;

    public ClassifiedAd(Guid id)
    {
      if (id == default)
      {
        throw new ArgumentException(
          "Identity must be specified",
          nameof(id)
        );
      }

      this.Id = id;
    }

    public void SetTitle(string title) => _title = title;
    public void UpdateText(string text) => _text = text;
    public void UpdatePrice(decimal price) => _price = price;
  }
}
