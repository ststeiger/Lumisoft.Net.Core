namespace BracketPipe
{
  using System;

  /// <summary>
  /// Represents a CSS comment token.
  /// </summary>
  public sealed class CssCommentToken : CssToken
  {
    #region Fields

    readonly Boolean _bad;

    #endregion

    #region ctor

    public CssCommentToken(String data, Boolean bad, TextPosition position)
        : base(CssTokenType.Comment, data, position)
    {
      _bad = bad;
    }

    #endregion

    #region Properties

    public Boolean IsBad
    {
      get { return _bad; }
    }

    #endregion
  }
}
