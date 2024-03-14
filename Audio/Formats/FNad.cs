using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace MusGen
{
	[Serializable]
	public class FNad
	{
		public FNadSample[] _samples;
		public int _ticks;

		public FNadSample[] AddNoteOffs()
		{
			List<FNadSample> list = new List<FNadSample>();
			list.AddRange(_samples);

			for (int sample = 0; sample < _samples.Length; sample++)
			{
				if (list[sample]._amplitude > 0)
				{
					for (int subSample = sample; subSample < _samples.Length; subSample++)
					{
						float noteOffTime = list[sample]._absoluteTime + list[sample]._duration;
						if (list[subSample]._absoluteTime > noteOffTime)
						{
							FNadSample noteOff = new FNadSample();
							noteOff._amplitude = 0;
							noteOff._absoluteTime = noteOffTime;
							noteOff._duration = 0;
							noteOff._index = list[sample]._index;
							
							list.Insert(subSample, noteOff);
						}
					}
				}
			}

			//List<FNadSample> sorted = list;// list.OrderBy(p => p._absoluteTime).ToList();

			list[0]._deltaTime = 0;
			for (int sample = 1; sample < _samples.Length; sample++)
				list[sample]._deltaTime = list[sample]._absoluteTime - list[sample - 1]._absoluteTime;
			
			return list.ToArray();

			Logger.Log("NoteOffs were added to FNad.");
		}

		public void ToMidiAndExport(string name)
		{
			FNadSample[] samplesWithNoteOffs = AddNoteOffs();

			var midiFile = new MidiFile();
			var trackChunk = new TrackChunk();

			short ticksPerQuarterNote = 480;

			midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(ticksPerQuarterNote);

			for (int sample = 0; sample < samplesWithNoteOffs.Length; sample++)
			{
				float index01 = samplesWithNoteOffs[sample]._index;
				int index = (int)(index01 * SpectrumFinder._frequenciesLg.Length);
				float amplitude01 = samplesWithNoteOffs[sample]._amplitude;
				byte velocity = (byte)(amplitude01 * 127);

				float frequency = SpectrumFinder._frequenciesLg[index];
				byte noteNumber = (byte)(69 + 12 * MathF.Log2(frequency / 440));

				if (noteNumber > 127)
					noteNumber = 127;

				var noteOnEvent = new NoteOnEvent();
				noteOnEvent.NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(noteNumber);
				noteOnEvent.Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(velocity);
				noteOnEvent.DeltaTime = (long)(samplesWithNoteOffs[sample]._deltaTime * 2000f);
				trackChunk.Events.Add(noteOnEvent);
			}

			midiFile.Chunks.Add(trackChunk);

			string path = $"{DiskE._programFiles}Export\\{name}.mid";
			midiFile.Write(path, true, MidiFileFormat.SingleTrack);
			DialogE.ShowFile(path);

			Logger.Log("FNad converted to midi. Midi exported.");
		}

		public void Export(string outName)
		{
			ProgressShower.Show("FNad exporting...");
			ProgressShower.Set(0.5);

			BinaryFormatter formatter = new BinaryFormatter();
			string path = $"{DiskE._programFiles}Export\\{outName}.fnad";
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				using (var compressionStream = new GZipStream(stream, CompressionMode.Compress))
				{
					formatter.Serialize(compressionStream, this);
				}
			}
			DialogE.ShowFile(path);
			Logger.Log("FNad was exported.");
			ProgressShower.Close();
		}

		public void ReadFromExport(string name)
		{
			Read($"{DiskE._programFiles}\\Export\\{name}.fnad");
		}

		public void Read(string path)
		{
			ProgressShower.Show("FNad reading...");
			ProgressShower.Set(0.5);

			BinaryFormatter formatter = new BinaryFormatter();
			FNad newObj;
			using (FileStream stream = new FileStream(path, FileMode.Open))
			{
				using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					newObj = (FNad)formatter.Deserialize(decompressionStream);
				}
			}

			_samples = newObj._samples;
			_ticks = newObj._ticks;

			ProgressShower.Close();
			Logger.Log("FNad was readed");
		}

		public FNad()
		{
		}
	}
}
