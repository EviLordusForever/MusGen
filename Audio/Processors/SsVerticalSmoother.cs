using System;

namespace MusGen
{
	public static class SSVerticalSmoother
	{
		public static SS Make(SS ss, int factor)
		{
			SS ssout = new SS(ss._s.Length, ss._sps);

			int y = ss._s[0].Length;

			ProgressShower.Show("Ss smoothing...");
			int step = (int)(ss._s.Length / 1000f);

			for (int s = 0; s < ss._s.Length; s++)
			{
				ssout._s[s] = new float[y];

				for (int c = 0; c < y; c++)
				{
					int from = Math.Max(c - factor, 0);
					int to = Math.Min(c + factor, y);
					int count = from - to;

					float sum = 0;

					for (int i = from; i <= from; i++)
						sum += ss._s[s][i];

					ssout._s[s][c] = sum / count;
				}

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return ssout;
		}
	}
}
