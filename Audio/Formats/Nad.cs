using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Extensions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;
using System.IO.Compression;

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
			Logger.Log($"Some nad info: duration {MathF.Round(_duration, 2)}s, samples {_samples.Length}, cs {_cs}", Brushes.Cyan);
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