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
	public static class WavToSpectrumImage
	{
		public static void Make(string wavInPath, string exportName)
		{
			Thread tr = new(Tr);
			tr.Name = "WavToImage";
			tr.Start();

			void Tr()
			{
				Logger.Log($"Wav to image started for ({wavInPath}).");
				Wav wavIn = new Wav();
				wavIn.Read(wavInPath);
				SS ss = WavToSSConvertor.Make(wavIn);
				WriteableBitmap img = SsToWbmpConvertor.Make(ss);
				string path = $"{DiskE._programFiles}\\Spectrograms\\{exportName}.jpg";
				GraphicsE.SaveJPG100(img, path);
				DialogE.ShowFolder(path);

				Logger.Log($"Wav to image finished. Saved as ({exportName})");
			}
		}
	}
}
