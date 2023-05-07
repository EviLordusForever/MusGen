using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Extensions;


namespace MusGen
{
	public static class JpgToWav
	{
		public static void Make(string jpgInPath, string exportName)
		{
			Thread tr = new(Tr);
			tr.Name = "JpgToWav";
			tr.Start();

			void Tr()
			{
				Logger.Log($"Jpg to wav started for ({jpgInPath}).");

				WriteableBitmap wbmp = WBMP.Load(jpgInPath);
				SS ss = WbmpToSsConvertor.Make(wbmp);
				Nad nad = SsToNadConvertor.Make(ss);
				Wav wav = NadToWavConvertor.Make(nad);
				wav.Export(exportName);

				Logger.Log($"Jpg to wav finished. Saved as ({exportName})");
			}
		}
	}
}
