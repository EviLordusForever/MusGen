using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using NAudio.Midi;

namespace MusGen
{
	public class Midi
	{
		public MidiFile _midiFile;

        public void Read(string path)
        {
            _midiFile = new MidiFile(path);
            Logger.Log("Midi was read.");
        }

        public Nad ToNad()
        {
            int minimalDistance = 1000000;
            int previousEventTime = 0;

            List<NoteEvent> noteEvents = new List<NoteEvent>();
            FillNoteEvents();

            long ticks = noteEvents.Last().AbsoluteTime;
            float ticksPerSecond = TicksPerSecond();
            float seconds = ticks / ticksPerSecond;

            Nad nad = new Nad((int)ticks, seconds, AP._cs, (ushort)AP.SpectrumSize);

            int index = 0;

            float[] notes = new float[128];

            for (int tick = 0; tick < ticks; tick++)
            {
                again:
                if (noteEvents[index].AbsoluteTime == tick)
                {
                    int note = noteEvents[index].NoteNumber;
                    notes[note] = noteEvents[index].Velocity;
                    index++;

                    int distance = tick - previousEventTime;
                    if (distance != 0)
                        if (distance < minimalDistance)
                            minimalDistance = distance;

                    previousEventTime = tick;

                    goto again;
                }

                nad._samples[tick] = new NadSample(0);

                for (int note = 0; note < 128; note++)
                {                    
                    float amplitude = notes[note] / 128f;
                    if (amplitude > 0)
                    {
                        float frequency = 440 * MathF.Pow(2, (note - 69) / 12f);
                        ushort id = SpectrumFinder.IndexByFrequency(frequency);
                        nad._samples[tick].Add(id, amplitude);
                    }
                }
            }

            int bps = (int)(ticksPerSecond / minimalDistance);
            Logger.Log($"Minimal distance {minimalDistance}, bps {bps}");
            return nad; // nad.Modify(8);

            void FillNoteEvents()
            {
                var mergedTrackChunk = new List<MidiEvent>();

                foreach (var trackChunk in _midiFile.Events)
                {
                    int accumulatedTime = 0;
                    int[] instruments = new int[16];

                    foreach (var midiEvent in trackChunk)
                    {
                        accumulatedTime += midiEvent.DeltaTime;

                        if (midiEvent.CommandCode == MidiCommandCode.PatchChange)
                            if (midiEvent is PatchChangeEvent patchChangeEvent)
                            {
                                int channel = patchChangeEvent.Channel;
                                instruments[channel] = patchChangeEvent.Patch;
                            }

                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn || midiEvent.CommandCode == MidiCommandCode.NoteOff)
                        {
                            var noteEvent = midiEvent as NoteEvent;
                            if (noteEvent != null)
                            {
                                int channel = noteEvent.Channel;
                                if (instruments[channel] < 96)
                                {
                                    noteEvent.AbsoluteTime = accumulatedTime;
                                    mergedTrackChunk.Add(noteEvent);
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }

                // Сортировка событий в объединенном trackChunk по absoluteTime
                mergedTrackChunk.Sort((e1, e2) => e1.AbsoluteTime.CompareTo(e2.AbsoluteTime));

                foreach (var midiEvent in mergedTrackChunk)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        var noteEvent = midiEvent as NoteEvent;
                        if (noteEvent != null)
                        {
                            noteEvents.Add(noteEvent);
                        }
                    }

                    if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                    {
                        var noteEvent = midiEvent as NoteEvent;
                        if (noteEvent != null)
                        {
                            noteEvent.Velocity = 0;
                            noteEvents.Add(noteEvent);
                        }
                    }
                }
            }
        }

        public float TicksPerSecond()
        {
            List<int> ticksPerSecond = new List<int>();
            foreach (var trackChunk in _midiFile.Events)
            {
                foreach (MidiEvent midiEvent in trackChunk)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        var metaEvent = midiEvent as MetaEvent;
                        if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                        {
                            int microsecondsPerQuarterNote = (metaEvent as TempoEvent).MicrosecondsPerQuarterNote;
                            int a = (int)(1000000 * _midiFile.DeltaTicksPerQuarterNote / microsecondsPerQuarterNote);
                            ticksPerSecond.Add(a);
                            Logger.Log(a);
                        }
                    }
                }
            }

            float average = (float)ticksPerSecond.Average();
            Logger.Log($"Average {average}");
            return average;
        }
    }
}
