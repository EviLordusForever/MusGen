using System.Text;

namespace Library
{
	public static class Math2
	{
		public static Random rnd = new Random();

		public static byte[] StringToByteArray(string hex)
		{
			return Encoding.ASCII.GetBytes(hex);
		}

		public static double BytesToDouble(byte firstByte, byte secondByte)
		{
			// convert two bytes to one double in the range -1 to 1

			int s = secondByte << 8 | firstByte;
			return s / 32768.0;
		}

		public static byte[] FloatToBytes(float d)
		{
			return BitConverter.GetBytes(d);
		}

		public static int Factorial(int number)
		{
			int n = 1;

			for (int i = 1; i <= number; i++)
				n *= i;

			return n;
		}

		public static int Combinations1(int n, int k)
		{
			if (n < k || n < 1 || k < 1)
				return 0;
			if (n == k)
				return 1;
			if (k == 1)
				return n;
			return Combinations1(n - 1, k - 1) + Combinations1(n - 1, k);
		}

		public static int Combinations2(int n, int k)
		{
			if (n > 13)
				throw new ArgumentException();
			return Factorial(n) / (Factorial(k) * Factorial(n - k));
		}

		public static double Combinations3(int n, int k)
		{
			double ret = 1;
			while (k > 0)
			{
				ret = ret * n / k;
				k--; 
				n--;
			}
			return ret;
		}

		public static double Interpolate(double from, double to, double actual)
		{
			return (actual - from) / (to - from);
		}

		public static float Max(float[] array)
		{
			float max = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] > max)
					max = array[i];
			return max;
		}

		public static int IndexOfMax(float[] array)
		{
			float max = array[0];
			int id = 0;
			for (int i = 1; i < array.Length; i++)
				if (array[i] > max)
				{
					id = i;
				}
			return id;
		}

		public static int Max(int[] array)
		{
			int max = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] > max)
					max = array[i];
			return max;
		}

		public static float Min(float[] array)
		{
			float min = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] < min)
					min = array[i];
			return min;
		}

		public static void FindMinAndMaxForLastNPoints(List<float> array, ref float currentMin, ref float currentMax, int n)
		{
			int count = array.Count;
			int lastIndex = count - 1;
			int firstIndex = count - n;

			if (count == 0)
				return;
			else if (count > n)
			{
				if (array[firstIndex - 1] == currentMin || array[firstIndex - 1] == currentMax)
					RefindMaxMin(ref currentMin, ref currentMax);
				else
				{
					if (array[lastIndex] > currentMax)
						currentMax = array[lastIndex];

					if (array[lastIndex] < currentMin)
						currentMin = array[lastIndex];
				}
			}
			else if (count == n)
				RefindMaxMin(ref currentMin, ref currentMax);
			else
				FindMinAndMaxForLastNPoints(array, ref currentMin, ref currentMax, count);

			void RefindMaxMin(ref float currentMin, ref float currentMax)
			{
				var subArray = array.GetRange(firstIndex, n).ToArray();
				currentMax = Max(subArray);
				currentMin = Min(subArray);
			}
		}

		public static float FindStandartDeviation(float[] input)
		{
			float standartDeviation = 0;
			for (int i = 0; i < input.Length; i++)
				standartDeviation += MathF.Pow(input[i], 2);
			standartDeviation /= input.Length;
			standartDeviation = MathF.Sqrt(standartDeviation);
			return standartDeviation;
		}

		public static double CalculateRandomness(double k, int n)
		{
			if (k == n / 2.0)
				return 1;
			else
			{
				double randomness;

				if (k <= n / 2.0)
					randomness = CumulativeDistributionFunction(k, n, 0.5);
				else
					randomness = CumulativeDistributionFunction(n - k, n, 0.5);

				return randomness * 2;
			}
		}

		public static double CalculateRandomness(double k, int n, double p)
		{
			if (k == n * p)
				return 1;
			else
			{
				double randomness;

				if (k < n * p)
					randomness = CumulativeDistributionFunction(k, n, p);
				else
					randomness = CumulativeDistributionFunction(n - k, n, 1 - p);

				return randomness * 2;
			}
		}

		public static double CumulativeDistributionFunction(double k, int n, double p)
		{
			var d = new MathNet.Numerics.Distributions.Binomial(p, n);
			return d.CumulativeDistribution(k);
		}

		public static bool CheckIfIsNumberic(string value) => int.TryParse(value, out int _);

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