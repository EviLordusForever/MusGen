using System;

namespace MusGen
{
	public static class AP
	{
		private static uint _sampleRate = 44100;

		private static int _fftSize = 1024 * 1; //1024
		public const int _lc = 16; //how much fftLow bigger than fft //16
		public static int _channels = 20;
		public const int _sps = 120;

		public static float _lcFadeStart = 0.5f;

		public static float _peakWidth_ForFixedNad = 70; //70

		//Peaks

		//For peaks finding by gauss model
		public static float _peakWidth_ForMultiNad_Gauss = 20000f;
		public static float _lowestPeak_FromAverage_Gauss = 0.125f / 8; //0.125f / 32
		public static float _lowestPeak_FromMaximum_Gauss = 0.01f / 8; //0.01f / 32
		public static int _peaksLimit_Gauss = 150; //150 or less

		//For peaks finding by FFT Recognition model
		public static float _peakBig = 3f;
		public static float _peakWidth_ForMultiNad_FRM = 30000f;
		public static float _lowestPeak_FromAverage_FRM = 0.25f / 1;
		public static float _lowestPeak_FromMaximum_FRM = 0.02f / 1;
		public static int _peaksLimit_FRM = 300;

		//

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
				if (_sampleRate != value)
				{
					_sampleRate = value;
					Logger.Log($"Sample rate was changed to {_sampleRate}");
					SpectrumFinder.Init();
					SpectrumDrawer.SetPianoImages();
					FftRecognitionModel.Init(_fftSize, (int)_sampleRate, _lc);
				}
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
				if (_fftSize != value)
				{
					_fftSize = value;
					Logger.Log($"FftSize was changed to {value}");
					SpectrumDrawer.Init();
					SpectrumFinder.Init();
					WindowFunction.Init();
					FftRecognitionModel.Init(_fftSize, (int)_sampleRate, _lc);
				}
			}
		}

		public static int SpectrumSize
		{
			get
			{
				return _fftSize / 2;
			}
		}
	}
}
