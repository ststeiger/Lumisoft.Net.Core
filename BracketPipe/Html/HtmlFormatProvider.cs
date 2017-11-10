using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  internal class HtmlFormatProvider : IFormatProvider, ICustomFormatter
  {
    private static HtmlFormatProvider _instance = new HtmlFormatProvider();

    public static HtmlFormatProvider Instance { get { return _instance; } }

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
      var value = default(string);
      var raw = format == "!";
      if (raw)
        format = string.Empty;

      if (arg is IFormattable)
        value = ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
      else if (arg != null)
        value = arg.ToString();

      if (raw)
        return value;

      return Pool.NewStringBuilder().AppendHtmlEncoded(value).ToString();
    }

    public object GetFormat(Type formatType)
    {
      if (formatType == typeof(ICustomFormatter))
        return this;
      else
        return null;
    }
  }
}
