using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;

namespace MusGen
{
	public static class PeriodFinder
	{
		public static float[] dft;
		public static int[] leadIndexes;

		public static double FindPeriod(Wav wav, int start, int length, out double minMismatch)
		{
			int minPeriod = 40;    //80
			int maxPeriod = 1200;   //600
			int points = 120;

			//length MORE then maxPeriod + 50;

			float[] mismatches = new float[maxPeriod];

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

			//GraficsMaker.MakeGraficPlus(n.ToString(), mismatches, new int[] { minPeriod, maxPeriod });

			return actualPeriod;
		}

		public static double FP_DFT_ANI(Wav wav, int start, int L, int step, out double minMismatch, double limit, double periodShow, int n)
		{
			double actualPeriod = FP_DFT(wav, start, L, step, out minMismatch);

			//GraficsMaker.MakeGraficPlus(n.ToString(), dft, new int[] { minPeriod, maxPeriod });

			return actualPeriod;
		}

		public static double FP_DFT(Wav wav, int start, int L, int step, out double a)
		{
			if (start + L >= wav.L.Length - 1)
			{
				a = 0.99;
				return 80;
			}

			double frequency = 0;
			float maxv = 0;

			dft = new float[300];

			int t = 0;

			for (double k = 0; t < dft.Length; k += 1 / 6.0)
			{
				float re = 0;
				float im = 0;

				for (int n = start; n < start + L; n += step)
				{
					re += wav.L[n] * (float)Math.Cos(2.0f * Math.PI * k * n / L);
					im += wav.L[n] * (float)Math.Sin(2.0f * Math.PI * k * n / L);
				}

				dft[t] = (float)Math.Sqrt(re * re + im * im);

				if (dft[t] > maxv)
				{
					maxv = dft[t];
					frequency = k;
					//t_leader = t;
				}

				t++;
			}

			a = maxv; ////

			return 1000 / frequency;
		}

		public static void FP_DFT_MULTI(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step, float trashSize, string graficName, float adaptiveCeiling)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			float pi2 = 2 * MathF.PI;
			float leadFrequency = 0;
			float leadAmplitude = 0;
			int leadIndex = 0;

			leadIndexes = new int[periods.Length];

			dft = new float[300];
			float[] dftClone = new float[300];

			int index = 0;

			for (float frequency = 0.12f; index < dft.Length; frequency += 1 / 6f)
			{
				float re = 0;
				float im = 0;

				float fakePoint = 0;
				float fakeStep = step * frequency;

				for (int sample = start; sample < start + L; sample += step)
				{
					re += wav.L[sample] * MathF.Cos(fakePoint);
					im += wav.L[sample] * MathF.Sin(fakePoint);

					fakePoint += fakeStep;
				}

				dft[index] = 0.5f * MathF.Sqrt(re * re + im * im);
				dftClone[index] = dft[index];

				index++;
			}

			FindLeadFrequency();
			float amplitudeMax = leadAmplitude; //why abs?
			float amplitudeOverflow = MathF.Max(leadAmplitude, 1);
			float ceiling = Math.Max(adaptiveCeiling, amplitudeMax / amplitudeOverflow);

			for (int i = 0; i < periods.Count(); i++)
			{
				FindLeadFrequency();

				periods[i] = 1 / leadFrequency;
				amplitudes[i] = leadAmplitude / amplitudeOverflow;
				leadIndexes[i] = leadIndex;

				//GraficsMaker.MakeGraficPlus($"A {i}", dftClone, leadIndexes, amplitudes, amplitudeMax);

				RemoveTrash(leadIndex, trashSize);
			}

			void FindLeadFrequency()
			{
				leadFrequency = 0;
				leadAmplitude = 0;
				leadIndex = 0;
				index = 0;				

				for (float frequency = 0.12f; index < dftClone.Length; frequency += 1 / 6f)
				{
					if (dftClone[index] > leadAmplitude)
					{
						leadAmplitude = dftClone[index];
						leadFrequency = frequency;
						leadIndex = index;
					}

					index++;
				}

				//leadAmplitude = dft[leadIndex];
				//is think this should be disabled
			}

			void RemoveTrash(int point, float size)
			{
				for (int i = 0; i < dftClone.Length; i++)
					dftClone[i] = dftClone[i] * MathF.Abs(MathF.Tanh((i - point)/size));
			}
		}

		public static double goertzel(double[] signal, long N, float freq, int samplerate)
		{
			double skn, skn1, skn2;
			skn = skn1 = skn2 = 0;

			double c = 2 * Math.PI * freq / samplerate;
			double cosan = Math.Cos(c);

			for (int i = 0; i < N; i++)
			{
				skn2 = skn1;
				skn1 = skn;
				skn = 2 * cosan * skn1 - skn2 + signal[i];
			}

			return skn - Math.Exp(-c) * skn1;
		}
	}
}