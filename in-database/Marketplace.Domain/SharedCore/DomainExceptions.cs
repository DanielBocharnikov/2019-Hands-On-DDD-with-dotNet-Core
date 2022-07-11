namespace Marketplace.Domain.SharedCore;

public static class DomainExceptions
{
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

  public class InvalidEntityStateException : Exception
  {
    public InvalidEntityStateException(object entity, string message)
      : base($"Entity {entity.GetType().Name} state change rejected, {message}")
    {
    }

    public InvalidEntityStateException()
    {
    }

    public InvalidEntityStateException(string? message) : base(message)
    {
    }

    public InvalidEntityStateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
  }

  public class ProfanityFoundException : Exception
  {
    public ProfanityFoundException()
    {
    }

    public ProfanityFoundException(string? text)
      : base($"Profanity found in text: {text}")
    {
    }

    public ProfanityFoundException(string? text, Exception? innerException)
      : base($"Profanity found in text: {text}", innerException)
    {
    }
  }
}