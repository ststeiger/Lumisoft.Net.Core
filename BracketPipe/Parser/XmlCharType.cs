namespace BracketPipe
{
  internal static class XmlCharType
  {
    internal static int CombineSurrogateChar(int lowChar, int highChar)
    {
      return lowChar - 56320 | (highChar - 55296 << 10) + 65536;
    }

    internal static bool IsHighSurrogate(int ch)
    {
      return XmlCharType.InRange(ch, 55296, 56319);
    }

    internal static bool IsLowSurrogate(int ch)
    {
      return XmlCharType.InRange(ch, 56320, 57343);
    }

    internal static bool IsSurrogate(int ch)
    {
      return XmlCharType.InRange(ch, 55296, 57343);
    }

    private static bool InRange(int value, int start, int end)
    {
      return value >= start && value <= end;
    }
  }
}
