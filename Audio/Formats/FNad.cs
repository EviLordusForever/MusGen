using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

			for (int sample = 0; sample < list.Count; sample++)
			{
				if (list[sample]._amplitude > 0)
				{
					float noteOffTime = list[sample]._absoluteTime + list[sample]._duration;

					for (int subSample = sample + 1; subSample < list.Count; subSample++)
					{
						if (list[subSample]._absoluteTime > noteOffTime)
						{
							FNadSample noteOff = new FNadSample();
							noteOff._amplitude = 0;
							noteOff._absoluteTime = noteOffTime;
							noteOff._duration = 0;
							noteOff._index = list[sample]._index;

							list.Insert(subSample, noteOff);
							break;
						}
						else if (list[subSample]._index == list[sample]._index)
						{
							FNadSample noteOff = new FNadSample();
							noteOff._amplitude = 0;
							noteOff._absoluteTime = (list[subSample]._absoluteTime + list[subSample - 1]._absoluteTime) / 2f;
							noteOff._duration = 0;
							noteOff._index = list[sample]._index;

							list.Insert(subSample, noteOff);
							break;
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
			ToMidiAndExport(name, 1);
		}

		public void ToMidiAndExport(string name, float speed)
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
				noteOnEvent.DeltaTime = (long)(samplesWithNoteOffs[sample]._deltaTime * 2000f / speed);
				trackChunk.Events.Add(noteOnEvent);
			}

			midiFile.Chunks.Add(trackChunk);

			string path = $"{DiskE._programFiles}Export\\{name}.mid";
			midiFile.Write(path, true, MidiFileFormat.SingleTrack);
			DialogE.ShowFile(path);

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true
			};

			Process.Start(startInfo);

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

		public void UltraNormalize()
		{
			List<FNadSample> samples = new List<FNadSample>();
			samples.AddRange(_samples);
			float averageDeltaTime = FindAverageDeltaTime();
			List<FNadSample[]> accords = FillAccords();

			samples.Clear();
			float absoluteTime = 0;
			foreach (FNadSample[] accord in accords)
				for (int i = 0; i < accord.Length; i++)
				{
					if (i == 0)
						accord[i]._deltaTime = averageDeltaTime;
					else
						accord[i]._deltaTime = 0;

					absoluteTime += accord[i]._deltaTime;
					accord[i]._absoluteTime = absoluteTime;
					accord[i]._amplitude = 1;

					samples.Add(accord[i]);
				}

			_samples = samples.ToArray();

			List<FNadSample[]> FillAccords()
			{
				List<FNadSample[]> accords = new List<FNadSample[]>();

				while (samples.Count > 0)
					accords.Add(GetAccord());

				accords = Normalize(accords);

				return accords;
			}

			FNadSample[] GetAccord()
			{
				List<FNadSample> accord = new List<FNadSample>();
				accord.Add(samples[0]);
				samples.RemoveAt(0);

				while (samples.Count > 0)
				{
					if (samples[0]._deltaTime <= Params._accordMaxTime)
					{
						accord.Add(samples[0]);
						samples.RemoveAt(0);
					}
					else
						break;
				}

				return accord.ToArray();
			}

			List<FNadSample[]> Normalize(List<FNadSample[]> accords)
			{
				int lows = 0;
				int highs = 0;

				float averageDelta = 0;
				for (int i = 0; i < accords.Count; i++)
					averageDelta += accords[i][0]._deltaTime;

				averageDelta /= accords.Count;

				Logger.Log($"Average delta = {averageDelta}");

				for (int i = 1; i < accords.Count; i++)
					if (accords[i][0]._deltaTime <= averageDelta * 0.5f)
					{
						List<FNadSample> merged = new List<FNadSample>();
						merged.AddRange(accords[i - 1]);
						merged.AddRange(accords[i]);
						accords[i - 1] = merged.ToArray();
						accords.RemoveAt(i);
						lows++;
						i--;
					}

				for (int i = 1; i < accords.Count; i++)
					if (accords[i][0]._deltaTime > averageDelta * 1.5f)
					{
						FNadSample[] clone = new FNadSample[accords[i - 1].Length];
						for (int j = 0; j < clone.Length; j++)
							clone[j] = accords[i - 1][j].DeepClone();

						clone[0]._deltaTime = averageDelta;
						accords.Insert(i, clone);
						accords[i + 1][0]._deltaTime -= averageDelta;
						highs++;
					}

				Logger.Log($"REMOVED {lows} LOWS & {highs} HIGHS!", Brushes.Magenta);

				return accords;
			}
			

			float FindAverageDeltaTime()
			{
				float summ = 0;
				float count = 0;

				for (int i = 0; i < samples.Count; i++)
					if (samples[i]._deltaTime > Params._accordMaxTime)
					{
						summ += samples[i]._deltaTime;
						count++;
					}

				return summ / count;
			}
		}

		public FNad()
		{
		}
	}
}
