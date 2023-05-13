using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class NadLimitsFilter
	{
		public static Nad Make(Nad nad)
		{
			ProgressShower.Show("Nad filtering by limits...");
			int step = (int)(Math.Max(1, nad.Width / 1000f));

			float sps = nad.Width / nad._duration;

			for (int s = 0; s < nad._samples.Length; s++)
			{
				nad._samples[s].Filter(sps);
				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return nad;
		}
	}
}
