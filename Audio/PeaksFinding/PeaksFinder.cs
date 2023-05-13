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

		public static List<int> FindEvery(float[] array1, out List<float> amps)
		{
			List<int> peakIndexes = new List<int>();
			amps = new List<float>();

			float[] array2 = new float[array1.Length];
			for (int i = 0; i < array1.Length; i++)
				array2[i] = array1[i];

			float amp = 0;
			int index = 0;

			_max = array1.Max();
			_average = array1.Average();

			_minFromMaximum = _max * AP._lowestPeak_FromMaximum;
			_minFromAverage = _average * AP._lowestPeak_FromAverage;

			float minimum = Math.Min(_minFromMaximum, _minFromAverage);

			FindPeak();

			for (int i = 0; i < AP._peaksLimit && amp > minimum; i++)
			{
				FindPeak();
				peakIndexes.Add(index);
				amps.Add(amp);
				RemovePeak();
			}

			if (peakIndexes.Count == 0)
			{
				peakIndexes.Add(array1.Length / 2);
				amps.Add(0);
			}

			return peakIndexes;

			void RemovePeak()
			{
				for (int x = 0; x < array2.Length; x++)
					array2[x] -= FftRecognitionModel._model[x, index] * amp * AP._peakBig;
			}

			void FindPeak()
			{
				amp = 0;
				index = 0;
				for (int i = 0; i < array2.Length; i++)
					if (array2[i] > amp)
					{
						amp = array2[i];
						index = i;
					}
			}
		}
	}
}
