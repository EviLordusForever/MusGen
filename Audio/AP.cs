﻿namespace MusGen
{
	public static class AP
	{
		public static uint _sampleRate = 44100;
		public static int _fftSize = 1024;
		public static int _lc = 16; //how much fftLow bigger than fft
		public static int _channels = 10;
		public static int _nadSamplesPerSecond = 100;
		public static float _peakSize = 70; //
		public static bool _logarithmicNad = true; //

		public static float _smoothXScale = 2000f;
		public static float _smoothYScale = 0.95f;

		public static int _wbmpResX = 256 * 2;
		public static int _wbmpResY = 16 * 9 * 2;

		public static float _fadeTime = 0.99f;
		public static string _waveForm = "sin";

		public static string _windowFunction = "hamming";

		public static int _graphType = 1;
	}
}