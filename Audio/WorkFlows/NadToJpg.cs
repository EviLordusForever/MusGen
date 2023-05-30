using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.WorkFlows
{
	public static class NadToJpg
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"Nad to Jpeg started for\n{nadInPath}");
			Nad nad = new Nad();
			nad.Read(nadInPath);
			var wbmp = NadToWbmp.Make(nad);
			WBMP.Export(wbmp, exportName);
			Logger.Log($"Nad to Jpeg finished. Saved as ({exportName})");
		}

	}
}
