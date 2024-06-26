using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Extensions
{
	public static class MathE
	{
		public static Random rnd = new Random();

		public static float[] StupiedFilter(float[] array, bool soft)
		{
			float[] res = new float[array.Length];
			for (int i = 1; i < array.Length - 2; i++)
				if (!soft)
				{
					if (array[i] > array[i - 1] && array[i] > array[i + 1])
						res[i] = array[i];
				}
				else
				{
					if (array[i] >= array[i - 1] && array[i] >= array[i + 1])
						res[i] = array[i];
				}
			return res;
		}

		public static List<ushort> StupiedFilterMask(float[] array, bool soft)
		{
			List<ushort> mask = new List<ushort>();
			for (ushort i = 1; i < array.Length - 2; i++)
				if (!soft)
				{
					if (array[i] > array[i - 1] && array[i] > array[i + 1])
						mask.Add(i);
				}
				else
				{
					if (array[i] >= array[i - 1] && array[i] > array[i + 1]
						|| array[i] > array[i - 1] && array[i] >= array[i + 1])
						mask.Add(i);
				}
			return mask;
		}

		public static double GeometricMean<T>(this T[] numbers) where T : struct, IConvertible
		{
			double product = 1.0;
			int count = numbers.Length;

			for (int i = 0; i < count; i++)
			{
				double value = Convert.ToDouble(numbers[i]);
				product *= value;
			}

			double geometricMean = Math.Pow(product, 1.0 / count);
			return geometricMean;
		}

		public static T[] GetValues<T>(T[] array, int[] indexes)
		{
			T[] values = new T[indexes.Length];
			for (int i = 0; i < indexes.Length; i++)
				values[i] = array[indexes[i]];
			return values;
		}

		public static T[] GetValues<T>(T[] array, ushort[] indexes)
		{
			T[] values = new T[indexes.Length];
			for (int i = 0; i < indexes.Length; i++)
				values[i] = array[indexes[i]];
			return values;
		}

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

		public static float FadeOut(float Zt1)
		{
			return (MathF.Cos(Zt1 * MathF.PI) + 1) / 2;
		}

		public static float FadeOutCubic(float Zt1)
		{
			//Fall faster
			return MathF.Pow(FadeOut(Zt1), 3);
		}

		public static float FadeOutCentered(float count, int n)
		{
			//Fall faster but symmetric
			float cos = MathF.Cos(count * MathF.PI);
			return (MathF.Pow(MathF.Abs(cos), 1f/n) * Math.Sign(cos) + 1) / 2;
		}

		public static float LogPro(float x, float base_)
		{
			return MathF.Pow(base_, x / base_);
		}

		public static float LogProReverse(float x, float base_)
		{
			return base_ * MathF.Log(x, base_);
		}

		public static float ToLogScale(float x, float base_)
		{
			return MathF.Log(x * (base_ - 1) + 1, base_);
		}

		public static float Gauss(float x, float center, float width, float heigh)
		{
			return heigh * width / (MathF.Pow(x - center, 2) + width);
		}

		public static float ToLogScaleReverse(float y, float base_)
		{
			return (MathF.Pow(base_, y) - 1) / (base_ - 1);
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

		public static T Interpolate<T>(T from, T to, T actual) where T : struct, IComparable, IConvertible
		{
			if (!typeof(T).IsPrimitive)
			{
				throw new ArgumentException("T must be a primitive type.");
			}

			double fromDouble = Convert.ToDouble(from);
			double toDouble = Convert.ToDouble(to);
			double actualDouble = Convert.ToDouble(actual);

			double resultDouble = (actualDouble - fromDouble) / (toDouble - fromDouble);

			return (T)Convert.ChangeType(resultDouble, typeof(T));
		}

		public static T GetAngle<T>(T x1, T y1, T x2, T y2) where T : struct, IComparable, IConvertible
		{
			if (!typeof(T).IsPrimitive)
			{
				throw new ArgumentException("T must be a primitive type.");
			}

			double x1Double = Convert.ToDouble(x1);
			double y1Double = Convert.ToDouble(y1);
			double x2Double = Convert.ToDouble(x2);
			double y2Double = Convert.ToDouble(y2);

			double angle = Math.Atan2(y2Double - y1Double, x2Double - x1Double);
			angle += Math.PI;
			angle /= 2 * Math.PI;

			return (T)Convert.ChangeType(angle, typeof(T));
		}

		public static T Max<T>(T[] array) where T : IComparable<T>
		{
			T max = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i].CompareTo(max) > 0)
					max = array[i];

			return max;
		}

		public static T Min<T>(T[] array) where T : IComparable<T>
		{
			T min = array[0];

			for (int i = 1; i < array.Length; i++)
				if (array[i].CompareTo(min) < 0)
					min = array[i];

			return min;
		}

		public static ushort IndexOfMax_short<T>(T[] array) where T : IComparable<T>
		{
			T max = array[0];
			ushort index = 0;
			for (ushort i = 1; i < array.Length; i++)
			{
				if (array[i].CompareTo(max) > 0)
				{
					max = array[i];
					index = i;
				}
			}
			return index;
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

		public static float StandartDeviation(float[] input)
		{
			float standartDeviation = 0;
			for (int i = 0; i < input.Length; i++)
				standartDeviation += MathF.Pow(input[i], 2);
			standartDeviation /= input.Length;
			standartDeviation = MathF.Sqrt(standartDeviation);
			return standartDeviation;
		}

		public static double Randomness(double k, int n)
		{
			if (k == n / 2.0)
				return 1;
			else
			{
				double randomness;

				if (k <= n / 2.0)
					randomness = CumulativeDistribution(k, n, 0.5);
				else
					randomness = CumulativeDistribution(n - k, n, 0.5);

				return randomness * 2;
			}
		}

		public static double Randomness(double k, int n, double p)
		{
			if (k == n * p)
				return 1;
			else
			{
				double randomness;

				if (k < n * p)
					randomness = CumulativeDistribution(k, n, p);
				else
					randomness = CumulativeDistribution(n - k, n, 1 - p);

				return randomness * 2;
			}
		}

		public static double CumulativeDistribution(double k, int n, double p)
		{
			var d = new MathNet.Numerics.Distributions.Binomial(p, n);
			return d.CumulativeDistribution(k);
		}

		public static bool IsNumeric(string value) => int.TryParse(value, out int _);
	}
}