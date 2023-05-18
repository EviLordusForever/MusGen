using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToMultiNad
	{
		public static Nad Make(SS ss)
		{
			AP.FftSize = ss.Height * 2;

			ProgressShower.Show("Ss to MultiNad...");
			int progressStep = (int)MathF.Ceiling(ss.Width / 1000f);

			AP.FftSize = ss.Height * 2;

			Nad nad = new Nad(AP._channels, ss._s.Length, 1f * ss._s.Length / ss._sps);

			for (int s = 0; s < ss._s.Length; s++)
			{
				nad._samples[s] = MakeSamplePlus(ss._s[s]);

				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / ss._s.Length);
			}

			ProgressShower.Close();

			return nad;
		}

		public static NadSample MakeSamplePlus(float[] spectrum)
		{
			List<float> amps;
			int[] indexes = PeaksFinding.PeaksFinder.FindEvery_Keras(spectrum, out amps).ToArray();
			var ns = new NadSample(indexes.Length);
			ns._indexes = indexes;
			ns._amplitudes = amps.ToArray();
			ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLogarithmic, ns._indexes);

			return ns;
		}
	}
}
