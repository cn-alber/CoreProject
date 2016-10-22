using System;
using System.Collections.Generic;

namespace API.Core
{
	public static class Random
	{
		private static string[] charArray = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"i",
			"j",
			"k",
			"l",
			"m",
			"n",
			"o",
			"p",
			"q",
			"r",
			"s",
			"t",
			"u",
			"v",
			"w",
			"x",
			"y",
			"z"
		};

		public static string GetString()
		{
			return Random.GetString(8, false);
		}

		public static string GetString(int length)
		{
			return Random.GetString(length, false);
		}

		public static string GetString(int length, bool alternation)
		{
			System.Random random = new System.Random();
			string text = string.Empty;
			int i = 0;
			if (alternation)
			{
				while (i < length)
				{
					if (Math.IEEERemainder((double)i, 2.0) == 0.0)
					{
						text += Random.charArray[random.Next(0, 9)];
					}
					else
					{
						text += Random.charArray[random.Next(10, 35)];
					}
					i++;
				}
			}
			else
			{
				while (i < length)
				{
					text += Random.charArray[random.Next(0, 35)];
					i++;
				}
			}
			return text;
		}

		public static IList<string> GetStringList(int length, bool alternation, int size, bool allowRepeat = false)
		{
			int num = Random.charArray.Length * length;
			if (num < size && !allowRepeat)
			{
				throw new ArgumentOutOfRangeException("size", "The size out of range.");
			}
			List<string> list = new List<string>(size);
			System.Random random = new System.Random();
			while (list.Count < size)
			{
				string text = string.Empty;
				int i = 0;
				if (alternation)
				{
					while (i < length)
					{
						if (Math.IEEERemainder((double)i, 2.0) == 0.0)
						{
							text += Random.charArray[random.Next(0, 9)];
						}
						else
						{
							text += Random.charArray[random.Next(10, 35)];
						}
						i++;
					}
				}
				else
				{
					while (i < length)
					{
						text += Random.charArray[random.Next(0, 35)];
						i++;
					}
				}
				if (allowRepeat || !list.Contains(text))
				{
					list.Add(text);
				}
			}
			return list;
		}

		public static int GetNumber()
		{
			return Random.GetNumber(10000000, 99999999);
		}

		public static int GetNumber(int length)
		{
			if (length >= 10)
			{
				return Random.GetNumber(1000000000, 2147483647);
			}
			int max = (int)Math.Pow(10.0, (double)(length + 1)) - 1;
			int min = (int)Math.Pow(10.0, (double)length);
			return Random.GetNumber(min, max);
		}

		public static int GetNumber(int min, int max)
		{
			System.Random random = new System.Random();
			return random.Next(min, max);
		}

		public static IList<int> GetNumberList(int min, int max, int size, bool allowRepeat = false)
		{
			int num = max - min;
			if (num < size && !allowRepeat)
			{
				throw new ArgumentOutOfRangeException("size", "The size out of the min to max range.");
			}
			List<int> list = new List<int>(size);
			System.Random random = new System.Random();
			while (list.Count < size)
			{
				int item = random.Next(min, max);
				if (allowRepeat || !list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
}
