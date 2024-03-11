using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace MusGen
{
	public static class FNAD_MID_exp
	{
		public static void Make(string path, string outName, float speed, float pitch)
		{
			FNad fnad2 = new FNad();
			fnad2.Read(path);
			fnad2.ToMidiAndExport(outName + "_from_FNAD");
			Logger.Log("Done.");
		}
	}
}
