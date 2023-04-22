using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library
{
	public static class Array2
	{
		public static float[] RescaleArrayToLog2(float[] array)
		{
			//changes array to logarithmic scale with intepolation
			//scale of array will remain exactly the same
			int indexA = 0;
			int indexB = 0;
			int L = array.Length;

			float[] newArray = new float[L];

			for (int x = 0; x < L; x++)
			{
				float index = L * MathF.Pow(1f * x / L, 2);
				indexB = (int)MathF.Floor(index);
				if (indexB - indexA < 1)
				{
					float power = index - indexB;
					newArray[x] = array[indexB] * (1 - power) + array[indexB + 1] * power;
				}
				else
				{
					indexA++;

					for (int i = indexA; i <= indexB; i++)
						newArray[x] += array[i];
					newArray[x] /= indexB - indexA + 1;
				}
				indexA = indexB;
			}

			return newArray;
		}

		public static int[] RescaleIndexesToLog2(int[] indexes, int arraySize)
		{
			//changes indexes of linear array
			//to indexes of array rescaled to logarithic scale by base 2
			//all are stay in the range from 0 to maximum
			int L = arraySize;

			for (int x = 0; x < indexes.Length; x++)
				indexes[x] = (int)(Math.Pow((indexes[x] + 0.0) / L, 0.5) * L);

			return indexes;
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
