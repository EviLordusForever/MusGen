namespace MusGen
{
	public static class AP
	{
		private static uint _sampleRate = 44100;

		private static int _fftSize = 1024 * 1; //1024
		public static int _lc = 16; //how much fftLow bigger than fft //16
		public static int _channels = 20;
		public static int _sps = (int)(_sampleRate / (_fftSize / 4));
		public static float _peakSize = 70; //

		public static int _wbmpResX = 256 * 2;
		public static int _wbmpResY = 16 * 9 * 2 * 2;

		public static float _fadeTime = 0.99f;
		public static string _waveForm = "sin";

		public static string _captureType = "system";
		public static string _windowFunction = "hamming";

		public static int _circularPianoImageDiameter = 500;

		public static float _adaptiveCeiling2Coefficient = 0.999f;

		public static float _adaptiveCeilingFallSpeed = 0.98f;
		public static float _adaptiveCeilingFallSpeedCircular = 0.96f;

		public static int _graphType = 1;

		public static float _phasesShift = 0.2f; //for SsToWav //0.2f good
		public static float _phasesShiftRandom = 0.3f; //0.3f good

		public static uint SampleRate
		{
			get
			{
				return _sampleRate;
			}
			set
			{
				_sampleRate = value;
				_sps = (int)(_sampleRate / (_fftSize / 4));
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
				_sps = (int)(_sampleRate / (_fftSize / 4));
				Logger.Log($"FftSize was set to {value}");
				SpectrumDrawer.Init();
				SpectrumFinder.Init();
				WindowFunction.Init();
			}
		}
	}
}
