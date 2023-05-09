using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class JpgOctaveReverse_Soft_Nad
	{
		public static void Make(string jpgInPath, string exportName)
		{
			Logger.Log($"Image to octave reverse (soft, nad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			ss = SsSoftOctaveReverser.Make(ss);
			Nad nad = SsToNad_Multi.Make(ss);
			Wav wav = NadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Image to octave reverse (soft, nad) finished. Saved as ({exportName})");
		}
	}
}
