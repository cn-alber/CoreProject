using System.Text;
using System.Text.RegularExpressions;

namespace VeryCodes
{
	internal class StringFilter
	{
		public static string FilterScript(string str)
		{
			string pattern = "<script[\\s\\S]+</script *>";
			return StringFilter.StripScriptAttributesFromTags(Regex.Replace(str, pattern, string.Empty, RegexOptions.IgnoreCase));
		}

		private static string StripScriptAttributesFromTags(string str)
		{
			string pattern = "(?<ScriptAttr>on\\w+=\\s*(['\"\\s]?)([/s/S]*[^\\1]*?)\\1)[\\s|>|/>]";
			Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			foreach (Match i in r.Matches(str))
			{
				string attrs = i.Groups["ScriptAttr"].Value;
				if (!string.IsNullOrEmpty(attrs))
				{
					str = str.Replace(attrs, string.Empty);
				}
			}
			str = StringFilter.FilterHrefScript(str);
			return str;
		}

		public static string FilterHrefScript(string str)
		{
			string regexstr = " href[ ^=]*=\\s*(['\"\\s]?)[\\w]*script+?:([/s/S]*[^\\1]*?)\\1[\\s]*";
			return Regex.Replace(str, regexstr, " ", RegexOptions.IgnoreCase);
		}

		public static string FilterSrc(string str)
		{
			string regexstr = " src *=\\s*(['\"\\s]?)[^\\.]+\\.(\\w+)\\1[\\s]*";
			return Regex.Replace(str, regexstr, " ", RegexOptions.IgnoreCase);
		}

		public static string FilterHtml(string str)
		{
			string[] aryReg = new string[]
			{
				"<style[\\s\\S]+</style>",
				"<.*?>",
				"<(.[^>]*)>",
				"([\\r\\n])[\\s]+",
				"&(quot|#34);",
				"&(amp|#38);",
				"&(lt|#60);",
				"&(gt|#62);",
				"&(nbsp|#160);",
				"&(iexcl|#161);",
				"&(cent|#162);",
				"&(pound|#163);",
				"&(copy|#169);",
				"&#(\\d+);",
				"-->",
				"<!--.*\\n"
			};
			string[] aryRep = new string[]
			{
				"",
				"",
				"",
				"",
				"\"",
				"&",
				"<",
				">",
				" ",
				"¡",
				"¢",
				"£",
				"©",
				"",
				"\r\n",
				""
			};
			string strOutput = str;
			for (int i = 0; i < aryReg.Length; i++)
			{
				Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
				strOutput = regex.Replace(strOutput, aryRep[i]);
			}
			strOutput = strOutput.Replace("<", "");
			strOutput = strOutput.Replace(">", "");
			return strOutput.Replace("\r\n", "");
		}

		public static string FilterObject(string content)
		{
			string regexstr = "<object[\\s\\S]+</object *>";
			return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
		}

		public static string FilterIframe(string content)
		{
			string regexstr = "<iframe[\\s\\S]+</iframe *>";
			return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
		}

		public static string FilterFrameset(string content)
		{
			string regexstr = "<frameset[\\s\\S]+</frameset *>";
			return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
		}

		public static string FilterSql(string str)
		{
			str = str.Replace("'", "''");
			str = str.Replace("<", "&lt;");
			str = str.Replace(">", "&gt;");
			return str;
		}

		public static string FilterBadWords(string keyWord, string chkStr)
		{
			if (chkStr == "")
			{
				return "";
			}
			string[] bwords = keyWord.Split(new char[]
			{
				'|'
			});
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < bwords.Length; i++)
			{
				string str = bwords[i].ToString().Trim();
				string regStr = str;
				Regex r = new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
				Match j = r.Match(chkStr);
				if (j.Success)
				{
					int k = j.Value.Length;
					sb.Insert(0, "*", k);
					string toStr = sb.ToString();
					chkStr = Regex.Replace(chkStr, regStr, toStr, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
				}
				sb.Remove(0, sb.Length);
			}
			return chkStr;
		}
	}
}
