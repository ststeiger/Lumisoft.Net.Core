namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;

  /// <summary>
  /// Represents a CSS string token.
  /// </summary>
  public sealed class CssStringToken : CssToken
  {
    #region Fields

    readonly Boolean _bad;
    readonly Char _quote;

    #endregion

    #region ctor

    public CssStringToken(String data, Boolean bad, Char quote, TextPosition position)
        : base(CssTokenType.String, data, position)
    {
      _bad = bad;
      _quote = quote;
    }

    #endregion

    #region Properties

    public Boolean IsBad
    {
      get { return _bad; }
    }

    public Char Quote
    {
      get { return _quote; }
    }

    #endregion
  }
}
