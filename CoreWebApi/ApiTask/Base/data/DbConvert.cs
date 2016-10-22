using System;

namespace API.Data
{
	public static class DbConvert
	{
		public static bool IsDbNull(object value)
		{
			return value == null || value == DBNull.Value;
		}

		public static short? ToInt16(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new short?(Convert.ToInt16(value));
		}

		public static short ToInt16(object value, short _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToInt16(value);
		}

		public static int? ToInt32(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new int?(Convert.ToInt32(value));
		}

		public static int ToInt32(object value, int _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToInt32(value);
		}

		public static long? ToInt64(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new long?(Convert.ToInt64(value));
		}

		public static long ToInt64(object value, long _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToInt64(value);
		}

		public static bool? ToBoolean(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new bool?(Convert.ToBoolean(value));
		}

		public static bool ToBoolean(object value, bool _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToBoolean(value);
		}

		public static byte? ToByte(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new byte?(Convert.ToByte(value));
		}

		public static byte ToByte(object value, byte _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToByte(value);
		}

		public static char? ToChar(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new char?(Convert.ToChar(value));
		}

		public static char ToChar(object value, char _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToChar(value);
		}

		public static DateTime? ToDateTime(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new DateTime?(Convert.ToDateTime(value));
		}

		public static DateTime ToDateTime(object value, DateTime _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToDateTime(value);
		}

		public static decimal? ToDecimal(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new decimal?(Convert.ToDecimal(value));
		}

		public static decimal ToDecimal(object value, decimal _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToDecimal(value);
		}

		public static double? ToDouble(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new double?(Convert.ToDouble(value));
		}

		public static double ToDouble(object value, double _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToDouble(value);
		}

		public static sbyte? ToSByte(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new sbyte?(Convert.ToSByte(value));
		}

		public static sbyte ToSByte(object value, sbyte _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToSByte(value);
		}

		public static float? ToSingle(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new float?(Convert.ToSingle(value));
		}

		public static float ToSingle(object value, float _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToSingle(value);
		}

		public static string ToString(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return Convert.ToString(value);
		}

		public static string ToString(object value, string _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToString(value);
		}

		public static ushort? ToUInt16(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new ushort?(Convert.ToUInt16(value));
		}

		public static ushort ToUInt16(object value, ushort _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToUInt16(value);
		}

		public static uint? ToUInt32(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new uint?(Convert.ToUInt32(value));
		}

		public static uint ToUInt32(object value, uint _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToUInt32(value);
		}

		public static ulong? ToUInt64(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new ulong?(Convert.ToUInt64(value));
		}

		public static ulong ToUInt64(object value, ulong _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return Convert.ToUInt64(value);
		}

		public static Guid? ToGuid(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new Guid?((Guid)value);
		}

		public static Guid ToGuid(object value, Guid _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return (Guid)value;
		}

		public static TimeSpan? ToTimeSpan(object value)
		{
			if (DbConvert.IsDbNull(value))
			{
				return null;
			}
			return new TimeSpan?(TimeSpan.ParseExact(value.ToString(), "HH:mm:ss", null));
		}

		public static TimeSpan ToTimeSpan(object value, TimeSpan _default)
		{
			if (DbConvert.IsDbNull(value))
			{
				return _default;
			}
			return DbConvert.ToTimeSpan(value).Value;
		}

		public static long TimestampToNumber(byte[] value)
		{
			return DbConvert.TimestampToNumber(value, 0L);
		}

		public static long TimestampToNumber(byte[] value, long _default)
		{
			if (value == null || value.Length == 0)
			{
				return _default;
			}
			Array.Reverse(value);
			return BitConverter.ToInt64(value, 0);
		}

		public static long TimestampToNumber(object value)
		{
			return DbConvert.TimestampToNumber(value, 0L);
		}

		public static long TimestampToNumber(object value, long _default)
		{
			byte[] array = value as byte[];
			if (array == null || array.Length == 0)
			{
				return _default;
			}
			Array.Reverse(array);
			return BitConverter.ToInt64(array, 0);
		}

		public static byte[] NumberToTimestamp(long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;
		}

		public static T ToEnum<T>(object value) where T : struct
		{
			T result = default(T);
			Enum.TryParse<T>(DbConvert.ToString(value), true, out result);
			return result;
		}
	}
}
