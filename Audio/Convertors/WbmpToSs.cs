using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Security.Cryptography.X509Certificates;

namespace MusGen
{
	public static class WbmpToSs
	{
		public static int width;

		public static SS Make(WriteableBitmap wbmp)
		{
			width = wbmp.PixelWidth - 2;

			SS ss = new SS(width, ReadSps(), AP._cs);
			
			ProgressShower.Show("Image to ss...");
			int step = (int)Math.Max(wbmp.PixelWidth / 1000f, 1);

			for (int x = 0; x < width; x++)
			{
				ss._s[x] = new float[wbmp.PixelHeight];
				for (int y = 0; y < wbmp.PixelHeight; y++)
				{
					byte b = WBMP.GetGreenValue(wbmp, x, wbmp.PixelHeight - 1 - y);
					ss._s[x][y] = MathE.ToLogScaleReverse(b / 255f, 10);
				}
				if (x % step == 0)
					ProgressShower.Set(1.0 * x / ss.Width);
			}

			ProgressShower.Close();			

			return ss;

			ushort ReadSps()
			{
				bool[] bits = new bool[16];

				for (int i = 0; i < 16; i++)
				{
					var clr = wbmp.GetPixel(wbmp.PixelWidth - 1, wbmp.PixelHeight - 1 - 16 + i);
					bits[i] = clr.G > 125;
				}

				ushort sps = 0;

				for (int i = 0; i < 16; i++)
					if (bits[i])
						sps |= (ushort)(1 << i);

				if (wbmp.GetPixel(wbmp.PixelWidth - 1, wbmp.PixelHeight - 1 - 16 - 1).G < 245)
				{
					sps = AP._sps;//(ushort)(AP.SampleRate / (wbmp.PixelHeight * 2 / 4));
					Logger.Log($"Cannot read \"spectrums per second\" from image. So it was set to {sps}", Brushes.Red);
				}
				else
					Logger.Log($"\"Spectrums per second\" from image was read as {sps}", Brushes.Cyan);

				return sps;
			}
		}
	}
}
