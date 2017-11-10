using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public sealed class HtmlEndOfFile : HtmlNode
  {
    #region Properties

    public override HtmlTokenType Type { get { return HtmlTokenType.EndOfFile; } }

    #endregion

    #region ctor

    /// <summary>
    /// Sets the default values.
    /// </summary>
    /// <param name="type">The type of the tag token.</param>
    /// <param name="position">The token's position.</param>
    public HtmlEndOfFile(TextPosition position)
        : base(position)
    {
    }

    internal override void AddToDebugDisplay(StringBuilder builder)
    {
      builder.Append("{EOF}");
    }

    #endregion
  }
}
