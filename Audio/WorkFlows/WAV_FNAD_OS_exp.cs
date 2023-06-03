using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen.Audio.WorkFlows
{
	public static class WAV_FNAD_OS_exp
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"WAV_FNAD_OS_exp started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			Nad nad = WavToFixedNad.Make(wavIn);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName + " (WAV_FNAD_OS)");

			Logger.Log($"WAV_FNAD_OS_exp finished. Saved as ({exportName})");
		}
	}
}
