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
using Extensions;

namespace MusGen
{
	public class Nad
	{
		public int _channelsCount;
		public NadSample[] _samples;
		public float _duration;

		public Nad(int samplesCount, float duration)
		{
			_channelsCount = -1;
			_samples = new NadSample[samplesCount];
			_duration = duration;
		}

		public Nad(int channelsCount, int samplesCount, float duration)
		{
			_channelsCount = channelsCount;
			_samples = new NadSample[samplesCount];
			_duration = duration;
		}

		public int Width
		{
			get
			{
				return _samples.Length;
			}
		}

		public static NadSample Compare(NadSample oldNad, NadSample newNad)
		{
			int c = AP._channels;
			int[,] distances = new int[c, c];
			float x = 0;
			float y = 0;

			for (int idNew = 0; idNew < c; idNew++)
				for (int idOld = 0; idOld < c; idOld++)
				{
					x = oldNad._indexes[idOld] - newNad._indexes[idNew];
					x /= (SpectrumFinder._spectrumLinear.Length / 2);
					x *= 4; //check

					y = oldNad._amplitudes[idOld] - newNad._amplitudes[idNew];

					distances[idOld, idNew] = (int)(1000 * Math.Sqrt(x * x + y * y));
				}

			int[] compares = HungarianAlgorithm.Run(distances);

			int[] _indexesCompared = new int[AP._channels];
			float[] _amplitudesCompared = new float[AP._channels];
			float[] _frequenciesCompared = new float[AP._channels];

			for (int id = 0; id < c; id++)
			{
				int idCompared = compares[id];

				_indexesCompared[id] = newNad._indexes[idCompared];
				_frequenciesCompared[id] = newNad._frequencies[idCompared];
				_amplitudesCompared[id] = newNad._amplitudes[idCompared];
			}

			newNad._indexes = _indexesCompared;
			newNad._amplitudes = _amplitudesCompared;
			newNad._frequencies = _frequenciesCompared;

			return newNad;
		}

        private TrackChunk Build(TempoMap tempoMap)
        {
            ProgramChangeEvent pce = new ProgramChangeEvent((SevenBitNumber)1);
            TrackChunk trackChunk = new TrackChunk(pce);

            using (var chordsManager = trackChunk.ManageChords())
            {
                for (int s = 0; s < _samples.Length; s++)
                {
                    var chords = chordsManager.Objects;

                    Note[] notes = _samples[s].GetMidiNotes();

                    chords.Add(new Chord(notes, s * 50));////
                }
            }

            return trackChunk;
        }

        public static float F(float t)
		{
			return MathF.Sign(MathF.Sin(t));
		}
	}
}