using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace BracketPipe
{
  public static partial class Html
  {
    /// <summary>
    /// Appends the <paramref name="value"/> after encoding characters (e.g. &lt;&gt;) 
    /// that have special meaning in HTML.
    /// </summary>
    /// <param name="builder">The builder to append to.</param>
    /// <param name="value">The value to encode and append.</param>
    /// <returns><paramref name="builder"/> so additional methods can be chained</returns>
    public static StringBuilder AppendHtmlEncoded(this StringBuilder builder, string value)
    {
      builder.EnsureCapacity(builder.Length + value.Length + 8);
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case Symbols.Ampersand: builder.Append("&amp;"); break;
          case Symbols.NoBreakSpace: builder.Append("&nbsp;"); break;
          case Symbols.GreaterThan: builder.Append("&gt;"); break;
          case Symbols.LessThan: builder.Append("&lt;"); break;
          case Symbols.DoubleQuote: builder.Append("&quot;"); break;
          case Symbols.SingleQuote: builder.Append("&apos;"); break;
          default: builder.Append(value[i]); break;
        }
      }
      return builder;
    }

    /// <summary>
    /// Formats the value of the current instance with HTML encoding.
    /// </summary>
    /// <param name="formattable">The formattable value to encode.</param>
    /// <returns>The value of the current instance HTML encoded</returns>
    public static string Format(IFormattable formattable)
    {
      return formattable.ToString(null, HtmlFormatProvider.Instance);
    }

    /// <summary>
    /// Replaces the format items in a specified string with the HTML-encoded 
    /// string representations of corresponding objects in a specified array. 
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An <see cref="object"/> array that contains zero or more objects to format.</param>
    /// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by 
    /// the HTML-encoded string representation of the corresponding objects in <paramref name="args"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="format"/>
    /// or <paramref name="args"/> is <c>null</c></exception>
    /// <exception cref="FormatException">
    /// <paramref name="format"/> is invalid
    /// -or-
    /// The index of a format item is less than zero, or greater
    /// than or equal to the length of the <paramref name="args"/> array.   
    /// </exception>
    public static string Format(string format, params object[] args)
    {
      return string.Format(HtmlFormatProvider.Instance, format, args);
    }

    /// <summary>
    /// Render parsed HTML to a string
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <returns>An <see cref="HtmlString"/> containing the rendered HTML</returns>
    public static HtmlString ToHtml(this IEnumerable<HtmlNode> reader)
    {
      using (var sw = new StringWriter())
      {
        ToHtml(reader, sw, new HtmlWriterSettings());
        return new HtmlString(sw.ToString());
      }
    }

    /// <summary>
    /// Render parsed HTML to a string
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="settings">Settings controlling how the HTML is rendered</param>
    /// <returns>An <see cref="HtmlString"/> containing the rendered HTML</returns>
    public static HtmlString ToHtml(this IEnumerable<HtmlNode> reader, HtmlWriterSettings settings)
    {
      using (var sw = new StringWriter())
      {
        ToHtml(reader, sw, settings);
        return new HtmlString(sw.ToString());
      }
    }

    /// <summary>
    /// Render parsed HTML to a <see cref="TextWriter"/>
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="writer"><see cref="TextWriter"/> to which the HTML is written</param>
    /// <param name="settings">Settings controlling how the HTML is rendered</param>
    public static void ToHtml(this IEnumerable<HtmlNode> reader, TextWriter writer, HtmlWriterSettings settings)
    {
      using (var w = new HtmlTextWriter(writer, settings))
      {
        ToHtml(reader, w);
        w.Flush();
      }
    }

    /// <summary>
    /// Convert parsed HTML to markdown
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <returns>A markdown representation of the HTML</returns>
    public static string ToMarkdown(this IEnumerable<HtmlNode> reader)
    {
      using (var sw = new StringWriter())
      {
        ToMarkdown(reader, sw, new MarkdownWriterSettings());
        return sw.ToString();
      }
    }

    /// <summary>
    /// Convert parsed HTML to markdown
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="settings">Settings controlling how the markdown is rendered</param>
    /// <returns>A markdown representation of the HTML</returns>
    public static string ToMarkdown(this IEnumerable<HtmlNode> reader, MarkdownWriterSettings settings)
    {
      using (var sw = new StringWriter())
      {
        ToMarkdown(reader, sw, settings);
        return sw.ToString();
      }
    }

    /// <summary>
    /// Convert parsed HTML to markdown
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="settings">Settings controlling how the markdown is rendered</param>
    /// <returns>A markdown representation of the HTML</returns>
    public static string ToMarkdown(TextSource html, MarkdownWriterSettings settings = null)
    {
      var sb = Pool.NewStringBuilder();
      sb.EnsureCapacity(html.Length);
      using (var sw = new StringWriter(sb))
      using (var reader = new HtmlReader(html, false))
      {
        reader.ToMarkdown(sw, settings);
        sw.Flush();
        return sb.ToPool();
      }
    }

    /// <summary>
    /// Convert parsed HTML to markdown
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="writer">Writer to which the markdown is written</param>
    /// <param name="settings">Settings controlling how the markdown is rendered</param>
    public static void ToMarkdown(this IEnumerable<HtmlNode> reader, TextWriter writer, MarkdownWriterSettings settings = null)
    {
      using (var w = new MarkdownWriter(writer, settings))
      {
        ToHtml(reader, w);
        w.Flush();
      }
    }


    /// <summary>
    /// Convert parsed HTML to plain text
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="settings">Settings controlling how the plain text is rendered</param>
    /// <returns>A plain text representation of the HTML</returns>
    public static string ToPlainText(this IEnumerable<HtmlNode> reader, TextWriterSettings settings = null)
    {
      using (var sw = new StringWriter())
      {
        ToPlainText(reader, sw, settings);
        return sw.ToString();
      }
    }

    /// <summary>
    /// Convert parsed HTML to plain text
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="settings">Settings controlling how the plain text is rendered</param>
    /// <returns>A plain text representation of the HTML</returns>
    public static string ToPlainText(TextSource html, TextWriterSettings settings = null)
    {
      var sb = Pool.NewStringBuilder();
      sb.EnsureCapacity(html.Length);
      using (var sw = new StringWriter(sb))
      using (var reader = new HtmlReader(html, false))
      {
        reader.ToPlainText(sw, settings);
        sw.Flush();
        return sb.ToPool();
      }
    }

    /// <summary>
    /// Convert parsed HTML to plain text
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="writer">Writer to which the plain text is written</param>
    /// <param name="settings">Settings controlling how the plain text is rendered</param>
    public static void ToPlainText(this IEnumerable<HtmlNode> reader, TextWriter writer, TextWriterSettings settings = null)
    {
      using (var w = new PlainTextWriter(writer, settings))
      {
        ToHtml(reader, w);
        w.Flush();
      }
    }

    /// <summary>
    /// Render parsed HTML to an <see cref="XmlWriter"/>
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="writer"><see cref="XmlWriter"/> to which the HTML is written</param>
    public static void ToHtml(this IEnumerable<HtmlNode> reader, XmlWriter writer)
    {
      HtmlStartTag tag;
      var htmlWriter = writer as HtmlTextWriter;

      foreach (var token in reader)
      {
        switch (token.Type)
        {
          case HtmlTokenType.Text:
            writer.WriteString(token.Value);
            break;
          case HtmlTokenType.Comment:
            if (htmlWriter == null)
              writer.WriteComment(token.Value);
            else
              htmlWriter.WriteComment(token.Value, ((HtmlComment)token).DownlevelRevealedConditional);
            break;
          case HtmlTokenType.Doctype:
            var docType = (HtmlDoctype)token;
            writer.WriteDocType(token.Value, docType.PublicIdentifier, docType.SystemIdentifier, null);
            break;
          case HtmlTokenType.StartTag:
            tag = (HtmlStartTag)token;
            writer.WriteStartElement(tag.Value);
            foreach (var attr in tag.Attributes)
            {
              if (attr.Key != null
                && attr.Key[0] != '"'
                && attr.Key[0] != '\''
                && attr.Key[0] != '<'
                && attr.Key[0] != '=')
                writer.WriteAttributeString(attr.Key, attr.Value);
            }
            if (HtmlTextWriter.VoidElements.Contains(tag.Value)
              || tag.IsSelfClosing)
            {
              if (htmlWriter == null)
                writer.WriteEndElement();
              else
                htmlWriter.WriteEndElement(token.Value);
            }
            break;
          case HtmlTokenType.EndTag:
            if (htmlWriter == null)
              writer.WriteEndElement();
            else
              htmlWriter.WriteEndElement(token.Value);
            break;
        }
      }
    }
  }
}
