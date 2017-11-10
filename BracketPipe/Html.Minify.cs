using BracketPipe.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BracketPipe
{
  /// <summary>
  /// Extensions and utility methods for handling HTML
  /// </summary>
  public static partial class Html
  {
    /// <summary>
    /// Compress HTML by removing unnecessary whitespace and comments
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="settings">Settings to control how the HTML is compressed</param>
    /// <returns>An <see cref="HtmlString"/> containing the compressed HTML</returns>
    public static HtmlString Minify(TextSource html, HtmlMinifySettings settings = null)
    {
      var sb = Pool.NewStringBuilder();
      sb.EnsureCapacity(html.Length);
      using (var sw = new StringWriter(sb))
      using (var reader = new HtmlReader(html, false))
      {
        reader.Minify(settings).ToHtml(sw, new HtmlWriterSettings());
        sw.Flush();
        return new HtmlString(sb.ToPool());
      }
    }

    /// <summary>
    /// Compress HTML by removing unnecessary whitespace and comments
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="writer">The writer where the HTML output is written. For best results, use a <see cref="HtmlTextWriter"/></param>
    /// <param name="settings">Settings to control how the HTML is compressed</param>
    public static void Minify(TextSource html, XmlWriter writer, HtmlMinifySettings settings = null)
    {
      using (var reader = new HtmlReader(html, false))
      {
        reader.Minify(settings).ToHtml(writer);
      }
    }

    private enum MinifyState : byte
    {
      Compressed,
      LastCharWasSpace,
      SpaceNeeded,
      InlineStartAfterSpace,
    }

    private enum ContainingTag : byte
    {
      None,
      WhitespacePreserve,
      Script
    }

    /// <summary>
    /// Compress HTML by removing unnecessary whitespace and comments
    /// </summary>
    /// <param name="reader">Stream of <see cref="HtmlNode"/> to minify</param>
    /// <param name="settings">Settings to control how the HTML is compressed</param>
    /// <returns>Stream of minified <see cref="HtmlNode"/></returns>
    public static IEnumerable<HtmlNode> Minify(this IEnumerable<HtmlNode> reader, HtmlMinifySettings settings = null)
    {
      settings = settings ?? HtmlMinifySettings.ReadOnlyDefault;
      var state = MinifyState.LastCharWasSpace;
      var tagState = ContainingTag.None;
      int trimStart;
      int trimEnd;
      StringBuilder builder = null;
      var jsMin = new JSMin();

      foreach (var node in reader)
      {
        if (node.Type == HtmlTokenType.Comment)
        {
          // Ignore comments, unless they are conditional
          if (node.Value.StartsWith("[if") || node.Value.EndsWith("endif]"))
            yield return node;
        }
        else if (node.Type == HtmlTokenType.Text)
        {
          if (node.Value == "" || node.Value == null)
          {
            // do nothing
          }
          else if (tagState == ContainingTag.WhitespacePreserve)
          {
            yield return node;
            state = MinifyState.LastCharWasSpace;
          }
          else if (tagState == ContainingTag.Script)
          {
            if (builder == null)
              builder = Pool.NewStringBuilder();
            builder.Append(node.Value);
          }
          else
          {
            TrimIndices(node.Value, out trimStart, out trimEnd);
            if (trimEnd < 0)
            {
              // Do nothing for an empty or null string
            }
            else if (trimEnd < trimStart)
            {
              if (state == MinifyState.Compressed)
                state = MinifyState.SpaceNeeded;
            }
            else
            {
              if (state == MinifyState.SpaceNeeded && trimStart == 0)
              {
                yield return new HtmlText(node.Position, " ");
                state = MinifyState.LastCharWasSpace;
              }

              if (state == MinifyState.LastCharWasSpace
                || state == MinifyState.InlineStartAfterSpace
                || trimStart == 0)
              {
                yield return new HtmlText(node.Position, GetCompressedString(node.Value, trimStart, trimEnd));
              }
              else
              {
                yield return new HtmlText(node.Position, GetCompressedString(node.Value, trimStart - 1, trimEnd));
              }

              if (trimEnd < node.Value.Length - 1)
                state = MinifyState.SpaceNeeded;
              else
                state = MinifyState.Compressed;
            }
          }
        }
        else
        {
          if (state == MinifyState.SpaceNeeded)
          {
            if (node.Type == HtmlTokenType.EndTag && !settings.InlineElements.Contains(node.Value))
            {
              state = MinifyState.LastCharWasSpace;
            }
            else
            {
              yield return new HtmlText(node.Position, " ");
              if (settings.PreserveSurroundingSpaceTags.Contains(node.Value))
                state = MinifyState.Compressed;
              else
                state = MinifyState.LastCharWasSpace;
            }
          }

          if (node.Type == HtmlTokenType.EndTag && node.Value == "script" && builder != null)
          {
            yield return new HtmlText(node.Position, Js.Minify(new TextSource(builder)));
            builder.ToPool();
            builder = null;
          }

          var tag = node as HtmlStartTag;
          if (tag != null)
          {
            if (tag.Attributes.Any(a => a.Key == "style" || a.Key == "class"))
            {
              var newTag = new HtmlStartTag(tag.Position, tag.Value);
              foreach (var attr in tag.Attributes)
              {
                if (attr.Key == "style")
                  newTag.Add(attr.Key, TrimStyleString(attr.Value));
                else if (attr.Key == "class")
                  newTag.Add(attr.Key, GetCompressedString(attr.Value));
                else
                  newTag.Attributes.Add(attr);
              }
              newTag.IsSelfClosing = tag.IsSelfClosing;
              yield return newTag;
            }
            else
            {
              yield return node;
            }

            if (state == MinifyState.LastCharWasSpace
              && settings.InlineElements.Contains(node.Value))
            {
              if (HtmlTextWriter.VoidElements.Contains(node.Value))
                state = MinifyState.Compressed;
              else
                state = MinifyState.InlineStartAfterSpace;
            }
          }
          else
          {
            yield return node;

            if (state == MinifyState.InlineStartAfterSpace)
              state = MinifyState.Compressed;
          }


          if (node.Type == HtmlTokenType.StartTag && settings.PreserveInnerSpaceTags.Contains(node.Value))
            tagState = ContainingTag.WhitespacePreserve;
          else if (node.Type == HtmlTokenType.StartTag && node.Value == "script")
            tagState = ContainingTag.Script;
          else if (node.Type == HtmlTokenType.EndTag &&
            (settings.PreserveInnerSpaceTags.Contains(node.Value) || node.Value == "script"))
            tagState = ContainingTag.None;
        }
      }
    }

    private static void TrimIndices(string value, out int start, out int end)
    {
      start = 0;
      end = -1;
      if (value == null)
        return;

      while (start < value.Length && value[start].IsSpaceCharacter()) start++;

      end = value.Length - 1;
      while (end >= start && value[end].IsSpaceCharacter()) end--;
    }
    private static string TrimStyleString(string value)
    {
      var start = 0;
      var end = -1;
      if (value == null || value == "")
        return value;

      while (start < value.Length && value[start].IsSpaceCharacter()) start++;

      end = value.Length - 1;
      while (end >= start && (value[end].IsSpaceCharacter() || value[end] == ';')) end--;

      if (start == 0 && end == value.Length - 1)
        return value;
      else
        return value.Substring(start, end - start + 1);
    }
    private static string GetCompressedString(string value)
    {
      int start;
      int end;
      TrimIndices(value, out start, out end);
      return GetCompressedString(value, start, end);
    }
    private static string GetCompressedString(string value, int start, int end)
    {
      if (end > start)
      {
        char[] buffer = null;
        int b = -1;

        for (var i = start + 1; i <= end; i++)
        {
          if (value[i].IsSpaceCharacter()
            && value[i - 1].IsSpaceCharacter())
          {
            if (buffer == null)
            {
              buffer = new char[value.Length];
              for (var j = start; j < i; j++)
                buffer[j - start] = value[j];
              b = i - start;
            }
          }
          else if (buffer != null)
          {
            buffer[b++] = value[i];
          }
        }

        if (buffer != null)
          return new string(buffer, 0, b);
      }
      if (start == 0 && end == value.Length - 1)
        return value;
      else if (end >= start)
        return value.Substring(start, end - start + 1);
      return string.Empty;
    }
  }
}
