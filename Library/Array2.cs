using System.Text;

namespace Library
{
	public static class Array2
	{
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
