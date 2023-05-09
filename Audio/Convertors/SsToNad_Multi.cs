﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToNad_Multi
	{
		public static Nad Make(SS ss)
		{
			AP.FftSize = ss.Height * 2;

			ProgressShower.Show("Ss to multinad...");
			int progressStep = ss._s.Length / 1000;

			AP.FftSize = ss.Height * 2;

			Nad nad = new Nad(AP._channels, ss._s.Length, ss._s.Length / ss._sps);

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
			int[] indexes = PeaksFinder.FindEvery(spectrum, AP._peakSize2, AP._lowestPeak, AP._peaksLimit).ToArray();
			var ns = new NadSample(indexes.Length);
			ns._indexes = indexes;
			ns._amplitudes = MathE.GetValues(spectrum, ns._indexes);
			ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLogarithmic, ns._indexes);

			return ns;
		}
	}
}