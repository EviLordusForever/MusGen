using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen
{
	public class Wav
	{
		public float[] L;
		public float[] R;

		uint chunkID;
		uint fileSize;
		uint riffType;
		uint fmtID;
		uint fmtSize;

		uint fmtCode;
		public uint channels;
		public uint sampleRate;
		uint byteRate;
		ushort fmtBlockAlign;
		ushort bitDepth;

		int dataID;
		int bytes;

		byte[] byteArray;

		public bool Read(string path)
		{
			return Read(path, 0);
		}

		public bool Read(string path, int bytesLimit)
		{
			L = R = null;

			//try
			{
				using (FileStream fs = File.Open(path, FileMode.Open))
				{
					using (BinaryReader reader = new BinaryReader(fs))
					{

						// chunk 0
						chunkID = reader.ReadUInt32();
						fileSize = reader.ReadUInt32();
						riffType = reader.ReadUInt32();


						// chunk 1
						fmtID = reader.ReadUInt32();
						fmtSize = reader.ReadUInt32(); // bytes for this chunk (expect 16 or 18)

						// 16 bytes coming...
						fmtCode = reader.ReadUInt16();
						channels = reader.ReadUInt16();
						sampleRate = reader.ReadUInt32();
						byteRate = reader.ReadUInt32();
						fmtBlockAlign = reader.ReadUInt16();
						bitDepth = reader.ReadUInt16();

						if (fmtSize == 18)
						{
							// Read any extra values
							int fmtExtraSize = reader.ReadInt16();
							reader.ReadBytes(fmtExtraSize);
						}

						// chunk 2
						dataID = reader.ReadInt32();
						bytes = reader.ReadInt32();
						if (bytesLimit > 0)
							bytes = Math.Min(bytesLimit, bytes);

						// DATA!
						byteArray = reader.ReadBytes(bytes);
					}
				}

				int bytesForSamp = bitDepth / 8;
				int nValues = bytes / bytesForSamp;


				float[] asFloat = null;
				switch (bitDepth)
				{
					case 64:
						double[]
							asDouble = new double[nValues];
						Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
						asFloat = Array.ConvertAll(asDouble, e => (float)e);
						break;
					case 32:
						asFloat = new float[nValues];
						Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
						break;
					case 16:
						short[]
							asInt16 = new short[nValues];
						Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
						asFloat = Array.ConvertAll(asInt16, e => e / (float)(short.MaxValue + 1));
						break;
					default:
						return false;
				}

				switch (channels)
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
						ProgressShower.ShowProgress("Reading wav...");
						for (int s = 0, v = 0; s < nSamps; s++)
						{
							L[s] = asFloat[v++];
							R[s] = asFloat[v++];

							if (s % 10000 == 0) //
								ProgressShower.SetProgress(1.0 * s / nSamps);
						}
						ProgressShower.CloseProgressForm();
						return true;
					default:
						return false;
				}
			}
			//catch (ArgumentException ex)
			{
				//UserAsker.Ask("Похоже, слишком короткое аудио: " + x.Message);
				return false;
			}
			//catch (Exception ex)
			{
				//UserAsker.Ask(ex.Message);
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

		public void Export(string name)
		{
			SaveTo($"{Disk2._programFiles}\\Export\\{name}.wav");
		}

		public void SaveTo(string path)
		{
			//sampleRate;

			using (FileStream f = new FileStream(path, FileMode.Create))
			{
				f.Write(Math2.StringToByteArray("RIFF")); //RIFF
				f.Write(BitConverter.GetBytes((uint)(44 + L.Length * bitDepth * channels))); //?
				f.Write(Math2.StringToByteArray("WAVE"));
				f.Write(Math2.StringToByteArray("fmt "));
				f.Write(BitConverter.GetBytes(16)); //Subchunk 1 size = 16

				f.Write(BitConverter.GetBytes((ushort)1)); //type format = 1 PCM
				f.Write(BitConverter.GetBytes((ushort)channels)); //Channels
				f.Write(BitConverter.GetBytes(sampleRate)); //
				f.Write(BitConverter.GetBytes(sampleRate * bitDepth * channels / 8)); //CORRECT
				f.Write(BitConverter.GetBytes(fmtBlockAlign)); //Block align
				f.Write(BitConverter.GetBytes(bitDepth)); //Bits per sample
				f.Write(Math2.StringToByteArray("data"));
				f.Write(BitConverter.GetBytes(L.Length * bitDepth * channels));

				for (int i = 0; i < L.Length; i++)
				{
					WriteSample(L[i]);
					if (channels == 2)
						WriteSample(R[i]);

					void WriteSample(float v)
					{
						//try
						//{
						if (bitDepth == 16)
						{
							short a = Convert.ToInt16(Math.Floor(v * Math.Pow(2, bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
						if (bitDepth == 32)
						{
							int a = Convert.ToInt32(Math.Floor(v * Math.Pow(2, bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
						if (bitDepth == 64)
						{
							long a = Convert.ToInt64(Math.Floor(v * Math.Pow(2, bitDepth - 1)));
							f.Write(BitConverter.GetBytes(a));
						}
						//}
						//catch (System.OverflowException ex)
						//{ 
						//}					
					}
				}
			}
		}

		public static bool CheckWav(string path)
		{
			if (Text2.StringAfterLast(path, ".").ToLower() != "wav")
				return false;

			Wav localWav = new Wav();
			bool isReadable = localWav.Read(path, (int)Math.Pow(2, 20));
			if (!isReadable) return false;
			if (localWav.L.Length < 10) return false;
			return true;
		}
	}
}
