﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavToWav_FixedNad
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Wav to wav (FixedNad) started for\n{wavInPath}");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			Nad nad = WavToFixedNad.Make(wavIn);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);
			Logger.Log($"Wav to wav (FixedNad) finished. Saved as ({exportName})");
		}
	}
}
