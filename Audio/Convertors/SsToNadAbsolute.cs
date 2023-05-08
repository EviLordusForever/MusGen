using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToNadAbsolute
	{
		public static Nad Make(SS ss)
		{
			ProgressShower.Show("Ss to nad absolute...");
			int progressStep = ss.Width / 1000;

			AP.FftSize = ss.Height * 2;
			SpectrumFinder.Init();

			Nad nad = new Nad(AP._channels, ss._s.Length, ss._s.Length / AP._sps);

			for (int s = 0; s < ss._s.Length; s++)
			{
				nad._samples[s] = MakeSample(ss._s[s]);

				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / ss._s.Length);
			}

			ProgressShower.Close();
			Logger.Log("Ss converted to nad absolute.");

			return nad;
		}

		public static NadSample MakeSample(float[] spectrum)
		{
			NadSample ns = new NadSample(spectrum.Length);

			ns._indexes = ArrayE.OneToN(spectrum.Length);
			ns._amplitudes = spectrum;
			ns._frequencies = SpectrumFinder._frequenciesLogarithmic;

			return ns;
		}
	}
}
