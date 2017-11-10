namespace BracketPipe
{
  using System;

  /// <summary>
  /// Represents a CSS color token.
  /// </summary>
  public sealed class CssColorToken : CssToken
  {
    #region ctor

    public CssColorToken(String data, TextPosition position)
        : base(CssTokenType.Color, data, position)
    {
    }

    #endregion

    #region Properties

    public Boolean IsBad
    {
      get { return Data.Length != 3 && Data.Length != 6; }
    }

    #endregion
  }
}
