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
	public static class NadToWbmp
	{
		public static WriteableBitmap Make(Nad nad)
		{
			WriteableBitmap wbmp = WBMP.Create(nad.Width, nad._specturmSize);

			ProgressShower.Show("Making image from nad...");

			int progressStep = (int)Math.Max(1, nad.Width / 1000f);			

			for (int s = 0; s < nad.Width; s++)
			{
				byte[] bytes = new byte[nad._specturmSize];

				for (int c = 0; c < nad._samples[s].Height; c++)
					bytes[nad._samples[s]._indexes[c]] = (byte)(MathE.ToLogScale(nad._samples[s]._amplitudes[c], 10) * 255);

				WBMP.WriteByteArrayToColumn(wbmp, bytes, s);

				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / nad.Width);
			}

			ProgressShower.Close();

			return wbmp;
		}
	}
}
