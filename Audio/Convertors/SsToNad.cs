using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToNad
	{
		public static Nad Make(SS ss, bool comparison)
		{
			ProgressShower.Show("Ss to nad...");
			int progressStep = ss._s.Length / 1000;

			AP.FftSize = ss.Height * 2;

			Nad nad = new Nad(AP._channels, ss._s.Length, ss._s.Length / ss._sps);

			for (int s = 0; s < ss._s.Length; s++)
			{
				nad._samples[s] = MakeSample(ss._s[s]);

				if (comparison && s > 0)
					nad._samples[s] = Nad.Compare(nad._samples[s - 1], nad._samples[s]);

				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / ss._s.Length);
			}

			ProgressShower.Close();

			return nad;
		}

		public static NadSample MakeSample(float[] spectrum)
		{
			NadSample ns = new NadSample(AP._channels);

			ns._indexes = PeaksFinder.Find(spectrum, AP._channels, AP._peakSize);
			ns._amplitudes = MathE.GetValues(spectrum, ns._indexes);
			ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLogarithmic, ns._indexes);

			return ns;
		}
	}
}
