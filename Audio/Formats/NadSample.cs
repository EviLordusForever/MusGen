﻿using System;
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
using System.Collections;

namespace MusGen
{
	public class NadSample
	{
		[JsonIgnore]
		public int[] _indexes;

		public float[] _frequencies;

		public float[] _amplitudes;

		public void RaisePitch(float pitch)
		{
			for (int c = 0; c < Height; c++)
			{
				_frequencies[c] *= pitch;
				_indexes[c] += (int)(SpectrumFinder._octaveSize * MathF.Log2(pitch));
				//check mb ^
			}
		}

		public void Filter(float sps)
		{
			List<float> newFrequencies = new List<float>();
			List<int> newIndexes = new List<int>();
			List<float> newAmplitudes = new List<float>();

			for (int c = 0; c < Height; c++)
				if (_frequencies[c] < AP.SampleRate / 2 && _frequencies[c] > sps)
				{
					newFrequencies.Add(_frequencies[c]);
					newIndexes.Add(_indexes[c]);
					newAmplitudes.Add(_amplitudes[c]);
				}

			_frequencies = newFrequencies.ToArray();
			_indexes = newIndexes.ToArray();
			_amplitudes = newAmplitudes.ToArray();
		}

		public NadSample(int channels)
		{
			_frequencies = new float[channels];
			_amplitudes = new float[channels];
			_indexes = new int[channels];
		}

		public int Height
		{
			get
			{
				return _indexes.Length;
			}
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
