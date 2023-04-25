using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace MusGen
{
	public static class AudioCapturer
	{
		public static WaveInEvent _waveInEvent;
		public static BufferedWaveProvider _bufferedWaveProvider;
		public static List<float> _samples = new List<float>();

		public static void Start(uint sampleRate, int bits, int channels)
		{
			_waveInEvent = new WaveInEvent();
			_waveInEvent.WaveFormat = new WaveFormat((int)sampleRate, bits, channels);
			_waveInEvent.BufferMilliseconds = 1000 / 100;
			_bufferedWaveProvider = new BufferedWaveProvider(_waveInEvent.WaveFormat);
			_waveInEvent.DataAvailable += (sender, e) =>
			{
				try
				{
					_bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
				}
				catch
				{
					_bufferedWaveProvider.ClearBuffer();
				}
			};

			_waveInEvent.StartRecording();
		}

		public static float[] GetSamples(int count)
		{
			byte[] buffer = new byte[_bufferedWaveProvider.BufferedBytes];
			int byteCount = _bufferedWaveProvider.Read(buffer, 0, buffer.Length);
			short[] newSamples = new short[byteCount / 2];
			Buffer.BlockCopy(buffer, 0, newSamples, 0, byteCount);

			for (int i = 0; i < newSamples.Length; i++)
				_samples.Add((float)newSamples[i] / (float)short.MaxValue);

			float[] res = new float[count];

			int startIndex = _samples.Count - count;

			if (startIndex >= 0)
				res = _samples.GetRange(startIndex, count).ToArray();

			if (_samples.Count > 110000)
				_samples.RemoveRange(0, 100000);

			return res;
		}

		public static void Stop()
		{
			_waveInEvent.StopRecording();
			_waveInEvent.Dispose();
		}
	}
}
