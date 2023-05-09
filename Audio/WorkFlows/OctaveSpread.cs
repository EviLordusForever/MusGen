using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen.Audio.WorkFlows
{
	public static class OctaveSpread
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Octave spread started for\n{wavInPath}");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			Nad nad = WavToFixedNad.Make(wavIn);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);
			Logger.Log($"Octave spread finished. Saved as ({exportName})");
		}
	}
}
