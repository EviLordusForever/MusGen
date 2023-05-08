using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SsReverser
	{
		public static SS Make(SS ssin)
		{
			ProgressShower.Show("Ss reversing...");
			int step = (int)(ssin._s.Length / 1000f);

			SS ssout = new SS(ssin._s.Length, ssin._sps);

			int y = ssin._s[0].Length;

			for (int s = 0; s < ssin._s.Length; s++)
			{
				ssout._s[s] = new float[y];

				for (int c = 0; c < y; c++)
					ssout._s[s][c] = ssin._s[s][y - 1 - c];

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return ssout;
		}
	}
}
