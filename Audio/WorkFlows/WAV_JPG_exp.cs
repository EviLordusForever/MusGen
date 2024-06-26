﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;
using Extensions;
using System.Diagnostics;

namespace MusGen
{
	public static class WAV_JPG_exp
	{
		public static void Make(string wavInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"WAV_JPG_exp started for\n{wavInPath}");

			Wav wavIn = new Wav();
			wavIn.Read(wavInPath);
			SS ss = WavToSS.Make(wavIn);
			WriteableBitmap wbmp = SsToWbmp.Make(ss);
			string path = $"{DiskE._programFiles}Spectrograms\\{exportName}.jpg";
			WBMP.Export(wbmp, exportName + " (NAD_JPG)");
			
			Logger.Log($"WAV_JPG_exp finished. Saved as ({exportName})");
		}
	}
}
