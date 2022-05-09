namespace Marketplace.Domain
{
  public class ClassifiedAd
  {
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

    public Guid Id { get; init; }
    private Guid _ownerId;
    private string _title;
    private string _text;
    private decimal _price;
  }
}
