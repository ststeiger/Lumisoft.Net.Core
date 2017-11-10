namespace BracketPipe
{
  using System;

  /// <summary>
  /// Represents a CSS keyword token.
  /// </summary>
  public sealed class CssKeywordToken : CssToken
  {
    #region ctor

    public CssKeywordToken(CssTokenType type, String data, TextPosition position)
        : base(type, data, position)
    {
    }

    #endregion
  }
}
