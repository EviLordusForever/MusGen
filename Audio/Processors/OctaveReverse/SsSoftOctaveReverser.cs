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

		public static SS Make(SS ss, float octaveShift, bool[] octaves)
		{
			ProgressShower.Show("Ss soft octave reversing...");
			int step = (int)(MathF.Max(1, ss.Width / 1000f));

			Init(ss.Width, ss.Height);

			SS ssout = new SS(_width, ss._sps, ss._cs);

			for (int s = 0; s < _width; s++)
			{
				ssout._s[s] = MakeOne(ss._s[s], octaveShift, octaves);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / _width);
			}

			ProgressShower.Close();

			return ssout;
		}		

		public static float[] MakeOne(float[] spectrum, float octaveShift, bool[] octaves, bool useRev2 = false)
		{
			float[] newone = new float[spectrum.Length];

			for (int octave = 0; octave < 9; octave++)
			{
				int from = (int)(SpectrumFinder._octavesIndexes[octave] - _octaveSize / 2 + octaveShift);
				int to = (int)(SpectrumFinder._octavesIndexes[octave + 1] + _octaveSize / 2 + octaveShift);
				int count = to - from;

				for (int i = from; i < to; i++)
				{
					int rev = to - 1 - (i - from);
					int rev2 = rev;
					if (!octaves[octave])
						rev = i;
					if (useRev2)
					{
						if (i >= 0 && rev2 >= 0 && i < _height && rev2 < _height)
						{
							float power = F(1f * (i - from) / count);
							newone[i] += spectrum[rev] * power;
						}
					}
					else
					{
						if (i >= 0 && rev >= 0 && i < _height && rev < _height)
						{
							float power = F(1f * (i - from) / count);
							newone[i] += spectrum[rev] * power;
						}
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
