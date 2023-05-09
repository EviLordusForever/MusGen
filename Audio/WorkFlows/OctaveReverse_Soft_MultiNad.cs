using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class OctaveReverse_Soft_MultiNad
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Wav to octave reverse (Soft, MultiNad) started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			ss = SsSoftOctaveReverser.Make(ss);
			Nad nad = SsToMultiNad.Make(ss);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);

			Logger.Log($"Wav to octave reverse (Soft, MultiNad) finished. Saved as ({exportName})");
		}
	}
}
