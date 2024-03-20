using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace MusGen
{
	public static class MID_MID_exp
	{
		public static void Make(string path, string outName, float speed, float pitch)
		{
			Midi midi = new Midi();
			midi.Read(path);
			Nad nad = midi.ToNad();
			FNad fnad = nad.ToFNad();
			fnad.ToMidiAndExport(outName + "_FNAD", speed);
			Logger.Log("Done.");
		}
	}
}
