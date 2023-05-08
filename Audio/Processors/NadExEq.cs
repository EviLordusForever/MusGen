using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class NadExEq
	{
		public static Nad Make(Nad nad, SS ex)
		{
			ProgressShower.Show("Nad equalising...");
			int step = (int)(nad._samples.Length / 1000f);

			int channels = nad._samples[0]._frequencies.Length;

			for (int s = 0; s < nad._samples.Length; s++)
			{
				for (int c = 0; c < channels; c++)
				{
					int index = nad._samples[s]._indexes[c];
					float amp = ex._s[s][index];
					if (nad._samples[s]._amplitudes[c] > amp)
						nad._samples[s]._amplitudes[c] = amp;
				}

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / step);
			}

			ProgressShower.Close();

			return nad;
		}
	}
}
