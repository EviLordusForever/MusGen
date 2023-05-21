using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class JpgNadOctaveReverse_Soft_MultiNad
	{
		public static void Make(string jpgInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Image to nad octave reverse (soft, MultiNad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);	
			SS ssO = WbmpToSs.Make(wbmp);
			float octaveShift = OctaveShifter.FindShift(ssO);
			SS ssR = SsSoftOctaveReverser.Make(ssO, octaveShift, AP._octavesForReverse);
			Nad nad = SsToMultiNad.Make(ssO);
			nad = MultiNadSoftOctaveReverser.Make(nad, octaveShift, AP._octavesForReverse);
			nad = SuperEqualiser.Make(nad, ssO, ssR);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			nad = NadLowSmoother.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Image to nad octave reverse (soft, MultiNad) finished. Saved as ({exportName})");
		}
	}
}
