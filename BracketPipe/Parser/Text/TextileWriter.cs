using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BracketPipe
{
  public class TextileWriter : XmlWriter
  {
    private TextWriter _writer;
    private Stack<HtmlStartTag> _nodes = new Stack<HtmlStartTag>();
    private int _ignoreDepth = int.MaxValue;
    private int _boldDepth = int.MaxValue;
    private int _italicDepth = int.MaxValue;
    private StringBuilder _buffer;
    private InternalState _state = InternalState.Start;
    private MinifyState _minify = MinifyState.LastCharWasSpace;
    private MarkdownWriterSettings _settings;
    private List<string> _linePrefix = new List<string>();
    private PreserveState _preserveWhitespace = PreserveState.None;
    private bool _outputStarted;
    private bool _lastWasMultiline;

    public TextileWriter(TextWriter writer) : this(writer, new MarkdownWriterSettings()) { }
    public TextileWriter(TextWriter writer, MarkdownWriterSettings settings)
    {
      _writer = writer;
      _settings = settings ?? new MarkdownWriterSettings();
    }

    public override WriteState WriteState
    {
      get
      {
        return (WriteState)_state;
      }
    }

    public override void Flush()
    {
      _writer.Flush();
    }

    public override string LookupPrefix(string ns)
    {
      throw new NotImplementedException();
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      WriteInternal(Convert.ToBase64String(buffer, index, count));
    }

    public override void WriteCData(string text)
    {
      WriteInternal(text);
    }

    public override void WriteCharEntity(char ch)
    {
      if (XmlCharType.IsSurrogate((int)ch))
        throw new ArgumentException("Invalid surrogate: Missing low character");

      switch (ch)
      {
        case '<':
          WriteEntityRef("lt");
          break;
        case '>':
          WriteEntityRef("gt");
          break;
        case '&':
          WriteEntityRef("amp");
          break;
        case '"':
          WriteEntityRef("quot");
          break;
        case '\'':
          WriteEntityRef("apos");
          break;
        default:
          var num = (int)ch;
          if (num == 160)
          {
            WriteEntityRef("nbsp");
          }
          else
          {
            var text = num.ToString("X", NumberFormatInfo.InvariantInfo);
            WriteEntityRef("#x" + text);
          }
          break;
      }
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      WriteInternal(new string(buffer, index, count));
    }

    public override void WriteComment(string text)
    {
      // Do nothing
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      // Do nothing
    }

    public override void WriteEndAttribute()
    {
      if (_nodes.Count < 1)
        throw new InvalidOperationException();
      _nodes.Peek().SetAttributeValue(this._buffer.ToPool());
      this._buffer = null;
      _state = InternalState.Element;
    }

    public override void WriteEndDocument()
    {
      // Do nothing
    }

    public override void WriteEndElement()
    {
      WriteStartElementEnd();
      var start = _nodes.Pop();
      if (_ignoreDepth > _nodes.Count)
        _ignoreDepth = int.MaxValue;

      string buffer;
      switch (start.Value)
      {
        case "a":
          if (start.TryGetValue("href", out buffer))
          {
            var href = buffer;
            if (start.TryGetValue("title", out buffer))
            {
              _writer.Write('(');
              _writer.Write(buffer);
              _writer.Write(')');
            }
            _writer.Write('"');
            _writer.Write(':');
            _writer.Write(href);
            _writer.Write(']');
          }
          else if (start.Attributes.Count > 0)
          {
            _writer.Write("</a>");
            _preserveWhitespace = PreserveState.None;
          }
          _minify = MinifyState.Compressed;
          break;
        case "acronym":
          if (start.TryGetValue("title", out buffer))
          {
            _writer.Write('(');
            _writer.Write(buffer);
            _writer.Write(')');
          }
          break;
        case "b":
          if (_boldDepth > _nodes.Count)
          {
            _writer.Write('*');
            _writer.Write('*');
            _boldDepth = int.MaxValue;
          }
          break;
        case "strong":
          if (_boldDepth > _nodes.Count)
          {
            _writer.Write('*');
            _boldDepth = int.MaxValue;
          }
          break;
        case "cite":
          _writer.Write('?');
          _writer.Write('?');
          break;
        case "code":
          if (_preserveWhitespace == PreserveState.None)
            _writer.Write('@');
          break;
        case "del":
          _writer.Write('-');
          _writer.Write(']');
          break;
        case "em":
          if (_italicDepth > _nodes.Count)
          {
            _writer.Write('_');
            _italicDepth = int.MaxValue;
          }
          break;
        case "i":
          if (_italicDepth > _nodes.Count)
          {
            _writer.Write('_');
            _writer.Write('_');
            _italicDepth = int.MaxValue;
          }
          break;
        case "ins":
          _writer.Write('+');
          _writer.Write(']');
          break;
        case "blockquote":
        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
        case "p":
          EndBlock();
          break;
        case "ol":
        case "ul":
          EndBlock();
          if (_linePrefix.Count > 0)
            _linePrefix.RemoveAt(_linePrefix.Count - 1);
          break;
        case "pre":
          EndBlock();
          _preserveWhitespace = PreserveState.None;
          _lastWasMultiline = true;
          break;
        case "sub":
          _writer.Write('~');
          _writer.Write(']');
          break;
        case "sup":
          _writer.Write('^');
          _writer.Write(']');
          break;
      }
    }

    public override void WriteEntityRef(string name)
    {
      WriteInternal("&");
      WriteInternal(name);
      WriteInternal(";");
    }

    public override void WriteFullEndElement()
    {
      WriteEndElement();
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      throw new NotSupportedException();
    }

    public override void WriteRaw(string data)
    {
      _writer.Write(data);
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      _writer.Write(buffer, index, count);
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (_nodes.Count < 1)
        throw new InvalidOperationException();
      _nodes.Peek().AddAttribute(localName);
      this._buffer = Pool.NewStringBuilder();
      _state = InternalState.Attribute;
    }

    public override void WriteStartDocument()
    {
      // Do nothing
    }

    public override void WriteStartDocument(bool standalone)
    {
      // Do nothing
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      WriteStartElementEnd(localName);
      var start = new HtmlStartTag(localName.ToLowerInvariant());
      _nodes.Push(start);
      _state = InternalState.Element;
      _lastWasMultiline = false;
    }

    private void WritePrefix()
    {
      var textWritten = false;
      for (var i = 0; i < _linePrefix.Count; i++)
      {
        _writer.Write(_linePrefix[i]);
        textWritten = textWritten || _linePrefix[i].Length > 0;
      }
      if (textWritten)
        _minify = MinifyState.SpaceNeeded;
    }

    public override void WriteString(string text)
    {
      if (_lastWasMultiline && text.Trim().Length > 0)
      {
        StartBlock("p", new HtmlStartTag("p"));
        _lastWasMultiline = false;
      }
      WriteInternal(text);
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      if (!XmlCharType.IsLowSurrogate((int)lowChar) || !XmlCharType.IsHighSurrogate((int)highChar))
        throw new InvalidOperationException("Invalid surrogate pair");

      var num = XmlCharType.CombineSurrogateChar((int)lowChar, (int)highChar);
      WriteInternal("&#x");
      WriteInternal(num.ToString("X", NumberFormatInfo.InvariantInfo));
      WriteInternal(";");
    }

    public override void WriteWhitespace(string ws)
    {
      WriteInternal(ws);
    }

    private void WriteInternal(string value)
    {
      WriteStartElementEnd();

      if (_ignoreDepth <= _nodes.Count)
        return;

      if (_preserveWhitespace == PreserveState.HtmlEncoding)
      {
        WriteEscaped(value);
        _outputStarted = true;
        return;
      }

      if (this._buffer != null)
      {
        this._buffer.Append(value);
      }
      else
      {
        for (var i = 0; i < value.Length; i++)
        {
          if (_preserveWhitespace == PreserveState.InternalLineFeed)
          {
            _writer.Write(_settings.NewLineChars);
            WritePrefix();
            _preserveWhitespace = PreserveState.Preserve;
          }

          if (char.IsWhiteSpace(value[i]) && value[i] != '\u00A0')
          {
            if (_preserveWhitespace == PreserveState.BeforeContent
              && value[i] == Symbols.LineFeed)
            {
              _preserveWhitespace = PreserveState.Preserve;
            }
            else if (_preserveWhitespace != PreserveState.None)
            {
              if (value[i] == Symbols.LineFeed)
              {
                _preserveWhitespace = PreserveState.InternalLineFeed;
              }
              else
              {
                _writer.Write(value[i]);
              }
            }
            else
            {
              if (_minify != MinifyState.LastCharWasSpace
                && _minify != MinifyState.BlockEnd)
                _minify = MinifyState.SpaceNeeded;
            }
          }
          else
          {
            switch (_minify)
            {
              case MinifyState.BlockEnd:
                _writer.Write(_settings.NewLineChars);
                _writer.Write(_settings.NewLineChars);
                _minify = MinifyState.Compressed;
                break;
              case MinifyState.SpaceNeeded:
                _writer.Write(' ');
                _minify = MinifyState.Compressed;
                break;
              case MinifyState.LastCharWasSpace:
                _minify = MinifyState.Compressed;
                break;
            }

            if (_preserveWhitespace == PreserveState.BeforeContent)
            {
              _preserveWhitespace = PreserveState.Preserve;
            }

            if (_preserveWhitespace == PreserveState.None)
            {
              switch (value[i])
              {
                case '\u201C':
                case '\u201D':
                  _writer.Write('"');
                  break;
                case '\u2018':
                case '\u2019':
                  _writer.Write('\'');
                  break;
                case '\u2014':
                  _writer.Write("--");
                  break;
                case '\u2013':
                  _writer.Write("-");
                  break;
                case '\u2026':
                  _writer.Write("...");
                  break;
                case '\u00D7':
                  _writer.Write('x');
                  break;
                case '\u00AE':
                  _writer.Write("(r)");
                  break;
                case '\u2122':
                  _writer.Write("(tm)");
                  break;
                case '\u00A9':
                  _writer.Write("(c)");
                  break;
                default:
                  _writer.Write(value[i]);
                  break;
              }
            }
            else
            {
              _writer.Write(value[i]);
            }
          }
        }
        _outputStarted = true;
      }
    }
    private void WriteStartElementEnd(string nextTag = null)
    {
      if (_nodes.Count > 0 && _state == InternalState.Element)
      {
        var start = _nodes.Peek();
        string buffer;
        switch (start.Value)
        {
          case "a":
            buffer = null;
            StartInline();
            if (start.Attributes.Count > 0
              && (!start.TryGetValue("href", out buffer)
                || string.IsNullOrEmpty(buffer)))
            {
              _writer.Write("<a ");
              foreach (var attr in start)
              {
                _writer.Write(attr.Key);
                if (!string.IsNullOrEmpty(attr.Value))
                {
                  _writer.Write('=');
                  _writer.Write('"');
                  WriteEscaped(attr.Value);
                  _writer.Write('"');
                }
              }
              _writer.Write('>');
              _preserveWhitespace = PreserveState.HtmlEncoding;
            }
            else if (!string.IsNullOrEmpty(buffer))
            {
              _writer.Write('[');
              _writer.Write('"');
              WriteAttributes(start, _writer);
            }
            break;
          case "blockquote":
            StartBlock("bq", start);
            if (start.TryGetValue("cite", out buffer))
            {
              _writer.Write(':');
              _writer.Write(buffer);
            }
            break;
          case "b":
            StartInline();
            if (_boldDepth > _nodes.Count)
            {
              _boldDepth = _nodes.Count;
              _writer.Write('*');
              _writer.Write('*');
              WriteAttributes(start, _writer);
            }
            break;
          case "strong":
            StartInline();
            if (_boldDepth > _nodes.Count)
            {
              _boldDepth = _nodes.Count;
              _writer.Write('*');
              WriteAttributes(start, _writer);
            }
            break;
          case "br":
            _minify = MinifyState.LastCharWasSpace;
            switch (_preserveWhitespace)
            {
              case PreserveState.BeforeContent:
                _preserveWhitespace = PreserveState.Preserve;
                break;
              case PreserveState.InternalLineFeed:
                _writer.Write(_settings.NewLineChars);
                WritePrefix();
                _preserveWhitespace = PreserveState.InternalLineFeed;
                break;
              case PreserveState.None:
                _writer.Write(_settings.NewLineChars);
                WritePrefix();
                break;
              default:
                _preserveWhitespace = PreserveState.InternalLineFeed;
                break;
            }
            break;
          case "cite":
            StartInline();
            _writer.Write('?');
            _writer.Write('?');
            WriteAttributes(start, _writer);
            break;
          case "code":
            if (_preserveWhitespace == PreserveState.None)
            {
              StartInline();
              _writer.Write('@');
              WriteAttributes(start, _writer);
            }
            break;
          case "del":
            StartInline();
            _writer.Write('[');
            _writer.Write('-');
            break;
          case "div":
            StartBlock("div", start);
            break;
          case "em":
            StartInline();
            if (_italicDepth > _nodes.Count)
            {
              _italicDepth = _nodes.Count;
              _writer.Write('_');
              WriteAttributes(start, _writer);
            }
            break;
          case "i":
            StartInline();
            if (_italicDepth > _nodes.Count)
            {
              _italicDepth = _nodes.Count;
              _writer.Write('_');
              _writer.Write('_');
              WriteAttributes(start, _writer);
            }
            break;
          case "h1":
            StartBlock("h1", start);
            break;
          case "h2":
            StartBlock("h2", start);
            break;
          case "h3":
            StartBlock("h3", start);
            break;
          case "h4":
            StartBlock("h4", start);
            break;
          case "h5":
            StartBlock("h5", start);
            break;
          case "h6":
            StartBlock("h6", start);
            break;
          case "img":
            StartInline();
            _writer.Write('!');
            WriteAttributes(start, _writer);
            _writer.Write(start["src"]);
            if (start.TryGetValue("alt", out buffer))
            {
              _writer.Write('(');
              _writer.Write(buffer);
              _writer.Write(')');
            }
            _writer.Write('!');
            _minify = MinifyState.Compressed;
            break;
          case "ins":
            StartInline();
            _writer.Write('[');
            _writer.Write('+');
            break;
          case "hr":
            StartBlock("---", start);
            EndBlock();
            _writer.Write(_settings.NewLineChars);
            _minify = MinifyState.LastCharWasSpace;
            break;
          case "li":
            if (_outputStarted)
              _writer.Write(_settings.NewLineChars);
            WritePrefix();
            break;
          case "p":
            StartBlock("", start);
            break;
          case "pre":
            if (nextTag == "code")
              StartBlock("bc", start, true);
            else
              StartBlock("pre", start, true);
            _preserveWhitespace = PreserveState.BeforeContent;
            break;
          case "span":
            _writer.Write('?');
            WriteAttributes(start, _writer);
            break;
          case "sub":
            StartInline();
            _writer.Write('[');
            _writer.Write('~');
            break;
          case "sup":
            StartInline();
            _writer.Write('[');
            _writer.Write('^');
            break;
          case "ol":
            StartList("#");
            break;
          case "ul":
            StartList("*");
            break;
          default:
            if (_settings.ShouldSkipElement(start))
            {
              _ignoreDepth = _nodes.Count;
            }
            break;
        }
        _state = InternalState.Content;
      }
    }

    private void WriteAttributes(HtmlStartTag start, TextWriter writer)
    {
      string buffer;
      var inParen = false;
      if (start.TryGetValue("class", out buffer))
      {
        writer.Write('(');
        writer.Write(buffer);
        inParen = true;
      }
      if (start.TryGetValue("id", out buffer))
      {
        if (!inParen)
          writer.Write('(');
        writer.Write('#');
        writer.Write(buffer);
        inParen = true;
      }
      if (inParen)
      {
        writer.Write(')');
      }
      else if (start.TryGetValue("style", out buffer))
      {
        writer.Write('{');
        writer.Write(buffer.Trim().TrimEnd(';'));
        writer.Write('}');
      }
      else if (start.TryGetValue("lang", out buffer))
      {
        writer.Write('[');
        writer.Write(buffer);
        writer.Write(']');
      }
    }

    private void StartInline()
    {
      if (_minify == MinifyState.SpaceNeeded)
      {
        _writer.Write(' ');
        _minify = MinifyState.LastCharWasSpace;
      }
    }
    private void StartList(string prefix)
    {
      if (_minify == MinifyState.Compressed
        || _minify == MinifyState.SpaceNeeded
        || _minify == MinifyState.BlockEnd)
      {
        _writer.Write(_settings.NewLineChars);
        _minify = MinifyState.LastCharWasSpace;
      }
      _linePrefix.Add(prefix ?? string.Empty);
    }
    private void StartBlock(string prefix, HtmlStartTag start, bool multiline = false)
    {
      if (_minify == MinifyState.Compressed
        || _minify == MinifyState.SpaceNeeded
        || _minify == MinifyState.BlockEnd)
      {
        _writer.Write(_settings.NewLineChars);
        _writer.Write(_settings.NewLineChars);
        _minify = MinifyState.LastCharWasSpace;
      }

      if (prefix == "" && (_lastWasMultiline || multiline || start.Any(k => k.Key == "class" || k.Key == "id" || k.Key == "style" || k.Key == "lang")))
        prefix = "p";
      _writer.Write(prefix);
      WriteAttributes(start, _writer);

      if (prefix != "")
      {
        _writer.Write('.');
        _minify = MinifyState.SpaceNeeded;
      }
      if (multiline)
        _writer.Write('.');
      _outputStarted = true;
    }
    private void EndBlock()
    {
      _minify = MinifyState.BlockEnd;
    }

    private void WriteEscaped(string text)
    {
      for (var i = 0; i < text.Length; i++)
      {
        switch (text[i])
        {
          case Symbols.Ampersand: _writer.Write("&amp;"); break;
          case Symbols.NoBreakSpace: _writer.Write("&nbsp;"); break;
          case Symbols.GreaterThan: _writer.Write("&gt;"); break;
          case Symbols.LessThan: _writer.Write("&lt;"); break;
          case Symbols.DoubleQuote:
            if (_settings.QuoteChar == Symbols.DoubleQuote)
              _writer.Write("&quot;");
            else
              _writer.Write('"');
            break;
          case Symbols.SingleQuote:
            if (_settings.QuoteChar == Symbols.SingleQuote)
              _writer.Write("&apos;");
            else
              _writer.Write("'");
            break;
          case Symbols.LineFeed: _writer.Write(_settings.NewLineChars); break;
          default: _writer.Write(text[i]); break;
        }
      }
    }


#if !PORTABLE
    public override void Close()
    {
      
    }
#endif

    private enum InternalState
    {
      Start = 0,
      Element = 2,
      Attribute = 3,
      Content = 4,
    }

    private enum PreserveState : byte
    {
      None,
      HtmlEncoding,
      BeforeContent,
      Preserve,
      InternalLineFeed
    }

    private enum MinifyState : byte
    {
      Compressed,
      LastCharWasSpace,
      SpaceNeeded,
      InlineStartAfterSpace,
      BlockEnd
    }
  }
}
