using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace MusGen
{
	public static class AudioCapture
	{
		public static WaveInEvent waveIn;
		public static BufferedWaveProvider bufferedWaveProvider;
		public static List<float> samples = new List<float>();

		public static void Start(uint sampleRate, int bits, int channels)
		{
			waveIn = new WaveInEvent();
			waveIn.WaveFormat = new WaveFormat((int)sampleRate, bits, channels);
			waveIn.BufferMilliseconds = 1000 / 100;
			bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
			waveIn.DataAvailable += (sender, e) =>
			{
				try
				{
					bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
				}
				catch
				{
					bufferedWaveProvider.ClearBuffer();
				}
			};

			waveIn.StartRecording();
		}

		public static float[] GetSamples(int count)
		{
			byte[] buffer = new byte[bufferedWaveProvider.BufferedBytes];
			int byteCount = bufferedWaveProvider.Read(buffer, 0, buffer.Length);
			short[] newSamples = new short[byteCount / 2];
			Buffer.BlockCopy(buffer, 0, newSamples, 0, byteCount);

			for (int i = 0; i < newSamples.Length; i++)
				samples.Add((float)newSamples[i] / (float)short.MaxValue);

			float[] res = new float[count];

			int startIndex = samples.Count - count;

			if (startIndex >= 0)
				res = samples.GetRange(startIndex, count).ToArray();

			return res;
		}

		public static void Stop()
		{
			waveIn.StopRecording();
			waveIn.Dispose();
		}
	}
}
