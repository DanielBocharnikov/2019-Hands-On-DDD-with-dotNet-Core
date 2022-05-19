using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain;

public sealed class ClassifiedAdTitle : ValueObject
{
  private readonly string _value;
  private static readonly Regex _regex = new("<.*?>");

  public static ClassifiedAdTitle FromString(string title)
  {
    CheckValidity(title);
    return new(title);
  }

  public static ClassifiedAdTitle FromHtml(string htmlTitle)
  {
    string? supportedTagsReplaced = htmlTitle
      .Replace("<i>", "*")
      .Replace("</i>", "*")
      .Replace("<b>", "**")
      .Replace("</b>", "**");

    string? value = _regex.Replace(supportedTagsReplaced, string.Empty);

    CheckValidity(value);

    return new ClassifiedAdTitle(value);
  }

  internal ClassifiedAdTitle(string value) => _value = value;

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return _value;
  }

  private static void CheckValidity(string value)
  {
    if (value.Length > 100)
    {
      throw new ArgumentOutOfRangeException(
        paramName: nameof(value),
        message: "Title cannot be longer than 100 characters"
      );
    }
  }

  public static implicit operator string(ClassifiedAdTitle self) =>
    self._value;
}