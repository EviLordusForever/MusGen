using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;
using System.Windows.Media.Imaging;
using System.IO;

namespace MusGen
{
	public static class NadOctaveReverse_Soft_MultiNad
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Wav to nad octave reverse (Soft, MultiNad) started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ssO = WavToSS.Make(wavIn);
			float octaveShift = OctaveShifter.FindShift(ssO);
			SS ssR = SsSoftOctaveReverser.Make(ssO, octaveShift, AP._octavesForReverse);
			Nad nad = SsToMultiNad.Make(ssO);
			nad = MultiNadSoftOctaveReverser.Make(nad, octaveShift, AP._octavesForReverse);
			nad = SuperEqualiser.Make(nad, ssO, ssR);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);

			Logger.Log($"Wav to nad octave reverse (Soft, MultiNad) finished. Saved as ({exportName})");
		}
	}
}
