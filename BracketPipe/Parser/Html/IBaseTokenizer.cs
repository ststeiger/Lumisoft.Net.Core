using System;
using System.Text;

namespace BracketPipe
{
  public interface IBaseTokenizer : IDisposable
  {
    ushort Column { get; }
    char CurrentChar { get; }
    int InsertionPoint { get; set; }
    ushort Line { get; }
    int Position { get; }
    TextSource Source { get; }
    StringBuilder StringBuffer { get; }

    char Advance();
    void Advance(int n);
    char Back();
    void Back(int n);
    bool ContinuesWithInsensitive(string s);
    bool ContinuesWithSensitive(string s);
    string FlushBuffer();
    TextPosition GetCurrentPosition();
    string PeekString(int length);
    char SkipSpaces();
  }
}
