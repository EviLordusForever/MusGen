using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;
using System;
using System.Collections;
using System.Data;
using Library;

namespace MusGen
{
	public static class PeriodFinder
	{
		public static int size = 300;
		public static float[] dft;
		public static float[] frequencies;
		public static int[] leadIndexes;
		public static float amplitudeOverflow;

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

		public static void FFT_MULTI(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, float trashSize, ref float adaptiveCeiling)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			float pi2 = 2 * MathF.PI;

			float leadFrequency = 0;
			float leadAmplitude = 0;
			int leadIndex = 0;

			leadIndexes = new int[periods.Length];

			dft = new float[L / 2];
			float[] dftClone = new float[L / 2];

			int index = 0;

			Complex[] complex = new Complex[L];
			for (int i = 0; i < L; i++)
				complex[i] = new Complex(wav.L[start + i], 0);

			complex = FFT.Forward(complex);

			for (int i = 0; i < L / 2; i++)
			{
				dft[i] = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
				dftClone[i] = dft[i];
			}

			FindLeader();
			float amplitudeMax = leadAmplitude;
			amplitudeOverflow = MathF.Max(leadAmplitude, 1);
			adaptiveCeiling = Math.Max(adaptiveCeiling, amplitudeMax);
			
			periods[0] = 1 / leadFrequency;
			amplitudes[0] = leadAmplitude / amplitudeOverflow;
			leadIndexes[0] = leadIndex;

			for (int i = 1; i < periods.Count(); i++)
			{
				RemoveTrash(leadIndex, trashSize);

				FindLeader();

				periods[i] = 1 / leadFrequency;
				amplitudes[i] = leadAmplitude / amplitudeOverflow;
				leadIndexes[i] = leadIndex;
			}

			void FindLeader()
			{
				leadIndex = 0;
				leadFrequency = 0;
				leadAmplitude = 0;

				for (int i = 0; i < dftClone.Length; i++)
					if (dftClone[i] > leadAmplitude)
					{
						leadAmplitude = dftClone[i];
						leadIndex = i;
					}

				leadFrequency = (1f * leadIndex / L) * wav.sampleRate;
			}

			void RemoveTrash(int point, float size)
			{
				for (int i = 0; i < dftClone.Length; i++)
					dftClone[i] = dftClone[i] * MathF.Abs(MathF.Tanh((i - point) / size));
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

		public static float[] DFT_EX_2(float[] fft, Wav wav, int start, int L)
		{
			Complex[] c = new Complex[L];

			for (int i = 0; i < L; i++)
				c[i] = new Complex(wav.L[i + start], 0);

			c = FFT.Forward(c);

			for (int i = 0; i < L; i++)
				fft[i] = (float)(Math.Sqrt(Math.Pow(c[i].Real, 2) + Math.Pow(c[i].Imaginary, 2)));

			return fft;
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

		public static class FFT
		{
			public static int Size { get; set; }

			static FFT()
			{
				Size = 0;
			}

			public static Complex[] Inverse(Complex[] input)
			{
				for (int i = 0; i < input.Length; i++)
				{
					input[i] = Complex.Conjugate(input[i]);
				}

				var transform = Forward(input, false);

				for (int i = 0; i < input.Length; i++)
				{
					transform[i] = Complex.Conjugate(transform[i]);
				}
				return transform;
			}

			public static Complex[] Forward(Complex[] input, bool phaseShift = true)
			{
				var result = new Complex[input.Length];
				var omega = (float)(-2.0 * Math.PI / input.Length);

				if (input.Length == 1)
				{
					result[0] = input[0];

					if (Complex.IsNaN(result[0]))
					{
						return new[] { new Complex(0, 0) };
					}
					return result;
				}

				var evenInput = new Complex[input.Length / 2];
				var oddInput = new Complex[input.Length / 2];

				for (var i = 0; i < input.Length / 2; i++)
				{
					evenInput[i] = input[2 * i];
					oddInput[i] = input[2 * i + 1];
				}

				var even = Forward(evenInput, phaseShift);
				var odd = Forward(oddInput, phaseShift);

				for (var k = 0; k < input.Length / 2; k++)
				{
					int phase;

					if (phaseShift)
						phase = k - Size / 2;
					else
						phase = k;

					odd[k] *= Complex.Polar(1, omega * phase);
				}

				for (var k = 0; k < input.Length / 2; k++)
				{
					result[k] = even[k] + odd[k];
					result[k + input.Length / 2] = even[k] - odd[k];
				}

				return result;
			}
		}

		public class Complex
		{
			public float Real { get; set; }
			public float Imaginary { get; set; }

			public Complex(float real, float imaginary)
			{
				Real = real;
				Imaginary = imaginary;
			}

			public static float Modulus(Complex number)
			{
				return (float)Math.Sqrt(number.Real * number.Real + number.Imaginary * number.Imaginary);
			}

			public static Complex Polar(float r, float angle)
			{
				return new Complex((float)(r * Math.Cos(angle)), (float)(r * Math.Sin(angle)));
			}

			public static Complex operator +(Complex a, Complex b)
			{
				return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
			}

			public static Complex operator +(float a, Complex b)
			{
				return new Complex(b.Real + a, b.Imaginary);
			}

			public static Complex operator +(Complex a, float b)
			{
				return new Complex(a.Real + b, a.Imaginary);
			}

			public static Complex operator -(Complex a, Complex b)
			{
				return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
			}

			public static Complex operator *(float a, Complex b)
			{
				return new Complex(b.Real * a, b.Imaginary * a);
			}

			public static Complex operator *(Complex a, float b)
			{
				return new Complex(a.Real * b, a.Imaginary * b);
			}

			public static Complex operator /(Complex b, float a)
			{
				return new Complex(b.Real / a, b.Imaginary / a);
			}

			public static Complex operator *(Complex a, Complex b)
			{
				return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Real * b.Imaginary + a.Imaginary * b.Real);
			}

			public static Complex Conjugate(Complex a)
			{
				return new Complex(a.Real, -a.Imaginary);
			}

			public static Complex operator /(float a, Complex b)
			{
				return new Complex(a / b.Real, a / b.Imaginary);
			}

			public static bool IsNaN(Complex a)
			{
				return float.IsNaN(a.Imaginary) || float.IsNaN(a.Real);
			}
		}
	}
}