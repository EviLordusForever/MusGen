using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;
using System;
using System.Collections;
using System.Data;

namespace MusGen
{
	public static class PeriodFinder
	{
		public static int size = 300;
		public static float[] dft;
		public static float[] frequencies;
		public static int[] leadIndexes;

		public static void DFT_MULTI(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step, float trashSize, string graficName, ref float adaptiveCeiling)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			float pi2 = 2 * MathF.PI;
			float leadFrequency = 0;
			float leadAmplitude = 0;
			int leadIndex = 0;

			leadIndexes = new int[periods.Length];

			dft = new float[size];
			frequencies = new float[size];
			float[] dftClone = new float[size];

			int index = 0;

			for (float frequency = 20f; index < dft.Length; frequency *= 1.023f)
			{
				float re = 0;
				float im = 0;

				float fakePoint = 0;
				float fakeStep = pi2 * step * frequency / wav.sampleRate;

				for (int sample = start; sample < start + L; sample += step)
				{
					re += wav.L[sample] * MathF.Cos(fakePoint);
					im += wav.L[sample] * MathF.Sin(fakePoint);

					fakePoint += fakeStep;
				}

				dft[index] = 0.5f * MathF.Sqrt(re * re + im * im);
				dftClone[index] = dft[index];

				frequencies[index] = frequency;
				index++;
			}

			FindLeadFrequency();
			float amplitudeMax = leadAmplitude; //why abs?
			float amplitudeOverflow = MathF.Max(leadAmplitude, 1);
			adaptiveCeiling = Math.Max(adaptiveCeiling, amplitudeMax / amplitudeOverflow);

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

				for (; index < dftClone.Length; index++)
				{
					if (dftClone[index] > leadAmplitude)
					{
						leadAmplitude = dftClone[index];
						leadFrequency = frequencies[index];
						leadIndex = index;
					}
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

		private static double[] DftTest(double[] input)
		{
			int length = input.Length;
			int maxFrequency = length;
			double[] real = new double[length];
			double[] imag = new double[length];
			double[] dft = new double[maxFrequency];
			double pi_div = 2.0 * Math.PI / length;
			for (int frequancy = 0; frequancy < maxFrequency; frequancy++)
			{
				double a = pi_div * frequancy;

				for (int t = 0; t < length; t++)
				{
					real[frequancy] += input[t] * Math.Cos(a * t);
					imag[frequancy] += input[t] * Math.Sin(a * t);
				}
				dft[frequancy] = Math.Sqrt(real[frequancy] * real[frequancy] + imag[frequancy] * imag[frequancy]) / length;
			}
			return dft;
		}

		public static float[] DFT_EX(ref float[] fft, Wav wav, int start, int L, int power)
		{
			fft = new float[L];
			float[] fft2 = new float[L];
			for (int i = 0; i < L; i++)
			{
				fft[i] = wav.L[i + start];
				fft2[i] = fft[i];
			}

			FFT1D(1, power, ref fft, ref fft2);
			float[] fft0 = new float[L / 2];

			for (int i = 0; i < L / 2; i++)
				fft0[i] = fft[i] + fft[L - i - 1];

			return fft0;
		}

		public static void FFT1D(int dir, int m, ref float[] x, ref float[] y)
		{
			long nn, i, i1, j, k, i2, l, l1, l2;
			float c1, c2, tx, ty, t1, t2, u1, u2, z;
			/* Calculate the number of points */
			nn = 1;
			for (i = 0; i < m; i++)
				nn *= 2;
			/* Do the bit reversal */
			i2 = nn >> 1;
			j = 0;
			for (i = 0; i < nn - 1; i++)
			{
				if (i < j)
				{
					tx = x[i];
					ty = y[i];
					x[i] = x[j];
					y[i] = y[j];
					x[j] = tx;
					y[j] = ty;
				}
				k = i2;
				while (k <= j)
				{
					j -= k;
					k >>= 1;
				}
				j += k;
			}
			/* Compute the FFT */
			c1 = -1f;
			c2 = 0f;
			l2 = 1;
			for (l = 0; l < m; l++)
			{
				l1 = l2;
				l2 <<= 1;
				u1 = 1f;
				u2 = 0f;
				for (j = 0; j < l1; j++)
				{
					for (i = j; i < nn; i += l2)
					{
						i1 = i + l1;
						t1 = u1 * x[i1] - u2 * y[i1];
						t2 = u1 * y[i1] + u2 * x[i1];
						x[i1] = x[i] - t1;
						y[i1] = y[i] - t2;
						x[i] += t1;
						y[i] += t2;
					}
					z = u1 * c1 - u2 * c2;
					u2 = u1 * c2 + u2 * c1;
					u1 = z;
				}
				c2 = MathF.Sqrt((1f - c1) / 2f);
				if (dir == 1)
					c2 = -c2;
				c1 = MathF.Sqrt((1f + c1) / 2f);
			}
			/* Scaling for forward transform */
			if (dir == 1)
			{
				for (i = 0; i < nn; i++)
				{
					x[i] /= (float)nn;
					y[i] /= (float)nn;
				}
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