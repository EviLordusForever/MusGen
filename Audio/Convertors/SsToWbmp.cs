using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MusGen
{
	public static class SsToWbmp
	{
		public static int _lastSample;

		public static WriteableBitmap Make(SS ss)
		{
			WriteableBitmap wbmp = WBMP.Create(ss.Width, ss.Height);

			ProgressShower.Show("Making image from ss...");

			int progressStep = (int)Math.Max(1, ss.Width / 1000f);

			byte[] bytes = new byte[ss.Height];

			for (int c = 0; c < ss.Width; c++)
			{
				for (int b = 0; b < bytes.Length; b++)
					bytes[b] = (byte)(MathE.ToLogScale(ss._s[c][b], 10) * 255);

				WBMP.WriteByteArrayToColumn(wbmp, bytes, c);

				if (c % progressStep == 0)
					ProgressShower.Set(1.0 * c / ss.Width);
			}

			AddSps();

			ProgressShower.Close();

			return wbmp;

			void AddSps()
			{
				UInt16 sps = (UInt16)AP._sps;
				bool[] bits = new bool[16];

				for (int i = 0; i < 16; i++)
					bits[i] = (sps & (1 << i)) != 0;

				for (int i = 0; i < 16; i++)
				{
					wbmp.SetPixel(wbmp.PixelWidth - 1, wbmp.PixelHeight - 1 - 16 + i, bits[i] ? Colors.White : Colors.Black);
					wbmp.SetPixel(wbmp.PixelWidth - 2, wbmp.PixelHeight - 1 - 16 + i, Colors.White);
				}
				wbmp.SetPixel(wbmp.PixelWidth - 2, wbmp.PixelHeight - 1 - 16 - 1, Colors.White);
				wbmp.SetPixel(wbmp.PixelWidth - 1, wbmp.PixelHeight - 1 - 16 - 1, Colors.White);
			}
		}
	}
}
