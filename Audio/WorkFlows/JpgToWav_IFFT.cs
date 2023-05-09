using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Extensions;


namespace MusGen
{
	public static class JpgToWav_IFFT
	{
		public static void Make(string jpgInPath, string exportName)
		{
			Logger.Log($"Jpg to wav (IFFT) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			//ss = SsCleaner.Make(ss);
			Wav wav = SsToWav.Make(ss);
			wav.Export(exportName);

			Logger.Log($"Jpg to wav (IFFT) finished. Saved as ({exportName})");
		}
	}
}
