using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

namespace MusGen
{
	public class Midi
	{
		public MidiFile _midiFile;

		public void Read(string path)
		{
			_midiFile = MidiFile.Read(path);
		}
	}
}
