using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsDelogariphmisator
	{
		public static SS Make(SS ssin)
		{
			ProgressShower.Show("Ss delogariphmisation...");
			int step = (int)(ssin._s.Length / 1000f);

			int width = ssin._s.Length;
			int height = ssin._s[0].Length;

			for (int s = 0; s < width; s++)
			{
				ssin._s[s] = ArrayE.RescaleArrayFromLog(ssin._s[s], height * 2, height);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return ssin;
		}
	}
}
