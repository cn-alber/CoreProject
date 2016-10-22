using System;

namespace API.Core.Internals
{
	internal class Base64Encoder
	{
		public static Base64Encoder Encoder = new Base64Encoder();

		private byte[] source;

		private int length;

		private int length2;

		private int blockCount;

		private int paddingCount;

		public static string GetEncodedString(byte[] inputBuffer)
		{
			return Base64Encoder.Encoder.GetEncoded(inputBuffer);
		}

		private void Init(byte[] input)
		{
			this.source = input;
			this.length = input.Length;
			if (this.length % 3 == 0)
			{
				this.paddingCount = 0;
				this.blockCount = this.length / 3;
			}
			else
			{
				this.paddingCount = 3 - this.length % 3;
				this.blockCount = (this.length + this.paddingCount) / 3;
			}
			this.length2 = this.length + this.paddingCount;
		}

		public string GetEncoded(byte[] inputBuffer)
		{
			this.Init(inputBuffer);
			byte[] array = new byte[this.length2];
			for (int i = 0; i < this.length2; i++)
			{
				if (i < this.length)
				{
					array[i] = this.source[i];
				}
				else
				{
					array[i] = 0;
				}
			}
			byte[] array2 = new byte[this.blockCount * 4];
			char[] array3 = new char[this.blockCount * 4];
			for (int j = 0; j < this.blockCount; j++)
			{
				byte b = array[j * 3];
				byte b2 = array[j * 3 + 1];
				byte b3 = array[j * 3 + 2];
				byte b4 = (byte)((b & 252) >> 2);
				byte b5 = (byte)((b & 3) << 4);
				byte b6 = (byte)((b2 & 240) >> 4);
				b6 += b5;
				b5 = (byte)((b2 & 15) << 2);
				byte b7 = (byte)((b3 & 192) >> 6);
				b7 += b5;
				byte b8 = Convert.ToByte( b3 & 63);
				array2[j * 4] = b4;
				array2[j * 4 + 1] = b6;
				array2[j * 4 + 2] = b7;
				array2[j * 4 + 3] = b8;
			}
			for (int k = 0; k < this.blockCount * 4; k++)
			{
				array3[k] = this.SixbitToChar(array2[k]);
			}
			switch (this.paddingCount)
			{
			case 1:
				array3[this.blockCount * 4 - 1] = '=';
				break;
			case 2:
				array3[this.blockCount * 4 - 1] = '=';
				array3[this.blockCount * 4 - 2] = '=';
				break;
			}
			return new string(array3);
		}

		private char SixbitToChar(byte @byte)
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
			if (@byte >= 0 && @byte <= 63)
			{
				return array[(int)@byte];
			}
			return ' ';
		}
	}
}
