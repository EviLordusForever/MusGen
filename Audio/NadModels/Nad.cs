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
		public int channelsCount;
		public NadSample[] samples;
		public float bps;

		public void MakeNad(Wav wav, int channelsCount)
		{
			this.channelsCount = channelsCount;
			bps = BPSFinder.FindBPS(wav);
			float sps = wav.sampleRate / bps;
			int nadSamplesCount = (int)(wav.Length / sps);
			samples = new NadSample[nadSamplesCount];

			ProgressShower.ShowProgress("Making nad");

			for (int s = 0; s < nadSamplesCount; s++)
			{
				samples[s] = new NadSample(channelsCount);

				float[] periods = new float[channelsCount];
				float[] amplitudes = new float[channelsCount];

				PeriodFinder.FP_DFT_MULTI_2(ref samples[s].periods, ref samples[s].amplitudes, wav, (int)(sps * s), (int)30000, (int)(30));
				ProgressShower.SetProgress(1.0 * s / nadSamplesCount);
			}

			ProgressShower.CloseProgressForm();
		}

		public Wav MakeWav(Wav wavOut)
		{
			ProgressShower.ShowProgress("Making wav from nad");

			float pi2 = MathF.PI * 2;

			float sint;

			float[] ampsOld = new float[channelsCount];
			float[] periodsOld = new float[channelsCount];
			float[] t = new float[channelsCount];

			for (int s = 0; s < wavOut.Length; s++)
			{
				sint = 0;

				float sampleRate = wavOut.sampleRate;
				float sps = wavOut.sampleRate / bps;
				int nadSampleNumber = Math.Min((int)(s / sps), samples.Length - 1); //////////

				for (int c = 0; c < channelsCount; c++)
				{
					ampsOld[c] = ampsOld[c] * 0.98f + samples[nadSampleNumber].amplitudes[c] * 0.02f;
					periodsOld[c] = periodsOld[c] * 0.98f + samples[nadSampleNumber].periods[c] * 0.02f;

					t[c] += 1f * pi2 / samples[nadSampleNumber].periods[c]; ////////
					sint += F(t[c]) * 0.99f * ampsOld[c];
				}

				wavOut.L[s] = sint / channelsCount;
				if (wavOut.channels == 2)
					wavOut.R[s] = sint / channelsCount;

				ProgressShower.SetProgress(1.0 * s / wavOut.Length);
			}

			ProgressShower.CloseProgressForm();

			return wavOut;
		}

		public MidiFile MakeMidi()
		{
			MidiFile midiFile = new MidiFile();

			var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(110));
			midiFile.ReplaceTempoMap(tempoMap);

			// Add first track chunk to the file

			var trackChunk1 = Build(tempoMap);
			midiFile.Chunks.Add(trackChunk1);

			// Add second track chunk to the file

			var trackChunk2 = Build(tempoMap);
			midiFile.Chunks.Add(trackChunk2);

            // Write MIDI data to file. See https://github.com/melanchall/drywetmidi/wiki/Writing-a-MIDI-file
            // to learn more

            return midiFile;
		}

        private static TrackChunk Build(TempoMap tempoMap)
        {
            // We can create a track chunk and put events in it via its constructor

            var trackChunk = new TrackChunk(
                new ProgramChangeEvent((SevenBitNumber)1)); // 'Acoustic Grand Piano' in GM

            // Insert notes via NotesManager class. See https://github.com/melanchall/drywetmidi/wiki/Notes
            // to learn more about managing notes

            using (var notesManager = trackChunk.ManageNotes())
            {
                var notes = notesManager.Objects;

                // Convert time span of 1 minute and 30 seconds to MIDI ticks. See
                // https://github.com/melanchall/drywetmidi/wiki/Time-and-length to learn more
                // about time and length representations and conversion between them

                var oneAndHalfMinute = TimeConverter.ConvertFrom(new MetricTimeSpan(0, 1, 30), tempoMap);

                // Insert two notes:
                // - A2 with length of 4/15 at 1 minute and 30 seconds from a file start
                // - B4 with length of 4 beats (1 beat = 1 quarter length at this case) at the start of a file

                notes.Add(new Note(noteName: mt.NoteName.A,
                                   octave: 2,
                                   length: LengthConverter.ConvertFrom(new MusicalTimeSpan(4, 15),
                                                                       time: oneAndHalfMinute,
                                                                       tempoMap: tempoMap),
                                   time: oneAndHalfMinute),
                          new Note(noteName: mt.NoteName.B,
                                   octave: 4,
                                   length: LengthConverter.ConvertFrom(new BarBeatTicksTimeSpan(0, 4),
                                                                       time: 0,
                                                                       tempoMap: tempoMap),
                                   time: 0));
            }

            // Insert chords via ChordsManager class. See https://github.com/melanchall/drywetmidi/wiki/Chords
            // to learn more about managing chords

            using (var chordsManager = trackChunk.ManageChords())
            {
                var chords = chordsManager.Objects;

                // Define notes for a chord:
                // - C2 with length of 30 seconds and 600 milliseconds
                // - C#3 with length of 300 milliseconds

                var notes = new[]
                {
            new Note(noteName: mt.NoteName.C,
                     octave: 2,
                     length: LengthConverter.ConvertFrom(new MetricTimeSpan(0, 0, 30, 600),
                                                         time: 0,
                                                         tempoMap: tempoMap)),
            new Note(noteName: mt.NoteName.CSharp,
                     octave: 3,
                     length: LengthConverter.ConvertFrom(new MetricTimeSpan(0, 0, 0, 300),
                                                         time: 0,
                                                         tempoMap: tempoMap))
        };

                // Insert the chord at different times:
                // - at the start of a file
                // - at 10 bars and 2 beats from a file start
                // - at 10 hours from a file start

                chords.Add(new Chord(notes,
                                     time: 0),
                           new Chord(notes,
                                     time: TimeConverter.ConvertFrom(new BarBeatTicksTimeSpan(10, 2),
                                                                     tempoMap)),
                           new Chord(notes,
                                     time: TimeConverter.ConvertFrom(new MetricTimeSpan(10, 0, 0),
                                                                     tempoMap)));
            }

            return trackChunk;
        }

        public static float F(float t)
		{
			return MathF.Sign(MathF.Sin(t));
		}
	}
}