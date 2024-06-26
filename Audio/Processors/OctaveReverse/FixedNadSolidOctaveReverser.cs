﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class FixedNadSolidOctaveReverser
	{
		public static Nad Make(Nad nad)
		{
			ProgressShower.Show("Fixed nad solid octave reversing...");
			int step = (int)(MathF.Max(1, nad.Width / 1000f));

			float[] octaves = SpectrumFinder._octavesIndexes;

			int globalRight = (int)octaves[9];
			int globalLeft = (int)octaves[0];
			int right = 0;
			int left = 0;

			for (int s = 0; s < nad.Width; s++)
			{
				for (int c = 0; c < nad._channelsCount; c++)
				{
					int id = nad._samples[s]._indexes[c];
					if (id < globalRight && id > globalLeft)
					{
						FindOctave(id);

						id = right - (id - left);
						if (id > 0)
						{
							nad._samples[s]._indexes[c] = (ushort)id;
						}
						else
							nad._samples[s]._amplitudes[c] = 0;
					}
					else
						nad._samples[s]._amplitudes[c] = 0;				
				}

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();
			Logger.Log("Nad octaves reversed solid.");

			return nad;

			void FindOctave(int id)
			{
				for (int i = 1; i <= 9; i++)
					if (id <= octaves[i] && id > octaves[i - 1])
					{
						right = (int)octaves[i];
						left = (int)octaves[i - 1];
						return;
					}

				right = globalLeft;
				left = globalRight;
			}
		}
	}
}
