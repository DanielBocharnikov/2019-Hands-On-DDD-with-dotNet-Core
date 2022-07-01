using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class ClassifiedAdTitle : ValueObject
{
  /// <summary>
  /// Factory method to represent that ClassifiedAdTitle is absent.
  /// </summary>
  public static readonly ClassifiedAdTitle None = new(string.Empty);

  public string Value { get; internal set; } = string.Empty;

  private static readonly Regex _regex = new("<.*?>");

  /// <summary>
  /// Ctor that used for reapplying event from aggregate root.
  /// </summary>
  /// <param name="value"></param>
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
    string supportedTagsReplaced = htmlTitle
      .Replace("<i>", "*")
      .Replace("</i>", "*")
      .Replace("<b>", "**")
      .Replace("</b>", "**");

    string value = _regex.Replace(supportedTagsReplaced, string.Empty);

    CheckValidity(value);

    return new(value);
  }

  /// <summary>
  /// Every ClassifiedAdTitle can be a string, so we use implicit convertion
  /// operation
  /// </summary>
  /// <param name="self"></param>
  public static implicit operator string(ClassifiedAdTitle self) => self.Value;

  /// <summary>
  /// Not every string can be ClassifiedAdTitle, so we use explicit convertion
  /// operation, and under the hood we using factory method to validate string.
  /// </summary>
  /// <param name="title"></param>
  public static explicit operator ClassifiedAdTitle(string title)
    => FromString(title);

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }

  private static void CheckValidity(string value)
  {
    if (value.Length > 100)
    {
      throw new ArgumentOutOfRangeException(paramName: nameof(value),
        message: "Title cannot be longer than 100 characters"
      );
    }
  }
}