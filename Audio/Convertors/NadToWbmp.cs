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
			int width = nad.Width;
			if (width > 65535)
			{
				Logger.Log($"Nad width is {width}, so image will be scaled down to 65535.", System.Windows.Media.Brushes.Orange);
				width = 65535;
			}

			WriteableBitmap wbmp = WBMP.Create(width, nad._specturmSize);

			ProgressShower.Show("Making image from nad...");

			int progressStep = (int)Math.Max(1, nad.Width / 1000f);

			if (nad.Width == width)
				for (int s = 0; s < nad.Width; s++) 
				{
					byte[] bytes = new byte[nad._specturmSize];

					for (int c = 0; c < nad._samples[s].Height; c++)
						bytes[nad._samples[s]._indexes[c]] = (byte)(MathE.ToLogScale(nad._samples[s]._amplitudes[c], 10) * 255);

					WBMP.WriteByteArrayToColumn(wbmp, bytes, s);

					if (s % progressStep == 0)
						ProgressShower.Set(1.0 * s / nad.Width);
				}
			else
			{
				int pns = 0;
				int count = 0;
				float[] floats = new float[nad._specturmSize];
				byte[] bytes = new byte[nad._specturmSize];

				for (int s = 0; s < nad.Width; s++)
				{
					int ns = (int)MathF.Floor(1f * s / nad.Width * width);

					if (ns != pns)
					{
						bytes = new byte[nad._specturmSize];

						for (int i = 0; i < nad._specturmSize; i++)
							bytes[i] = (byte)(floats[i] / count * 255);

						WBMP.WriteByteArrayToColumn(wbmp, bytes, ns);
						
						count = 0;
						pns = ns;
						floats = new float[nad._specturmSize];
					}

					count++;

					for (int c = 0; c < nad._samples[s].Height; c++)
						floats[nad._samples[s]._indexes[c]] += MathE.ToLogScale(nad._samples[s]._amplitudes[c], 10);

					if (s % progressStep == 0)
						ProgressShower.Set(1.0 * s / nad.Width);
				}
			}

			ProgressShower.Close();

			return wbmp;
		}
	}
}
