using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Core
{
	public static class Convertor
	{
		public static byte[] StringToByteArray(string hex)
		{
			return Encoding.ASCII.GetBytes(hex);
		}

		public static double BytesToDouble(byte firstByte, byte secondByte)
		{
			// convert two bytes to one double in the range -1 to 1

			int s = secondByte << 8 | firstByte;
			return s / 32768.0;
		}

		public static byte[] FloatToBytes(float d)
		{
			return BitConverter.GetBytes(d);
		}
	}
}
