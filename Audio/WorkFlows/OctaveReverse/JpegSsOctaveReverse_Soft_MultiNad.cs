using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class JpgSsOctaveReverse_Soft_MultiNad
	{
		public static void Make(string jpgInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Image to ss octave reverse (soft, MultiNad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ssO = WbmpToSs.Make(wbmp);
			float octaveShift = OctaveShifter.FindShift(ssO);
			SS ssR = SsSoftOctaveReverser.Make(ssO, octaveShift, AP._octavesForReverse);
			ssR = SuperEqualiser.Make(ssO, ssR);
			Nad nad = SsToMultiNad.Make(ssR);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Image to ss octave reverse (soft, MultiNad) finished. Saved as ({exportName})");
		}
	}
}
