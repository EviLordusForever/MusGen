using System;
using Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MusGen
{
	public static class PeaksFinder
	{
		public static float _average;
		public static float _max;
		public static float _minFromAverage;
		public static float _minFromMaximum;

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

		public static List<int> FindEvery(float[] array1)
		{
			List<int> peakIndexes = new List<int>();

			List<int> mask = MathE.StupiedFilterMask(array1, true);

			float[] array2 = new float[array1.Length];
			for (int i = 0; i < mask.Count; i++)
				array2[mask[i]] = array1[mask[i]];

			if (mask.Count == 0)
			{
				peakIndexes.Add(array1.Length / 2);
				return peakIndexes;
			}

			float max1 = 0;
			float max2 = 0;
			int maskIndex = 0;

			_average = Average();
			FindPeak();
			_max = max1;

			_minFromAverage = _average * AP._lowestPeak_FromAverage;
			_minFromMaximum = _max * AP._lowestPeak_FromMaximum;

			float minimum = Math.Min(_minFromMaximum, _minFromAverage);

			for (int i = 0; i < AP._peaksLimit && max2 > minimum && mask.Count > 0; i++)
			{
				FindPeak();
				peakIndexes.Add(mask[maskIndex]);
				RemovePeak();
			}

			if (peakIndexes.Count == 0)
				peakIndexes.Add(array1.Length / 2);

			return peakIndexes;

			void RemovePeak()
			{
				float localPeakSize = AP._peakWidth_ForMultiNad * (max1 / minimum); //
				int point = mask[maskIndex];
				mask.RemoveAt(maskIndex);

				for (int i = 0; i < mask.Count; i++)
					array2[mask[i]] *= MathF.Abs(MathF.Tanh((mask[i] - point) / localPeakSize));
			}

			void FindPeak()
			{
				max2 = 0;
				maskIndex = 0;
				for (int i = 0; i < mask.Count; i++)
					if (array2[mask[i]] > max2)
					{
						max1 = array1[mask[i]];
						max2 = array2[mask[i]];
						maskIndex = i;
					}
			}

			float Average()
			{
				return array1.Average();
			}
		}
	}
}
