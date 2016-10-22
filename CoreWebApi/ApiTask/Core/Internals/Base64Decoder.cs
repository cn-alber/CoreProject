using System;

namespace API.Core.Internals
{
	internal class Base64Decoder
	{
		public static Base64Decoder Decoder = new Base64Decoder();

		private char[] source;

		private int length;

		private int length2;

		private int length3;

		private int blockCount;

		private int paddingCount;

		public static byte[] GetDecodedBytes(string inputText)
		{
			return Base64Decoder.Decoder.GetDecoded(inputText);
		}

		private void Init(char[] input)
		{
			int num = 0;
			this.source = input;
			this.length = input.Length;
			for (int i = 0; i < 2; i++)
			{
				if (input[this.length - i - 1] == '=')
				{
					num++;
				}
			}
			this.paddingCount = num;
			this.blockCount = this.length / 4;
			this.length2 = this.blockCount * 3;
		}

		public byte[] GetDecoded(string inputText)
		{
			this.Init(inputText.ToCharArray());
			byte[] array = new byte[this.length];
			byte[] array2 = new byte[this.length2];
			for (int i = 0; i < this.length; i++)
			{
				array[i] = this.CharToSixbit(this.source[i]);
			}
			for (int j = 0; j < this.blockCount; j++)
			{
				byte b = array[j * 4];
				byte b2 = array[j * 4 + 1];
				byte b3 = array[j * 4 + 2];
				byte b4 = array[j * 4 + 3];
				byte b5 = (byte)(b << 2);
				byte b6 = (byte)((b2 & 48) >> 4);
				b6 += b5;
				b5 = (byte)((b2 & 15) << 4);
				byte b7 = (byte)((b3 & 60) >> 2);
				b7 += b5;
				b5 = (byte)((b3 & 3) << 6);
				byte b8 = b4;
				b8 += b5;
				array2[j * 3] = b6;
				array2[j * 3 + 1] = b7;
				array2[j * 3 + 2] = b8;
			}
			this.length3 = this.length2 - this.paddingCount;
			byte[] array3 = new byte[this.length3];
			for (int k = 0; k < this.length3; k++)
			{
				array3[k] = array2[k];
			}
			return array3;
		}

		private byte CharToSixbit(char @char)
		{
			char[] array = new char[]
			{
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'I',
				'J',
				'K',
				'L',
				'M',
				'N',
				'O',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				'a',
				'b',
				'c',
				'd',
				'e',
				'f',
				'g',
				'h',
				'i',
				'j',
				'k',
				'l',
				'm',
				'n',
				'o',
				'p',
				'q',
				'r',
				's',
				't',
				'u',
				'v',
				'w',
				'x',
				'y',
				'z',
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'+',
				'/'
			};
			if (@char == '=')
			{
				return 0;
			}
			for (int i = 0; i < 64; i++)
			{
				if (array[i] == @char)
				{
					return (byte)i;
				}
			}
			return 0;
		}
	}
}
