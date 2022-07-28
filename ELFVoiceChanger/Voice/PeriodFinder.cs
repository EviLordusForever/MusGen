using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;

namespace ELFVoiceChanger.Voice
{
	public static class PeriodFinder
	{
		public static int minPeriod = 40;    //80
		public static int maxPeriod = 1200;   //600
		public static int points = 120;
		public static float[] mismatches;

		public static double FindPeriod(Wav wav, int start, int length, out double minMismatch)
		{
			//length MORE then maxPeriod + 50;

			mismatches = new float[maxPeriod];

			if (start + length >= wav.L.Length - maxPeriod - points)
			{
				minMismatch = 0.99;
				return 100;
			}

			double actualPeriod = 1;
			minMismatch = 1;

			int delta = (length - maxPeriod) / points;

			for (double period = minPeriod; period < maxPeriod; period *= 1.015)
			{
				double mismatch = 0;

				for (int sample = start; sample < start + length; sample += delta)
				{
					mismatch += Math.Abs(wav.L[sample] - wav.L[sample + (int)period]);
					if (wav.channels == 2) //Is this necessary?
						mismatch += Math.Abs(wav.R[sample] - wav.R[sample + (int)period]);
				}
				mismatch /= points;

				mismatches[(int)period] = (float)mismatch;

				if (mismatch < minMismatch)
				{
					minMismatch = mismatch;
					actualPeriod = period;
				}
			}

			while (true)
			{				
				int half = (int)(1.05 * actualPeriod / 2);
				int half2 = (int)(0.95 * actualPeriod / 2);

				while (half > half2)
				{
					half--;
					if (half < minPeriod)
						break;
					if (mismatches[half] > 0 && mismatches[half] < mismatches[(int)actualPeriod] * 1.5)
					{
						actualPeriod = half;
						continue;
					}
				}

				int third = (int)(1.05 * actualPeriod / 3);
				int third2 = (int)(0.95 * actualPeriod / 3);

				while (third > third2)
				{
					third--;
					if (third < minPeriod)
						break;
					if (mismatches[third] > 0 && mismatches[third] < mismatches[(int)actualPeriod] * 1.5)
					{
						actualPeriod = third;
						continue;
					}
				}

				int fourth = (int)(1.05 * actualPeriod / 4);
				int fourth2 = (int)(0.95 * actualPeriod / 4);

				while (fourth > fourth2)
				{
					fourth--;
					if (fourth < minPeriod)
						break;
					if (mismatches[fourth] > 0 && mismatches[fourth] < mismatches[(int)actualPeriod] * 1.5)
					{
						actualPeriod = fourth;
						continue;
					}
				}

				break;
			} //important fix

			return actualPeriod;
		}

		public static double FindPeriod_WithAnimation(Wav wav, int start, int length, out double minMismatch, double limit, double periodShow, int n)
		{
			//length MORE then maxPeriod + 50;

			double actualPeriod = FindPeriod(wav, start, length, out minMismatch);

			GraficsMaker.MakeGraficPlus(n.ToString(), mismatches, minPeriod, maxPeriod, Convert.ToInt32(actualPeriod), limit, periodShow);

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