using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class Octaver
	{
		public static float[] Do(float[] array)
		{
			int octaveSize = (int)((SpectrumFinder._octavesIndexes[9] - SpectrumFinder._octavesIndexes[0]) / 9 + 0.5f);
			float[] newArray = new float[octaveSize];

			for (int i = 0; i < 9; i++)
			{
				int start = (int)(SpectrumFinder._octavesIndexes[i]);
				int end = (int)(SpectrumFinder._octavesIndexes[i + 1]);
				int count = end - start;

				float[] subArray = ArrayE.SubArray(array, start, count);
				subArray = ArrayE.InterpolateArray(subArray, octaveSize);

				for (int j = 0; j < octaveSize; j++)
					newArray[j] += subArray[j];
			}

			for (int j = 0; j < newArray.Count(); j++)
				newArray[j] /= 9;

			return newArray;
		}
	}
}
