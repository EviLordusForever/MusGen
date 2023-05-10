using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;
using Extensions;
using System.Diagnostics;

namespace MusGen
{
	public static class WavToJpg
	{
		public static void Make(string wavInPath, string exportName)
		{
			Logger.Log($"Wav to jpg started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			ss = SsLowPhaseSmoother.Make(ss);
			WriteableBitmap img = SsToWbmp.Make(ss);
			string path = $"{DiskE._programFiles}Spectrograms\\{exportName}.jpg";
			GraphicsE.SaveJPG100(img, path);
			DialogE.ShowFile(path);

			Logger.Log($"Wav to jpg finished. Saved as ({exportName})");
		}
	}
}
