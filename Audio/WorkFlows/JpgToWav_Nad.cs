using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Extensions;


namespace MusGen
{
	public static class JpgToWav_Nad
	{
		public static void Make(string jpgInPath, string exportName)
		{
			Logger.Log($"Jpg to wav (Nad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			Nad nad = SsToNad_Multi.Make(ss);
			Wav wav = NadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Jpg to wav (Nad) finished. Saved as ({exportName})");
		}
	}
}
