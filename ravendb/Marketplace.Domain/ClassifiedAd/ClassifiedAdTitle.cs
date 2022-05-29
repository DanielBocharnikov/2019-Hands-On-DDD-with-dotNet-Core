using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class ClassifiedAdTitle : ValueObject
{
  public static readonly ClassifiedAdTitle None = new(string.Empty);

  public string Value { get; internal set; } = string.Empty;

  private static readonly Regex _regex = new("<.*?>");

  internal ClassifiedAdTitle(string value) => Value = value;

  /// <summary>
  /// Satisfies serialization requirements.
  /// </summary>
  private ClassifiedAdTitle()
  {
  }

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

    return new(value);
  }

  public static implicit operator string(ClassifiedAdTitle self) => self.Value;

  public static implicit operator ClassifiedAdTitle(string title) => new(title);

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value;
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
}
