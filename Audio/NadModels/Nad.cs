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

				PeriodFinder.FP_DFT_MULTI(ref samples[s].periods, ref samples[s].amplitudes, wav, (int)(sps * s), 4000, 20, 15, "");
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
                for (int s = 0; s < samples.Length; s++)
                {
                    var chords = chordsManager.Objects;

                    Note[] notes = samples[s].GetMidiNotes((byte)s);

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