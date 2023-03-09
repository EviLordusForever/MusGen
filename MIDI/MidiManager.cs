using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

namespace ELFMusGen
{
	public static class MidiManager
	{
		public static MidiFile Read(string path)
		{
			return MidiFile.Read(path);
		}
	}
}
