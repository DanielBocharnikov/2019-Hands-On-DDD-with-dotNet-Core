namespace Marketplace.Domain;

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