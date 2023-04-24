using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library
{
	public static class Array2
	{
		public static float[] RescaleArrayToLog(float[] array, float base_, int new_size)
		{
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
					indexA++;

					for (int i = indexA; i <= indexB; i++)
						newArray[x] += array[i];
					newArray[x] /= indexB - indexA + 1;
				}
				indexA = indexB;			
			}

			return newArray;
		}

		public static int[] RescaleIndexesToLog(int[] indexes, int arraySize)
		{
			float L = arraySize;

			for (int x = 0; x < indexes.Length; x++)
				indexes[x] = (int)(Math2.ToLogScale(indexes[x] / L, L) * L);

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
