using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class FftRecognitionModel
	{
		public static float[,] _model;
		private static float _max;
		private static float[] _maxesForColumns;
		private static int _size;
		//one column - one coefficient
		//one row - one equation
		//so spectrum is answers and it is vertical

		public static void Init(int fftSize, int sampleRate, int lc)
		{
			_size = fftSize / 2;
			_model = new float[_size, _size];
			_maxesForColumns = new float[_size];

			ProgressShower.Show("Initialising fft recognition model.");

			for (int fi = 0; fi < _size; fi++)
			{
				float frequency = SpectrumFinder._frequenciesLogarithmic[fi];
				float[] signal = new float[fftSize * lc];
				float[] signalLow = new float[fftSize * lc];

				for (int x = 0; x < fftSize * lc; x++)
				{
					float t = 1f * x / sampleRate; //time in seconds
					signal[x] = MathF.Sin(2f * MathF.PI * frequency * t);
					signalLow[x] = signal[x];
				}

				float cutOff = 0.5f * (sampleRate / lc);

				signalLow = KaiserFilter.Make(signalLow, sampleRate, cutOff, AP._kaiserFilterLength_ForProcessing, AP._kaiserFilterBeta, false);

				float[] spectrum = SpectrumFinder.Find(signal, signalLow);

				int column = fi;

				for (int row = 0; row < _size; row++)
				{
					_model[row, column] = spectrum[row];
					_maxesForColumns[column] = Math.Max(_maxesForColumns[column], spectrum[row]);
				}

				_max = Math.Max(_max, _maxesForColumns[column]);

				ProgressShower.Set(1.0 * fi / _size);
			}

			Normalize();
			ProgressShower.Close();
			Logger.Log("Fft recognition model was initialized.");
		}

		private static void Normalize()
		{
			for (int row = 0; row < _size; row++)
				for (int column = 0; column < _size; column++)
					_model[row, column] /= _maxesForColumns[column];
		}
	}
}
