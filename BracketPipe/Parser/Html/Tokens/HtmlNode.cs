namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;
  using System.Diagnostics;
  using System.Text;

  /// <summary>
  /// The abstract base class of any HTML token.
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public abstract class HtmlNode
  {
    #region Fields

    readonly TextPosition _position;
    String _value;

    #endregion

    #region ctor

    public HtmlNode(TextPosition position)
        : this(position, null)
    {
    }

    public HtmlNode(TextPosition position, String value)
    {
      _position = position;
      _value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of a tag token.
    /// </summary>
    public String Value
    {
      get { return _value; }
      internal set { _value = value; }
    }

    /// <summary>
    /// Gets the position of the token.
    /// </summary>
    public TextPosition Position
    {
      get { return _position; }
    }

    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    public abstract HtmlTokenType Type { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected string DebuggerDisplay
    {
      get
      {
        var builder = new StringBuilder();
        AddToDebugDisplay(builder);
        return builder.ToString();
      }
    }

    #endregion

    #region Methods

    public override string ToString()
    {
      return this.Value;
    }

    internal abstract void AddToDebugDisplay(StringBuilder builder);
    #endregion
  }
}
