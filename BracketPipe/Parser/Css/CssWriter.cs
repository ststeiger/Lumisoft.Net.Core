using BracketPipe.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public class CssWriter : IDisposable
  {
    private TextWriter _writer;

    public CssWriter(TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      _writer = writer;
    }

    public void Dispose()
    {
      _writer.Dispose();
    }

    public void Write(CssColorToken color)
    {
      _writer.Write('#');
      _writer.Write(color.Data);
    }
    public void Write(CssCommentToken comment)
    {
      _writer.Write("/*");
      _writer.Write(comment.Data);
      if (!comment.IsBad)
        _writer.Write("*/");
    }
    public void Write(CssFunctionToken func)
    {
      _writer.Write(func.Data);
      _writer.Write('(');
      foreach (var arg in func)
        Write(arg);
    }
    public void Write(CssKeywordToken keyword)
    {
      switch (keyword.Type)
      {
        case CssTokenType.Hash:
          _writer.Write('#');
          _writer.Write(keyword.Data);
          break;
        case CssTokenType.AtKeyword:
          _writer.Write('@');
          _writer.Write(keyword.Data);
          break;
        case CssTokenType.Function:
          _writer.Write(keyword.Data);
          _writer.Write('(');
          break;
        default:
          _writer.Write(keyword.Data);
          break;
      }
    }
    public void Write(CssStringToken str)
    {
      WriteString(str.Data);
    }
    public void Write(CssUnitToken unit)
    {
      _writer.Write(unit.Data);
      _writer.Write(unit.Unit);
    }
    public void Write(CssUrlToken url)
    {
      _writer.Write(url.FunctionName);
      _writer.Write('(');
      WriteString(url.Data);
      _writer.Write(')');
    }
    public void Write(CssPropertyToken prop)
    {
      _writer.Write(prop.Data);
      _writer.Write(':');
      foreach (var arg in prop)
        Write(arg);
      if (prop.IsTerminated)
        _writer.Write(';');
    }
    public void Write(CssAtGroupToken group)
    {
      _writer.Write('@');
      _writer.Write(group.Data);
      foreach (var arg in group)
        Write(arg);
      if (group.IsTerminated)
        _writer.Write(';');
    }

    public void Write(CssToken token)
    {
      if (token is CssColorToken)
        Write((CssColorToken)token);
      else if (token is CssCommentToken)
        Write((CssCommentToken)token);
      else if (token is CssFunctionToken)
        Write((CssFunctionToken)token);
      else if (token is CssKeywordToken)
        Write((CssKeywordToken)token);
      else if (token is CssPropertyToken)
        Write((CssPropertyToken)token);
      else if (token is CssStringToken)
        Write((CssStringToken)token);
      else if (token is CssUnitToken)
        Write((CssUnitToken)token);
      else if (token is CssUrlToken)
        Write((CssUrlToken)token);
      else
        _writer.Write(token.Data);
    }

    private void WriteString(string value)
    {
      _writer.Write(Symbols.DoubleQuote);
      if (!String.IsNullOrEmpty(value))
      {
        for (var i = 0; i < value.Length; i++)
        {
          var character = value[i];

          if (character == Symbols.Null)
          {
            throw new ArgumentException("Invalid character");
          }
          else if (character == Symbols.DoubleQuote || character == Symbols.ReverseSolidus)
          {
            _writer.Write(Symbols.ReverseSolidus);
            _writer.Write(character);
          }
          else if (character.IsInRange(0x1, 0x1f) || character == (Char)0x7b)
          {
            _writer.Write(Symbols.ReverseSolidus);
            _writer.Write(character.ToHex());
            _writer.Write(i + 1 != value.Length ? " " : "");
          }
          else
          {
            _writer.Write(character);
          }
        }
      }

      _writer.Write(Symbols.DoubleQuote);
    }

  }
}
