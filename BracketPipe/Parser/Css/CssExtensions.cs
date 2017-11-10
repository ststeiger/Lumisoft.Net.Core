using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public static class CssExtensions
  {
    public static string ToValue(this CssToken token)
    {
      var sb = Pool.NewStringBuilder();
      using (var sw = new StringWriter(sb))
      using (var w = new CssWriter(sw))
      {
        w.Write(token);
        sw.Flush();
        return sb.ToPool();
      }
    }

    public static IEnumerable<CssToken> RemoveComments(this IEnumerable<CssToken> reader)
    {
      foreach (var token in reader)
      {
        if (token.Type != CssTokenType.Comment)
          yield return token;
      }
    }

    public static IEnumerable<CssToken> Normalize(this IEnumerable<CssToken> reader)
    {
      return reader.Normalize1().Normalize2().GetProperties();
    }

    /// <summary>
    /// Convert idents to functions and remove comments
    /// </summary>
    private static IEnumerable<CssToken> Normalize1(this IEnumerable<CssToken> reader)
    {
      using (var e = reader.GetEnumerator())
      {
        while (e.MoveNext())
        {
          var idx = (e.Current.ToValue() ?? "").IndexOf('(');
          if (e.Current.Type == CssTokenType.Ident && idx > 0)
          {
            using (var sw = new StringWriter())
            using (var writer = new CssWriter(sw))
            {
              writer.Write(e.Current);
              while (!e.Current.ToValue().EndsWith(")") && e.MoveNext())
              {
                writer.Write(e.Current);
              }

              sw.Flush();
              var tokens = new CssTokenizer(sw.ToString());
              foreach (var token in tokens)
              {
                yield return token;
              }
            }
          }
          else if (e.Current.Type != CssTokenType.Comment
            && e.Current.Type != CssTokenType.Cdc
            && e.Current.Type != CssTokenType.Cdo)
          {
            yield return e.Current;
          }
        }
      }
    }

    /// <summary>
    /// Combine consecutive ident tokens along with ident tokens followed by functions
    /// </summary>
    private static IEnumerable<CssToken> Normalize2(this IEnumerable<CssToken> reader)
    {
      using (var e = reader.GetEnumerator())
      {
        if (!e.MoveNext())
          yield break;

        var prev = e.Current;
        while (e.MoveNext())
        {
          if (e.Current.Type == CssTokenType.Ident
            && prev.Type == CssTokenType.Ident)
          {
            prev = new CssToken(CssTokenType.Ident, prev.Data + e.Current.Data, prev.Position);
          }
          else if (e.Current.Type == CssTokenType.Function
            && prev.Type == CssTokenType.Ident)
          {
            var currFunc = (CssFunctionToken)e.Current;
            var func = new CssFunctionToken(prev.Data + e.Current.Data, prev.Position);
            foreach (var arg in currFunc.ArgumentTokens)
              func.AddArgumentToken(arg);
            prev = func;
          }
          else
          {
            yield return prev;
            prev = e.Current;
          }
        }
        yield return prev;
      }
    }

    /// <summary>
    /// Create properties and @-rule groups
    /// </summary>
    private static IEnumerable<CssToken> GetProperties(this IEnumerable<CssToken> reader)
    {
      using (var e = reader.GetEnumerator())
      {
        if (!e.MoveNext())
          yield break;

        var prev = e.Current;
        if (e.Current.Type == CssTokenType.AtKeyword)
          prev = new CssAtGroupToken(e.Current.Data, e.Current.Position);
        else
          prev = e.Current;
        while (e.MoveNext())
        {
          if (prev.Type == CssTokenType.Ident
            && e.Current.Data == ":")
          {
            prev = new CssPropertyToken(prev.Data, prev.Position);
          }
          else if (prev.Type == CssTokenType.Property)
          {
            var prop = (CssPropertyToken)prev;
            if (prop.IsTerminated || e.Current.Data == "}" || e.Current.Data == ")")
            {
              yield return prev;
              prev = e.Current;
            }
            else if (e.Current.Data == ";")
            {
              prop.IsTerminated = true;
            }
            else
            {
              prop.AddArgumentToken(e.Current);
            }
          }
          else if (prev.Type == CssTokenType.AtGroup)
          {
            var group = (CssAtGroupToken)prev;
            if (group.IsTerminated || e.Current.Data == "{")
            {
              yield return prev;
              prev = e.Current;
            }
            else if (e.Current.Data == ";")
            {
              group.IsTerminated = true;
            }
            else
            {
              group.AddArgumentToken(e.Current);
            }
          }
          else
          {
            yield return prev;
            if (e.Current.Type == CssTokenType.AtKeyword)
              prev = new CssAtGroupToken(e.Current.Data, e.Current.Position);
            else
              prev = e.Current;

          }
        }
        yield return prev;
      }
    }
  }
}
