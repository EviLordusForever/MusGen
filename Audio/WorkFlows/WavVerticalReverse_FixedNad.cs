using System;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavVerticalReverse_FixedNad
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Wav vertical reverse (FixedNad) started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			ss = SsLowPhaseSmoother.Make(ss);
			Nad nad = SsToFixedNad.Make(ss, true);
			int left = (int)SpectrumFinder._octavesIndexes[0];
			int right = (int)SpectrumFinder._octavesIndexes[9];
			nad = NadReverser.Make(nad, left, right);
			ss = SSVerticalSmoother.Make(ss, 5);
			nad = NadExampleEq.Make(nad, ss);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			Wav wavOut = EveryNadToWav.Make(nad);
			wavOut.Export(exportName);

			Logger.Log($"Wav vertical reverse (FixedNad) finished. Saved as ({exportName})");
		}
	}
}
