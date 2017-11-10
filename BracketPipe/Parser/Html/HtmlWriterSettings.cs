using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public class HtmlWriterSettings
  {
    public bool Indent { get; set; }
    public string IndentChars { get; set; }
    public string NewLineChars { get; set; }
    public bool NewLineOnAttributes { get; set; }
    public char QuoteChar { get; set; }
    //public bool ReplaceConsecutiveSpaceNonBreaking { get; set; }

    public HtmlWriterSettings()
    {
      this.Indent = false;
      this.IndentChars = "  ";
      this.NewLineChars = Environment.NewLine;
      this.NewLineOnAttributes = false;
      this.QuoteChar = '"';
      //this.ReplaceConsecutiveSpaceNonBreaking = false;
    }
  }
}
