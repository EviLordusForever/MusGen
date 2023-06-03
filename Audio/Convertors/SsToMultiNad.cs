using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using MusGen.Audio.Processors;

namespace MusGen
{
	public static class SsToMultiNad
	{
		public static Nad Make(SS ss)
		{
			ProgressShower.Show("SS to MNAD...");
			int progressStep = (int)MathF.Ceiling(ss.Width / 1000f);

			//AP.FftSize = ss.Height * 2;

			Nad nad = new Nad(ss.Width, 1f * ss.Width / ss._sps, ss._cs, (ushort)ss.Height);

			for (int s = 0; s < ss._s.Length; s++)
			{
				nad._samples[s] = MakeSample_Multi(ss._s[s]);

				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / ss._s.Length);
			}

			ProgressShower.Close();

			nad = NadCurveEqualiser.Make(nad, SMM._model._curve);
			int[] points = ArrayE.Multiply(AP._normalEQpoints, AP.SpectrumSizeGG);

			float[] curve2 = ArrayE.CreateInterpolatedArray(AP.SpectrumSizeGG, points, AP._normalEQvalues);
			nad = NadCurveEqualiser.Make(nad, curve2);

			return nad;
		}

		public static NadSample MakeSample_Multi(float[] spectrum)
		{
			List<float> amps;
			ushort[] indexes = PeaksFinding.PeaksFinder.FindEvery_By_Stupied(spectrum, out amps).ToArray();
			var ns = new NadSample(indexes.Length);
			ns._indexes = indexes;
			ns._amplitudes = amps.ToArray();

			return ns;
		}
	}
}
