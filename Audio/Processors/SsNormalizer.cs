using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SsNormalizer
	{
		public static SS Make(SS ss)
		{
			ProgressShower.Show("SS normalisation part 1...");
			int step = (int)(MathF.Max(1, ss.Width / 1000f));

			float max = 0;

			for (int s = 0; s < ss.Width; s++)
			{
				for (int c = 0; c < ss.Height; c++)
					if (ss._s[s][c] > max) 
						max = ss._s[s][c];

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / ss.Width);
			}

			ProgressShower.Show("SS normalisation part 2...");
			ProgressShower.Set(0);

			for (int s = 0; s < ss.Width; s++)
			{
				for (int c = 0; c < ss.Height; c++)
					ss._s[s][c] /= max;

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / ss.Width);
			}

			ProgressShower.Close();
			return ss;
		}
	}
}
