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
	public static class WbmpToSsConvertor
	{
		public static int _lastSample;

		public static SS Make(WriteableBitmap wbmp)
		{
			_lastSample = wbmp.PixelWidth;

			SS ss = new SS(wbmp.PixelWidth, AP._sps);

			int progressStep = wbmp.PixelWidth / 1000;
			ProgressShower.Show("Image to ss...");

			for (int x = 0; x < _lastSample; x++)
			{
				ss._s[x] = new float[wbmp.PixelHeight];
				for (int y = 0; y < wbmp.PixelHeight; y++)
				{
					byte b = WBMP.GetGreenValue(wbmp, x, wbmp.PixelHeight - 1 - y);
					ss._s[x][y] = MathE.ToLogScaleReverse(b / 255f, 10);
				}
				if (x % progressStep == 0)
					ProgressShower.Set(1.0 * x / ss._s.Length);
			}

			ProgressShower.Close();

			return ss;
		}
	}
}
