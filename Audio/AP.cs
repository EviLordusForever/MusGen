namespace MusGen
{
	public static class AP
	{
		public static uint _sampleRateNadToWav = 44100;
		public static uint _sampleRateRTFFTMicrophone = 44100;
		public static uint _sampleRateRTFFTSystem = 44100;
		private static uint _sampleRate = 44100;
		public static int _fftSize = 1024;
		public static int _lc = 16; //how much fftLow bigger than fft
		public static int _channels = 30;
		public static int _sps = 100;
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
	}
}
