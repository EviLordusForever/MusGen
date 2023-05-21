using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavToWav_MultiNad
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Wav to wav (MultiNad) started for\n{wavInPath}");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			Nad nad = SsToMultiNad.Make(ss);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);
			Logger.Log($"Wav to wav (MultiNad) finished. Saved as ({exportName})");
		}
	}
}
