using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class NadToWav
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Nad to Wav started for\n{nadInPath}");
			Nad nad = new Nad(0, 0, 0, 0);
			nad.Read(nadInPath);
			nad = NadLowSmoother.Make(nad);
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(exportName);
			Logger.Log($"Nad to Wav finished. Saved as ({exportName})");
		}
	}
}
