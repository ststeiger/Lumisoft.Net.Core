namespace BracketPipe
{
  //using BracketPipe.Dom;
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;
#if !NET40
  using System.Runtime.CompilerServices;
#endif

  /// <summary>
  /// Extensions to be used exclusively by the parser or the tokenizer.
  /// </summary>
  static class HtmlParserExtensions
  {
    public static Int32 GetCode(this HtmlParseError code)
    {
      return (Int32)code;
    }
  }
}
