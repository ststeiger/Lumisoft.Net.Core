namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Text;

  /// <summary>
  /// Common methods and variables of all tokenizers.
  /// </summary>
  public sealed class BaseTokenizerNoPosition : IBaseTokenizer
  {
    #region Fields

    readonly TextSource _source;

    Char _currentChar;

    #endregion

    #region ctor

    public BaseTokenizerNoPosition(TextSource source)
    {
      StringBuffer = Pool.NewStringBuilder();
      _source = source;
      _currentChar = Symbols.Null;
    }

    #endregion

    #region Properties

    public StringBuilder StringBuffer
    {
      get;
      private set;
    }

    public TextSource Source
    {
      get { return _source; }
    }

    public Int32 InsertionPoint
    {
      get { return _source.Index; }
      set
      {
        var delta = _source.Index - value;

        while (delta > 0)
        {
          BackUnsafe();
          delta--;
        }

        while (delta < 0)
        {
          AdvanceUnsafe();
          delta++;
        }
      }
    }

    public UInt16 Line
    {
      get { return 1; }
    }

    public UInt16 Column
    {
      get { return 1; }
    }

    public Int32 Position
    {
      get { return _source.Index; }
    }

    public Char CurrentChar
    {
      get { return _currentChar; }
    }

    #endregion

    #region Methods

    public String FlushBuffer()
    {
      var content = StringBuffer.ToString();
      StringBuffer.Clear();
      return content;
    }

    public void Dispose()
    {
      var isDisposed = StringBuffer == null;

      if (!isDisposed)
      {
        var disposable = _source as IDisposable;
        disposable?.Dispose();
        StringBuffer.Clear().ToPool();
        StringBuffer = null;
      }
    }

    public TextPosition GetCurrentPosition()
    {
      return default(TextPosition);
    }

    public Boolean ContinuesWithInsensitive(String s)
    {
      var content = PeekString(s.Length);
      return content.Length == s.Length && content.Isi(s);
    }

    public Boolean ContinuesWithSensitive(String s)
    {
      var content = PeekString(s.Length);
      return content.Length == s.Length && content.Isi(s);
    }

    public String PeekString(Int32 length)
    {
      var mark = _source.Index;
      _source.Index--;
      var content = _source.ReadCharacters(length);
      _source.Index = mark;
      return content;
    }

    public Char SkipSpaces()
    {
      var c = Advance();

      while (c.IsSpaceCharacter())
      {
        c = Advance();
      }

      return c;
    }

    #endregion

    #region Source Management

    public Char Advance()
    {
      if (_currentChar != Symbols.EndOfFile)
        AdvanceUnsafe();
      return _currentChar;
    }

    public void Advance(Int32 n)
    {
      while (n-- > 0 && _currentChar != Symbols.EndOfFile)
      {
        AdvanceUnsafe();
      }
    }

    public Char Back()
    {
      if (InsertionPoint > 0)
        BackUnsafe();
      return _currentChar;
    }

    public void Back(Int32 n)
    {
      while (n-- > 0 && InsertionPoint > 0)
      {
        BackUnsafe();
      }
    }

    #endregion

    #region Helpers

    void AdvanceUnsafe()
    {
      _currentChar = NormalizeForward(_source.ReadCharacter());
    }

    void BackUnsafe()
    {
      _source.Index -= 1;

      if (_source.Index == 0)
      {
        _currentChar = Symbols.Null;
        return;
      }

      var c = NormalizeBackward(_source[_source.Index - 1]);

      if (c != Symbols.Null)
        _currentChar = c;
    }

    Char NormalizeForward(Char p)
    {
      if (p != Symbols.CarriageReturn)
      {
        return p;
      }
      else if (_source.ReadCharacter() != Symbols.LineFeed)
      {
        _source.Index--;
      }

      return Symbols.LineFeed;
    }

    Char NormalizeBackward(Char p)
    {
      if (p != Symbols.CarriageReturn)
      {
        return p;
      }
      else if (_source.Index < _source.Length && _source[_source.Index] == Symbols.LineFeed)
      {
        BackUnsafe();
        return Symbols.Null;
      }

      return Symbols.LineFeed;
    }

    #endregion
  }
}
