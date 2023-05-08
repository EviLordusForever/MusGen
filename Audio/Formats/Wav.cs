using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public class Wav
	{
		public float[] L;
		public float[] R;

		uint _chunkID;
		uint _fileSize;
		uint _riffType;
		uint _fmtID;
		uint _fmtSize;

		uint _fmtCode;
		public uint _channels;
		public int _sampleRate;
		uint _byteRate;
		ushort _fmtBlockAlign;
		ushort _bitDepth;

		int _dataID;
		int _bytes;

		public Wav(int length = 0, int channels = 1)
		{
			_fileSize = 0;
			_riffType = 0;
			_fmtID = 0;
			_fmtSize = 16;

			_fmtCode = 1;
			_channels = (uint)channels;
			_sampleRate = 44100;
			_byteRate = 176400;
			_fmtBlockAlign = 4;
			_bitDepth = 16;

			_dataID = 0;
			_bytes = 0;

			L = new float[length];
			if (_channels == 2)
				R = new float[length];
		}

		public bool Read(string path)
		{
			return Read(path, 0);
		}

		public bool Read(string path, int bytesLimit)
		{
			L = R = null;
			byte[] byteArray;

			using (FileStream fs = File.Open(path, FileMode.Open))
			{
				using (BinaryReader reader = new BinaryReader(fs))
				{
					// chunk 0
					_chunkID = reader.ReadUInt32();
					_fileSize = reader.ReadUInt32();
					_riffType = reader.ReadUInt32();

					// chunk 1
					_fmtID = reader.ReadUInt32();
					_fmtSize = reader.ReadUInt32(); // bytes for this chunk (expect 16 or 18)

					// 16 bytes coming...
					_fmtCode = reader.ReadUInt16();
					_channels = reader.ReadUInt16();
					_sampleRate = (int)reader.ReadUInt32();
					_byteRate = reader.ReadUInt32();
					_fmtBlockAlign = reader.ReadUInt16();
					_bitDepth = reader.ReadUInt16();

					if (_fmtSize == 18)
					{
						// Read any extra values
						int fmtExtraSize = reader.ReadInt16();
						reader.ReadBytes(fmtExtraSize);
					}

					// chunk 2
					_dataID = reader.ReadInt32();
					_bytes = reader.ReadInt32();
					if (bytesLimit > 0)
						_bytes = Math.Min(bytesLimit, _bytes);

					// DATA!
					byteArray = reader.ReadBytes(_bytes);
				}
			}

			int bytesForSamp = _bitDepth / 8;
			int nValues = _bytes / bytesForSamp;


			float[] asFloat = null;
			switch (_bitDepth)
			{
				case 64:
					double[]
						asDouble = new double[nValues];
					Buffer.BlockCopy(byteArray, 0, asDouble, 0, _bytes);
					asFloat = Array.ConvertAll(asDouble, e => (float)e);
					break;
				case 32:
					asFloat = new float[nValues];
					Buffer.BlockCopy(byteArray, 0, asFloat, 0, _bytes);
					break;
				case 16:
					short[]
						asInt16 = new short[nValues];
					Buffer.BlockCopy(byteArray, 0, asInt16, 0, _bytes);
					asFloat = Array.ConvertAll(asInt16, e => e / (float)(short.MaxValue + 1));
					break;
				default:
					return false;
			}

			switch (_channels)
			{
				case 1:
					L = asFloat;
					R = null;
					return true;
				case 2:
					// de-interleave
					int nSamps = nValues / 2;
					L = new float[nSamps];
					R = new float[nSamps];
					ProgressShower.Show("Reading wav...");
					for (int s = 0, v = 0; s < nSamps; s++)
					{
						L[s] = asFloat[v++];
						R[s] = asFloat[v++];

						if (s % 10000 == 0) //
							ProgressShower.Set(1.0 * s / nSamps);
					}
					ProgressShower.Close();
					return true;
				default:
					return false;
			}

			return false;
		}

		public int Length
		{
			get
			{
				return L.Length;
			}
		}

		public void Export(string name, bool show = true)
		{
			string path = $"{DiskE._programFiles}Export\\{name}.wav";
			Save(path);
			if (show)
				DialogE.ShowFile(path);
		}

		public void Save(string path)
		{
			using (FileStream f = new FileStream(path, FileMode.Create))
			{
				f.Write(MathE.StringToByteArray("RIFF")); //RIFF
				f.Write(BitConverter.GetBytes((uint)(44 + L.Length * _bitDepth * _channels / 8))); //Chunk size
				f.Write(MathE.StringToByteArray("WAVE"));
				f.Write(MathE.StringToByteArray("fmt "));
				f.Write(BitConverter.GetBytes(16)); //Subchunk 1 size = 16

				f.Write(BitConverter.GetBytes((ushort)1)); //type format = 1 PCM
				f.Write(BitConverter.GetBytes((ushort)_channels)); //Channels
				f.Write(BitConverter.GetBytes((uint)_sampleRate)); //
				f.Write(BitConverter.GetBytes((uint)(_sampleRate * _bitDepth * _channels / 8))); //CORRECT
				_fmtBlockAlign = (ushort)(_channels * _bitDepth / 8);
				f.Write(BitConverter.GetBytes(_fmtBlockAlign)); //Block align
				f.Write(BitConverter.GetBytes(_bitDepth)); //Bits per sample, 16
				f.Write(MathE.StringToByteArray("data"));
				f.Write(BitConverter.GetBytes(L.Length * _bitDepth * _channels / 8));

				ProgressShower.Show("Saving wav...");

				for (int s = 0; s < L.Length; s++)
				{
					WriteSample(L[s]);
					if (_channels == 2)
						WriteSample(R[s]);

					if (s % 5000 == 0) //
						ProgressShower.Set(1.0 * s / L.Length);
				}

				ProgressShower.Close();

				void WriteSample(float v)
				{
					v = MathF.Min(0.99f, v);
					v = MathF.Max(-0.99f, v);

					try
					{
						if (_bitDepth == 16)
						{
							short a = Convert.ToInt16(Math.Floor(v * Math.Pow(2, _bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
						if (_bitDepth == 32)
						{
							int a = Convert.ToInt32(Math.Floor(v * Math.Pow(2, _bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
						if (_bitDepth == 64)
						{
							long a = Convert.ToInt64(Math.Floor(v * Math.Pow(2, _bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
					}
					catch (System.OverflowException ex)
					{
					}
				}
			}
		}

		public static bool CheckWav(string path)
		{
			if (TextE.StringAfterLast(path, ".").ToLower() != "wav")
				return false;

			Wav localWav = new Wav();
			bool isReadable = localWav.Read(path, (int)Math.Pow(2, 20));
			if (!isReadable) return false;
			if (localWav.L.Length < 10) return false;
			return true;
		}
	}
}
