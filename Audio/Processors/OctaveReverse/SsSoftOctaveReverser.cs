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
				int fromBefore = (int)(SpectrumFinder._octavesIndexes[octave] - _octaveSize / 2 + octaveShift);
				int toBefore = (int)(SpectrumFinder._octavesIndexes[octave + 1] + _octaveSize / 2 + octaveShift);
				int count = toBefore - fromBefore;
				int toShifted = toBefore - (int)octaveShift;
				int fromShifted = fromBefore - (int)octaveShift;

				for (int i = fromBefore; i < toBefore; i++)
				{
					int rev = toBefore - 1 - (i - fromBefore);
					int orig = i;

					int point = rev;
					if (!octaves[octave])
						point = orig;

					int iShift = i - (int)octaveShift;

					if (useRev2)
					{
						if (iShift >= 0 && orig >= 0 && iShift < _height && orig < _height)
						{
							if (rev >= 0 && rev < _height)
							{
								float power = F(1f * (i - fromBefore) / count);
								newone[iShift] += spectrum[point] * power;
							}
						}
					}
					else
					{
						if (iShift >= 0 && point >= 0 && iShift < _height && point < _height)
						{
							float power = F(1f * (i - fromBefore) / count);
							newone[iShift] += spectrum[point] * power;
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
