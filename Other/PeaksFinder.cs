using System;
using Extensions;

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
	}
}
