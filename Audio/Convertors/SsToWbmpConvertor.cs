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
	public static class SsToWbmpConvertor
	{
		public static int _lastSample;

		public static WriteableBitmap Make(SS ss)
		{
			_lastSample = ss._s.Length;

			WriteableBitmap wbmp = WBMP.Create(ss._s.Length, AP._fftSize / 2);

			ProgressShower.Show("Making image from ss...");

			int progressStep = ss._s.Length / 1000;

			byte[] bytes = new byte[ss._s[0].Length];

			for (int c = 0; c < _lastSample; c++)
			{
				for (int b = 0; b < bytes.Length; b++)
					bytes[b] = (byte)(MathE.ToLogScale(ss._s[c][b], 10) * 255);

				WBMP.WriteByteArrayToColumn(wbmp, bytes, c);

				if (c % progressStep == 0)
					ProgressShower.Set(1.0 * c / ss._s.Length);
			}

			ProgressShower.Close();

			return wbmp;
		}
	}
}
