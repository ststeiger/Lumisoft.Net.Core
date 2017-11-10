namespace BracketPipe
{
  using BracketPipe.Css;
  //using BracketPipe.Dom.Css;
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Extensions to be used exclusively by the parser or the tokenizer.
  /// </summary>
  static class CssParserExtensions
  {
    /// <summary>
    /// Gets the corresponding token type for the function name.
    /// </summary>
    /// <param name="functionName">The name to match.</param>
    /// <returns>The token type for the name.</returns>
    public static CssTokenType GetTypeFromName(this String functionName)
    {
      if (string.Equals(functionName, FunctionNames.Url, StringComparison.OrdinalIgnoreCase)
        || string.Equals(functionName, FunctionNames.UrlPrefix, StringComparison.OrdinalIgnoreCase)
        || string.Equals(functionName, FunctionNames.Domain, StringComparison.OrdinalIgnoreCase))
        return CssTokenType.Url;
      return CssTokenType.Function;
      //var creator = default(Func<String, DocumentFunction>);
      //return functionTypes.TryGetValue(functionName, out creator) ? CssTokenType.Url : CssTokenType.Function;
    }

    /// <summary>
    /// Retrieves a number describing the error of a given error code.
    /// </summary>
    /// <param name="code">A specific error code.</param>
    /// <returns>The code of the error.</returns>
    public static Int32 GetCode(this CssParseError code)
    {
      return (Int32)code;
    }
  }
}
