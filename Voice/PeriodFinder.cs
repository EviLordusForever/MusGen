using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;

namespace MusGen.Voice
{
	public static class PerioudFinder
	{
		public static int minPeriod = 80;    //
		public static int maxPeriod = 600;
		public static int points = 50;

		public static double FindPeriod(Wav wav, int start, int end)
		{
			//end - start MORE than maxPeriod
			int t = 0;
			float[] mismatches = new float[maxPeriod - minPeriod];

			double actualPeriod = 1;
			double minMismatch = 1;

			int delta = ((end - start) - maxPeriod) / points;

			for (double perioud = minPeriod; perioud < maxPeriod; perioud *= 1.025)
			{
				double mismatch = 0;

				for (int sample = start; sample < end - maxPeriod; sample += delta)
				{
						mismatch += Math.Abs(wav.L[sample] - wav.L[sample + (int)perioud]);
						if (wav.channels == 2) //Is this necessary?
							mismatch += Math.Abs(wav.R[sample] - wav.R[sample + (int)perioud]);
				}
				mismatch /= points;

				mismatches[t++] = (float)mismatch;

				if (mismatch < minMismatch)
				{
					minMismatch = mismatch;
					actualPeriod = perioud;
				}
			}

			//GraficsMaker.MakeGrafic(mismatches, 2000); //
			//UserAsker.Ask("Период: " + actualFrequency);
			return actualPeriod;
		}

		public static double goertzel(double[] sngData, long N, float freq, int samplerate)
		{
			double skn, skn1, skn2;
			skn = skn1 = skn2 = 0;

			double c = 2 * Math.PI * freq / samplerate;
			double cosan = Math.Cos(c);

			for (int i = 0; i < N; i++)
			{
				skn2 = skn1;
				skn1 = skn;
				skn = 2 * cosan * skn1 - skn2 + sngData[i];
			}

			return skn - Math.Exp(-c) * skn1;
		}
	}
}