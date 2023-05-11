using System;

namespace MusGen
{
	public static class AP
	{
		private static uint _sampleRate = 44100;

		private static int _fftSize = 1024 * 1; //1024
		public static int _ifftScale = 2;
		public const int _lc = 16; //how much fftLow bigger than fft //16
		public static int _channels = 20;
		public const int _sps = 120;

		public static float _peakWidth_ForFixedNad = 70; //70

		public static float _peakWidth_ForMultiNad = 20000f;
		public static float _lowestPeak_FromAverage = 0.125f / 8; //0.125f / 32
		public static float _lowestPeak_FromMaximum = 0.01f / 8; //0.01f / 32
		public static int _peaksLimit = 150; //150 or less

		public static int _wbmpResX = 256 * 2;
		public static int _wbmpResY = 16 * 9 * 2 * 2;

		public static float _fadeTime = 0.99f;
		public static string _waveForm = "sin";

		public static string _captureType = "system";
		public static string _windowFunction = "hamming";

		public static int _kaiserFilterLength_ForProcessing = 100;
		public static int _kaiserFilterLength_ForRealtime = 50;
		public static float _kaiserFilterBeta = 5f;

		public static int _circularPianoImageDiameter = 500;

		public static float _adaptiveCeiling2Coefficient = 0.999f;

		public static float _adaptiveCeilingFallSpeed = 0.98f;
		public static float _adaptiveCeilingFallSpeedCircular = 0.96f;

		public static int _graphType = 1;
		
		public static uint SampleRate
		{
			get
			{
				return _sampleRate;
			}
			set
			{
				_sampleRate = value;
				Logger.Log($"Sample rate was set to {_sampleRate}");
				SpectrumFinder.Init();
				SpectrumDrawer.SetPianoImages();
			}
		}

		public static int FftSize
		{
			get
			{
				return _fftSize;
			}
			set
			{
				_fftSize = value;
				Logger.Log($"FftSize was set to {value}");
				SpectrumDrawer.Init();
				SpectrumFinder.Init();
				WindowFunction.Init();
			}
		}
	}
}
