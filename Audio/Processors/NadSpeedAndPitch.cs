using System;

namespace MusGen
{
	public static class NadModifySpeedAndPitch
	{
		public static Nad Make(Nad nad, float speed, float pitch)
		{
			ProgressShower.Show("Modifying speed and pitch...");
			int progressStep = (int)(Math.Max(1, nad.Width / 1000f));

			nad._duration /= speed;

			for (int s = 0; s < nad.Width; s++)
			{
				nad._samples[s].RaisePitch(pitch);
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / progressStep);
			}

			ProgressShower.Close();

			return nad;
		}
	}
}
