namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Text;

  /// <summary>
  /// Common methods and variables of all tokenizers.
  /// </summary>
  public sealed class BaseTokenizer : IBaseTokenizer
  {
    #region Fields

    readonly Stack<UInt16> _columns;
    readonly TextSource _source;

    UInt16 _column;
    UInt16 _row;
    Char _currentChar;

    #endregion

    #region ctor

    public BaseTokenizer(TextSource source)
    {
      StringBuffer = Pool.NewStringBuilder();
      _columns = new Stack<UInt16>(128);
      _source = source;
      _currentChar = Symbols.Null;
      _column = 0;
      _row = 1;
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
      get { return _row; }
    }

    public UInt16 Column
    {
      get { return _column; }
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
      return new TextPosition(_row, _column, Position);
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
      if (_currentChar == Symbols.LineFeed)
      {
        _columns.Push(_column);
        _column = 1;
        _row++;
      }
      else
      {
        _column++;
      }

      _currentChar = NormalizeForward(_source.ReadCharacter());
    }

    void BackUnsafe()
    {
      _source.Index -= 1;

      if (_source.Index == 0)
      {
        _column = 0;
        _currentChar = Symbols.Null;
        return;
      }

      var c = NormalizeBackward(_source[_source.Index - 1]);

      if (c == Symbols.LineFeed)
      {
        _column = _columns.Count != 0 ? _columns.Pop() : (UInt16)1;
        _row--;
        _currentChar = c;
      }
      else if (c != Symbols.Null)
      {
        _currentChar = c;
        _column--;
      }
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
