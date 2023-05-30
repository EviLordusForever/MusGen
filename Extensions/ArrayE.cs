using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
	public static class ArrayE
	{
		public static float[] CreateInterpolatedArray(int length, int[] points, float[] values)
		{
			float[] array = new float[length];

			int j = 0;
			int k = 1;

			for (int i = 0; i < length - 1; i++)
			{
				if (i >= points[k])
				{
					j = k;
					k++;

					if (k >= points.Length)
						k = points.Length - 1;
				}

				float t = (float)(i - points[j]) / (points[k] - points[j]);
				array[i] = values[j] * (1 - t) + values[k] * t;
			}

			array[length - 1] = values[values.Length - 1];

			return array;
		}

		public static float[] Add(float[] array, float[]array2)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] += array2[i];

			return array;
		}

		public static float[] Substract(float[] array, float[] array2)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] -= array2[i];

			return array;
		}

		public static float[] Divide(float[] array, float a)
		{
			float[] newArray = new float[array.Length];
			for (int i = 0; i < array.Length; i++)
				newArray[i] = array[i] / a;

			return newArray;
		}

		public static float[] StupiedStretch(float[] array, int factor)
		{
			float[] newArray = new float[array.Length * factor];
			for (int i = 0; i < array.Length; i++)
				newArray[i * factor] = array[i];
			return newArray;
		}

		public static float[] NormalStretch(float[] array, int new_size)
		{
			int factor = (int)(new_size / array.Length);

			float[] newArray = new float[array.Length * factor];
			for (int i = 0; i < newArray.Length; i++)
				newArray[i] = array[i / factor];
			return newArray;
		}

		public static float[] RescaleArrayToLog(float[] array, float base_, int new_size, bool byMax)
		{
			//return NormalStretch(array, new_size);

			int indexA = 0;
			int indexB = 0;
			int size = array.Count();

			float[] newArray = new float[(int)new_size];

			for (int x = 0; x < new_size; x++)
			{
				float index = MathF.Pow(base_, 1f * x / new_size - 1) * size;

				indexB = (int)MathF.Floor(index);
				if (indexB + 1 >= size)
				{
					newArray[x] = array[indexB];
				}
				else if (indexB - indexA < 1) //improve?
				{
					float power = index - indexB;
					newArray[x] = array[indexB] * (1 - power) + array[indexB + 1] * power;
				}
				else
				{
					if (byMax)
					{
						indexA++;

						for (int i = indexA; i <= indexB; i++)
							newArray[x] = MathF.Max(newArray[x], array[i]);
					}
					else
					{
						for (int i = indexA + 1; i <= indexB; i++)
							newArray[x] += array[i];

						newArray[x] /= (indexB - indexA);

						indexA++;
					}
				}
				indexA = indexB;			
			}

			return newArray;
		}

		public static float[] SmoothArray(float[] array, int n)
		{
			float summ = 0;

			for (int s = 0; s < array.Length + n; s++)
			{
				if (s < array.Length)
					summ += array[s];
				else
					summ += array[array.Length - 1];

				if (s >= n)
				{
					array[s - n] = summ / n;
					summ -= array[s - n];
				}
			}

			return array;
		}

		public static float[] MixArrays(float[] ar1, float[] ar2, float[] mask1, float[] mask2)
		{
			for (int i = 0; i < ar1.Length; i++)
				ar1[i] = ar1[i] * mask1[i] + ar2[i] * mask2[i];

			return ar1;
		}

		public static float[] SmoothArrayCopy(float[] array, int n)
		{
			float[] arrayCopy = new float[array.Length];
			float summ = 0;
			int li = 0;
			int ri = 0;
			int nhalf = n / 2;

			float oldli = 0;
			float oldri = 0;

			for (int s = -nhalf; s < array.Length; s++)
			{
				oldli = li;
				oldri = ri;
				ri = Math.Min(s + nhalf, array.Length - 1);
				li = Math.Max(s - nhalf, 0);

				if (oldri != ri)
					summ += array[ri];
				if (oldli != li)
					summ -= array[li];

				if (s >= 0)
					arrayCopy[s] = summ / (ri - li + 1);
			}

			return arrayCopy;
		}

		public static float[] RescaleArrayFromLog(float[] array, float base_, int new_size)
		{
			int indexA = 0;
			int indexB = 0;
			int size = array.Count();

			float[] newArray = new float[(int)new_size];

			for (int x = 1; x < new_size; x++)
			{
				float index = (MathF.Log(1f * x / size, base_) + 1) * new_size;

				indexB = (int)MathF.Floor(index);
				if (indexB + 1 >= size)
				{
					newArray[x] = array[indexB];
				}
				else if (indexB - indexA < 1) //improve?
				{
					float power = index - indexB;
					newArray[x] = array[indexB] * (1 - power) + array[indexB + 1] * power;
				}
				else
				{
					indexA++;

					for (int i = indexA; i <= indexB; i++)
						newArray[x] = MathF.Max(newArray[x], array[i]);
				}
				indexA = indexB;
			}

			return newArray;
		}

		public static int[] OneToN(int N)
		{
			int[] array = new int[N];
			for (int i = 0; i < N; i++)
				array[i] = i;
			return array;
		}

		public static float[] OneToNFloat(int N)
		{
			float[] array = new float[N];
			for (int i = 0; i < N; i++)
				array[i] = i;
			return array;
		}

		public static int[] RescaleIndexesToLog(int[] indexes, int arraySize)
		{
			float L = arraySize;

			for (int x = 0; x < indexes.Length; x++)
				indexes[x] = (int)(MathE.ToLogScale(indexes[x] / L, L) * L);

			return indexes;
		}

		public static T[] InterpolateArray<T>(T[] source, int targetLength) where T : struct, IConvertible
		{
			if (!typeof(T).IsPrimitive)
			{
				throw new ArgumentException("The type parameter T must be a primitive type.", nameof(T));
			}

			T[] result = new T[targetLength];
			double step = (double)(source.Length - 1) / (targetLength - 1);

			for (int i = 0; i < targetLength; i++)
			{
				double index = i * step;
				int roundedIndex = (int)Math.Floor(index);
				double fraction = index - roundedIndex;

				if (roundedIndex >= source.Length - 1)
				{
					result[i] = source[source.Length - 1];
				}
				else
				{
					dynamic value1 = source[roundedIndex];
					dynamic value2 = source[roundedIndex + 1];
					result[i] = (T)((value1 * (1.0 - fraction)) + (value2 * fraction));
				}
			}

			return result;
		}

		public static T[] SubArray<T>(this T[] array, int offset, int length)
		{
			return array.Skip(offset).Take(length).ToArray();
		}

		public static T[] Convert2DArrayTo1D<T>(T[][] array2D)
		{
			List<T> lst = new List<T>();
			foreach (T[] a in array2D)
				lst.AddRange(a);

			return lst.ToArray();
		}

		public static T[] Convert2DArrayTo1D<T>(T[,] array2D)
		{
			return array2D.Cast<T>().ToArray();
		}

		public static T[] Concatenate<T>(T[] first, T[] second)
		{
			if (first == null)
				return second;
			if (second == null)
				return first;

			return first.Concat(second).ToArray();
		}

		public static string ToString<T>(T[,] twoDArray)
		{
			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < twoDArray.GetLength(0); i++)
			{
				for (int j = 0; j < twoDArray.GetLength(1); j++)
				{
					stringBuilder.Append($"{twoDArray[i, j].ToString()}\t");
				}
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}
	}
}
