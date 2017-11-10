using BracketPipe.TextileStates;
using System.Collections.Generic;

namespace BracketPipe
{
  public static class Textile
  {
    public static IEnumerable<HtmlNode> ToHtmlNodes(TextSource source, TextileParseSettings settings)
    {
      var parse = new ParseState(source, settings);
      var output = new ParseOutput();
      var state = new ParagraphState();
      state.TryParse(parse, output);
      return output.GetNodes();
    }

    public static HtmlString ToHtml(TextSource source)
    {
      return ToHtmlNodes(source, new TextileParseSettings()).ToHtml();
    }
  }
}
