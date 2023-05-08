using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SsSoftOctaveReverser
	{
		private static float _octaveSize;
		private static int _width;
		private static int _height;

		public static SS Make(SS ss)
		{
			Init(ss.Width, ss.Height);

			SS ssout = new SS(_width, ss._sps);

			for (int s = 0; s < _width; s++)
				ssout._s[s] = MakeOne(ss._s[s]);

			return ssout;
		}		

		public static float[] MakeOne(float[] spectrum)
		{
			float[] newone = new float[spectrum.Length];

			for (int octave = 0; octave < 9; octave++)
			{
				int from = (int)(SpectrumFinder._octavesIndexes[octave] - _octaveSize / 2);
				int to = (int)(SpectrumFinder._octavesIndexes[octave + 1] + _octaveSize / 2);
				int count = to - from;

				for (int i = from; i < to; i++)
				{
					int rev = to - 1 - (i - from);
					if (octave == 8)
						rev = i;
					if (i >= 0 && rev >= 0 && i < _height && rev < _height)
					{
						float power = F(1f * (i - from) / count);
						newone[i] += spectrum[rev] * power;
					}
				}
			}

			return newone; 

			float F(float x)
			{
				return (MathF.Cos((2 * x + 1) * MathF.PI) + 1f) / 2f;
			}
		}

		public static void Init(int width, int height)
		{			
			_width = width;
			_height = height;
			float difference = SpectrumFinder._octavesIndexes[9] - SpectrumFinder._octavesIndexes[0];
			_octaveSize = difference / 9;
		}
	}
}
