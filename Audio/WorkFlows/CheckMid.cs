using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class CheckMid
	{
		public static void Make(string path, string outName, float speed, float pitch)
		{
			Midi midi = new Midi();
			midi.Read(path);
			Nad nad = midi.ToNad();
			FNad fnad = nad.ToFNad();
			fnad.UltraNormalize();			
			fnad.ToMidiAndExport(outName + "_Ultra");
			Logger.Log("Done.");
		}
	}
}
