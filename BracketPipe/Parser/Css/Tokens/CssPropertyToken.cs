using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public class CssPropertyToken : CssToken, IEnumerable<CssToken>
  {
    #region Fields

    readonly List<CssToken> _arguments;

    #endregion

    #region ctor

    public CssPropertyToken(string data, TextPosition position) : base(CssTokenType.Property, data, position)
    {
      _arguments = new List<CssToken>();
    }

    #endregion

    #region Properties

    public bool IsTerminated { get; set; }
    public int ArgumentCount { get { return _arguments.Count; } }
    public IEnumerable<CssToken> ArgumentTokens { get { return _arguments; } }

    #endregion

    #region Methods

    public void AddArgumentToken(CssToken token)
    {
      _arguments.Add(token);
    }

    public IEnumerator<CssToken> GetEnumerator()
    {
      return _arguments.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

  }
}
