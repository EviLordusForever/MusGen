using System;
using static Library.Array2;

namespace Library
{
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
			float omega = -2f * MathF.PI / input.Length;

			if (input.Length == 1)
			{
				result[0] = input[0];

				if (Complex.IsNaN(result[0]))
					return new[] { new Complex(0, 0) };

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

		public static void FFTv2(int dir, int m, ref float[] x, ref float[] y)
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
	}
}
