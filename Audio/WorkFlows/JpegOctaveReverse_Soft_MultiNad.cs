﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class JpgOctaveReverse_Soft_MultiNad
	{
		public static void Make(string jpgInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Image to octave reverse (soft, MultiNad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			ss = SsLowPhaseSmoother.Make(ss);
			ss = SsSoftOctaveReverser.Make(ss);
			ss = SsLowPhaseSmoother.Make(ss);
			Nad nad = SsToMultiNad.Make(ss);
			nad = NadModifySpeedAndPitch.Make(nad, speed, pitch);
			nad = NadLimitsFilter.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Image to octave reverse (soft, MultiNad) finished. Saved as ({exportName})");
		}
	}
}