using System;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace MusGen
{
	public static class AudioCapturerSystem
	{
		public static WasapiLoopbackCapture _capture;
		public static List<float> _samples = new List<float>();

		private static int _timesLow;
		private static int _timesFast;
		private static float _coefficient;
		private static int _newSamplesCount;
		private static int _fixIndex;

		public static void Start(uint sampleRate, int bits, int channels)
		{
			_capture = new WasapiLoopbackCapture();
			_capture.WaveFormat = new WaveFormat((int)sampleRate, bits, channels);			

			_capture.DataAvailable += (sender, e) =>
			{
				_timesLow++;

				if (_timesLow > 10)
				{
					_coefficient = (_coefficient + 1f * _timesLow / _timesFast) / 2;
					_timesLow = 0;
					_timesFast = 0;
				}

				lock (e.Buffer)
				{
					for (int i = 0; i < e.BytesRecorded; i += 2)
						_samples.Add(1f * BitConverter.ToInt16(e.Buffer, i) / Int16.MaxValue);

					_newSamplesCount = e.BytesRecorded / 2;
					_fixIndex = _newSamplesCount;
				}
			};

			_capture.StartRecording();
			Logger.Log($"Capturing from system was started.");
		}

		public static float[] GetSamples(int count)
		{
			float[] res = new float[count];

			int startIndex = _samples.Count - count - _fixIndex;

			_fixIndex -= (int)(_newSamplesCount * _coefficient);
			_fixIndex = _fixIndex > 0 ? _fixIndex : 0;

			lock (_samples)
			{
				if (startIndex >= 0)
					res = _samples.GetRange(startIndex, count).ToArray();

				if (_samples.Count > 300000)
					_samples.RemoveRange(0, 100000);
			}

			_timesFast++;

			return res;
		}

		public static void Stop()
		{
			_capture.StopRecording();
			_capture.Dispose();
			Logger.Log($"Capturing from system was stopped.");
		}
	}
}
