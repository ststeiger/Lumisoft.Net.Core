namespace BracketPipe.TextileStates
{
  internal class SingleCharInlineState : BaseState, IInlineState
  {
    private readonly char _marker;
    private readonly string _tag;
    private readonly bool _allowBracket;

    public SingleCharInlineState(char marker, string tag, bool allowBracket)
    {
      _marker = marker;
      _tag = tag;
      _allowBracket = allowBracket;
    }

    internal override bool TryParse(ParseState state, ParseOutput output)
    {
      var start = state.Index;
      var ch = state.ReadCharacter();
      var inBracket = false;

      if (_allowBracket && ch == '[')
      {
        inBracket = true;
        ch = state.ReadCharacter();
      }

      // Bail if current character is not the marker
      // -or- current character is marker and the previous character was 
      // neither a bracket or whitespace
      if (ch != _marker
        || (!inBracket && start > 0 && !char.IsWhiteSpace(state[start - 1])))
        return state.Reset(start);

      ch = state.ReadCharacter();
      if (ch == _marker)
        return state.Reset(start);
      state.StartRun(start + (inBracket ? 2 : 1));

      while (ch != Symbols.EndOfFile)
      {
        if (ch == _marker)
        {
          if (inBracket)
          {
            var next = state.ReadCharacter();
            ch = next == ']' ? Symbols.EndOfFile : next;
          }
          else
          {
            var next = state.ReadCharacter();
            if (char.IsWhiteSpace(ch))
            {
              ch = Symbols.EndOfFile;
              state.Index--;
            }
            else
            {
              ch = next;
            }
          }
        }
        else
        {
          ch = state.ReadInlineOrCharacter(output);
        }
      }

      output.Add(new HtmlStartTag(_tag));
      output.Add(state.EndRun(inBracket ? -2 : -1));
      output.Add(new HtmlEndTag(_tag));
      return true;
    }
  }
}
