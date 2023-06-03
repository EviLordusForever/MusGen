using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.WorkFlows
{
	public static class NAD_JPG_exp
	{
		public static void Make(string nadInPath, string exportName, float speed, float pitch)
		{
			Logger.Log($"NAD_JPG_exp for\n{nadInPath}");
			Nad nad = new Nad();
			nad.Read(nadInPath);
			var wbmp = NadToWbmp.Make(nad);
			WBMP.Export(wbmp, exportName + " (NAD_JPG)");
			Logger.Log($"NAD_JPG_exp finished. Saved as ({exportName})");
		}

	}
}
