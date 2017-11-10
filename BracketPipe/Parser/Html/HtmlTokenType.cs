namespace BracketPipe
{
  /// <summary>
  /// An enumation of all possible tokens.
  /// </summary>
  public enum HtmlTokenType : byte
  {
    /// <summary>
    /// The DOCTYPE token.
    /// </summary>
    Doctype = 10,
    /// <summary>
    /// The start tag token to mark open tags.
    /// </summary>
    StartTag = 1,
    /// <summary>
    /// The end tag token to mark ending tags.
    /// </summary>
    EndTag = 15,
    /// <summary>
    /// The comment tag to mark comments.
    /// </summary>
    Comment = 8,
    /// <summary>
    /// The character token to mark a character data.
    /// </summary>
    Text = 3,
    /// <summary>
    /// The End-Of-File token to mark the end.
    /// </summary>
    EndOfFile = 18
  }
}
