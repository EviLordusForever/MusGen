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
			Nad nad = WavToNad.Make(wavIn);
			Wav wavOut = NadToWav.Make(nad);
			wavOut.Export(exportName);
			Logger.Log($"Octave spread finished. Saved as ({exportName})");
		}
	}
}
