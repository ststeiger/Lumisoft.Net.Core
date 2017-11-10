using System;

namespace BracketPipe
{
  /// <summary>
  /// Represents a <see cref="string"/> containing HTML
  /// </summary>
  /// <seealso cref="System.IEquatable{System.String}" />
  /// <seealso cref="System.IEquatable{BracketPipe.HtmlString}" />
  public sealed class HtmlString
    : IEquatable<string>
    , IEquatable<HtmlString>
  {
    private readonly string _html;

    /// <summary>
    /// Gets the number of characters in the current <see cref="HtmlString"/> object.
    /// </summary>
    /// <value>
    /// The number of characters in the current <see cref="HtmlString"/>.
    /// </value>
    public int Length { get { return _html.Length; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlString"/> class from HTML
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <exception cref="ArgumentNullException">html</exception>
    public HtmlString(string html)
    {
      if (html == null)
        throw new ArgumentNullException("html");
      _html = html;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(string other)
    {
      return string.Equals(_html, other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(HtmlString other)
    {
      if (other == null)
        return false;
      return string.Equals(_html, other._html);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj is string)
        return Equals((string)obj);
      if (obj is HtmlString)
        return Equals((HtmlString)obj);
      return false;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return _html.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return _html;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="HtmlString"/> to <see cref="System.String"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator string(HtmlString value)
    {
      return value._html;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="a">The first <see cref="HtmlString"/></param>
    /// <param name="b">The second <see cref="HtmlString"/></param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(HtmlString a, HtmlString b)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(a, b))
        return true;

      // If one is null, but not both, return false.
      if (((object)a == null) || ((object)b == null))
        return false;

      // Return true if the fields match:
      return a.Equals(b);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="a">The first <see cref="HtmlString"/></param>
    /// <param name="b">The second <see cref="HtmlString"/></param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(HtmlString a, HtmlString b)
    {
      return !(a == b);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(HtmlString a, string b)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(a, b))
        return true;

      // If one is null, but not both, return false.
      if (((object)a == null) || ((object)b == null))
        return false;

      // Return true if the fields match:
      return a.Equals(b);
    }
    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(HtmlString a, string b)
    {
      return !(a == b);
    }
    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <param name="a">a.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(string b, HtmlString a)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(a, b))
        return true;

      // If one is null, but not both, return false.
      if (((object)a == null) || ((object)b == null))
        return false;

      // Return true if the fields match:
      return a.Equals(b);
    }
    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <param name="a">a.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(string b, HtmlString a)
    {
      return !(a == b);
    }
  }
}
