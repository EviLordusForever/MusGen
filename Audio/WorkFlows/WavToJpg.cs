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
			Thread tr = new(Tr);
			tr.Name = "WavToJpg";
			tr.Start();

			void Tr()
			{
				Logger.Log($"Wav to jpg started for ({wavInPath}).");
				Wav wavIn = new Wav();
				wavIn.Read(wavInPath);
				SS ss = WavToSSConvertor.Make(wavIn);
				WriteableBitmap img = SsToWbmpConvertor.Make(ss);
				string path = $"{DiskE._programFiles}\\Spectrograms\\{exportName}.jpg";
				GraphicsE.SaveJPG100(img, path);
				DialogE.ShowFolder(path);

				Logger.Log($"Wav to jpg finished. Saved as ({exportName})");
			}
		}
	}
}
