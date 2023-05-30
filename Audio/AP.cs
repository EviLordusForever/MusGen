using System;

namespace MusGen
{
	public static class AP
	{
		public static bool[] _octavesForReverse = new bool[] { true, true, true, true, true, true, true, true, false };

		private static uint _sampleRate = 44100;

		private static int _fftSize = 1024 * 1; //1024
		public const int _lc = 16; //how much fftLow bigger than fft //16
		public static short _channels = 20;
		public const ushort _sps = 120; //spectrums per second
		public static ushort _cs = 5; //how much spectrums per one specturm
		public static ushort _gg = 2; //how much

		public static int _smootherL = 8; //2 //8 //it is actually improves sound
		public static int _smootherH = 4;
		public static float _smootherFadeStart = 0.5f;
		public static float _smootherFadeEnd = 0.85f;

		public static int _iters = 4; //idk if it doing something
		public static float _speed = 1f;
		public static float _spectrumMiddleSmoother = 25f;

		public static float _nadMin = 0.0015f;

		public static float _lcFadeStart = 0.5f;

		public static float _peakWidth_ForFixedNad = 70; //70

		//Peaks

		//For peaks finding by gauss model
		public static float _peakWidth_ForMultiNad_Gauss = 80000f;
		public static float _lowestPeak_FromAverage_Gauss = 0.125f / 8; //0.125f / 32
		public static float _lowestPeak_FromMaximum_Gauss = 0.01f / 8; //0.01f / 32
		public static int _peaksLimit_Gauss = 150; //150 or less

		//For peaks finding by FFT Recognition model
		public static float _peakBig = 3f;
		public static float _peakWidth_ForMultiNad_FRM = 30000f;
		public static float _lowestPeak_FromAverage_FRM = 0.25f / 1;
		public static float _lowestPeak_FromMaximum_FRM = 0.02f / 1;
		public static int _peaksLimit_FRM = 300;



		// EQ

		public static int _nadModelSmoother = 12;
		public static int _ssModelSmoother = 12;

		public static float _omegaEqSmoothY = 0.2f; //from height
		public static float _omegaEqSmoothX = 1; //in seconds
		 
		//

		public static float _spectrumModelThreshold = 0.05f;

		public static int _wbmpResX = 256 * 2 * 2;
		public static int _wbmpResY = 16 * 9 * 2;

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
					SMM.Init();
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
					SMM.Init();
				}
			}
		}

		public static int SpectrumSizeGG
		{
			get
			{
				return SpectrumSize;
			}
		}

		public static int SpectrumSize
		{
			get
			{
				return _fftSize / 2 * AP._gg;
			}
		}

		public static int SpectrumSizeNoGG
		{
			get
			{
				return _fftSize / 2;
			}
		}
	}
}
