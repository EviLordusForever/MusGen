using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class MultiNadSoftOctaveReverser
	{
		public static Nad Make(Nad nadIn)
		{
			ProgressShower.Show("Nad soft octave reversing...");
			int step = (int)(MathF.Max(1, nadIn.Width / 1000f));

			SsSoftOctaveReverser.Init(nadIn.Width, AP.SpectrumSize);

			Nad nadOut = new Nad(nadIn.Width, nadIn._duration);

			for (int s = 0; s < nadIn.Width; s++)
			{
				nadOut._samples[s] = MakeOne(nadIn._samples[s]);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return nadOut;
		}		

		public static NadSample MakeOne(NadSample nads)
		{
			float[] spectrum = new float[AP.SpectrumSize];

			for (int n = 0; n < nads.Height; n++)
				spectrum[nads._indexes[n]] = nads._amplitudes[n];

			float[] newSpectrum = SsSoftOctaveReverser.MakeOne(spectrum);

			int count = 0;
			for (int i = 0; i < newSpectrum.Length; i++)
				if (newSpectrum[i] > 0)
					count++;

			NadSample newOne = new NadSample(count);
			int ni = 0;
			for (int si = 0; si < newSpectrum.Length; si++)
				if (newSpectrum[si] > 0)
				{
					newOne._indexes[ni] = si;
					newOne._amplitudes[ni] = newSpectrum[si];
					newOne._frequencies[ni] = SpectrumFinder._frequenciesLogarithmic[si];
					ni++;
				}

			return newOne;
		}
	}
}
