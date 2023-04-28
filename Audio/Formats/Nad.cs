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
	public class Nad
	{
		public int _channelsCount;
		public NadSample[] _samples;
		public int _duration;

		public Nad(int channelsCount, int samplesCount, int duration)
		{
			_channelsCount = channelsCount;
			_samples = new NadSample[samplesCount];
			_duration = duration;
		}

		public MidiFile MakeMidi()
		{
			MidiFile midiFile = new MidiFile();

			var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(110));
			midiFile.ReplaceTempoMap(tempoMap);

			var trackChunk1 = Build(tempoMap);
			midiFile.Chunks.Add(trackChunk1);

			var trackChunk2 = Build(tempoMap);
			midiFile.Chunks.Add(trackChunk2);

            // Write MIDI data to file. See https://github.com/melanchall/drywetmidi/wiki/Writing-a-MIDI-file

            return midiFile;
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