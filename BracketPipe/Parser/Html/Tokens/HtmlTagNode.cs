namespace BracketPipe
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Class for StartTagToken and EndTagToken.
  /// </summary>
  public abstract class HtmlTagNode : HtmlNode
  {
    #region Properties

    /// <summary>
    /// Gets or sets the state of the self-closing flag.
    /// </summary>
    public virtual Boolean IsSelfClosing
    {
      get { return false; }
      set { /* do nothing */ }
    }
    #endregion

    #region ctor

    public HtmlTagNode(TextPosition position) : base (position) { }
    public HtmlTagNode(TextPosition position, String value) : base(position, value) { }

    #endregion

    #region Methods

    internal virtual void AddAttribute(String name) { }
    internal virtual void SetAttributeValue(String value) { }

    #endregion
  }
}
