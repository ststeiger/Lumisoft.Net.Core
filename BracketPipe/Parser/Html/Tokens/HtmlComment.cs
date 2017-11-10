using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public sealed class HtmlComment : HtmlNode
  {
    #region Properties

    public bool DownlevelRevealedConditional { get; set; }
    public override HtmlTokenType Type { get { return HtmlTokenType.Comment; } }

    #endregion

    #region ctor

    public HtmlComment(string value) : base(TextPosition.Empty, value) { }
    /// <summary>
    /// Sets the default values.
    /// </summary>
    /// <param name="type">The type of the tag token.</param>
    /// <param name="position">The token's position.</param>
    public HtmlComment(TextPosition position, string value)
        : base(position, value)
    {
    }

    internal override void AddToDebugDisplay(StringBuilder builder)
    {
      builder.Append("<!--");
      builder.Append(Value);
      builder.Append("-->");
    }
    #endregion
  }
}
