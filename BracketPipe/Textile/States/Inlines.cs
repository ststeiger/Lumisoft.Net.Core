namespace BracketPipe.TextileStates
{
  public static class Inlines
  {
    public static BaseState StrongImportanceState { get; } = new SingleCharInlineState('*', "strong", false);
    public static BaseState StressEmphasisState { get; } = new SingleCharInlineState('_', "em", false);
    public static BaseState InsertionState { get; } = new SingleCharInlineState('+', "ins", true);
    public static BaseState DeletionState { get; } = new SingleCharInlineState('-', "del", true);
    public static BaseState SubscriptState { get; } = new SingleCharInlineState('~', "sub", true);
    public static BaseState SuperscriptState { get; } = new SingleCharInlineState('^', "sup", true);
    public static BaseState StylisticOffsetState { get; } = new DoubleCharInlineState('*', "b");
    public static BaseState AlternateVoiceState { get; } = new DoubleCharInlineState('_', "i");
    public static BaseState CitationState { get; } = new DoubleCharInlineState('?', "cite");

    public static BaseState[] All { get; } = new[]
    {
      StrongImportanceState,
      StressEmphasisState,
      InsertionState,
      DeletionState,
      SubscriptState,
      SuperscriptState,
      StylisticOffsetState,
      AlternateVoiceState,
      CitationState,
    };
  }
}
