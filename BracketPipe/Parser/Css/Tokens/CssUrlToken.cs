namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;

  /// <summary>
  /// Represents a CSS URL token.
  /// </summary>
  public sealed class CssUrlToken : CssToken
  {
    #region Fields

    readonly Boolean _bad;
    readonly String _functionName;

    #endregion

    #region ctor

    public CssUrlToken(String functionName, String data, Boolean bad, TextPosition position)
        : base(CssTokenType.Url, data, position)
    {
      _bad = bad;
      _functionName = functionName;
    }

    #endregion

    #region Properties

    public Boolean IsBad
    {
      get { return _bad; }
    }

    public String FunctionName
    {
      get { return _functionName; }
    }

    #endregion
  }
}
