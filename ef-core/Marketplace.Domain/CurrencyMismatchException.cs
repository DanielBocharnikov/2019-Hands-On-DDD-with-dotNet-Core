namespace Marketplace.Domain;

public class CurrencyMismatchException : Exception
{
  public CurrencyMismatchException(string message) : base(message)
  {
  }

  public CurrencyMismatchException()
  {
  }

  public CurrencyMismatchException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
