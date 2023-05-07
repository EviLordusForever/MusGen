using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToNadConvertor
	{
		public static Nad Make(SS ss)
		{
			ProgressShower.Show("Ss to nad...");
			int progressStep = ss._s.Length / 1000;

			AP._fftSize = ss._s[0].Length * 2;
			SpectrumFinder.Init();

			Nad nad = new Nad(AP._channels, ss._s.Length, ss._s.Length / AP._sps);

			for (int s = 0; s < ss._s.Length; s++)
			{
				nad._samples[s] = MakeSample(ss._s[s]);

				if (s % progressStep == 0)
					ProgressShower.Set(s / ss._s.Length);
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
