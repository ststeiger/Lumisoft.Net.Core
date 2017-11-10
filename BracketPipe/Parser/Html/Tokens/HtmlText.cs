using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public sealed class HtmlText : HtmlNode
  {
    #region Properties

    public override HtmlTokenType Type { get { return HtmlTokenType.Text; } }

    #endregion

    #region ctor

    public HtmlText(string value) : base(TextPosition.Empty, value) { }
    /// <summary>
    /// Sets the default values.
    /// </summary>
    /// <param name="type">The type of the tag token.</param>
    /// <param name="position">The token's position.</param>
    public HtmlText(TextPosition position, string value) : base(position, value) { }

    internal override void AddToDebugDisplay(StringBuilder builder)
    {
      builder.EnsureCapacity(builder.Length + Value.Length);
      for (var i = 0; i < Value.Length; i++)
      {
        switch (Value[i])
        {
          case '\r':
            builder.Append('\\').Append('r');
            break;
          case '\n':
            builder.Append('\\').Append('n');
            break;
          case '\t':
            builder.Append('\\').Append('t');
            break;
          default:
            builder.Append(Value[i]);
            break;
        }
      }
    }

    #endregion
  }
}
