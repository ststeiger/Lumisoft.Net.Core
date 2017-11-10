using System.Collections.Generic;

namespace BracketPipe
{
  class ParseOutput
  {
    private List<HtmlNode> _nodes;

    public void Add(HtmlNode node)
    {
      if (node != null)
        _nodes.Add(node);
    }

    public IEnumerable<HtmlNode> GetNodes()
    {
      return _nodes.ToArray();
    }
  }
}
