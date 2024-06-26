﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class NadLowSmoother
	{
		public static Nad Make(Nad nad)
		{
			ProgressShower.Show("Nad low smoothing...");
			float step = (int)Math.Max(1, nad.Width / 2000f);

			float _sps = nad.Width / nad._duration;

			Nad nadOut = new Nad(nad._channelsCount, nad.Width, nad._duration, nad._cs, nad._specturmSize);

			JustCopyToNew();

			for (int s = 0; s < nad.Width; s++)
			{
				MakeOne(s);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / nad.Width);
			}

			ProgressShower.Close();
			return nadOut;

			void MakeOne(int s)
			{
				for (int c = 0; c < nad._samples[s].Height; c++)
				{
					float frequency = SpectrumFinder._frequenciesLg[nad._samples[s]._indexes[c]];
					int n = (int)MathF.Ceiling(_sps / frequency);

					if (n > 1)
					{
						ushort index = nad._samples[s]._indexes[c];
						float amplitude = nad._samples[s]._amplitudes[c];

						for (int ns = s + 1; (ns < s + n) && (ns < nadOut._samples.Length); ns++)
							nadOut._samples[ns].Add(index, amplitude);
					}
				}
			}

			void JustCopyToNew()
			{
				for (int s = 0; s < nad.Width; s++)
				{
					nadOut._samples[s] = new NadSample(nad._samples[s].Height);
					for (int c = 0; c < nad._samples[s].Height; c++)
					{						
						nadOut._samples[s]._amplitudes[c] = nad._samples[s]._amplitudes[c];
						nadOut._samples[s]._indexes[c] = nad._samples[s]._indexes[c];
					}
				}
			}
		}
	}
}
