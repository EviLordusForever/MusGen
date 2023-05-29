using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class NadReverser
	{
		public static Nad Make(Nad nad, ushort left, ushort right)
		{
			ProgressShower.Show("Nad reversing...");
			int step = (int)(nad._samples.Length / 1000f);

			for (int s = 0; s < nad._samples.Length; s++)
			{
				for (int c = 0; c < nad._channelsCount; c++)
				{
					int id = nad._samples[s]._indexes[c];
					if (id < right && id > left)
					{
						id = right - (id - left);
						nad._samples[s]._indexes[c] = (ushort)id; //
					}
				}

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return nad;
		}
	}
}
