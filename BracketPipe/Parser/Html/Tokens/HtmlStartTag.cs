using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public class HtmlStartTag : HtmlTagNode, IEnumerable<KeyValuePair<String, String>>
  {
    #region Fields

    private readonly List<KeyValuePair<String, String>> _attributes = new List<KeyValuePair<string, string>>();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of attributes.
    /// </summary>
    public IList<KeyValuePair<String, String>> Attributes
    {
      get { return _attributes; }
    }

    /// <summary>
    /// Gets or sets the state of the self-closing flag.
    /// </summary>
    public override bool IsSelfClosing { get; set; }

    /// <summary>
    /// Gets a value indicating whether this tag is empty (contains no children).
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty { get { return IsSelfClosing || HtmlTextWriter.VoidElements.Contains(Value); } }

    /// <summary>
    /// Gets the value of the attribute with the given name or an empty
    /// string if the attribute is not available.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>The value of the attribute.</returns>
    public String this[String name]
    {
      get
      {
        for (var i = 0; i < _attributes.Count; i++)
        {
          if (_attributes[i].Key == name)
            return _attributes[i].Value;
        }

        return String.Empty;
      }
      set
      {
        for (var i = 0; i < _attributes.Count; i++)
        {
          if (_attributes[i].Key == name)
          {
            _attributes[i] = new KeyValuePair<string, string>(name, value);
            return;
          }
        }
        Add(name, value);
      }
    }

    public override HtmlTokenType Type { get { return HtmlTokenType.StartTag; } }

    #endregion

    #region ctor

    public HtmlStartTag(String name) : base(TextPosition.Empty, name) { }
    internal HtmlStartTag(TextPosition position) : base(position) { }
    public HtmlStartTag(TextPosition position, String name) : base(position, name) { }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a new attribute to the list of attributes.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    public HtmlStartTag Add(String name, String value)
    {
      _attributes.Add(new KeyValuePair<String, String>(name, value));
      return this;
    }

    public bool TryGetValue(string name, out string value)
    {
      for (var i = 0; i < _attributes.Count; i++)
      {
        if (_attributes[i].Key == name)
        {
          value = _attributes[i].Value;
          return true;
        }
      }
      value = string.Empty;
      return false;
    }

    /// <summary>
    /// Adds a new attribute to the list of attributes. The value will
    /// be set to an empty string.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    internal override void AddAttribute(String name)
    {
      _attributes.Add(new KeyValuePair<String, String>(name, String.Empty));
    }

    /// <summary>
    /// Sets the value of the last added attribute.
    /// </summary>
    /// <param name="value">The value to set.</param>
    internal override void SetAttributeValue(String value)
    {
      _attributes[_attributes.Count - 1] = new KeyValuePair<String, String>(_attributes[_attributes.Count - 1].Key, value);
    }

    public IEnumerator<KeyValuePair<String, String>> GetEnumerator()
    {
      return _attributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    internal override void AddToDebugDisplay(StringBuilder builder)
    {
      builder.Append('<').Append(Value);
      foreach (var attr in _attributes)
      {
        builder.Append(' ').Append(attr.Key);
        if (!string.IsNullOrEmpty(attr.Value))
          builder.Append('=').Append('"').Append(attr.Value).Append('"');
      }
      builder.Append('>');
    }

    #endregion
  }
}
