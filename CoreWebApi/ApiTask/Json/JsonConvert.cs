using API.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace API.Json
{
	public static class JsonConvert
	{
		private static readonly string _BACKSLASH = "@__BACKSLASH";

		private static readonly string _QUOTECHAR = "@__QUOTECHAR";

		private static readonly string _COLONCHAR = "@__COLONCHAR";

		private static readonly string _COMMACHAR = "@__COMMACHAR";

		private static readonly string _BRACES_BOF = "@__BRACES_BOF";

		private static readonly string _BRACES_EOF = "@__BRACES_EOF";

		private static readonly string _BRACKETS_BOF = "@__BRACKETS_BOF";

		private static readonly string _BRACKETS_EOF = "@__BRACKETS_EOF";

		private static Regex ENCODE_REGEX = new Regex("\\\"[^\\\"]*\\\"", RegexOptions.Compiled);

		private static Regex DSO_REGEX = new Regex("(\\\"(?<key>[^\\\"]+)\\\"\\s*:\\s*(?<value>\\\"[^\\\"]*\\\"))|(\\\"(?<key>[^\\\"]+)\\\"\\s*:\\s*(?<value>[^,\\\"\\}]+))", RegexOptions.Compiled);

		private static Regex DATETIME_REGEX = new Regex("^[1-2]\\d\\d\\d/\\d?\\d/\\d?\\d \\d?\\d:\\d\\d:\\d\\d\\.\\d\\d\\d$", RegexOptions.Compiled);

		private static Regex DSA_REGEX = new Regex("(?<value>\\\"[^\\\"]*\\\")|(?<value>[^,\\s\\[\\]]+)", RegexOptions.Compiled);

		private static Regex DESERIALIZE_REGEX = new Regex("(\\{[^\\[\\]\\{\\}]*\\})|(\\[[^\\[\\]\\{\\}]*\\])", RegexOptions.Compiled);

		private static readonly string[] UNICODE_FLAGS = new string[]
		{
			"\\u",
			"/u"
		};

		private static readonly Regex UNICODE_REGEX = new Regex("^[\\\\/]u", RegexOptions.Compiled);

		private static string EncodeString(string text)
		{
			if (text == null || text.Length == 0)
			{
				return text;
			}
			text = text.Replace("\\\\", JsonConvert._BACKSLASH);
			text = text.Replace("\\\"", JsonConvert._QUOTECHAR);
			MatchCollection matchCollection = JsonConvert.ENCODE_REGEX.Matches(text);
			foreach (Match match in matchCollection)
			{
				text = text.Replace(match.Value, match.Value.Replace(":", JsonConvert._COLONCHAR).Replace(",", JsonConvert._COMMACHAR).Replace("{", JsonConvert._BRACES_BOF).Replace("}", JsonConvert._BRACES_EOF).Replace("[", JsonConvert._BRACKETS_BOF).Replace("]", JsonConvert._BRACKETS_EOF));
			}
			return text;
		}

		private static string DecodeString(string text)
		{
			if (text == null || text.Length == 0)
			{
				return text;
			}
			text = text.Replace(JsonConvert._QUOTECHAR, "\"");
			text = text.Replace(JsonConvert._BACKSLASH, "\\");
			return text.Replace(JsonConvert._COLONCHAR, ":").Replace(JsonConvert._COMMACHAR, ",").Replace(JsonConvert._BRACES_BOF, "{").Replace(JsonConvert._BRACES_EOF, "}").Replace(JsonConvert._BRACKETS_BOF, "[").Replace(JsonConvert._BRACKETS_EOF, "]");
		}

		private static string EncodeScript(string script, bool unicodeEncoding = false)
		{
			if (script == null || script.Length == 0)
			{
				return script;
			}
			script = script.Replace("\\", "\\\\");
			script = script.Replace("\"", "\\\"");
			script = script.Replace("\r", "\\r");
			script = script.Replace("\n", "\\n");
			if (unicodeEncoding)
			{
				script = JsonConvert.EncodeUnicode(script);
			}
			return script;
		}

		private static string EncodeScript(object script, bool unicodeEncoding = false)
		{
			if (script == null)
			{
				return null;
			}
			if (script is string)
			{
				return JsonConvert.EncodeScript((string)script, unicodeEncoding);
			}
			return script.ToString();
		}

		private static string DecodeScript(string script, bool unicodeDecoding)
		{
			if (script == null || script.Length == 0)
			{
				return script;
			}
			script = script.Replace("\\\\", "\\");
			script = script.Replace("\\\"", "\"");
			script = script.Replace("\\r", "\r");
			script = script.Replace("\\n", "\n");
			if (unicodeDecoding)
			{
				script = JsonConvert.DecodeUnicode(script);
			}
			return script;
		}

		private static JsonObject DeserializeSingletonObject(JsonObject jsonObject, string text, bool unicodeDecoding)
		{
			JsonObject jsonObject2 = new JsonObject();
			MatchCollection matchCollection = JsonConvert.DSO_REGEX.Matches(text);
			foreach (Match match in matchCollection)
			{
				string key = JsonConvert.DecodeString(match.Groups["key"].Value);
				string text2 = match.Groups["value"].Value;
				object value = null;
				if (!jsonObject.TryGetValue(text2, out value))
				{
					text2 = JsonConvert.DecodeScript(JsonConvert.DecodeString(text2), unicodeDecoding);
					if (text2 != "null")
					{
						decimal num;
						if (text2[0] == '"')
						{
							text2 = text2.Substring(1, text2.Length - 2);
							DateTime dateTime;
							if (JsonConvert.DATETIME_REGEX.IsMatch(text2) && DateTime.TryParseExact(text2, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.None, out dateTime))
							{
								value = dateTime;
							}
							else
							{
								value = text2;
							}
						}
						else if (text2 == "true")
						{
							value = true;
						}
						else if (text2 == "false")
						{
							value = false;
						}
						else if (decimal.TryParse(text2, out num))
						{
							value = num;
						}
					}
				}
				jsonObject2[key] = value;
			}
			return jsonObject2;
		}

		private static JsonArray DeserializeSingletonArray(JsonObject jsonObject, string text, bool unicodeDecoding)
		{
			JsonArray jsonArray = new JsonArray();
			MatchCollection matchCollection = JsonConvert.DSA_REGEX.Matches(text);
			foreach (Match match in matchCollection)
			{
				string text2 = match.Groups["value"].Value;
				object item = null;
				if (!jsonObject.TryGetValue(text2, out item))
				{
					text2 = JsonConvert.DecodeScript(JsonConvert.DecodeString(text2), unicodeDecoding);
					if (text2 != "null")
					{
						decimal num;
						if (text2[0] == '"')
						{
							text2 = text2.Substring(1, text2.Length - 2);
							DateTime dateTime;
							if (JsonConvert.DATETIME_REGEX.IsMatch(text2) && DateTime.TryParseExact(text2, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.None, out dateTime))
							{
								item = dateTime;
							}
							else
							{
								item = text2;
							}
						}
						else if (text2 == "true")
						{
							item = true;
						}
						else if (text2 == "false")
						{
							item = false;
						}
						else if (decimal.TryParse(text2, out num))
						{
							item = num;
						}
					}
				}
				jsonArray.Add(item);
			}
			return jsonArray;
		}

		private static string Deserialize(JsonObject jsonObject, string text, bool unicodeDecoding)
		{
			text = JsonConvert.EncodeString(text);
			int num = 0;
			while (true)
			{
				MatchCollection matchCollection = JsonConvert.DESERIALIZE_REGEX.Matches(text);
				if (matchCollection != null && matchCollection.Count != 0)
				{
                    IEnumerator enumerator  = matchCollection.GetEnumerator();
					
						while (enumerator.MoveNext())
						{
							Match match = (Match)enumerator.Current;
							string text2 = "___key" + num + "___";
							string value = match.Value;
							if (value[0] == '{')
							{
								jsonObject.Add(text2, JsonConvert.DeserializeSingletonObject(jsonObject, value, unicodeDecoding));
							}
							else
							{
								jsonObject.Add(text2, JsonConvert.DeserializeSingletonArray(jsonObject, value, unicodeDecoding));
							}
							text = text.Replace(value, text2);
							num++;
						}
                  

                }
				break;
			}
			return text;
		}

		public static string EncodeUnicode(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			string[] array = new string[s.Length];
			for (int i = 0; i < s.Length; i++)
			{
				array[i] = "\\u" + char.ConvertToUtf32(s[i].ToString(), 0).ToString("x");
			}
			return string.Concat(array);
		}

		public static string DecodeUnicode(string s)
		{
			if (string.IsNullOrEmpty(s) || !JsonConvert.UNICODE_REGEX.IsMatch(s))
			{
				return s;
			}
			string[] array = s.Split(JsonConvert.UNICODE_FLAGS, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Length < 2 || text.Trim() == string.Empty)
				{
					if (i > 0)
					{
						array[i] = "\\u" + array[i];
					}
				}
				else
				{
					for (int j = (text.Length > 4) ? 4 : text.Length; j >= 2; j--)
					{
						try
						{
							if (j > text.Length)
							{
								array[i] = char.ConvertFromUtf32(Convert.ToInt32(text.Substring(0, j), 16)) + text.Substring(j);
							}
							else
							{
								array[i] = char.ConvertFromUtf32(Convert.ToInt32(text.Substring(0, j), 16));
							}
							break;
						}
						catch
						{
						}
					}
				}
			}
			return string.Concat(array);
		}

		public static JsonObject DeserializeObject(string text, bool unicodeDecoding = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			JsonObject jsonObject = new JsonObject();
			return jsonObject[JsonConvert.Deserialize(jsonObject, text.Trim(), unicodeDecoding)] as JsonObject;
		}

		public static JsonObject DeserializeObject(byte[] data, bool unicodeDecoding = false)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), unicodeDecoding);
		}

		public static JsonArray DeserializeArray(string text, bool unicodeDecoding = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			JsonObject jsonObject = new JsonObject();
			return jsonObject[JsonConvert.Deserialize(jsonObject, text.Trim(), unicodeDecoding)] as JsonArray;
		}

		public static JsonArray DeserializeArray(byte[] data, bool unicodeDecoding = false)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			return JsonConvert.DeserializeArray(Encoding.UTF8.GetString(data), unicodeDecoding);
		}

		public static string SerializeObject(JsonObject jsonObject, bool unicodeEncoding = false)
		{
			return JsonConvert.SerializeData(jsonObject, false);
		}

		public static string SerializeData(IDictionary<string, object> jsonData, bool unicodeEncoding = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			foreach (KeyValuePair<string, object> current in jsonData)
			{
				if (current.Value == null || current.Value == DBNull.Value)
				{
					stringBuilder.AppendFormat("\"{0}\":null,", current.Key);
				}
				else if (current.Value is JsonObject)
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, JsonConvert.SerializeObject((JsonObject)current.Value, false));
				}
				else if (current.Value is IDictionary<string, object>)
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, JsonConvert.SerializeData((IDictionary<string, object>)current.Value, false));
				}
				else if (current.Value is JsonArray)
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, JsonConvert.SerializeArray((JsonArray)current.Value, false));
				}
				else if (current.Value is DbFastRow)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray(((DbFastRow)current.Value).Values, false));
				}
				else if (current.Value is IEnumerable<object>)
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, JsonConvert.SerializeArray((IEnumerable<object>)current.Value, false));
				}
				else if (current.Value is bool)
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, current.Value.ToString().ToLower());
				}
				else if (current.Value is string)
				{
					stringBuilder.AppendFormat("\"{0}\":\"{1}\",", current.Key, JsonConvert.EncodeScript((string)current.Value, unicodeEncoding));
				}
				else if (current.Value is DateTime)
				{
					stringBuilder.AppendFormat("\"{0}\":\"{1}\",", current.Key, ((DateTime)current.Value).ToString("yyyy/MM/dd HH:mm:ss.fff"));
				}
				else if (current.Value is Enum)
				{
					stringBuilder.AppendFormat("\"{0}\":\"{1}\",", current.Key, current.Value.ToString());
				}
				else
				{
					stringBuilder.AppendFormat("\"{0}\":{1},", current.Key, current.Value);
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string SerializeData(IDictionary<string, object> jsonData, string[] keys, bool unicodeEncoding = false)
		{
			if (keys == null || keys.Length == 0)
			{
				return JsonConvert.SerializeData(jsonData, false);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			for (int i = 0; i < keys.Length; i++)
			{
				string text = keys[i];
				object obj;
				if (jsonData.TryGetValue(text, out obj))
				{
					if (obj == null || obj == DBNull.Value)
					{
						stringBuilder.AppendFormat("\"{0}\":null,", text);
					}
					else if (obj is JsonObject)
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, JsonConvert.SerializeObject((JsonObject)obj, false));
					}
					else if (obj is IDictionary<string, object>)
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, JsonConvert.SerializeData((IDictionary<string, object>)obj, false));
					}
					else if (obj is JsonArray)
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, JsonConvert.SerializeArray((JsonArray)obj, false));
					}
					else if (obj is DbFastRow)
					{
						stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray(((DbFastRow)obj).Values, false));
					}
					else if (obj is IEnumerable<object>)
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, JsonConvert.SerializeArray((IEnumerable<object>)obj, false));
					}
					else if (obj is bool)
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, obj.ToString().ToLower());
					}
					else if (obj is string)
					{
						stringBuilder.AppendFormat("\"{0}\":\"{1}\",", text, JsonConvert.EncodeScript((string)obj, unicodeEncoding));
					}
					else if (obj is DateTime)
					{
						stringBuilder.AppendFormat("\"{0}\":\"{1}\",", text, ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss.fff"));
					}
					else if (obj is Enum)
					{
						stringBuilder.AppendFormat("\"{0}\":\"{1}\",", text, obj.ToString());
					}
					else
					{
						stringBuilder.AppendFormat("\"{0}\":{1},", text, obj);
					}
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string SerializeData<T>(IEnumerable<KeyValuePair<string, T>> jsonData, bool unicodeEncoding = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			foreach (KeyValuePair<string, T> current in jsonData)
			{
				if (current.Value == null)
				{
					stringBuilder.AppendFormat("\"{0}\":null,", current.Key);
				}
				else
				{
					stringBuilder.AppendFormat("\"{0}\":\"{1}\",", current.Key, JsonConvert.EncodeScript(current.Value, unicodeEncoding));
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string SerializeData<T>(IEnumerable<KeyValuePair<string, T>> jsonData, string[] keys, bool unicodeEncoding = false)
		{
			if (keys == null || keys.Length == 0)
			{
				return JsonConvert.SerializeData<T>(jsonData, false);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			foreach (KeyValuePair<string, T> current in jsonData)
			{
				if (Array.IndexOf<string>(keys, current.Key) >= 0)
				{
					if (current.Value == null)
					{
						stringBuilder.AppendFormat("\"{0}\":null,", current.Key);
					}
					else
					{
						stringBuilder.AppendFormat("\"{0}\":\"{1}\",", current.Key, JsonConvert.EncodeScript(current.Value, unicodeEncoding));
					}
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string SerializeArray(JsonArray jsonArray, bool unicodeEncoding = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			for (int i = 0; i < jsonArray.Count; i++)
			{
				object obj = jsonArray[i];
				if (obj is JsonObject)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeObject((JsonObject)obj, false));
				}
				else if (obj is IDictionary<string, object>)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeData((IDictionary<string, object>)obj, false));
				}
				else if (obj is JsonArray)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray((JsonArray)obj, false));
				}
				else if (obj is DbFastRow)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray(((DbFastRow)obj).Values, false));
				}
				else if (obj is IEnumerable<object>)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray((IEnumerable<object>)obj, false));
				}
				else if (obj is bool)
				{
					stringBuilder.AppendFormat("{0},", obj.ToString().ToLower());
				}
				else if (obj is string)
				{
					stringBuilder.AppendFormat("\"{0}\",", JsonConvert.EncodeScript((string)obj, unicodeEncoding));
				}
				else if (obj is DateTime)
				{
					stringBuilder.AppendFormat("\"{0}\",", ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss.fff"));
				}
				else if (obj is Enum)
				{
					stringBuilder.AppendFormat("\"{0}\",", obj.ToString());
				}
				else
				{
					stringBuilder.AppendFormat("{0},", obj);
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string SerializeArray(IEnumerable<object> jsonArray, bool unicodeEncoding = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			foreach (object current in jsonArray)
			{
				if (current is JsonObject)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeObject((JsonObject)current, false));
				}
				else if (current is IDictionary<string, object>)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeData((IDictionary<string, object>)current, false));
				}
				else if (current is JsonArray)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray((JsonArray)current, false));
				}
				else if (current is DbFastRow)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray(((DbFastRow)current).Values, false));
				}
				else if (current is IEnumerable<object>)
				{
					stringBuilder.AppendFormat("{0},", JsonConvert.SerializeArray((IEnumerable<object>)current, false));
				}
				else if (current is bool)
				{
					stringBuilder.AppendFormat("{0},", current.ToString().ToLower());
				}
				else if (current is string)
				{
					stringBuilder.AppendFormat("\"{0}\",", JsonConvert.EncodeScript((string)current, unicodeEncoding));
				}
				else if (current is DateTime)
				{
					stringBuilder.AppendFormat("\"{0}\",", ((DateTime)current).ToString("yyyy/MM/dd HH:mm:ss.fff"));
				}
				else if (current is Enum)
				{
					stringBuilder.AppendFormat("\"{0}\",", current.ToString());
				}
				else
				{
					stringBuilder.AppendFormat("{0},", current);
				}
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
