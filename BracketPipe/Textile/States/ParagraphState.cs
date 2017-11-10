namespace BracketPipe.TextileStates
{
  internal class ParagraphState : BaseState
  {
    internal override bool TryParse(ParseState state, ParseOutput output)
    {
      output.Add(new HtmlStartTag("p"));
      state.StartRun();
      var ch = state.ReadInlineOrCharacter(output);
      while (ch != Symbols.EndOfFile)
      {
        ch = state.ReadInlineOrCharacter(output);
      }
      output.Add(state.EndRun());
      output.Add(new HtmlEndTag("p"));
      return true;
    }
  }
}
