using System;
using System.Collections.Generic;

namespace API.Json
{
	public sealed class JsonObject : Dictionary<string, object>
	{
		internal static readonly char[] WhitespaceChars = new char[]
		{
			'\t',
			'\n',
			'\v',
			'\f',
			'\r',
			' ',
			'\u0085',
			'\u00a0',
			'\u1680',
			'\u2000',
			'\u2001',
			'\u2002',
			'\u2003',
			'\u2004',
			'\u2005',
			'\u2006',
			'\u2007',
			'\u2008',
			'\u2009',
			'\u200a',
			'​',
			'\u2028',
			'\u2029',
			'\u3000',
			'﻿',
			'/'
		};

		public static readonly JsonObject Empty = new JsonObject();

		public new object this[string key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				object result;
				if (base.TryGetValue(key, out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					base.Remove(key);
					return;
				}
				base[key] = value;
			}
		}

		public JsonObject()
		{
		}


		public object SelectObject(string path)
		{
			if (path == null)
			{
				return null;
			}
			path = path.Trim(JsonObject.WhitespaceChars);
			if (path.Length == 0)
			{
				return null;
			}
			string[] array = path.Split(new char[]
			{
				'/'
			});
			string key = array[0];
			object obj = this[key];
			if (obj != null)
			{
				for (int i = 1; i < array.Length; i++)
				{
					JsonObject jsonObject = obj as JsonObject;
					if (jsonObject == null)
					{
						obj = null;
					}
					else
					{
						obj = jsonObject[array[i]];
					}
					if (obj == null)
					{
						break;
					}
				}
			}
			return obj;
		}

		public T SelectObject<T>(string path)
		{
			object obj = this.SelectObject(path);
			if (obj == null)
			{
				return default(T);
			}
			if (obj is decimal)
			{
				return (T)((object)Convert.ChangeType(obj, typeof(T)));
			}
			return (T)((object)obj);
		}

		public T SelectObject<T>(string path, T defaultValue)
		{
			object obj = this.SelectObject(path);
			if (obj == null)
			{
				return defaultValue;
			}
			if (obj is decimal)
			{
				return (T)((object)Convert.ChangeType(obj, typeof(T)));
			}
			return (T)((object)obj);
		}
	}
}
