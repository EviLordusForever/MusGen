using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using mt = Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Standards;
using Melanchall.DryWetMidi.Tools;


namespace MusGen
{
	public class NadSample
	{
		public float[] periods; //logarithmic?
		public float[] amplitudes;

		public NadSample(int channelsCount)
		{
			periods = new float[channelsCount];
			amplitudes = new float[channelsCount];
		}

		public Note[] GetMidiNotes(byte delme)
		{
			Note[] notes = new Note[periods.Length];
			for (int n = 0; n < notes.Length; n++)
			{
				float frequency = 1 / periods[n];
				byte note = (byte)(Math.Log2(frequency) * 100000000);

				SevenBitNumber sbn = new SevenBitNumber(note);
				notes[n] = new Note(sbn); ////////////////////////////
				notes[n].Velocity = new SevenBitNumber((byte)(amplitudes[n] * 127));
				notes[n].Length = 450;
				notes[n].OffVelocity = new SevenBitNumber(127);
			}

			return notes;
		}
	}
}
