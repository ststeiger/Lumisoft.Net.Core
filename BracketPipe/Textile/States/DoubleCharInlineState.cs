namespace BracketPipe.TextileStates
{
  internal class DoubleCharInlineState : BaseState, IInlineState
  {
    private readonly char _marker;
    private readonly string _tag;

    public DoubleCharInlineState(char marker, string tag)
    {
      _marker = marker;
      _tag = tag;
    }

    internal override bool TryParse(ParseState state, ParseOutput output)
    {
      if (state.Peek() != _marker)
        return false;

      var start = state.Index;
      state.ReadCharacter(); // consume _marker
      var ch = state.ReadCharacter();
      if (ch != _marker)
      {
        state.Index = start;
        return false;
      }
      ch = state.ReadCharacter();
      if (ch == _marker)
      {
        state.Index = start;
        return false;
      }

      state.StartRun(start + 2);
      while (ch != Symbols.EndOfFile)
      {
        if (ch == _marker)
        {
          var next = state.ReadCharacter();
          ch = next == _marker ? Symbols.EndOfFile : next;
        }
        else
        {
          ch = state.ReadInlineOrCharacter(output);
        }
      }

      output.Add(new HtmlStartTag(_tag));
      output.Add(state.EndRun(-2));
      output.Add(new HtmlEndTag(_tag));
      return true;
    }
  }
}
