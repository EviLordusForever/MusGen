using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;
using System.IO.Compression;
/*using NAudio;
using NAudio.Midi;*/
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace MusGen
{
	[Serializable]
	public class Nad
	{
		public short _channelsCount;
		public NadSample[] _samples;
		public float _duration;
		public ushort _cs;
		public ushort _specturmSize;

		public Nad()
		{
		}

		public Nad(int samplesCount, float duration)
		{
			_channelsCount = -1;
			_samples = new NadSample[samplesCount];
			_duration = duration;
			_cs = AP._cs;
			_specturmSize = (ushort)AP.SpectrumSize;
		}

		public Nad(int samplesCount, float duration, ushort cs, ushort spectrumSize)
		{
			_channelsCount = -1;
			_samples = new NadSample[samplesCount];
			_duration = duration;
			_cs = cs;
			_specturmSize = spectrumSize;
		}

		public Nad(short channelsCount, int samplesCount, float duration, ushort cs, ushort spectrumSize)
		{
			_channelsCount = channelsCount;
			_samples = new NadSample[samplesCount];
			_duration = duration;
			_cs = cs;
			_specturmSize = spectrumSize;
		}

		public Nad Modify(int bps)
		{
			int oldSamplesCount = _samples.Length;
			int newSamplesCount = (int)(bps * _duration);
			Nad nad = new Nad(newSamplesCount, _duration);

			for (int sample = 0; sample < newSamplesCount; sample++)
			{
				nad._samples[sample] = new NadSample(0);

				int from = (int)(1f * sample / newSamplesCount * oldSamplesCount);
				int to = (int)(1f * (sample + 1) / newSamplesCount * oldSamplesCount);
				int count = to - from;

				for (int i = from; i < to; i++)
					nad._samples[sample].Add2(_samples[i]);

				nad._samples[sample].Divide(count);
			}

			Logger.Log("Nad successfully modified!");

			return nad;
		}

		public void Normalise(float max)
		{
			ProgressShower.Show("Nad normalising...");
			int step = (int)Math.Max(Width / 1000f, 1);

			for (int s = 0; s < Width; s++)
			{
				_samples[s].Normalise(max);

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / Width);
			}

			ProgressShower.Close();
		}

		public void Export(string outName)
		{
			ProgressShower.Show("Nad exporting...");
			ProgressShower.Set(0.5);

			BinaryFormatter formatter = new BinaryFormatter();
			string path = $"{DiskE._programFiles}Export\\Nads\\{outName}.nad";
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				using (var compressionStream = new GZipStream(stream, CompressionMode.Compress))
				{
					formatter.Serialize(compressionStream, this);
				}
			}
			DialogE.ShowFile(path);
			ProgressShower.Close();
		}

		public void ReadFromExport(string name)
		{
			Read($"{DiskE._programFiles}\\Export\\Nads\\{name}.nad");
		}

		public void Read(string path)
		{
			ProgressShower.Show("Nad reading...");
			ProgressShower.Set(0.5);

			BinaryFormatter formatter = new BinaryFormatter();
			Nad newObj;
			using (FileStream stream = new FileStream(path, FileMode.Open))
			{
				using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					newObj = (Nad)formatter.Deserialize(decompressionStream);
				}
			}

			_channelsCount = newObj._channelsCount;
			_samples = newObj._samples;
			_duration = newObj._duration;
			_cs = newObj._cs;
			_specturmSize = newObj._specturmSize;

			ProgressShower.Close();
			Logger.Log($"Some nad info: duration {MathF.Round(_duration, 2)}s, samples {_samples.Length}, cs {_cs}, size {SizeKb} KB", Brushes.Cyan);
		}

		private int Size
		{
			get
			{
				int size = 0;
				for (int i = 0; i < _samples.Length; i++)
					size += _samples[i].Height;
				return size;
			}
		}

		private double SizeKb
		{
			get
			{
				return Math.Round((Size * 6 + 2 + 2 + 4 + 2) / 1024.0, 2);
			}
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

			ushort[] _indexesCompared = new ushort[AP._channels];
			float[] _amplitudesCompared = new float[AP._channels];

			for (int id = 0; id < c; id++)
			{
				int idCompared = compares[id];

				_indexesCompared[id] = newNad._indexes[idCompared];
				_amplitudesCompared[id] = newNad._amplitudes[idCompared];
			}

			newNad._indexes = _indexesCompared;
			newNad._amplitudes = _amplitudesCompared;

			return newNad;
		}

        public void ToMidi(string name)
        {
			var midiFile = new MidiFile();
			var trackChunk = new TrackChunk();
			float durationInSeconds = _duration;

			short ticksPerQuarterNote = 480;

			midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(ticksPerQuarterNote);
			//new SmpteTimeDivision(Melanchall.DryWetMidi.Common.SmpteFormat.TwentyFour, 40);

			int durationInTicks = (int)(durationInSeconds * ticksPerQuarterNote * 4);


			float[] oldNotes = new float[128];

			int lastEventTime = 0;

			for (int sample = 0; sample < _samples.Length; sample++)
			{
				float[] newNotes = new float[128];
				int timeInTicks = (int)(durationInTicks / _samples.Length * sample / 2);

				for (int i = 0; i < _samples[sample].Height; i++)
				{
					ushort index = _samples[sample]._indexes[i];
					float amplitude = _samples[sample]._amplitudes[i];
					byte velocity = (byte)(amplitude * 128);
					float frequency = SpectrumFinder._frequenciesLg[index];
					byte noteNumber = (byte)(69 + 12 * MathF.Log2(frequency / 440));

					newNotes[noteNumber] = velocity;

					if (newNotes[noteNumber] > 0 && oldNotes[noteNumber] == 0)
					{
						var noteOnEvent = new NoteOnEvent();
						noteOnEvent.NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(noteNumber);
						noteOnEvent.Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(velocity);
						noteOnEvent.DeltaTime = timeInTicks - lastEventTime;
						lastEventTime = timeInTicks;
						trackChunk.Events.Add(noteOnEvent);
					}
					else if (newNotes[noteNumber] == 0 && oldNotes[noteNumber] > 0)
					{
						var noteOffEvent = new NoteOffEvent();
						noteOffEvent.NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(noteNumber);
						noteOffEvent.Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(velocity);
						noteOffEvent.DeltaTime = timeInTicks - lastEventTime;
						lastEventTime = timeInTicks;
						trackChunk.Events.Add(noteOffEvent);
					}
				}

				oldNotes = newNotes;
			}

			midiFile.Chunks.Add(trackChunk);
			string path = $"{DiskE._programFiles}Export\\{name}.mid";
			midiFile.Write(path, true, MidiFileFormat.SingleTrack);
		}

		public FNad ToFNad()
		{
			FNad fnad = new FNad();
			List<FNadSample> fnadSamples = new List<FNadSample>();

			short ticksPerQuarterNote = 480;

			int fullDurationInTicks = (int)(_duration * ticksPerQuarterNote * 4);

			float[,] map = new float[_samples.Length, SpectrumFinder._frequenciesLg.Length];

			FillMap();

			int lastEventTime = 0;

			for (int sample = 0; sample < _samples.Length; sample++)
			{
				int absoluteTimeInTicks = (int)(fullDurationInTicks * (1f * sample / _samples.Length) / 2);

				for (int index = 0; index < SpectrumFinder._frequenciesLg.Length; index++)
				{
					if (map[sample, index] > 0)
					{
						FNadSample fnadSample = new FNadSample();
						float index01 = 1f * index / SpectrumFinder._frequenciesLg.Length;
						float amplitude01 = map[sample, index];
						
						int deltaTimeInTicks = absoluteTimeInTicks - lastEventTime;
						lastEventTime = absoluteTimeInTicks;

						int samplesOfNote = 0;
						for (int subSample = sample; subSample < _samples.Length; subSample++)
						{
							if (map[subSample, index] > 0)
							{
								samplesOfNote++;
								map[subSample, index] = 0;
							}
							else
								break;
						}

						int noteDurationInTicks = (int)(fullDurationInTicks * (1f * samplesOfNote / _samples.Length));

						fnadSample._amplitude = amplitude01;
						fnadSample._index = index01;
						fnadSample._deltaTime = deltaTimeInTicks / 2000f;
						fnadSample._absoluteTime = absoluteTimeInTicks / 2000f;
						fnadSample._duration = noteDurationInTicks / 2000f;

						fnadSamples.Add(fnadSample);
					}
				}
			}

			fnad._samples = fnadSamples.ToArray();
			fnad._ticks = fullDurationInTicks;
			Logger.Log("Nad converted to FNad");
			return fnad;

			void FillMap()
			{
				for (int sample = 0; sample < _samples.Length; sample++)
				{
					for (int i = 0; i < _samples[sample].Height; i++)
					{
						ushort index = _samples[sample]._indexes[i];
						float amplitude = _samples[sample]._amplitudes[i];
						map[sample, index] = amplitude;
					}
				}
			}
		}
	}
}