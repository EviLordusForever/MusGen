using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class OctaveReverse_Soft_Nad
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Wav to soft ss octave reverse started for ({wavInPath}).");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			ss = SsSoftOctaveReverser.Make(ss);
			Nad nad = SsToNad.Make(ss, true);
			Wav wavOut = NadToWav.Make(nad);
			wavOut.Export(exportName);

			Logger.Log($"Wav to soft ss octave reverse finished. Saved as ({exportName})");
		}
	}
}
