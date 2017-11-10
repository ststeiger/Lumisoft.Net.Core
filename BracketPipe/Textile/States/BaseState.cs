namespace BracketPipe.TextileStates
{
  public abstract class BaseState
  {
    internal abstract bool TryParse(ParseState state, ParseOutput output);
  }
}
