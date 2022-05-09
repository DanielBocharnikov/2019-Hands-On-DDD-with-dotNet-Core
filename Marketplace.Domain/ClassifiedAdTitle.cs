using System.Text.RegularExpressions;

using Marketplace.Framework;

namespace Marketplace.Domain
{
  public class ClassifiedAdTitle : ValueObject
  {
    public static ClassifiedAdTitle FromString(string title) =>
      new ClassifiedAdTitle(title);

    public static ClassifiedAdTitle FromHtml(string htmlTitle)
    {
      var supportedTagsReplaced = htmlTitle
        .Replace("<i>", "*")
        .Replace("</i>", "*")
        .Replace("<b>", "**")
        .Replace("</b>", "**");

      return new ClassifiedAdTitle(
        Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty)
      );
    }

    private readonly string _value;

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
}
