using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Extensions;


namespace MusGen
{
	public static class JpgToWav_MultiNad
	{
		public static void Make(string jpgInPath, string exportName)
		{
			Logger.Log($"Jpg to wav (MultiNad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			Nad nad = SsToMultiNad.Make(ss);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);

			Logger.Log($"Jpg to wav (MultiNad) finished. Saved as ({exportName})");
		}
	}
}
