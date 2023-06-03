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
	public static class WAV_SORS_MNAD_WAV_exp
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"WAV_SORS_MNAD_WAV_exp started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ssO = WavToSS.Make(wavIn);
			float octaveShift = OctaveShifter.FindShift(ssO);
			SS ssR = SsSoftOctaveReverser.Make(ssO, octaveShift, AP._octavesForReverse);
			ssR = SuperEqualiser.Make(ssO, ssR);
			Nad nad = SsToMultiNad.Make(ssR);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName + " (WAV_SORS_MNAD_WAV)");

			Logger.Log($"WAV_SORS_MNAD_WAV_exp finished. Saved as ({exportName})");
		}
	}
}
