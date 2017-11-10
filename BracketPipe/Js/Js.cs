using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BracketPipe
{
  public static class Js
  {
    public static string Minify(TextSource js)
    {
      var min = new JSMin();
      return min.Minify(js);
    }
  }
}
