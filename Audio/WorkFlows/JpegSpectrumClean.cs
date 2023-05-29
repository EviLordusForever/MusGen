using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusGen
{
	public static class JpegSpectrumClean
	{
		public static void Make(string jpgInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Image to clean image (MultiNad) started for\n{jpgInPath}");

			WriteableBitmap wbmp = WBMP.Load(jpgInPath);
			SS ss = WbmpToSs.Make(wbmp);
			ss = SsTrueCleaner.Make(ss);
			DiskE.WriteToProgramFiles("delme2", "csv", ss.ToCsv(), false);
			ss = SsNormalizer.Make(ss);
			WriteableBitmap wbmp2 = SsToWbmp.Make(ss);
			string path = $"{DiskE._programFiles}Spectrograms\\{exportName}.jpg";
			GraphicsE.SaveJPG100(wbmp2, path);
			DialogE.ShowFile(path);

			Logger.Log($"Image to clean iamge (MultiNad) finished. Saved as ({exportName})");
		}
	}
}
