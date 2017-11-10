namespace BracketPipe
{
  using System;
  using System.Globalization;

  /// <summary>
  /// Represents a CSS unit token.
  /// </summary>
  public sealed class CssUnitToken : CssToken
  {
    #region Fields

    readonly String _unit;

    #endregion

    #region ctor

    public CssUnitToken(CssTokenType type, String value, String dimension, TextPosition position)
        : base(type, value, position)
    {
      _unit = dimension;
    }

    #endregion

    #region Properties

    public Single Value
    {
      get { return Single.Parse(Data, CultureInfo.InvariantCulture); }
    }

    public String Unit
    {
      get { return _unit; }
    }

    #endregion
  }
}
