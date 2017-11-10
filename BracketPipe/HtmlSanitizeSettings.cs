using System;
using System.Collections.Generic;

namespace BracketPipe
{
  /// <summary>
  /// Settings controlling what CSS and HTML is permitted
  /// </summary>
  public class HtmlSanitizeSettings
  {
    private HashSet<string> _allowedAttributes;
    private HashSet<string> _allowedCssAtRules;
    private HashSet<string> _allowedCssFunctions;
    private HashSet<string> _allowedCssProps;
    private HashSet<string> _allowedTags;
    private HashSet<string> _allowedSchemes;
    private HashSet<string> _uriAttributes;

#if NET35
    /// <summary>
    /// HTML attribute names which are permitted through the sanitize routine
    /// </summary>
    public HashSet<string> AllowedAttributes { get { return _allowedAttributes; } }
    /// <summary>
    /// CSS @-rules permitted
    /// </summary>
    public HashSet<string> AllowedCssAtRules { get { return _allowedCssAtRules; } }
    /// <summary>
    /// CSS functions permitted
    /// </summary>
    public HashSet<string> AllowedCssFunctions { get { return _allowedCssFunctions; } }
    /// <summary>
    /// CSS property names which are permitted through the sanitize routine
    /// </summary>
    public HashSet<string> AllowedCssProps { get { return _allowedCssProps; } }
    /// <summary>
    /// HTML tag names which are permitted through the sanitize routine
    /// </summary>
    public HashSet<string> AllowedTags { get { return _allowedTags; } }
    /// <summary>
    /// Allowed URI schemes (e.g. in the <c>href</c> attribute of <c>a</c> tags)
    /// </summary>
    public HashSet<string> AllowedSchemes { get { return _allowedSchemes; } }
    /// <summary>
    /// Attributes which contain a URI
    /// </summary>
    public HashSet<string> UriAttributes { get { return _uriAttributes; } }
#else
    /// <summary>
    /// HTML attribute names which are permitted through the sanitize routine
    /// </summary>
    public ISet<string> AllowedAttributes { get { return _allowedAttributes; } }
    /// <summary>
    /// CSS @-rules permitted
    /// </summary>
    public ISet<string> AllowedCssAtRules { get { return _allowedCssAtRules; } }
    /// <summary>
    /// CSS functions permitted
    /// </summary>
    public ISet<string> AllowedCssFunctions { get { return _allowedCssFunctions; } }
    /// <summary>
    /// CSS property names which are permitted through the sanitize routine
    /// </summary>
    public ISet<string> AllowedCssProps { get { return _allowedCssProps; } }
    /// <summary>
    /// HTML tag names which are permitted through the sanitize routine
    /// </summary>
    public ISet<string> AllowedTags { get { return _allowedTags; } }
    /// <summary>
    /// Allowed URI schemes (e.g. in the <c>href</c> attribute of <c>a</c> tags)
    /// </summary>
    public ISet<string> AllowedSchemes { get { return _allowedSchemes; } }
    /// <summary>
    /// Attributes which contain a URI
    /// </summary>
    public ISet<string> UriAttributes { get { return _uriAttributes; } }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlSanitizeSettings"/> class.
    /// </summary>
    public HtmlSanitizeSettings()
    {
      _allowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _allowedCssAtRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _allowedCssFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _allowedCssProps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _allowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _allowedSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _uriAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    private HtmlSanitizeSettings(bool setDefaults)
    {
      _allowedAttributes = new HashSet<string>(new string[]
      {
        "abbr",
        "accept",
        "accept-charset",
        "accesskey",
        "action",
        "align",
        "alt",
        "autocomplete",
        "autosave",
        "axis",
        "bgcolor",
        "border",
        "cellpadding",
        "cellspacing",
        "challenge",
        "char",
        "charoff",
        "charset",
        "checked",
        "cite",
        "clear",
        "color",
        "cols",
        "colspan",
        "compact",
        "contenteditable",
        "coords",
        "datetime",
        "dir",
        "disabled",
        "draggable",
        "dropzone",
        "enctype",
        "for",
        "frame",
        "headers",
        "height",
        "high",
        "href",
        "hreflang",
        "hspace",
        "ismap",
        "keytype",
        "label",
        "lang",
        "list",
        "longdesc",
        "low",
        "max",
        "maxlength",
        "media",
        "method",
        "min",
        "multiple",
        "name",
        "nohref",
        "noshade",
        "novalidate",
        "nowrap",
        "open",
        "optimum",
        "pattern",
        "placeholder",
        "prompt",
        "pubdate",
        "radiogroup",
        "readonly",
        "rel",
        "required",
        "rev",
        "reversed",
        "rows",
        "rowspan",
        "rules",
        "scope",
        "selected",
        "shape",
        "size",
        "span",
        "spellcheck",
        "src",
        "start",
        "step",
        "style",
        "summary",
        "tabindex",
        "target",
        "title",
        "type",
        "usemap",
        "valign",
        "value",
        "vspace",
        "width",
        "wrap"
      }, StringComparer.OrdinalIgnoreCase);
      _allowedCssAtRules = new HashSet<string>(new string[]
      {
        "charset",
        "counter-style",
        "document",
        "font-face",
        "font-feature-values",
        "keyframes",
        "media",
        "namespace",
        "page",
        "styleset",
        "supports",
        "viewport",
      }, StringComparer.OrdinalIgnoreCase);
      _allowedCssFunctions = new HashSet<string>(new string[]
      {
        "annotation",
        "attr",
        "blur",
        "brightness",
        "calc",
        "contrast",
        "drop-shadow",
        "element",
        "fit-content",
        "format",
        "grayscale",
        "hsl",
        "hsla",
        "hue-rotate",
        "image",
        "invert",
        "linear-gradient",
        "local",
        "matrix",
        "matrix3d",
        "minmax",
        "opacity",
        "perspective",
        "radial-gradient",
        "repeating-linear-gradient",
        "repeating-radial-gradient",
        "rgb",
        "rgba",
        "rotate",
        "rotate3d",
        "rotateX",
        "rotateY",
        "rotateZ",
        "saturate",
        "scale",
        "scale3d",
        "scaleX",
        "scaleY",
        "scaleZ",
        "sepia",
        "skew",
        "skewX",
        "skewY",
        "symbols",
        "translate",
        "translate3d",
        "translateX",
        "translateY",
        "translateZ",
        "var",
      }, StringComparer.OrdinalIgnoreCase);
      _allowedCssProps = new HashSet<string>(new string[]
      {
        "background",
        "background-attachment",
        "background-color",
        "background-image",
        "background-position",
        "background-repeat",
        "border",
        "border-bottom",
        "border-bottom-color",
        "border-bottom-style",
        "border-bottom-width",
        "border-collapse",
        "border-color",
        "border-left",
        "border-left-color",
        "border-left-style",
        "border-left-width",
        "border-right",
        "border-right-color",
        "border-right-style",
        "border-right-width",
        "border-spacing",
        "border-style",
        "border-top",
        "border-top-color",
        "border-top-style",
        "border-top-width",
        "border-width",
        "bottom",
        "caption-side",
        "clear",
        "clip",
        "color",
        "content",
        "counter-increment",
        "counter-reset",
        "cursor",
        "direction",
        "display",
        "empty-cells",
        "float",
        "font",
        "font-family",
        "font-size",
        "font-style",
        "font-variant",
        "font-weight",
        "height",
        "left",
        "letter-spacing",
        "line-height",
        "list-style",
        "list-style-image",
        "list-style-position",
        "list-style-type",
        "margin",
        "margin-bottom",
        "margin-left",
        "margin-right",
        "margin-top",
        "max-height",
        "max-width",
        "min-height",
        "min-width",
        "opacity",
        "orphans",
        "outline",
        "outline-color",
        "outline-style",
        "outline-width",
        "overflow",
        "padding",
        "padding-bottom",
        "padding-left",
        "padding-right",
        "padding-top",
        "page-break-after",
        "page-break-before",
        "page-break-inside",
        "quotes",
        "right",
        "table-layout",
        "text-align",
        "text-decoration",
        "text-indent",
        "text-transform",
        "top",
        "unicode-bidi",
        "vertical-align",
        "visibility",
        "white-space",
        "widows",
        "width",
        "word-spacing",
        "z-index"
      }, StringComparer.OrdinalIgnoreCase);
      _allowedTags = new HashSet<string>(new string[]
      {
        "a",
        "abbr",
        "acronym",
        "address",
        "area",
        "article",
        "aside",
        "b",
        "bdi",
        "bdo",
        "big",
        "blockquote",
        "br",
        "button",
        "caption",
        "center",
        "cite",
        "code",
        "col",
        "colgroup",
        "data",
        "datalist",
        "dd",
        "del",
        "details",
        "dfn",
        "dir",
        "div",
        "dl",
        "dt",
        "em",
        "fieldset",
        "figcaption",
        "figure",
        "font",
        "footer",
        "form",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "header",
        "hr",
        "i",
        "img",
        "input",
        "ins",
        "kbd",
        "keygen",
        "label",
        "legend",
        "li",
        "main",
        "map",
        "mark",
        "menu",
        "menuitem",
        "meter",
        "nav",
        "ol",
        "optgroup",
        "option",
        "output",
        "p",
        "pre",
        "q",
        "rp",
        "rt",
        "ruby",
        "s",
        "samp",
        "section",
        "select",
        "small",
        "span",
        "strike",
        "strong",
        "sub",
        "summary",
        "sup",
        "table",
        "tbody",
        "td",
        "textarea",
        "tfoot",
        "th",
        "thead",
        "time",
        "tr",
        "tt",
        "u",
        "ul",
        "var",
        "wbr",
      }, StringComparer.OrdinalIgnoreCase);
      _allowedSchemes = new HashSet<string>(new string[]
      {
        "http",
        "https",
      }, StringComparer.OrdinalIgnoreCase);
      _uriAttributes = new HashSet<string>(new string[]
      {
        "action",
        "background",
        "dynsrc",
        "href",
        "lowsrc",
        "src"
      }, StringComparer.OrdinalIgnoreCase);
    }

    private static HtmlSanitizeSettings _default = new HtmlSanitizeSettings(true);

    internal static HtmlSanitizeSettings ReadOnlyDefault { get { return _default; } }

    /// <summary>
    /// Create a new settings object using default values
    /// </summary>
    /// <returns>A new instance of <see cref="HtmlMinifySettings"/></returns>
    public static HtmlSanitizeSettings Default()
    {
      return new HtmlSanitizeSettings(true);
    }
  }
}
