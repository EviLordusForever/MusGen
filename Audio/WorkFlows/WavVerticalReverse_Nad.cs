using System;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using Extensions;

namespace MusGen
{
	public static class WavVerticalReverse_Nad
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Wav vertical reverse (nad) started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			Nad nad = SsToNad.Make(ss, true);
			int left = (int)SpectrumFinder._octavesIndexes[0];
			int right = (int)SpectrumFinder._octavesIndexes[9];
			nad = NadReverser.Make(nad, left, right);
			ss = SSVerticalSmoother.Make(ss, 5);
			nad = NadExEq.Make(nad, ss);
			Wav wavOut = NadToWav.Make(nad);
			wavOut.Export(exportName);

			Logger.Log($"Wav vertical reverse (nad) finished. Saved as ({exportName})");
		}
	}
}
