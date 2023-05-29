using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.WorkFlows
{
	public static class WavToMultiNad
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Wav to MultiNad started for\n{wavInPath}");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			Nad nad = SsToMultiNad.Make(ss);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad.Export(exportName);
			Logger.Log($"Wav to MultiNad finished. Saved as ({exportName})");
		}
	}
}