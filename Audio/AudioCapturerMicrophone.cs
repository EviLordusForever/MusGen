using System;
using System.Collections.Generic;
using System.Collections;
using NAudio.Wave;

namespace MusGen
{
	public static class AudioCapturerMicrophone
	{
		public static WaveInEvent _waveInEvent;
		public static List<float> _samples = new List<float>();

		public static void Start(uint sampleRate, int bits, int channels)
		{
			_waveInEvent = new WaveInEvent();
			_waveInEvent.WaveFormat = new WaveFormat((int)sampleRate, bits, channels);
			_waveInEvent.BufferMilliseconds = 1000 / AP._nadSamplesPerSecond;
			_waveInEvent.DataAvailable += (sender, e) =>
			{
				for (int i = 0; i < e.Buffer.Length; i += 2)
					_samples.Add(BitConverter.ToInt16(e.Buffer, i));
			};

			_waveInEvent.StartRecording();

			Logger.Log($"Capturing from microphone was started.");
		}

		public static float[] GetSamples(int count)
		{
			float[] res = new float[count];

			int startIndex = _samples.Count - count;

			if (startIndex >= 0)
				res = _samples.GetRange(startIndex, count).ToArray();

			if (_samples.Count > 300000)
				_samples.RemoveRange(0, 100000);

			return res;
		}

		public static void Stop()
		{
			_waveInEvent.StopRecording();
			_waveInEvent.Dispose();
			Logger.Log($"Capturing from microphone was stopped.");
		}
	}
}
