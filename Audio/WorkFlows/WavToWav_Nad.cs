using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavToWav_Nad
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Wav to nad to wav started for ({wavInPath}).");
			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			Nad nad = WavToNad.Make(wavIn);
			Wav wavOut = NadToWav.Make(nad);
			wavOut.Export(exportName);
			Logger.Log($"Wav to nad to wav finished. Saved as ({exportName})");
		}
	}
}
