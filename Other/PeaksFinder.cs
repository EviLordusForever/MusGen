﻿using System;
using Extensions;

namespace MusGen
{
	public static class PeaksFinder
	{
		//peakSize * FFTsize / 1024 !!!
		//array[0] = 0; !!!

		public static int[] Find(float[] array, int count, float peakSize)
		{
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
				return MathE.IndexOfMax(array);
			}

			void RemovePeak(int point)
			{
				for (int i = 0; i < array.Length; i++)
					array[i] = array[i] * MathF.Abs(MathF.Tanh((i - point) / peakSize));
			}
		}
	}
}
