namespace Marketplace.Domain;

public class CurrencyMismatchException : Exception
{
  public CurrencyMismatchException(string message) : base(message)
  {
  }
}
