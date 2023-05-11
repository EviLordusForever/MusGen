using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SsLowPhaseSmoother
	{
		public static SS Make(SS ss)
		{
			ProgressShower.Show("Ss low phase smoothing.");
			float frequency = 0;
			for (int c = 0; frequency < ss._sps; c++)
			{
				frequency = SpectrumFinder._frequenciesLogarithmic[c];
				int n = (int)MathF.Ceiling(ss._sps / frequency);

				float summ = 0;

				for (int s = 0; s < ss.Width + n; s++)
				{
					if (s < ss.Width)
						summ += ss._s[s][c];
					else
						summ += ss._s[ss.Width - 1][c];

					if (s >= n)
					{
						ss._s[s - n][c] = summ / n;
						summ -= ss._s[s - n][c];
					}
				}
			}

			ProgressShower.Close();

			return ss;
		}
	}
}
