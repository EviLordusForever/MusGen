﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;

namespace MusGen
{
	public static class PeriodFinder
	{
		public static int minPeriod = 40;    //80
		public static int maxPeriod = 1200;   //600
		public static int points = 120;
		public static float[] mismatches;
		public static float[] dft;
		public static int t_leader = 0;

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

		public static float[] DFT()
		{
			int T = 2205;

			float[] sign = new float[T];
			for (int i = 0; i < T; i++)
				sign[i] = (float)(Math.Sin(2.0f * Math.PI * 120.0f * i / 44100.0f) / 2 + Math.Sin(2.0f * Math.PI * 300.0f * i / 44100.0f) + Math.Sin(2.0f * Math.PI * 900.0f * i / 44100.0f));

			float[] re = new float[T];
			float[] im = new float[T];
			float[] dft = new float[T];

			for (int k = 0; k < T; k++)
			{
				for (int n = 0; n < T; n++)
				{
					re[k] += sign[n] * (float)Math.Cos(2.0f * Math.PI * k * n / T);
					im[k] += sign[n] * (float)Math.Sin(2.0f * Math.PI * k * n / T);
				}
				dft[k] = (float)Math.Sqrt(re[k] * re[k] + im[k] * im[k]);
			}

			GraficsMaker.MakeGraficLite(dft, 50);

			return dft;
		}

		public static double FP_DFT_ANI(Wav wav, int start, int L, int step, out double minMismatch, double limit, double periodShow, int n)
		{
			double actualPeriod = FP_DFT(wav, start, L, step, out minMismatch);

			GraficsMaker.MakeGraficPlus(n.ToString(), dft, minPeriod, maxPeriod, Convert.ToInt32(actualPeriod), limit, periodShow);

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
					t_leader = t;
				}

				t++;
			}

			a = maxv; ////

			return 1000 / frequency;
		}

		public static void FP_DFT_MULTI(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			double frequency = 0;
			float maxv = 0;

			dft = new float[300];
			double[] ks = new double[300];

			int t = 0;

			for (double k = 0.12; t < dft.Length; k += 1 / 6.0)
			{
				float re = 0;
				float im = 0;

				for (int s = start; s < start + L; s += step)
				{
					re += wav.L[s] * (float)Math.Cos(2.0f * Math.PI * k * s / L);
					im += wav.L[s] * (float)Math.Sin(2.0f * Math.PI * k * s / L);
				}

				dft[t] = (float)Math.Sqrt(re * re + im * im);

				ks[t] = k;
				if (dft[t] > maxv)
				{
					maxv = dft[t];
					frequency = k;
				}

				t++;
			}

			double maxmaxv = Math.Max(Math.Abs(maxv), 1);
			periods[0] = (float)(1000 / frequency);
			amplitudes[0] = (float)(maxv / maxmaxv);

			int t2 = Math.Min(t, dft.Length - 1);
			int d = 3; //

			for (int i = 1; i < periods.Count(); i++)
			{
				t = t2;
				while (t + d < dft.Length - 1 && dft[t + d] <= dft[t])
				{
					dft[t] = 0;
					t++;
				}
				t = t2;
				while (t - d >= 0 && dft[t - d] <= dft[t])
				{
					dft[t] = 0;
					t--;
				} //important fix

				maxv = 0;
				t = 0;

				for (double k = 0.12; t < dft.Length; k += 1 / 6.0)
				{
					ks[t] = k;

					if (dft[t] > maxv)
					{
						maxv = dft[t];
						frequency = k;
						t2 = t;
					}

					t++;
				}

				periods[i] = (float)(1000 / frequency);
				amplitudes[i] = (float)(maxv / maxmaxv);
			}
		}

		public static void FP_DFT_MULTI_2(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			float frequency = 0;
			float aMax = 0;
			int indexOfMax = 0;

			dft = new float[300];
			float[] dftClone = new float[300];

			int j = 0;

			for (float k = 0.12f; j < dft.Length; k += 1 / 6f)
			{
				float re = 0;
				float im = 0;

				for (int s = start; s < start + L; s += step)
				{
					re += wav.L[s] * (float)Math.Cos(2.0f * Math.PI * k * s / L);
					im += wav.L[s] * (float)Math.Sin(2.0f * Math.PI * k * s / L);
				}

				dft[j] = (float)Math.Sqrt(re * re + im * im);
				dftClone[j] = dft[j];

				if (dft[j] > aMax)
				{
					aMax = dft[j];
					frequency = k;
				}

				j++;
			}

			float aCeiling = MathF.Max(MathF.Abs(aMax), 1);

			periods[0] = 1000f / frequency;
			amplitudes[0] = aMax / aCeiling;

			for (int i = 1; i < periods.Count(); i++)
			{
				RemoveTrash(indexOfMax, 10f); ///////size
				FindFrequency();

				periods[i] = (float)(1000 / frequency);
				amplitudes[i] = (float)(aMax / aCeiling);
			}

			void FindFrequency()
			{
				aMax = 0;
				j = 0;				

				for (float k = 0.12f; j < dftClone.Length; k += 1 / 6f)
				{
					if (dftClone[j] > aMax)
					{
						aMax = dftClone[j];
						frequency = k;
						indexOfMax = j;
					}

					j++;
				}

				aMax = dft[indexOfMax];
			}

			void RemoveTrash(int point, float size)
			{
				for (int i = 0; i < dftClone.Length; i++)
					dftClone[i] = dftClone[i] * MathF.Abs(MathF.Tanh((i - point)/size));
			}
		}

		public static double[] DFT2(double[] inreal, double[] inimag, double[] outreal, double[] outimag)
		{
			int n = inreal.Length;
			for (int k = 0; k < n; k++)
			{  // For each output element
				double sumreal = 0;
				double sumimag = 0;
				for (int t = 0; t < n; t++)
				{  // For each input element
					double angle = 2 * Math.PI * t * k / n;
					sumreal += inreal[t] * Math.Cos(angle) + inimag[t] * Math.Sin(angle);
					sumimag += -inreal[t] * Math.Sin(angle) + inimag[t] * Math.Cos(angle);
				}
				outreal[k] = sumreal;
				outimag[k] = sumimag;
			}
			return outreal;
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