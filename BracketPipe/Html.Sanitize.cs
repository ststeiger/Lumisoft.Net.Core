using BracketPipe.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BracketPipe
{
  public static partial class Html
  {
    private static readonly Regex _emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

    /// <summary>
    /// Sanitizes the specified HTML, removing scripts, styles, and tags 
    /// which might pose a security concern
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="settings">Settings controlling what CSS and HTML is permitted in the result</param>
    /// <returns>An <see cref="HtmlString"/> containing only the permitted elements</returns>
    /// <remarks>
    /// The goal of sanitization is to prevent XSS patterns
    /// described on <a href="https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet">XSS Filter Evasion Cheat Sheet</a>
    /// </remarks>
    public static HtmlString Sanitize(TextSource html, HtmlSanitizeSettings settings = null)
    {
      var sb = new StringBuilder(html.Length);
      using (var reader = new HtmlReader(html, false))
      using (var sw = new StringWriter(sb))
      {
        reader.Sanitize(settings).ToHtml(sw, new HtmlWriterSettings());
        return new HtmlString(sw.ToString());
      }
    }

    /// <summary>
    /// Sanitizes the specified HTML, removing scripts, styles, and tags 
    /// which might pose a security concern
    /// </summary>
    /// <param name="html">The HTML content to minify. A <see cref="string"/> or <see cref="Stream"/> can also be used.</param>
    /// <param name="writer">Writer to which the sanitized HTML is written</param>
    /// <param name="settings">Settings controlling what CSS and HTML is permitted in the result</param>
    /// <remarks>
    /// The goal of sanitization is to prevent XSS patterns
    /// described on <a href="https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet">XSS Filter Evasion Cheat Sheet</a>
    /// </remarks>
    public static void Sanitize(TextSource html, XmlWriter writer, HtmlSanitizeSettings settings = null)
    {
      using (var reader = new HtmlReader(html, false))
      {
        reader.Sanitize(settings).ToHtml(writer);
      }
    }

    /// <summary>
    /// Sanitizes the specified HTML, removing scripts, styles, and tags 
    /// which might pose a security concern
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <returns>A stream of sanitized <see cref="HtmlNode"/></returns>
    /// <remarks>
    /// The goal of sanitization is to prevent XSS patterns
    /// described on <a href="https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet">XSS Filter Evasion Cheat Sheet</a>
    /// </remarks>
    public static IEnumerable<HtmlNode> Sanitize(this IEnumerable<HtmlNode> reader)
    {
      return Sanitize(reader, HtmlSanitizeSettings.ReadOnlyDefault);
    }

    /// <summary>
    /// Sanitizes the specified HTML, removing scripts, styles, and tags 
    /// which might pose a security concern
    /// </summary>
    /// <param name="reader">A stream of <see cref="HtmlNode"/></param>
    /// <param name="settings">Settings controlling what CSS and HTML is permitted in the result</param>
    /// <returns>A stream of sanitized <see cref="HtmlNode"/></returns>
    /// <remarks>
    /// The goal of sanitization is to prevent XSS patterns
    /// described on <a href="https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet">XSS Filter Evasion Cheat Sheet</a>
    /// </remarks>
    public static IEnumerable<HtmlNode> Sanitize(this IEnumerable<HtmlNode> reader, HtmlSanitizeSettings settings)
    {
      var removeDepth = -1;
      var inStyle = false;
      settings = settings ?? HtmlSanitizeSettings.ReadOnlyDefault;

      foreach (var origToken in reader)
      {
        var token = origToken;
        if (token.Type == HtmlTokenType.StartTag && _emailRegex.IsMatch(token.Value))
          token = new HtmlText(token.Position, "<" + token.Value + ">");

        switch (token.Type)
        {
          case HtmlTokenType.Text:
            if (removeDepth < 0)
            {
              if (inStyle)
                yield return new HtmlText(token.Position, SanitizeCss(token.Value, settings, true));
              else
                yield return token;
            }
            break;
          case HtmlTokenType.Comment:
            // No need to risk weird comments that might be interpreted as content (e.g. in IE)
            break;
          case HtmlTokenType.Doctype:
            // Doctypes should not appear in snippets
            break;
          case HtmlTokenType.StartTag:
            var tag = (HtmlStartTag)token;
            if (removeDepth < 0)
            {
              if (settings.AllowedTags.Contains(token.Value))
              {
                if (token.Value == "style")
                  inStyle = true;

                var allowed = AllowedAttributes(tag, settings).ToArray();

                if (tag.Value == "img" && !allowed.Any(k => k.Key == "src"))
                {
                  if (!HtmlTextWriter.VoidElements.Contains(tag.Value) && !tag.IsSelfClosing)
                    removeDepth = 0;
                }
                else
                {
                  var newTag = new HtmlStartTag(tag.Position, tag.Value);
                  newTag.IsSelfClosing = tag.IsSelfClosing;
                  foreach (var attr in allowed)
                  {
                    newTag.Attributes.Add(attr);
                  }
                  yield return newTag;
                }
              }
              else if (!HtmlTextWriter.VoidElements.Contains(tag.Value) && !tag.IsSelfClosing)
              {
                removeDepth = 0;
              }
            }
            else
            {
              if (!HtmlTextWriter.VoidElements.Contains(tag.Value) && !tag.IsSelfClosing)
                removeDepth++;
            }
            break;
          case HtmlTokenType.EndTag:
            if (removeDepth < 0 && settings.AllowedTags.Contains(token.Value))
              yield return token;
            else
              removeDepth--;

            if (token.Value == "style")
              inStyle = false;
            break;
        }
      }
    }

    private static bool IsValidTagName(string name)
    {
      if (name == null)
        return false;
      for (var i = 0; i < name.Length; i++)
      {
        if (!char.IsLetterOrDigit(name[i])
          && name[i] != ':'
          && name[i] != '_'
          && name[i] != '-'
          && name[i] != '.'
          && name[i] > ' '
          && name[i] < 127)
          return false;
      }
      return true;
    }

    private static IEnumerable<KeyValuePair<string, string>> AllowedAttributes(HtmlStartTag tag, HtmlSanitizeSettings settings)
    {
      for (var i = 0; i < tag.Attributes.Count; i++)
      {
        if (!settings.AllowedAttributes.Contains(tag.Attributes[i].Key))
        {
          // Do nothing
        }
        else if (string.Equals(tag.Attributes[i].Key, "style", StringComparison.OrdinalIgnoreCase))
        {
          var style = SanitizeCss(tag.Attributes[i].Value, settings, false);
          if (!style.IsNullOrWhiteSpace())
            yield return new KeyValuePair<string, string>(tag.Attributes[i].Key, style);
        }
        else if (settings.UriAttributes.Contains(tag.Attributes[i].Key))
        {
          var url = SanitizeUrl(tag.Attributes[i].Value, settings);
          if (url != null)
            yield return new KeyValuePair<string, string>(tag.Attributes[i].Key, url);
        }
        else if (!tag.Attributes[i].Value.StartsWith("&{"))
        {
          yield return tag.Attributes[i];
        }
      }
    }

    private static string SanitizeCss(string css, HtmlSanitizeSettings settings, bool styleTag)
    {
      using (var sw = new StringWriter())
      using (var writer = new CssWriter(sw))
      {
        foreach (var token in new CssTokenizer(css).Normalize())
        {
          var prop = token as CssPropertyToken;
          var group = token as CssAtGroupToken;
          if (prop != null)
          {
            if (settings.AllowedCssProps.Contains(prop.Data))
            {
              var removeProp = false;
              foreach (var arg in prop)
              {
                if (arg.Type == CssTokenType.Function && !settings.AllowedCssFunctions.Contains(arg.Data))
                {
                  removeProp = true;
                }
                else if (arg.Type == CssTokenType.Url)
                {
                  var url = SanitizeUrl(arg.Data, settings);
                  if (url == null)
                    removeProp = true;
                }
                else if (arg.Data.IndexOf('<') >= 0 || arg.Data.IndexOf('>') >= 0)
                {
                  removeProp = true;
                }
              }

              if (!removeProp)
                writer.Write(token);
            }
          }
          else if (group != null)
          {
            if (settings.AllowedCssAtRules.Contains(group.Data))
              writer.Write(group);
          }
          else if (styleTag && (token.Type != CssTokenType.Function || settings.AllowedCssFunctions.Contains(token.Data)))
          {
            writer.Write(token);
          }
        }
        sw.Flush();
        return sw.ToString();
      }
    }

    /// <summary>
    /// Sanitizes a URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="baseUrl">The base URL relative URLs are resolved against (empty or null for no resolution).</param>
    /// <returns>The sanitized URL or null if no safe URL can be created.</returns>
    private static string SanitizeUrl(string url, HtmlSanitizeSettings settings)
    {
      var uri = GetSafeUri(url, settings);

      if (uri == null) return null;

      try
      {
        return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Tries to create a safe <see cref="Uri"/> object from a string.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The <see cref="Uri"/> object or null if no safe <see cref="Uri"/> can be created.</returns>
    private static Uri GetSafeUri(string url, HtmlSanitizeSettings settings)
    {
      Uri uri;
      if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri)
          || !uri.IsAbsoluteUri && !IsWellFormedRelativeUri(uri)
          || uri.IsAbsoluteUri && !settings.AllowedSchemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase))
        return null;
      if (!uri.IsAbsoluteUri && uri.ToString().IndexOf(':') > 0)
        return null;

      return uri;
    }

    private static readonly Uri _exampleUri = new Uri("http://www.example.com/");
    private static bool IsWellFormedRelativeUri(Uri uri)
    {
      Uri absoluteUri;
      return uri.OriginalString.IndexOf(':') < 0
        && Uri.TryCreate(_exampleUri, uri, out absoluteUri);
    }
  }
}
