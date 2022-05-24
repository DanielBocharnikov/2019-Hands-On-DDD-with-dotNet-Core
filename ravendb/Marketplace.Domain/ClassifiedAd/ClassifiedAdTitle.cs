using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class ClassifiedAdTitle : ValueObject
{
  public string? Value { get; init; }

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

  internal ClassifiedAdTitle(string value) => Value = value;

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
    self.Value!;

  public static implicit operator ClassifiedAdTitle(string value) =>
    new(value);

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value!;
  }

  private ClassifiedAdTitle() { }
}
