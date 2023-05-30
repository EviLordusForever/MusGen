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
using System.Collections;

namespace MusGen
{
	[Serializable]
	public class NadSample
	{
		public ushort[] _indexes;

		public float[] _amplitudes;

		public void RaisePitch(float pitch)
		{
			for (int c = 0; c < Height; c++)
				_indexes[c] += (ushort)(SpectrumFinder._octaveSize * MathF.Log2(pitch));
				//check mb ^
		}

		public void Filter(float sps)
		{
			List<ushort> newIndexes = new List<ushort>();
			List<float> newAmplitudes = new List<float>();

			for (int c = 0; c < Height; c++)
			{
				float frq = SpectrumFinder._frequenciesLg[_indexes[c]];

				if (frq < AP.SampleRate / 2 &&
					frq > SpectrumFinder._frequenciesLg[1] &&
					_amplitudes[c] > AP._nadMin)
				{
					newIndexes.Add(_indexes[c]);
					newAmplitudes.Add(_amplitudes[c]);
				}
			}

			_indexes = newIndexes.ToArray();
			_amplitudes = newAmplitudes.ToArray();
		}

		public void Add(ushort index, float amp)
		{
			if (_indexes.Contains(index))
			{
				int i = Array.IndexOf(_indexes, index);
				_amplitudes[i] = amp * 0.5f + _amplitudes[i] * 0.5f;
			}
			else
			{
				Array.Resize(ref _indexes, _indexes.Length + 1);
				Array.Resize(ref _amplitudes, _amplitudes.Length + 1);

				_indexes[_indexes.Length - 1] = index;
				_amplitudes[_amplitudes.Length - 1] = amp;
			}
		}


		public void Normalise(float max)
		{
			for (int c = 0; c < Height; c++)
				_amplitudes[c] /= max;
		}

		public void EQ(float[] curve, ref float max)
		{
			for (int c = 0; c < Height; c++)
			{
				_amplitudes[c] *= curve[_indexes[c]];
				max = max > _amplitudes[c] ? max : _amplitudes[c];
			}
		}

		public NadSample(int height)
		{
			_amplitudes = new float[height];
			_indexes = new ushort[height];
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
			Note[] notes = new Note[_indexes.Length];
			for (int n = 0; n < notes.Length; n++)
			{
				float frequency = SpectrumFinder._frequenciesLg[_indexes[n]];
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
