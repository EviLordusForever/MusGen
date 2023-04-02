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
		public static int size = 300;
		public static float[] dft;
		public static float[] frequencies;
		public static int[] leadIndexes;

		public static void FP_DFT_MULTI(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step, float trashSize, string graficName, float adaptiveCeiling)
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

			for (float frequency = 20f; index < dft.Length; frequency *= 1.05f)
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