using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain;

public sealed class ClassifiedAdTitle : ValueObject
{
  private readonly string _value;

  public static ClassifiedAdTitle FromString(string title) =>
    new(title);

  public static ClassifiedAdTitle FromHtml(string htmlTitle)
  {
    string? supportedTagsReplaced = htmlTitle
      .Replace("<i>", "*")
      .Replace("</i>", "*")
      .Replace("<b>", "**")
      .Replace("</b>", "**");

    return new ClassifiedAdTitle(
      Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty)
    );
  }

  private ClassifiedAdTitle(string value)
  {
    if (value.Length > 100)
    {
      throw new ArgumentOutOfRangeException(
        paramName: nameof(value),
        message: "Title cannot be longer than 100 characters"
      );
    }

    _value = value;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return _value;
  }
}
