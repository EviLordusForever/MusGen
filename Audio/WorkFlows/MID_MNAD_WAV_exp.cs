using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace MusGen
{
	public static class MID_MNAD_WAV_exp
	{
		public static Nad Make(string path, string outName, float speed, float pitch)
		{
			Midi midi = new Midi();
			midi.Read(path);
			Nad nad = midi.ToNad();
			Wav wav = EveryNadToWav.Make(nad);
			wav.Export(outName);
			nad.ToMidi(outName);
			FNad fnad = nad.ToFNad();
			fnad.Export(outName);
			FNad fnad2 = new FNad();
			fnad2.ReadFromExport(outName);
			fnad2.ToMidiAndExport(outName + "_FNAD");
			Logger.Log("Done.");
			return nad;
		}
	}
}
