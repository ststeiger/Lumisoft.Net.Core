using BracketPipe.TextileStates;
using System.Collections.Generic;

namespace BracketPipe
{
  public class TextileParseSettings
  {
    private List<BaseState> _inlines = new List<BaseState>(TextileStates.Inlines.All);

    public IEnumerable<BaseState> Inlines { get { return _inlines; } }
  }
}
