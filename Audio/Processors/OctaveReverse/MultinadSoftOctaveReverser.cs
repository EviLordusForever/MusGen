using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class MultiNadSoftOctaveReverser
	{
		private static float _max;

		public static Nad Make(Nad nadIn, float octaveShift, bool[] octaves)
		{
			ProgressShower.Show("Nad soft octave reversing...");
			int step = (int)(MathF.Max(1, nadIn.Width / 1000f));

			SsSoftOctaveReverser.Init(nadIn.Width, AP.SpectrumSize);

			Nad nadOut = new Nad(nadIn.Width, nadIn._duration, nadIn._cs, nadIn._specturmSize);

			for (int s = 0; s < nadIn.Width; s++)
			{
				nadOut._samples[s] = MakeOne(nadIn._samples[s], octaveShift, octaves);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / nadIn.Width);
			}

			ProgressShower.Close();

			nadOut.Normalise(_max);

			return nadOut;
		}		

		public static NadSample MakeOne(NadSample nads, float octaveShift, bool[] octaves)
		{
			float[] spectrum = new float[AP.SpectrumSize];

			for (int n = 0; n < nads.Height; n++)
				spectrum[nads._indexes[n]] += nads._amplitudes[n];

			float[] newSpectrum = SsSoftOctaveReverser.MakeOne(spectrum, octaveShift, octaves);

			int count = 0;
			for (int i = 0; i < newSpectrum.Length; i++)
				if (newSpectrum[i] > 0) //ok
					count++;

			NadSample newOne = new NadSample(count);
			int ni = 0;
			for (ushort si = 0; si < newSpectrum.Length; si++)
				if (newSpectrum[si] > 0)
				{
					newOne._indexes[ni] = si;
					newOne._amplitudes[ni] = newSpectrum[si];
					_max = _max > newOne._amplitudes[ni] ? _max : newOne._amplitudes[ni];
					ni++;
				}

			return newOne;
		}
	}
}
