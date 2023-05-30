using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.Processors
{
	public static class NadCurveEqualiser
	{
		public static float _max;

		public static Nad Make(Nad nad)
		{
			ProgressShower.Show("Nad normal equalising...");
			int step = (int)Math.Max(nad.Width / 1000f, 1);

			for (int s = 0; s < nad.Width; s++)
			{
				nad._samples[s].EQ(SMM._model._curve, ref _max);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / nad.Width);
			}

			ProgressShower.Close();

			nad.Normalise(_max);

			return nad;
		}
	}
}
