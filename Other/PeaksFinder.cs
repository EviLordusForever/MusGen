using System;
using Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MusGen
{
	public static class PeaksFinder
	{
		public static int[] Find(float[] array, int count, float peakSize)
		{
			float[] array2 = (float[])array.Clone();

		    int[] peakIndexes = new int[count];
			peakIndexes[0] = FindPeak();
			for (int i = 1; i < count; i++)
			{
				RemovePeak(peakIndexes[i - 1]);
				peakIndexes[i] = FindPeak();
			}
			return peakIndexes;

			int FindPeak()
			{
				return MathE.IndexOfMax(array2);
			}

			void RemovePeak(int point)
			{
				for (int i = 0; i < array2.Length; i++)
					array2[i] = array2[i] * MathF.Abs(MathF.Tanh((i - point) / peakSize));
			}
		}

		public static List<int> FindEvery(float[] array, float peakSize, float percentage, int limit)
		{
			float[] array2 = MathE.StupiedFilter(array);
			float average = array2.Average();
			float minimum = average * percentage;

			float value = 0;
			int index = 0;

			List<int> peakIndexes = new List<int>();
			FindPeak();
			peakIndexes.Add(index);

			for (int i = 1; i < limit && value > minimum; i++)
			{
				RemovePeak(peakIndexes[i - 1]);
				FindPeak();
				peakIndexes.Add(index);
			}
			return peakIndexes;

			void FindPeak()
			{
				index = MathE.IndexOfMax(array2);
				value = array[index];
			}

			void RemovePeak(int point)
			{
				float localPeakSize = peakSize * (value / average);
				for (int i = 0; i < array2.Length; i++)
					array2[i] = array2[i] * MathF.Abs(MathF.Tanh((i - point) / localPeakSize));
			}
		}
	}
}
