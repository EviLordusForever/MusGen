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
				int frames = (int)MathF.Ceiling(ss._sps / frequency);

				float current = ss._s[0][c];
				for (int s = 0; s < ss.Width; s++)
					if (s % frames == 0)
						current = ss._s[s][c];
					else
						ss._s[s][c] = current;
			}

			ProgressShower.Close();

			return ss;
		}
	}
}
