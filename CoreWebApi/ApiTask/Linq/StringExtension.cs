using System;
using System.Text;
using System.Text.RegularExpressions;
using VeryCodes;

public static class StringExtension
{
	private static Regex XML_REGEX = new Regex("[\\x00-\\x08\\x0b-\\x0c\\x0e-\\x1f]", RegexOptions.Compiled);

	private static Regex HTML_REGEX = new Regex("<[^>]*>", RegexOptions.Compiled);

	private static NHtmlFilter _htmlFilter = new NHtmlFilter(false);

	public static bool IsNullOrEmpty(this string s)
	{
		return s == null || s.Length == 0;
	}

	public static bool IsNullOrEmpty(this string s, bool ingoreWhiteSpace)
	{
		if (ingoreWhiteSpace)
		{
			return string.IsNullOrWhiteSpace(s);
		}
		return s == null || s.Length == 0;
	}

	public static int[] ToAsciiArray(this string s)
	{
		if (s == null)
		{
			return null;
		}
		int[] asciiArray = new int[s.Length];
		for (int i = 0; i < s.Length; i++)
		{
			asciiArray[i] = (int)s[i];
		}
		return asciiArray;
	}

	public static string TrimXml(this string s)
	{
		if (string.IsNullOrWhiteSpace(s) || s[0] == '<')
		{
			return s;
		}
		int x = s.IndexOf('<');
		if (x > 0)
		{
			s = s.Substring(x);
		}
		return s;
	}

	public static string EncodeXml(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		string output = StringExtension.XML_REGEX.Replace(s, string.Empty);
		output = output.Replace("&", "&amp;");
		output = output.Replace("<", "&lt;");
		output = output.Replace(">", "&gt;");
		output = output.Replace("'", "&apos;");
		return output.Replace("\"", "&quot;");
	}

	public static string DecodeXml(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		string output = s.Replace("&quot;", "\"");
		output = output.Replace("&apos;", "'");
		output = output.Replace("&gt;", ">");
		output = output.Replace("&lt;", "<");
		return output.Replace("&amp;", "&");
	}

	public static string EncodeHex(this string s)
	{
		string output = string.Empty;
		for (int i = 0; i < s.Length; i++)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(s.Substring(i, 1));
			string lowCode = Convert.ToString(bytes[0], 16);
			if (lowCode.Length == 1)
			{
				lowCode = "0" + lowCode;
			}
			string hightCode = Convert.ToString(bytes[1], 16);
			if (hightCode.Length == 1)
			{
				hightCode = "0" + hightCode;
			}
			output = output + lowCode + hightCode;
		}
		return output;
	}

	public static string DecodeHex(this string s)
	{
		string output = string.Empty;
		if (s.Length % 4 != 0)
		{
			throw new FormatException("Invalid hex string.");
		}
		for (int i = 0; i < s.Length; i += 4)
		{
			byte[] bytes = new byte[2];
			string lowCode = s.Substring(i, 2);
			bytes[0] = Convert.ToByte(lowCode, 16);
			string highCode = s.Substring(i + 2, 2);
			bytes[1] = Convert.ToByte(highCode, 16);
			string character = Encoding.Unicode.GetString(bytes);
			output += character;
		}
		return output;
	}

	public static string EscapeSql(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		return s.Replace("'", "''");
	}

	public static string EscapeUrl(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		return Uri.EscapeDataString(s);
	}

	public static string UnescapeUrl(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		return Uri.UnescapeDataString(s);
	}

	public static string EscapeJson(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		s = s.Replace("\\", "\\\\");
		s = s.Replace("\"", "\\\"");
		s = s.Replace("\r", "\\r");
		s = s.Replace("\n", "\\n");
		return s;
	}

	public static string Reverse(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		string output = string.Empty;
		for (int i = s.Length - 1; i > -1; i--)
		{
			output += s[i];
		}
		return output;
	}

	public static string SubLeft(this string s, int totalWidth)
	{
		if (s == null || s.Length <= totalWidth)
		{
			return s;
		}
		return s.Substring(0, totalWidth);
	}

	public static string SubLeft(this string s, int totalWidth, string postfix)
	{
		if (s == null || s.Length <= totalWidth)
		{
			return s;
		}
		if (string.IsNullOrEmpty(postfix))
		{
			return s.Substring(0, totalWidth);
		}
		totalWidth -= postfix.Length;
		return s.Substring(0, totalWidth) + postfix;
	}

	public static string SubRight(this string s, int totalWidth)
	{
		if (s == null || s.Length <= totalWidth)
		{
			return s;
		}
		return s.Substring(s.Length - totalWidth);
	}

	public static string SubRight(this string s, int totalWidth, string prefix)
	{
		if (s == null || s.Length <= totalWidth)
		{
			return s;
		}
		if (string.IsNullOrEmpty(prefix))
		{
			s.Substring(s.Length - totalWidth);
		}
		totalWidth -= prefix.Length;
		return prefix + s.Substring(s.Length - totalWidth);
	}

	public static string WipeHtml(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		return StringExtension.HTML_REGEX.Replace(s, string.Empty);
	}

	public static string WipeScript(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		s = StringFilter.FilterScript(s);
		s = StringFilter.FilterObject(s);
		s = StringFilter.FilterIframe(s);
		return s;
	}
}
