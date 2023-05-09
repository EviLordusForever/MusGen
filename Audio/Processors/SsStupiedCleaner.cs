using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

namespace MusGen
{
	public static class SsStupiedCleaner
	{
		public static SS Make(SS ssin)
		{
			ProgressShower.Show("Ss cleaning...");
			int step = (int)(ssin._s.Length / 1000f);

			SS ssout = new SS(ssin.Width, ssin._sps);

			for (int s = 0; s < ssin.Width; s++)
			{
				ssout._s[s] = MathE.StupiedFilter(ssin._s[s]);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return ssout;
		}
	}
}
