namespace BracketPipe
{
  using BracketPipe.Css;
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Collections;

  /// <summary>
  /// The CSS tokenizer.
  /// See http://dev.w3.org/csswg/css-syntax/#tokenization for more details.
  /// </summary>
  public class CssTokenizer
    : IEnumerator<CssToken>
    , IEnumerable<CssToken>
    , IDisposable
  {
    #region Fields

    private BaseTokenizer _base;
    private Boolean _valueMode;
    private TextPosition _position;
    private CssToken _current;

    #endregion

    #region Events

    /// <summary>
    /// Fired in case of a parse error.
    /// </summary>
    public event EventHandler<CssErrorEvent> Error;

    #endregion

    #region ctor

    /// <summary>
    /// CSS Tokenization
    /// </summary>
    /// <param name="source">The source code manager.</param>
    public CssTokenizer(TextSource source)
    {
      _base = new BaseTokenizer(source);
      _valueMode = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets if we are currently in value mode.
    /// </summary>
    public Boolean IsInValue
    {
      get { return _valueMode; }
      set { _valueMode = value; }
    }

    public CssToken Current { get { return _current; } }
    object IEnumerator.Current { get { return _current; } }

    #endregion

    #region Methods

    /// <summary>
    /// Reads the next token.
    /// </summary>
    /// <returns><c>true</c> if the reader has reached the end; <c>false</c> otherwise</returns>
    public bool Read()
    {
      return NextToken().Type != CssTokenType.EndOfFile;
    }

    /// <summary>
    /// Gets the next available token.
    /// </summary>
    /// <returns>The next available token.</returns>
    public CssToken NextToken()
    {
      var current = _base.Advance();
      _position = _base.GetCurrentPosition();
      _current = Data(current);
      return _current;
    }

    internal void RaiseErrorOccurred(CssParseError error, TextPosition position)
    {
      var handler = Error;

      if (handler != null)
      {
        var errorEvent = new CssErrorEvent(error, position);
        handler.Invoke(this, errorEvent);
      }
    }

    #endregion

    #region States

    /// <summary>
    /// 4.4.1. Data state
    /// </summary>
    CssToken Data(Char current)
    {
      _position = _base.GetCurrentPosition();

      switch (current)
      {
        case Symbols.FormFeed:
        case Symbols.LineFeed:
        case Symbols.CarriageReturn:
        case Symbols.Tab:
        case Symbols.Space:
          return NewWhitespace(current);

        case Symbols.DoubleQuote:
          return StringDQ();

        case Symbols.Num:
          return _valueMode ? ColorLiteral() : HashStart();

        case Symbols.Dollar:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.Ends);
          }

          return NewDelimiter(_base.Back());

        case Symbols.SingleQuote:
          return StringSQ();

        case Symbols.RoundBracketOpen:
          return NewOpenRound();

        case Symbols.RoundBracketClose:
          return NewCloseRound();

        case Symbols.Asterisk:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.InText);
          }

          return NewDelimiter(_base.Back());

        case Symbols.Plus:
          {
            var c1 = _base.Advance();

            if (c1 != Symbols.EndOfFile)
            {
              var c2 = _base.Advance();
              _base.Back(2);

              if (c1.IsDigit() || (c1 == Symbols.Dot && c2.IsDigit()))
              {
                return NumberStart(current);
              }
            }
            else
            {
              _base.Back();
            }

            return NewDelimiter(current);
          }

        case Symbols.Comma:
          return NewComma();

        case Symbols.Dot:
          {
            var c = _base.Advance();

            if (c.IsDigit())
            {
              return NumberStart(_base.Back());
            }

            return NewDelimiter(_base.Back());
          }

        case Symbols.Minus:
          {
            var c1 = _base.Advance();

            if (c1 != Symbols.EndOfFile)
            {
              var c2 = _base.Advance();
              _base.Back(2);

              if (c1.IsDigit() || (c1 == Symbols.Dot && c2.IsDigit()))
              {
                return NumberStart(current);
              }
              else if (c1.IsNameStart())
              {
                return IdentStart(current);
              }
              else if (c1 == Symbols.ReverseSolidus && !c2.IsLineBreak() && c2 != Symbols.EndOfFile)
              {
                return IdentStart(current);
              }
              else if (c1 == Symbols.Minus && c2 == Symbols.GreaterThan)
              {
                _base.Advance(2);
                return NewCloseComment();
              }
            }
            else
            {
              _base.Back();
            }

            return NewDelimiter(current);
          }

        case Symbols.Solidus:
          current = _base.Advance();

          if (current == Symbols.Asterisk)
          {
            return Comment();
          }

          return NewDelimiter(_base.Back());

        case Symbols.ReverseSolidus:
          current = _base.Advance();

          if (current.IsLineBreak())
          {
            RaiseErrorOccurred(CssParseError.LineBreakUnexpected);
            return NewDelimiter(_base.Back());
          }
          else if (current == Symbols.EndOfFile)
          {
            RaiseErrorOccurred(CssParseError.EOF);
            return NewDelimiter(_base.Back());
          }

          return IdentStart(_base.Back());
        case Symbols.Colon:
          return NewColon();

        case Symbols.Semicolon:
          return NewSemicolon();

        case Symbols.LessThan:
          current = _base.Advance();

          if (current == Symbols.ExclamationMark)
          {
            current = _base.Advance();

            if (current == Symbols.Minus)
            {
              current = _base.Advance();

              if (current == Symbols.Minus)
              {
                return NewOpenComment();
              }

              current = _base.Back();
            }

            current = _base.Back();
          }

          return NewDelimiter(_base.Back());

        case Symbols.At:
          return AtKeywordStart();

        case Symbols.SquareBracketOpen:
          return NewOpenSquare();

        case Symbols.SquareBracketClose:
          return NewCloseSquare();

        case Symbols.Accent:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.Begins);
          }

          return NewDelimiter(_base.Back());

        case Symbols.CurlyBracketOpen:
          return NewOpenCurly();

        case Symbols.CurlyBracketClose:
          return NewCloseCurly();

        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          return NumberStart(current);

        case 'U':
        case 'u':
          current = _base.Advance();

          if (current == Symbols.Plus)
          {
            current = _base.Advance();

            if (current.IsHex() || current == Symbols.QuestionMark)
            {
              return UnicodeRange(current);
            }

            current = _base.Back();
          }

          return IdentStart(_base.Back());

        case Symbols.Pipe:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.InToken);
          }
          else if (current == Symbols.Pipe)
          {
            return NewColumn();
          }

          return NewDelimiter(_base.Back());

        case Symbols.Tilde:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.InList);
          }

          return NewDelimiter(_base.Back());

        case Symbols.EndOfFile:
          return NewEof();

        case Symbols.ExclamationMark:
          current = _base.Advance();

          if (current == Symbols.Equality)
          {
            return NewMatch(CombinatorSymbols.Unlike);
          }

          return NewDelimiter(_base.Back());

        default:
          if (current.IsNameStart())
          {
            return IdentStart(current);
          }

          return NewDelimiter(current);
      }
    }

    /// <summary>
    /// 4.4.2. Double quoted string state
    /// </summary>
    CssToken StringDQ()
    {
      while (true)
      {
        var current = _base.Advance();

        switch (current)
        {
          case Symbols.DoubleQuote:
          case Symbols.EndOfFile:
            return NewString(_base.FlushBuffer(), Symbols.DoubleQuote);

          case Symbols.FormFeed:
          case Symbols.LineFeed:
            RaiseErrorOccurred(CssParseError.LineBreakUnexpected);
            _base.Back();
            return NewString(_base.FlushBuffer(), Symbols.DoubleQuote, bad: true);

          case Symbols.ReverseSolidus:
            current = _base.Advance();

            if (current.IsLineBreak())
            {
              _base.StringBuffer.AppendLine();
            }
            else if (current != Symbols.EndOfFile)
            {
              _base.StringBuffer.Append(ConsumeEscape(current));
            }
            else
            {
              RaiseErrorOccurred(CssParseError.EOF);
              _base.Back();
              return NewString(_base.FlushBuffer(), Symbols.DoubleQuote, bad: true);
            }

            break;

          default:
            _base.StringBuffer.Append(current);
            break;
        }
      }
    }

    /// <summary>
    /// 4.4.3. Single quoted string state
    /// </summary>
    CssToken StringSQ()
    {
      while (true)
      {
        var current = _base.Advance();

        switch (current)
        {
          case Symbols.SingleQuote:
          case Symbols.EndOfFile:
            return NewString(_base.FlushBuffer(), Symbols.SingleQuote);

          case Symbols.FormFeed:
          case Symbols.LineFeed:
            RaiseErrorOccurred(CssParseError.LineBreakUnexpected);
            _base.Back();
            return NewString(_base.FlushBuffer(), Symbols.SingleQuote, bad: true);

          case Symbols.ReverseSolidus:
            current = _base.Advance();

            if (current.IsLineBreak())
            {
              _base.StringBuffer.AppendLine();
            }
            else if (current != Symbols.EndOfFile)
            {
              _base.StringBuffer.Append(ConsumeEscape(current));
            }
            else
            {
              RaiseErrorOccurred(CssParseError.EOF);
              _base.Back();
              return NewString(_base.FlushBuffer(), Symbols.SingleQuote, bad: true);
            }

            break;

          default:
            _base.StringBuffer.Append(current);
            break;
        }
      }
    }

    /// <summary>
    /// Color literal state.
    /// </summary>
    CssToken ColorLiteral()
    {
      var current = _base.Advance();

      while (current.IsHex())
      {
        _base.StringBuffer.Append(current);
        current = _base.Advance();
      }

      _base.Back();
      return NewColor(_base.FlushBuffer());
    }

    /// <summary>
    /// 4.4.4. Hash state
    /// </summary>
    CssToken HashStart()
    {
      var current = _base.Advance();

      if (current.IsNameStart())
      {
        _base.StringBuffer.Append(current);
        return HashRest();
      }
      else if (IsValidEscape(current))
      {
        current = _base.Advance();
        _base.StringBuffer.Append(ConsumeEscape(current));
        return HashRest();
      }
      else if (current == Symbols.ReverseSolidus)
      {
        RaiseErrorOccurred(CssParseError.InvalidCharacter);
        _base.Back();
        return NewDelimiter(Symbols.Num);
      }
      else
      {
        _base.Back();
        return NewDelimiter(Symbols.Num);
      }
    }

    /// <summary>
    /// 4.4.5. Hash-rest state
    /// </summary>
    CssToken HashRest()
    {
      while (true)
      {
        var current = _base.Advance();

        if (current.IsName())
        {
          _base.StringBuffer.Append(current);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else if (current == Symbols.ReverseSolidus)
        {
          RaiseErrorOccurred(CssParseError.InvalidCharacter);
          _base.Back();
          return NewHash(_base.FlushBuffer());
        }
        else
        {
          _base.Back();
          return NewHash(_base.FlushBuffer());
        }
      }
    }

    /// <summary>
    /// 4.4.6. Comment state
    /// </summary>
    CssToken Comment()
    {
      var current = _base.Advance();

      while (current != Symbols.EndOfFile)
      {
        if (current == Symbols.Asterisk)
        {
          current = _base.Advance();

          if (current == Symbols.Solidus)
          {
            return NewComment(_base.FlushBuffer());
          }

          _base.StringBuffer.Append(Symbols.Asterisk);
        }
        else
        {
          _base.StringBuffer.Append(current);
          current = _base.Advance();
        }
      }

      RaiseErrorOccurred(CssParseError.EOF);
      return NewComment(_base.FlushBuffer(), bad: true);
    }

    /// <summary>
    /// 4.4.7. At-keyword state
    /// </summary>
    CssToken AtKeywordStart()
    {
      var current = _base.Advance();

      if (current == Symbols.Minus)
      {
        current = _base.Advance();

        if (current.IsNameStart() || IsValidEscape(current))
        {
          _base.StringBuffer.Append(Symbols.Minus);
          return AtKeywordRest(current);
        }

        _base.Back(2);
        return NewDelimiter(Symbols.At);
      }
      else if (current.IsNameStart())
      {
        _base.StringBuffer.Append(current);
        return AtKeywordRest(_base.Advance());
      }
      else if (IsValidEscape(current))
      {
        current = _base.Advance();
        _base.StringBuffer.Append(ConsumeEscape(current));
        return AtKeywordRest(_base.Advance());
      }
      else
      {
        _base.Back();
        return NewDelimiter(Symbols.At);
      }
    }

    /// <summary>
    /// 4.4.8. At-keyword-rest state
    /// </summary>
    CssToken AtKeywordRest(Char current)
    {
      while (true)
      {
        if (current.IsName())
        {
          _base.StringBuffer.Append(current);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else
        {
          _base.Back();
          return NewAtKeyword(_base.FlushBuffer());
        }

        current = _base.Advance();
      }
    }

    /// <summary>
    /// 4.4.9. Ident state
    /// </summary>
    CssToken IdentStart(Char current)
    {
      if (current == Symbols.Minus)
      {
        current = _base.Advance();

        if (current.IsNameStart() || IsValidEscape(current))
        {
          _base.StringBuffer.Append(Symbols.Minus);
          return IdentRest(current);
        }

        _base.Back();
        return NewDelimiter(Symbols.Minus);
      }
      else if (current.IsNameStart())
      {
        _base.StringBuffer.Append(current);
        return IdentRest(_base.Advance());
      }
      else if (current == Symbols.ReverseSolidus && IsValidEscape(current))
      {
        current = _base.Advance();
        _base.StringBuffer.Append(ConsumeEscape(current));
        return IdentRest(_base.Advance());
      }

      return Data(current);
    }

    /// <summary>
    /// 4.4.10. Ident-rest state
    /// </summary>
    CssToken IdentRest(Char current)
    {
      while (true)
      {
        if (current.IsName())
        {
          _base.StringBuffer.Append(current);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else if (current == Symbols.RoundBracketOpen)
        {
          var name = _base.FlushBuffer();
          var type = name.GetTypeFromName();
          return type == CssTokenType.Function ? NewFunction(name) : UrlStart(name.ToLowerInvariant());
        }
        else
        {
          _base.Back();
          return NewIdent(_base.FlushBuffer());
        }

        current = _base.Advance();
      }
    }

    /// <summary>
    /// 4.4.11. Transform-function-whitespace state
    /// </summary>
    CssToken TransformFunctionWhitespace(Char current)
    {
      while (true)
      {
        current = _base.Advance();

        if (current == Symbols.RoundBracketOpen)
        {
          _base.Back();
          return NewFunction(_base.FlushBuffer());
        }
        else if (!current.IsSpaceCharacter())
        {
          _base.Back(2);
          return NewIdent(_base.FlushBuffer());
        }
      }
    }

    /// <summary>
    /// 4.4.12. Number state
    /// </summary>
    CssToken NumberStart(Char current)
    {
      while (true)
      {
        if (current.IsOneOf(Symbols.Plus, Symbols.Minus))
        {
          _base.StringBuffer.Append(current);
          current = _base.Advance();

          if (current == Symbols.Dot)
          {
            _base.StringBuffer.Append(current);
            _base.StringBuffer.Append(_base.Advance());
            return NumberFraction();
          }

          _base.StringBuffer.Append(current);
          return NumberRest();
        }
        else if (current == Symbols.Dot)
        {
          _base.StringBuffer.Append(current);
          _base.StringBuffer.Append(_base.Advance());
          return NumberFraction();
        }
        else if (current.IsDigit())
        {
          _base.StringBuffer.Append(current);
          return NumberRest();
        }

        current = _base.Advance();
      }
    }

    /// <summary>
    /// 4.4.13. Number-rest state
    /// </summary>
    CssToken NumberRest()
    {
      var current = _base.Advance();

      while (true)
      {

        if (current.IsDigit())
        {
          _base.StringBuffer.Append(current);
        }
        else if (current.IsNameStart())
        {
          var number = _base.FlushBuffer();
          _base.StringBuffer.Append(current);
          return Dimension(number);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          var number = _base.FlushBuffer();
          _base.StringBuffer.Append(ConsumeEscape(current));
          return Dimension(number);
        }
        else
        {
          break;
        }

        current = _base.Advance();
      }

      switch (current)
      {
        case Symbols.Dot:
          current = _base.Advance();

          if (current.IsDigit())
          {
            _base.StringBuffer.Append(Symbols.Dot).Append(current);
            return NumberFraction();
          }

          _base.Back();
          return NewNumber(_base.FlushBuffer());

        case '%':
          return NewPercentage(_base.FlushBuffer());

        case 'e':
        case 'E':
          return NumberExponential(current);

        case Symbols.Minus:
          return NumberDash();

        default:
          _base.Back();
          return NewNumber(_base.FlushBuffer());
      }
    }

    /// <summary>
    /// 4.4.14. Number-fraction state
    /// </summary>
    CssToken NumberFraction()
    {
      var current = _base.Advance();

      while (true)
      {
        if (current.IsDigit())
        {
          _base.StringBuffer.Append(current);
        }
        else if (current.IsNameStart())
        {
          var number = _base.FlushBuffer();
          _base.StringBuffer.Append(current);
          return Dimension(number);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          var number = _base.FlushBuffer();
          _base.StringBuffer.Append(ConsumeEscape(current));
          return Dimension(number);
        }
        else
        {
          break;
        }

        current = _base.Advance();
      }

      switch (current)
      {
        case 'e':
        case 'E':
          return NumberExponential(current);

        case '%':
          return NewPercentage(_base.FlushBuffer());

        case Symbols.Minus:
          return NumberDash();

        default:
          _base.Back();
          return NewNumber(_base.FlushBuffer());
      }
    }

    /// <summary>
    /// 4.4.15. Dimension state
    /// </summary>
    CssToken Dimension(String number)
    {
      while (true)
      {
        var current = _base.Advance();

        if (current.IsLetter())
        {
          _base.StringBuffer.Append(current);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else
        {
          _base.Back();
          return NewDimension(number, _base.FlushBuffer());
        }
      }
    }

    /// <summary>
    /// 4.4.16. SciNotation state
    /// </summary>
    CssToken SciNotation()
    {
      while (true)
      {
        var current = _base.Advance();

        if (current.IsDigit())
        {
          _base.StringBuffer.Append(current);
        }
        else
        {
          _base.Back();
          return NewNumber(_base.FlushBuffer());
        }
      }
    }

    /// <summary>
    /// 4.4.17. URL state
    /// </summary>
    CssToken UrlStart(String functionName)
    {
      var current = _base.SkipSpaces();

      switch (current)
      {
        case Symbols.EndOfFile:
          RaiseErrorOccurred(CssParseError.EOF);
          return NewUrl(functionName, String.Empty, bad: true);

        case Symbols.DoubleQuote:
          return UrlDQ(functionName);

        case Symbols.SingleQuote:
          return UrlSQ(functionName);

        case Symbols.RoundBracketClose:
          return NewUrl(functionName, String.Empty, bad: false);

        default:
          return UrlUQ(current, functionName);
      }
    }

    /// <summary>
    /// 4.4.18. URL-double-quoted state
    /// </summary>
    CssToken UrlDQ(String functionName)
    {
      while (true)
      {
        var current = _base.Advance();

        if (current.IsLineBreak())
        {
          RaiseErrorOccurred(CssParseError.LineBreakUnexpected);
          return UrlBad(functionName);
        }
        else if (Symbols.EndOfFile == current)
        {
          return NewUrl(functionName, _base.FlushBuffer());
        }
        else if (current == Symbols.DoubleQuote)
        {
          return UrlEnd(functionName);
        }
        else if (current != Symbols.ReverseSolidus)
        {
          _base.StringBuffer.Append(current);
        }
        else
        {
          current = _base.Advance();

          if (current == Symbols.EndOfFile)
          {
            _base.Back(2);
            RaiseErrorOccurred(CssParseError.EOF);
            return NewUrl(functionName, _base.FlushBuffer(), bad: true);
          }
          else if (current.IsLineBreak())
          {
            _base.StringBuffer.AppendLine();
          }
          else
          {
            _base.StringBuffer.Append(ConsumeEscape(current));
          }
        }
      }
    }

    /// <summary>
    /// 4.4.19. URL-single-quoted state
    /// </summary>
    CssToken UrlSQ(String functionName)
    {
      while (true)
      {
        var current = _base.Advance();

        if (current.IsLineBreak())
        {
          RaiseErrorOccurred(CssParseError.LineBreakUnexpected);
          return UrlBad(functionName);
        }
        else if (current == Symbols.EndOfFile)
        {
          return NewUrl(functionName, _base.FlushBuffer());
        }
        else if (current == Symbols.SingleQuote)
        {
          return UrlEnd(functionName);
        }
        else if (current != Symbols.ReverseSolidus)
        {
          _base.StringBuffer.Append(current);
        }
        else
        {
          current = _base.Advance();

          if (current == Symbols.EndOfFile)
          {
            _base.Back(2);
            RaiseErrorOccurred(CssParseError.EOF);
            return NewUrl(functionName, _base.FlushBuffer(), bad: true);
          }
          else if (current.IsLineBreak())
          {
            _base.StringBuffer.AppendLine();
          }
          else
          {
            _base.StringBuffer.Append(ConsumeEscape(current));
          }
        }
      }
    }

    /// <summary>
    /// 4.4.21. URL-unquoted state
    /// </summary>
    CssToken UrlUQ(Char current, String functionName)
    {
      while (true)
      {
        if (current.IsSpaceCharacter())
        {
          return UrlEnd(functionName);
        }
        else if (current.IsOneOf(Symbols.RoundBracketClose, Symbols.EndOfFile))
        {
          return NewUrl(functionName, _base.FlushBuffer());
        }
        else if (current.IsOneOf(Symbols.DoubleQuote, Symbols.SingleQuote, Symbols.RoundBracketOpen) || current.IsNonPrintable())
        {
          RaiseErrorOccurred(CssParseError.InvalidCharacter);
          return UrlBad(functionName);
        }
        else if (current != Symbols.ReverseSolidus)
        {
          _base.StringBuffer.Append(current);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else
        {
          RaiseErrorOccurred(CssParseError.InvalidCharacter);
          return UrlBad(functionName);
        }

        current = _base.Advance();
      }
    }

    /// <summary>
    /// 4.4.20. URL-end state
    /// </summary>
    CssToken UrlEnd(String functionName)
    {
      while (true)
      {
        var current = _base.Advance();

        if (current == Symbols.RoundBracketClose)
        {
          return NewUrl(functionName, _base.FlushBuffer());
        }
        else if (!current.IsSpaceCharacter())
        {
          RaiseErrorOccurred(CssParseError.InvalidCharacter);
          _base.Back();
          return UrlBad(functionName);
        }
      }
    }

    /// <summary>
    /// 4.4.22. Bad URL state
    /// </summary>
    CssToken UrlBad(String functionName)
    {
      var current = _base.CurrentChar;
      var curly = 0;
      var round = 1;

      while (current != Symbols.EndOfFile)
      {
        if (current == Symbols.Semicolon)
        {
          _base.Back();
          return NewUrl(functionName, _base.FlushBuffer(), true);
        }
        else if (current == Symbols.CurlyBracketClose && --curly == -1)
        {
          _base.Back();
          return NewUrl(functionName, _base.FlushBuffer(), true);
        }
        else if (current == Symbols.RoundBracketClose && --round == 0)
        {
          return NewUrl(functionName, _base.FlushBuffer(), true);
        }
        else if (IsValidEscape(current))
        {
          current = _base.Advance();
          _base.StringBuffer.Append(ConsumeEscape(current));
        }
        else
        {
          if (current == Symbols.RoundBracketOpen)
          {
            ++round;
          }
          else if (curly == Symbols.CurlyBracketOpen)
          {
            ++curly;
          }

          _base.StringBuffer.Append(current);
        }

        current = _base.Advance();
      }

      RaiseErrorOccurred(CssParseError.EOF);
      return NewUrl(functionName, _base.FlushBuffer(), bad: true);
    }

    /// <summary>
    /// 4.4.23. Unicode-range State
    /// </summary>
    CssToken UnicodeRange(Char current)
    {
      for (var i = 0; i < 6 && current.IsHex(); i++)
      {
        _base.StringBuffer.Append(current);
        current = _base.Advance();
      }

      if (_base.StringBuffer.Length != 6)
      {
        for (var i = 0; i < 6 - _base.StringBuffer.Length; i++)
        {
          if (current != Symbols.QuestionMark)
          {
            current = _base.Back();
            break;
          }

          _base.StringBuffer.Append(current);
          current = _base.Advance();
        }

        return NewRange(_base.FlushBuffer());
      }
      else if (current == Symbols.Minus)
      {
        current = _base.Advance();

        if (current.IsHex())
        {
          var start = _base.FlushBuffer();

          for (var i = 0; i < 6; i++)
          {
            if (!current.IsHex())
            {
              current = _base.Back();
              break;
            }

            _base.StringBuffer.Append(current);
            current = _base.Advance();
          }

          var end = _base.FlushBuffer();
          return NewRange(start, end);
        }
        else
        {
          _base.Back(2);
          return NewRange(_base.FlushBuffer());
        }
      }
      else
      {
        _base.Back();
        return NewRange(_base.FlushBuffer());
      }
    }

    #endregion

    #region Tokens

    CssToken NewMatch(String match)
    {
      return new CssToken(CssTokenType.Match, match, _position);
    }

    CssToken NewColumn()
    {
      return new CssToken(CssTokenType.Column, CombinatorSymbols.Column, _position);
    }

    CssToken NewCloseCurly()
    {
      return new CssToken(CssTokenType.CurlyBracketClose, "}", _position);
    }

    CssToken NewOpenCurly()
    {
      return new CssToken(CssTokenType.CurlyBracketOpen, "{", _position);
    }

    CssToken NewCloseSquare()
    {
      return new CssToken(CssTokenType.SquareBracketClose, "]", _position);
    }

    CssToken NewOpenSquare()
    {
      return new CssToken(CssTokenType.SquareBracketOpen, "[", _position);
    }

    CssToken NewOpenComment()
    {
      return new CssToken(CssTokenType.Cdo, "<!--", _position);
    }

    CssToken NewSemicolon()
    {
      return new CssToken(CssTokenType.Semicolon, ";", _position);
    }

    CssToken NewColon()
    {
      return new CssToken(CssTokenType.Colon, ":", _position);
    }

    CssToken NewCloseComment()
    {
      return new CssToken(CssTokenType.Cdc, "-->", _position);
    }

    CssToken NewComma()
    {
      return new CssToken(CssTokenType.Comma, ",", _position);
    }

    CssToken NewCloseRound()
    {
      return new CssToken(CssTokenType.RoundBracketClose, ")", _position);
    }

    CssToken NewOpenRound()
    {
      return new CssToken(CssTokenType.RoundBracketOpen, "(", _position);
    }

    CssToken NewString(String value, Char quote, Boolean bad = false)
    {
      return new CssStringToken(value, bad, quote, _position);
    }

    CssToken NewHash(String value)
    {
      return new CssKeywordToken(CssTokenType.Hash, value, _position);
    }

    CssToken NewComment(String value, Boolean bad = false)
    {
      return new CssCommentToken(value, bad, _position);
    }

    CssToken NewAtKeyword(String value)
    {
      return new CssKeywordToken(CssTokenType.AtKeyword, value, _position);
    }

    CssToken NewIdent(String value)
    {
      return new CssKeywordToken(CssTokenType.Ident, value, _position);
    }

    CssToken NewFunction(String value)
    {
      var function = new CssFunctionToken(value, _position);
      var token = NextToken();

      while (token.Type != CssTokenType.EndOfFile)
      {
        function.AddArgumentToken(token);

        if (token.Type == CssTokenType.RoundBracketClose)
          break;

        token = NextToken();
      }

      return function;
    }

    CssToken NewPercentage(String value)
    {
      return new CssUnitToken(CssTokenType.Percentage, value, "%", _position);
    }

    CssToken NewDimension(String value, String unit)
    {
      return new CssUnitToken(CssTokenType.Dimension, value, unit, _position);
    }

    CssToken NewUrl(String functionName, String data, Boolean bad = false)
    {
      return new CssUrlToken(functionName, data, bad, _position);
    }

    CssToken NewRange(String range)
    {
      return new CssRangeToken(range, _position);
    }

    CssToken NewRange(String start, String end)
    {
      return new CssRangeToken(start, end, _position);
    }

    CssToken NewWhitespace(Char c)
    {
      return new CssToken(CssTokenType.Whitespace, c.ToString(), _position);
    }

    CssToken NewNumber(String number)
    {
      return new CssNumberToken(number, _position);
    }

    CssToken NewDelimiter(Char c)
    {
      return new CssToken(CssTokenType.Delim, c.ToString(), _position);
    }

    CssToken NewColor(String text)
    {
      return new CssColorToken(text, _position);
    }

    CssToken NewEof()
    {
      return new CssToken(CssTokenType.EndOfFile, String.Empty, _position);
    }

    #endregion

    #region Helpers

    CssToken NumberExponential(Char letter)
    {
      var current = _base.Advance();

      if (current.IsDigit())
      {
        _base.StringBuffer.Append(letter).Append(current);
        return SciNotation();
      }
      else if (current == Symbols.Plus || current == Symbols.Minus)
      {
        var op = current;
        current = _base.Advance();

        if (current.IsDigit())
        {
          _base.StringBuffer.Append(letter).Append(op).Append(current);
          return SciNotation();
        }

        _base.Back();
      }

      var number = _base.FlushBuffer();
      _base.StringBuffer.Append(letter);
      _base.Back();
      return Dimension(number);
    }

    CssToken NumberDash()
    {
      var current = _base.Advance();

      if (current.IsNameStart())
      {
        var number = _base.FlushBuffer();
        _base.StringBuffer.Append(Symbols.Minus).Append(current);
        return Dimension(number);
      }
      else if (IsValidEscape(current))
      {
        current = _base.Advance();
        var number = _base.FlushBuffer();
        _base.StringBuffer.Append(Symbols.Minus).Append(ConsumeEscape(current));
        return Dimension(number);
      }
      else
      {
        _base.Back(2);
        return NewNumber(_base.FlushBuffer());
      }
    }

    String ConsumeEscape(Char current)
    {
      if (current.IsHex())
      {
        var isHex = true;
        var escape = new Char[6];
        var length = 0;

        while (isHex && length < escape.Length)
        {
          escape[length++] = current;
          current = _base.Advance();
          isHex = current.IsHex();
        }

        if (!current.IsSpaceCharacter())
        {
          _base.Back();
        }

        var code = Int32.Parse(new String(escape, 0, length), NumberStyles.HexNumber);

        if (!code.IsInvalid())
        {
          return code.ConvertFromUtf32();
        }

        current = Symbols.Replacement;
      }

      return current.ToString();
    }

    Boolean IsValidEscape(Char current)
    {
      if (current == Symbols.ReverseSolidus)
      {
        current = _base.Advance();
        _base.Back();

        return current != Symbols.EndOfFile && !current.IsLineBreak();
      }

      return false;
    }

    void RaiseErrorOccurred(CssParseError code)
    {
      RaiseErrorOccurred(code, _base.GetCurrentPosition());
    }

    bool IEnumerator.MoveNext()
    {
      return NextToken().Type != CssTokenType.EndOfFile;
    }

    void IEnumerator.Reset()
    {
      throw new NotSupportedException();
    }

    public IEnumerator<CssToken> GetEnumerator()
    {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Dispose()
    {
      _base.Dispose();
    }

    #endregion
  }
}
