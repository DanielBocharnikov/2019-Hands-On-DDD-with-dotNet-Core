namespace Marketplace.Domain
{
  public class Price : Money
  {
    public Price(decimal amount) : base(amount)
    {
      if (amount < 0)
      {
        throw new ArgumentOutOfRangeException(
          paramName: nameof(amount),
          message: "Price cannot be negative"
        );
      }
    }
  }
}
