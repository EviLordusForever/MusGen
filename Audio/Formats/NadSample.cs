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
using Newtonsoft.Json;


namespace MusGen
{
	public class NadSample
	{
		[JsonIgnore]
		public int[] _indexes;

		public float[] _frequencies;

		public float[] _amplitudes;

		public NadSample(int channels)
		{
			_frequencies = new float[channels];
			_amplitudes = new float[channels];
			_indexes = new int[channels];
		}

		public Note[] GetMidiNotes()
		{
			Note[] notes = new Note[_frequencies.Length];
			for (int n = 0; n < notes.Length; n++)
			{
				float frequency = _frequencies[n];
				byte note = (byte)(Math.Log2(frequency) * 100000000);

				SevenBitNumber sbn = new SevenBitNumber(note);
				notes[n] = new Note(sbn); ////////////////////////////
				notes[n].Velocity = new SevenBitNumber((byte)(_amplitudes[n] * 127));
				notes[n].Length = 450;
				notes[n].OffVelocity = new SevenBitNumber(127);
			}

			return notes;
		}		
	}
}
