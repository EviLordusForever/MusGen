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
			ProgressShower.Show("Ss stupied cleaning...");
			int step = (int)(MathF.Max(1, ssin.Width / 1000f));

			SS ssout = new SS(ssin.Width, ssin._sps, ssin._cs);

			for (int s = 0; s < ssin.Width; s++)
			{
				ssout._s[s] = MathE.StupiedFilter(ssin._s[s], true); //

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return ssout;
		}
	}
}
