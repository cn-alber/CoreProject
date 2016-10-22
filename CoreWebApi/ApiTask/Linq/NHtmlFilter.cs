using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal class NHtmlFilter
{
	protected static readonly RegexOptions REGEX_FLAGS_SI = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;

	private static string P_COMMENTS = "<!--(.*?)-->";

	private static Regex P_COMMENT = new Regex("^!--(.*)--$", NHtmlFilter.REGEX_FLAGS_SI);

	private static string P_TAGS = "<(.*?)>";

	private static Regex P_END_TAG = new Regex("^/([a-z0-9]+)", NHtmlFilter.REGEX_FLAGS_SI);

	private static Regex P_START_TAG = new Regex("^([a-z0-9]+)(.*?)(/?)$", NHtmlFilter.REGEX_FLAGS_SI);

	private static Regex P_QUOTED_ATTRIBUTES = new Regex("([a-z0-9]+)=([\"'])(.*?)\\2", NHtmlFilter.REGEX_FLAGS_SI);

	private static Regex P_UNQUOTED_ATTRIBUTES = new Regex("([a-z0-9]+)(=)([^\"\\s']+)", NHtmlFilter.REGEX_FLAGS_SI);

	private static Regex P_PROTOCOL = new Regex("^([^:]+):", NHtmlFilter.REGEX_FLAGS_SI);

	private static Regex P_ENTITY = new Regex("&#(\\d+);?");

	private static Regex P_ENTITY_UNICODE = new Regex("&#x([0-9a-f]+);?");

	private static Regex P_ENCODE = new Regex("%([0-9a-f]{2});?");

	private static Regex P_VALID_ENTITIES = new Regex("&([^&;]*)(?=(;|&|$))");

	private static Regex P_VALID_QUOTES = new Regex("(>|^)([^<]+?)(<|$)", RegexOptions.Compiled | RegexOptions.Singleline);

	private static string P_END_ARROW = "^>";

	private static string P_BODY_TO_END = "<([^>]*?)(?=<|$)";

	private static string P_XML_CONTENT = "(^|>)([^<]*?)(?=>)";

	private static string P_STRAY_LEFT_ARROW = "<([^>]*?)(?=<|$)";

	private static string P_STRAY_RIGHT_ARROW = "(^|>)([^<]*?)(?=>)";

	private static string P_QUOTE = "\"";

	private static string P_LEFT_ARROW = "<";

	private static string P_RIGHT_ARROW = ">";

	private static string P_BOTH_ARROWS = "<>";

	private static Dictionary<string, string> P_REMOVE_PAIR_BLANKS = new Dictionary<string, string>();

	private static Dictionary<string, string> P_REMOVE_SELF_BLANKS = new Dictionary<string, string>();

	protected static bool alwaysMakeTags = true;

	protected static bool stripComment = true;

	private string[] vDisallowed;

	protected Dictionary<string, List<string>> vAllowed;

	protected Dictionary<string, int> vTagCounts;

	protected string[] vSelfClosingTags;

	protected string[] vNeedClosingTags;

	protected string[] vProtocolAtts;

	protected string[] vAllowedProtocols;

	protected string[] vRemoveBlanks;

	protected string[] vAllowedEntities;

	protected bool vDebug;

	public NHtmlFilter() : this(false)
	{
	}

	public NHtmlFilter(bool debug)
	{
		this.vDebug = debug;
		this.vAllowed = new Dictionary<string, List<string>>();
		this.vTagCounts = new Dictionary<string, int>();
		List<string> a_atts = new List<string>();
		a_atts.Add("href");
		a_atts.Add("target");
		this.vAllowed.Add("a", a_atts);
		List<string> img_atts = new List<string>();
		img_atts.Add("src");
		img_atts.Add("width");
		img_atts.Add("height");
		img_atts.Add("alt");
		this.vAllowed.Add("img", img_atts);
		List<string> no_atts = new List<string>();
		this.vAllowed.Add("b", no_atts);
		this.vAllowed.Add("strong", no_atts);
		this.vAllowed.Add("i", no_atts);
		this.vAllowed.Add("em", no_atts);
		this.vSelfClosingTags = new string[]
		{
			"img"
		};
		this.vNeedClosingTags = new string[]
		{
			"a",
			"b",
			"strong",
			"i",
			"em"
		};
		this.vDisallowed = new string[0];
		this.vAllowedProtocols = new string[]
		{
			"http",
			"mailto"
		};
		this.vProtocolAtts = new string[]
		{
			"src",
			"href"
		};
		this.vRemoveBlanks = new string[]
		{
			"a",
			"b",
			"strong",
			"i",
			"em"
		};
		this.vAllowedEntities = new string[]
		{
			"amp",
			"gt",
			"lt",
			"quot"
		};
		NHtmlFilter.stripComment = true;
		NHtmlFilter.alwaysMakeTags = true;
	}

	protected void reset()
	{
		this.vTagCounts = new Dictionary<string, int>();
	}

	protected void debug(string msg)
	{
		bool arg_06_0 = this.vDebug;
	}

	public static string chr(int dec)
	{
		return string.Concat((char)dec);
	}

	public static string htmlSpecialChars(string str)
	{
		str = str.Replace(NHtmlFilter.P_QUOTE, "&quot;");
		str = str.Replace(NHtmlFilter.P_LEFT_ARROW, "&lt;");
		str = str.Replace(NHtmlFilter.P_RIGHT_ARROW, "&gt;");
		str = str.Replace("\n", "<br>");
		return str;
	}

	public string filter(string input)
	{
		this.reset();
		this.debug("************************************************");
		this.debug("              INPUT: " + input);
		string s = this.escapeComments(input);
		this.debug("     escapeComments: " + s);
		s = this.balanceHTML(s);
		this.debug("        balanceHTML: " + s);
		s = this.checkTags(s);
		this.debug("          checkTags: " + s);
		s = this.processRemoveBlanks(s);
		this.debug("processRemoveBlanks: " + s);
		s = this.validateEntities(s);
		this.debug("    validateEntites: " + s);
		this.debug("************************************************\n\n");
		return s;
	}

	protected string escapeComments(string s)
	{
		return Regex.Replace(s, NHtmlFilter.P_COMMENTS, new MatchEvaluator(NHtmlFilter.ConverMatchComments), RegexOptions.Singleline);
	}

	protected string regexReplace(string regex_pattern, string replacement, string s)
	{
		return Regex.Replace(s, regex_pattern, replacement);
	}

	protected string balanceHTML(string s)
	{
		if (NHtmlFilter.alwaysMakeTags)
		{
			s = this.regexReplace(NHtmlFilter.P_END_ARROW, "", s);
			s = this.regexReplace(NHtmlFilter.P_BODY_TO_END, "<$1>", s);
			s = this.regexReplace(NHtmlFilter.P_XML_CONTENT, "$1<$2", s);
		}
		else
		{
			s = this.regexReplace(NHtmlFilter.P_STRAY_LEFT_ARROW, "&lt;$1", s);
			s = this.regexReplace(NHtmlFilter.P_STRAY_RIGHT_ARROW, "$1$2&gt;<", s);
			s = s.Replace(NHtmlFilter.P_BOTH_ARROWS, "");
		}
		return s;
	}

	protected string checkTags(string s)
	{
		s = Regex.Replace(s, NHtmlFilter.P_TAGS, new MatchEvaluator(this.ConverMatchTags), RegexOptions.Singleline);
		foreach (string key in this.vTagCounts.Keys)
		{
			for (int ii = 0; ii < this.vTagCounts[key]; ii++)
			{
				s = s + "</" + key + ">";
			}
		}
		return s;
	}

	protected string processRemoveBlanks(string s)
	{
		string[] array = this.vRemoveBlanks;
		for (int i = 0; i < array.Length; i++)
		{
			string tag = array[i];
			s = this.regexReplace(string.Concat(new string[]
			{
				"<",
				tag,
				"(\\s[^>]*)?></",
				tag,
				">"
			}), "", s);
			s = this.regexReplace("<" + tag + "(\\s[^>]*)?/>", "", s);
		}
		return s;
	}

	private string processTag(string s)
	{
		Match i = NHtmlFilter.P_END_TAG.Match(s);
		if (i.Success)
		{
			string name = i.Groups[1].Value.ToLower();
			if (this.allowed(name) && !NHtmlFilter.inArray(name, this.vSelfClosingTags) && this.vTagCounts.ContainsKey(name))
			{
				this.vTagCounts[name] = this.vTagCounts[name] - 1;
				return "</" + name + ">";
			}
		}
		i = NHtmlFilter.P_START_TAG.Match(s);
		if (i.Success)
		{
			string name2 = i.Groups[1].Value.ToLower();
			string body = i.Groups[2].Value;
			string ending = i.Groups[3].Value;
			if (this.allowed(name2))
			{
				string @params = "";
				MatchCollection m2 = NHtmlFilter.P_QUOTED_ATTRIBUTES.Matches(body);
				MatchCollection m3 = NHtmlFilter.P_UNQUOTED_ATTRIBUTES.Matches(body);
				List<string> paramNames = new List<string>();
				List<string> paramValues = new List<string>();
				foreach (Match match in m2)
				{
					paramNames.Add(match.Groups[1].Value);
					paramValues.Add(match.Groups[3].Value);
				}
				foreach (Match match2 in m3)
				{
					paramNames.Add(match2.Groups[1].Value);
					paramValues.Add(match2.Groups[3].Value);
				}
				for (int ii = 0; ii < paramNames.Count; ii++)
				{
					string paramName = paramNames[ii].ToLower();
					string paramValue = paramValues[ii];
					if (this.allowedAttribute(name2, paramName))
					{
						if (NHtmlFilter.inArray(paramName, this.vProtocolAtts))
						{
							paramValue = this.processParamProtocol(paramValue);
						}
						string text = @params;
						@params = string.Concat(new string[]
						{
							text,
							" ",
							paramName,
							"=\"",
							paramValue,
							"\""
						});
					}
				}
				if (NHtmlFilter.inArray(name2, this.vSelfClosingTags))
				{
					ending = " /";
				}
				if (NHtmlFilter.inArray(name2, this.vNeedClosingTags))
				{
					ending = "";
				}
				if (ending == null || ending.Length < 1)
				{
					if (this.vTagCounts.ContainsKey(name2))
					{
						this.vTagCounts[name2] = this.vTagCounts[name2] + 1;
					}
					else
					{
						this.vTagCounts.Add(name2, 1);
					}
				}
				else
				{
					ending = " /";
				}
				return string.Concat(new string[]
				{
					"<",
					name2,
					@params,
					ending,
					">"
				});
			}
			return "";
		}
		else
		{
			i = NHtmlFilter.P_COMMENT.Match(s);
			if (!NHtmlFilter.stripComment && i.Success)
			{
				return "<" + i.Value + ">";
			}
			return "";
		}
	}

	private string processParamProtocol(string s)
	{
		s = this.decodeEntities(s);
		Match i = NHtmlFilter.P_PROTOCOL.Match(s);
		if (i.Success)
		{
			string protocol = i.Groups[1].Value;
			if (!NHtmlFilter.inArray(protocol, this.vAllowedProtocols))
			{
				s = "#" + s.Substring(protocol.Length + 1, s.Length - protocol.Length - 1);
				if (s.StartsWith("#//"))
				{
					s = "#" + s.Substring(3, s.Length - 3);
				}
			}
		}
		return s;
	}

	private string decodeEntities(string s)
	{
		s = NHtmlFilter.P_ENTITY.Replace(s, new MatchEvaluator(this.ConverMatchEntity));
		s = NHtmlFilter.P_ENTITY_UNICODE.Replace(s, new MatchEvaluator(this.ConverMatchEntityUnicode));
		s = NHtmlFilter.P_ENCODE.Replace(s, new MatchEvaluator(this.ConverMatchEntityUnicode));
		s = this.validateEntities(s);
		return s;
	}

	private string validateEntities(string s)
	{
		s = NHtmlFilter.P_VALID_ENTITIES.Replace(s, new MatchEvaluator(this.ConverMatchValidEntities));
		s = NHtmlFilter.P_VALID_QUOTES.Replace(s, new MatchEvaluator(this.ConverMatchValidQuotes));
		return s;
	}

	private static bool inArray(string s, string[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			string item = array[i];
			if (item != null && item.Equals(s))
			{
				return true;
			}
		}
		return false;
	}

	private bool allowed(string name)
	{
		return (this.vAllowed.Count == 0 || this.vAllowed.ContainsKey(name)) && !NHtmlFilter.inArray(name, this.vDisallowed);
	}

	private bool allowedAttribute(string name, string paramName)
	{
		return this.allowed(name) && (this.vAllowed.Count == 0 || this.vAllowed[name].Contains(paramName));
	}

	private string checkEntity(string preamble, string term)
	{
		if (!";".Equals(term) || !this.isValidEntity(preamble))
		{
			return "&amp;" + preamble;
		}
		return '&' + preamble;
	}

	private bool isValidEntity(string entity)
	{
		return NHtmlFilter.inArray(entity, this.vAllowedEntities);
	}

	private static string ConverMatchComments(Match match)
	{
		return "<!--" + NHtmlFilter.htmlSpecialChars(match.Groups[1].Value) + "-->";
	}

	private string ConverMatchTags(Match match)
	{
		return this.processTag(match.Groups[1].Value);
	}

	private string ConverMatchEntity(Match match)
	{
		string v = match.Groups[1].Value;
		int @decimal = int.Parse(v);
		return NHtmlFilter.chr(@decimal);
	}

	private string ConverMatchEntityUnicode(Match match)
	{
		string v = match.Groups[1].Value;
		int @decimal = Convert.ToInt32("0x" + v, 16);
		return NHtmlFilter.chr(@decimal);
	}

	private string ConverMatchValidEntities(Match match)
	{
		string one = match.Groups[1].Value;
		string two = match.Groups[2].Value;
		return this.checkEntity(one, two);
	}

	private string ConverMatchValidQuotes(Match match)
	{
		string one = match.Groups[1].Value;
		string two = match.Groups[2].Value;
		string three = match.Groups[3].Value;
		return one + this.regexReplace(NHtmlFilter.P_QUOTE, "&quot;", two) + three;
	}

	public bool isAlwaysMakeTags()
	{
		return NHtmlFilter.alwaysMakeTags;
	}

	public bool isStripComments()
	{
		return NHtmlFilter.stripComment;
	}
}
