using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.Voice
{
	public static class PerioudFinder
	{
		public static int minPeriod = 10;    //
		public static int maxPeriod = 1000;
		public static int points = 30;

		public static double FindFrequency(Wav wav, int start, int end)
		{
			int t = 0;
			float[] mismatches = new float[maxPeriod - minPeriod];

			double actualFrequency = 0;
			double minMismatch = 1;

			int delta = (end - start) / points;

			for (double perioud = minPeriod; perioud < maxPeriod; perioud *= 1.05)
			{
				double mismatch = 0;

				for (int sample = start; sample < end - 1; sample += delta)
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
					actualFrequency = perioud;
				}
			}

			GraficsMaker.MakeGrafic(mismatches, 2000); //
			UserAsker.Ask("Период: " + actualFrequency);
			return actualFrequency;
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