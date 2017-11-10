using System.Text;

namespace BracketPipe
{
  internal class ParseState
  {
    private TextSource _source;
    private TextileParseSettings _settings;
    private int _runStart = int.MaxValue;

    public ParseState(TextSource source, TextileParseSettings settings)
    {
      _source = source;
      _settings = settings;
    }

    public char this[int index] { get { return _source[index]; } }
    public int Index
    {
      get { return _source.Index; }
      set { _source.Index = value; }
    }

    public int Peek()
    {
      return _source.Peek();
    }

    public char ReadCharacter()
    {
      return _source.ReadCharacter();
    }

    public char ReadInlineOrCharacter(ParseOutput output)
    {
      foreach (var inline in _settings.Inlines)
      {
        if (inline.TryParse(this, output))
          return ReadCharacter();
      }
      return ReadCharacter();
    }

    public bool Reset(int index)
    {
      _source.Index = index;
      return false;
    }

    public override string ToString()
    {
      return _source.ToString();
    }

    public string ToString(int start, int length)
    {
      return _source.ToString(start, length);
    }

    public void StartRun()
    {
      StartRun(_source.Index);
    }

    public void StartRun(int start)
    {
      _runStart = start;
    }

    public HtmlText EndRun()
    {
      return EndRun(_source.Index);
    }

    public HtmlText EndRun(int end)
    {
      if (end <= 0)
        end = _source.Index + end;
      if (end > _runStart)
        return new HtmlText(GetText(end));
      return null;
    }

    private string GetText(int end)
    {
      var builder = new StringBuilder();

      for (var i = _runStart; i < end; i++)
      {
        switch (_source[i])
        {
          case '"':
            if (i == _runStart || char.IsWhiteSpace(_source[i - 1]))
              builder.Append((char)8220);
            else
              builder.Append((char)8221);
            break;
          case '\'':
            if (i == _runStart || char.IsWhiteSpace(_source[i - 1]))
              builder.Append((char)8216);
            else
              builder.Append((char)8217);
            break;
          case '-':
            if ((i + 1) < end && _source[i + 1] == '-')
            {
              i++;
              builder.Append((char)8212);
            }
            else if ((i == _runStart || char.IsWhiteSpace(_source[i - 1]))
              && ((i + 1) >= end || char.IsWhiteSpace(_source[i + 1])))
            {
              builder.Append((char)8211);
            }
            else
            {
              builder.Append(_source[i]);
            }
            break;
          case '.':
            if ((i + 2) < end && _source[i + 1] == '.' && _source[i + 2] == '.')
            {
              i += 2;
              builder.Append((char)8230);
            }
            else
            {
              builder.Append(_source[i]);
            }
            break;
          case 'x':

            break;
          case '(':
            if ((i + 2) < end && _source[i + 1] == 'r' && _source[i + 2] == ')')
            {
              i += 2;
              builder.Append((char)174);
            }
            else if ((i + 3) < end && _source[i + 1] == 't' && _source[i + 2] == 'm' && _source[i + 3] == ')')
            {
              i += 3;
              builder.Append((char)8482);
            }
            break;
          default:
            builder.Append(_source[i]);
            break;
        }
      }

      return builder.ToString();
    }
  }
}
