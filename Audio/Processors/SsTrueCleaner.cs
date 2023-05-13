using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SsTrueCleaner
	{
		public static SS Make(SS ss)
		{
			ProgressShower.Show("Ss to true ss...");
			float step = (int)Math.Max(1, ss.Width / 2000f);

			for (int s = 0; s < ss.Width; s++)
			{
				ss._s[s] = ss._s[s].Reverse().ToArray();
				ss._s[s] = SpectrumToTrueSpectrum.Make(ss._s[s]);
				ss._s[s] = ss._s[s].Reverse().ToArray();

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / ss.Width);
			}

			ProgressShower.Close();
			return ss;
		}
	}
}
