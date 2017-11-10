using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace BracketPipe
{
  public class PlainTextWriter : XmlWriter
  {
    private TextWriter _writer;
    private Stack<HtmlStartTag> _nodes = new Stack<HtmlStartTag>();
    private int _ignoreDepth = int.MaxValue;
    private StringBuilder _attributeBuffer;
    private InternalState _state = InternalState.Start;
    private MinifyState _minify = MinifyState.LastCharWasSpace;
    private List<string> _linePrefix = new List<string>();
    private PreserveState _preserveWhitespace = PreserveState.None;
    private bool _outputStarted;
    private int _pos = 0;
    private TextWriterSettings _settings;

    private StringBuilder _buffer;
    private List<KeyValuePair<string, string>> _metadata = new List<KeyValuePair<string, string>>();
    private List<PlainTextLink> _links = new List<PlainTextLink>();

    public ICollection<PlainTextLink> Links { get { return _links; } }
    public ICollection<KeyValuePair<string, string>> Metadata { get { return _metadata; } }

    public PlainTextWriter(TextWriter writer) : this(writer, new TextWriterSettings()) { }

    public PlainTextWriter(TextWriter writer, TextWriterSettings settings)
    {
      _writer = writer;
      _settings = settings ?? new TextWriterSettings();
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
      _nodes.Peek().SetAttributeValue(this._attributeBuffer.ToPool());
      this._attributeBuffer = null;
      _state = InternalState.Element;
    }

    public override void WriteEndDocument()
    {
      // Do nothing
    }

    public override void WriteEndElement()
    {
      WriteStartElementEnd();
      if (_nodes.Count <= 0)
        return;
        
      var start = _nodes.Pop();
      if (_ignoreDepth > _nodes.Count)
        _ignoreDepth = int.MaxValue;

      switch (start.Value)
      {
        case "a":
          _minify = MinifyState.Compressed;
          if (_buffer != null)
          {
            var text = _buffer.ToPool();
            if (!string.IsNullOrEmpty(start["href"]))
            {
              _links.Add(new PlainTextLink()
              {
                Href = start["href"],
                Offset = _pos - text.Length,
                Tag = start,
                Text = text
              });
            }
          }
          _buffer = null;
          break;
        case "blockquote":
        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
        case "ol":
        case "p":
        case "ul":
          EndBlock();
          break;
        case "pre":
          EndBlock();
          _preserveWhitespace = PreserveState.None;
          break;
        case "title":
          _metadata.Add(new KeyValuePair<string, string>("title", _buffer.ToPool()));
          _buffer = null;
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
      Write(data);
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
      this._attributeBuffer = Pool.NewStringBuilder();
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
      WriteStartElementEnd();
      var start = new HtmlStartTag(localName.ToLowerInvariant());
      _nodes.Push(start);
      switch (start.Value)
      {
        case "b":
        case "strong":
          StartInline();
          break;
        case "blockquote":
          StartBlock("");
          break;
        case "br":
          _minify = MinifyState.LastCharWasSpace;
          switch (_preserveWhitespace)
          {
            case PreserveState.BeforeContent:
              _preserveWhitespace = PreserveState.Preserve;
              break;
            case PreserveState.InternalLineFeed:
              Write(Environment.NewLine);
              WritePrefix();
              _preserveWhitespace = PreserveState.InternalLineFeed;
              break;
            case PreserveState.None:
              Write(Environment.NewLine);
              WritePrefix();
              break;
            default:
              _preserveWhitespace = PreserveState.InternalLineFeed;
              break;
          }
          break;
        case "code":
          if (_preserveWhitespace == PreserveState.None)
          {
            StartInline();
          }
          break;
        case "em":
        case "i":
          StartInline();
          break;
        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
          StartBlock("");
          break;
        case "title":
          _buffer = Pool.NewStringBuilder();
          _ignoreDepth = _nodes.Count;
          break;
        case "hr":
          StartBlock("* * *");
          EndBlock();
          Write(Environment.NewLine);
          _minify = MinifyState.LastCharWasSpace;
          break;
        case "li":
          if (_outputStarted)
            Write(Environment.NewLine);
          if (_linePrefix.Count > 0
            && _linePrefix[_linePrefix.Count - 1].Length > 0
            && char.IsDigit(_linePrefix[_linePrefix.Count - 1][0]))
          {
            var value = _linePrefix[_linePrefix.Count - 1];
            value = string.Format("{0}. ", int.Parse(value.Substring(0, value.Length - 2)) + 1);
            _linePrefix[_linePrefix.Count - 1] = value;
          }
          WritePrefix();
          _minify = MinifyState.LastCharWasSpace;
          break;
        case "p":
          StartBlock("");
          break;
        case "pre":
          StartBlock("    ");
          _preserveWhitespace = PreserveState.BeforeContent;
          break;
        case "ol":
          StartList("0. ");
          break;
        case "ul":
          StartList("- ");
          break;
      }
      _state = InternalState.Element;
    }

    private void WritePrefix()
    {
      for (var i = 0; i < _linePrefix.Count; i++)
      {
        // Only indent on nested lists
        if (i < _linePrefix.Count - 1
          && _linePrefix[i].Length > 0
          && (_linePrefix[i][0] == '-' || char.IsDigit(_linePrefix[i][0])))
          Write(new string(' ', _linePrefix[i].Length));
        else
          Write(_linePrefix[i]);
      }
    }

    public override void WriteString(string text)
    {
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

    private void Write(string value)
    {
      _writer.Write(value);
      if (_buffer != null)
        _buffer.Append(value);
      _pos += (value ?? "").Length;
    }
    private void Write(char value)
    {
      _writer.Write(value);
      if (_buffer != null)
        _buffer.Append(value);
      _pos++;
    }
    private void WriteInternal(string value)
    {
      WriteStartElementEnd();

      if (_ignoreDepth <= _nodes.Count)
      {
        if (this._attributeBuffer != null)
          this._attributeBuffer.Append(value);
        if (_buffer != null)
          _buffer.Append(value);
        return;
      }

      if (_preserveWhitespace == PreserveState.HtmlEncoding)
      {
        WriteEscaped(value);
        _outputStarted = true;
        return;
      }

      if (this._attributeBuffer != null)
      {
        this._attributeBuffer.Append(value);
      }
      else
      {
        for (var i = 0; i < value.Length; i++)
        {
          if (_preserveWhitespace == PreserveState.InternalLineFeed)
          {
            Write(Environment.NewLine);
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
                Write(value[i]);
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
                Write(Environment.NewLine);
                Write(Environment.NewLine);
                _minify = MinifyState.Compressed;
                break;
              case MinifyState.SpaceNeeded:
                Write(' ');
                _minify = MinifyState.Compressed;
                break;
              case MinifyState.LastCharWasSpace:
                _minify = MinifyState.Compressed;
                break;
            }

            if (_preserveWhitespace == PreserveState.None)
            {
              // No escaping required
            }
            else if (_preserveWhitespace == PreserveState.BeforeContent)
            {
              _preserveWhitespace = PreserveState.Preserve;
            }
            Write(value[i]);
          }
        }
        _outputStarted = true;
      }
    }
    private void WriteStartElementEnd()
    {
      if (_nodes.Count > 0 && _state == InternalState.Element)
      {
        var start = _nodes.Peek();
        switch (start.Value)
        {
          case "a":
            StartInline();
            _buffer = Pool.NewStringBuilder();
            break;
          case "area":
            if (!string.IsNullOrEmpty(start["href"]))
            {
              _links.Add(new PlainTextLink()
              {
                Href = start["href"],
                Offset = _pos,
                Tag = start
              });
            }
            break;
          case "link":
            if (!string.IsNullOrEmpty(start["href"])
              && start["rel"] == "canonical")
            {
              _links.Add(new PlainTextLink()
              {
                Href = start["href"],
                Offset = _pos,
                Tag = start
              });
            }
            break;
          case "meta":
            var name = start["name"];
            var content = start["content"];
            if (!string.IsNullOrEmpty(content))
            {
              if (string.IsNullOrEmpty(name))
                name = start["property"];
              if (!string.IsNullOrEmpty(name))
                _metadata.Add(new KeyValuePair<string, string>(name, content));
            }
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

    private void StartInline()
    {
      if (_minify == MinifyState.SpaceNeeded)
      {
        Write(' ');
        _minify = MinifyState.LastCharWasSpace;
      }
    }
    private void StartList(string prefix)
    {
      if (_minify == MinifyState.Compressed
        || _minify == MinifyState.SpaceNeeded
        || _minify == MinifyState.BlockEnd)
      {
        Write(Environment.NewLine);
        _minify = MinifyState.LastCharWasSpace;
      }
      _linePrefix.Add(prefix ?? string.Empty);
    }
    private void StartBlock(string prefix)
    {
      var prefixRequired = false;
      if (_minify == MinifyState.Compressed
        || _minify == MinifyState.SpaceNeeded
        || _minify == MinifyState.BlockEnd)
      {
        Write(Environment.NewLine);
        Write(Environment.NewLine);
        _minify = MinifyState.LastCharWasSpace;
        prefixRequired = true;
      }
      _linePrefix.Add(prefix ?? string.Empty);
      if (prefixRequired || prefix != string.Empty)
        WritePrefix();
      _outputStarted = true;
    }
    private void EndBlock()
    {
      _minify = MinifyState.BlockEnd;
      if (_linePrefix.Count > 0)
        _linePrefix.RemoveAt(_linePrefix.Count - 1);
    }

    private void WriteEscaped(string text)
    {
      for (var i = 0; i < text.Length; i++)
      {
        switch (text[i])
        {
          case Symbols.Ampersand: Write("&amp;"); break;
          case Symbols.NoBreakSpace: Write("&nbsp;"); break;
          case Symbols.GreaterThan: Write("&gt;"); break;
          case Symbols.LessThan: Write("&lt;"); break;
          case Symbols.DoubleQuote:
              Write("&quot;");
            break;
          case Symbols.SingleQuote:
              Write("'");
            break;
          case Symbols.LineFeed: Write(Environment.NewLine); break;
          default: Write(text[i]); break;
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
