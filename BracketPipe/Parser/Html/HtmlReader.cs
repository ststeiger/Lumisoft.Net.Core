namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Collections;
  using System.Xml;
  using System.Linq;
  using System.Xml.Linq;

  /// <summary>
  /// Performs the tokenization of the source code. Follows the tokenization algorithm at:
  /// http://www.w3.org/html/wg/drafts/html/master/syntax.html
  /// </summary>
  public sealed class HtmlReader : XmlReader
    , IEnumerator<HtmlNode>
    , IEnumerable<HtmlNode>
    , IXmlLineInfo
    , IDisposable
  {
    #region Fields

    private IBaseTokenizer _base;
    private readonly HtmlEntityService _resolver;
    private String _lastStartTag;
    private TextPosition _position;
    private int _svgDepth = -1;
    private int _mathMlDepth = -1;
    private HtmlNode _current;

    #endregion

    #region Events

    /// <summary>
    /// Fired in case of a parse error.
    /// </summary>
    public event EventHandler<HtmlErrorEvent> Error;

    #endregion

    #region ctor

    /// <summary>
    /// See 8.2.4 Tokenization
    /// </summary>
    /// <param name="source">The source code manager.</param>
    public HtmlReader(TextSource source) : this(source, true) { }

    /// <summary>
    /// See 8.2.4 Tokenization
    /// </summary>
    /// <param name="source">The source code manager.</param>
    /// <param name="trackPosition">Whether to track the position of a node in the source text.</param>
    public HtmlReader(TextSource source, bool trackPosition)
    {
      _base = trackPosition
        ? (IBaseTokenizer)new BaseTokenizer(source)
        : new BaseTokenizerNoPosition(source);
      State = HtmlParseMode.PCData;
      IsAcceptingCharacterData = false;
      IsStrictMode = false;
      _lastStartTag = String.Empty;
      _resolver = HtmlEntityService.Resolver;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Current node on which the enumerator is positioned
    /// </summary>
    public HtmlNode Current { get { return _current; } }

    /// <summary>
    /// Gets or sets if CDATA sections are accepted.
    /// </summary>
    public Boolean IsAcceptingCharacterData { get; set; }

    /// <summary>
    /// Gets or sets the current parse mode.
    /// </summary>
    public HtmlParseMode State { get; set; }

    /// <summary>
    /// Gets or sets if strict mode is used.
    /// </summary>
    public Boolean IsStrictMode { get; set; }

    object IEnumerator.Current { get { return _current; } }
    int IXmlLineInfo.LineNumber { get { return _current.Position.Line; } }
    int IXmlLineInfo.LinePosition { get { return _current.Position.Column; } }

    #endregion

    #region Methods

    /// <summary>
    /// Returns the next node
    /// </summary>
    public HtmlNode NextNode()
    {
      Read();
      return _current;
    }

    /// <summary>
    /// Positions the reader at the next node.
    /// </summary>
    /// <returns>Returns <c>true</c> if more nodes are available, <c>false</c> otherwise</returns>
    public override bool Read()
    {
      _attrIndex = -1;
      var current = _base.Advance();
      _position = _base.GetCurrentPosition();

      if (current != Symbols.EndOfFile)
      {
        _current = default(HtmlNode);
        switch (State)
        {
          case HtmlParseMode.PCData:
            _current = Data(current);
            break;
          case HtmlParseMode.RCData:
            _current = RCData(current);
            break;
          case HtmlParseMode.Plaintext:
            _current = Plaintext(current);
            break;
          case HtmlParseMode.Rawtext:
            _current = Rawtext(current);
            break;
          case HtmlParseMode.Script:
            _current = ScriptData(current);
            break;
        }

        if (_current.Type == HtmlTokenType.StartTag
          && !IsEmptyElement)
          _depth++;
        else if (_current.Type == HtmlTokenType.EndTag)
          _depth--;

        var tag = _current as HtmlTagNode;
        if (_svgDepth < 0
          && tag != null
          && _current.Type == HtmlTokenType.StartTag
          && _current.Value.Is(TagNames.Svg))
        {
          _svgDepth = 0;
          _current = HtmlForeign.SvgConfig((HtmlStartTag)tag);
          return true;
        }
        else if (_svgDepth >= 0 && tag != null)
        {
          switch (tag.Type)
          {
            case HtmlTokenType.StartTag:
              if (!tag.IsSelfClosing)
                _svgDepth++;
              _current = HtmlForeign.SvgConfig((HtmlStartTag)tag);
              return true;
            case HtmlTokenType.EndTag:
              _svgDepth--;
              break;
          }
        }
        else if (_mathMlDepth < 0
          && tag != null
          && _current.Type == HtmlTokenType.StartTag
          && _current.Value.Is(TagNames.Math))
        {
          _mathMlDepth = 0;
          _current = HtmlForeign.MathMlConfig((HtmlStartTag)tag);
          return true;
        }
        else if (_mathMlDepth >= 0 && tag != null)
        {
          switch (tag.Type)
          {
            case HtmlTokenType.StartTag:
              if (!tag.IsSelfClosing)
                _mathMlDepth++;
              _current = HtmlForeign.MathMlConfig((HtmlStartTag)tag);
              return true;
            case HtmlTokenType.EndTag:
              _mathMlDepth--;
              break;
          }
        }

        return true;
      }

      _current = NewEof(acceptable: true);
      return false;
    }

    internal void RaiseErrorOccurred(HtmlParseError code, TextPosition position)
    {
      var handler = Error;

      if (IsStrictMode)
      {
        var message = "Error while parsing the provided HTML document.";
        throw new HtmlParseException(code.GetCode(), message, position);
      }
      else if (handler != null)
      {
        var errorEvent = new HtmlErrorEvent(code, position);
        handler.Invoke(this, errorEvent);
      }
    }

    #endregion

    #region Data

    /// <summary>
    /// See 8.2.4.1 Data state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode Data(Char c)
    {
      return c == Symbols.LessThan ? TagOpen(_base.Advance()) : DataText(c);
    }

    private HtmlNode DataText(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.LessThan:
          case Symbols.EndOfFile:
            _base.Back();
            return NewCharacter();

          case Symbols.Ampersand:
            AppendCharacterReference(_base.Advance());
            break;

          case Symbols.Null:
            RaiseErrorOccurred(HtmlParseError.Null);
            break;

          default:
            _base.StringBuffer.Append(c);
            break;
        }

        c = _base.Advance();
      }
    }

    #endregion

    #region Plaintext

    /// <summary>
    /// See 8.2.4.7 PLAINTEXT state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode Plaintext(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.Null:
            AppendReplacement();
            break;

          case Symbols.EndOfFile:
            _base.Back();
            return NewCharacter();

          default:
            _base.StringBuffer.Append(c);
            break;
        }

        c = _base.Advance();
      }
    }

    #endregion

    #region RCData

    /// <summary>
    /// See 8.2.4.3 RCDATA state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode RCData(Char c)
    {
      return c == Symbols.LessThan ? RCDataLt(_base.Advance()) : RCDataText(c);
    }

    private HtmlNode RCDataText(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.Ampersand:
            AppendCharacterReference(_base.Advance());
            break;

          case Symbols.LessThan:
          case Symbols.EndOfFile:
            _base.Back();
            return NewCharacter();

          case Symbols.Null:
            AppendReplacement();
            break;

          default:
            _base.StringBuffer.Append(c);
            break;
        }

        c = _base.Advance();
      }
    }

    /// <summary>
    /// See 8.2.4.11 RCDATA less-than sign state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode RCDataLt(Char c)
    {
      if (c == Symbols.Solidus)
      {
        // See 8.2.4.12 RCDATA end tag open state
        c = _base.Advance();

        if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
          return RCDataNameEndTag(_base.Advance());
        }
        else if (c.IsLowercaseAscii())
        {
          _base.StringBuffer.Append(c);
          return RCDataNameEndTag(_base.Advance());
        }
        else
        {
          _base.StringBuffer.Append(Symbols.LessThan).Append(Symbols.Solidus);
          return RCDataText(c);
        }
      }
      else
      {
        _base.StringBuffer.Append(Symbols.LessThan);
        return RCDataText(c);
      }
    }

    /// <summary>
    /// See 8.2.4.13 RCDATA end tag name state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode RCDataNameEndTag(Char c)
    {
      while (true)
      {
        var token = CreateIfAppropriate(c);

        if (token != null)
        {
          return token;
        }
        else if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        }
        else if (c.IsLowercaseAscii())
        {
          _base.StringBuffer.Append(c);
        }
        else
        {
          _base.StringBuffer.Insert(0, Symbols.LessThan).Insert(1, Symbols.Solidus);
          return RCDataText(c);
        }

        c = _base.Advance();
      }
    }

    #endregion

    #region Rawtext

    /// <summary>
    /// See 8.2.4.5 RAWTEXT state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode Rawtext(Char c)
    {
      return c == Symbols.LessThan ? RawtextLT(_base.Advance()) : RawtextText(c);
    }

    private HtmlNode RawtextText(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.LessThan:
          case Symbols.EndOfFile:
            _base.Back();
            return NewCharacter();

          case Symbols.Null:
            AppendReplacement();
            break;

          default:
            _base.StringBuffer.Append(c);
            break;
        }

        c = _base.Advance();
      }
    }

    /// <summary>
    /// See 8.2.4.14 RAWTEXT less-than sign state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode RawtextLT(Char c)
    {
      if (c == Symbols.Solidus)
      {
        // See 8.2.4.15 RAWTEXT end tag open state
        c = _base.Advance();

        if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
          return RawtextNameEndTag(_base.Advance());
        }
        else if (c.IsLowercaseAscii())
        {
          _base.StringBuffer.Append(c);
          return RawtextNameEndTag(_base.Advance());
        }
        else
        {
          _base.StringBuffer.Append(Symbols.LessThan).Append(Symbols.Solidus);
          return RawtextText(c);
        }
      }
      else
      {
        _base.StringBuffer.Append(Symbols.LessThan);
        return RawtextText(c);
      }
    }

    /// <summary>
    /// See 8.2.4.16 RAWTEXT end tag name state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode RawtextNameEndTag(Char c)
    {
      while (true)
      {
        var token = CreateIfAppropriate(c);

        if (token != null)
        {
          return token;
        }
        else if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        }
        else if (c.IsLowercaseAscii())
        {
          _base.StringBuffer.Append(c);
        }
        else
        {
          _base.StringBuffer.Insert(0, Symbols.LessThan).Insert(1, Symbols.Solidus);
          return RawtextText(c);
        }

        c = _base.Advance();
      }
    }

    #endregion

    #region CDATA

    /// <summary>
    /// See 8.2.4.68 CDATA section state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CharacterData(Char c)
    {
      while (true)
      {
        if (c == Symbols.EndOfFile)
        {
          _base.Back();
          break;
        }
        else if (c == Symbols.SquareBracketClose && _base.ContinuesWithSensitive("]]>"))
        {
          _base.Advance(2);
          break;
        }
        else
        {
          _base.StringBuffer.Append(c);
          c = _base.Advance();
        }
      }

      return NewCharacter();
    }

    /// <summary>
    /// See 8.2.4.69 Tokenizing character references
    /// </summary>
    /// <param name="c">The next input character.</param>
    /// <param name="allowedCharacter">The additionally allowed character if there is one.</param>
    private void AppendCharacterReference(Char c, Char allowedCharacter = Symbols.Null)
    {
      if (c.IsSpaceCharacter() || c == Symbols.LessThan || c == Symbols.EndOfFile || c == Symbols.Ampersand || c == allowedCharacter)
      {
        _base.Back();
        _base.StringBuffer.Append(Symbols.Ampersand);
      }
      else
      {
        var entity = default(String);

        if (c == Symbols.Num)
        {
          entity = GetNumericCharacterReference(_base.Advance());
        }
        else
        {
          entity = GetLookupCharacterReference(c, allowedCharacter);
        }

        if (entity == null)
        {
          _base.StringBuffer.Append(Symbols.Ampersand);
        }
        else
        {
          _base.StringBuffer.Append(entity);
        }
      }
    }

    private String GetNumericCharacterReference(Char c)
    {
      var exp = 10;
      var basis = 1;
      var num = 0;
      var nums = new List<Int32>();
      var isHex = c == 'x' || c == 'X';

      if (isHex)
      {
        exp = 16;

        while ((c = _base.Advance()).IsHex())
        {
          nums.Add(c.FromHex());
        }
      }
      else
      {
        while (c.IsDigit())
        {
          nums.Add(c.FromHex());
          c = _base.Advance();
        }
      }

      for (var i = nums.Count - 1; i >= 0; i--)
      {
        num += nums[i] * basis;
        basis *= exp;
      }

      if (nums.Count == 0)
      {
        _base.Back(2);

        if (isHex)
        {
          _base.Back();
        }

        RaiseErrorOccurred(HtmlParseError.CharacterReferenceWrongNumber);
        return null;
      }

      if (c != Symbols.Semicolon)
      {
        RaiseErrorOccurred(HtmlParseError.CharacterReferenceSemicolonMissing);
        _base.Back();
      }

      if (HtmlEntityService.IsInCharacterTable(num))
      {
        RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidCode);
        return HtmlEntityService.GetSymbolFromTable(num);
      }
      else if (HtmlEntityService.IsInvalidNumber(num))
      {
        RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidNumber);
        return Symbols.Replacement.ToString();
      }
      else if (HtmlEntityService.IsInInvalidRange(num))
      {
        RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidRange);
      }

      return num.ConvertFromUtf32();
    }

    private String GetLookupCharacterReference(Char c, Char allowedCharacter)
    {
      var entity = default(String);
      var start = _base.InsertionPoint - 1;
      var reference = new Char[32];
      var index = 0;
      var chr = _base.CurrentChar;

      do
      {
        if (chr == Symbols.Semicolon || !chr.IsName())
        {
          break;
        }

        reference[index++] = chr;
        chr = _base.Advance();
      }
      while (chr != Symbols.EndOfFile && index < 31);

      if (chr == Symbols.Semicolon)
      {
        reference[index] = Symbols.Semicolon;
        var value = new String(reference, 0, index + 1);
        entity = _resolver.GetSymbol(value);
      }

      while (entity == null && index > 0)
      {
        var value = new String(reference, 0, index--);
        entity = _resolver.GetSymbol(value);

        if (entity == null)
        {
          _base.Back();
        }
      }

      chr = _base.CurrentChar;

      if (chr != Symbols.Semicolon)
      {
        if (allowedCharacter != Symbols.Null && (chr == Symbols.Equality || chr.IsAlphanumericAscii()))
        {
          if (chr == Symbols.Equality)
          {
            RaiseErrorOccurred(HtmlParseError.CharacterReferenceAttributeEqualsFound);
          }

          _base.InsertionPoint = start;
          return null;
        }

        _base.Back();
        RaiseErrorOccurred(HtmlParseError.CharacterReferenceNotTerminated);
      }

      return entity;
    }

    #endregion

    #region Tags

    /// <summary>
    /// See 8.2.4.8 Tag open state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode TagOpen(Char c)
    {
      if (c == Symbols.Solidus)
      {
        return TagEnd(_base.Advance());
      }
      else if (c.IsLowercaseAscii())
      {
        _base.StringBuffer.Append(c);
        return TagName(NewTagOpen());
      }
      else if (c.IsUppercaseAscii())
      {
        _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        return TagName(NewTagOpen());
      }
      else if (c == Symbols.ExclamationMark)
      {
        return MarkupDeclaration(_base.Advance());
      }
      else if (c != Symbols.QuestionMark)
      {
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.AmbiguousOpenTag);
        _base.StringBuffer.Append(Symbols.LessThan);
        return DataText(c);
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.BogusComment);
        return BogusComment(c);
      }
    }

    /// <summary>
    /// See 8.2.4.9 End tag open state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode TagEnd(Char c)
    {
      if (c.IsLowercaseAscii())
      {
        _base.StringBuffer.Append(c);
        return TagName(NewTagClose());
      }
      else if (c.IsUppercaseAscii())
      {
        _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        return TagName(NewTagClose());
      }
      else if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        return Data(_base.Advance());
      }
      else if (c == Symbols.EndOfFile)
      {
        _base.Back();
        RaiseErrorOccurred(HtmlParseError.EOF);
        _base.StringBuffer.Append(Symbols.LessThan).Append(Symbols.Solidus);
        return NewCharacter();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.BogusComment);
        return BogusComment(c);
      }
    }

    /// <summary>
    /// See 8.2.4.10 Tag name state
    /// </summary>
    /// <param name="tag">The current tag token.</param>
    private HtmlNode TagName(HtmlTagNode tag)
    {
      while (true)
      {
        var c = _base.Advance();

        if (c == Symbols.GreaterThan)
        {
          tag.Value = _base.FlushBuffer();
          return EmitTag(tag);
        }
        else if (c.IsSpaceCharacter())
        {
          tag.Value = _base.FlushBuffer();
          return ParseAttributes(tag);
        }
        else if (c == Symbols.Solidus)
        {
          tag.Value = _base.FlushBuffer();
          return TagSelfClosing(tag);
        }
        else if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        }
        else if (c == Symbols.Null)
        {
          AppendReplacement();
        }
        else if (c != Symbols.EndOfFile)
        {
          _base.StringBuffer.Append(c);
        }
        else
        {
          return NewEof();
        }
      }
    }

    /// <summary>
    /// See 8.2.4.43 Self-closing start tag state
    /// </summary>
    /// <param name="tag">The current tag token.</param>
    private HtmlNode TagSelfClosing(HtmlTagNode tag)
    {
      switch (_base.Advance())
      {
        case Symbols.GreaterThan:
          tag.IsSelfClosing = true;
          return EmitTag(tag);
        case Symbols.EndOfFile:
          return NewEof();
        default:
          RaiseErrorOccurred(HtmlParseError.ClosingSlashMisplaced);
          _base.Back();
          return ParseAttributes(tag);
      }
    }

    /// <summary>
    /// See 8.2.4.45 Markup declaration open state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode MarkupDeclaration(Char c)
    {
      if (_base.ContinuesWithSensitive("--"))
      {
        _base.Advance();
        return CommentStart(_base.Advance());
      }
      else if (_base.ContinuesWithInsensitive(TagNames.Doctype))
      {
        _base.Advance(6);
        return Doctype(_base.Advance());
      }
      else if (IsAcceptingCharacterData && _base.ContinuesWithSensitive(Keywords.CData))
      {
        _base.Advance(6);
        return CharacterData(_base.Advance());
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.UndefinedMarkupDeclaration);
        return BogusComment(c, true);
      }
    }

    #endregion

    #region Comments

    /// <summary>
    /// See 8.2.4.44 Bogus comment state
    /// </summary>
    /// <param name="c">The current character.</param>
    private HtmlNode BogusComment(Char c, bool hadExclamation = false)
    {
      _base.StringBuffer.Clear();

      var downlevelRevealedConditional = hadExclamation && c == '[';

      while (true)
      {
        switch (c)
        {
          case Symbols.GreaterThan:
            break;
          case Symbols.EndOfFile:
            _base.Back();
            break;
          case Symbols.Null:
            c = Symbols.Replacement;
            goto default;
          default:
            _base.StringBuffer.Append(c);
            c = _base.Advance();
            continue;
        }

        State = HtmlParseMode.PCData;
        var result = NewComment();
        result.DownlevelRevealedConditional = downlevelRevealedConditional;
        return result;
      }
    }

    /// <summary>
    /// See 8.2.4.46 Comment start state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CommentStart(Char c)
    {
      _base.StringBuffer.Clear();

      switch (c)
      {
        case Symbols.Minus:
          return CommentDashStart(_base.Advance()) ?? Comment(_base.Advance());
        case Symbols.Null:
          AppendReplacement();
          return Comment(_base.Advance());
        case Symbols.GreaterThan:
          State = HtmlParseMode.PCData;
          RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
          break;
        case Symbols.EndOfFile:
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          break;
        default:
          _base.StringBuffer.Append(c);
          return Comment(_base.Advance());
      }

      return NewComment();
    }

    /// <summary>
    /// See 8.2.4.47 Comment start dash state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CommentDashStart(Char c)
    {
      switch (c)
      {
        case Symbols.Minus:
          return CommentEnd(_base.Advance());
        case Symbols.Null:
          RaiseErrorOccurred(HtmlParseError.Null);
          _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Replacement);
          return Comment(_base.Advance());
        case Symbols.GreaterThan:
          State = HtmlParseMode.PCData;
          RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
          break;
        case Symbols.EndOfFile:
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          break;
        default:
          _base.StringBuffer.Append(Symbols.Minus).Append(c);
          return Comment(_base.Advance());
      }

      return NewComment();
    }

    /// <summary>
    /// See 8.2.4.48 Comment state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode Comment(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.Minus:
            var result = CommentDashEnd(_base.Advance());

            if (result != null)
            {
              return result;
            }

            break;
          case Symbols.EndOfFile:
            RaiseErrorOccurred(HtmlParseError.EOF);
            _base.Back();
            return NewComment();
          case Symbols.Null:
            AppendReplacement();
            break;
          default:
            _base.StringBuffer.Append(c);
            break;
        }

        c = _base.Advance();
      }
    }

    /// <summary>
    /// See 8.2.4.49 Comment end dash state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CommentDashEnd(Char c)
    {
      switch (c)
      {
        case Symbols.Minus:
          return CommentEnd(_base.Advance());
        case Symbols.EndOfFile:
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          return NewComment();
        case Symbols.Null:
          RaiseErrorOccurred(HtmlParseError.Null);
          c = Symbols.Replacement;
          break;
      }

      _base.StringBuffer.Append(Symbols.Minus).Append(c);
      return null;
    }

    /// <summary>
    /// See 8.2.4.50 Comment end state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CommentEnd(Char c)
    {
      while (true)
      {
        switch (c)
        {
          case Symbols.GreaterThan:
            State = HtmlParseMode.PCData;
            return NewComment();
          case Symbols.Null:
            RaiseErrorOccurred(HtmlParseError.Null);
            _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Replacement);
            return null;
          case Symbols.ExclamationMark:
            RaiseErrorOccurred(HtmlParseError.CommentEndedWithEM);
            return CommentBangEnd(_base.Advance());
          case Symbols.Minus:
            RaiseErrorOccurred(HtmlParseError.CommentEndedWithDash);
            _base.StringBuffer.Append(Symbols.Minus);
            break;
          case Symbols.EndOfFile:
            RaiseErrorOccurred(HtmlParseError.EOF);
            _base.Back();
            return NewComment();
          default:
            RaiseErrorOccurred(HtmlParseError.CommentEndedUnexpected);
            _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Minus).Append(c);
            return null;
        }

        c = _base.Advance();
      }
    }

    /// <summary>
    /// See 8.2.4.51 Comment end bang state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode CommentBangEnd(Char c)
    {
      switch (c)
      {
        case Symbols.Minus:
          _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Minus).Append(Symbols.ExclamationMark);
          return CommentDashEnd(_base.Advance());
        case Symbols.GreaterThan:
          State = HtmlParseMode.PCData;
          break;
        case Symbols.Null:
          RaiseErrorOccurred(HtmlParseError.Null);
          _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Minus).Append(Symbols.ExclamationMark).Append(Symbols.Replacement);
          return null;
        case Symbols.EndOfFile:
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          break;
        default:
          _base.StringBuffer.Append(Symbols.Minus).Append(Symbols.Minus).Append(Symbols.ExclamationMark).Append(c);
          return null;
      }

      return NewComment();
    }

    #endregion

    #region Doctype

    /// <summary>
    /// See 8.2.4.52 DOCTYPE state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode Doctype(Char c)
    {
      if (c.IsSpaceCharacter())
      {
        return DoctypeNameBefore(_base.Advance());
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        _base.Back();
        return NewDoctype(true);
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeUnexpected);
        return DoctypeNameBefore(c);
      }
    }

    /// <summary>
    /// See 8.2.4.53 Before DOCTYPE name state
    /// </summary>
    /// <param name="c">The next input character.</param>
    private HtmlNode DoctypeNameBefore(Char c)
    {
      while (c.IsSpaceCharacter())
        c = _base.Advance();

      if (c.IsUppercaseAscii())
      {
        var doctype = NewDoctype(false);
        _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        return DoctypeName(doctype);
      }
      else if (c == Symbols.Null)
      {
        var doctype = NewDoctype(false);
        AppendReplacement();
        return DoctypeName(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        var doctype = NewDoctype(true);
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        return doctype;
      }
      else if (c == Symbols.EndOfFile)
      {
        var doctype = NewDoctype(true);
        RaiseErrorOccurred(HtmlParseError.EOF);
        _base.Back();
        return doctype;
      }
      else
      {
        var doctype = NewDoctype(false);
        _base.StringBuffer.Append(c);
        return DoctypeName(doctype);
      }
    }

    /// <summary>
    /// See 8.2.4.54 DOCTYPE name state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeName(HtmlDoctype doctype)
    {
      while (true)
      {
        var c = _base.Advance();

        if (c.IsSpaceCharacter())
        {
          doctype.Value = _base.FlushBuffer();
          return DoctypeNameAfter(doctype);
        }
        else if (c == Symbols.GreaterThan)
        {
          State = HtmlParseMode.PCData;
          doctype.Value = _base.FlushBuffer();
          break;
        }
        else if (c.IsUppercaseAscii())
        {
          _base.StringBuffer.Append(Char.ToLowerInvariant(c));
        }
        else if (c == Symbols.Null)
        {
          AppendReplacement();
        }
        else if (c == Symbols.EndOfFile)
        {
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          doctype.IsQuirksForced = true;
          doctype.Value = _base.FlushBuffer();
          break;
        }
        else
        {
          _base.StringBuffer.Append(c);
        }
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.55 After DOCTYPE name state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeNameAfter(HtmlDoctype doctype)
    {
      var c = _base.SkipSpaces();

      if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        _base.Back();
        doctype.IsQuirksForced = true;
      }
      else if (_base.ContinuesWithInsensitive(Keywords.Public))
      {
        _base.Advance(5);
        return DoctypePublic(doctype);
      }
      else if (_base.ContinuesWithInsensitive(Keywords.System))
      {
        _base.Advance(5);
        return DoctypeSystem(doctype);
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeUnexpectedAfterName);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.56 After DOCTYPE public keyword state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypePublic(HtmlDoctype doctype)
    {
      var c = _base.Advance();

      if (c.IsSpaceCharacter())
      {
        return DoctypePublicIdentifierBefore(doctype);
      }
      else if (c == Symbols.DoubleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.PublicIdentifier = String.Empty;
        return DoctypePublicIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.PublicIdentifier = String.Empty;
        return DoctypePublicIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypePublicInvalid);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.57 Before DOCTYPE public identifier state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypePublicIdentifierBefore(HtmlDoctype doctype)
    {
      var c = _base.SkipSpaces();

      if (c == Symbols.DoubleQuote)
      {
        doctype.PublicIdentifier = String.Empty;
        return DoctypePublicIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        doctype.PublicIdentifier = String.Empty;
        return DoctypePublicIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypePublicInvalid);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.58 DOCTYPE public identifier (double-quoted) state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypePublicIdentifierDoubleQuoted(HtmlDoctype doctype)
    {
      while (true)
      {
        var c = _base.Advance();

        if (c == Symbols.DoubleQuote)
        {
          doctype.PublicIdentifier = _base.FlushBuffer();
          return DoctypePublicIdentifierAfter(doctype);
        }
        else if (c == Symbols.Null)
        {
          AppendReplacement();
        }
        else if (c == Symbols.GreaterThan)
        {
          State = HtmlParseMode.PCData;
          RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
          doctype.IsQuirksForced = true;
          doctype.PublicIdentifier = _base.FlushBuffer();
          break;
        }
        else if (c == Symbols.EndOfFile)
        {
          RaiseErrorOccurred(HtmlParseError.EOF);
          _base.Back();
          doctype.IsQuirksForced = true;
          doctype.PublicIdentifier = _base.FlushBuffer();
          break;
        }
        else
        {
          _base.StringBuffer.Append(c);
        }
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.59 DOCTYPE public identifier (single-quoted) state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypePublicIdentifierSingleQuoted(HtmlDoctype doctype)
    {
      while (true)
      {
        var c = _base.Advance();

        if (c == Symbols.SingleQuote)
        {
          doctype.PublicIdentifier = _base.FlushBuffer();
          return DoctypePublicIdentifierAfter(doctype);
        }
        else if (c == Symbols.Null)
        {
          AppendReplacement();
        }
        else if (c == Symbols.GreaterThan)
        {
          State = HtmlParseMode.PCData;
          RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
          doctype.IsQuirksForced = true;
          doctype.PublicIdentifier = _base.FlushBuffer();
          break;
        }
        else if (c == Symbols.EndOfFile)
        {
          RaiseErrorOccurred(HtmlParseError.EOF);
          doctype.IsQuirksForced = true;
          doctype.PublicIdentifier = _base.FlushBuffer();
          _base.Back();
          break;
        }
        else
        {
          _base.StringBuffer.Append(c);
        }
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.60 After DOCTYPE public identifier state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypePublicIdentifierAfter(HtmlDoctype doctype)
    {
      var c = _base.Advance();

      if (c.IsSpaceCharacter())
      {
        return DoctypeBetween(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
      }
      else if (c == Symbols.DoubleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.61 Between DOCTYPE public and system identifiers state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeBetween(HtmlDoctype doctype)
    {
      var c = _base.SkipSpaces();

      if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
      }
      else if (c == Symbols.DoubleQuote)
      {
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.62 After DOCTYPE system keyword state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeSystem(HtmlDoctype doctype)
    {
      var c = _base.Advance();

      if (c.IsSpaceCharacter())
      {
        State = HtmlParseMode.PCData;
        return DoctypeSystemIdentifierBefore(doctype);
      }
      else if (c == Symbols.DoubleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.SystemIdentifier = _base.FlushBuffer();
        doctype.IsQuirksForced = true;
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeSystemInvalid);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.63 Before DOCTYPE system identifier state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeSystemIdentifierBefore(HtmlDoctype doctype)
    {
      var c = _base.SkipSpaces();

      if (c == Symbols.DoubleQuote)
      {
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierDoubleQuoted(doctype);
      }
      else if (c == Symbols.SingleQuote)
      {
        doctype.SystemIdentifier = String.Empty;
        return DoctypeSystemIdentifierSingleQuoted(doctype);
      }
      else if (c == Symbols.GreaterThan)
      {
        State = HtmlParseMode.PCData;
        RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
        doctype.SystemIdentifier = _base.FlushBuffer();
      }
      else if (c == Symbols.EndOfFile)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        doctype.SystemIdentifier = _base.FlushBuffer();
        _base.Back();
      }
      else
      {
        RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.64 DOCTYPE system identifier (double-quoted) state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeSystemIdentifierDoubleQuoted(HtmlDoctype doctype)
    {
      while (true)
      {
        var c = _base.Advance();

        if (c == Symbols.DoubleQuote)
        {
          doctype.SystemIdentifier = _base.FlushBuffer();
          return DoctypeSystemIdentifierAfter(doctype);
        }
        else if (c == Symbols.Null)
        {
          AppendReplacement();
        }
        else if (c == Symbols.GreaterThan)
        {
          State = HtmlParseMode.PCData;
          RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
          doctype.IsQuirksForced = true;
          doctype.SystemIdentifier = _base.FlushBuffer();
          break;
        }
        else if (c == Symbols.EndOfFile)
        {
          RaiseErrorOccurred(HtmlParseError.EOF);
          doctype.IsQuirksForced = true;
          doctype.SystemIdentifier = _base.FlushBuffer();
          _base.Back();
          break;
        }
        else
        {
          _base.StringBuffer.Append(c);
        }
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.65 DOCTYPE system identifier (single-quoted) state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeSystemIdentifierSingleQuoted(HtmlDoctype doctype)
    {
      while (true)
      {
        var c = _base.Advance();

        switch (c)
        {
          case Symbols.SingleQuote:
            doctype.SystemIdentifier = _base.FlushBuffer();
            return DoctypeSystemIdentifierAfter(doctype);
          case Symbols.Null:
            AppendReplacement();
            continue;
          case Symbols.GreaterThan:
            State = HtmlParseMode.PCData;
            RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
            doctype.IsQuirksForced = true;
            doctype.SystemIdentifier = _base.FlushBuffer();
            break;
          case Symbols.EndOfFile:
            RaiseErrorOccurred(HtmlParseError.EOF);
            doctype.IsQuirksForced = true;
            doctype.SystemIdentifier = _base.FlushBuffer();
            _base.Back();
            break;
          default:
            _base.StringBuffer.Append(c);
            continue;
        }

        return doctype;
      }
    }

    /// <summary>
    /// See 8.2.4.66 After DOCTYPE system identifier state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode DoctypeSystemIdentifierAfter(HtmlDoctype doctype)
    {
      var c = _base.SkipSpaces();

      switch (c)
      {
        case Symbols.GreaterThan:
          State = HtmlParseMode.PCData;
          break;
        case Symbols.EndOfFile:
          RaiseErrorOccurred(HtmlParseError.EOF);
          doctype.IsQuirksForced = true;
          _base.Back();
          break;
        default:
          RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
          return BogusDoctype(doctype);
      }

      return doctype;
    }

    /// <summary>
    /// See 8.2.4.67 Bogus DOCTYPE state
    /// </summary>
    /// <param name="doctype">The current doctype token.</param>
    private HtmlNode BogusDoctype(HtmlDoctype doctype)
    {
      while (true)
      {
        switch (_base.Advance())
        {
          case Symbols.GreaterThan:
            State = HtmlParseMode.PCData;
            break;
          case Symbols.EndOfFile:
            _base.Back();
            break;
          default:
            continue;
        }

        return doctype;
      }
    }

    #endregion

    #region Attributes

    private enum AttributeState : byte
    {
      BeforeName,
      Name,
      AfterName,
      BeforeValue,
      QuotedValue,
      AfterValue,
      UnquotedValue
    }

    private HtmlNode ParseAttributes(HtmlTagNode tag)
    {
      var state = AttributeState.BeforeName;
      var quote = Symbols.DoubleQuote;
      var c = Symbols.Null;

      while (true)
      {
        switch (state)
        {
          // See 8.2.4.34 Before attribute name state
          case AttributeState.BeforeName:
            {
              c = _base.SkipSpaces();

              if (c == Symbols.Solidus)
              {
                return TagSelfClosing(tag);
              }
              else if (c == Symbols.GreaterThan)
              {
                return EmitTag(tag);
              }
              else if (c.IsUppercaseAscii())
              {
                _base.StringBuffer.Append(Char.ToLowerInvariant(c));
                state = AttributeState.Name;
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
                state = AttributeState.Name;
              }
              else if (c == Symbols.SingleQuote || c == Symbols.DoubleQuote || c == Symbols.Equality || c == Symbols.LessThan)
              {
                RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                _base.StringBuffer.Append(c);
                state = AttributeState.Name;
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
                state = AttributeState.Name;
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.35 Attribute name state
          case AttributeState.Name:
            {
              c = _base.Advance();

              if (c == Symbols.Equality)
              {
                tag.AddAttribute(_base.FlushBuffer());
                state = AttributeState.BeforeValue;
              }
              else if (c == Symbols.GreaterThan)
              {
                tag.AddAttribute(_base.FlushBuffer());
                return EmitTag(tag);
              }
              else if (c.IsSpaceCharacter())
              {
                tag.AddAttribute(_base.FlushBuffer());
                state = AttributeState.AfterName;
              }
              else if (c == Symbols.Solidus)
              {
                tag.AddAttribute(_base.FlushBuffer());
                return TagSelfClosing(tag);
              }
              else if (c.IsUppercaseAscii())
              {
                _base.StringBuffer.Append(Char.ToLowerInvariant(c));
              }
              else if (c == Symbols.DoubleQuote || c == Symbols.SingleQuote || c == Symbols.LessThan)
              {
                RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                _base.StringBuffer.Append(c);
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.36 After attribute name state
          case AttributeState.AfterName:
            {
              c = _base.SkipSpaces();

              if (c == Symbols.GreaterThan)
              {
                return EmitTag(tag);
              }
              else if (c == Symbols.Equality)
              {
                state = AttributeState.BeforeValue;
              }
              else if (c == Symbols.Solidus)
              {
                return TagSelfClosing(tag);
              }
              else if (c.IsUppercaseAscii())
              {
                _base.StringBuffer.Append(Char.ToLowerInvariant(c));
                state = AttributeState.Name;
              }
              else if (c == Symbols.DoubleQuote || c == Symbols.SingleQuote || c == Symbols.LessThan)
              {
                RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                _base.StringBuffer.Append(c);
                state = AttributeState.Name;
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
                state = AttributeState.Name;
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
                state = AttributeState.Name;
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.37 Before attribute value state
          case AttributeState.BeforeValue:
            {
              c = _base.SkipSpaces();

              if (c == Symbols.DoubleQuote || c == Symbols.SingleQuote)
              {
                state = AttributeState.QuotedValue;
                quote = c;
              }
              else if (c == Symbols.Ampersand)
              {
                state = AttributeState.UnquotedValue;
              }
              else if (c == Symbols.GreaterThan)
              {
                RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
                return EmitTag(tag);
              }
              else if (c == Symbols.LessThan || c == Symbols.Equality || c == Symbols.CurvedQuote)
              {
                RaiseErrorOccurred(HtmlParseError.AttributeValueInvalid);
                _base.StringBuffer.Append(c);
                state = AttributeState.UnquotedValue;
                c = _base.Advance();
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
                state = AttributeState.UnquotedValue;
                c = _base.Advance();
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
                state = AttributeState.UnquotedValue;
                c = _base.Advance();
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.38 Attribute value (double-quoted) state
          // and 8.2.4.39 Attribute value (single-quoted) state
          case AttributeState.QuotedValue:
            {
              c = _base.Advance();

              if (c == quote)
              {
                tag.SetAttributeValue(_base.FlushBuffer());
                state = AttributeState.AfterValue;
              }
              else if (c == Symbols.Ampersand)
              {
                AppendCharacterReference(_base.Advance(), quote);
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.40 Attribute value (unquoted) state
          case AttributeState.UnquotedValue:
            {
              if (c == Symbols.GreaterThan)
              {
                tag.SetAttributeValue(_base.FlushBuffer());
                return EmitTag(tag);
              }
              else if (c.IsSpaceCharacter())
              {
                tag.SetAttributeValue(_base.FlushBuffer());
                state = AttributeState.BeforeName;
              }
              else if (c == Symbols.Ampersand)
              {
                AppendCharacterReference(_base.Advance(), Symbols.GreaterThan);
                c = _base.Advance();
              }
              else if (c == Symbols.Null)
              {
                AppendReplacement();
                c = _base.Advance();
              }
              else if (c == Symbols.DoubleQuote || c == Symbols.SingleQuote || c == Symbols.LessThan || c == Symbols.Equality || c == Symbols.CurvedQuote)
              {
                RaiseErrorOccurred(HtmlParseError.AttributeValueInvalid);
                _base.StringBuffer.Append(c);
                c = _base.Advance();
              }
              else if (c != Symbols.EndOfFile)
              {
                _base.StringBuffer.Append(c);
                c = _base.Advance();
              }
              else
              {
                return NewEof();
              }

              break;
            }

          // See 8.2.4.42 After attribute value (quoted) state
          case AttributeState.AfterValue:
            {
              c = _base.Advance();

              if (c == Symbols.GreaterThan)
              {
                return EmitTag(tag);
              }
              else if (c.IsSpaceCharacter())
              {
                state = AttributeState.BeforeName;
              }
              else if (c == Symbols.Solidus)
              {
                return TagSelfClosing(tag);
              }
              else if (c == Symbols.EndOfFile)
              {
                return NewEof();
              }
              else
              {
                RaiseErrorOccurred(HtmlParseError.AttributeNameExpected);
                _base.Back();
                state = AttributeState.BeforeName;
              }

              break;
            }
        }
      }
    }

    #endregion

    #region Script

    private enum ScriptState : byte
    {
      Normal,
      OpenTag,
      EndTag,
      StartEscape,
      Escaped,
      StartEscapeDash,
      EscapedDash,
      EscapedDashDash,
      EscapedOpenTag,
      EscapedEndTag,
      EscapedNameEndTag,
      StartDoubleEscape,
      EscapedDouble,
      EscapedDoubleDash,
      EscapedDoubleDashDash,
      EscapedDoubleOpenTag,
      EndDoubleEscape
    }

    private HtmlNode ScriptData(Char c)
    {
      var length = _lastStartTag.Length;
      var scriptLength = TagNames.Script.Length;
      var state = ScriptState.Normal;
      var offset = 0;

      while (true)
      {
        switch (state)
        {
          // See 8.2.4.6 Script data state
          case ScriptState.Normal:
            {
              switch (c)
              {
                case Symbols.Null:
                  AppendReplacement();
                  break;

                case Symbols.LessThan:
                  _base.StringBuffer.Append(Symbols.LessThan);
                  state = ScriptState.OpenTag;
                  continue;

                case Symbols.EndOfFile:
                  _base.Back();
                  return NewCharacter();

                default:
                  _base.StringBuffer.Append(c);
                  break;
              }

              c = _base.Advance();
              break;
            }

          // See 8.2.4.17 Script data less-than sign state
          case ScriptState.OpenTag:
            {
              c = _base.Advance();

              if (c == Symbols.Solidus)
              {
                state = ScriptState.EndTag;
              }
              else if (c == Symbols.ExclamationMark)
              {
                state = ScriptState.StartEscape;
              }
              else
              {
                state = ScriptState.Normal;
              }

              break;
            }

          // See 8.2.4.20 Script data escape start state
          case ScriptState.StartEscape:
            {
              _base.StringBuffer.Append(Symbols.ExclamationMark);
              c = _base.Advance();

              if (c == Symbols.Minus)
              {
                state = ScriptState.StartEscapeDash;
              }
              else
              {
                state = ScriptState.Normal;
              }

              break;
            }

          // See 8.2.4.21 Script data escape start dash state
          case ScriptState.StartEscapeDash:
            {
              c = _base.Advance();
              _base.StringBuffer.Append(Symbols.Minus);

              if (c == Symbols.Minus)
              {
                _base.StringBuffer.Append(Symbols.Minus);
                state = ScriptState.EscapedDashDash;
              }
              else
              {
                state = ScriptState.Normal;
              }

              break;
            }

          // See 8.2.4.18 Script data end tag open state
          case ScriptState.EndTag:
            {
              c = _base.Advance();
              offset = _base.StringBuffer.Append(Symbols.Solidus).Length;
              var tag = NewTagClose();

              while (c.IsLetter())
              {
                // See 8.2.4.19 Script data end tag name state
                _base.StringBuffer.Append(c);
                c = _base.Advance();
                var isspace = c.IsSpaceCharacter();
                var isclosed = c == Symbols.GreaterThan;
                var isslash = c == Symbols.Solidus;
                var hasLength = _base.StringBuffer.Length - offset == length;

                if (hasLength && (isspace || isclosed || isslash))
                {
                  var name = _base.StringBuffer.ToString(offset, length);

                  if (name.Isi(_lastStartTag))
                  {
                    if (offset > 2)
                    {
                      _base.Back(3 + length);
                      _base.StringBuffer.Remove(offset - 2, length + 2);
                      return NewCharacter();
                    }

                    _base.StringBuffer.Clear();

                    if (isspace)
                    {
                      tag.Value = _lastStartTag;
                      return ParseAttributes(tag);
                    }
                    else if (isslash)
                    {
                      tag.Value = _lastStartTag;
                      return TagSelfClosing(tag);
                    }
                    else if (isclosed)
                    {
                      tag.Value = _lastStartTag;
                      return EmitTag(tag);
                    }
                  }
                }
              }

              state = ScriptState.Normal;
              break;
            }

          // See 8.2.4.22 Script data escaped state
          case ScriptState.Escaped:
            {
              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  c = _base.Advance();
                  state = ScriptState.EscapedDash;
                  continue;
                case Symbols.LessThan:
                  c = _base.Advance();
                  state = ScriptState.EscapedOpenTag;
                  continue;
                case Symbols.Null:
                  AppendReplacement();
                  break;
                case Symbols.EndOfFile:
                  _base.Back();
                  return NewCharacter();
                default:
                  state = ScriptState.Normal;
                  continue;
              }

              c = _base.Advance();
              break;
            }

          // See 8.2.4.23 Script data escaped dash state
          case ScriptState.EscapedDash:
            {
              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  state = ScriptState.EscapedDashDash;
                  continue;
                case Symbols.LessThan:
                  c = _base.Advance();
                  state = ScriptState.EscapedOpenTag;
                  continue;
                case Symbols.Null:
                  AppendReplacement();
                  break;
                case Symbols.EndOfFile:
                  _base.Back();
                  return NewCharacter();
                default:
                  _base.StringBuffer.Append(c);
                  break;
              }

              c = _base.Advance();
              state = ScriptState.Escaped;
              break;
            }

          // See 8.2.4.24 Script data escaped dash dash state
          case ScriptState.EscapedDashDash:
            {
              c = _base.Advance();

              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  break;
                case Symbols.LessThan:
                  c = _base.Advance();
                  state = ScriptState.EscapedOpenTag;
                  continue;
                case Symbols.GreaterThan:
                  _base.StringBuffer.Append(Symbols.GreaterThan);
                  c = _base.Advance();
                  state = ScriptState.Normal;
                  continue;
                case Symbols.Null:
                  AppendReplacement();
                  c = _base.Advance();
                  state = ScriptState.Escaped;
                  continue;
                case Symbols.EndOfFile:
                  return NewCharacter();
                default:
                  _base.StringBuffer.Append(c);
                  c = _base.Advance();
                  state = ScriptState.Escaped;
                  continue;
              }

              break;
            }

          // See 8.2.4.25 Script data escaped less-than sign state
          case ScriptState.EscapedOpenTag:
            {
              if (c == Symbols.Solidus)
              {
                c = _base.Advance();
                state = ScriptState.EscapedEndTag;
              }
              else if (c.IsLetter())
              {
                offset = _base.StringBuffer.Append(Symbols.LessThan).Length;
                _base.StringBuffer.Append(c);
                state = ScriptState.StartDoubleEscape;
              }
              else
              {
                _base.StringBuffer.Append(Symbols.LessThan);
                state = ScriptState.Escaped;
              }

              break;
            }

          // See 8.2.4.26 Script data escaped end tag open state
          case ScriptState.EscapedEndTag:
            {
              offset = _base.StringBuffer.Append(Symbols.LessThan).Append(Symbols.Solidus).Length;

              if (c.IsLetter())
              {
                _base.StringBuffer.Append(c);
                state = ScriptState.EscapedNameEndTag;
              }
              else
              {
                state = ScriptState.Escaped;
              }

              break;
            }

          // See 8.2.4.27 Script data escaped end tag name state
          case ScriptState.EscapedNameEndTag:
            {
              c = _base.Advance();
              var hasLength = _base.StringBuffer.Length - offset == scriptLength;

              if (hasLength && (c == Symbols.Solidus || c == Symbols.GreaterThan || c.IsSpaceCharacter()) &&
                  _base.StringBuffer.ToString(offset, scriptLength).Isi(TagNames.Script))
              {
                _base.Back(scriptLength + 3);
                _base.StringBuffer.Remove(offset - 2, scriptLength + 2);
                return NewCharacter();
              }
              else if (!c.IsLetter())
              {
                state = ScriptState.Escaped;
              }
              else
              {
                _base.StringBuffer.Append(c);
              }

              break;
            }

          // See 8.2.4.28 Script data double escape start state
          case ScriptState.StartDoubleEscape:
            {
              c = _base.Advance();
              var hasLength = _base.StringBuffer.Length - offset == scriptLength;

              if (hasLength && (c == Symbols.Solidus || c == Symbols.GreaterThan || c.IsSpaceCharacter()))
              {
                var isscript = _base.StringBuffer.ToString(offset, scriptLength).Isi(TagNames.Script);
                _base.StringBuffer.Append(c);
                c = _base.Advance();
                state = isscript ? ScriptState.EscapedDouble : ScriptState.Escaped;
              }
              else if (c.IsLetter())
              {
                _base.StringBuffer.Append(c);
              }
              else
              {
                state = ScriptState.Escaped;
              }

              break;
            }

          // See 8.2.4.29 Script data double escaped state
          case ScriptState.EscapedDouble:
            {
              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  c = _base.Advance();
                  state = ScriptState.EscapedDoubleDash;
                  continue;

                case Symbols.LessThan:
                  _base.StringBuffer.Append(Symbols.LessThan);
                  c = _base.Advance();
                  state = ScriptState.EscapedDoubleOpenTag;
                  continue;

                case Symbols.Null:
                  AppendReplacement();
                  break;

                case Symbols.EndOfFile:
                  RaiseErrorOccurred(HtmlParseError.EOF);
                  _base.Back();
                  return NewCharacter();
              }

              _base.StringBuffer.Append(c);
              c = _base.Advance();
              break;
            }

          // See 8.2.4.30 Script data double escaped dash state
          case ScriptState.EscapedDoubleDash:
            {
              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  state = ScriptState.EscapedDoubleDashDash;
                  continue;

                case Symbols.LessThan:
                  _base.StringBuffer.Append(Symbols.LessThan);
                  c = _base.Advance();
                  state = ScriptState.EscapedDoubleOpenTag;
                  continue;

                case Symbols.Null:
                  RaiseErrorOccurred(HtmlParseError.Null);
                  c = Symbols.Replacement;
                  break;

                case Symbols.EndOfFile:
                  RaiseErrorOccurred(HtmlParseError.EOF);
                  _base.Back();
                  return NewCharacter();
              }

              state = ScriptState.EscapedDouble;
              break;
            }

          // See 8.2.4.31 Script data double escaped dash dash state
          case ScriptState.EscapedDoubleDashDash:
            {
              c = _base.Advance();

              switch (c)
              {
                case Symbols.Minus:
                  _base.StringBuffer.Append(Symbols.Minus);
                  break;

                case Symbols.LessThan:
                  _base.StringBuffer.Append(Symbols.LessThan);
                  c = _base.Advance();
                  state = ScriptState.EscapedDoubleOpenTag;
                  continue;

                case Symbols.GreaterThan:
                  _base.StringBuffer.Append(Symbols.GreaterThan);
                  c = _base.Advance();
                  state = ScriptState.Normal;
                  continue;

                case Symbols.Null:
                  AppendReplacement();
                  c = _base.Advance();
                  state = ScriptState.EscapedDouble;
                  continue;

                case Symbols.EndOfFile:
                  RaiseErrorOccurred(HtmlParseError.EOF);
                  _base.Back();
                  return NewCharacter();

                default:
                  _base.StringBuffer.Append(c);
                  c = _base.Advance();
                  state = ScriptState.EscapedDouble;
                  continue;
              }

              break;
            }

          // See 8.2.4.32 Script data double escaped less-than sign state
          case ScriptState.EscapedDoubleOpenTag:
            {
              if (c == Symbols.Solidus)
              {
                offset = _base.StringBuffer.Append(Symbols.Solidus).Length;
                state = ScriptState.EndDoubleEscape;
              }
              else
              {
                state = ScriptState.EscapedDouble;
              }

              break;
            }

          // See 8.2.4.33 Script data double escape end state
          case ScriptState.EndDoubleEscape:
            {
              c = _base.Advance();
              var hasLength = _base.StringBuffer.Length - offset == scriptLength;

              if (hasLength && (c.IsSpaceCharacter() || c == Symbols.Solidus || c == Symbols.GreaterThan))
              {
                var isscript = _base.StringBuffer.ToString(offset, scriptLength).Isi(TagNames.Script);
                _base.StringBuffer.Append(c);
                c = _base.Advance();
                state = isscript ? ScriptState.Escaped : ScriptState.EscapedDouble;
              }
              else if (c.IsLetter())
              {
                _base.StringBuffer.Append(c);
              }
              else
              {
                state = ScriptState.EscapedDouble;
              }

              break;
            }
        }
      }
    }

    #endregion

    #region Tokens

    private HtmlNode NewCharacter()
    {
      var content = _base.FlushBuffer();
      return new HtmlText(_position, content);
    }

    private HtmlComment NewComment()
    {
      var content = _base.FlushBuffer();
      return new HtmlComment(_position, content);
    }

    private HtmlNode NewEof(Boolean acceptable = false)
    {
      if (!acceptable)
      {
        RaiseErrorOccurred(HtmlParseError.EOF);
      }

      return new HtmlEndOfFile(_position);
    }

    private HtmlDoctype NewDoctype(Boolean quirksForced)
    {
      return new HtmlDoctype(quirksForced, _position);
    }

    private HtmlTagNode NewTagOpen()
    {
      return new HtmlStartTag(_position);
    }

    private HtmlTagNode NewTagClose()
    {
      return new HtmlEndTag(_position);
    }

    #endregion

    #region Helpers

    private void RaiseErrorOccurred(HtmlParseError code)
    {
      RaiseErrorOccurred(code, _base.GetCurrentPosition());
    }

    private void AppendReplacement()
    {
      RaiseErrorOccurred(HtmlParseError.Null);
      _base.StringBuffer.Append(Symbols.Replacement);
    }

    private HtmlNode CreateIfAppropriate(Char c)
    {
      var isspace = c.IsSpaceCharacter();
      var isclosed = c == Symbols.GreaterThan;
      var isslash = c == Symbols.Solidus;
      var hasLength = _base.StringBuffer.Length == _lastStartTag.Length;

      if (hasLength && (isspace || isclosed || isslash) && _base.StringBuffer.ToString().Is(_lastStartTag))
      {
        var tag = NewTagClose();
        _base.StringBuffer.Clear();

        if (isspace)
        {
          tag.Value = _lastStartTag;
          return ParseAttributes(tag);
        }
        else if (isslash)
        {
          tag.Value = _lastStartTag;
          return TagSelfClosing(tag);
        }
        else if (isclosed)
        {
          tag.Value = _lastStartTag;
          return EmitTag(tag);
        }
      }

      return null;
    }

    private HtmlNode EmitTag(HtmlTagNode tag)
    {
      State = HtmlParseMode.PCData;

      var start = tag as HtmlStartTag;
      if (start != null)
      {
        var attributes = start.Attributes;
        for (var i = attributes.Count - 1; i > 0; i--)
        {
          for (var j = i - 1; j >= 0; j--)
          {
            if (attributes[j].Key == attributes[i].Key)
            {
              attributes.RemoveAt(i);
              RaiseErrorOccurred(HtmlParseError.AttributeDuplicateOmitted, tag.Position);
              break;
            }
          }
        }

        if (tag.Value.Is(TagNames.Script))
          State = HtmlParseMode.Script;

        _lastStartTag = tag.Value;
      }

      return tag;
    }

    public IEnumerator<HtmlNode> GetEnumerator()
    {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    bool IEnumerator.MoveNext()
    {
      return Read();
    }

    void IEnumerator.Reset()
    {
      throw new NotSupportedException();
    }

    bool IXmlLineInfo.HasLineInfo()
    {
      return true;
    }

    protected override void Dispose(bool disposing)
    {
      _base.Dispose();
    }
    #endregion

    #region XmlReader

    private int _attrIndex;
    private int _depth = 0;

    public override int AttributeCount
    {
      get
      {
        var tag = _current as HtmlStartTag;
        if (tag == null)
          return 0;
        return tag.Attributes.Count;
      }
    }

    public override string BaseURI
    {
      get
      {
        return string.Empty;
      }
    }

    public override int Depth
    {
      get
      {
        return 0;
      }
    }

    public override bool EOF
    {
      get
      {
        return _current != null && _current.Type == HtmlTokenType.EndOfFile;
      }
    }

    public override bool IsEmptyElement
    {
      get
      {
        var tag = _current as HtmlStartTag;
        if (tag == null)
          return false;
        return tag.IsEmpty;
      }
    }

    public override string LocalName { get { return GetName().Last(); } }

    private readonly static string[] _emptyStringArray = new string[] { "" };

    private string[] GetName()
    {
      if (_attrIndex >= 0)
        return ((HtmlStartTag)_current).Attributes[_attrIndex].Key.Split(':');
      if (_attrIndex < -1)
        return _emptyStringArray;
      if (_current == null)
        return _emptyStringArray;
      if (_current.Type == HtmlTokenType.EndTag
        || _current.Type == HtmlTokenType.StartTag
        || _current.Type == HtmlTokenType.Doctype)
        return _current.Value.Split(':');
      return _emptyStringArray;
    }

    public override string NamespaceURI
    {
      get
      {
        return string.Empty;
      }
    }

    private XmlNameTable _table = new NameTable();
    public override XmlNameTable NameTable
    {
      get { return _table; }
    }

    public override XmlNodeType NodeType
    {
      get
      {
        if (_current == null)
          return XmlNodeType.None;
        if (_attrIndex >= 0)
          return XmlNodeType.Attribute;
        if (_attrIndex < -1)
          return XmlNodeType.Text;
        switch (_current.Type)
        {
          case HtmlTokenType.Comment:
            return XmlNodeType.Comment;
          case HtmlTokenType.Doctype:
            return XmlNodeType.DocumentType;
          case HtmlTokenType.EndTag:
            return XmlNodeType.EndElement;
          case HtmlTokenType.StartTag:
            return XmlNodeType.Element;
          case HtmlTokenType.Text:
            var val = _current.Value ?? "";
            for (var i = 0; i < val.Length; i++)
            {
              if (!char.IsWhiteSpace(val[i]))
                return XmlNodeType.Text;
            }
            return _depth > 0 ? XmlNodeType.SignificantWhitespace : XmlNodeType.Whitespace;
        }
        return XmlNodeType.None;
      }
    }

    public override string Prefix
    {
      get
      {
        //var name = GetName();
        //if (name.Length > 1)
        //  return name[0];
        return string.Empty;
      }
    }

    public override ReadState ReadState
    {
      get
      {
        if (_current == null)
          return ReadState.Initial;
        if (_current.Type == HtmlTokenType.EndOfFile)
          return ReadState.EndOfFile;
        return ReadState.Interactive;
      }
    }

    public override string Value
    {
      get
      {
        if (_attrIndex >= 0)
          return ((HtmlStartTag)_current).Attributes[_attrIndex].Value;
        if (_attrIndex < -1)
          return ((HtmlStartTag)_current).Attributes[_attrIndex * -1 - 2].Value;
        if (_current == null)
          return string.Empty;
        switch (_current.Type)
        {
          case HtmlTokenType.Comment:
          case HtmlTokenType.Text:
            return _current.Value;
        }
        return string.Empty;
      }
    }


    public override string GetAttribute(int i)
    {
      var tag = _current as HtmlStartTag;
      if (tag == null)
        return string.Empty;
      return tag.Attributes[i].Value;
    }

    public override string GetAttribute(string name)
    {
      return GetAttribute(name, null);
    }

    public override string GetAttribute(string name, string namespaceURI)
    {
      var tag = _current as HtmlStartTag;
      if (tag == null)
        return string.Empty;
      return tag[name];
    }

    public override string LookupNamespace(string prefix)
    {
      return string.Empty;
    }

    public override bool MoveToAttribute(string name)
    {
      return MoveToAttribute(name, null);
    }

    public override bool MoveToAttribute(string name, string ns)
    {
      var tag = _current as HtmlStartTag;
      if (tag != null)
      {
        for (var i = 0; i < tag.Attributes.Count; i++)
        {
          if (string.Equals(tag.Attributes[i].Key, name, StringComparison.OrdinalIgnoreCase))
          {
            _attrIndex = i;
            return true;
          }
        }
      }
      _attrIndex = -1;
      return false;
    }

    public override bool MoveToElement()
    {
      _attrIndex = -1;
      return _current != null && _current.Type == HtmlTokenType.StartTag;
    }

    public override bool MoveToFirstAttribute()
    {
      var tag = _current as HtmlStartTag;
      if (tag == null || tag.Attributes.Count < 1)
      {
        _attrIndex = -1;
        return false;
      }
      _attrIndex = 0;
      return true;
    }

    public override bool MoveToNextAttribute()
    {
      var tag = _current as HtmlStartTag;
      if (tag == null || _attrIndex + 1 >= tag.Attributes.Count)
      {
        _attrIndex = -1;
        return false;
      }
      _attrIndex++;
      return true;
    }

    public override bool ReadAttributeValue()
    {
      if (_attrIndex < -1)
      {
        _attrIndex = _attrIndex * -1 - 2;
        return false;
      }
      else
      {
        _attrIndex = _attrIndex * -1 - 2;
        return true;
      }
    }

    public override void ResolveEntity()
    {
      // Do nothing
    }

    public IEnumerable<XElement> Elements(Func<HtmlStartTag, bool> predicate = null)
    {
      predicate = predicate ?? (n => true);

      MoveToContent();
      var continueNoRead = true;
      while (continueNoRead || Read())
      {
        continueNoRead = false;
        if (Current.Type == HtmlTokenType.StartTag && predicate((HtmlStartTag)Current))
        {
          var el = XNode.ReadFrom(this) as XElement;
          if (el != null)
            yield return el;
          continueNoRead = true;
        }
      }
    }

#if NET35
    public override bool HasValue
    {
      get { return true; }
    }
#endif
#if !PORTABLE
    public override void Close()
    {
      
    }
#endif
    #endregion
  }
}
