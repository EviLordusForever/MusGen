using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.WorkFlows
{
	public static class NAD_NORS_exp
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"NAD_NORS_exp started for\n{nadInPath}");

			Nad nad_O = new Nad(0, 0, 0, 0);
			nad_O.Read(nadInPath);
			float octaveShift = OctaveShifter.FindShift(nad_O);
			Nad nad_R = MultiNadSoftOctaveReverser.Make(nad_O, octaveShift, AP._octavesForReverse);
			nad_R = SuperEqualiser.Make(nad_R, nad_O);
			nad_R = NadModifySpeedAndPitch.Make(nad_R, speed, pitch);
			nad_R = NadLimitsFilter.Make(nad_R);
			nad_R = NadLowSmoother.Make(nad_R);
			Wav wavOut = EveryNadToWav.Make(nad_R);
			wavOut.Export(exportName + " (NAD_NORS)");

			Logger.Log($"NAD_NORS_exp finished. Saved as ({exportName})");
		}
	}
}
